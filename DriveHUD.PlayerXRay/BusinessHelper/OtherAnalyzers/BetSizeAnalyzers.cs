using DriveHUD.Entities;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.OtherAnalyzers
{
    public class BetSizeAnalyzers
    {
        const decimal ONE_HUNDRED_PERCENTS = 100;

        //here is possible: a player makes check then hero makes bet then a player makes raise and hero is facing raise 
        public static decimal GetFacingRaiseSizePot(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            if (targetStreet == Street.Flop || targetStreet == Street.Turn || targetStreet == Street.River)
            {
                HandAction raiseAction =
                  playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                      .FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);

                if (raiseAction == null || raiseAction.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                    return -1;

                HandAction heroAction = playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                    .SkipWhile(x => x != raiseAction)
                    .FirstOrDefault(x => x != raiseAction && x.HandActionType != HandActionType.FOLD || x.PlayerName == playerstatistic.Playerstatistic.PlayerName);

                if (heroAction?.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(raiseAction.Amount)
                        / playerstatistic.HandHistory.HandActions.TakeWhile(x => x != heroAction).Sum(x => Math.Abs(x.Amount));
            }


            //calculations in this part are different  from flop, turn or river. 
            //here we take first hero action facing, excluding hero actions as BB or SB 
            if (targetStreet == Street.Preflop)
                if (playerstatistic.Playerstatistic.FacingPreflop == EnumFacingPreflop.Raiser)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(playerstatistic.HandHistory.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.RAISE).Amount)
                        / playerstatistic.HandHistory.HandActions.TakeWhile(x => x.HandActionType != HandActionType.RAISE).Sum(x => Math.Abs(x.Amount));

            return -1;
        }

        public static decimal GetFacingBetSizePot(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            if (targetStreet == Street.Flop || targetStreet == Street.Turn || targetStreet == Street.River)
            {
                HandAction betAction =
                    playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                                                       .SkipWhile(x => x.HandActionType != HandActionType.BET)
                                                       .TakeWhile(x => x.PlayerName != playerstatistic.Playerstatistic.PlayerName)
                                                       .LastOrDefault(x => x.HandActionType == HandActionType.BET || x.HandActionType == HandActionType.RAISE);

                if (betAction == null || betAction.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                    return -1;

                HandAction heroAction = playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                                                                           .SkipWhile(x => x != betAction)
                                                                           .FirstOrDefault(x => x != betAction && x.HandActionType != HandActionType.FOLD || x.PlayerName == playerstatistic.Playerstatistic.PlayerName);


                if (heroAction?.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(betAction.Amount) / playerstatistic.HandHistory.HandActions.TakeWhile(x => x != heroAction).Sum(x => Math.Abs(x.Amount));

            }

            return -1;
        }



        public static decimal GetFacingBetAndDidRaiseSizePot(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            if (targetStreet == Street.Flop || targetStreet == Street.Turn || targetStreet == Street.River)
            {
                //HandAction betAction =
                //    playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                //                                       .FirstOrDefault(x => x.HandActionType == HandActionType.BET);
                HandAction betAction =
                    playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                                                       .SkipWhile(x => x.HandActionType != HandActionType.BET)
                                                       .TakeWhile(x => x.PlayerName != playerstatistic.Playerstatistic.PlayerName)
                                                       .LastOrDefault(x => x.HandActionType == HandActionType.BET || x.HandActionType == HandActionType.RAISE);

                if (betAction == null || betAction.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                    return -1;

                HandAction heroAction = playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                                                                           .SkipWhile(x => x != betAction)
                                                                           .FirstOrDefault(x => x != betAction && x.HandActionType != HandActionType.FOLD || x.PlayerName == playerstatistic.Playerstatistic.PlayerName);

                if (heroAction != null && heroAction.PlayerName == playerstatistic.Playerstatistic.PlayerName && heroAction.HandActionType == HandActionType.RAISE)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(heroAction.Amount) / playerstatistic.HandHistory.HandActions.TakeWhile(x => x != heroAction).Sum(x => Math.Abs(x.Amount));
            }


            //no bet here just check if facing limpers or unopened
            if (targetStreet == Street.Preflop)
            {
                HandAction raiseAction = null;
                if (playerstatistic.Playerstatistic.FacingPreflop == EnumFacingPreflop.Limper ||
                    playerstatistic.Playerstatistic.FacingPreflop == EnumFacingPreflop.MultipleLimpers ||
                    playerstatistic.Playerstatistic.FacingPreflop == EnumFacingPreflop.Unopened)
                    raiseAction = playerstatistic.HandHistory.HandActions.FirstOrDefault(x => x.Street == Street.Preflop && x.HandActionType == HandActionType.RAISE);

                if (raiseAction != null && raiseAction.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(raiseAction.Amount) / playerstatistic.HandHistory.HandActions.TakeWhile(x => x != raiseAction).Sum(x => Math.Abs(x.Amount));
            }

            return -1;
        }
    }
}