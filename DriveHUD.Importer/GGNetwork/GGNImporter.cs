//-----------------------------------------------------------------------
// <copyright file="GGNImporter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.FastParsing;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Models;

namespace DriveHUD.Importers.GGNetwork
{
    internal class GGNImporter : GenericImporter, IGGNImporter
    {
        private const string CertificateResourceName = "DriveHUD.Importers.GGNetwork.rootCert.pfx";

        private const string processName = "GGNet";

        private ProxyServer proxyServer;

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.GGN;
            }
        }

        protected override string ProcessName
        {
            get
            {
                return processName;
            }
        }

        protected override void DoImport()
        {
            try
            {
                // get settings
                var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

                if (settings != null)
                {
                    IsAdvancedLogEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;
                }

                // install certificate
                var certificate = InitializeCertificate();

                // create proxy
                proxyServer = new ProxyServer
                {
                    RootCertificate = certificate,
                    TrustRootCertificate = true,
                    ForwardToUpstreamGateway = true
                };

                // configure proxy
                var port = GetAvailablePort();

                var endPoint = new ExplicitProxyEndPoint(IPAddress.Any, port, true)
                {
                    IncludedHttpsHostNameRegex = GetHttpsHostNames()
                };

                if (IsAdvancedLogEnabled)
                {
                    proxyServer.ExceptionFunc = ex => LogProvider.Log.Error(this, $"Proxy server returned the error.", ex);
                }

                proxyServer.AddEndPoint(endPoint);
                proxyServer.BeforeRequest += OnProxyServerBeforeRequest;

                // start proxy
                proxyServer.Start();
                proxyServer.SetAsSystemProxy(endPoint, ProxyProtocolType.AllHttp);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"{SiteString} auto-import failed.", e);

                Clean();
                RaiseProcessStopped();
            }
        }

        public override void Stop()
        {
            base.Stop();

            Task.Run(() =>
            {
                Clean();
                RaiseProcessStopped();
            });
        }

        protected async Task OnProxyServerBeforeRequest(object arg1, SessionEventArgs arg2)
        {
            await Task.Run(() =>
            {
                if ((arg2.WebSession.Request.Host.Contains("session") || arg2.WebSession.Request.Host.Contains("cashgame")
                    || arg2.WebSession.Request.Host.Contains("tourney")) &&
                    arg2.WebSession.Request.Host.Contains("rhymetree.net"))
                {
                    arg2.DataReceived += OnProxyDataReceived;
                }
            });
        }

        /// <summary>
        /// Processes hand histories
        /// </summary>
        /// <param name="handHistories"></param>
        protected void ProcessHands(IEnumerable<HandHistory> handHistories)
        {
            var gameInfo = new GameInfo
            {
                PokerSite = EnumPokerSites.GGN,
                UpdateAction = UpdateGameInfo
            };

            foreach (var handHistory in handHistories)
            {
                var handHistoryText = SerializationHelper.SerializeObject(handHistory);
                ProcessHand(handHistoryText, gameInfo);
            }
        }

        /// <summary>
        /// Updates <see cref="GameInfo"/> when hand has been parsed already
        /// </summary>
        /// <param name="parsingResults">Parsing results</param>
        /// <param name="gameInfo">Game information</param>
        protected virtual void UpdateGameInfo(IEnumerable<ParsingResult> parsingResults, GameInfo gameInfo)
        {
            var parsingResult = parsingResults?.FirstOrDefault();

            if (parsingResult == null || parsingResult.Source == null || gameInfo == null)
            {
                return;
            }

            var window = FindWindow(parsingResult);

            if (window == IntPtr.Zero)
            {
                return;
            }

            var title = WinApi.GetWindowText(window);

            gameInfo.GameNumber = parsingResult.Source.HandId;
            gameInfo.Session = window.ToInt32().ToString();
            gameInfo.WindowHandle = window.ToInt32();
            gameInfo.TournamentSpeed = ParserUtils.ParseTournamentSpeed(title);
        }

        protected override bool InternalMatch(string title, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
              parsingResult.Source == null || parsingResult.Source.GameDescription == null || string.IsNullOrEmpty(parsingResult.Source.TableName))
            {
                return false;
            }

            return title.Contains(parsingResult.Source.TableName);
        }

        /// <summary>
        /// Processes proxy data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnProxyDataReceived(object sender, DataEventArgs e)
        {
            try
            {
                if (e.Count == 0)
                {
                    return;
                }

                var data = e.Buffer.Take(e.Count).ToArray();

                if (!GGNUtils.CheckData(data))
                {
                    return;
                }

                var startIndex = GGNUtils.GetStartIndex(data);

                if (startIndex == 0)
                {
                    return;
                }

                var jsonStr = GGNUtils.ExtractDataAsync(data).Result;

                if (string.IsNullOrEmpty(jsonStr))
                {
                    return;
                }

                var dataType = GGNUtils.GetDataType(jsonStr);

                switch (dataType)
                {
                    case GGNDataType.CashGameHandHistory:
                        {
                            var handHistoryInfo = GGNUtils.DeserializeHandHistory(jsonStr);

                            if (handHistoryInfo?.HandHistory == null)
                            {
                                return;
                            }

                            var handHistory = GGNConverter.ConvertCashHandHistory(handHistoryInfo);

                            ProcessHands(new[] { handHistory });

                            break;
                        }
                    case GGNDataType.CashGameHandHistories:
                        {
                            var handHistoriesInfo = GGNUtils.DeserializeHandHistories(jsonStr);

                            if (handHistoriesInfo?.Histories == null || handHistoriesInfo.Histories.Count <= 0)
                            {
                                return;
                            }

                            var handHistories = GGNConverter.ConvertCashHandHistories(handHistoriesInfo);

                            ProcessHands(handHistories);

                            break;
                        }
                    case GGNDataType.TourneyHandHistory:
                        {
                            var handHistoryInfo = GGNUtils.DeserializeHandHistory(jsonStr);

                            if (handHistoryInfo?.HandHistory == null)
                            {
                                return;
                            }

                            var handHistory = GGNConverter.ConvertTournamentHandHistory(handHistoryInfo);

                            break;
                        }
                    case GGNDataType.TourneyHandHistories:
                        {
                            var handHistoriesInfo = GGNUtils.DeserializeHandHistories(jsonStr);

                            if (handHistoriesInfo?.Histories == null || handHistoriesInfo.Histories.Count <= 0)
                            {
                                return;
                            }

                            var handHistories = GGNConverter.ConvertTournamentHandHistories(handHistoriesInfo);

                            break;
                        }
                    case GGNDataType.TourneyInfo:
                        {
                            break;
                        }
                    case GGNDataType.Unknown:
                        break;
                }

            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"{SiteString} data couldn't be parsed", ex);
            }
        }

        /// <summary>
        /// Initializes certificate for proxy server to process https traffic
        /// </summary>
        protected X509Certificate2 InitializeCertificate()
        {
            try
            {
                var resourcesAssembly = GetType().Assembly;

                byte[] certificateBytes;

                using (var resourceStream = resourcesAssembly.GetManifestResourceStream(CertificateResourceName))
                {
                    using (var ms = new MemoryStream())
                    {
                        resourceStream.CopyTo(ms);
                        certificateBytes = ms.ToArray();
                    }
                }

                var certificate = new X509Certificate2(certificateBytes, string.Empty, X509KeyStorageFlags.Exportable);

                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

                store.Open(OpenFlags.ReadWrite);

                if (!store.Certificates.Contains(certificate))
                {
                    store.Add(certificate);
                    LogProvider.Log.Info(this, "Certificate for proxy server has been added to store.");
                }

                store.Close();

                return certificate;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Certificate has not been installed"), e);
            }
        }

        /// <summary>
        /// Gets available port for proxy server
        /// </summary>
        /// <returns>Port</returns>
        protected int GetAvailablePort()
        {
            return 0;
        }

        /// <summary>
        /// Gets the list of ggn host names
        /// </summary>        
        protected IEnumerable<string> GetHttpsHostNames()
        {
            return new[] { "rhymetree.net" };
        }

        /// <summary>
        /// Cleans up resources
        /// </summary>
        protected void Clean()
        {
            if (proxyServer != null)
            {
                proxyServer.BeforeRequest -= OnProxyServerBeforeRequest;

                if (proxyServer.ProxyRunning)
                {
                    proxyServer.Stop();
                }
            }
        }
    }
}