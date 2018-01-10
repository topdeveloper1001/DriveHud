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

using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.FastParsing;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DriveHUD.Importers.ExternalImporter
{
    internal class ExternalImporter : GenericImporter, IExternalImporter
    {
        private static readonly object locker = new object();

        private const string processName = "GGNet";

        private ServiceHost serviceHost;

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

        public override string SiteString
        {
            get
            {
                return "GenericExternalImporter";
            }
        }

        protected override void DoImport()
        {
            lock (locker)
            {
                try
                {
                    var baseAddress = new Uri(StringFormatter.GetImporterPipeAddress());
                    var baseAddresses = new Uri[] { baseAddress };

                    var service = new DHImporterService(this);

                    if (serviceHost != null)
                    {
                        serviceHost.Faulted -= OnServiceHostFaulted;
                    }

                    serviceHost = new ServiceHost(service, baseAddresses);
                    serviceHost.AddServiceEndpoint(typeof(IDHImporterService), new NetNamedPipeBinding(), nameof(IDHImporterService.ImportHandHistory));
                    serviceHost.Faulted += OnServiceHostFaulted;

                    serviceHost.Open();
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"{SiteString} auto-import failed.", e);

                    Clean();
                    RaiseProcessStopped();
                }
            }
        }

        private void OnServiceHostFaulted(object sender, EventArgs e)
        {
            LogProvider.Log.Error(this, $"Service host faulted [{SiteString}]");
        }

        public override void Stop()
        {
            base.Stop();

            Task.Run(() =>
            {
                lock (locker)
                {
                    Clean();
                    RaiseProcessStopped();
                }
            });
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

            if (parsingResult.Source.GameDescription.IsTournament)
            {
                return ExternalImporterUtils.IsTournamentTableMatch(title, parsingResult.Source.TableName);
            }

            return ExternalImporterUtils.IsCashTableMatch(title, parsingResult.Source.TableName);
        }

        /// <summary>
        /// Cleans up resources
        /// </summary>
        protected void Clean()
        {
            if (serviceHost == null)
            {
                return;
            }

            try
            {
                serviceHost.Faulted -= OnServiceHostFaulted;
                serviceHost.Close();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could close host service [{SiteString}]", e);
            }
        }

        protected override bool IsDisabled()
        {
            return false;
        }

        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
        private class DHImporterService : IDHImporterService
        {
            private ExternalImporter importer;

            public DHImporterService(ExternalImporter importer)
            {
                if (importer == null)
                {
                    throw new ArgumentNullException(nameof(importer));
                }

                this.importer = importer;
            }

            public void ImportHandHistory(string handHistory)
            {
                if (string.IsNullOrEmpty(handHistory))
                {
                    return;
                }

                try
                {
                    var gameInfo = new GameInfo
                    {
                        PokerSite = EnumPokerSites.GGN,
                        UpdateAction = importer.UpdateGameInfo
                    };

                    importer.ProcessHand(handHistory, gameInfo);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Hand has not been processed [{importer.SiteString}]", e);
                }
            }
        }
    }
}