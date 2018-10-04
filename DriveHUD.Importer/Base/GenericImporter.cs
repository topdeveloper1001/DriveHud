//-----------------------------------------------------------------------
// <copyright file="GenericImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DriveHUD.Importers
{
    internal abstract class GenericImporter : BaseImporter
    {
        protected IEventAggregator eventAggregator;

        public GenericImporter()
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        #region Properties

        protected abstract string[] ProcessNames { get; }

        protected virtual bool IsAdvancedLogEnabled { get; set; }

        protected virtual bool SupportDuplicates => false;

        #endregion

        #region Infrastructure

        /// <summary>
        /// Processes the specified hand history text
        /// </summary>
        /// <param name="handHistory">Hand history to process</param>
        /// <param name="gameInfo"></param>
        protected virtual void ProcessHand(string handHistory, GameInfo gameInfo)
        {
            var importer = ServiceLocator.Current.GetInstance<IFileImporter>();
            var progress = new DHProgress();

            IEnumerable<ParsingResult> parsingResult = null;

            if (gameInfo.GameNumber != 0)
            {
                LogProvider.Log.Info(this, $"Hand {gameInfo.GameNumber} processed. [{SiteString}]");
            }

            try
            {
                parsingResult = ImportHand(handHistory, gameInfo, importer, progress);
            }
            catch (DHLicenseNotSupportedException)
            {
                LogProvider.Log.Error(this, $"Hand(s) has not been imported. License issue. [{SiteString}]");

                var windowHwnd = new IntPtr(gameInfo.WindowHandle);

                if (windowHwnd != IntPtr.Zero && WinApi.IsWindow(windowHwnd))
                {
                    SendPreImporedData("Notifications_HudLayout_PreLoadingText_NoLicense", windowHwnd);
                }
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(gameInfo.FullFileName))
                {
                    LogProvider.Log.Error(this, $"Hand(s) from '{gameInfo.FullFileName}' has not been imported. [{SiteString}]", e);
                }
                else
                {
                    LogProvider.Log.Error(this, $"Hand(s) has not been imported. [{SiteString}]", e);
                }
            }

            if (parsingResult == null)
            {
                return;
            }

            foreach (var result in parsingResult)
            {
                if (result.HandHistory == null)
                {
                    continue;
                }

                if (!SupportDuplicates && result.IsDuplicate)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. Duplicate. [{1}]", result.HandHistory.Gamenumber, SiteString));
                    continue;
                }

                if (!result.WasImported)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. [{1}]", result.HandHistory.Gamenumber, SiteString));
                    continue;
                }

                LogProvider.Log.Info(this, string.Format("Hand {0} has been imported in {2}ms. [{1}]", result.HandHistory.Gamenumber, SiteString, result.Duration));

                if (gameInfo.WindowHandle == 0 || !WinApi.IsWindow(new IntPtr(gameInfo.WindowHandle)))
                {
                    gameInfo.WindowHandle = FindWindow(result).ToInt32();
                }

                gameInfo.GameFormat = ParseGameFormat(result);
                gameInfo.GameType = ParseGameType(result);
                gameInfo.TableType = ParseTableType(result, gameInfo);
                gameInfo.GameNumber = result.HandHistory.Gamenumber;

                var playerList = GetPlayerList(result.Source, gameInfo);

                var dataImportedArgs = new DataImportedEventArgs(playerList, gameInfo, result.Source?.Hero, result.HandHistory.Gamenumber);

                PublishImportedResults(dataImportedArgs);
            }
        }

        protected virtual IEnumerable<ParsingResult> ImportHand(string handHistory, GameInfo gameInfo, IFileImporter importer, DHProgress progress)
        {
            return importer.Import(handHistory, progress, gameInfo);
        }

        protected virtual void SendPreImporedData(string loadingTextKey, IntPtr windowHandle)
        {
            var gameInfo = new GameInfo
            {
                PokerSite = Site,
                TableType = EnumTableType.Nine,
                WindowHandle = windowHandle.ToInt32()
            };

            var loadingText = CommonResourceManager.Instance.GetResourceString(loadingTextKey);

            var eventArgs = new PreImportedDataEventArgs(gameInfo, loadingText);
            eventAggregator.GetEvent<PreImportedDataEvent>().Publish(eventArgs);
        }

        protected virtual IntPtr FindWindow(ParsingResult parsingResult)
        {
            try
            {
                var pokerClientProcesses = GetPokerClientProcesses(ProcessNames);

                if (IsAdvancedLogEnabled)
                {
                    var processesNames = string.Join(", ", pokerClientProcesses.Select(x => x.ProcessName).ToArray());
                    LogProvider.Log.Info($"Possible client processes: {processesNames} [{SiteString}]");
                }

                var handles = new List<IntPtr>();

                foreach (var pokerClientProcess in pokerClientProcesses)
                {
                    foreach (ProcessThread thread in pokerClientProcess.Threads)
                    {
                        WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                        {
                            handles.Add(hWnd);
                            return true;
                        }, IntPtr.Zero);
                    }
                }

                foreach (var handle in handles)
                {
                    var title = WinApi.GetWindowText(handle);

                    if (!string.IsNullOrEmpty(title))
                    {
                        if (Match(title, handle, parsingResult))
                        {
                            return handle;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not find table '{parsingResult?.Source?.TableName}'. [{SiteString}]", e);
            }

            return IntPtr.Zero;
        }

        protected virtual bool Match(string title, IntPtr handle, ParsingResult parsingResult)
        {
            var matchResult = InternalMatch(title, handle, parsingResult);

            if (IsAdvancedLogEnabled)
            {
                var tableName = parsingResult?.Source?.TableName;
                LogProvider.Log.Info($"Checking if window '{title}' [{handle}] matches '{tableName}' table: {matchResult} [{SiteString}]");
            }

            return matchResult;
        }

        protected abstract bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult);

        protected virtual GameFormat ParseGameFormat(ParsingResult parsingResult)
        {
            if (parsingResult.Source != null && parsingResult.Source.GameDescription != null)
            {
                if (parsingResult.Source.GameDescription.IsTournament)
                {
                    return parsingResult.TournamentsTags == TournamentsTags.MTT ? GameFormat.MTT : GameFormat.SnG;
                }

                if (Site == EnumPokerSites.PokerStars && parsingResult.Source.GameDescription.TableType.Contains(TableTypeDescription.FastFold))
                {
                    return GameFormat.Zoom;
                }
            }

            return GameFormat.Cash;
        }

        protected virtual Bovada.GameType ParseGameType(ParsingResult parsingResult)
        {
            if (parsingResult.Source == null || parsingResult.Source.GameDescription == null)
            {
                return Bovada.GameType.Holdem;
            }

            switch (parsingResult.Source.GameDescription.GameType)
            {
                case GameType.CapPotLimitOmaha:
                case GameType.FiveCardPotLimitOmaha:
                case GameType.FixedLimitOmaha:
                case GameType.NoLimitOmaha:
                case GameType.PotLimitOmaha:
                    return Bovada.GameType.Omaha;
                case GameType.FiveCardPotLimitOmahaHiLo:
                case GameType.PotLimitOmahaHiLo:
                case GameType.NoLimitOmahaHiLo:
                case GameType.FixedLimitOmahaHiLo:
                    return Bovada.GameType.OmahaHiLo;
                default:
                    return Bovada.GameType.Holdem;
            }
        }

        protected virtual EnumTableType ParseTableType(ParsingResult parsingResult, GameInfo gameInfo)
        {
            if (parsingResult.Source == null || parsingResult.Source.GameDescription == null)
            {
                return EnumTableType.Nine;
            }

            var tableType = (EnumTableType)parsingResult.Source.GameDescription.SeatType.MaxPlayers;
            return tableType;
        }

        protected virtual PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            return handHistory.Players;
        }

        protected virtual void PublishImportedResults(DataImportedEventArgs args)
        {
            eventAggregator.GetEvent<DataImportedEvent>().Publish(args);
        }

        /// <summary>
        /// Get client process
        /// </summary>
        /// <returns>Client process if exist, otherwise - null</returns>
        protected virtual Process[] GetPokerClientProcesses(string[] processNames)
        {
            return Utils.GetProcessesByNames(processNames);
        }

        #endregion
    }
}