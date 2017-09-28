using System;
using System.Collections.Generic;
using System.Linq;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.BusinessHelper.TextureAnalyzers.Flush;
using AcePokerSolutions.BusinessHelper.TextureAnalyzers.Straight;
using AcePokerSolutions.BusinessHelper.TextureHelpers;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.DataAccessHelper.DriveHUD;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.DataTypes.InsertManagerObjects;
using AcePokerSolutions.DataTypes.NotesTreeObjects;
using AcePokerSolutions.DataTypes.NotesTreeObjects.ActionsObjects;
using DriveHUD.Common.Log;

namespace AcePokerSolutions.BusinessHelper
{
    public class NoteManager
    {
        public static DatabaseNote GetPlayerNote(NoteObject note)
        {
            List<Playerstatistic> selectedPlayerStatistics = StaticStorage.Playerstatistics;

            selectedPlayerStatistics = FilterByPositionCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByPreflopFacingCondition(selectedPlayerStatistics, note.Settings); //todo to finish
            selectedPlayerStatistics = FilterByPositionThreeBetCondition(selectedPlayerStatistics, note.Settings); //todo to finish
            selectedPlayerStatistics = FilterByPositionRaiserCondition(selectedPlayerStatistics, note.Settings); //todo to finish
            selectedPlayerStatistics = FilterByNoOfPlayerCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByStakeCondition(selectedPlayerStatistics, note.Settings);

            selectedPlayerStatistics = FilterByHoleCardCondition(selectedPlayerStatistics, note.Settings);

            selectedPlayerStatistics = FilterByPreflopActionCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByFlopActionCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByTurnActionCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByRiverActionCondition(selectedPlayerStatistics, note.Settings);

            selectedPlayerStatistics = FilterByFlopTextureCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByTurnTextureCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByRiverTextureCondition(selectedPlayerStatistics, note.Settings);



            selectedPlayerStatistics = FilterByFlopHandTextureCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByTurnHandTextureCondition(selectedPlayerStatistics, note.Settings);
            selectedPlayerStatistics = FilterByRiverHandTextureCondition(selectedPlayerStatistics, note.Settings);




            selectedPlayerStatistics = FilterByAllSelectedFilters(selectedPlayerStatistics, note.Settings.SelectedFilters, note.Settings.SelectedFiltersComparison);


            //may be need to put this item in the beginning
            selectedPlayerStatistics = FilterByPlayersCondition(StaticStorage.Playerstatistics, note.Settings);   //todo check that if player's stats are outside the range then all statistics not considered anymore

            DatabaseNote dbNote = BuildNote(note, selectedPlayerStatistics);
            return dbNote;
        }


        #region hand value analyzers
        //todo 100% tested for flop;  need to test for turn and river 
        private static List<Playerstatistic> FilterByRiverHandTextureCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            if (settings.RiverHvSettings.AnyHv && settings.RiverHvSettings.AnyFlushDraws && settings.RiverHvSettings.AnyStraightDraws)
                return playerStatistics;

            List<Playerstatistic> fileteredList = playerStatistics;

            if (!settings.RiverHvSettings.AnyHv && settings.RiverHvSettings.SelectedHv.Count > 0)
            {
                foreach (int i in settings.RiverHvSettings.SelectedHv)
                {
                    HandValueEnum handValueEnum = (HandValueEnum)i;
                    fileteredList = NoteManagerHelper.HandValueFilterHelper(fileteredList, handValueEnum, Street.River);
                }

            }

            return fileteredList;
        }

        private static List<Playerstatistic> FilterByFlopHandTextureCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            if (settings.FlopHvSettings.AnyHv && settings.FlopHvSettings.AnyFlushDraws && settings.FlopHvSettings.AnyStraightDraws)
                return playerStatistics;

            List<Playerstatistic> fileteredList = playerStatistics;

            if (!settings.FlopHvSettings.AnyHv && settings.FlopHvSettings.SelectedHv.Count > 0)
            {
                foreach (int i in settings.FlopHvSettings.SelectedHv)
                {
                    HandValueEnum handValueEnum = (HandValueEnum)i;
                    fileteredList = NoteManagerHelper.HandValueFilterHelper(fileteredList, handValueEnum, Street.Flop);
                }

            }

            if (!settings.FlopHvSettings.AnyFlushDraws && settings.FlopHvSettings.SelectedFlushDraws.Count > 0)
            {
                foreach (int i in settings.FlopHvSettings.SelectedFlushDraws)
                {
                    HandValueFlushDrawEnum handValueFlushDrawEnum = (HandValueFlushDrawEnum)i;
                    fileteredList = NoteManagerHelper.HandValueFilterFlushDrawHelper(fileteredList, handValueFlushDrawEnum, Street.Flop);
                }
            }

            if (!settings.FlopHvSettings.AnyStraightDraws && settings.FlopHvSettings.SelectedStraighDraws.Count > 0)
            {
                foreach (int i in settings.FlopHvSettings.SelectedStraighDraws)
                {
                    HandValueStraightDraw handValueStraightDrawEnum = (HandValueStraightDraw)i;
                    fileteredList = NoteManagerHelper.HandValueFilterStraightDrawHelper(fileteredList, handValueStraightDrawEnum, Street.Flop);
                }
            }

            return fileteredList;
        }

        private static List<Playerstatistic> FilterByTurnHandTextureCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            if (settings.TurnHvSettings.AnyHv && settings.TurnHvSettings.AnyFlushDraws && settings.TurnHvSettings.AnyStraightDraws)
                return playerStatistics;

            List<Playerstatistic> fileteredList = playerStatistics;

            if (!settings.TurnHvSettings.AnyHv && settings.TurnHvSettings.SelectedHv.Count > 0)
            {
                foreach (int i in settings.TurnHvSettings.SelectedHv)
                {
                    HandValueEnum handValueEnum = (HandValueEnum)i;
                    fileteredList = NoteManagerHelper.HandValueFilterHelper(fileteredList, handValueEnum, Street.Turn);
                }

            }

            if (!settings.TurnHvSettings.AnyFlushDraws && settings.TurnHvSettings.SelectedFlushDraws.Count > 0)
            {
                foreach (int i in settings.TurnHvSettings.SelectedFlushDraws)
                {
                    HandValueFlushDrawEnum handValueFlushDrawEnum = (HandValueFlushDrawEnum)i;
                    fileteredList = NoteManagerHelper.HandValueFilterFlushDrawHelper(fileteredList, handValueFlushDrawEnum, Street.Turn);
                }
            }

            if (!settings.TurnHvSettings.AnyStraightDraws && settings.TurnHvSettings.SelectedStraighDraws.Count > 0)
            {
                foreach (int i in settings.TurnHvSettings.SelectedStraighDraws)
                {
                    HandValueStraightDraw handValueStraightDrawEnum = (HandValueStraightDraw)i;
                    fileteredList = NoteManagerHelper.HandValueFilterStraightDrawHelper(fileteredList, handValueStraightDrawEnum, Street.Turn);
                }
            }

            return fileteredList;
        }
        #endregion

        #region Board Texture

        private static List<Playerstatistic> FilterByFlopTextureCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            if (!settings.FlopTextureSettings.IsFlushCardFilter && !settings.FlopTextureSettings.IsOpenEndedStraightDrawsFilter && !settings.FlopTextureSettings.IsPossibleStraightsFilter && !settings.FlopTextureSettings.IsGutshotsFilter && !settings.FlopTextureSettings.IsHighcardFilter && !settings.FlopTextureSettings.IsCardTextureFilter && !settings.FlopTextureSettings.IsPairedFilter)
                return playerStatistics;

            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            foreach (Playerstatistic playerstatistic in playerStatistics)
            {
                if (settings.FlopTextureSettings.IsFlushCardFilter)
                {
                    //filter for rainbow flush
                    if (settings.FlopTextureSettings.FlushCard == FlopFlushCardsEnum.Rainbow && !new NoPossibleFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Flop))
                        continue;
                    //filter for two of one suit
                    if (settings.FlopTextureSettings.FlushCard == FlopFlushCardsEnum.TwoOfOneSuit && !new TwoOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Flop))
                        continue;
                    //filter for three of one suit
                    if (settings.FlopTextureSettings.FlushCard == FlopFlushCardsEnum.ThreeOfOneSuit && !new ThreeOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Flop))
                        continue;
                }

                //filter for open-ended straights
                //if number of open ended straigths is not equal to the number we need, then we skip such playerstatistic
                if (settings.FlopTextureSettings.IsOpenEndedStraightDrawsFilter)
                    if (new OpenEndedStraightTextureAnalyzer().Analyze(playerstatistic.Board, Street.Flop) != settings.FlopTextureSettings.OpenEndedStraightDraws)
                        continue;

                //filter for possible straights and according < > or ==
                if (settings.FlopTextureSettings.IsPossibleStraightsFilter)
                    if (!BoardTextureAnalyzerHelpers.CheckEquality(settings.FlopTextureSettings.PossibleStraightsCompare, new PossibleStraightTextureAnalyzer().Analyze(playerstatistic.Board, Street.Flop), settings.FlopTextureSettings.PossibleStraights))
                        continue;

                //filter for gutshot straights
                if (settings.FlopTextureSettings.IsGutshotsFilter)
                    if (new GutShotBeatNutsTextureAnalyzer().Analyze(playerstatistic.Board, Street.Flop) != settings.FlopTextureSettings.Gutshots)
                        continue;

                //filter for the highest card
                if (settings.FlopTextureSettings.IsHighcardFilter)
                    if (BoardTextureAnalyzerHelpers.HighestBoardCardRank(playerstatistic.Board, Street.Flop) != Card.GetCardRank(settings.FlopTextureSettings.HighestCard))
                        continue;

                //filter for exact flop texture
                if (settings.FlopTextureSettings.IsCardTextureFilter)
                    if (!BoardTextureAnalyzerHelpers.BoardContainsExactTextureCards(playerstatistic.Board, settings.FlopTextureSettings.SelectedCardTextureList, Street.Flop))
                        continue;

                //filter for flop is Paired
                if (settings.FlopTextureSettings.IsPairedFilter)
                    if (!BoardTextureAnalyzerHelpers.BoardContainsAPair(playerstatistic.Board, Street.Flop))
                        continue;

                fileteredList.Add(playerstatistic);
            }

            return fileteredList;
        }

        private static List<Playerstatistic> FilterByTurnTextureCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            if (!settings.TurnTextureSettings.IsFlushCardFilter && !settings.TurnTextureSettings.IsOpenEndedStraightDrawsFilter && !settings.TurnTextureSettings.IsPossibleStraightsFilter && !settings.TurnTextureSettings.IsGutshotsFilter && !settings.TurnTextureSettings.IsHighcardFilter && !settings.TurnTextureSettings.IsCardTextureFilter && !settings.TurnTextureSettings.IsPairedFilter)
                return playerStatistics;

            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            foreach (Playerstatistic playerstatistic in playerStatistics)
            {
                //filters for turn flush
                if (settings.TurnTextureSettings.IsFlushCardFilter)
                {
                    //filter for rainbow flush
                    if (settings.TurnTextureSettings.FlushCard == TurnFlushCardsEnum.Rainbow && !new NoPossibleFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn))
                        continue;
                    //filter for two of two suits
                    if (settings.TurnTextureSettings.FlushCard == TurnFlushCardsEnum.TwoOfTwoSuits && !new TwoOfTwoSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn))
                        continue;
                    //filter for two of one suit
                    if (settings.TurnTextureSettings.FlushCard == TurnFlushCardsEnum.TwoOfOneSuit && !new TwoOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn))
                        continue;
                    //filter for three of one suit
                    if (settings.TurnTextureSettings.FlushCard == TurnFlushCardsEnum.ThreeOfOneSuit && !new ThreeOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn))
                        continue;
                    //filter for four of one suit  
                    if (settings.TurnTextureSettings.FlushCard == TurnFlushCardsEnum.FourOfOneSuit && !new FourOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn))
                        continue;
                }

                //filter for open-ended straights
                //if number of open ended straigths is not equal to the number we need, then we skip such playerstatistic
                if (settings.TurnTextureSettings.IsOpenEndedStraightDrawsFilter)
                    if (new OpenEndedStraightTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn) != settings.TurnTextureSettings.OpenEndedStraightDraws)
                        continue;

                //filter for possible straights and according < > or ==
                if (settings.TurnTextureSettings.IsPossibleStraightsFilter)
                    if (!BoardTextureAnalyzerHelpers.CheckEquality(settings.TurnTextureSettings.PossibleStraightsCompare, new PossibleStraightTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn), settings.TurnTextureSettings.PossibleStraights))
                        continue;

                //filter for gutshot straights
                if (settings.TurnTextureSettings.IsGutshotsFilter)
                    if (new GutShotBeatNutsTextureAnalyzer().Analyze(playerstatistic.Board, Street.Turn) != settings.TurnTextureSettings.Gutshots)
                        continue;

                //filter for the highest card
                if (settings.TurnTextureSettings.IsHighcardFilter)
                    if (BoardTextureAnalyzerHelpers.HighestBoardCardRank(playerstatistic.Board, Street.Turn) != Card.GetCardRank(settings.TurnTextureSettings.HighestCard))
                        continue;

                //filter for exact turn texture
                if (settings.TurnTextureSettings.IsCardTextureFilter)
                    if (!BoardTextureAnalyzerHelpers.BoardContainsExactTextureCards(playerstatistic.Board, settings.TurnTextureSettings.SelectedCardTextureList, Street.Turn))
                        continue;

                //filter for Turn is Paired
                if (settings.TurnTextureSettings.IsPairedFilter)
                    if (!BoardTextureAnalyzerHelpers.BoardContainsAPair(playerstatistic.Board, Street.Turn))
                        continue;


                fileteredList.Add(playerstatistic);
            }

            return fileteredList;
        }

        private static List<Playerstatistic> FilterByRiverTextureCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            if (!settings.RiverTextureSettings.IsFlushCardFilter && !settings.RiverTextureSettings.IsPossibleStraightsFilter && !settings.RiverTextureSettings.IsHighcardFilter && !settings.RiverTextureSettings.IsCardTextureFilter && !settings.RiverTextureSettings.IsPairedFilter)
                return playerStatistics;

            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            foreach (Playerstatistic playerstatistic in playerStatistics)
            {
                //filters for river flush
                if (settings.RiverTextureSettings.IsFlushCardFilter)
                {
                    //filter for no possible flush
                    if (settings.RiverTextureSettings.FlushCard == RiverFlushCardsEnum.NoFlush && !new NoPossibleFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.River))
                        continue;
                    //filter for three of one suit
                    if (settings.RiverTextureSettings.FlushCard == RiverFlushCardsEnum.ThreeCardsOneSuit && !new ThreeOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.River))
                        continue;
                    //filter for four of one suit  
                    if (settings.RiverTextureSettings.FlushCard == RiverFlushCardsEnum.FourCardsOneSuit && !new FourOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.River))
                        continue;
                    //filter for five of one suit  
                    if (settings.RiverTextureSettings.FlushCard == RiverFlushCardsEnum.FiveCardsOneSuit && !new FiveOfOneSuitFlushTextureAnalyzer().Analyze(playerstatistic.Board, Street.River))
                        continue;
                }

                //filter for possible straights and according < > or ==
                if (settings.RiverTextureSettings.IsPossibleStraightsFilter)
                    if (!BoardTextureAnalyzerHelpers.CheckEquality(settings.RiverTextureSettings.PossibleStraightsCompare, new PossibleStraightTextureAnalyzer().Analyze(playerstatistic.Board, Street.River), settings.RiverTextureSettings.PossibleStraights))
                        continue;

                //filter for the highest card
                if (settings.RiverTextureSettings.IsHighcardFilter)
                    if (BoardTextureAnalyzerHelpers.HighestBoardCardRank(playerstatistic.Board, Street.River) != Card.GetCardRank(settings.RiverTextureSettings.HighestCard))
                        continue;

                //filter for exact river texture
                if (settings.RiverTextureSettings.IsCardTextureFilter)
                    if (!BoardTextureAnalyzerHelpers.BoardContainsExactTextureCards(playerstatistic.Board, settings.RiverTextureSettings.SelectedCardTextureList, Street.River))
                        continue;

                //filter for river is Paired
                if (settings.RiverTextureSettings.IsPairedFilter)
                    if (!BoardTextureAnalyzerHelpers.BoardContainsAPair(playerstatistic.Board, Street.River))
                        continue;

                fileteredList.Add(playerstatistic);
            }


            return fileteredList;
        }

        #endregion

        #region ActionCondition

        private static List<Playerstatistic> FilterByRiverActionCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            List<ActionTypeEnum> riverObligatoryActions = GetObligatoryActions(settings.RiverActions);

            foreach (Playerstatistic playerstatistic in playerStatistics)
            {
                List<HandAction> riverActions = playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.River && x.PlayerName == StaticStorage.CurrentPlayerName).ToList();

                if (CompareHandActionsWithObligatoryHandActions(riverActions, riverObligatoryActions))
                    fileteredList.Add(playerstatistic);
            }
            return fileteredList;
        }

        private static List<Playerstatistic> FilterByTurnActionCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            List<ActionTypeEnum> turnObligatoryActions = GetObligatoryActions(settings.TurnActions);

            foreach (Playerstatistic playerstatistic in playerStatistics)
            {
                List<HandAction> turnActions = playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Turn && x.PlayerName == StaticStorage.CurrentPlayerName).ToList();

                if (CompareHandActionsWithObligatoryHandActions(turnActions, turnObligatoryActions))
                    fileteredList.Add(playerstatistic);
            }
            return fileteredList;
        }

        private static List<Playerstatistic> FilterByFlopActionCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            List<ActionTypeEnum> flopObligatoryActions = GetObligatoryActions(settings.FlopActions);

            foreach (Playerstatistic playerstatistic in playerStatistics)
            {
                List<HandAction> flopActions = playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Flop && x.PlayerName == StaticStorage.CurrentPlayerName).ToList();

                if (CompareHandActionsWithObligatoryHandActions(flopActions, flopObligatoryActions))
                    fileteredList.Add(playerstatistic);
            }
            return fileteredList;
        }

        private static List<Playerstatistic> FilterByPreflopActionCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            List<ActionTypeEnum> preflopObligatoryActions = GetObligatoryActions(settings.PreflopActions);

            foreach (Playerstatistic playerstatistic in playerStatistics)
            {
                List<HandAction> preflopActions = playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Preflop && x.PlayerName == StaticStorage.CurrentPlayerName && x.HandActionType != HandActionType.SMALL_BLIND && x.HandActionType != HandActionType.BIG_BLIND).ToList();

                if (CompareHandActionsWithObligatoryHandActions(preflopActions, preflopObligatoryActions))
                    fileteredList.Add(playerstatistic);
            }
            return fileteredList;
        }

        private static bool CompareHandActionsWithObligatoryHandActions(List<HandAction> heroHandActions, List<ActionTypeEnum> obligatoryHandActions)
        {
            if (heroHandActions.Count < obligatoryHandActions.Count)
                return false;

            for (int i = 0; i < obligatoryHandActions.Count; i++)
            {
                if (heroHandActions[i].HandActionType == ToHandActionType(obligatoryHandActions[i]))
                    continue;

                return false;
            }

            //foreach (ActionTypeEnum obligatoryHandAction in obligatoryHandActions)
            //{
            //    if (obligatoryHandAction == ActionTypeEnum.Any)
            //        continue;

            //    if (heroHandActions[obligatoryHandActions.IndexOf(obligatoryHandAction)].HandActionType ==
            //        ToHandActionType(obligatoryHandAction))
            //        continue;

            //    return false;
            //}
            return true;
        }

        private static HandActionType ToHandActionType(ActionTypeEnum actionTypeEnum)
        {
            switch (actionTypeEnum)
            {
                //we don't consider HandActionType.Any because this case is eliminated before enum conversion is used
                case ActionTypeEnum.Bet:
                    return HandActionType.BET;
                case ActionTypeEnum.Check:
                    return HandActionType.CHECK;
                case ActionTypeEnum.Call:
                    return HandActionType.CALL;
                case ActionTypeEnum.Raise:
                    return HandActionType.RAISE;
                case ActionTypeEnum.Fold:
                    return HandActionType.FOLD;
                default:
                    LogProvider.Log.Error(typeof(NoteManager), "Current player failed to load");
                    return HandActionType.UNKNOWN;
            }
        }

        private static List<ActionTypeEnum> GetObligatoryActions(ActionSettings actionSettings)
        {
            List<ActionTypeEnum> list = new List<ActionTypeEnum>();
            if (actionSettings.FirstType == ActionTypeEnum.Any && actionSettings.SecondType == ActionTypeEnum.Any && actionSettings.FirstType == ActionTypeEnum.Any && actionSettings.FourthType == ActionTypeEnum.Any)
                return list;

            list.Add(actionSettings.FirstType);

            if (actionSettings.SecondType == ActionTypeEnum.Any && actionSettings.ThirdType == ActionTypeEnum.Any && actionSettings.FourthType == ActionTypeEnum.Any)
                return list;

            list.Add(actionSettings.SecondType);

            if (actionSettings.ThirdType == ActionTypeEnum.Any && actionSettings.FourthType == ActionTypeEnum.Any)
                return list;

            list.Add(actionSettings.ThirdType);

            if (actionSettings.FourthType == ActionTypeEnum.Any)
                return list;

            list.Add(actionSettings.FourthType);

            return list;
        }

        #endregion

        private static List<Playerstatistic> FilterByPlayersCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            if (settings.TagPlayer.Include && settings.FishPlayer.Include && settings.GamblerPlayer.Include && settings.NitPlayer.Include && settings.WhalePlayer.Include && settings.LagPlayer.Include && settings.RockPlayer.Include)
                return playerStatistics;

            if (!settings.TagPlayer.Include)
            {
                bool inRange = CheckPlayerInRange(settings.TagPlayer);
                if (inRange)
                    return new List<Playerstatistic>();
            }
            if (!settings.FishPlayer.Include)
            {
                bool inRange = CheckPlayerInRange(settings.FishPlayer);
                if (inRange)
                    return new List<Playerstatistic>();
            }
            if (!settings.GamblerPlayer.Include)
            {
                bool inRange = CheckPlayerInRange(settings.GamblerPlayer);
                if (inRange)
                    return new List<Playerstatistic>();
            }
            if (!settings.WhalePlayer.Include)
            {
                bool inRange = CheckPlayerInRange(settings.WhalePlayer);
                if (inRange)
                    return new List<Playerstatistic>();
            }
            if (!settings.LagPlayer.Include)
            {
                bool inRange = CheckPlayerInRange(settings.LagPlayer);
                if (inRange)
                    return new List<Playerstatistic>();
            }
            if (!settings.NitPlayer.Include)
            {
                bool inRange = CheckPlayerInRange(settings.NitPlayer);
                if (inRange)
                    return new List<Playerstatistic>();
            }
            if (!settings.RockPlayer.Include)
            {
                bool inRange = CheckPlayerInRange(settings.RockPlayer);
                if (inRange)
                    return new List<Playerstatistic>();
            }

            return playerStatistics; //if not in any excluded range return incomning data
        }

        private static bool CheckPlayerInRange(PlayerObject playerObj)
        {
            HudLightIndicators hudLightIndicators = new HudLightIndicators(StaticStorage.Playerstatistics);

            if ((hudLightIndicators.VPIPObject.Value < (decimal)playerObj.VpIpMax && hudLightIndicators.VPIPObject.Value > (decimal)playerObj.VpIpMin) && (hudLightIndicators.AggPrObject.Value < (decimal)playerObj.AggMax && hudLightIndicators.AggPrObject.Value > (decimal)playerObj.AggMin) && (hudLightIndicators.PFRObject.Value < (decimal)playerObj.PfrMax && hudLightIndicators.PFRObject.Value > (decimal)playerObj.PfrMin) && (hudLightIndicators.ThreeBetObject.Value < (decimal)playerObj.ThreeBetMax && hudLightIndicators.ThreeBetObject.Value > (decimal)playerObj.ThreeBetMin) && (hudLightIndicators.WTSDObject.Value < (decimal)playerObj.WtsdMax && hudLightIndicators.WTSDObject.Value > (decimal)playerObj.WtsdMin) && (hudLightIndicators.WSSDObject.Value < (decimal)playerObj.WsdMax && hudLightIndicators.WSSDObject.Value > (decimal)playerObj.WsdMin) && (hudLightIndicators.WSWSFObject.Value < (decimal)playerObj.WwsfMax && hudLightIndicators.WSWSFObject.Value > (decimal)playerObj.WwsfMin))
            {
                return true;
            }

            return false;
        }

        private static List<Playerstatistic> FilterByHoleCardCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            // list if excluded hole cards
            if (settings.ExcludedCardsList.Count == 0)
                return playerStatistics;

            List<long> list = new List<long>();

            foreach (string excludedCard in settings.ExcludedCardsList)
            {
                list.Add(HoleCardsHelper.GetHoleCardValue(excludedCard));
            }

            //list of the selected hole cards
            List<long> selectedCards = new List<long>();

            for (int i = 1; i < 170; i++)
            {
                if (!list.Contains(i))
                {
                    selectedCards.Add(i);
                }
            }

            return playerStatistics.Where(x => x.Cards.Length > 0 && selectedCards.Contains(HoleCardsHelper.ConverterCardsToSuitedUnsuited(x.Cards))).ToList();
        }

        private static List<Playerstatistic> FilterByStakeCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = playerStatistics;

            if (settings.ExcludedStakes.Count == 0)
                return playerStatistics;

            foreach (Stake stake in StaticStorage.Stakes)
            {
                //ignoring any excluded stake
                if (settings.ExcludedStakes.Count(p => p.Name == stake.Name) > 0)
                    continue;

                fileteredList = fileteredList.Where(x => x.PokergametypeId == stake.ID && x.BigBlind == stake.StakeValue).ToList();
            }

            return fileteredList;
        }

        private static List<Playerstatistic> FilterByNoOfPlayerCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            int low = 0, high = 0;

            if (settings.PlayersNoCustom)
            {
                low = settings.PlayersNoMinVal;
                high = settings.PlayersNoMaxVal;
            }
            else
            {
                if (settings.PlayersNoHeadsUp)
                    low = 2;
                else if (settings.PlayersNo34)
                    low = 3;
                else if (settings.PlayersNo56)
                    low = 5;
                else if (settings.PlayersNoMax)
                    low = 7;
                if (settings.PlayersNoMax)
                    high = 10;
                else if (settings.PlayersNo56)
                    high = 6;
                else if (settings.PlayersNo34)
                    high = 4;
                else if (settings.PlayersNoHeadsUp)
                    high = 2;
            }

            List<Playerstatistic> filteredList = new List<Playerstatistic>();
            try
            {
                filteredList.AddRange(playerStatistics.Where(x => x.HandHistory.GameDescription?.SeatType.MaxPlayers <= high && x.HandHistory.GameDescription?.SeatType.MaxPlayers >= low));
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(NoteManager), "FilterByNoOfPlayerCondition method exception", ex);                
            }

            return filteredList;
        }

        private static List<Playerstatistic> FilterByPositionRaiserCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            //if everything is unchecked return entry list of Playerstatistics
            if (!settings.PositionBBRaiser && !settings.PositionButtonRaiser && !settings.PositionCutoffRaiser && !settings.PositionEarlyRaiser && !settings.PositionMiddleRaiser && !settings.PositionSBRaiser)
                return playerStatistics;

            foreach (Playerstatistic playerStatistic in playerStatistics)
            {
                List<HandAction> facedHandActions = new List<HandAction>();

                foreach (HandAction hA in playerStatistic.HandHistory.Actions.Where(hA => hA.HandActionType != HandActionType.SMALL_BLIND && hA.HandActionType != HandActionType.BIG_BLIND))
                {
                    if (hA.PlayerName != playerStatistic.PlayerName)
                        facedHandActions.Add(hA);
                    else
                        break;
                }
                HandAction handAction = facedHandActions.FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);
                string positionFirstRaiser = "";
                if (handAction != null)
                    positionFirstRaiser = DAL.GetPlayerPosition(handAction.PlayerName, playerStatistic.GameNumber, playerStatistic.PokersiteId, playerStatistic.Time);

                if ((positionFirstRaiser == "SB" && settings.PositionSBRaiser) || (positionFirstRaiser == "BB" && settings.PositionBBRaiser) || (positionFirstRaiser == "EP" && settings.PositionEarlyRaiser) || (positionFirstRaiser == "MP" && settings.PositionMiddleRaiser) || (positionFirstRaiser == "CO" && settings.PositionCutoffRaiser) || (positionFirstRaiser == "BTN" && settings.PositionButtonRaiser))
                    fileteredList.Add(playerStatistic);
            }

            return fileteredList;
        }


        private static List<Playerstatistic> FilterByPositionThreeBetCondition(List<Playerstatistic> playerStatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            if (!settings.PositionBB3Bet && !settings.PositionSB3Bet && !settings.PositionButton3Bet && !settings.PositionCutoff3Bet && !settings.PositionEarly3Bet && !settings.PositionMiddle3Bet)
                return playerStatistics;

            foreach (Playerstatistic playerStatistic in playerStatistics)
            {
                List<HandAction> facedHandActions = new List<HandAction>();

                foreach (HandAction hA in playerStatistic.HandHistory.Actions.Where(hA => hA.HandActionType != HandActionType.SMALL_BLIND && hA.HandActionType != HandActionType.BIG_BLIND))
                {
                    if (hA.PlayerName != playerStatistic.PlayerName)
                        facedHandActions.Add(hA);
                    else
                        break;
                }

                int raisesNumber = facedHandActions.Count(x => x.HandActionType == HandActionType.RAISE);

                if (raisesNumber < 2)
                    continue;

                HandAction handAction = facedHandActions.Where(x => x.HandActionType == HandActionType.RAISE).ElementAt(1);

                string positionFirstRaiser = DAL.GetPlayerPosition(handAction?.PlayerName, playerStatistic.GameNumber, playerStatistic.PokersiteId, playerStatistic.Time);

                if ((positionFirstRaiser == "SB" && settings.PositionSB3Bet) || (positionFirstRaiser == "BB" && settings.PositionBB3Bet) || (positionFirstRaiser == "EP" && settings.PositionEarly3Bet) || (positionFirstRaiser == "MP" && settings.PositionMiddle3Bet) || (positionFirstRaiser == "CO" && settings.PositionCutoff3Bet) || (positionFirstRaiser == "BTN" && settings.PositionButton3Bet))
                    fileteredList.Add(playerStatistic);
            }

            return fileteredList;
        }

        private static List<Playerstatistic> FilterByPreflopFacingCondition(List<Playerstatistic> playerstatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            if (!settings.Facing1Limper && !settings.Facing1Raiser && !settings.Facing2PlusLimpers && !settings.Facing2Raisers && !settings.FacingRaisersCallers && !settings.FacingUnopened)
                return new List<Playerstatistic>();

            if (settings.Facing1Limper && settings.Facing1Raiser && settings.Facing2PlusLimpers && settings.Facing2Raisers && settings.FacingRaisersCallers && settings.FacingUnopened)
                return playerstatistics;

            if (settings.Facing1Limper)
                fileteredList.AddRange(playerstatistics.Where(x => x.FacingPreflop == EnumFacingPreflop.Limper).ToList());
            if (settings.Facing1Raiser)
                fileteredList.AddRange(playerstatistics.Where(x => x.FacingPreflop == EnumFacingPreflop.Raiser).ToList());
            if (settings.Facing2PlusLimpers)
                fileteredList.AddRange(playerstatistics.Where(x => x.FacingPreflop == EnumFacingPreflop.MultipleLimpers).ToList());
            if (settings.Facing2Raisers)
                fileteredList.AddRange(playerstatistics.Where(x => x.FacingPreflop == EnumFacingPreflop.ThreeBet).ToList()); //todo check if correct
            if (settings.FacingRaisersCallers)
                fileteredList.AddRange(playerstatistics.Where(x => x.FacingPreflop == EnumFacingPreflop.MultipleCallers).ToList()); //todo check if correct
            if (settings.FacingUnopened)
                fileteredList.AddRange(playerstatistics.Where(x => x.FacingPreflop == EnumFacingPreflop.Unopened).ToList());

            return fileteredList;
        }

        private static List<Playerstatistic> FilterByPositionCondition(List<Playerstatistic> playerstatistics, NoteSettingsObject settings)
        {
            List<Playerstatistic> fileteredList = new List<Playerstatistic>();

            if (!settings.PositionBB && !settings.PositionButton && !settings.PositionSB && !settings.PositionMiddle && !settings.PositionCutoff && !settings.PositionEarly)
                return new List<Playerstatistic>();
            if (settings.PositionSB)
                fileteredList.AddRange(playerstatistics.Where(x => x.Position == EnumPosition.SB).ToList());
            if (settings.PositionBB)
                fileteredList.AddRange(playerstatistics.Where(x => x.Position == EnumPosition.BB).ToList());
            if (settings.PositionButton)
                fileteredList.AddRange(playerstatistics.Where(x => x.Position == EnumPosition.BTN).ToList());
            if (settings.PositionCutoff)
                fileteredList.AddRange(playerstatistics.Where(x => x.Position == EnumPosition.CO).ToList());
            if (settings.PositionEarly)
                fileteredList.AddRange(playerstatistics.Where(x => x.Position == EnumPosition.EP || x.Position == EnumPosition.UTG || x.Position == EnumPosition.UTG_1 || x.Position == EnumPosition.UTG_2).ToList());
            if (settings.PositionMiddle)
                fileteredList.AddRange(playerstatistics.Where(x => x.Position == EnumPosition.MP || x.Position == EnumPosition.MP1 || x.Position == EnumPosition.MP2 || x.Position == EnumPosition.MP3).ToList());

            return fileteredList;
        }

        #region Filters


        //todo check what filter to keep filters or filtersComparison
        private static List<Playerstatistic> FilterByAllSelectedFilters(List<Playerstatistic> playerstatistics, ICollection<FilterObject> filters, ICollection<FilterObject> filtersComparison)
        {
            List<Playerstatistic> fileteredList = playerstatistics;
            if (filters.Count == 0 && filtersComparison.Count == 0)
                return fileteredList;

            //foreach (FilterObject filter in filtersComparison)
            //    fileteredList = NoteManagerHelper.FilterByASelectedFilter(fileteredList, filter);

            foreach (FilterObject filter in filters)
                fileteredList = NoteManagerHelper.FilterByASelectedFilter(fileteredList, filter);

            return fileteredList;
        }



        #endregion

        /// <summary>
        /// build note(string) from available data
        /// </summary>
        /// <param name="note">this is note (NoteObject)</param>        
        /// <param name="selectedPlayerstatistics">List of statistics that participate in creation of note (string)</param>
        /// <returns></returns>
        private static DatabaseNote BuildNote(NoteObject note, List<Playerstatistic> selectedPlayerstatistics)
        {
            string message = note.Name + " ";
            List<string> cards = new List<string>();

            foreach (Playerstatistic playerstatistic in selectedPlayerstatistics.Where(x => !string.IsNullOrEmpty(x.Cards))) //todo check if we need to display cases when hole cards are absent
                cards.Add(playerstatistic.Cards);

            string card = "{"; //beginning of the note message
            foreach (string c in cards.Where(x => !string.IsNullOrEmpty(x)))
                if (cards.IndexOf(c) != cards.Count - 1)
                    card += c + ", "; //midle part of the note message 
                else
                    card += c + "}"; //closing part of the note message

            message += card + $" ({cards.Count})";

            if (card.Length == 0) //returning an empty object if note has no information to display
                return new DatabaseNote();

            long playerId;
            long.TryParse(StaticStorage.CurrentPlayer, out playerId);

            DatabaseNote dbNote = new DatabaseNote
            {
                Message = message,
                PlayerID = playerId
            };

            return dbNote;
        }
    }
}
