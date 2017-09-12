//-----------------------------------------------------------------------
// <copyright file="HandHistoryParserFastImpl.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandHistories.Parser.Parsers.FastParser.Base
{
    internal abstract class HandHistoryParserFastImpl : IHandHistoryParser
    {
        private static readonly Regex LineSplitRegex = new Regex("\n|\r", RegexOptions.Compiled);
        private static readonly Regex HandSplitRegex = new Regex("\r\n\r\n", RegexOptions.Compiled);

        public abstract EnumPokerSites SiteName { get; }

        public virtual bool RequiresAdjustedRaiseSizes
        {
            get { return false; }
        }

        public virtual bool RequiresActionSorting
        {
            get { return false; }
        }

        public virtual bool RequiresAllInDetection
        {
            get { return false; }
        }

        public virtual bool RequiresAllInUpdates
        {
            get { return false; }
        }

        public virtual bool RequiresTotalPotCalculation
        {
            get { return false; }
        }

        public virtual bool SupportRunItTwice
        {
            get { return false; }
        }

        public virtual bool RequiresBetWinAdjustment
        {
            get { return false; }
        }

        public virtual bool RequiresSeatTypeAdjustment
        {
            get
            {
                return false;
            }
        }

        public virtual bool RequiresTournamentSpeedAdjustment
        {
            get
            {
                return false;
            }
        }

        public virtual IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false)
                            .Select(s => s.Trim('\r', '\n'));
        }

        public virtual IEnumerable<string[]> SplitUpMultipleHandsToLines(string rawHandHistories)
        {
            foreach (var hand in SplitUpMultipleHands(rawHandHistories))
            {
                yield return SplitHandsLines(hand);
            }
        }

        protected virtual string[] SplitHandsLines(string handText)
        {
            string[] text = handText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = text[i].Trim();
            }
            return text;
        }

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            try
            {
                string[] lines = SplitHandsLines(handText);

                bool isCancelled;

                if (IsValidOrCanceledHand(lines, out isCancelled) == false)
                {
                    throw new InvalidHandException(handText ?? "NULL");
                }

                return ParseFullHandSummary(lines, isCancelled);
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                LogProvider.Log.Warn(this, string.Format("Couldn't parse hand {0} with error {1} and trace {2}", handText, ex.Message, ex.StackTrace));

                return null;
            }
        }

        protected HandHistorySummary ParseFullHandSummary(string[] handLines, bool isCancelled = false)
        {
            HandHistorySummary handHistorySummary = new HandHistorySummary();

            handHistorySummary.Cancelled = isCancelled;
            handHistorySummary.DateOfHandUtc = ParseDateUtc(handLines);
            handHistorySummary.GameDescription = ParseGameDescriptor(handLines);

            if (handHistorySummary.GameDescription.PokerFormat == PokerFormat.Tournament)
            {
                handHistorySummary.GameDescription.Tournament = ParseTournament(handLines);
            }

            handHistorySummary.HandId = ParseHandId(handLines);
            handHistorySummary.TableName = ParseTableName(handLines);
            handHistorySummary.NumPlayersSeated = ParsePlayers(handLines).Count;
            handHistorySummary.DealerButtonPosition = ParseDealerPosition(handLines);
            handHistorySummary.FullHandHistoryText = string.Join("\r\n", handLines);

            try
            {
                ParseExtraHandInformation(handLines, handHistorySummary);
            }
            catch
            {
                throw new ExtraHandParsingAction(handLines[0]);
            }

            return handHistorySummary;
        }

        protected virtual void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistorySummary)
        {
            // do nothing
        }

        public virtual HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false)
        {
            var handHistory = new HandHistory();

            try
            {
                handText = ClearHandHistory(handText);

                string[] handLines = SplitHandsLines(handText);

                // parse summary hand
                if (IsSummaryHand(handLines))
                {
                    return ParseSummaryHand(handLines, handHistory);
                }

                bool isCancelled;

                if (!IsValidOrCanceledHand(handLines, out isCancelled))
                {
                    throw new InvalidHandException(handText ?? "NULL");
                }

                //Set members outside of the constructor for easier performance analysis               
                handHistory.DateOfHandUtc = ParseDateUtc(handLines);
                handHistory.GameDescription = ParseGameDescriptor(handLines);

                if (handHistory.GameDescription.PokerFormat == PokerFormat.Tournament)
                {
                    handHistory.GameDescription.Tournament = ParseTournament(handLines);
                }

                handHistory.HandId = ParseHandId(handLines);
                handHistory.TableName = ParseTableName(handLines);
                handHistory.DealerButtonPosition = ParseDealerPosition(handLines);
                handHistory.FullHandHistoryText = string.Join("\r\n", handLines);
                handHistory.CommunityCards = ParseCommunityCards(handLines);
                handHistory.Cancelled = isCancelled;

                handHistory.Players = ParsePlayers(handLines);
                handHistory.NumPlayersSeated = handHistory.Players.Count;

                string heroName = ParseHeroName(handLines, handHistory.Players);
                handHistory.Hero = handHistory.Players.FirstOrDefault(p => p.PlayerName == heroName);

                if (handHistory.Cancelled)
                {
                    return handHistory;
                }

                if (handHistory.Players.Count(p => p.IsSittingOut == false) <= 1)
                {
                    throw new PlayersException(handText, "Only found " + handHistory.Players.Count + " players with actions.");
                }

                handHistory.HandActions = ParseHandActions(handLines, handHistory.GameDescription.GameType);

                if (handHistory.GameDescription.Limit.BigBlind == 0 || handHistory.GameDescription.Limit.SmallBlind == 0)
                {
                    ParseBlinds(handHistory);
                }

                if (handHistory.TotalPot == null)
                {
                    handHistory.TotalPot = handHistory.HandActions.Where(x => x.Amount < 0).Sum(x => Math.Abs(x.Amount));
                }

                if (RequiresActionSorting)
                {
                    handHistory.HandActions = OrderHandActions(handHistory.HandActions);
                }

                if (RequiresAdjustedRaiseSizes)
                {
                    handHistory.HandActions = AdjustRaiseSizes(handHistory.HandActions);
                }

                if (RequiresAllInDetection)
                {
                    handHistory.HandActions = AllInActionHelper.IdentifyAllInActions(ParsePlayers(handLines), handHistory.HandActions);
                }

                if (RequiresAllInUpdates)
                {
                    handHistory.HandActions = AllInActionHelper.UpdateAllInActions(handHistory.HandActions);
                }

                if (RequiresSeatTypeAdjustment)
                {
                    AdjustSeatTypes(handHistory);
                }

                if (handHistory.GameDescription.IsTournament && RequiresTournamentSpeedAdjustment)
                {
                    AdjustTournamentSpeed(handHistory);
                }

                HandAction anteAction = handHistory.HandActions.FirstOrDefault(a => a.HandActionType == HandActionType.ANTE);

                if (anteAction != null)
                {
                    handHistory.GameDescription.Limit.IsAnteTable = true;
                    handHistory.GameDescription.Limit.Ante = Math.Abs(anteAction.Amount);
                }

                try
                {
                    ParseExtraHandInformation(handLines, handHistory);
                }
                catch (Exception)
                {
                    throw new ExtraHandParsingAction(handLines[0]);
                }

                // remove inactive players
                var players = handHistory.HandActions.Where(x => x.Street == Street.Preflop).Select(x => x.PlayerName).ToList();

                foreach (var player in handHistory.Players.ToList())
                {
                    if (players.Contains(player.PlayerName))
                    {
                        if (RequiresBetWinAdjustment)
                        {
                            var playerActions = handHistory.HandActions.Where(x => x.PlayerName.Equals(player.PlayerName));

                            if (playerActions != null && playerActions.Any())
                            {
                                player.Win = playerActions.Where(x => x.IsWinningsAction).Sum(x => x.Amount);
                                player.Bet = Math.Abs(playerActions.Where(x => x.Amount < 0).Sum(x => x.Amount));
                            }
                        }

                        continue;
                    }

                    handHistory.Players.Remove(player);
                }

                return handHistory;
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                LogProvider.Log.Warn(this, string.Format("Couldn't parse hand {0} with error {1} and trace {2}", handText, ex.Message, ex.StackTrace));

                handHistory.Exception = ex;

                return handHistory;
            }
        }

        protected abstract string ParseHeroName(string[] handlines, PlayerList playerList);

        public string ParseHeroName(string handText)
        {
            return ParseHeroName(SplitHandsLines(handText), null);
        }

        public int ParseDealerPosition(string handText)
        {
            try
            {
                return ParseDealerPosition(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new PlayersException(handText, "ParseDealerPosition: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract int ParseDealerPosition(string[] handLines);

        public DateTime ParseDateUtc(string handText)
        {
            try
            {
                return ParseDateUtc(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new ParseHandDateException(handText, "ParseDateUtc: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract DateTime ParseDateUtc(string[] handLines);

        public long ParseHandId(string handText)
        {
            try
            {
                return ParseHandId(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new HandIdException(handText, "ParseHandId: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract long ParseHandId(string[] handLines);

        public string ParseTableName(string handText)
        {
            try
            {
                return ParseTableName(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new TableNameException(handText, "ParseTableName: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract string ParseTableName(string[] handLines);

        public GameDescriptor ParseGameDescriptor(string handText)
        {
            string[] lines = SplitHandsLines(handText);

            return ParseGameDescriptor(lines);
        }

        protected GameDescriptor ParseGameDescriptor(string[] handLines)
        {
            return new GameDescriptor(ParsePokerFormat(handLines),
                                      SiteName,
                                      ParseGameType(handLines),
                                      ParseLimit(handLines),
                                      ParseTableType(handLines),
                                      ParseSeatType(handLines),
                                      null);
        }

        protected abstract PokerFormat ParsePokerFormat(string[] handLines);

        public PokerFormat ParsePokerFormat(string handText)
        {
            try
            {
                return ParsePokerFormat(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new PokerFormatException(handText, "ParsePokerFormat: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract TournamentDescriptor ParseTournament(string[] handLines);

        public SeatType ParseSeatType(string handText)
        {
            try
            {
                return ParseSeatType(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new SeatTypeException(handText, "ParseSeatType: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract SeatType ParseSeatType(string[] handLines);

        public GameType ParseGameType(string handText)
        {
            try
            {
                return ParseGameType(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new UnrecognizedGameTypeException(handText, "ParseGameType: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract GameType ParseGameType(string[] handLines);

        public TableType ParseTableType(string handText)
        {
            try
            {
                return ParseTableType(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new TableTypeException(handText, "ParseTableType: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract TableType ParseTableType(string[] handLines);

        public Limit ParseLimit(string handText)
        {
            try
            {
                return ParseLimit(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new LimitException(handText, "ParseLimit: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract Limit ParseLimit(string[] handLines);

        public Buyin ParseBuyin(string handText)
        {
            try
            {
                return ParseBuyin(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new BuyinException(handText, "ParseBuyin: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract Buyin ParseBuyin(string[] handLines);

        public int ParseNumPlayers(string handText)
        {
            try
            {
                return ParsePlayers(SplitHandsLines(handText)).Count;
            }
            catch (Exception ex)
            {
                throw new PlayersException(handText, "ParseNumPlayers: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        public bool IsValidHand(string handText)
        {
            return IsValidHand(SplitHandsLines(handText));
        }

        public abstract bool IsValidHand(string[] handLines);

        public bool IsValidOrCanceledHand(string handText, out bool isCancelled)
        {
            return IsValidOrCanceledHand(SplitHandsLines(handText), out isCancelled);
        }

        public abstract bool IsValidOrCanceledHand(string[] handLines, out bool isCancelled);

        public List<HandAction> ParseHandActions(string handText)
        {
            try
            {
                string[] handLines = SplitHandsLines(handText);
                GameType gameType = ParseGameType(handLines);
                List<HandAction> handActions = ParseHandActions(handLines, gameType);

                if (RequiresActionSorting)
                {
                    handActions = OrderHandActions(handActions);
                }
                if (RequiresAdjustedRaiseSizes)
                {
                    handActions = AdjustRaiseSizes(handActions);
                }
                if (RequiresAllInDetection)
                {
                    handActions = AllInActionHelper.IdentifyAllInActions(ParsePlayers(handLines), handActions);
                }
                if (RequiresAllInUpdates)
                {
                    handActions = AllInActionHelper.UpdateAllInActions(handActions);
                }

                return handActions;
            }
            catch (Exception ex)
            {
                throw new HandActionException(handText, "ParseHandActions: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        // We pass the game-type in as it can change the actions and parsing logic
        protected abstract List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown);

        /// <summary>
        /// Sometimes hand actions are listed out of order, but with an order number or time stamp (this happens on IPoker).
        /// In these cases the hands must be reorganized before being displayed.
        /// </summary>
        protected List<HandAction> OrderHandActions(List<HandAction> handActions)
        {
            return handActions.OrderBy(action => action.ActionNumber).ToList();
        }

        /// <summary>
        /// Re-raise amounts are handled differently on different sites. Consider the
        /// situation where:
        ///          Player1 Bets $10
        ///          Player2 Raises to $30 total (call of $10, raise $20)
        ///          Player1 Raises to $100 total (call of $20, raise $70)
        /// 
        /// Party will display this as: Bet 10, Raise 30, Raise 70
        /// Stars will display this as: Bet 10, Raise to 30, Raise to 100. 
        /// 
        /// In the case for Stars we will need to deduct previous action amounts from the raise to figure out how
        /// much that raise actual is i.e Player 1 only wagered $90 more.
        /// </summary>
        /// <param name="handActions"></param>
        protected List<HandAction> AdjustRaiseSizes(List<HandAction> handActions)
        {
            var actionsByStreets = handActions.GroupBy(h => h.Street);

            foreach (var actionsByStreet in actionsByStreets)
            {
                List<HandAction> actions = actionsByStreet.ToList();

                // loop backward through the actions and subtracting the action prior to each raise
                // from that raise amount
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    HandAction currentAction = actions[i];

                    if (currentAction.HandActionType != HandActionType.RAISE)
                    {
                        continue;
                    }

                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (actions[j].PlayerName.Equals(currentAction.PlayerName))
                        {
                            // Ante's don't get counted in the raise action lines
                            if (actions[j].HandActionType == HandActionType.ANTE)
                            {
                                continue;
                            }

                            // a POSTS SB is always dead money
                            // a POSTS BB needs to be deducted completely
                            // a POSTS SB+BB only the BB needs to be deducted
                            if (actions[j].HandActionType == HandActionType.POSTS)
                            {
                                // we use <= due to the negative numbers
                                if (actions[j].Amount <= actions.First(a => a.HandActionType == HandActionType.BIG_BLIND).Amount)
                                {
                                    currentAction.DecreaseAmount(actions.First(a => a.HandActionType == HandActionType.BIG_BLIND).Amount);
                                }
                                continue;
                            }

                            // If the player previously called any future raise will be the entire amount
                            if (actions[j].HandActionType == HandActionType.CALL)
                            {
                                currentAction.DecreaseAmount(actions[j].Amount);
                                continue;
                            }


                            // Player who posted SB/BB/SB+BB can check on their first action
                            if (actions[j].HandActionType == HandActionType.CHECK)
                            {
                                continue;
                            }

                            currentAction.DecreaseAmount(actions[j].Amount);
                            break;
                        }
                    }
                }
            }

            return handActions;
        }

        public PlayerList ParsePlayers(string handText)
        {
            try
            {
                return ParsePlayers(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new PlayersException(handText, "ParsePlayers: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract PlayerList ParsePlayers(string[] handLines);

        public BoardCards ParseCommunityCards(string handText)
        {
            try
            {
                return ParseCommunityCards(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new CardException(handText, "ParseCommunityCards: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract BoardCards ParseCommunityCards(string[] handLines);

        protected virtual void ParseBlinds(HandHistory handHistory)
        {
            if (handHistory == null)
            {
                return;
            }

            var blindsActions = handHistory.HandActions.Where(x => x.IsBlinds).ToArray();

            var bigBlindAction = blindsActions.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND);
            var smallBlindAction = blindsActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND);

            var smallBlind = 0m;
            var bigBlind = 0m;

            if (bigBlindAction != null)
            {
                bigBlind = bigBlindAction.Amount;
            }

            if (smallBlindAction != null)
            {
                smallBlind = smallBlindAction.Amount;
            }

            if (smallBlind == 0)
            {
                smallBlind = bigBlind / 2m;
            }

            if (bigBlind == 0)
            {
                bigBlind = smallBlind * 2m;
            }

            handHistory.GameDescription.Limit.SmallBlind = Math.Abs(smallBlind);
            handHistory.GameDescription.Limit.BigBlind = Math.Abs(bigBlind);
        }

        protected virtual bool IsSummaryHand(string[] handLines)
        {
            return false;
        }

        protected virtual HandHistory ParseSummaryHand(string[] handLines, HandHistory handHistory)
        {
            throw new NotImplementedException("Method not defined");
        }

        public virtual string GetTournamentIdFromFileName(string filename)
        {
            return string.Empty;
        }

        protected virtual string ClearHandHistory(string handText)
        {
            return handText;
        }

        protected virtual void AdjustSeatTypes(HandHistory handHistory)
        {
        }

        protected virtual void AdjustTournamentSpeed(HandHistory handHistory)
        {
        }
    }
}