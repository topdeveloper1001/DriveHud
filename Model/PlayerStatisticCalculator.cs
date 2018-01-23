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

using DriveHUD.Common.Linq;
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
        public Playerstatistic CalculateStatistic(ParsingResult result, Players u, Dictionary<string, Dictionary<Street, decimal>> calculatedEquity)
        {
            HandHistory parsedHand = result.Source;
            var player = u.Playername;

            Playerstatistic stat = new Playerstatistic
            {
                PlayerName = player,
                Numberofplayers = (short)parsedHand.NumPlayersActive,
                PlayerId = u.PlayerId,
                GametypeId = (short)result.GameType.GametypeId,
                MaxPlayers = result.GameType.Tablesize
            };

            var currentPlayer = parsedHand.Players.FirstOrDefault(x => string.Equals(x.PlayerName, player, StringComparison.OrdinalIgnoreCase));
            stat.StartingStack = currentPlayer.StartingStack;
            stat.CurrencyId = (short)parsedHand.GameDescription.Limit.Currency;
            stat.PokergametypeId = (short)(parsedHand.GameDescription.GameType);

            HandAction[] playerHandActions = parsedHand.HandActions.Where(x => x.PlayerName == player).ToArray();

            int call = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street > Street.Preflop);
            int bet = playerHandActions.Count(handAction => handAction.IsBet() && handAction.Street > Street.Preflop);
            int raises = playerHandActions.Count(handAction => handAction.IsRaise() && handAction.Street > Street.Preflop);

            var isWalkHand = parsedHand.HandActions.All(x => !x.IsBet() && !x.IsCall() && !x.IsRaise());

            stat.TotalbetsFlop = playerHandActions.Count(handAction => (handAction.IsBet() || handAction.IsRaise()) && handAction.Street == Street.Flop);
            stat.TotalbetsTurn = playerHandActions.Count(handAction => (handAction.IsBet() || handAction.IsRaise()) && handAction.Street == Street.Turn);
            stat.TotalbetsRiver = playerHandActions.Count(handAction => (handAction.IsBet() || handAction.IsRaise()) && handAction.Street == Street.River);

            stat.TotalcallsFlop = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street == Street.Flop);
            stat.TotalcallsTurn = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street == Street.Turn);
            stat.TotalcallsRiver = playerHandActions.Count(handAction => handAction.IsCall() && handAction.Street == Street.River);

            bool playerFolded = playerHandActions.Any(x => x.IsFold);

            bool wasRiver = CardHelper.IsStreetAvailable(parsedHand.CommunityCards, Street.River);
            bool wasTurn = wasRiver || CardHelper.IsStreetAvailable(parsedHand.CommunityCards, Street.Turn);
            bool wasFlop = wasTurn || CardHelper.IsStreetAvailable(parsedHand.CommunityCards, Street.Flop);

            bool playedRiver = playerHandActions.Any(x => x.Street == Street.River) || (wasRiver && !playerFolded);
            bool playedTurn = playedRiver || playerHandActions.Any(x => x.Street == Street.Turn) || (wasTurn && !playerFolded);
            bool playedFlop = playedTurn || playerHandActions.Any(x => x.Street == Street.Flop) || (wasFlop && !playerFolded);

            decimal netWon = playerHandActions.Sum(x => x.Amount);

            bool won = playerHandActions.Any(handAction => handAction.IsWinningsAction) && netWon > 0;

            bool sawShowDown = !playerFolded &&
                parsedHand.HandActions
                .GroupBy(x => x.PlayerName)
                .Where(x => x.Key != player)
                .Any(x => x.All(p => !p.IsFold)) && playedFlop;

            bool wonShowdown = sawShowDown && won;

            bool wonFlop = playedFlop && won;
            bool wonTurn = playedTurn && won;
            bool wonRiver = playedRiver && won;

            bool vpip = playerHandActions.PreFlopAny(handAction => handAction.IsRaise() || handAction.IsCall());
            bool pfr = playerHandActions.PreFlopAny(handAction => handAction.IsRaise());
            bool pfrOcurred = parsedHand.PreFlop.Any(handAction => handAction.HandActionType == HandActionType.RAISE);
            int pfrRaisers = parsedHand.PreFlop.Where(x => x.HandActionType == HandActionType.RAISE).Count();

            var preflops = parsedHand.PreFlop.ToList();

            bool aggresiveFlop = playerHandActions.FlopAny(handAction => handAction.IsRaise() || handAction.IsBet());
            bool aggresiveTurn = playerHandActions.TurnAny(handAction => handAction.IsRaise() || handAction.IsBet());
            bool aggresiveRiver = playerHandActions.RiverAny(handAction => handAction.IsRaise() || handAction.IsBet());

            var betOnFlopAction = playerHandActions.FirstOrDefault(x => x.Street == Street.Flop && x.IsBet());
            var betOnTurnAction = playerHandActions.FirstOrDefault(x => x.Street == Street.Turn && x.IsBet());

            bool betOnFlop = betOnFlopAction != null;
            bool betOnTurn = betOnTurnAction != null;

            bool isCheckedFlop = playerHandActions.FirstOrDefault(x => x.Street == Street.Flop)?.IsCheck ?? false;
            bool isCheckedTurn = playerHandActions.FirstOrDefault(x => x.Street == Street.Turn)?.IsCheck ?? false;
            bool isFoldedFlop = playerHandActions.FlopAny(a => a.IsFold);

            var positionFlopPlayer = GetInPositionPlayer(parsedHand, Street.Preflop);
            var preflopInPosition = positionFlopPlayer != null && positionFlopPlayer.PlayerName == player && playedFlop;

            bool isBluffPreflop = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.Preflop);
            bool isBluffFlop = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.Flop);
            bool isBluffTurn = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.Turn);
            bool isBluffRiver = IsBluff(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.River);

            bool isMonotonePreflop = IsMonotone(parsedHand.CommunityCards, Street.Flop);
            bool isRagPreflop = IsRag(currentPlayer.HoleCards, parsedHand.CommunityCards, Street.Preflop);

            Player cutoff = GetCutOffPlayer(parsedHand);
            Player dealer = GetDealerPlayer(parsedHand);

            var numberOfActivePlayerOnFlop = parsedHand.NumPlayersActive - parsedHand.PreFlop.Count(x => x.IsFold);

            #region C-Bet

            var flopCBet = new ConditionalBet();
            var turnCBet = new ConditionalBet();
            var riverCBet = new ConditionalBet();

            if (pfrOcurred)
            {
                var wasAllIn = parsedHand.PreFlop.OfType<AllInAction>().Any(x => x.PlayerName != player);

                string raiser = parsedHand.PreFlop.Last(x => x.IsRaise()).PlayerName;

                if (wasFlop && !wasAllIn)
                {
                    var flops = parsedHand.Flop.ToList();

                    wasAllIn = flops.OfType<AllInAction>().Any(x => x.PlayerName != player);

                    CalculateContinuationBet(flopCBet, flops, player, raiser);

                    var wasReraise = flops.Any(x => (!flopCBet.Made && x.IsRaise()) || (x.PlayerName != player && x.IsRaise()));

                    if (flopCBet.Happened && wasTurn && !wasAllIn && !wasReraise)
                    {
                        var turns = parsedHand.Turn.ToList();

                        wasAllIn = turns.OfType<AllInAction>().Any(x => x.PlayerName != player);

                        CalculateContinuationBet(turnCBet, turns, player, raiser);

                        wasReraise = turns.Any(x => (!turnCBet.Made && x.IsRaise()) || (x.PlayerName != player && x.IsRaise()));

                        if (turnCBet.Happened && wasRiver && !wasAllIn && !wasReraise)
                        {
                            var rivers = parsedHand.River.ToList();
                            CalculateContinuationBet(riverCBet, rivers, player, raiser);
                        }
                    }
                }
            }

            var turnIpPassFlopCbet = new ConditionalBet();

            var positionturnPlayer = GetInPositionPlayer(parsedHand, Street.Flop);

            var flopInPosition = positionturnPlayer != null && positionturnPlayer.PlayerName == player;

            if (positionturnPlayer != null && positionturnPlayer.PlayerName == player && flopCBet.Passed)
            {
                var action = parsedHand.Turn.FirstOrDefault(x => x.PlayerName == player);

                if (action != null)
                {
                    turnIpPassFlopCbet.CheckAction(action);
                }
            }

            var riverIpPassFlopCbet = new ConditionalBet();

            var positionRiverPlayer = GetInPositionPlayer(parsedHand, Street.Turn);

            if (positionRiverPlayer != null && positionRiverPlayer.PlayerName == player && turnCBet.Passed)
            {
                var action = parsedHand.River.FirstOrDefault(x => x.PlayerName == player);

                if (action != null)
                {
                    riverIpPassFlopCbet.CheckAction(action);
                }
            }

            #endregion

            #region Float

            bool isFloatFlop = flopCBet.Called && currentPlayer.hasHoleCards && isBluffFlop;

            #endregion

            #region 3bet/4bet/5bet

            var threeBet = new ConditionalBet();
            var fourBet = new ConditionalBet();
            var fiveBet = new ConditionalBet();

            if (pfrOcurred)
            {
                var raiser = preflops.FirstOrDefault(x => x.HandActionType == HandActionType.RAISE).PlayerName;

                Calculate3Bet(threeBet, preflops, player, raiser);
                Calculate4Bet(fourBet, preflops, player, raiser, parsedHand.Players);
                Calculate5Bet(threeBet, fourBet, fiveBet, preflops, player, raiser);
            }

            #endregion

            #region 2 preflop raisers
            ConditionalBet twoPfr = new ConditionalBet();

            if (pfrOcurred)
                Calculate2PreflopRaisers(twoPfr, preflops, player);

            #endregion

            #region Steal

            StealAttempt stealAttempt = new StealAttempt();

            // First big blind action is from player who is actually on BB spot, next actions are from players that joined game and payer BB
            var isBigBlind = parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND)?.PlayerName == player;
            var isSmallBlind = parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND)?.PlayerName == player;
            var isDealer = GetDealerPlayer(parsedHand)?.PlayerName == player;

            CalculateSteal(stealAttempt, parsedHand, player, isBigBlind || isSmallBlind);

            #endregion

            #region Cold Call

            var coldCall = new Condition();
            if (pfrOcurred)
                CalculateColdCall(coldCall, preflops, player);

            ConditionalBet coldCall3Bet = new ConditionalBet();
            CalculateColdCall3Bet(coldCall3Bet, preflops, player);

            ConditionalBet coldCall4Bet = new ConditionalBet();
            CalculateColdCall4Bet(coldCall4Bet, preflops, player);

            //calculation of cold call after open raise at BTN position
            ConditionalBet coldCallVsBtnOpen = new ConditionalBet();
            if (dealer != null)
                ColdCallafterPositionalOpenRaise(coldCallVsBtnOpen, preflops, player, dealer.PlayerName);

            //calculation of cold call after open raise at Sb position
            ConditionalBet coldCallVsSbOpen = new ConditionalBet();
            if (parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND) != null)
                ColdCallafterPositionalOpenRaise(coldCallVsSbOpen, preflops, player, parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND).PlayerName);

            //calculation of cold call after open raise at Co position
            ConditionalBet coldCallVsCoOpen = new ConditionalBet();
            if (cutoff != null)
                ColdCallafterPositionalOpenRaise(coldCallVsCoOpen, preflops, player, cutoff.PlayerName);

            #endregion

            #region Squeeze

            var squeezBet = new ConditionalBet();
            if (pfrOcurred)
                CalculateSqueeze(squeezBet, preflops, player);

            #endregion

            #region Raise

            var flopRaise = playedFlop ? CalculateRaise(parsedHand.Flop, player, parsedHand.Players) : new ConditionalBet();
            var turnRaise = playedTurn ? CalculateRaise(parsedHand.Turn, player, parsedHand.Players) : new ConditionalBet();
            var riverRaise = playedRiver ? CalculateRaise(parsedHand.River, player, parsedHand.Players) : new ConditionalBet();

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

            #region Bet when checked to 

            Condition flopBetWhenCheckedTo = new Condition();
            Condition turnBetWhenCheckedTo = new Condition();
            Condition riverBetWhenCheckedTo = new Condition();

            CalculateBetWhenCheckedTo(flopBetWhenCheckedTo, parsedHand.Flop, player);
            CalculateBetWhenCheckedTo(turnBetWhenCheckedTo, parsedHand.Turn, player);
            CalculateBetWhenCheckedTo(riverBetWhenCheckedTo, parsedHand.River, player);

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


            stat.SmallBlind = Math.Abs(parsedHand.GameDescription.Limit.SmallBlind);
            stat.BigBlind = Math.Abs(parsedHand.GameDescription.Limit.BigBlind);
            stat.Ante = 0;
            if (parsedHand.GameDescription.Limit.IsAnteTable)
            {
                stat.Ante = Math.Abs(parsedHand.GameDescription.Limit.Ante);
            }
            stat.IsSmallBlind = isSmallBlind;
            stat.IsBigBlind = isBigBlind;
            stat.IsCutoff = cutoff != null && cutoff.PlayerName == player;
            stat.IsDealer = dealer != null && dealer.PlayerName == player;

            bool isOpenRaise = IsOpenRaise(preflops, player);
            bool isRaisedLimpers = IsRaisedLimpers(preflops, player);

            var flopCheckRaise = CalculateCheckRaise(parsedHand.Flop, player, parsedHand.Players);
            var turnCheckRaise = CalculateCheckRaise(parsedHand.Turn, player, parsedHand.Players);
            var riverCheckRaise = CalculateCheckRaise(parsedHand.River, player, parsedHand.Players);

            stat.IsRelativePosition = IsRelativePosition(parsedHand.HandActions, player);
            stat.IsRelative3BetPosition = IsRelative3BetPosition(parsedHand.HandActions, player);

            #region True Aggression

            int flopTrueAggressionBets = flopCheckRaise.Made ? 2 : ((aggresiveFlop && !flopCBet.Made) ? 1 : 0);
            int turnTrueAggressionBets = turnCheckRaise.Made ? 2 : (aggresiveTurn ? 1 : 0);
            int riverTrueAggressionBets = riverCheckRaise.Made ? 2 : (aggresiveRiver ? 1 : 0);

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
            stat.PfrOop = pfr && !preflopInPosition ? 1 : 0;

            stat.DidDonkBet = donkBet.Made ? 1 : 0;
            stat.CouldDonkBet = donkBet.Possible ? 1 : 0;

            stat.Didthreebet = threeBet.Made ? 1 : 0;
            stat.DidThreeBetIp = threeBet.Made && preflopInPosition ? 1 : 0;
            stat.CouldThreeBetIp = threeBet.Possible && preflopInPosition ? 1 : 0;
            stat.DidThreeBetOop = threeBet.Made && !preflopInPosition ? 1 : 0;
            stat.CouldThreeBetOop = threeBet.Possible && !preflopInPosition ? 1 : 0;
            stat.Couldthreebet = threeBet.Possible ? 1 : 0;
            stat.Facedthreebetpreflop = threeBet.Faced ? 1 : 0;
            stat.Foldedtothreebetpreflop = threeBet.Folded ? 1 : 0;
            stat.Calledthreebetpreflop = threeBet.Called ? 1 : 0;
            stat.Raisedthreebetpreflop = threeBet.Raised ? 1 : 0;

            stat.Totalhands = 1;
            stat.Totalcalls = call;
            stat.Totalbets = bet + raises;
            stat.NumberOfWalks = isWalkHand && won ? 1 : 0;
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

            if (cutoff != null)
            {
                var cutoffAction = preflops
                    .Where(x => x.PlayerName == cutoff.PlayerName && x.HandActionType != HandActionType.ANTE)
                    .FirstOrDefault();

                var cutoffRaised = cutoffAction != null ? cutoffAction.IsRaise() : false;

                stat.Buttonstealfaced = isDealer && cutoffRaised ? 1 : 0;
                stat.Buttonstealdefended = isDealer && cutoffRaised && vpip ? 1 : 0;
                stat.Buttonstealfolded = isDealer && cutoffRaised && !vpip ? 1 : 0;
                stat.Buttonstealreraised = isDealer && cutoffRaised && pfr ? 1 : 0;
            }

            stat.Bigblindstealfaced = isBigBlind && stealAttempt.Faced ? 1 : 0;
            stat.Bigblindstealdefended = isBigBlind && stealAttempt.Defended ? 1 : 0;
            stat.Bigblindstealfolded = isBigBlind && stealAttempt.Folded ? 1 : 0;
            stat.Bigblindstealreraised = isBigBlind && stealAttempt.Raised ? 1 : 0;

            stat.Smallblindstealfaced = isSmallBlind && stealAttempt.Faced ? 1 : 0;
            stat.Smallblindstealattempted = isSmallBlind && stealAttempt.Attempted ? 1 : 0;
            stat.Smallblindstealdefended = isSmallBlind && stealAttempt.Defended ? 1 : 0;
            stat.Smallblindstealfolded = isSmallBlind && stealAttempt.Folded ? 1 : 0;
            stat.Smallblindstealreraised = isSmallBlind && stealAttempt.Raised ? 1 : 0;

            stat.BlindsStealDefended = stealAttempt.Defended && (isSmallBlind || isBigBlind) ? 1 : 0;

            stat.Flopcontinuationbetpossible = flopCBet.Possible ? 1 : 0;
            stat.Flopcontinuationbetmade = flopCBet.Made ? 1 : 0;
            stat.Flopcontinuationipbetmade = flopCBet.Made && preflopInPosition ? 1 : 0;
            stat.Flopcontinuationipbetpossible = flopCBet.Possible && preflopInPosition ? 1 : 0;
            stat.Flopcontinuationoopbetmade = flopCBet.Made && !preflopInPosition ? 1 : 0;
            stat.Flopcontinuationoopbetpossible = flopCBet.Possible && !preflopInPosition ? 1 : 0;
            stat.FlopContinuationBetInThreeBetPotPossible = flopCBet.Possible && threeBet.Made ? 1 : 0;
            stat.FlopContinuationBetInThreeBetPotMade = flopCBet.Made && threeBet.Made ? 1 : 0;
            stat.FlopContinuationBetInFourBetPotPossible = flopCBet.Possible && fourBet.Made ? 1 : 0;
            stat.FlopContinuationBetInFourBetPotMade = flopCBet.Made && fourBet.Made ? 1 : 0;
            stat.FlopContinuationBetVsOneOpponentPossible = flopCBet.Possible && (numberOfActivePlayerOnFlop == 2) ? 1 : 0;
            stat.FlopContinuationBetVsOneOpponentMade = flopCBet.Made && (numberOfActivePlayerOnFlop == 2) ? 1 : 0;
            stat.FlopContinuationBetVsTwoOpponentsPossible = flopCBet.Possible && (numberOfActivePlayerOnFlop == 3) ? 1 : 0;
            stat.FlopContinuationBetVsTwoOpponentsMade = flopCBet.Made && (numberOfActivePlayerOnFlop == 3) ? 1 : 0;
            stat.MultiWayFlopContinuationBetPossible = flopCBet.Possible && (numberOfActivePlayerOnFlop > 2) ? 1 : 0;
            stat.MultiWayFlopContinuationBetMade = flopCBet.Made && (numberOfActivePlayerOnFlop > 2) ? 1 : 0;
            stat.FlopContinuationBetMonotonePotPossible = flopCBet.Possible && isMonotonePreflop ? 1 : 0;
            stat.FlopContinuationBetMonotonePotMade = flopCBet.Made && isMonotonePreflop ? 1 : 0;
            stat.FlopContinuationBetRagPotPossible = flopCBet.Possible && isRagPreflop ? 1 : 0;
            stat.FlopContinuationBetRagPotMade = flopCBet.Made && isRagPreflop ? 1 : 0;

            if (pfrRaisers == 2)
            {
                stat.FacingFlopContinuationBetFromThreeBetPot = flopCBet.Faced ? 1 : 0;
                stat.FoldedToFlopContinuationBetFromThreeBetPot = flopCBet.Folded ? 1 : 0;
                stat.CalledFlopContinuationBetFromThreeBetPot = flopCBet.Called ? 1 : 0;
                stat.RaisedFlopContinuationBetFromThreeBetPot = flopCBet.Raised ? 1 : 0;
            }

            if (pfrRaisers == 3)
            {
                stat.FacingFlopContinuationBetFromFourBetPot = flopCBet.Faced ? 1 : 0;
                stat.FoldedToFlopContinuationBetFromFourBetPot = flopCBet.Folded ? 1 : 0;
                stat.CalledFlopContinuationBetFromFourBetPot = flopCBet.Called ? 1 : 0;
                stat.RaisedFlopContinuationBetFromFourBetPot = flopCBet.Raised ? 1 : 0;
            }

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

            stat.FacingflopcontinuationbetIP = flopCBet.Faced && preflopInPosition ? 1 : 0;
            stat.FacingflopcontinuationbetOOP = flopCBet.Faced && !preflopInPosition ? 1 : 0;
            stat.CalledflopcontinuationbetIP = flopCBet.Called && preflopInPosition ? 1 : 0;
            stat.CalledflopcontinuationbetOOP = flopCBet.Called && !preflopInPosition ? 1 : 0;
            stat.FoldToFlopcontinuationbetIP = flopCBet.Folded && preflopInPosition ? 1 : 0;
            stat.FoldToFlopcontinuationbetOOP = flopCBet.Folded && !preflopInPosition ? 1 : 0;

            stat.Couldfourbet = fourBet.Possible ? 1 : 0;
            stat.Didfourbet = fourBet.Made ? 1 : 0;
            stat.Facedfourbetpreflop = fourBet.Faced ? 1 : 0;
            stat.Foldedtofourbetpreflop = fourBet.Folded ? 1 : 0;
            stat.Calledfourbetpreflop = fourBet.Called ? 1 : 0;
            stat.Raisedfourbetpreflop = fourBet.Raised ? 1 : 0;

            stat.Could5Bet = fiveBet.Possible ? 1 : 0;
            stat.Did5Bet = fiveBet.Made ? 1 : 0;

            stat.Facingtwopreflopraisers = twoPfr.Faced ? 1 : 0;
            stat.Calledtwopreflopraisers = twoPfr.Called ? 1 : 0;
            stat.Raisedtwopreflopraisers = twoPfr.Raised ? 1 : 0;

            stat.Turnfoldippassonflopcb = turnIpPassFlopCbet.Folded ? 1 : 0;
            stat.Turncallippassonflopcb = turnIpPassFlopCbet.Called ? 1 : 0;
            stat.Turnraiseippassonflopcb = turnIpPassFlopCbet.Raised ? 1 : 0;
            stat.Riverfoldippassonturncb = riverIpPassFlopCbet.Folded ? 1 : 0;
            stat.Rivercallippassonturncb = riverIpPassFlopCbet.Called ? 1 : 0;
            stat.Riverraiseippassonturncb = riverIpPassFlopCbet.Raised ? 1 : 0;

            stat.CheckedRiverAfterBBLine = betOnFlop && betOnTurn && playerHandActions.Any(x => x.Street == Street.River && x.IsCheck) ? 1 : 0;
            stat.CouldCheckRiverAfterBBLine = playedRiver && betOnFlop && betOnTurn && parsedHand.River.TakeWhile(x => x.PlayerName != player).All(x => x.IsCheck || x.IsFold) ? 1 : 0;

            var checkOnTurn = playerHandActions.TurnAny(x => x.IsCheck);

            stat.DidRiverBet = playerHandActions.RiverAny(x => x.IsBet()) ? 1 : 0;
            stat.CouldRiverBet = parsedHand.River.TakeWhile(x => x.PlayerName != player).All(x => x.IsCheck || x.IsFold) ? 1 : 0;
            stat.DidBetRiverOnBXLine = betOnFlop && checkOnTurn && stat.DidRiverBet == 1 ? 1 : 0;
            stat.CouldBetRiverOnBXLine = betOnFlop && checkOnTurn && stat.CouldRiverBet == 1 ? 1 : 0;

            stat.Playedyearandmonth = int.Parse(parsedHand.DateOfHandUtc.ToString("yyyyMM"));

            stat.Couldsqueeze = squeezBet.Possible ? 1 : 0;
            stat.Didsqueeze = squeezBet.Made ? 1 : 0;

            stat.DidOpenRaise = isOpenRaise ? 1 : 0;
            stat.IsRaisedLimpers = isRaisedLimpers ? 1 : 0;
            stat.DidCheckRaise = flopCheckRaise.Made || turnCheckRaise.Made || riverCheckRaise.Made ? 1 : 0;
            stat.DidFlopCheckRaise = flopCheckRaise.Made ? 1 : 0;
            stat.DidTurnCheckRaise = turnCheckRaise.Made ? 1 : 0;
            stat.DidRiverCheckRaise = riverCheckRaise.Made ? 1 : 0;
            stat.CouldFlopCheckRaise = flopCheckRaise.Possible ? 1 : 0;
            stat.CouldTurnCheckRaise = turnCheckRaise.Possible ? 1 : 0;
            stat.CouldRiverCheckRaise = riverCheckRaise.Possible ? 1 : 0;
            stat.FacedFlopCheckRaise = flopCheckRaise.Faced ? 1 : 0;
            stat.FoldedToFlopCheckRaise = flopCheckRaise.Folded ? 1 : 0;
            stat.FacedTurnCheckRaise = turnCheckRaise.Faced ? 1 : 0;
            stat.CalledTurnCheckRaise = turnCheckRaise.Called ? 1 : 0;
            stat.FoldedToTurnCheckRaise = turnCheckRaise.Folded ? 1 : 0;
            stat.FacedRiverCheckRaise = riverCheckRaise.Faced ? 1 : 0;
            stat.FoldedToRiverCheckRaise = riverCheckRaise.Folded ? 1 : 0;

            stat.Couldcoldcall = coldCall.Possible ? 1 : 0;
            stat.Didcoldcall = coldCall.Made ? 1 : 0;
            stat.DidColdCallIp = coldCall.Made && preflopInPosition ? 1 : 0;
            stat.DidColdCallOop = coldCall.Made && !preflopInPosition ? 1 : 0;
            stat.DidColdCallThreeBet = coldCall3Bet.Made ? 1 : 0;
            stat.CouldColdCallThreeBet = coldCall3Bet.Possible ? 1 : 0;
            stat.DidColdCallFourBet = coldCall4Bet.Made ? 1 : 0;
            stat.CouldColdCallFourBet = coldCall4Bet.Possible ? 1 : 0;
            stat.DidColdCallVsOpenRaiseBtn = coldCallVsBtnOpen.Made ? 1 : 0;
            stat.CouldColdCallVsOpenRaiseBtn = coldCallVsBtnOpen.Possible ? 1 : 0;
            stat.DidColdCallVsOpenRaiseSb = coldCallVsSbOpen.Made ? 1 : 0;
            stat.CouldColdCallVsOpenRaiseSb = coldCallVsSbOpen.Possible ? 1 : 0;
            stat.DidColdCallVsOpenRaiseCo = coldCallVsCoOpen.Made ? 1 : 0;
            stat.CouldColdCallVsOpenRaiseCo = coldCallVsCoOpen.Possible ? 1 : 0;

            stat.DidDelayedTurnCBet = flopCBet.Possible && !flopCBet.Made && betOnTurn ? 1 : 0;

            var couldBetOnFlop = parsedHand.Flop != null &&
                ((parsedHand.Flop.FirstOrDefault()?.PlayerName == player) ||
                parsedHand.Flop.Any(x => x.PlayerName == player && (x.IsBet() || x.IsCheck)));

            var couldBetOnTurn = parsedHand.Turn != null &&
                ((parsedHand.Turn.FirstOrDefault()?.PlayerName == player) ||
                parsedHand.Turn.Any(x => x.PlayerName == player && (x.IsBet() || x.IsCheck)));

            stat.CouldDelayedTurnCBet = flopCBet.Possible && !flopCBet.Made && couldBetOnTurn ? 1 : 0;

            stat.DidDelayedTurnCBetIn3BetPot = pfr && flopCBet.Possible && !flopCBet.Made && betOnTurn ? 1 : 0;
            stat.CouldDelayedTurnCBetIn3BetPot = pfr && flopCBet.Possible && !flopCBet.Made && couldBetOnTurn ? 1 : 0;

            stat.PlayedFloatFlop = isFloatFlop ? 1 : 0;

            stat.CouldRaiseFlop = flopRaise.Possible ? 1 : 0;
            stat.DidRaiseFlop = flopRaise.Made ? 1 : 0;
            stat.CouldRaiseTurn = turnRaise.Possible ? 1 : 0;
            stat.DidRaiseTurn = turnRaise.Made ? 1 : 0;
            stat.CouldRaiseRiver = riverRaise.Possible ? 1 : 0;
            stat.DidRaiseRiver = riverRaise.Made ? 1 : 0;

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

            stat.BetFoldFlopPfrRaiser = pfr && betOnFlop && isFoldedFlop ? 1 : 0;
            stat.CouldBetFoldFlopPfrRaiser = pfr && couldBetOnFlop ? 1 : 0;
            stat.CheckFoldFlopPfrOop = pfr && isCheckedFlop && isFoldedFlop && !preflopInPosition ? 1 : 0;
            stat.CheckFoldFlop3BetOop = isCheckedFlop && threeBet.Made && isFoldedFlop && !preflopInPosition ? 1 : 0;
            stat.BetFlopCalled3BetPreflopIp = betOnFlop && threeBet.Called && preflopInPosition ? 1 : 0;
            stat.CouldBetFlopCalled3BetPreflopIp = couldBetOnFlop && threeBet.Called && preflopInPosition ? 1 : 0;

            stat.FacedRaiseFlop = flopRaise.Faced ? 1 : 0;
            stat.FoldedFacedRaiseFlop = flopRaise.Folded ? 1 : 0;
            stat.CalledFacedRaiseFlop = flopRaise.Called ? 1 : 0;
            stat.ReraisedFacedRaiseFlop = flopRaise.Raised ? 1 : 0;

            stat.FacedRaiseTurn = turnRaise.Faced ? 1 : 0;
            stat.FoldedFacedRaiseTurn = turnRaise.Folded ? 1 : 0;
            stat.CalledFacedRaiseTurn = turnRaise.Called ? 1 : 0;
            stat.ReraisedFacedRaiseTurn = turnRaise.Raised ? 1 : 0;

            stat.FacedRaiseRiver = riverRaise.Faced ? 1 : 0;
            stat.FoldedFacedRaiseRiver = riverRaise.Folded ? 1 : 0;
            stat.CalledFacedRaiseRiver = riverRaise.Called ? 1 : 0;
            stat.ReraisedFacedRaiseRiver = riverRaise.Raised ? 1 : 0;

            stat.CanBetWhenCheckedToFlop = flopBetWhenCheckedTo.Possible ? 1 : 0;
            stat.DidBetWhenCheckedToFlop = flopBetWhenCheckedTo.Made ? 1 : 0;

            stat.CanBetWhenCheckedToTurn = turnBetWhenCheckedTo.Possible ? 1 : 0;
            stat.DidBetWhenCheckedToTurn = turnBetWhenCheckedTo.Made ? 1 : 0;

            stat.CanBetWhenCheckedToRiver = riverBetWhenCheckedTo.Possible ? 1 : 0;
            stat.DidBetWhenCheckedToRiver = riverBetWhenCheckedTo.Made ? 1 : 0;

            stat.FacedSqueez = squeezBet.Faced ? 1 : 0;
            stat.FoldedFacedSqueez = squeezBet.Folded ? 1 : 0;
            stat.CalledFacedSqueez = squeezBet.Called ? 1 : 0;
            stat.ReraisedFacedSqueez = squeezBet.Raised ? 1 : 0;

            #region Additional

            stat.GameNumber = parsedHand.HandId;
            stat.PokersiteId = result.HandHistory.PokersiteId;
            stat.TableType = parsedHand.GameDescription.TableType.ToString();
            stat.Time = parsedHand.DateOfHandUtc;
            stat.Cards = Converter.ToHoleCards(currentPlayer.HoleCards);

            stat.Pot = parsedHand.TotalPot ?? 0;
            stat.CalculateTotalPot();

            List<string> line = new List<string>();

            foreach (var streetLine in parsedHand.HandActions.Where(x => x.PlayerName == stat.PlayerName || x is StreetAction).GroupBy(x => x.Street))
            {
                line.Add(String.Join(string.Empty, streetLine.Select(Converter.ActionToString)));
            }

            stat.Line = string.Join(StringFormatter.ActionLineSeparator, line).Trim(StringFormatter.ActionLineSeparator.ToCharArray());

            stat.Board = parsedHand.CommunityCards != null ? parsedHand.CommunityCards.ToString() : string.Empty;
            stat.Allin = Converter.ToAllin(parsedHand, stat);
            stat.Action = Converter.ToAction(stat);
            stat.Position = Converter.ToPosition(parsedHand, stat);
            stat.PositionString = Converter.ToPositionString(stat.Position);
            stat.FacingPreflop = Converter.ToFacingPreflop(parsedHand.PreFlop, player);

            HandAction lastHeroActionStreetAction;

            if (Converter.CanAllInEquity(parsedHand, stat, out lastHeroActionStreetAction))
            {
                if (calculatedEquity != null && calculatedEquity.ContainsKey(player) && calculatedEquity[player].ContainsKey(lastHeroActionStreetAction.Street))
                {
                    stat.Equity = calculatedEquity[player][lastHeroActionStreetAction.Street];
                }
                else
                {
                    var equityByPlayer = Converter.CalculateAllInEquity(parsedHand, stat);

                    stat.Equity = equityByPlayer != null && equityByPlayer.ContainsKey(player) ? equityByPlayer[player] : 0;

                    if (equityByPlayer != null && calculatedEquity != null)
                    {
                        foreach (var equity in equityByPlayer)
                        {
                            if (!calculatedEquity.ContainsKey(equity.Key))
                            {
                                calculatedEquity.Add(equity.Key, new Dictionary<Street, decimal>());
                            }

                            if (!calculatedEquity[equity.Key].ContainsKey(lastHeroActionStreetAction.Street))
                            {
                                calculatedEquity[equity.Key].Add(lastHeroActionStreetAction.Street, equity.Value);
                            }
                        }
                    }
                }
            }

            if (stat.Equity > 0)
            {
                stat.EVDiff = GetExpectedValue(parsedHand, playerHandActions, stat.Equity, player) - netWon;
            }
            else
            {
                stat.EVDiff = 0;
            }

            if (parsedHand.GameDescription.Limit.SmallBlind > 0 && parsedHand.GameDescription.Limit.BigBlind > 0)
            {
                stat.GameType = string.Format("{0}/{1} - {2}", parsedHand.GameDescription.Limit.SmallBlind, parsedHand.GameDescription.Limit.BigBlind, parsedHand.GameDescription.GameType);
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

            stat.StealPossible = stealAttempt.Possible ? 1 : 0;
            stat.StealMade = stealAttempt.Attempted ? 1 : 0;

            stat.CouldThreeBetVsSteal = stealAttempt.Faced && threeBet.Possible ? 1 : 0;
            stat.DidThreeBetVsSteal = stealAttempt.Faced && threeBet.Made ? 1 : 0;

            stat.PlayerFolded = playerFolded;

            stat.CouldCheckRiverOnBXLine = checkRiverOnBxLine.Possible ? 1 : 0;
            stat.DidCheckRiverOnBXLine = checkRiverOnBxLine.Made ? 1 : 0;

            stat.TotalAggressiveBets = flopTrueAggressionBets + turnTrueAggressionBets + riverTrueAggressionBets;

            stat.FacedHandsUpOnFlop = playedFlop && (numberOfActivePlayerOnFlop == 2) ? 1 : 0;
            stat.FacedMultiWayOnFlop = playedFlop && (numberOfActivePlayerOnFlop > 2) ? 1 : 0;

            stat.StackInBBs = stat.StartingStack / stat.BigBlind;
            stat.MRatio = CalculateMRatio(stat);
            stat.CalculatePositionalStats();

            stat.PreflopIP = preflopInPosition ? 1 : 0;
            stat.PreflopOOP = !preflopInPosition ? 1 : 0;

            stat.FacedCheckRaiseVsFlopCBet = flopCBet.Raised && flopCheckRaise.Made &&
                parsedHand.Flop.SkipWhile(x => x.PlayerName == player && x.IsRaise()).Any(x => x.PlayerName != player && x.IsRaise()) ? 1 : 0;

            stat.CalledCheckRaiseVsFlopCBet = stat.FacedCheckRaiseVsFlopCBet == 1 && parsedHand.Flop.Last(x => x.PlayerName == player).IsCall() ? 1 : 0;
            stat.FoldedCheckRaiseVsFlopCBet = stat.FacedCheckRaiseVsFlopCBet == 1 && parsedHand.Flop.Last(x => x.PlayerName == player).IsFold ? 1 : 0;

            stat.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP = pfr && flopCBet.Possible && isCheckedFlop && !preflopInPosition &&
                isCheckedTurn && parsedHand.Turn.SkipWhile(x => !x.IsBet()).TakeWhile(x => x.PlayerName != player).All(x => !x.IsRaise(), true) ? 1 : 0;

            stat.CheckedCalledTurnWhenCheckedFlopAsPfr = stat.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP == 1 &&
                (playerHandActions.Where(x => x.Street == Street.Turn).Skip(1).FirstOrDefault()?.IsCall() ?? false) ? 1 : 0;

            stat.CheckedFoldedToTurnWhenCheckedFlopAsPfr = stat.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP == 1 &&
                (playerHandActions.Where(x => x.Street == Street.Turn).Skip(1).FirstOrDefault()?.IsFold ?? false) ? 1 : 0;

            var playerActionOnTurnBet = GetPlayerActionOnTurnBet(parsedHand.Turn, player);
            var facedTurnBetWhenCheckedFlopAsPfr = pfr && flopCBet.Possible && isCheckedFlop && playerActionOnTurnBet != null;

            stat.FacedTurnBetWhenCheckedFlopAsPfr = facedTurnBetWhenCheckedFlopAsPfr ? 1 : 0;
            stat.CalledTurnBetWhenCheckedFlopAsPfr = facedTurnBetWhenCheckedFlopAsPfr && playerActionOnTurnBet.IsCall() ? 1 : 0;
            stat.FoldedToTurnBetWhenCheckedFlopAsPfr = facedTurnBetWhenCheckedFlopAsPfr && playerActionOnTurnBet.IsFold ? 1 : 0;
            stat.RaisedTurnBetWhenCheckedFlopAsPfr = facedTurnBetWhenCheckedFlopAsPfr && playerActionOnTurnBet.IsRaise() ? 1 : 0;

            stat.FlopBetToPotRatio = betOnFlop ? Math.Abs(betOnFlopAction.Amount / parsedHand.PreFlop.Sum(x => x.Amount)) : 0;
            stat.TurnBetToPotRatio = betOnTurn ? Math.Abs(betOnTurnAction.Amount / parsedHand.HandActions
                .Where(x => x.Street == Street.Preflop || x.Street == Street.Flop)
                .Sum(x => x.Amount)) : 0;

            #endregion

            var lastFlopAction = parsedHand.Flop.LastOrDefault();

            if (lastFlopAction != null)
            {
                stat.DidFlopCheckBehind = playedTurn && lastFlopAction.PlayerName == player && lastFlopAction.IsCheck ? 1 : 0;
                stat.CouldFlopCheckBehind = preflopInPosition && !parsedHand.Flop.Any(x => x.IsBet() && x.PlayerName != player) ? 1 : 0;
            }

            return stat;
        }

        private static HandAction GetPlayerActionOnTurnBet(IEnumerable<HandAction> turnActions, string player, bool checkAllowed = false)
        {
            var wasBet = false;

            foreach (var action in turnActions)
            {
                if (action.IsBet())
                {
                    wasBet = true;

                    if (action.PlayerName == player)
                    {
                        return null;
                    }

                    continue;
                }

                if (!wasBet)
                {
                    if (!checkAllowed && action.PlayerName == player)
                    {
                        return null;
                    }

                    continue;
                }

                if (action.IsRaise() && action.PlayerName != player)
                {
                    return null;
                }

                if (action.PlayerName == player)
                {
                    return action;
                }
            }

            return null;
        }

        /// <summary>
        /// Calculate the check-raise based events for the specified player
        /// </summary>
        /// <param name="actions">The list of actions of the hand</param>
        /// <param name="player">The player for whom the check-raise events will be determined</param>
        /// <returns>The check-raise object</returns>
        private ConditionalBet CalculateCheckRaise(IEnumerable<HandAction> actions, string player, PlayerList players)
        {
            var checkRaise = new ConditionalBet();

            if (!actions.Any())
            {
                return checkRaise;
            }

            var checkedPlayers = new List<string>();
            var betIsAllIn = false;
            var heroMadeBet = false;

            foreach (var action in actions)
            {
                if (action.IsCheck)
                {
                    checkedPlayers.Add(action.PlayerName);
                    continue;
                }

                if (checkedPlayers.Count == 0)
                {
                    return checkRaise;
                }

                if (action.IsBet())
                {
                    if (player == action.PlayerName)
                    {
                        heroMadeBet = true;
                    }

                    // if player who made bets went all-in then we need to check if check-raise is possible for Hero
                    if (action.IsAllIn || action.IsAllInAction)
                    {
                        betIsAllIn = true;
                        continue;
                    }

                    checkRaise.Possible = checkedPlayers.Contains(player);
                }

                if (betIsAllIn && !checkRaise.Possible)
                {
                    // check-raise isn't possible because player can only call or fold because of all-in bet
                    if (action.PlayerName == player)
                    {
                        checkRaise.Possible = false;
                        break;
                    }

                    // if someone raise and his raise isn't all-in, then Hero still can make raise
                    if (action.IsRaise() && !action.IsAllIn && !action.IsAllInAction)
                    {
                        checkRaise.Possible = true;
                    }
                }

                if (action.IsRaise())
                {
                    if (checkedPlayers.Contains(action.PlayerName))
                    {
                        checkRaise.Happened = true;

                        if (string.IsNullOrEmpty(checkRaise.HappenedByPlayer))
                        {
                            checkRaise.HappenedByPlayer = action.PlayerName;
                        }

                        if (action.PlayerName == player)
                        {
                            checkRaise.Made = true;
                        }
                    }
                }

                if (heroMadeBet && checkRaise.Happened &&
                    action.PlayerName == player && checkRaise.HappenedByPlayer != player)
                {
                    checkRaise.CheckAction(action);
                    break;
                }
            }

            return checkRaise;
        }

        private static decimal GetExpectedValue(HandHistory parsedHand, HandAction[] playerHandActions, decimal equity, string player)
        {
            // potential win amount (might be reduced if hero got uncalled bet or went all-in)
            var canWin = parsedHand.WinningActions.Sum(x => x.Amount);
            var moneyInPot = playerHandActions.Where(x => x.Amount < 0).Sum(x => Math.Abs(x.Amount));

            if (playerHandActions.Any(x => x.HandActionType == HandActionType.UNCALLED_BET))
            {
                var uncalledBetAction = playerHandActions.FirstOrDefault(x => x.HandActionType == HandActionType.UNCALLED_BET);
                moneyInPot -= uncalledBetAction.Amount;

                return canWin * equity - moneyInPot;
            }

            var moneyInPotByPlayers = (from action in parsedHand.HandActions
                                       group action by action.PlayerName into grouped
                                       select new
                                       {
                                           PlayerName = grouped.Key,
                                           IsAllIn = grouped.Any(x => x.IsAllIn || x.IsAllInAction),
                                           Sum = Math.Abs(grouped.Where(x => x.Amount < 0).Sum(x => x.Amount))
                                       }).ToArray();

            var heroMoneyInPot = moneyInPotByPlayers.FirstOrDefault(x => x.PlayerName == player);

            if (heroMoneyInPot.IsAllIn)
            {
                canWin = moneyInPotByPlayers.Select(x =>
                {
                    if (x.Sum > heroMoneyInPot.Sum)
                    {
                        return heroMoneyInPot.Sum;
                    }

                    return x.Sum;
                }).Sum();

                var uncalledBetActions = parsedHand.HandActions.Where(x => x.HandActionType == HandActionType.UNCALLED_BET).ToArray();

                var allUncalledBets = uncalledBetActions.Length > 0 ? uncalledBetActions.Sum(x => x.Amount) : 0;

                var totalMoneyInPot = moneyInPotByPlayers.Sum(x => x.Sum) - allUncalledBets;

                var rake = totalMoneyInPot - parsedHand.WinningActions.Sum(x => x.Amount);

                canWin -= rake;

                moneyInPot = heroMoneyInPot.Sum;
            }

            var playersWentAllInExceptHero = moneyInPotByPlayers.Where(x => x.IsAllIn && x.PlayerName != player).ToArray();

            var maxPotOfAllInPlayer = playersWentAllInExceptHero.Length > 0 ? playersWentAllInExceptHero.Max(x => x.Sum) : 0;

            var uncalledBet = !moneyInPotByPlayers.Any(x => x.Sum > maxPotOfAllInPlayer && x.PlayerName != player) && maxPotOfAllInPlayer < heroMoneyInPot.Sum ?
                heroMoneyInPot.Sum - maxPotOfAllInPlayer : 0;

            canWin -= uncalledBet;
            moneyInPot -= uncalledBet;

            var expectedValue = canWin * equity - moneyInPot;
            return expectedValue;
        }

        #region Infrastructure

        internal static decimal CalculateMRatio(Playerstatistic stat)
        {
            decimal totalAntes = stat.Ante * stat.Numberofplayers;
            decimal mRatioValue = stat.StartingStack / (Math.Abs(stat.SmallBlind) + Math.Abs(stat.BigBlind) + Math.Abs(totalAntes));

            return mRatioValue;
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

        private static ConditionalBet CalculateRaise(IEnumerable<HandAction> streetActions, string player, PlayerList players)
        {
            var raise = new ConditionalBet();

            var wasBet = false;
            var wasAllInBet = false;
            var heroMadeBet = false;
            var heroMadeActionAfterBet = false;
            var allInPlayers = new List<string>();

            foreach (var action in streetActions)
            {
                if (!wasBet)
                {
                    if (action.IsBet())
                    {
                        wasBet = true;
                        heroMadeBet = action.PlayerName == player;

                        if (action.IsAllIn || action.IsAllInAction)
                        {
                            wasAllInBet = true;
                            allInPlayers.Add(action.PlayerName);
                            continue;
                        }

                        raise.Possible = !heroMadeBet;
                    }

                    continue;
                }

                if (!heroMadeBet && wasAllInBet && !raise.Happened && !raise.Possible)
                {
                    if (action.PlayerName != player)
                    {
                        if ((action.IsCall() || action.IsRaise()))
                        {
                            if (!action.IsAllIn && !action.IsAllInAction)
                            {
                                raise.Possible = true;
                            }
                            else
                            {
                                allInPlayers.Add(action.PlayerName);
                            }
                        }
                        // fold
                        else
                        {
                            if (heroMadeActionAfterBet)
                            {
                                var allInPlayersStacks = (from pl in players
                                                          join allInPlayer in allInPlayers on pl.PlayerName equals allInPlayer
                                                          select pl.StartingStack).ToArray();

                                var actionPlayer = players.FirstOrDefault(x => x.PlayerName == action.PlayerName);

                                if (actionPlayer != null && allInPlayersStacks.All(x => x < actionPlayer.StartingStack))
                                {
                                    raise.Possible = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        heroMadeActionAfterBet = true;
                    }
                }

                if (action.IsRaise())
                {
                    if (!raise.Happened)
                    {
                        raise.Happened = true;
                        raise.HappenedByPlayer = action.PlayerName;
                    }

                    if (!heroMadeBet && action.PlayerName == player)
                    {
                        raise.Possible = true;
                        raise.Made = true;
                    }
                }

                if (action.PlayerName == player)
                {
                    if (raise.Happened && raise.HappenedByPlayer != player)
                    {
                        raise.CheckAction(action);
                        break;
                    }
                }
            }

            return raise;
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

        private bool IsRag(HoleCards holeCards, BoardCards communityCards, Street street)
        {
            if (holeCards == null || communityCards == null || !CardHelper.IsStreetAvailable(communityCards.ToString(), street))
            {
                return false;
            }

            var boardCards = communityCards.GetBoardOnStreet(street);
            var hand = new HoldemHand.Hand(holeCards.ToString(), boardCards.ToString());

            return !HoldemHand.Hand.IsFlushDraw(hand.MaskValue, 0UL)
                && !HoldemHand.Hand.IsStraightDraw(hand.MaskValue, 0UL);
        }

        private bool IsMonotone(BoardCards communityCards, Street street)
        {
            if (communityCards == null || !CardHelper.IsStreetAvailable(communityCards.ToString(), street))
            {
                return false;
            }

            return communityCards.GetBoardOnStreet(street).GroupBy(x => x.Suit).Count() == 1;
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
                case "EP":
                    stat.PfrInEp = stat.Pfrhands;
                    stat.LimpEp = stat.LimpMade;
                    stat.DidColdCallInEp = stat.Didcoldcall;
                    break;
                case "MP":
                    stat.DidThreeBetInMp = stat.Didthreebet;
                    stat.DidFourBetInMp = stat.Didfourbet;
                    stat.DidColdCallInMp = stat.Didcoldcall;
                    stat.LimpMp = stat.LimpMade;
                    stat.PfrInMp = stat.Pfrhands;
                    return;
                case "CO":
                    stat.DidThreeBetInCo = stat.Didthreebet;
                    stat.DidFourBetInCo = stat.Didfourbet;
                    stat.DidColdCallInCo = stat.Didcoldcall;
                    stat.LimpCo = stat.LimpMade;
                    stat.PfrInCo = stat.Pfrhands;
                    return;
                case "BTN":
                    stat.DidThreeBetInBtn = stat.Didthreebet;
                    stat.DidFourBetInBtn = stat.Didfourbet;
                    stat.DidColdCallInBtn = stat.Didcoldcall;
                    stat.LimpBtn = stat.LimpMade;
                    stat.PfrInBtn = stat.Pfrhands;
                    return;
                case "SB":
                    stat.DidThreeBetInSb = stat.Didthreebet;
                    stat.DidFourBetInSb = stat.Didfourbet;
                    stat.DidColdCallInSb = stat.Didcoldcall;
                    stat.LimpSb = stat.LimpMade;
                    stat.PfrInSb = stat.Pfrhands;
                    return;
                case "BB":
                    stat.DidThreeBetInBb = stat.Didthreebet;
                    stat.DidFourBetInBb = stat.Didfourbet;
                    stat.DidColdCallInBb = stat.Didcoldcall;
                    stat.PfrInBb = stat.Pfrhands;
                    return;
                default:
                    return;
            }
        }

        private static void CalculateSteal(StealAttempt stealAttempt, HandHistory parsedHand, string player, bool isBlindPosition)
        {
            var stealers = new List<string>();

            stealers.Add(GetCutOffPlayer(parsedHand)?.PlayerName);
            stealers.Add(parsedHand.Players.FirstOrDefault(x => x.SeatNumber == parsedHand.DealerButtonPosition)?.PlayerName);
            stealers.Add(parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND)?.PlayerName);

            bool wasSteal = false;
            foreach (var action in parsedHand.PreFlop.Where(x => !x.IsBlinds))
            {
                if (wasSteal)
                {
                    if (action.PlayerName == player)
                    {
                        if (!isBlindPosition)
                        {
                            return;
                        }

                        stealAttempt.Faced = true;
                        stealAttempt.Defended = action.IsCall() || action.IsRaise();
                        stealAttempt.Raised = action.IsRaise();
                        stealAttempt.Folded = action.IsFold;

                        return;
                    }

                    if (!action.IsFold)
                    {
                        if (action.IsCall() && parsedHand.HandActions.Any(x => x.PlayerName == action.PlayerName && x.HandActionType == HandActionType.SMALL_BLIND))
                        {
                            continue;
                        }

                        return;
                    }
                }
                else
                {
                    if (stealers.Contains(action.PlayerName))
                    {
                        if (action.PlayerName == player)
                        {
                            stealAttempt.Possible = true;
                            stealAttempt.Attempted = action.IsRaise();
                            return;
                        }

                        if (action.IsRaise())
                        {
                            wasSteal = true;
                            continue;
                        }
                    }

                    if (!action.IsFold)
                    {
                        return;
                    }
                }
            }
        }

        private static void CalculateCoRaise(StealAttempt stealAtempt, HandHistory parsedHand, string player)
        {
            if (parsedHand.Players.FirstOrDefault(x => x.SeatNumber == parsedHand.DealerButtonPosition)?.PlayerName !=
                player)
                return;

            var stealers = new List<string> { GetCutOffPlayer(parsedHand)?.PlayerName };
            var wasSteal = false;
            foreach (var action in parsedHand.PreFlop.Where(x => !x.IsBlinds))
            {
                if (wasSteal)
                {
                    if (action.PlayerName == player)
                    {
                        stealAtempt.Faced = true;
                        stealAtempt.Defended = action.IsCall || action.IsRaise();
                        stealAtempt.Raised = action.IsRaise();
                        stealAtempt.Folded = action.IsFold;
                        return;
                    }

                    if (action.IsRaise())
                        return;
                }
                else
                {
                    if (action.IsRaise() && stealers.Contains(action.PlayerName))
                    {
                        wasSteal = true;
                        continue;
                    }

                    if (!action.IsFold)
                        return;
                }
            }
        }

        private static void Calculate4Bet(ConditionalBet fourBet, List<HandAction> actions, string player, string raiser, PlayerList players)
        {
            var start3Bet = false;
            var threeBetHappened = false;
            var threeBetIsAllIn = false;
            var threeBetAllInPlayerStack = 0m;
            var threeBetMade = false;
            var fourBetIsAllIn = false;
            var fourBetAllInPlayerStack = 0m;
            var callAfterThreeBet = false;
            var callAfterFourBet = false;
            var playersCannot3Bet = new HashSet<string>();

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];

                if (start3Bet)
                {
                    if (!threeBetHappened)
                    {
                        if (playersCannot3Bet.Contains(action.PlayerName))
                        {
                            continue;
                        }

                        if (!action.IsRaise() || action.PlayerName == raiser)
                        {
                            continue;
                        }

                        threeBetHappened = true;

                        if (action.IsAllInAction || action.IsAllIn)
                        {
                            threeBetIsAllIn = true;

                            var allInPlayer = players.FirstOrDefault(x => x.PlayerName == action.PlayerName);

                            if (allInPlayer != null)
                            {
                                threeBetAllInPlayerStack = allInPlayer.StartingStack;
                            }
                        }

                        // player does 3-bet
                        threeBetMade = action.PlayerName == player;

                        continue;
                    }

                    // 3-bet happened
                    if (!fourBet.Happened)
                    {
                        if (action.IsCall() && action.PlayerName != player)
                        {
                            callAfterThreeBet = true;
                        }

                        if (playersCannot3Bet.Contains(action.PlayerName))
                        {
                            continue;
                        }

                        if (action.PlayerName == player && raiser == action.PlayerName)
                        {
                            if (threeBetIsAllIn && !callAfterThreeBet)
                            {
                                var couldRaise = false;

                                for (var j = i + 1; j < actions.Count; j++)
                                {
                                    var playerToAct = players.FirstOrDefault(x => actions[j].PlayerName == x.PlayerName);

                                    if (playerToAct != null && playerToAct.StartingStack > threeBetAllInPlayerStack)
                                    {
                                        couldRaise = true;
                                        break;
                                    }
                                }

                                if (!couldRaise)
                                {
                                    return;
                                }
                            }

                            fourBet.Possible = true;
                        }

                        if (!action.IsRaise() || (action.PlayerName != raiser && !threeBetMade))
                        {
                            continue;
                        }

                        fourBet.Happened = true;
                        fourBet.HappenedByPlayer = action.PlayerName;

                        if (action.IsAllInAction || action.IsAllIn)
                        {
                            fourBetIsAllIn = true;

                            var allInPlayer = players.FirstOrDefault(x => x.PlayerName == action.PlayerName);

                            if (allInPlayer != null)
                            {
                                fourBetAllInPlayerStack = allInPlayer.StartingStack;
                            }
                        }

                        if (action.PlayerName != player)
                        {
                            continue;
                        }

                        fourBet.Made = true;
                        return;
                    }

                    if (action.IsCall() && action.PlayerName != player)
                    {
                        callAfterFourBet = true;
                    }

                    if (action.PlayerName == player && threeBetMade)
                    {
                        if (fourBet.CheckAction(action))
                        {
                            if (fourBetIsAllIn && !callAfterFourBet)
                            {
                                var couldRaise = false;

                                for (var j = i + 1; j < actions.Count; j++)
                                {
                                    var playerToAct = players.FirstOrDefault(x => actions[j].PlayerName == x.PlayerName);

                                    if (playerToAct != null && playerToAct.StartingStack > fourBetAllInPlayerStack)
                                    {
                                        couldRaise = true;
                                        break;
                                    }
                                }

                                if (!couldRaise)
                                {
                                    return;
                                }
                            }

                            fourBet.Possible = true;

                            return;
                        }
                    }
                }
                else
                {
                    if (action.IsRaise() && action.PlayerName == raiser)
                    {
                        start3Bet = true;
                        continue;
                    }

                    if (action.IsCall() && !playersCannot3Bet.Contains(action.PlayerName))
                    {
                        playersCannot3Bet.Add(action.PlayerName);
                    }
                }
            }
        }

        private static void Calculate5Bet(ConditionalBet threeBet, ConditionalBet fourBet, ConditionalBet fiveBet, List<HandAction> actions, string player, string raiser)
        {
            if (!threeBet.Happened && !fourBet.Happened)
            {
                return;
            }

            // could did 5-bet
            if (fourBet.Faced)
            {
                var fourBetAction = actions.LastOrDefault(x => x.PlayerName == fourBet.HappenedByPlayer && x.IsRaise());
                fiveBet.Possible = !fourBetAction.IsAllIn && !fourBetAction.IsAllInAction;

                var raises = actions.Count(x => x.PlayerName == player && x.IsRaise());

                if (raises > 1)
                {
                    fiveBet.Possible = true;
                    fiveBet.Made = raises > 1;
                }
            }


        }

        private static void CalculateColdCall3Bet(ConditionalBet coldCall3Bet, List<HandAction> preflops, string player)
        {
            var raisers = new List<string>();

            bool canThreeBet = false;
            bool wasThreeBet = false;

            foreach (var action in preflops)
            {
                if (wasThreeBet)
                {
                    if (action.PlayerName == player)
                        coldCall3Bet.Possible = true;

                    if (action.IsRaise)
                        return;

                    if (!action.IsCall() || action.PlayerName != player)
                        continue;

                    coldCall3Bet.Made = true;
                    return;
                }
                else if (canThreeBet)
                {
                    if (action.IsRaise())
                    {
                        if (action.PlayerName == player)
                            return;

                        wasThreeBet = true;
                    }
                }
                else
                {
                    if (action.IsRaise())
                    {
                        canThreeBet = true;
                    }
                }
            }
        }

        private static void CalculateColdCall4Bet(ConditionalBet coldCall4Bet, List<HandAction> preflops, string player)
        {
            var raisers = new List<string>();

            bool canThreeBet = false;
            bool wasThreeBet = false;
            bool wasFourBet = false;

            foreach (var action in preflops)
            {
                if (wasFourBet)
                {
                    if (action.PlayerName == player)
                        coldCall4Bet.Possible = true;

                    if (action.IsRaise)
                        return;

                    if (!action.IsCall() || action.PlayerName != player)
                        continue;

                    coldCall4Bet.Made = true;
                    return;
                }
                else if (wasThreeBet)
                {
                    if (action.IsRaise())
                    {
                        if (action.PlayerName == player)
                            return;

                        wasFourBet = true;
                    }
                }
                else if (canThreeBet)
                {
                    if (action.IsRaise())
                        wasThreeBet = true;
                }
                else
                {
                    if (action.IsRaise())
                        canThreeBet = true;
                }
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

        /// <summary>
        /// Determines if player did cold call
        /// </summary>
        /// <param name="coldCall">Cold call <see cref="Condition"/></param>
        /// <param name="preflops">Preflop <see cref="HandAction"/> actions</param>
        /// <param name="player">Player</param>
        private static void CalculateColdCall(Condition coldCall, IList<HandAction> preflops, string player)
        {
            bool wasColdRaise = false;

            foreach (var action in preflops)
            {
                if (wasColdRaise)
                {
                    if (action.PlayerName != player)
                    {
                        continue;
                    }

                    coldCall.Possible = true;

                    if (action.IsCall())
                    {
                        coldCall.Made = true;
                    }

                    return;
                }

                if (action.PlayerName == player && action.IsRaise())
                {
                    return;
                }

                if (action.IsRaise())
                {
                    wasColdRaise = true;
                }
            }
        }

        private static void CalculateSqueeze(ConditionalBet squeezBet, IList<HandAction> preflops, string player)
        {
            List<string> squeezePlayers = new List<string>();
            bool wasSqueezeRaise = false, wasSqueezeCall = false;
            foreach (var action in preflops)
            {
                if (wasSqueezeRaise)
                {
                    if (wasSqueezeCall)
                    {
                        if (!squeezBet.Happened)
                        {
                            if (squeezePlayers.Contains(action.PlayerName))
                                return;

                            squeezBet.Happened = action.IsRaise();

                            if (action.PlayerName == player)
                            {
                                squeezBet.Possible = true;
                                if (action.IsRaise())
                                {
                                    squeezBet.Made = true;
                                }

                                return;
                            }

                            if (action.IsCall())
                            {
                                squeezePlayers.Add(action.PlayerName);
                            }

                            if (squeezBet.Happened)
                            {
                                // squuez bet happened, but player is not the one who did it. check if player is within squeezed list
                                if (!squeezePlayers.Contains(player))
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (action.PlayerName == player)
                            {
                                squeezBet.CheckAction(action);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (action.IsRaise())
                        {
                            return;
                        }

                        if (action.IsCall())
                        {
                            wasSqueezeCall = true;
                            squeezePlayers.Add(action.PlayerName);
                        }
                    }
                }
                else
                {
                    if (!action.IsRaise())
                        continue;

                    wasSqueezeRaise = true;
                    squeezePlayers.Add(action.PlayerName);
                }
            }
        }

        public static void Calculate3Bet(ConditionalBet threeBet, IList<HandAction> actions, string player, string raiser)
        {
            bool start3Bet = false;

            var playersCannot3Bet = new HashSet<string>();

            foreach (var action in actions)
            {
                if (start3Bet)
                {
                    if (!threeBet.Happened)
                    {
                        if (playersCannot3Bet.Contains(action.PlayerName))
                        {
                            continue;
                        }

                        if (action.PlayerName == player && action.PlayerName != raiser)
                        {
                            threeBet.Possible = true;
                        }

                        if (!action.IsRaise() || action.PlayerName == raiser)
                        {
                            continue;
                        }

                        threeBet.Happened = true;
                        threeBet.HappenedByPlayer = action.PlayerName;

                        if (action.PlayerName != player)
                        {
                            continue;
                        }

                        threeBet.Made = true;
                        return;
                    }

                    if (action.PlayerName == player && raiser == action.PlayerName)
                    {
                        if (threeBet.CheckAction(action))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (action.IsRaise() && action.PlayerName == raiser)
                    {
                        start3Bet = true;
                        continue;
                    }

                    if (action.IsCall() && !playersCannot3Bet.Contains(action.PlayerName))
                    {
                        playersCannot3Bet.Add(action.PlayerName);
                    }
                }
            }
        }


        private static void ColdCallafterPositionalOpenRaise(ConditionalBet coldCallVsBtnOpen,
                                                             IList<HandAction> preflops,
                                                             string player,
                                                             string playerPositionalOpenRaiseName)
        {
            bool btnOpen = false;

            foreach (var action in preflops.Where(act => !act.IsBlinds))
            {
                if (btnOpen)
                {
                    if (action.IsCall() && action.PlayerName != player)
                    {
                        coldCallVsBtnOpen.Happened = true;
                        continue;
                    }
                    if (action.IsRaise() && action.PlayerName != player)
                        if (coldCallVsBtnOpen.Happened)
                            return;
                        else
                            continue;

                    if (action.PlayerName == player)
                        coldCallVsBtnOpen.Possible = true;


                    if (action.IsFold && action.PlayerName == player)
                        return;

                    if (action.IsCall && action.PlayerName == player)
                    {
                        coldCallVsBtnOpen.Made = true;
                        return;
                    }
                }
                else
                {
                    if (!action.IsFold && !action.IsRaise)
                        return;
                    if (action.IsRaise && action.PlayerName == playerPositionalOpenRaiseName)
                        btnOpen = true;
                    if (action.IsRaise && action.PlayerName != playerPositionalOpenRaiseName)
                        return;
                    if (action.IsFold && action.PlayerName == player)
                        return;
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
            var bb = preflops.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND)?.PlayerName;

            if (!string.IsNullOrEmpty(bb) && bb == player) // can't limp on bb
            {
                return;
            }

            foreach (var action in preflops.Where(x => !x.IsBlinds))
            {
                if (action.PlayerName == player)
                {
                    if (!limp.Made)
                    {
                        limp.Possible = true;
                        if (action.IsCall() || action.IsCheck)
                        {
                            limp.Made = true;
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    // somebody raised after we had limped
                    {
                        limp.CheckAction(action);
                        return;
                    }
                }

                // somebody raised before we had a chance to limp
                if (action.IsRaise() && !limp.Made)
                {
                    return;
                }
            }
        }

        private static void CalculateBetWhenCheckedTo(Condition betWhenCheckedTo, IEnumerable<HandAction> streetActions, string player)
        {
            bool wasCheck = false;

            foreach (var action in streetActions)
            {
                if (!wasCheck)
                {
                    if (action.IsCheck)
                    {
                        wasCheck = true;
                        continue;
                    }

                    if (action.IsBet() || action.PlayerName == player)
                    {
                        return;
                    }
                }
                else
                {
                    if (action.PlayerName != player)
                    {
                        if (action.IsBet())
                        {
                            return;
                        }

                        continue;
                    }

                    betWhenCheckedTo.Possible = true;
                    betWhenCheckedTo.Made = action.IsBet();

                    return;
                }
            }
        }

        private static bool IsOpenRaise(IList<HandAction> preflops, string player)
        {
            var playerAction = preflops.FirstOrDefault(x => !x.IsBlinds && !x.IsFold);
            if (playerAction != null && playerAction.PlayerName == player)
            {
                return playerAction.IsRaise();
            }
            return false;
        }

        private static bool IsRaisedLimpers(IList<HandAction> preflops, string player)
        {
            bool limpersFound = false;
            foreach (var action in preflops.Where(x => !x.IsBlinds))
            {
                if (action.PlayerName == player)
                {
                    return limpersFound && action.IsRaise();
                }

                if (action.IsRaise())
                {
                    return false;
                }

                limpersFound = limpersFound || action.IsCall() || action.IsCheck;
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
            {
                return null;
            }

            var players = hand.Players.ToList();

            var button = players.FirstOrDefault(x => x.SeatNumber == hand.DealerButtonPosition);

            var orderedPlayers = players.OrderBy(x => x.SeatNumber).ToArray();

            var btnPlayerIndex = orderedPlayers.FindIndex(x => x.SeatNumber == hand.DealerButtonPosition);

            if (btnPlayerIndex < 0)
            {
                var co = hand.HandActions
                    .Select(h => h.PlayerName)
                    .Distinct()
                    .Where(x => x != button?.PlayerName)
                    .LastOrDefault();

                return hand.Players.FirstOrDefault(x => x.PlayerName == co);
            }

            var coPlayer = btnPlayerIndex == 0 ? orderedPlayers.Last() : orderedPlayers[btnPlayerIndex - 1];

            return coPlayer;
        }

        private static Player GetDealerPlayer(HandHistory hand)
        {
            var buttonPlayer = hand.Players.FirstOrDefault(x => x.SeatNumber == hand.DealerButtonPosition);
            return buttonPlayer;
        }

        private static Player GetInPositionPlayer(HandHistory hand, Street street)
        {
            var actions = hand.HandActions.Street(street)
                .Where(x => !string.IsNullOrWhiteSpace(x.PlayerName)
                    && x.HandActionType != HandActionType.ANTE)
                .ToArray();

            var sbAction = actions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND);

            // DHUD-273: if only 2 players are playing. there is a possible case when SB is a dealer, then he will be in position even if his action was first
            if (hand.Players.Count == 2)
            {
                if (sbAction != null)
                {
                    var sbPlayer = hand.Players.FirstOrDefault(x => x.PlayerName == sbAction.PlayerName);

                    if (sbPlayer != null && sbPlayer.SeatNumber == hand.DealerButtonPosition)
                    {
                        return sbPlayer;
                    }
                }
            }

            var players = new List<string>();

            foreach (var action in actions.Where(x => x.HandActionType != HandActionType.POSTS))
            {
                if (players.Contains(action.PlayerName))
                {
                    break;
                }

                players.Add(action.PlayerName);
            }

            // DHUD-273: remove players who folded from the list
            var foldedPlayers = actions.Where(x => x.HandActionType == HandActionType.FOLD).Select(x => x.PlayerName).ToArray();
            foldedPlayers.ForEach(x => players.Remove(x));

            if (players.Count > 1)
            {
                var ipPlayer = players.LastOrDefault();

                // DHUD-273: SB can't be in position if he isn't a dealer
                if (sbAction != null && ipPlayer == sbAction.PlayerName)
                {
                    return null;
                }

                return hand.Players.FirstOrDefault(x => x.PlayerName == ipPlayer);
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