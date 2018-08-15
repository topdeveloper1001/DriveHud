//-----------------------------------------------------------------------
// <copyright file="PokerStarsZoomImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Model.Site;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DriveHUD.Importers.PokerStars
{
    internal class PokerStarsZoomImporter : BaseImporter, IPokerStarsZoomImporter
    {
        private const string ProcessName = "PokerStars";

        private const string WindowClassName = "PokerStarsTableFrameClass";

        private const string AuditFileFilter = "*.dat";

        private const int ReadingTimeout = 500;

        private Process pokerClientProcess;

        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.PokerStars; }
        }

        protected ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.PokerStarsZoom;
            }
        }

        protected override void DoImport()
        {
            var auditPath = PokerStarsConfiguration.GetAuditPath();
            var dataManager = ServiceLocator.Current.GetInstance<IPokerStarsZoomDataManager>();

            DirectoryInfo auditDirectory = null;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (pokerClientProcess == null || pokerClientProcess.HasExited)
                    {
                        if (pokerClientProcess != null && pokerClientProcess.HasExited)
                        {
                            LogProvider.Log.Info(this, $"Process \"{ProcessName}\" has exited. [{Identifier}]");
                        }

                        pokerClientProcess = GetPokerClientProcess();

                        if (pokerClientProcess == null)
                        {
                            try
                            {
                                Task.Delay(ReadingTimeout).Wait(cancellationTokenSource.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }

                            continue;
                        }
                    }

                    if (auditDirectory == null)
                    {
                        auditDirectory = new DirectoryInfo(auditPath);
                    }

                    var auditFiles = auditDirectory.GetFiles(AuditFileFilter);

                    foreach (var auditFile in auditFiles)
                    {
                        var dataObject = ParseAuditFile(auditFile);
                        dataManager.ProcessData(dataObject);
                    }

                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            Task.Delay(ReadingTimeout).Wait(cancellationTokenSource.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Failed to process audit files [{Identifier}]", e);
                }
            }

            RaiseProcessStopped();
        }

        private PokerStarsZoomDataObject ParseAuditFile(FileInfo auditFile)
        {
            try
            {
                var auditFileContent = File.ReadAllText(auditFile.FullName);

                if (string.IsNullOrEmpty(auditFileContent))
                {
                    return null;
                }

                var auditData = auditFileContent
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x =>
                    {
                        var keyValue = x.Trim().Split('=');
                        var keyValuePair = new KeyValuePair<string, string>(keyValue[0].ToLowerInvariant(), keyValue.Length < 1 ? string.Empty : keyValue[1]);
                        return keyValuePair;
                    })
                    .ToDictionary(x => x.Key, x => x.Value);

                var dataObject = new PokerStarsZoomDataObject();

                // Parse table name
                if (auditData.TryGetValue("tablenumber", out string tableName))
                {
                    dataObject.TableName = tableName;

                    var tableHandle = FindWindow(tableName, out string title);
                    dataObject.Handle = tableHandle.ToInt32();
                    dataObject.Title = title;
                }

                // Parse game type
                if (auditData.TryGetValue("game", out string game))
                {
                    if (game.ContainsIgnoreCase("HOLDEM"))
                    {
                        dataObject.GameType = Bovada.GameType.Holdem;
                    }
                    else if (game.ContainsIgnoreCase("OMAHA"))
                    {
                        if (auditData.TryGetValue("hi-lo", out string hiLo) && bool.TryParse(hiLo, out bool isHiLo))
                        {
                            dataObject.GameType = isHiLo ? Bovada.GameType.OmahaHiLo : Bovada.GameType.Omaha;
                        }
                        else
                        {
                            dataObject.GameType = Bovada.GameType.Omaha;
                        }
                    }
                }

                // Parse game format
                if (auditData.TryGetValue("zoom", out string zoom) && bool.TryParse(zoom, out bool isZoom))
                {
                    if (auditData.TryGetValue("currency", out string currency) &&
                        currency.Equals("TOURNAMENT", StringComparison.OrdinalIgnoreCase))
                    {
                        dataObject.GameFormat = GameFormat.MTT;
                    }
                    else
                    {
                        dataObject.GameFormat = isZoom ? GameFormat.Zoom : GameFormat.Cash;
                    }
                }

                // Parse maxseats 
                if (auditData.TryGetValue("maxseats", out string maxseats) && byte.TryParse(maxseats, out byte maxPlayers))
                {
                    dataObject.MaxPlayers = maxPlayers;
                    dataObject.TableType = (EnumTableType)maxPlayers;
                }

                // Parse hero
                if (auditData.TryGetValue("user", out string heroName))
                {
                    dataObject.HeroName = HttpUtility.UrlDecode(heroName);
                }

                var players = new List<PlayerDataObject>();

                // Parse players
                for (byte i = 1; i <= dataObject.MaxPlayers; i++)
                {
                    if (auditData.TryGetValue($"seat{i}", out string playerName))
                    {
                        players.Add(new PlayerDataObject
                        {
                            Player = HttpUtility.UrlDecode(playerName),
                            Seat = i
                        });
                    }
                }

                if (players.Count != 0)
                {
                    dataObject.Players = players.ToArray();
                }

                if (dataObject.IsValid)
                {
                    return dataObject;
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Could not parse audit file at {auditFile.FullName} [{Identifier}]", ex);
            }

            return null;
        }

        private IntPtr FindWindow(string tableName, out string tableTitle)
        {
            var handle = IntPtr.Zero;
            tableTitle = string.Empty;

            try
            {
                if (pokerClientProcess == null || pokerClientProcess.HasExited)
                {
                    return handle;
                }

                var title = string.Empty;

                foreach (ProcessThread thread in pokerClientProcess.Threads)
                {
                    WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                    {
                        title = WinApi.GetWindowText(hWnd);
                        var windowClassName = WinApi.GetClassName(hWnd);

                        if (!string.IsNullOrEmpty(title) &&
                            windowClassName.Equals(WindowClassName, StringComparison.OrdinalIgnoreCase) &&
                            title.Replace(" Table ", " ").ContainsIgnoreCase(tableName) && !title.ContainsIgnoreCase(" Lobby"))
                        {
                            handle = hWnd;
                            return false;
                        }

                        return true;
                    }, IntPtr.Zero);
                }

                if (handle != IntPtr.Zero)
                {
                    tableTitle = title;
                }

                return handle;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Failed to find window of table '{tableName}'", ex);
            }

            return handle;
        }

        private Process GetPokerClientProcess()
        {
            try
            {
                var processes = Process.GetProcesses();
                var pokerClientProcess = processes.FirstOrDefault(x => x.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase));
                return pokerClientProcess;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to find process '{ProcessName}' [{Identifier}]", e);
                return null;
            }
        }

        protected override bool IsDisabled()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == Site);

            return siteSettings != null && (!siteSettings.Enabled || !siteSettings.FastPokerEnabled);
        }
    }
}