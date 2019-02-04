//-----------------------------------------------------------------------
// <copyright file="Adda52TableService.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers.Adda52
{
    internal class Adda52TableService : BaseImporter, IAdda52TableService
    {
        private const string ProcessName = "poker";
        private const string CrazyPineappleTableName = "Crazy Pineapple";
        private const int ScanInterval = 2500;
        private Process pokerClientProcess;
        private readonly ReaderWriterLockSlim lockObject;
        private readonly IEventAggregator eventAggregator;
        private readonly ISettingsService settingService;
        protected Dictionary<IntPtr, string> openedTables;

        public Adda52TableService()
        {
            lockObject = new ReaderWriterLockSlim();
            openedTables = new Dictionary<IntPtr, string>();
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            settingService = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        #region Properties

        protected override EnumPokerSites Site => EnumPokerSites.Adda52;

        protected override string ImporterName => "table service";

        protected bool IsAdvancedLogging { get; set; }

        protected bool? IsAdda52Launched { get; set; }

        #endregion

        #region IAdda52TableService implementation

        public IntPtr[] GetAllWindows()
        {
            lockObject.EnterReadLock();

            try
            {
                return openedTables.Keys.ToArray();
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        public IntPtr GetWindow(HandHistory handHistory)
        {
            if (handHistory == null || handHistory.GameDescription == null)
            {
                return IntPtr.Zero;
            }

            lockObject.EnterReadLock();

            try
            {
                var handle = handHistory.GameDescription.IsTournament ?
                    FindTournamentTableHandle(handHistory) :
                    FindCashTableHandle(handHistory);

                return handle;
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        private IntPtr FindCashTableHandle(HandHistory handHistory)
        {
            if (handHistory.GameDescription.Limit == null ||
                string.IsNullOrEmpty(handHistory.TableName) || handHistory.TableName.Length < 2)
            {
                LogProvider.Log.Warn(this, $"Failed to find cash table because of null limit or empty table name. [{SiteString}]");
                return IntPtr.Zero;
            }

            var isFreeroll = handHistory.GameDescription.Limit.Currency == Currency.PlayMoney;

            var cashTableKey = CashTableKey.From(handHistory.GameDescription.Limit.BigBlind,
                handHistory.GameDescription.CashBuyInHigh,
                handHistory.GameDescription.TableType.FirstOrDefault(),
                handHistory.GameDescription.GameType, handHistory.GameDescription.SeatType.MaxPlayers);

            var tableName = string.Empty;

            if (isFreeroll)
            {
                if (!freerollTables.TryGetValue(cashTableKey, out tableName))
                {
                    LogProvider.Log.Warn(this, $"Failed to find predefined freeroll table for BB={cashTableKey.BigBlind}, GT={cashTableKey.GameType}, S={cashTableKey.TableTypeDescription}. [{SiteString}]");
                }
            }
            else
            {
                if (!ringTables.TryGetValue(cashTableKey, out tableName))
                {
                    LogProvider.Log.Warn(this, $"Failed to find predefined ring table for BB={cashTableKey.BigBlind}, GT={cashTableKey.GameType}, S={cashTableKey.TableTypeDescription}. [{SiteString}]");
                }
            }

            if (string.IsNullOrEmpty(tableName))
            {
                return IntPtr.Zero;
            }

            var tableNumber = handHistory.TableName.Substring(handHistory.TableName.Length - 2);

            var possibleTableName = $"{tableName} - {tableNumber}";

            var openedTableKeyValue = openedTables.FirstOrDefault(x => x.Value.ContainsIgnoreCase(possibleTableName));

            return openedTableKeyValue.Key;
        }

        #region Cash tables predefined data

        private static readonly ReadOnlyDictionary<CashTableKey, string> freerollTables = new ReadOnlyDictionary<CashTableKey, string>(
            new Dictionary<CashTableKey, string>
            {
                [CashTableKey.From(20, 1000, TableTypeDescription.Regular, GameType.PotLimitOmahaHiLo, 6)] = "PLO Omaha Freeroll Hi/Lo",
                [CashTableKey.From(50, 3000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Freeroll Trips",
                [CashTableKey.From(100, 5000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Freeroll Quads",
                [CashTableKey.From(100, 5000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 9)] = "Trips Turbo",
                [CashTableKey.From(100, 5000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "Starters PLO",
                [CashTableKey.From(200, 10000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "Pot Limit Omaha",
                [CashTableKey.From(200, 10000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Freeroll Fullboat",
                [CashTableKey.From(400, 25000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "Highstakes PLO",
                [CashTableKey.From(400, 25000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Freeroll Highstakes",
                [CashTableKey.From(1000, 50000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Freeroll Kings"
            });

        private static readonly ReadOnlyDictionary<CashTableKey, string> ringTables = new ReadOnlyDictionary<CashTableKey, string>(
            new Dictionary<CashTableKey, string>
            {
                [CashTableKey.From(2, 80, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Pune Pockets",
                [CashTableKey.From(2, 80, TableTypeDescription.Regular, GameType.PotLimitOmahaHiLo, 6)] = "PLO Hi/Lo",
                [CashTableKey.From(4, 200, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Ahmedabad Aces",
                [CashTableKey.From(4, 400, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Chandigarh Crack",
                [CashTableKey.From(6, 600, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Kolkata Riders",
                [CashTableKey.From(10, 1000, TableTypeDescription.Speed, GameType.PotLimitOmaha, 6)] = "Pot Limit Omaha",
                [CashTableKey.From(10, 1000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 9)] = "Trichi Trips",
                [CashTableKey.From(10, 1600, TableTypeDescription.Speed, GameType.NoLimitHoldem, 2)] = "Lucknow Headsup",
                [CashTableKey.From(10, 1000, TableTypeDescription.Speed, GameType.PotLimitOmahaHiLo, 6)] = "PLO Hi/Lo Devil",
                [CashTableKey.From(10, 1200, TableTypeDescription.Speed, GameType.NoLimitHoldem, 6)] = "Taj Texas",
                [CashTableKey.From(20, 4000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 2)] = "Headsup Hurricane",
                [CashTableKey.From(20, 2000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 6)] = "Tight Turbo",
                [CashTableKey.From(20, 2000, TableTypeDescription.Speed, GameType.PotLimitOmaha, 6)] = "PLO Studs",
                [CashTableKey.From(30, 4000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 6)] = "Patna Pirates",
                [CashTableKey.From(50, 6000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 6)] = "Bangalore Bluff",
                [CashTableKey.From(50, 6000, TableTypeDescription.Speed, GameType.PotLimitOmaha, 6)] = "PLO Sharks",
                [CashTableKey.From(50, 10000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 6)] = "Delhi Deuce",
                [CashTableKey.From(50, 6000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 2)] = "Delhi Headsup",
                [CashTableKey.From(50, 4000, TableTypeDescription.Speed, GameType.PotLimitOmahaHiLo, 6)] = "PLO Hi/Lo Sharks",
                [CashTableKey.From(50, 6000, TableTypeDescription.Anonymous, GameType.NoLimitHoldem, 6)] = "Surat Anonymous",
                [CashTableKey.From(50, 6000, TableTypeDescription.Anonymous, GameType.PotLimitOmaha, 6)] = "Lucknow PLO Anonymous",
                [CashTableKey.From(50, 6000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 8)] = "Mysore Magic",
                [CashTableKey.From(50, 6000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 9)] = "Bangalore Bluff",
                [CashTableKey.From(100, 8000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 8)] = "Chennai Chill",
                [CashTableKey.From(100, 10000, TableTypeDescription.Speed, GameType.NoLimitHoldem, 2)] = "Hyberabad Headsup",
                [CashTableKey.From(100, 8000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Chennai Chase",
                [CashTableKey.From(100, 20000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Summer",
                [CashTableKey.From(100, 20000, TableTypeDescription.Regular, GameType.PotLimitOmahaHiLo, 6)] = "PLO Hi/Lo Chase",
                [CashTableKey.From(200, 20000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Summer Shorts",
                [CashTableKey.From(200, 40000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Kochi Kickets",
                [CashTableKey.From(200, 40000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Deepstack",
                [CashTableKey.From(200, 40000, TableTypeDescription.Anonymous, GameType.PotLimitOmaha, 6)] = "Mumbai PLO Anonymous",
                [CashTableKey.From(400, 80000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Summer Chase",
                [CashTableKey.From(400, 40000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 2)] = "Himachal Heat",
                [CashTableKey.From(400, 80000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Chase",
                [CashTableKey.From(400, 48000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Shimla Shuffle",
                [CashTableKey.From(400, 48000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Short",
                [CashTableKey.From(600, 120000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Daman Draw",
                [CashTableKey.From(600, 120000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Score",
                [CashTableKey.From(1000, 160000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Grande",
                [CashTableKey.From(1000, 200000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Gujarat Grande",
                [CashTableKey.From(2000, 400000, TableTypeDescription.Regular, GameType.NoLimitHoldem, 6)] = "Rajasthan Royale",
                [CashTableKey.From(2000, 400000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Royale",
                [CashTableKey.From(4000, 1000000, TableTypeDescription.Regular, GameType.PotLimitOmaha, 6)] = "PLO Tycoons",
            });

        #endregion

        private IntPtr FindTournamentTableHandle(HandHistory handHistory)
        {
            var handle = handHistory.GameDescription.Tournament.TournamentsTags == TournamentsTags.STT ?
                FindSttTableHandle(handHistory) : FindMttTableHandle(handHistory);

            return handle;
        }

        private IntPtr FindSttTableHandle(HandHistory handHistory)
        {
            if (handHistory.GameDescription.Tournament.BuyIn == null ||
                string.IsNullOrEmpty(handHistory.TableName) || handHistory.TableName.Length < 2)
            {
                return IntPtr.Zero;
            }

            var totalPrizePool = handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue * handHistory.GameDescription.SeatType.MaxPlayers;

            var tableNumber = handHistory.TableName.Substring(handHistory.TableName.Length - 2);

            var possibleTableName = $"SNG {handHistory.GameDescription.SeatType.MaxPlayers} Player Prize {totalPrizePool } - {tableNumber}";

            var openedTableKeyValue = openedTables.FirstOrDefault(x => x.Value.ContainsIgnoreCase(possibleTableName));
            return openedTableKeyValue.Key;
        }

        private IntPtr FindMttTableHandle(HandHistory handHistory)
        {
            if (string.IsNullOrEmpty(handHistory.GameDescription.Tournament.TournamentName) ||
               string.IsNullOrEmpty(handHistory.TableName) || handHistory.TableName.Length < 2)
            {
                return IntPtr.Zero;
            }

            var tableNumber = handHistory.TableName.Substring(handHistory.TableName.Length - 2);
            var possibleTableName = $"{handHistory.GameDescription.Tournament.TournamentName} - {tableNumber}";

            var openedTableKeyValue = openedTables.FirstOrDefault(x => x.Value.ContainsIgnoreCase(possibleTableName));
            return openedTableKeyValue.Key;
        }

        #endregion

        #region BaseImporter implementation

        protected override void DoImport()
        {
            IsAdda52Launched = null;
            IsAdvancedLogging = settingService.GetSettings()?.GeneralSettings?.IsAdvancedLoggingEnabled ?? false;

            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    if (pokerClientProcess == null || pokerClientProcess.HasExited)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        if (pokerClientProcess != null && pokerClientProcess.HasExited)
                        {
                            IsAdda52Launched = null;
                        }

                        pokerClientProcess = GetPokerClientProcess();

                        if (pokerClientProcess == null)
                        {
                            IsAdda52Launched = false;

                            try
                            {
                                Task.Delay(ScanInterval).Wait(cancellationTokenSource.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }

                            continue;
                        }
                        else if (!IsAdda52Launched.HasValue)
                        {
                            IsAdda52Launched = true;
                        }
                    }

                    var handles = new List<IntPtr>();

                    foreach (ProcessThread thread in pokerClientProcess.Threads)
                    {
                        WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                        {
                            handles.Add(hWnd);
                            return true;
                        }, IntPtr.Zero);
                    }

                    ProcessHandles(handles);

                    try
                    {
                        Task.Delay(ScanInterval).Wait(cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to detect tables. [{SiteString}]", e);
            }

            RaiseProcessStopped();
        }

        #endregion

        #region Infrastructure

        private void ProcessHandles(List<IntPtr> handles)
        {
            lockObject.EnterWriteLock();

            try
            {
                RemoveClosedHandles(handles);

                foreach (var handle in handles)
                {
                    var title = WinApi.GetWindowText(handle);

                    if (!Match(title))
                    {
                        continue;
                    }

                    if (!openedTables.ContainsKey(handle))
                    {
                        openedTables.Add(handle, title);

                        NotifyHUDTableIsDetected(handle, title);

                        LogProvider.Log.Info(this, $"Detected {title} table. [{SiteString}]");
                    }

                    openedTables[handle] = title;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to process handles. [{SiteString}]", e);
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

        private void RemoveClosedHandles(List<IntPtr> handles)
        {
            var handlesToRemove = (from existingHandle in openedTables.Keys
                                   join handle in handles on existingHandle equals handle into gj
                                   from joinedHandle in gj.DefaultIfEmpty()
                                   where joinedHandle == IntPtr.Zero
                                   select existingHandle).ToArray();

            foreach (var handle in handlesToRemove)
            {
                openedTables.Remove(handle);
            }
        }

        private void NotifyHUDTableIsDetected(IntPtr handle, string title)
        {
            string loadingText;

            if (IsAdda52Launched == true)
            {
                loadingText = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_PreLoadingText_Adda52FailedToLoad");
            }
            else if (title.ContainsIgnoreCase(CrazyPineappleTableName))
            {
                loadingText = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_PreLoadingText_Adda52CrazyPineappleNotSupported"); ;
            }
            else
            {
                loadingText = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_PreLoadingText_Adda52");
            }

            SendHUDTableNotification(handle, loadingText);
        }

        private void SendHUDTableNotification(IntPtr handle, string loadingText)
        {
            var gameInfo = new GameInfo
            {
                PokerSite = EnumPokerSites.Adda52,
                TableType = EnumTableType.Nine,
                WindowHandle = handle.ToInt32()
            };

            Task.Run(() =>
            {
                Task.Delay(ScanInterval).Wait();

                var eventArgs = new PreImportedDataEventArgs(gameInfo, loadingText);
                eventAggregator.GetEvent<PreImportedDataEvent>().Publish(eventArgs);
            });
        }

        protected bool Match(string title)
        {
            return !string.IsNullOrEmpty(title) && title.ContainsIgnoreCase("| Blinds : ");
        }

        protected Process GetPokerClientProcess()
        {
            var process = Utils.GetProcessesByNames(new[] { ProcessName }).FirstOrDefault(x =>
            {
                try
                {
                    if (IsAdvancedLogging)
                    {
                        LogProvider.Log.Info(this, $"Found the suitable process {x.ProcessName} [{x.Id}], main window = [{x.MainWindowHandle}] [{SiteString}]");
                    }

                    if (!IsAssociatedWindow(x.MainWindowHandle))
                    {
                        if (IsAdvancedLogging)
                        {
                            LogProvider.Log.Info(this, $"The process {x.ProcessName} [{x.Id}] has no main window. Checking all associated windows [{SiteString}]");
                        }

                        var wasFound = false;

                        foreach (ProcessThread thread in x.Threads)
                        {
                            WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                            {
                                if (IsAssociatedWindow(hWnd))
                                {
                                    wasFound = true;
                                    return false;
                                }

                                return true;
                            }, IntPtr.Zero);
                        }

                        return wasFound;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not check if process matched conditions. [{SiteString}]", e);
                    return false;
                }
            });

            return process;
        }

        protected bool IsAssociatedWindow(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return false;
            }

            var windowTitle = WinApi.GetWindowText(handle);
            var windowClassName = WinApi.GetClassName(handle);

            var match = windowTitle.ContainsIgnoreCase("adda52.com") && windowClassName.ContainsIgnoreCase("Chrome_WidgetWin");

            if (IsAdvancedLogging)
            {
                LogProvider.Log.Info(this, $"Check if the window [{handle}, {windowTitle}, {windowClassName}] of process matches: {match} [{SiteString}]");
            }

            return match;
        }

        #endregion

        #region Helpers 

        private class CashTableKey
        {
            public CashTableKey(decimal bigBlind, TableTypeDescription tableTypeDescription, GameType gameType, int seats, decimal buyin)
            {
                BigBlind = bigBlind;
                TableTypeDescription = tableTypeDescription;
                GameType = gameType;
                Seats = seats;
                Buyin = buyin;
            }

            public static CashTableKey From(decimal bigBlind, decimal buyin, TableTypeDescription tableTypeDescription, GameType gameType, int seats)
            {
                return new CashTableKey(bigBlind, tableTypeDescription, gameType, seats, buyin);
            }

            public decimal BigBlind { get; set; }

            public TableTypeDescription TableTypeDescription { get; set; }

            public GameType GameType { get; set; }

            public int Seats { get; set; }

            public decimal Buyin { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as CashTableKey);
            }

            protected bool Equals(CashTableKey cashTableKey)
            {
                return cashTableKey != null && cashTableKey.BigBlind == BigBlind &&
                    cashTableKey.GameType == GameType && cashTableKey.TableTypeDescription == TableTypeDescription &&
                    cashTableKey.Seats == Seats && cashTableKey.Buyin == Buyin;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + BigBlind.GetHashCode();
                    hashcode = (hashcode * 31) + GameType.GetHashCode();
                    hashcode = (hashcode * 31) + TableTypeDescription.GetHashCode();
                    hashcode = (hashcode * 31) + Seats.GetHashCode();
                    hashcode = (hashcode * 31) + Buyin.GetHashCode();
                    return hashcode;
                }
            }
        }

        #endregion
    }
}