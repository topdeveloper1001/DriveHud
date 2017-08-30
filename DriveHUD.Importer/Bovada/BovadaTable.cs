//-----------------------------------------------------------------------
// <copyright file="BovadaTable.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.Builders.iPoker;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Site;
using Newtonsoft.Json.Linq;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Class represents Bovada table
    /// </summary>
    internal class BovadaTable : IPokerTable
    {
        private const int delayBeforeImport = 5000;

        private ManualResetEventSlim importHandResetEvent = new ManualResetEventSlim(false);

        private List<Command> commands;

        private List<Command> middleHandCommands;

        private Dictionary<int, int> initialStacks;

        private Dictionary<int, int> initialAfterBlindStacks;

        private Dictionary<int, int> seatsPlayerIds;

        private ConcurrentDictionary<int, PlayerFinalPosition> playersFinalPositions;

        private bool hasResultCommand = false;

        private bool intialStacksAdded = false;

        private bool initialAfterBlindStacksAdded = false;

        private IHandHistoryBuilder handHistoryBuilder;

        private bool tableIdHasChanged = false;

        private ISiteConfigurationService configurationService;

        private IEventAggregator eventAggregator;

        public BovadaTable(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            Initialize();
        }

        private void Initialize()
        {
            handHistoryBuilder = new PokerBuilder();

            commands = new List<Command>();
            middleHandCommands = new List<Command>();

            initialStacks = new Dictionary<int, int>();
            initialAfterBlindStacks = new Dictionary<int, int>();

            playersFinalPositions = new ConcurrentDictionary<int, PlayerFinalPosition>();

            seatsPlayerIds = Enumerable.Range(1, 9).ToDictionary(x => x, x => 0);

            BeginDate = DateTime.Now;
        }

        #region Properties

        public bool IsInitialized
        {
            get
            {
                return CurrentHandNumber != 0 && TableId != 0 && WindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(TableName);
            }
        }

        public virtual ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.Bovada;
            }
        }

        #region General Table Data

        public uint Uid
        {
            get;
            private set;
        }

        public string TableName
        {
            get;
            set;
        }

        public uint TableId
        {
            get;
            set;
        }

        public uint TableIndex
        {
            get;
            set;
        }

        public int TableType
        {
            get;
            set;
        }

        public EnumCurrency Currency
        {
            get;
            set;
        }

        public GameFormat GameFormat
        {
            get;
            set;
        }

        public GameLimit GameLimit
        {
            get;
            set;
        }

        public IntPtr WindowHandle
        {
            get;
            set;
        }

        public int MaxSeat
        {
            get;
            set;
        }

        public GameType GameType
        {
            get;
            set;
        }

        public bool IsRealMoney
        {
            get;
            set;
        }

        public uint TournamentId
        {
            get
            {
                if (GameFormat != GameFormat.Cash)
                {
                    return TableId;
                }

                return 0;
            }
        }

        public bool IsTournament
        {
            get
            {
                return GameFormat == GameFormat.MTT || GameFormat == GameFormat.SnG;
            }
        }

        public bool IsZonePokerTable
        {
            get;
            set;
        }

        public bool HeroWasMoved
        {
            get;
            set;
        }

        #endregion

        public uint CurrentHandNumber
        {
            get;
            private set;
        }

        public int BigBlind
        {
            get;
            set;
        }

        public int SmallBlind
        {
            get;
            set;
        }

        public int BuyIn
        {
            get;
            set;
        }

        public int BuyInFee
        {
            get;
            set;
        }

        public BovadaTableState TableState
        {
            get;
            set;
        }

        public int DealerSeat
        {
            get;
            set;
        }

        public bool IsInvalid
        {
            get;
            private set;
        }


        public int LastNotHeroPlayerFinishedPlace
        {
            get;
            set;
        }

        public bool IsHeroFinishedTournament
        {
            get;
            set;
        }

        #region IPokerTable implementation

        private Dictionary<int, string> playersOnTable = new Dictionary<int, string>();

        public Dictionary<int, string> PlayersOnTable
        {
            get
            {
                return playersOnTable;
            }
        }

        public int HeroSeat
        {
            get;
            private set;
        }

        public DateTime BeginDate
        {
            get;
            private set;
        }

        public string InitialTableTitle
        {
            get;
            private set;
        }

        #endregion

        #endregion

        /// <summary>
        /// Process stream data object 
        /// </summary>
        /// <param name="dataObject">Stream data object</param>
        /// <returns>Result of processing</returns>
        public void ProcessCommand<TObject>(TObject dataObject)
        {
            var catcherDataObject = dataObject as BovadaCatcherDataObject;

            if (catcherDataObject == null)
            {
                throw new ArgumentNullException("cmdObj");
            }

            if (catcherDataObject.cmd == null)
            {
                throw new ArgumentException("cmdObj isn't defined");
            }

            ProcessHandle(catcherDataObject.handle);
            ProcessUrlData(catcherDataObject.url);

            ProcessCmdObject(catcherDataObject.cmd);
        }

        #region Infrastructure

        /// <summary>
        /// Process URL data to obtain table general info
        /// </summary>
        /// <param name="urlText">URL</param>
        private void ProcessUrlData(string urlText)
        {
            if (string.IsNullOrEmpty(urlText) || urlText.Length < 20)
            {
                return;
            }

            try
            {
                var uri = new Uri(urlText);
                var parsedQueryString = HttpUtility.ParseQueryString(uri.Query);
                var currencyText = parsedQueryString.Get("currency");
                var gameFormatText = parsedQueryString.Get("gameFormat");
                var gameTypeText = parsedQueryString.Get("gameType");
                var gameLimitText = parsedQueryString.Get("limit");
                var seatText = parsedQueryString.Get("seat");
                var tableIdText = parsedQueryString.Get("tableId");
                var tableNameText = parsedQueryString.Get("tableName");

                Currency = BovadaConverters.ConvertCurrency(currencyText);
                GameFormat = BovadaConverters.ConvertGameFormat(gameFormatText);
                GameType = BovadaConverters.ConvertGameType(gameTypeText);
                GameLimit = BovadaConverters.ConvertGameLimit(gameLimitText);
                MaxSeat = int.Parse(seatText);

                var tableId = uint.Parse(tableIdText);
                tableIdHasChanged = TableId > 0 ? tableId != TableId : false;
                TableId = tableId;

                TableName = HttpUtility.UrlDecode(tableNameText);

                // DHUD-261 fix for ignition update
                if (WindowHandle != IntPtr.Zero && string.IsNullOrEmpty(TableName) && !IsTournament)
                {
                    TableName = $"Table{TableId}";
                }
            }
            catch
            {
                LogProvider.Log.Warn(this, "URL couldn't be parsed. Bad format.");
            }
        }

        /// <summary>
        /// Process windows handle data
        /// </summary>
        /// <param name="handleText">Text with windows handle</param>
        private void ProcessHandle(string handleText)
        {
            if ((WindowHandle != IntPtr.Zero) || string.IsNullOrEmpty(handleText) || handleText.Equals("0xFFFFFFFF"))
            {
                return;
            }

            var handle = BovadaConverters.ConvertHexStringToInt32(handleText);
            WindowHandle = new IntPtr(handle);
        }

        /// <summary>
        /// Process command data object
        /// </summary>
        /// <param name="cmdObj">Command data object</param>
        protected virtual void ProcessCmdObject(BovadaCommandDataObject cmdObj)
        {
            if (cmdObj.pid == null)
            {
                return;
            }

            var isResultCommand = false;
            var refreshTableTitleRequired = false;

            var pid = cmdObj.pid.ToUpperInvariant();

            switch (pid)
            {
                case "T_CONNECT_INFO":
                case "CONNECT_INFO":
                    ParseConnectInfo(cmdObj);
                    break;

                case "PLAY_TABLE_NUMBER":
                    ParsePlayTableNumber(cmdObj);
                    break;

                case "CO_ZONE_CHANGE_TABLE_INFO":
                    ClearInfo();
                    break;

                case "PLAY_TOUR_LEVEL_INFO":
                    refreshTableTitleRequired = IsInitialized;
                    break;

                case "PLAY_STAGE_INFO":
                    ParseStageInfo(cmdObj);
                    break;

                case "PLAY_TOUR_STAGENUMBER":
                    ParseTourStageNumber(cmdObj);
                    break;

                case "CO_TABLE_STATE":
                    ParseTableState(cmdObj.tableState);
                    break;

                case "CO_DEALER_SEAT":
                    ParseDealerSeat(cmdObj);
                    break;

                // pocket cards
                case "CO_CARDTABLE_INFO":
                    AddPocketCardsCommands(cmdObj);
                    break;

                // community cards
                case "CO_BCARD3_INFO":

                    foreach (var card in cmdObj.bcard)
                    {
                        AddCommunityCardCommand(card);
                    }

                    break;

                case "CO_BCARD1_INFO":
                    AddCommunityCardCommand(cmdObj.card.ToString());
                    break;

                // card shown
                case "CO_PCARD_INFO":

                    var jArrayCards = cmdObj.card as JArray;
                    var cards = jArrayCards.Values<int>().ToArray();

                    if (cards != null)
                    {
                        foreach (var card in cards)
                        {
                            AddCardShownCommand(card, cmdObj.seat);
                        }
                    }

                    break;

                case "CO_BLIND_INFO":
                    AddBlindsCommand(cmdObj);
                    break;

                // player action
                case "CO_SELECT_INFO":
                    AddPlayerActionCommand(cmdObj);
                    break;
                case "CO_SELECT_SPEED_INFO":
                    AddPlayerActionCommands(cmdObj);
                    break;

                case "CO_RESULT_INFO":
                    var jArrayAccounts = cmdObj.account as JArray;
                    var accounts = jArrayAccounts.Values<int>().ToArray();
                    AddShowStacksCommands(accounts);
                    isResultCommand = hasResultCommand = true;
                    break;

                case "PLAY_TOUR_OPTION_INFO_V3":
                    BuyIn = cmdObj.buyin;
                    BuyInFee = cmdObj.buyinFee;
                    break;

                case "PLAY_SEAT_INFO":

                    var playerState = (BovadaPlayerState)cmdObj.state;

                    if (playerState != BovadaPlayerState.Playing)
                    {
                        break;
                    }

                    if (cmdObj.type == 0)
                    {
                        AddPlayerRemovedCommand(cmdObj);
                    }
                    else
                    {
                        AddPlayerAddedCommand(cmdObj);
                    }

                    break;

                case "CO_TABLE_INFO":

                    if (IsZonePokerTable)
                    {
                        var accountJArray = cmdObj.account as JArray;
                        var account = accountJArray.Values<int>().ToArray();

                        for (var i = 0; i < account.Length; i++)
                        {
                            if (account[i] == 0)
                            {
                                continue;
                            }

                            var newCmbObj = new BovadaCommandDataObject
                            {
                                account = account[i],
                                seat = i + 1
                            };

                            AddPlayerAddedCommand(newCmbObj);
                        }
                    }

                    if (cmdObj.regSeatNo == null)
                    {
                        break;
                    }

                    var regSeatNoJArray = cmdObj.regSeatNo as JArray;
                    var regSeatNo = regSeatNoJArray.Values<int>().ToArray();

                    for (var i = 0; i < regSeatNo.Length; i++)
                    {
                        var seat = i + 1;

                        if (!seatsPlayerIds.ContainsKey(seat))
                        {
                            seatsPlayerIds.Add(seat, 0);
                        }

                        seatsPlayerIds[seat] = regSeatNo[i];
                    }

                    ParseTableState(cmdObj.tableState);

                    break;

                case "TCO_ANTE_INFO_ALL":
                    AddAnteCommands(cmdObj);
                    break;

                case "CO_ZONE_NO_INFO":
                    IsZonePokerTable = true;
                    break;


                case "PLAY_CLEAR_INFO":

                    if (IsZonePokerTable && commands.Count > 0)
                    {
                        isResultCommand = true;
                    }

                    break;

                case "PLAYER_FINISHED_TOURNAMENT":
                    // if hero finished skip all further such commands
                    if (IsHeroFinishedTournament)
                    {
                        break;
                    }

                    bool isHeroFinished;
                    AddPlayerFinishedTournamentCommand(cmdObj, out isHeroFinished);
                    isResultCommand = isHeroFinished && hasResultCommand;

                    if (isHeroFinished)
                    {
                        IsHeroFinishedTournament = true;
                        // need to set up it hero + 1, because in hand history builder we set place as place - 1
                        LastNotHeroPlayerFinishedPlace = cmdObj.place + 1;
                    }

                    break;

                case "PLAY_TOUR_PRIZE_INFO_V2":
                case "PLAY_TOUR_PRIZE_INFO_REMATCH_V2":

                    var playerFinalPosition = new PlayerFinalPosition
                    {
                        Seat = cmdObj.seat,
                        Place = cmdObj.rank,
                        Prize = BovadaConverters.ConvertPrizeTextToDecimal(cmdObj.prize)
                    };

                    playersFinalPositions.AddOrUpdate(playerFinalPosition.Seat, playerFinalPosition, (key, oldValue) => playerFinalPosition);

                    break;

                case "CO_OPTION_INFO":
                    ParseOptionInfo(cmdObj);
                    break;

                default:
                    break;
            }

            // hand might be ready to push
            if (isResultCommand && !IsInvalid && IsInitialized)
            {
                Validate();

                if (!IsInvalid)
                {
                    UpdateHandNumberCommand();
                    RecoverWaitingBBCommand();
                    RecoverPreflopCommand();
                    RecoverPocketCardsCommand();
                    AddInitialStacks();
                    AddStacksAfterBlind();

                    // Push hand                    
                    var handModel = new HandModel2(commands.ToList());

                    // skip zone poker
                    if (handModel.IsZonePoker || IsZonePokerTable)
                    {
                        return;
                    }

                    handModel.GameType = GameType;
                    handModel.GameLimit = GameLimit;
                    handModel.GameFormat = GameFormat;

                    var configuration = configurationService.Get("Bovada");

                    TryToFixCommunityCardsCommands(handModel);
                    UpdatePlayersOnTable(handModel, configuration);

                    if (handModel.CashOrTournament == CashOrTournament.Tournament && !TournamentsAndPlayers.Keys.Contains(handModel.TournamentNumber))
                    {
                        TournamentsAndPlayers.Add(handModel.TournamentNumber, new Dictionary<int, string>());
                    }

                    UpdatePlayersAddedRemoved(handModel, configuration, true);

                    InitializeActiveTableDict(handModel, configuration);

                    Game game = null;
                    var handHistoryXml = handHistoryBuilder.BuildXml(handModel, this, configuration, out game);

                    UpdatePlayersAddedRemoved(handModel, configuration, false);

                    LogProvider.Log.Info(this, string.Format("Hand {0} processed. [{1}]", handModel.HandNumber, Identifier));

                    var gameInfo = new GameInfo
                    {
                        GameNumber = (long)handModel.HandNumber,
                        GameType = handModel.GameType,
                        GameFormat = handModel.GameFormat,
                        PokerSite = EnumPokerSites.Ignition,
                        TableType = (EnumTableType)MaxSeat,
                        Session = WindowHandle.ToInt32().ToString(),
                        WindowHandle = WindowHandle.ToInt32()
                    };

                    importHandResetEvent.Reset();

                    ImportHand(handHistoryXml, handModel.HandNumber, gameInfo, game);
                }
            }
        }

        #region Command Parsers

        protected virtual void ParseConnectInfo(BovadaCommandDataObject cmdObj)
        {
            // do nothing
        }

        protected virtual void ParseOptionInfo(BovadaCommandDataObject cmdObj)
        {
            // do nothing
        }

        protected virtual void ParseDealerSeat(BovadaCommandDataObject cmdObj)
        {
            if (DealerSeat != 0)
            {
                return;
            }

            AddDealerSeatCommand(cmdObj.seat);
            DealerSeat = cmdObj.seat;
        }

        protected virtual void ParseTourStageNumber(BovadaCommandDataObject cmdObj)
        {
            ParseStageInfo(cmdObj);
        }

        protected virtual void ParseStageInfo(BovadaCommandDataObject cmdObj)
        {
            if (CurrentHandNumber == cmdObj.stageNo)
            {
                return;
            }

            CurrentHandNumber = cmdObj.stageNo;

            if (!IsZonePokerTable)
            {
                ClearInfo();
            }

            AddHandNumberTableName();
            // add missed add/remove commands from middle hand list
            commands.AddRange(middleHandCommands);
            middleHandCommands.Clear();

            importHandResetEvent.Set();
        }

        protected virtual void ParsePlayTableNumber(BovadaCommandDataObject cmdObj)
        {
            // remember that hero was moved, so in history builder we don't need to update seat if preferred seat is set 
            if ((GameFormat == GameFormat.MTT || GameFormat == GameFormat.SnG) && TableIndex != 0 && TableIndex != cmdObj.tableNo)
            {
                HeroWasMoved = true;
            }

            TableIndex = cmdObj.tableNo;
        }

        #endregion

        private void ImportHand(XmlDocument handHistoryXml, ulong handNumber, GameInfo gameInfo, Game game)
        {
            Task.Run(() =>
            {
                // wait for tournament results
                if (gameInfo.GameFormat == GameFormat.MTT || gameInfo.GameFormat == GameFormat.SnG)
                {
                    importHandResetEvent.Wait(delayBeforeImport);

                    if (playersFinalPositions.ContainsKey(HeroSeat))
                    {
                        try
                        {
                            var heroFinalPosition = playersFinalPositions[HeroSeat];

                            var placeNode = handHistoryXml.SelectSingleNode("//place");
                            var winNode = handHistoryXml.SelectSingleNode("//win");

                            if (placeNode != null)
                            {
                                placeNode.InnerText = heroFinalPosition.Place.ToString();
                            }

                            if (winNode != null)
                            {
                                winNode.InnerText = heroFinalPosition.Prize.ToString();
                            }
                        }
                        catch (Exception e)
                        {
                            LogProvider.Log.Error(this, string.Format("Hand {0} final place has not been processed [{1}]", handNumber, Identifier), e);
                        }
                    }
                }

                var dbImporter = ServiceLocator.Current.GetInstance<IFileImporter>();
                var progress = new DHProgress();

                IEnumerable<ParsingResult> parsingResult = null;

                try
                {
                    parsingResult = dbImporter.Import(handHistoryXml.InnerXml, progress, gameInfo);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, string.Format("Hand {0} has not been imported. [{1}]", handNumber, Identifier), e);
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

                    if (result.IsDuplicate)
                    {
                        LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. Duplicate. [{1}]", result.HandHistory.Gamenumber, Identifier));
                        continue;
                    }

                    if (!result.WasImported)
                    {
                        LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. [{1}]", result.HandHistory.Gamenumber, Identifier));
                        continue;
                    }

                    LogProvider.Log.Info(this, string.Format("Hand {0} has been imported. [{1}]", result.HandHistory.Gamenumber, Identifier));

                    var dataImportedArgs = new DataImportedEventArgs(result.Source.Players, gameInfo);

                    eventAggregator.GetEvent<DataImportedEvent>().Publish(dataImportedArgs);
                }
            });
        }

        private int GetHeroSeat(string heroName, HandModel2 handModel, Game game)
        {
            return game.General.Players.FirstOrDefault(x => x.Name == heroName)?.Seat ?? handModel.HeroSeat;
        }

        private void ClearInfo()
        {
            commands.Clear();
            initialStacks.Clear();
            initialAfterBlindStacks.Clear();
            DealerSeat = 0;
            TableState = BovadaTableState.Unknown;
            IsInvalid = false;
            hasResultCommand = false;
            intialStacksAdded = false;
            initialAfterBlindStacksAdded = false;
        }

        private void AddHandNumberTableName()
        {
            var handNumberTableName = new HandNumberTableName
            {
                HandNumber = CurrentHandNumber,
                TableID = IsTournament || IsZonePokerTable ? TableIndex : TableId,
                TableName = TableName,
                TableType = MaxSeat,
                WindowHandle = WindowHandle.ToInt32(),
                TournamentID = TournamentId,
                IsTournament = IsTournament,
                BuyIn = ConvertStack(BuyIn),
                Rake = ConvertStack(BuyInFee),
                HeroWasMoved = HeroWasMoved,
                GameFormat = GameFormat,
                LastNotHeroPlayerFinishedPlace = LastNotHeroPlayerFinishedPlace
            };

            AddCommand(CommandCodeEnum.HandNumberTableName, handNumberTableName, 0);
        }

        private void AddHandPhaseV2Command(int tableState)
        {
            var handPhaseV2 = new HandPhaseV2
            {
                HandPhaseID = tableState,
                HandPhaseEnum = (HandPhaseEnum)tableState
            };

            AddCommand(CommandCodeEnum.HandPhaseV2, handPhaseV2);
        }

        private void AddDealerSeatCommand(int seat)
        {
            var dealerSeat = new DealerSeat
            {
                SeatNumber = seat
            };

            AddCommand(CommandCodeEnum.DealerSeat, dealerSeat);
        }

        private void AddPocketCardsCommands(BovadaCommandDataObject cmdObj)
        {
            var commands = new List<Command>();

            for (var i = 1; i < 10; i++)
            {
                var seatProperty = string.Format("Seat{0}", i);

                var propInfo = typeof(BovadaCommandDataObject).GetProperty(seatProperty);

                var pocketCardsArray = propInfo.GetValue(cmdObj, null) as int[];

                if (pocketCardsArray != null)
                {
                    foreach (var pocketCardInt in pocketCardsArray)
                    {
                        var card = BovadaConverters.ConvertCardIntToString(pocketCardInt);

                        if (!string.IsNullOrEmpty(card))
                        {
                            HeroSeat = i;
                        }

                        var pocketCard = new PocketCards
                        {
                            SeatNumber = i,
                            Card = card,
                            PlayerID = seatsPlayerIds[i]
                        };

                        AddCommand(CommandCodeEnum.PocketCards, pocketCard);
                    }
                }
            }
        }

        private void AddCommunityCardCommand(string card)
        {
            var cardInt = int.Parse(card);
            AddCommunityCardCommand(cardInt);
        }

        private void AddCommunityCardCommand(int card)
        {
            var communityCard = new CommunityCard
            {
                Card = BovadaConverters.ConvertCardIntToString(card)
            };

            AddCommand(CommandCodeEnum.CommunityCard, communityCard);
        }

        private void AddCardShownCommand(int card, int seat)
        {
            var cardShown = new CardShown
            {
                Card = BovadaConverters.ConvertCardIntToString(card),
                SeatNumber = seat,
                PlayerID = seatsPlayerIds[seat]
            };

            AddCommand(CommandCodeEnum.CardShown, cardShown);
        }

        private void AddBlindsCommand(BovadaCommandDataObject cmdObj)
        {
            var btn = int.Parse(cmdObj.btn.ToString());

            var actionType = (btn == 2) ? PlayerActionEnum.PostSB :
                                ((btn == 4) ? PlayerActionEnum.PostBB : PlayerActionEnum.Post);

            var playerAction = new PlayerAction
            {
                SeatNumber = cmdObj.seat,
                PlayerActionID = (int)actionType,
                PlayerActionEnum = actionType,
                PlayerID = seatsPlayerIds[cmdObj.seat]
            };

            AddCommand(CommandCodeEnum.PlayerAction, playerAction);

            InitializeStacks(cmdObj);

            var account = int.Parse(cmdObj.account.ToString());

            if (!initialAfterBlindStacks.ContainsKey(cmdObj.seat))
            {
                initialAfterBlindStacks.Add(cmdObj.seat, account);
            }
            else
            {
                initialAfterBlindStacks[cmdObj.seat] = account;
            }
        }

        private void AddPlayerActionCommand(BovadaCommandDataObject cmdObj)
        {
            var btn = int.Parse(cmdObj.btn.ToString());

            var actionType = (BovadaPlayerActionType)btn;
            var actionEnum = BovadaConverters.ConvertActionTypeToActionEnum(actionType);

            var playerAction = new PlayerAction
            {
                SeatNumber = cmdObj.seat,
                PlayerActionID = (int)actionEnum,
                PlayerActionEnum = actionEnum,
                PlayerID = seatsPlayerIds[cmdObj.seat]
            };

            AddCommand(CommandCodeEnum.PlayerAction, playerAction);

            InitializeStacks(cmdObj);
            AddSetStackCommand(cmdObj);
        }

        private void AddPlayerActionCommands(BovadaCommandDataObject cmdObj)
        {
            var jArrayBtns = cmdObj.btn as JArray;
            var btns = jArrayBtns.Values<int>().ToArray();

            var jArrayAccounts = cmdObj.account as JArray;
            var accounts = jArrayAccounts.Values<int>().ToArray();

            var jArrayBets = cmdObj.bet as JArray;
            var bets = jArrayBets.Values<int>().ToArray();

            var jArrayRaises = cmdObj.raise as JArray;
            var raises = jArrayRaises.Values<int>().ToArray();

            for (var i = 0; i < accounts.Length; i++)
            {
                if (btns[i] == 0)
                {
                    continue;
                }

                var newCmdObj = new BovadaCommandDataObject
                {
                    account = accounts[i],
                    bet = bets[i],
                    btn = btns[i],
                    raise = raises[i],
                    seat = i + 1
                };

                AddPlayerActionCommand(newCmdObj);
            }
        }

        private void AddAnteCommands(BovadaCommandDataObject cmdObj)
        {
            var jArrayAccounts = cmdObj.account as JArray;
            var accounts = jArrayAccounts.Values<int>().ToArray();

            var jArrayAnte = cmdObj.ante as JArray;
            var ante = jArrayAnte.Values<int>().ToArray();

            for (var i = 0; i < ante.Length; i++)
            {
                if (ante[i] == 0)
                {
                    continue;
                }

                var seat = i + 1;

                var newCmdObj = new BovadaCommandDataObject
                {
                    account = accounts[i],
                    bet = 0,
                    raise = 0,
                    btn = (int)BovadaPlayerActionType.Ante,
                    ante = ante[i],
                    seat = seat
                };

                AddPlayerActionCommand(newCmdObj);

                if (!initialAfterBlindStacks.ContainsKey(seat))
                {
                    initialAfterBlindStacks.Add(seat, accounts[i]);
                }
            }
        }

        private void AddCommand(CommandCodeEnum code, object commandObject, int insertIndex = -1)
        {
            var command = new Command
            {
                CommandCode = (int)code,
                CommandCodeEnum = code,
                HandNumber = CurrentHandNumber,
                CommandObject = commandObject
            };

            if (insertIndex > -1)
            {
                commands.Insert(insertIndex, command);
            }
            else
            {
                commands.Add(command);
            }

            // if result command already exists then add add/remove command to middle hand list
            if (hasResultCommand && ((code == CommandCodeEnum.PlayerAdded) || code == CommandCodeEnum.PlayerRemoved))
            {
                var playerAdded = commandObject as PlayerAdded;

                if (playerAdded != null)
                {
                    playerAdded.Previous = true;
                }

                var playerRemoved = commandObject as PlayerRemoved;

                if (playerRemoved != null)
                {
                    playerRemoved.Previous = true;
                }

                middleHandCommands.Add(command);
            }
        }

        private void InitializeStacks(BovadaCommandDataObject cmdObj)
        {
            if (initialStacks.ContainsKey(cmdObj.seat))
            {
                return;
            }

            var account = cmdObj.account != null ? int.Parse(cmdObj.account.ToString()) : 0;
            var raise = cmdObj.raise != null ? int.Parse(cmdObj.raise.ToString()) : 0;
            var bet = cmdObj.bet != null ? int.Parse(cmdObj.bet.ToString()) : 0;
            var ante = cmdObj.ante != null ? int.Parse(cmdObj.ante.ToString()) : 0;
            var dead = cmdObj.dead != null ? int.Parse(cmdObj.dead.ToString()) : 0;

            var addToAccount = raise != 0 ? raise : bet;

            var initialStack = account + addToAccount + ante + dead;

            initialStacks.Add(cmdObj.seat, initialStack);
        }

        private void AddSetStackCommand(BovadaCommandDataObject cmdObj)
        {
            var account = int.Parse(cmdObj.account.ToString());

            var setStack = new Stack
            {
                SeatNumber = cmdObj.seat,
                StackValue = ConvertStack(account),
                PlayerID = seatsPlayerIds[cmdObj.seat]
            };

            AddCommand(CommandCodeEnum.SetStack, setStack);
        }

        private void AddShowStacksCommands(int[] accounts)
        {
            for (var i = 0; i < accounts.Length; i++)
            {
                var seat = i + 1;

                if (!initialStacks.ContainsKey(seat))
                {
                    continue;
                }

                var setStack = new Stack
                {
                    SeatNumber = seat,
                    StackValue = ConvertStack(accounts[i]),
                    PlayerID = seatsPlayerIds[seat]
                };

                AddCommand(CommandCodeEnum.SetStack, setStack);
            }
        }

        private void AddPlayerAddedCommand(BovadaCommandDataObject cmdObj)
        {
            var regSeatNo = cmdObj.regSeatNo != null ? int.Parse(cmdObj.regSeatNo.ToString()) : 0;

            seatsPlayerIds[cmdObj.seat] = regSeatNo;

            var playerAdded = new PlayerAdded
            {
                PlayerID = regSeatNo,
                SeatNumber = cmdObj.seat,
                IsHero = !string.IsNullOrEmpty(cmdObj.nickname)
            };

            AddCommand(CommandCodeEnum.PlayerAdded, playerAdded);

            // we need to add initial stacks because probably not all players will do actions
            if (IsZonePokerTable)
            {
                var newCmdObj = new BovadaCommandDataObject
                {
                    account = cmdObj.account,
                    seat = cmdObj.seat
                };

                InitializeStacks(cmdObj);
            }
        }

        private void AddPlayerRemovedCommand(BovadaCommandDataObject cmdObj)
        {
            var playerRemoved = new PlayerRemoved
            {
                SeatNumber = cmdObj.seat
            };

            AddCommand(CommandCodeEnum.PlayerRemoved, playerRemoved);
        }

        private void AddPlayerFinishedTournamentCommand(BovadaCommandDataObject cmdObj, out bool isHeroFinished)
        {
            isHeroFinished = false;

            var regSeatNo = cmdObj.regSeatNo != null ? int.Parse(cmdObj.regSeatNo.ToString()) : 0;

            var playerSeat = seatsPlayerIds.Where(x => x.Value == regSeatNo).Select(x => x.Key).FirstOrDefault();

            // remember last finished place      
            LastNotHeroPlayerFinishedPlace = cmdObj.place;

            if (playerSeat == 0)
            {
                return;
            }

            var tourneyPlayerFinished = new TourneyPlayerFinished
            {
                PlayerID = regSeatNo,
                Rank = cmdObj.place,
                SeatNumber = playerSeat
            };

            if (playerSeat == HeroSeat)
            {
                isHeroFinished = true;
            }

            AddCommand(CommandCodeEnum.TourneyPlayerFinished, tourneyPlayerFinished);
        }

        private void Validate()
        {
            // check header command
            if (!commands.Any(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName))
            {
                IsInvalid = true;
                return;
            }

            // check community cards
            var communityCards = commands.FilterCommands<CommunityCard>(CommandCodeEnum.CommunityCard).ToArray();

            if (communityCards.Length > 5)
            {
                IsInvalid = true;
                return;
            }
        }

        private void ValidateTableState(int tableState)
        {
            // do not check table state for zone poker
            if (IsZonePokerTable)
            {
                return;
            }

            var newTableState = (BovadaTableState)tableState;

            if (TableState == BovadaTableState.Unknown && newTableState != BovadaTableState.Initializing && newTableState != BovadaTableState.Preparing)
            {
                IsInvalid = true;
                return;
            }

            if (TableState == BovadaTableState.Initializing && newTableState == BovadaTableState.Preflop)
            {
                return;
            }

            if (newTableState == TableState)
            {
                IsInvalid = true;
                return;
            }

            // workaround because of obfuscation doesn't use names so Enum.GetValues is useless
            var tableStates = new int[] {
                (int)BovadaTableState.Unknown,
                (int)BovadaTableState.Preparing,
                (int)BovadaTableState.Initializing,
                (int)BovadaTableState.PostingBlinds,
                (int)BovadaTableState.Preflop,
                (int)BovadaTableState.PostFlop,
                (int)BovadaTableState.Turn,
                (int)BovadaTableState.River,
                (int)BovadaTableState.TournamentShowdown,
                (int)BovadaTableState.TournamentShowStacks,
                (int)BovadaTableState.ShowDown,
                (int)BovadaTableState.ShowStacks
            };

            var currentTableStateIndex = tableStates.FindIndex(x => x == (int)TableState);
            var newTableStateIndex = tableStates.FindIndex(x => x == tableState);

            if (TableState != BovadaTableState.Unknown && newTableState != BovadaTableState.Unknown && newTableState != BovadaTableState.ShowDown && newTableState != BovadaTableState.ShowStacks && (newTableStateIndex - currentTableStateIndex) != 1)
            {
                IsInvalid = true;
            }
        }

        private void AddInitialStacks()
        {
            if (intialStacksAdded)
            {
                return;
            }

            foreach (var initialStack in initialStacks.OrderByDescending(x => x.Key))
            {
                var setStack = new Stack
                {
                    SeatNumber = initialStack.Key,
                    StackValue = ConvertStack(initialStack.Value),
                    PlayerID = seatsPlayerIds[initialStack.Key]
                };

                AddCommand(CommandCodeEnum.SetStack, setStack, 1);
            }

            intialStacksAdded = true;
        }

        private void AddStacksAfterBlind()
        {
            if (initialAfterBlindStacksAdded)
            {
                return;
            }

            foreach (var initialStack in initialStacks.OrderByDescending(x => x.Key))
            {
                int stackValue;

                if (initialAfterBlindStacks.ContainsKey(initialStack.Key))
                {
                    stackValue = initialAfterBlindStacks[initialStack.Key];
                }
                else
                {
                    stackValue = initialStack.Value;
                }

                var setStack = new Stack
                {
                    SeatNumber = initialStack.Key,
                    StackValue = ConvertStack(stackValue),
                    PlayerID = seatsPlayerIds[initialStack.Key]
                };

                var preflopPhaseIndex = commands.FindIndex(x => x.CommandCodeEnum == CommandCodeEnum.HandPhaseV2 && ((HandPhaseV2)x.CommandObject).HandPhaseEnum == HandPhaseEnum.Preflop);

                AddCommand(CommandCodeEnum.SetStack, setStack, preflopPhaseIndex + 1);
            }

            initialAfterBlindStacksAdded = true;
        }

        protected virtual void UpdateHandNumberCommand()
        {
            var handNumberTableNameCommand = commands.FilterCommands<HandNumberTableName>(CommandCodeEnum.HandNumberTableName).FirstOrDefault();

            if (handNumberTableNameCommand == null)
            {
                return;
            }

            handNumberTableNameCommand.TableID = IsTournament || IsZonePokerTable ? TableIndex : TableId;
            handNumberTableNameCommand.TableName = TableName;
            handNumberTableNameCommand.TableType = MaxSeat;
            handNumberTableNameCommand.WindowHandle = WindowHandle.ToInt32();
            handNumberTableNameCommand.TournamentID = TournamentId;
            handNumberTableNameCommand.IsTournament = IsTournament;
            handNumberTableNameCommand.BuyIn = ConvertStack(BuyIn);
            handNumberTableNameCommand.Rake = ConvertStack(BuyInFee);
            handNumberTableNameCommand.GameFormat = GameFormat;
            handNumberTableNameCommand.HeroWasMoved = HeroWasMoved;
            handNumberTableNameCommand.LastNotHeroPlayerFinishedPlace = LastNotHeroPlayerFinishedPlace;

            foreach (var command in commands)
            {
                if (command.HandNumber == 0)
                {
                    command.HandNumber = CurrentHandNumber;
                }
            }
        }

        private void RecoverWaitingBBCommand()
        {
            var waitingForBBCommand = commands.FilterCommands<HandPhaseV2>(CommandCodeEnum.HandPhaseV2).FirstOrDefault(x => x.HandPhaseEnum == HandPhaseEnum.WaitingForBB);

            if (waitingForBBCommand != null)
            {
                return;
            }

            var dealerSeatIndex = commands.FindIndex(x => x.CommandCodeEnum == CommandCodeEnum.DealerSeat);

            var waitingForBBHandPhase = new HandPhaseV2
            {
                HandPhaseEnum = HandPhaseEnum.WaitingForBB,
                HandPhaseID = 4
            };

            AddCommand(CommandCodeEnum.HandPhaseV2, waitingForBBHandPhase, dealerSeatIndex + 1);
        }

        private void RecoverPreflopCommand()
        {
            var preflopCommand = commands.FilterCommands<HandPhaseV2>(CommandCodeEnum.HandPhaseV2).FirstOrDefault(x => x.HandPhaseEnum == HandPhaseEnum.Preflop);

            if (preflopCommand != null)
            {
                return;
            }

            Func<PlayerActionEnum, int> getPostIndex = (actionType) =>
            {
                return commands.FindIndex(x => x.CommandCodeEnum == CommandCodeEnum.PlayerAction && ((PlayerAction)x.CommandObject).PlayerActionEnum == actionType);
            };

            var postIndex = getPostIndex(PlayerActionEnum.Post);

            if (postIndex <= 0)
            {
                postIndex = getPostIndex(PlayerActionEnum.PostBB);

                if (postIndex <= 0)
                {
                    postIndex = getPostIndex(PlayerActionEnum.PostSB);
                }
            }

            var preflopCommandHandPhase = new HandPhaseV2
            {
                HandPhaseEnum = HandPhaseEnum.Preflop,
                HandPhaseID = 8
            };

            AddCommand(CommandCodeEnum.HandPhaseV2, preflopCommandHandPhase, postIndex + 1);
        }

        private void RecoverPocketCardsCommand()
        {
            var pocketCardsCommand = commands.FilterCommands<PocketCards>(CommandCodeEnum.PocketCards).FirstOrDefault();

            if (pocketCardsCommand != null)
            {
                return;
            }

            var preflopIndex = commands.FindIndex(x => x.CommandCodeEnum == CommandCodeEnum.HandPhaseV2 && ((HandPhaseV2)x.CommandObject).HandPhaseEnum == HandPhaseEnum.Preflop);

            for (var i = MaxSeat; i > 0; i--)
            {
                if (!initialStacks.ContainsKey(i))
                {
                    continue;
                }

                var pocketCard = new PocketCards
                {
                    SeatNumber = i,
                    Card = string.Empty,
                    PlayerID = seatsPlayerIds[i]
                };

                AddCommand(CommandCodeEnum.PocketCards, pocketCard, preflopIndex + 1);
                AddCommand(CommandCodeEnum.PocketCards, pocketCard, preflopIndex + 1);
            }
        }

        private void TryToFixCommunityCardsCommands(HandModel2 handModel)
        {
            var communityCardsCommands = handModel.Commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.CommunityCard).ToList();

            if (communityCardsCommands.Count < 3)
            {
                return;
            }

            Action<List<Command>> removeCommunityCards = cmds =>
            {
                if (cmds == null)
                {
                    return;
                }

                var commandsToRemove = cmds.Where(x => x.CommandCodeEnum == CommandCodeEnum.CommunityCard).ToList();
                commandsToRemove.ForEach(x => cmds.Remove(x));
            };

            removeCommunityCards(handModel.PreflopCommands);
            removeCommunityCards(handModel.PostflopCommands);
            removeCommunityCards(handModel.TurnCommands);
            removeCommunityCards(handModel.RiverCommands);
            removeCommunityCards(handModel.ShowDownCommands);

            handModel.PostflopCommands.InsertRange(1, communityCardsCommands.Take(3));

            if (communityCardsCommands.Count > 3 && handModel.TurnCommands != null)
            {
                handModel.TurnCommands.InsertRange(1, communityCardsCommands.Skip(3).Take(1));

                if (communityCardsCommands.Count > 4 && handModel.RiverCommands != null)
                {
                    handModel.RiverCommands.InsertRange(1, communityCardsCommands.Skip(4).Take(1));
                }
            }
        }

        private void UpdatePlayersOnTable(HandModel2 handModel, ISiteConfiguration configuration)
        {
            if (handModel.HeroSeat < 1)
            {
                return;
            }

            if (PlayersOnTable != null && PlayersOnTable.ContainsKey(handModel.HeroSeat))
            {
                PlayersOnTable[handModel.HeroSeat] = configuration.HeroName;
            }
        }

        private void ParseTableState(int tableState)
        {
            tableState = BovadaConverters.ConvertTableState(tableState);
            ValidateTableState(tableState);
            TableState = (BovadaTableState)tableState;
            AddHandPhaseV2Command(tableState);
        }

        private decimal ConvertStack(int stack)
        {
            return ((decimal)stack) / 100m;
        }

        #endregion

        #region Players Handling

        private static Dictionary<long, Dictionary<int, string>> TournamentsAndPlayers = new Dictionary<long, Dictionary<int, string>>();

        private void UpdatePlayersAddedRemoved(HandModel2 hand, ISiteConfiguration configuration, bool previous)
        {
            //update players on table structure
            foreach (var item in hand.SeatsRemovedOrAddedCommands)
            {
                if (item.CommandCodeEnum == CommandCodeEnum.PlayerRemoved)
                {
                    var playerRemoved = (PlayerRemoved)item.CommandObject;

                    if (playerRemoved.Previous != previous)
                    {
                        continue;
                    }

                    if (PlayersOnTable.Keys.Contains(playerRemoved.SeatNumber))
                    {
                        PlayersOnTable.Remove(playerRemoved.SeatNumber);
                    }
                }

                if (item.CommandCodeEnum == CommandCodeEnum.PlayerAdded)
                {
                    var playerAdded = (PlayerAdded)item.CommandObject;

                    if (playerAdded.Previous != previous)
                    {
                        continue;
                    }

                    if (!PlayersOnTable.Keys.Contains(playerAdded.SeatNumber))
                    {
                        var newPlayer = string.Empty;

                        if (playerAdded.IsHero == true)
                        {
                            newPlayer = configuration.HeroName;
                        }
                        else
                        {
                            if (hand.CashOrTournament == CashOrTournament.Tournament)
                            {
                                if (TournamentsAndPlayers.Keys.Contains(hand.TournamentNumber) &&
                                    TournamentsAndPlayers[hand.TournamentNumber].Where(x => x.Key == playerAdded.PlayerID).Count() == 0)
                                {
                                    newPlayer = Utils.GenerateRandomPlayerName(playerAdded.PlayerID);
                                    TournamentsAndPlayers[hand.TournamentNumber].Add(playerAdded.PlayerID, newPlayer);
                                }
                                else if (TournamentsAndPlayers.Keys.Contains(hand.TournamentNumber))
                                {
                                    newPlayer = TournamentsAndPlayers[hand.TournamentNumber][playerAdded.PlayerID];
                                }
                            }
                            else
                            {
                                newPlayer = Utils.GenerateRandomPlayerName(playerAdded.SeatNumber);
                            }
                        }

                        PlayersOnTable.Add(playerAdded.SeatNumber, newPlayer);
                    }
                }
            }
        }

        private void InitializeActiveTableDict(HandModel2 hand, ISiteConfiguration configuration)
        {
            // Moving to new table - remove all players
            if (((hand.CashOrTournament == CashOrTournament.Tournament) || hand.IsZonePoker) && (HeroWasMoved || tableIdHasChanged))
            {
                PlayersOnTable.Clear();
                HeroWasMoved = false;
            }

            var playersToAdd = hand.SeatsOnTable.Except(PlayersOnTable.Keys).ToArray();

            if (playersToAdd.Length > 0)
            {
                foreach (var item in playersToAdd)
                {
                    var newPlayer = string.Empty;

                    if (item != hand.HeroSeat)
                    {
                        if (hand.CashOrTournament == CashOrTournament.Tournament)
                        {
                            if (TournamentsAndPlayers.ContainsKey(hand.TournamentNumber) &&
                                TournamentsAndPlayers[hand.TournamentNumber].Where(x => x.Key == hand.SeatsPlayerID[item]).Count() == 0)
                            {
                                if (hand.SeatsPlayerID[item] == 0)
                                {
                                    hand.SeatsPlayerID[item] = Utils.GenerateRandomNumber();
                                }
                                newPlayer = Utils.GenerateRandomPlayerName(hand.SeatsPlayerID[item]);
                                TournamentsAndPlayers[hand.TournamentNumber].Add(hand.SeatsPlayerID[item], newPlayer);
                            }
                            else if (TournamentsAndPlayers.Keys.Contains(hand.TournamentNumber))
                            {
                                newPlayer = TournamentsAndPlayers[hand.TournamentNumber][hand.SeatsPlayerID[item]];
                            }
                        }
                        else
                        {
                            newPlayer = Utils.GenerateRandomPlayerName(item);
                        }
                    }
                    else
                    {
                        newPlayer = configuration.HeroName;
                    }

                    PlayersOnTable.Add(item, newPlayer);
                }
            }
        }

        private class PlayerFinalPosition
        {
            public int Seat { get; set; }

            public decimal Prize { get; set; }

            public int Place { get; set; }
        }

        #endregion
    }
}