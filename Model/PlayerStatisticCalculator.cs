//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticCalculator.cs" company="Ace Poker Solutions">
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
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Model.Extensions;
using Model.Importer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    /// <summary>
    /// Class to calculate player statistic
    /// </summary>
    public class PlayerStatisticCalculator : IPlayerStatisticCalculator
    {
        public Playerstatistic CalculateStatistic(ParsingResult result, Players u)
        {
            var parsedHand = result.Source;
            var player = u.Playername;

            var stat = new Playerstatistic
            {
                PlayerName = player,
                Numberofplayers = (short)parsedHand.NumPlayersActive,
                PlayerId = u.PlayerId,
                GametypeId = (short)result.GameType.GametypeId,
            };

            var currentPlayer = parsedHand.Players.FirstOrDefault(x => string.Equals(x.PlayerName, player, StringComparison.OrdinalIgnoreCase));
            stat.StartingStack = currentPlayer.StartingStack;
            stat.CurrencyId = (short)parsedHand.GameDescription.Limit.Currency;
            stat.PokergametypeId = (short)(parsedHand.GameDescription.GameType);

            var playerHandActions = parsedHand.HandActions.Where(x => x.PlayerName == player).ToArray();

            int call = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street > Street.Preflop);
            int bet = playerHandActions.Count(handAction => handAction.IsBet() && handAction.Street > Street.Preflop);
            int raises = playerHandActions.Count(handAction => handAction.IsRaise() && handAction.Street > Street.Preflop);

            stat.TotalbetsFlop = playerHandActions.Count(handAction => (handAction.IsBet() || handAction.IsRaise()) && handAction.Street == Street.Flop);
            stat.TotalbetsTurn = playerHandActions.Count(handAction => (handAction.IsBet() || handAction.IsRaise()) && handAction.Street == Street.Turn);
            stat.TotalbetsRiver = playerHandActions.Count(handAction => (handAction.IsBet() || handAction.IsRaise()) && handAction.Street == Street.River);

            stat.TotalcallsFlop = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street == Street.Flop);
            stat.TotalcallsTurn = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street == Street.Turn);
            stat.TotalcallsRiver = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street == Street.River);

            bool playerFolded = playerHandActions.Any(x => x.IsFold);
            bool sawShowDown = !playerFolded &&
                parsedHand.HandActions
                .GroupBy(x => x.PlayerName)
                .Where(x => x.Key != player)
                .Any(x => x.All(p => !p.IsFold));

            bool wasRiver = CardHelper.IsStreetAvailable(parsedHand.CommunityCards, Street.River);
            bool wasTurn = wasRiver || CardHelper.IsStreetAvailable(parsedHand.CommunityCards, Street.Turn);
            bool wasFlop = wasTurn || CardHelper.IsStreetAvailable(parsedHand.CommunityCards, Street.Flop);

            bool playedRiver = playerHandActions.Any(x => x.Street == Street.River);
            bool playedTurn = playedRiver || playerHandActions.Any(x => x.Street == Street.Turn);
            bool playedFlop = playedTurn || playerHandActions.Any(x => x.Street == Street.Flop);

            bool won = playerHandActions.Any(handAction => handAction.IsWinningsAction);

            bool wonShowdown = sawShowDown && won;

            decimal netWon = playerHandActions.Sum(x => x.Amount);

            bool wonFlop = playedFlop && won;
            bool wonTurn = playedTurn && won;
            bool wonRiver = playedRiver && won;

            bool vpip = playerHandActions.PreFlopAny(handAction => handAction.IsRaise() || handAction.IsCall());
            bool pfr = playerHandActions.PreFlopAny(handAction => handAction.IsRaise());
            bool pfrOcurred = parsedHand.PreFlop.Any(handAction => handAction.HandActionType == HandActionType.RAISE);

            var preflops = parsedHand.PreFlop.ToList();

            bool aggresiveFlop = playerHandActions.FlopAny(handAction => handAction.IsRaise() || handAction.IsBet());
            bool aggresiveTurn = playerHandActions.TurnAny(handAction => handAction.IsRaise() || handAction.IsBet());
            bool aggresiveRiver = playerHandActions.RiverAny(handAction => handAction.IsRaise() || handAction.IsBet());

            bool betOnFlop = playerHandActions.FlopAny(handAction => handAction.IsBet());
            bool betOnTurn = playerHandActions.TurnAny(handAction => handAction.IsBet());

            bool isCheckedFlop = playerHandActions.FirstOrDefault(x => x.Street == Street.Flop)?.IsCheck ?? false;

            var positionFlopPlayer = GetInPositionPlayer(parsedHand, Street.Preflop);
            var preflopInPosition = positionFlopPlayer != null && positionFlopPlayer.PlayerName == player;

            bool isBluffPreflop = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.Preflop);
            bool isBluffFlop = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.Flop);
            bool isBluffTurn = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.Turn);
            bool isBluffRiver = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.River);

            var numberOfActivePlayerOfFlop = parsedHand.NumPlayersActive - parsedHand.PreFlop.Count(x => x.IsFold);
            #region cbet

            ConditionalBet flopCBet = new ConditionalBet(), turnCBet = new ConditionalBet(), riverCBet = new ConditionalBet();

            if (pfrOcurred)
            {
                var wasAllIn = parsedHand.PreFlop.OfType<AllInAction>().Any(x => x.PlayerName != player);

                string raiser = parsedHand.PreFlop.Last(x => x.IsRaise()).PlayerName;

                if (wasFlop && !wasAllIn)
                {
                    var flops = parsedHand.Flop.ToList();
                    wasAllIn = flops.OfType<AllInAction>().Any(x => x.PlayerName != player);
                    var wasReraise = flops.Any(x => x.PlayerName != player && x.IsRaise());

                    CalculateContinuationBet(flopCBet, flops, player, raiser);

                    if (flopCBet.Happened && wasTurn && !wasAllIn && !wasReraise)
                    {
                        var turns = parsedHand.Turn.ToList();
                        wasAllIn = turns.OfType<AllInAction>().Any(x => x.PlayerName != player);
                        wasReraise = turns.Any(x => x.PlayerName != player && x.IsRaise());

                        CalculateContinuationBet(turnCBet, turns, player, raiser);

                        if (turnCBet.Happened && wasRiver && !wasAllIn && !wasReraise)
                        {
                            var rivers = parsedHand.River.ToList();
                            CalculateContinuationBet(riverCBet, rivers, player, raiser);
                        }
                    }
                }
            }

            ConditionalBet turnIpPassFlopCbet = new ConditionalBet();
            var positionturnPlayer = GetInPositionPlayer(parsedHand, Street.Flop);

            var flopInPosition = positionturnPlayer != null && positionturnPlayer.PlayerName == player;

            if (positionturnPlayer != null && positionturnPlayer.PlayerName == player && flopCBet.Passed)
            {
                var action = parsedHand.Turn.FirstOrDefault(x => x.PlayerName == player);
                if (action != null)
                    turnIpPassFlopCbet.CheckAction(action);
            }

            ConditionalBet riverIpPassFlopCbet = new ConditionalBet();
            var positionRiverPlayer = GetInPositionPlayer(parsedHand, Street.Turn);
            if (positionRiverPlayer != null && positionRiverPlayer.PlayerName == player && turnCBet.Passed)
            {
                var action = parsedHand.River.FirstOrDefault(x => x.PlayerName == player);
                if (action != null)
                    riverIpPassFlopCbet.CheckAction(action);
            }

            #endregion

            #region Float

            bool isFloatFlop = flopCBet.Called && currentPlayer.hasHoleCards && isBluffFlop;

            #endregion

            #region 3bet

            ConditionalBet threeBet = new ConditionalBet();

            if (pfrOcurred)
            {
                var raiser = preflops.FirstOrDefault(x => x.HandActionType == HandActionType.RAISE).PlayerName;
                Calculate3Bet(threeBet, preflops, player, raiser);
            }

            #endregion

            #region 4bet

            ConditionalBet fourBet = new ConditionalBet();

            if (pfrOcurred)
                Calculate4Bet(fourBet, preflops, player);

            #endregion

            #region 2 preflop raisers
            ConditionalBet twoPfr = new ConditionalBet();

            if (pfrOcurred)
                Calculate2PreflopRaisers(twoPfr, preflops, player);

            #endregion

            #region Steal

            StealAttempt stealBBAtempt = new StealAttempt();
            StealAttempt stealSBAtempt = new StealAttempt();
            ConditionalBet stealBet = new ConditionalBet();
            ConditionalBet threeBetVsSteal = new ConditionalBet();

            CalculateSteal(stealBet, parsedHand, player);
            Calculate3BetVsSteal(threeBetVsSteal, parsedHand, player);
            // First big blind action is from player who is actually on BB spot, next actions are from players that joined game and payer BB
            var bigBlind = parsedHand.HandActions.Any(x => x.PlayerName == player && x.HandActionType == HandActionType.BIG_BLIND);
            if (bigBlind)
                CalculateBigBlindSteal(stealBBAtempt, parsedHand, player);

            var smallBlind = parsedHand.HandActions.Any(x => x.PlayerName == player && x.HandActionType == HandActionType.SMALL_BLIND);
            if (smallBlind)
                CalculateSmallBlindSteal(stealSBAtempt, parsedHand, player);

            #endregion

            #region Cold Call

            var coldCall = new Condition();
            if (pfrOcurred)
                CalculateColdCall(coldCall, preflops, player);

            #endregion

            #region Squeeze

            var squeeze = new Condition();
            if (pfrOcurred)
                CalculateSqueeze(squeeze, preflops, player);

            #endregion

            #region Raise

            var flopRaise = CalculateRaise(parsedHand.Flop, player);
            var turnRaise = CalculateRaise(parsedHand.Turn, player);
            var riverRaise = CalculateRaise(parsedHand.River, player);

            #endregion

            #region Check river on bx line

            Condition checkRiverOnBxLine = new Condition();
            if (playedRiver)
            {
                CalculateCheckRiverOnBxLine(checkRiverOnBxLine, parsedHand, player);
            }
            #endregion

            #region Limp

            ConditionalBet limp = new ConditionalBet();
            CalculateLimpBet(limp, preflops, player);

            #endregion

            #region Donk Bet

            Condition donkBet = new Condition();
            if (pfrOcurred && !pfr)
            {
                CalculateDonkBet(donkBet, parsedHand.HandActions, player);
            }

            #endregion

            var nomSmallPot = parsedHand.TotalPot > 5 * parsedHand.GameDescription.Limit.BigBlind;
            var largePot = parsedHand.TotalPot > 40 * parsedHand.GameDescription.Limit.BigBlind;

            var wasRaise = parsedHand.PreFlop.Any(handAction => handAction.IsRaise());

            var limped = !wasRaise && parsedHand.PreFlop.Any(handAction => handAction.PlayerName == player && (handAction.HandActionType == HandActionType.CALL || handAction.HandActionType == HandActionType.CHECK));
            var firstCall = parsedHand.PreFlop.FirstOrDefault(handAction => handAction.HandActionType == HandActionType.CALL);
            var smallBlindAction = parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND);
            if (firstCall != null && smallBlindAction != null)
            {
                if (firstCall.PlayerName == smallBlindAction.PlayerName)
                    limped = false;
            }

            var cutoff = GetCutOffPlayer(parsedHand);
            var dealer = GetDealerPlayer(parsedHand);

            stat.SmallBlind = Math.Abs(parsedHand.GameDescription.Limit.SmallBlind);
            stat.BigBlind = Math.Abs(parsedHand.GameDescription.Limit.BigBlind);
            stat.Ante = 0;
            if (parsedHand.GameDescription.Limit.IsAnteTable)
            {
                stat.Ante = Math.Abs(parsedHand.GameDescription.Limit.Ante);
            }
            stat.IsSmallBlind = smallBlind;
            stat.IsBigBlind = bigBlind;
            stat.IsCutoff = cutoff != null && cutoff.PlayerName == player;
            stat.IsDealer = dealer != null && dealer.PlayerName == player;

            bool isOpenRaise = IsOpenRaise(preflops, player);
            bool isRaisedLimpers = IsRaisedLimpers(preflops, player);
            Dictionary<Street, bool> checkRaisedOnStreetDictionary = GetCheckRaisedOnStreets(parsedHand.HandActions, player);

            stat.IsRelativePosition = IsRelativePosition(parsedHand.HandActions, player);
            stat.IsRelative3BetPosition = IsRelative3BetPosition(parsedHand.HandActions, player);

            #region True Aggression

            int flopTrueAggressionBets = checkRaisedOnStreetDictionary[Street.Flop] ? 2 : ((aggresiveFlop && !flopCBet.Made) ? 1 : 0);
            int turnTrueAggressionBets = checkRaisedOnStreetDictionary[Street.Turn] ? 2 : (aggresiveTurn ? 1 : 0);
            int riverTrueAggressionBets = checkRaisedOnStreetDictionary[Street.River] ? 2 : (aggresiveRiver ? 1 : 0);
            #endregion

            #region Tournament

            stat.IsTourney = parsedHand.GameDescription.IsTournament;

            if (stat.IsTourney)
            {
                stat.TournamentId = parsedHand.GameDescription.Tournament.TournamentId;
            }

            #endregion

            stat.Sawshowdown = sawShowDown ? 1 : 0;
            stat.Sawnonsmallshowdown = sawShowDown && nomSmallPot ? 1 : 0;
            stat.Sawnonsmallshowdownlimpedflop = sawShowDown && nomSmallPot && limped ? 1 : 0;
            stat.Sawlargeshowdown = sawShowDown && largePot ? 1 : 0;
            stat.Sawlargeshowdownlimpedflop = sawShowDown && largePot && limped ? 1 : 0;

            stat.Sawflop = playedFlop ? 1 : 0;
            stat.SawTurn = playedTurn ? 1 : 0;
            stat.SawRiver = playedRiver ? 1 : 0;

            stat.FlopAggPossible = playedFlop ? 1 : 0;
            stat.TurnAggPossible = playedTurn ? 1 : 0;
            stat.RiverAggPossible = playedRiver ? 1 : 0;

            stat.Wonshowdown = wonShowdown ? 1 : 0;
            stat.Wonnonsmallshowdown = wonShowdown && nomSmallPot ? 1 : 0;
            stat.Wonnonsmallshowdownlimpedflop = wonShowdown && nomSmallPot && limped ? 1 : 0;
            stat.Wonlargeshowdown = wonShowdown && largePot ? 1 : 0;
            stat.Wonlargeshowdownlimpedflop = wonShowdown && largePot && limped ? 1 : 0;
            stat.Wonhand = won ? 1 : 0;
            stat.Wonhandwhensawflop = wonFlop ? 1 : 0;
            stat.Wonhandwhensawturn = wonTurn ? 1 : 0;
            stat.Wonhandwhensawriver = wonRiver ? 1 : 0;

            stat.Pfrhands = pfr ? 1 : 0;
            stat.Vpiphands = vpip ? 1 : 0;

            stat.DidDonkBet = donkBet.Made ? 1 : 0;
            stat.CouldDonkBet = donkBet.Possible ? 1 : 0;

            stat.Didthreebet = threeBet.Made ? 1 : 0;
            stat.DidThreeBetIp = threeBet.Made && flopInPosition ? 1 : 0;
            stat.DidThreeBetOop = threeBet.Made && !flopInPosition ? 1 : 0;
            stat.Couldthreebet = threeBet.Possible ? 1 : 0;
            stat.Facedthreebetpreflop = threeBet.Faced ? 1 : 0;
            stat.Foldedtothreebetpreflop = threeBet.Folded ? 1 : 0;
            stat.Calledthreebetpreflop = threeBet.Called ? 1 : 0;
            stat.Raisedthreebetpreflop = threeBet.Raised ? 1 : 0;

            stat.Totalhands = 1;
            stat.Totalcalls = call;
            stat.Totalbets = bet + raises;
            stat.Totalpostflopstreetsplayed = (playedFlop ? 1 : 0) + (playedTurn ? 1 : 0) + (playedRiver ? 1 : 0);
            stat.Totalaggressivepostflopstreetsseen = (aggresiveFlop ? 1 : 0) + (aggresiveRiver ? 1 : 0) + (aggresiveTurn ? 1 : 0);
            stat.Totalamountwonincents = (int)(netWon * 100);

            if (parsedHand.Rake != null && stat.Numberofplayers > 0)
            {
                stat.Totalrakeincents = ((int)decimal.Round((decimal)(parsedHand.Rake / stat.Numberofplayers) * 100, MidpointRounding.AwayFromZero));
            }
            else
            {
                stat.Totalrakeincents = 0;
            }

            stat.Bigblindstealfaced = stealBBAtempt.Faced ? 1 : 0;
            stat.Bigblindstealdefended = stealBBAtempt.Defended ? 1 : 0;
            stat.Bigblindstealfolded = stealBBAtempt.Folded ? 1 : 0;
            stat.Bigblindstealreraised = stealBBAtempt.Raised ? 1 : 0;

            stat.Smallblindstealfaced = stealSBAtempt.Faced ? 1 : 0;
            stat.Smallblindstealattempted = stealSBAtempt.Attempted ? 1 : 0;
            stat.Smallblindstealdefended = stealSBAtempt.Defended ? 1 : 0;
            stat.Smallblindstealfolded = stealSBAtempt.Folded ? 1 : 0;
            stat.Smallblindstealreraised = stealSBAtempt.Raised ? 1 : 0;

            stat.BlindsStealDefended = stealSBAtempt.Defended || stealBBAtempt.Defended ? 1 : 0;

            stat.Flopcontinuationbetpossible = flopCBet.Possible ? 1 : 0;
            stat.Flopcontinuationbetmade = flopCBet.Made ? 1 : 0;
            stat.Flopcontinuationipbetmade = flopCBet.Made && flopInPosition ? 1 : 0;
            stat.Flopcontinuationoopbetmade = flopCBet.Made && !flopInPosition ? 1 : 0;
            stat.Turncontinuationbetpossible = turnCBet.Possible ? 1 : 0;
            stat.Turncontinuationbetmade = turnCBet.Made ? 1 : 0;
            stat.Rivercontinuationbetpossible = riverCBet.Possible ? 1 : 0;
            stat.Rivercontinuationbetmade = riverCBet.Made ? 1 : 0;

            stat.TurnContinuationBetWithAirMade = turnCBet.Made && isBluffTurn ? 1 : 0;

            stat.Facingflopcontinuationbet = flopCBet.Faced ? 1 : 0;
            stat.Foldedtoflopcontinuationbet = flopCBet.Folded ? 1 : 0;
            stat.Calledflopcontinuationbet = flopCBet.Called ? 1 : 0;
            stat.Raisedflopcontinuationbet = flopCBet.Raised ? 1 : 0;
            stat.Facingturncontinuationbet = turnCBet.Faced ? 1 : 0;
            stat.Foldedtoturncontinuationbet = turnCBet.Folded ? 1 : 0;
            stat.Calledturncontinuationbet = turnCBet.Called ? 1 : 0;
            stat.Raisedturncontinuationbet = turnCBet.Raised ? 1 : 0;
            stat.Facingrivercontinuationbet = riverCBet.Faced ? 1 : 0;
            stat.Foldedtorivercontinuationbet = riverCBet.Folded ? 1 : 0;
            stat.Calledrivercontinuationbet = riverCBet.Called ? 1 : 0;
            stat.Raisedrivercontinuationbet = riverCBet.Raised ? 1 : 0;

            stat.Couldfourbet = fourBet.Possible ? 1 : 0;
            stat.Didfourbet = fourBet.Made ? 1 : 0;
            stat.Facedfourbetpreflop = fourBet.Faced ? 1 : 0;
            stat.Foldedtofourbetpreflop = fourBet.Folded ? 1 : 0;
            stat.Calledfourbetpreflop = fourBet.Called ? 1 : 0;
            stat.Raisedfourbetpreflop = fourBet.Raised ? 1 : 0;

            stat.Facingtwopreflopraisers = twoPfr.Faced ? 1 : 0;
            stat.Calledtwopreflopraisers = twoPfr.Called ? 1 : 0;
            stat.Raisedtwopreflopraisers = twoPfr.Raised ? 1 : 0;

            stat.Turnfoldippassonflopcb = turnIpPassFlopCbet.Folded ? 1 : 0;
            stat.Turncallippassonflopcb = turnIpPassFlopCbet.Called ? 1 : 0;
            stat.Turnraiseippassonflopcb = turnIpPassFlopCbet.Raised ? 1 : 0;
            stat.Riverfoldippassonturncb = riverIpPassFlopCbet.Folded ? 1 : 0;
            stat.Rivercallippassonturncb = riverIpPassFlopCbet.Called ? 1 : 0;
            stat.Riverraiseippassonturncb = riverIpPassFlopCbet.Raised ? 1 : 0;

            stat.Playedyearandmonth = int.Parse(parsedHand.DateOfHandUtc.ToString("yyyyMM"));

            stat.Couldsqueeze = squeeze.Possible ? 1 : 0;
            stat.Didsqueeze = squeeze.Made ? 1 : 0;

            stat.DidOpenRaise = isOpenRaise ? 1 : 0;
            stat.IsRaisedLimpers = isRaisedLimpers ? 1 : 0;
            stat.DidCheckRaise = checkRaisedOnStreetDictionary.Any(x => x.Value) ? 1 : 0;
            stat.DidFlopCheckRaise = checkRaisedOnStreetDictionary[Street.Flop] ? 1 : 0;
            stat.DidTurnCheckRaise = checkRaisedOnStreetDictionary[Street.Turn] ? 1 : 0;
            stat.DidRiverCheckRaise = checkRaisedOnStreetDictionary[Street.River] ? 1 : 0;

            stat.Couldcoldcall = coldCall.Possible ? 1 : 0;
            stat.Didcoldcall = coldCall.Made ? 1 : 0;
            stat.DidColdCallIp = coldCall.Made && preflopInPosition ? 1 : 0;
            stat.DidColdCallOop = coldCall.Made && !preflopInPosition ? 1 : 0;

            stat.DidDelayedTurnCBet = flopCBet.Possible && !flopCBet.Made && betOnTurn ? 1 : 0;
            stat.CouldDelayedTurnCBet = flopCBet.Possible && !flopCBet.Made && playedTurn ? 1 : 0;

            stat.PlayedFloatFlop = isFloatFlop ? 1 : 0;

            stat.CouldRaiseFlop = flopRaise.Count(x => x.Possible);
            stat.DidRaiseFlop = flopRaise.Count(x => x.Made);
            stat.CouldRaiseTurn = turnRaise.Count(x => x.Possible);
            stat.DidRaiseTurn = turnRaise.Count(x => x.Made);
            stat.CouldRaiseRiver = riverRaise.Count(x => x.Possible);
            stat.DidRaiseRiver = riverRaise.Count(x => x.Made);

            stat.WasFlop = wasFlop ? 1 : 0;
            stat.WasTurn = wasTurn ? 1 : 0;
            stat.WasRiver = wasRiver ? 1 : 0;

            stat.DidBluffedRiver = playedRiver && isBluffRiver ? 1 : 0;

            stat.DidCheckFlop = isCheckedFlop ? 1 : 0;

            stat.LimpPossible = limp.Possible ? 1 : 0;
            stat.LimpMade = limp.Made ? 1 : 0;
            stat.LimpFaced = limp.Faced ? 1 : 0;
            stat.LimpCalled = limp.Called ? 1 : 0;
            stat.LimpFolded = limp.Folded ? 1 : 0;
            stat.LimpReraised = limp.Raised ? 1 : 0;

            #region Additional

            stat.GameNumber = parsedHand.HandId;
            stat.PokersiteId = result.HandHistory.PokersiteId;
            stat.TableType = parsedHand.GameDescription.TableType.ToString();
            stat.Time = parsedHand.DateOfHandUtc;
            stat.Cards = Converter.ToHoleCards(currentPlayer.HoleCards);

            stat.Pot = parsedHand.TotalPot ?? 0;
            CalculateTotalPotValues(stat);

            List<string> line = new List<string>();
            foreach (var streetLine in parsedHand.HandActions.Where(x => x.PlayerName == stat.PlayerName || x is StreetAction).GroupBy(x => x.Street))
            {
                line.Add(String.Join(string.Empty, streetLine.Select(Converter.ActionToString)));
            }
            stat.Line = string.Join(StringFormatter.ActionLineSeparator, line).Trim(StringFormatter.ActionLineSeparator.ToCharArray());

            stat.Board = parsedHand.CommunityCards != null ? parsedHand.CommunityCards.ToString() : string.Empty;
            stat.Allin = Converter.ToAllin(parsedHand, stat);
            stat.Action = Converter.ToAction(stat);
            stat.Equity = Converter.CalculateAllInEquity(parsedHand, stat);
            stat.Position = Converter.ToPosition(parsedHand, stat);
            stat.PositionString = Converter.ToPositionString(stat.Position);
            stat.FacingPreflop = Converter.ToFacingPreflop(parsedHand.PreFlop, player);

            if (parsedHand.GameDescription.Limit.SmallBlind > 0 && parsedHand.GameDescription.Limit.BigBlind > 0)
            {
                stat.GameType = string.Format("{0}/{1} - {2}", parsedHand.GameDescription.Limit.SmallBlind,
                    parsedHand.GameDescription.Limit.BigBlind, parsedHand.GameDescription.GameType);
            }
            else
            {
                stat.GameType = parsedHand.GameDescription.GameType.ToString();
            }

            stat.SawUnopenedPot = (!parsedHand.PreFlop.Any(x => x.IsRaise())) ? 1 : 0;

            CalculateUnopenedPot(stat, parsedHand);
            CalculatePositionalStats(stat);

            stat.DidthreebetBluffInSb = (stat.DidThreeBetInSb != 0) && isBluffPreflop ? 1 : 0;
            stat.DidthreebetBluffInBb = (stat.DidThreeBetInBb != 0) && isBluffPreflop ? 1 : 0;
            stat.DidthreebetBluffInBlinds = (stat.DidthreebetBluffInSb) != 0 || (stat.DidthreebetBluffInBb != 0) ? 1 : 0;

            stat.DidfourbetBluff = (stat.Didfourbet != 0) && isBluffPreflop ? 1 : 0;
            stat.DidFourBetBluffInBtn = (stat.DidFourBetInBtn != 0) && isBluffPreflop ? 1 : 0;

            stat.StealPossible = stealBet.Possible ? 1 : 0;
            stat.StealMade = stealBet.Made ? 1 : 0;

            stat.CouldThreeBetVsSteal = threeBetVsSteal.Possible ? 1 : 0;
            stat.DidThreeBetVsSteal = threeBetVsSteal.Made ? 1 : 0;

            stat.PlayerFolded = playerFolded;

            stat.CouldCheckRiverOnBXLine = checkRiverOnBxLine.Possible ? 1 : 0;
            stat.DidCheckRiverOnBXLine = checkRiverOnBxLine.Made ? 1 : 0;

            stat.TotalAggressiveBets = flopTrueAggressionBets + turnTrueAggressionBets + riverTrueAggressionBets;

            stat.FacedHandsUpOnFlop = playedFlop && (numberOfActivePlayerOfFlop == 2) ? 1 : 0;
            stat.FacedMultiWayOnFlop = playedFlop && (numberOfActivePlayerOfFlop > 2) ? 1 : 0;

            stat.StackInBBs = stat.StartingStack / stat.BigBlind;
            stat.MRatio = CalculateMRatio(stat);
            CalculatePositionalData(stat);

            #endregion

            return stat;
        }

        #region Infrastructure

        internal static decimal CalculateMRatio(Playerstatistic stat)
        {
            decimal totalAntes = stat.Ante * stat.Numberofplayers;
            decimal mRatioValue = stat.StartingStack / (Math.Abs(stat.SmallBlind) + Math.Abs(stat.BigBlind) + Math.Abs(totalAntes));

            return mRatioValue;
        }

        internal static bool IsUnopened(Playerstatistic stat)
        {
            return (stat.FacingPreflop == EnumFacingPreflop.Unopened
                || stat.FacingPreflop == EnumFacingPreflop.Limper
                || stat.FacingPreflop == EnumFacingPreflop.MultipleLimpers);
        }

        public static void CalculatePositionalData(Playerstatistic stat)
        {
            int unopened = IsUnopened(stat) ? 1 : 0;

            if (stat.PositionTotal == null)
            {
                stat.PositionTotal = new PositionalStat();
            }

            if (stat.PositionUnoppened == null)
            {
                stat.PositionUnoppened = new PositionalStat();
            }

            if (stat.PositionVPIP == null)
            {
                stat.PositionVPIP = new PositionalStat();
            }

            if (stat.PositionDidColdCall == null)
            {
                stat.PositionDidColdCall = new PositionalStat();
            }

            if (stat.PositionCouldColdCall == null)
            {
                stat.PositionCouldColdCall = new PositionalStat();
            }

            if (stat.PositionDidThreeBet == null)
            {
                stat.PositionDidThreeBet = new PositionalStat();
            }

            if (stat.PositionCouldThreeBet == null)
            {
                stat.PositionCouldThreeBet = new PositionalStat();
            }

            stat.PositionTotal.SetPositionalStat(stat.Position, 1);
            stat.PositionUnoppened.SetPositionalStat(stat.Position, unopened);
            stat.PositionVPIP.SetPositionalStat(stat.Position, stat.Vpiphands);
            stat.PositionDidColdCall.SetPositionalStat(stat.Position, stat.Didcoldcall);
            stat.PositionCouldColdCall.SetPositionalStat(stat.Position, stat.Couldcoldcall);
            stat.PositionDidThreeBet.SetPositionalStat(stat.Position, stat.Didthreebet);
            stat.PositionCouldThreeBet.SetPositionalStat(stat.Position, stat.Couldthreebet);

        }

        public static void CalculateTotalPotValues(Playerstatistic stat)
        {
            stat.TotalPot = stat.Pot;
            stat.TotalPotInBB = (stat.TotalPot != 0) && (stat.BigBlind) != 0 ? stat.TotalPot / stat.BigBlind : 0;
        }

        private void CalculateCheckRiverOnBxLine(Condition checkRiverOnBxLine, HandHistory parsedHand, string player)
        {
            var flopAction = parsedHand.Flop.FirstOrDefault(x => x.PlayerName == player);
            var turnAction = parsedHand.Turn.FirstOrDefault(x => x.PlayerName == player);
            var riverAction = parsedHand.River.FirstOrDefault(x => x.PlayerName == player);

            if (flopAction != null && turnAction != null && riverAction != null)
            {
                if (flopAction.IsBet() && turnAction.HandActionType == HandActionType.CHECK)
                {
                    checkRiverOnBxLine.Possible = true;
                    checkRiverOnBxLine.Happened = checkRiverOnBxLine.Made = riverAction.HandActionType == HandActionType.CHECK;
                }
            }
        }

        private static IEnumerable<Condition> CalculateRaise(IEnumerable<HandAction> streetActions, string player)
        {
            var conditions = new List<Condition>();
            if (streetActions.Any(x => x.IsBet()))
            {
                var betIndex = streetActions.ToList().IndexOf(streetActions.First(x => x.IsBet()));
                var playerActionsAfterBet = streetActions.Skip(betIndex + 1).Where(x => x.PlayerName == player);
                foreach (var action in playerActionsAfterBet)
                {
                    Condition raiseCondition = new Condition();
                    raiseCondition.Possible = true;
                    raiseCondition.Made = raiseCondition.Happened = action.IsRaise();

                    conditions.Add(raiseCondition);
                }
            }
            return conditions;
        }

        private static bool IsBluff(HoleCards holeCards, BoardCards communityCards, Street street)
        {
            if (holeCards == null || communityCards == null || !CardHelper.IsStreetAvailable(communityCards.ToString(), street))
            {
                return false;
            }

            var boardCards = communityCards.GetBoardOnStreet(street);
            var hand = new HoldemHand.Hand(holeCards.ToString(), boardCards.ToString());
            if (hand.HandTypeValue != HoldemHand.Hand.HandTypes.HighCard)
            {
                return false;
            }

            return !HoldemHand.Hand.IsFlushDraw(hand.MaskValue, 0UL)
                && !HoldemHand.Hand.IsStraightDraw(hand.MaskValue, 0UL);
        }

        private static void CalculateUnopenedPot(Playerstatistic stat, HandHistory parsedHand)
        {
            var firstRaiser = parsedHand.PreFlop.FirstOrDefault(x => x.IsRaise());
            stat.FirstRaiser = (firstRaiser != null && firstRaiser.PlayerName == stat.PlayerName) ? 1 : 0;

            if (stat.FirstRaiser == 0)
                return;

            switch (stat.PositionString)
            {
                case "EP":
                    stat.UO_PFR_EP = stat.Pfrhands;
                    return;
                case "MP":
                    stat.UO_PFR_MP = stat.Pfrhands;
                    return;
                case "CO":
                    stat.UO_PFR_CO = stat.Pfrhands;
                    return;
                case "BTN":
                    stat.UO_PFR_BN = stat.Pfrhands;
                    return;
                case "SB":
                    stat.UO_PFR_SB = stat.Pfrhands;
                    return;
                case "BB":
                    stat.UO_PFR_BB = stat.Pfrhands;
                    return;
            }
        }

        private static void CalculatePositionalStats(Playerstatistic stat)
        {
            switch (stat.PositionString)
            {
                case "MP":
                    stat.DidThreeBetInMp = stat.Didthreebet;
                    stat.DidFourBetInMp = stat.Didfourbet;
                    stat.DidColdCallInMp = stat.Didcoldcall;
                    return;
                case "CO":
                    stat.DidThreeBetInCo = stat.Didthreebet;
                    stat.DidFourBetInCo = stat.Didfourbet;
                    stat.DidColdCallInCo = stat.Didcoldcall;
                    return;
                case "BTN":
                    stat.DidThreeBetInBtn = stat.Didthreebet;
                    stat.DidFourBetInBtn = stat.Didfourbet;
                    stat.DidColdCallInBtn = stat.Didcoldcall;
                    return;
                case "SB":
                    stat.DidThreeBetInSb = stat.Didthreebet;
                    stat.DidFourBetInSb = stat.Didfourbet;
                    stat.DidColdCallInSb = stat.Didcoldcall;
                    return;
                case "BB":
                    stat.DidThreeBetInBb = stat.Didthreebet;
                    stat.DidFourBetInBb = stat.Didfourbet;
                    stat.DidColdCallInBb = stat.Didcoldcall;
                    return;
                default:
                    return;
            }
        }

        private static void CalculateSmallBlindSteal(StealAttempt stealSbAtempt, HandHistory parsedHand, string player)
        {
            if (parsedHand.PreFlop.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND)?.PlayerName != player)
            {
                return;
            }

            var stealers = new List<string>();

            stealers.Add(GetCutOffPlayer(parsedHand)?.PlayerName);
            stealers.Add(parsedHand.Players.FirstOrDefault(x => x.SeatNumber == parsedHand.DealerButtonPosition)?.PlayerName);
            stealers.Add(parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND)?.PlayerName);

            bool wasSteal = false;
            foreach (var action in parsedHand.PreFlop)
            {
                if (wasSteal)
                {
                    if (action.PlayerName == player)
                    {
                        stealSbAtempt.Faced = true;
                        stealSbAtempt.Defended = action.IsCall || action.IsRaise();
                        stealSbAtempt.Raised = action.IsRaise();
                        stealSbAtempt.Folded = action.IsFold;

                        return;
                    }

                    if (action.IsRaise())
                    {
                        return;
                    }
                }
                else
                {
                    if (action.IsRaise() && stealers.Contains(action.PlayerName))
                    {
                        wasSteal = true;
                        if (action.PlayerName == player)
                        {
                            stealSbAtempt.Attempted = true;
                            return;
                        }
                        continue;
                    }

                    if (action.IsRaise())
                    {
                        return;
                    }
                }
            }
        }

        private static void CalculateSteal(ConditionalBet stealBet, HandHistory parsedHand, string player)
        {
            var stealers = new List<string>();

            stealers.Add(GetCutOffPlayer(parsedHand)?.PlayerName);
            stealers.Add(parsedHand.Players.FirstOrDefault(x => x.SeatNumber == parsedHand.DealerButtonPosition)?.PlayerName);
            stealers.Add(parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND)?.PlayerName);

            if (!stealers.Where(x => !string.IsNullOrWhiteSpace(x)).Any(x => x == player))
            {
                return;
            }

            foreach (var action in parsedHand.PreFlop)
            {
                if (action.PlayerName == player)
                {
                    stealBet.Possible = true;
                    stealBet.Made = action.IsRaise();
                    return;
                }

                if (action.IsRaise())
                {
                    return;
                }
            }
        }

        private static void CalculateBigBlindSteal(StealAttempt stealBbAtempt, HandHistory parsedHand, string player)
        {
            if (parsedHand.PreFlop.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND)?.PlayerName != player)
            {
                return;
            }

            var stealers = new List<string>();

            stealers.Add(GetCutOffPlayer(parsedHand)?.PlayerName);
            stealers.Add(parsedHand.Players.FirstOrDefault(x => x.SeatNumber == parsedHand.DealerButtonPosition)?.PlayerName);
            stealers.Add(parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND)?.PlayerName);

            bool wasSteal = false;
            foreach (var action in parsedHand.PreFlop)
            {
                if (wasSteal)
                {
                    if (action.PlayerName == player)
                    {
                        stealBbAtempt.Faced = true;
                        stealBbAtempt.Defended = action.IsCall || action.IsRaise();
                        stealBbAtempt.Raised = action.IsRaise();
                        stealBbAtempt.Folded = action.IsFold;

                        return;
                    }

                    if (action.IsRaise())
                    {
                        return;
                    }
                }
                else
                {
                    if (action.IsRaise() && stealers.Contains(action.PlayerName))
                    {
                        wasSteal = true;
                        continue;
                    }

                    if (action.IsRaise())
                    {
                        return;
                    }
                }
            }
        }

        private static void Calculate3BetVsSteal(ConditionalBet threeBetVsSteal, HandHistory parsedHand, string player)
        {
            var stealers = new List<string>();

            stealers.Add(GetCutOffPlayer(parsedHand)?.PlayerName);
            stealers.Add(parsedHand.Players.FirstOrDefault(x => x.SeatNumber == parsedHand.DealerButtonPosition)?.PlayerName);
            stealers.Add(parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND)?.PlayerName);

            bool isThreeBetVsStealPossible = false;
            string stealer = string.Empty;
            int stealerIndex = -1;
            for (int i = 0; i < parsedHand.PreFlop.Count(); i++)
            {
                var action = parsedHand.PreFlop.ElementAt(i);

                if (isThreeBetVsStealPossible)
                {
                    Calculate3Bet(threeBetVsSteal, parsedHand.PreFlop.Skip(stealerIndex).ToList(), player, stealer);
                }
                else
                {
                    if (action.IsRaise() && stealers.Contains(action.PlayerName))
                    {
                        isThreeBetVsStealPossible = true;
                        stealer = action.PlayerName;
                        stealerIndex = i;
                    }

                    if (action.IsRaise())
                    {
                        return;
                    }
                }
            }
        }

        private static void Calculate4Bet(ConditionalBet fourBet, List<HandAction> preflops, string player)
        {
            var raisers = new List<string>();
            HandAction first2NdRaiseAction = null;

            foreach (var action in preflops)
            {
                if (action.PlayerName == player && raisers.Contains(player))
                {
                    fourBet.Possible = true;
                }

                if (!action.IsRaise()) continue;

                if (raisers.Contains(action.PlayerName))
                {
                    first2NdRaiseAction = action;
                    if (action.PlayerName == player)
                    {
                        fourBet.Made = true;
                        //If player made 4bet can return ,because he cant fold,call or raise own bet
                        return;
                    }

                    break;
                }

                raisers.Add(action.PlayerName);
            }

            if (first2NdRaiseAction == null)
                return;

            var fourBetRaiser = first2NdRaiseAction.PlayerName;
            fourBet.Happened = true;

            int raiserIndex = preflops.IndexOf(first2NdRaiseAction);
            for (int i = raiserIndex + 1; i < preflops.Count; i++)
            {
                var action = preflops[i];

                if (action.PlayerName == player)
                {
                    fourBet.CheckAction(action);
                }

                if (action.PlayerName == fourBetRaiser)
                    break;
            }
        }

        private static void Calculate2PreflopRaisers(ConditionalBet twoPfr, List<HandAction> preflops, string player)
        {
            var raisers = preflops.Where(x => x.IsRaise()).ToList();
            if (raisers.Count > 1 && raisers[0].PlayerName != player && raisers[1].PlayerName != player)
            {
                int raiserIndex = preflops.IndexOf(raisers[1]);

                for (int i = 0; i < preflops.Count; i++)
                {
                    var action = preflops[i];
                    if (i <= raiserIndex)
                    {
                        if (action.PlayerName == player)
                            break;
                        if (!action.IsRaise() &&
                            (action.PlayerName == raisers[0].PlayerName ||
                             action.PlayerName == raisers[1].PlayerName))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (action.PlayerName == raisers[0].PlayerName)
                            break;

                        if (action.PlayerName == player)
                        {
                            if (twoPfr.CheckAction(action))
                                break;
                        }
                    }
                }
            }
        }

        private static void CalculateColdCall(Condition coldCall, IList<HandAction> preflops, string player)
        {
            bool wasColdRaise = false;
            foreach (var action in preflops)
            {
                if (wasColdRaise)
                {
                    if (action.PlayerName != player)
                        continue;

                    coldCall.Possible = true;
                    if (action.IsCall())
                        coldCall.Made = true;

                    return;
                }

                if (action.PlayerName == player && action.IsRaise())
                    return;

                if (action.IsRaise())
                    wasColdRaise = true;
            }
        }

        private static void CalculateSqueeze(Condition squeeze, IList<HandAction> preflops, string player)
        {
            string squeezeStarter = null;
            bool wasSqueezeRaise = false, wasSqueezeCall = false;
            foreach (var action in preflops)
            {
                if (wasSqueezeRaise)
                {
                    if (wasSqueezeCall)
                    {
                        if (action.PlayerName == squeezeStarter)
                            return;

                        if (action.PlayerName != player && action.IsRaise())
                            return;

                        if (action.PlayerName != player)
                            continue;

                        squeeze.Possible = true;
                        if (action.IsRaise())
                            squeeze.Made = true;
                    }
                    else
                    {
                        if (action.PlayerName == player)
                            return;

                        if (action.IsCall())
                            wasSqueezeCall = true;
                    }
                }
                else
                {
                    if (action.PlayerName == player)
                        return;

                    if (!action.IsRaise())
                        continue;

                    wasSqueezeRaise = true;
                    squeezeStarter = action.PlayerName;
                }
            }
        }

        private static void Calculate3Bet(ConditionalBet threeBet, IList<HandAction> actions, string player, string raiser)
        {
            bool start3Bet = false;
            foreach (var action in actions)
            {
                if (start3Bet)
                {
                    if (!threeBet.Happened)
                    {
                        if (action.PlayerName == player && action.PlayerName != raiser)
                        {
                            threeBet.Possible = true;
                        }

                        if (!action.IsRaise() || action.PlayerName == raiser)
                            continue;

                        threeBet.Happened = true;
                        if (action.PlayerName != player)
                            continue;
                        threeBet.Made = true;
                        return;
                    }

                    if (action.PlayerName == player && player == raiser)
                    {
                        if (threeBet.CheckAction(action))
                            return;
                    }
                }
                else
                {
                    if (action.PlayerName == player)
                    {
                        if (action.IsCall() || action.HandActionType == HandActionType.CHECK)
                            return;
                    }

                    if (action.IsRaise() && action.PlayerName == raiser)
                    {
                        start3Bet = true;
                    }
                }
            }
        }

        private static void CalculateContinuationBet(ConditionalBet cbet, IList<HandAction> actions, string player, string raiser)
        {
            foreach (var action in actions)
            {
                if (action.PlayerName == player)
                {
                    if (raiser == player)
                    {
                        cbet.Possible = true;

                        if (!action.IsBet())
                            return;

                        cbet.Made = true;
                        cbet.Happened = true;

                        return;
                    }
                }
                else
                {
                    if (action.IsBet())
                    {
                        if (raiser == action.PlayerName)
                            cbet.Happened = true;
                        else
                            return;
                    }
                }

                if (!cbet.Happened)
                    continue;

                if (action.PlayerName == player)
                {
                    if (cbet.CheckAction(action))
                        return;
                }
                else
                {
                    if (action.IsRaise()) // someone else raised cbet before player could act
                        return;
                }
            }
        }

        private static void CalculateLimpBet(ConditionalBet limp, IList<HandAction> preflops, string player)
        {
            if (preflops.Where(x => x.IsBlinds).Any(x => x.PlayerName == player))
            {
                return;
            }

            foreach (var action in preflops.Where(x => !x.IsBlinds))
            {
                if (limp.Made)
                {
                    if (action.PlayerName == player)
                    {
                        limp.CheckAction(action);
                        return;
                    }
                    continue;
                }

                if (action.PlayerName == player)
                {
                    limp.Possible = true;
                    if (action.IsCall())
                    {
                        limp.Made = true;
                    }
                    else
                    {
                        return;
                    }
                }

                if (action.IsRaise())
                {
                    return;
                }
            }
        }

        private static Dictionary<Street, bool> GetCheckRaisedOnStreets(IList<HandAction> actions, string player)
        {
            Dictionary<Street, bool> _checkedRaisedOnStreetDictionary = new Dictionary<Street, bool>()
            {
                { Street.Flop, false },
                { Street.Turn, false },
                { Street.River, false },
            };

            foreach (var group in actions.Where(x => x.PlayerName == player).GroupBy(x => x.Street).Where(x => x.Key >= Street.Flop && x.Key <= Street.River && x.Count() > 1))
            {
                var checkAction = group.FirstOrDefault(x => x.HandActionType == HandActionType.CHECK);
                var raiseAction = group.FirstOrDefault(x => x.IsRaise());

                if (checkAction != null && raiseAction != null)
                {
                    if (_checkedRaisedOnStreetDictionary.ContainsKey(group.Key))
                    {
                        _checkedRaisedOnStreetDictionary[group.Key] = true;
                    }
                    else
                    {
                        LogProvider.Log.Warn(typeof(PlayerStatisticCalculator), String.Format("GetCheckRaisedOnStreets: failed to add check raise action on street: {0}", group.Key));
                    }
                }
            }

            return _checkedRaisedOnStreetDictionary;
        }

        private static bool IsOpenRaise(IList<HandAction> preflops, string player)
        {
            var playerAction = preflops.Where(x => !x.IsBlinds).FirstOrDefault();
            if (playerAction != null && playerAction.PlayerName == player)
            {
                return playerAction.IsRaise();
            }
            return false;
        }

        private static bool IsRaisedLimpers(IList<HandAction> preflops, string player)
        {
            var firstPlayerAction = preflops.Where(x => !x.IsBlinds).FirstOrDefault(x => x.PlayerName == player);
            if (firstPlayerAction != null && firstPlayerAction.IsRaise())
            {
                var blinds = preflops.Where(x => x.IsBlinds).Select(x => x.PlayerName).Distinct();
                var limpers = preflops.Take(preflops.IndexOf(firstPlayerAction)).Where(x => !x.IsBlinds);
                if (limpers.Any() && !limpers.Any(x => x.IsRaise()) && limpers.Any(x => x.IsCall() && !blinds.Contains(x.PlayerName)))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsRelativePosition(IList<HandAction> actions, string player)
        {
            foreach (var group in actions.GroupBy(x => x.Street).Where(x => x.Any(a => a.IsRaise() || a.IsBet())))
            {
                var lastPlayer = group.LastOrDefault();
                if (lastPlayer != null && lastPlayer.PlayerName == player)
                    return true;
            }
            return false;
        }

        private static bool IsRelative3BetPosition(IList<HandAction> actions, string player)
        {
            foreach (var group in actions.GroupBy(x => x.Street))
            {
                if (IsRelativePosition(group.ToList(), player))
                {
                    ConditionalBet threeBet = new ConditionalBet();

                    var raiser = group.FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);
                    if (raiser != null)
                    {
                        Calculate3Bet(threeBet, group.ToList(), player, raiser.PlayerName);
                        if (threeBet.Happened)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static Player GetCutOffPlayer(HandHistory hand)
        {
            if (hand.Players.Count == 2)
                return null;
            var players = hand.Players.ToList();
            var smallBlind = hand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND);
            if (smallBlind != null)
                players.Remove(players.FirstOrDefault(x => x.PlayerName == smallBlind.PlayerName));

            var buttonPlayer = hand.Players.FirstOrDefault(x => x.SeatNumber == hand.DealerButtonPosition);
            int buttonIndex = buttonPlayer != null ? players.IndexOf(buttonPlayer) : 0;

            int cutoffIndex = (buttonIndex == 0 ? players.Count : buttonIndex) - 1;

            return players[cutoffIndex];
        }

        private static Player GetDealerPlayer(HandHistory hand)
        {
            var buttonPlayer = hand.Players.FirstOrDefault(x => x.SeatNumber == hand.DealerButtonPosition);
            return buttonPlayer;
        }

        private static Player GetInPositionPlayer(HandHistory hand, Street street)
        {
            var actions = hand.HandActions.Where(x => x.Street == street && !string.IsNullOrWhiteSpace(x.PlayerName)).ToList();
            var buttonPlayer = hand.Players.FirstOrDefault(x => x.SeatNumber == hand.DealerButtonPosition);
            int buttonIndex = hand.Players.ToList().IndexOf(buttonPlayer);

            int shift = hand.Players.Count - 1 - buttonIndex;
            for (int i = hand.Players.Count - 1; i >= 0; i--)
            {
                int index = i - shift;
                if (index < 0)
                    index += (hand.Players.Count);

                var player = hand.Players[index];
                var action = actions.LastOrDefault(x => x.PlayerName == player.PlayerName);
                if (action != null && action.HandActionType != HandActionType.FOLD)
                    return player;
            }

            return null;
        }

        private static void CalculateDonkBet(Condition donkBet, IList<HandAction> actions, string player)
        {
            var raisers = actions.PreFlopWhere(x => x.IsRaise());
            if (raisers.Any(x => x.PlayerName == player) || !raisers.Any())
            {
                return;
            }

            foreach (var action in actions.Street(Street.Flop))
            {
                if (raisers.Any(x => x.PlayerName == action.PlayerName))
                {
                    return;
                }

                if (action.PlayerName == player)
                {
                    donkBet.Possible = true;
                    donkBet.Made = action.IsBet();
                }

                // somebody else did a donk bet
                if (action.IsBet())
                {
                    return;
                }
            }
        }

        #endregion
    }
}