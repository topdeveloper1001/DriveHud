using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.DataAccessHelper.DriveHUD;
using DriveHUD.PlayerXRay.DataTypes;

namespace DriveHUD.PlayerXRay.BusinessHelper.OtherAnalyzers
{
    public class BetSizeAnalyzers
    {
        const decimal ONE_HUNDRED_PERCENTS = 100;

        //here is possible: a player makes check then hero makes bet then a player makes raise and hero is facing raise 
        public static decimal GetFacingRaiseSizePot(Playerstatistic playerstatistic, Street targetStreet)
        {
            if (targetStreet == Street.Flop || targetStreet == Street.Turn || targetStreet == Street.River)
            {
                HandAction raiseAction =
                  playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                      .FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);

                if (raiseAction == null || raiseAction.PlayerName == playerstatistic.PlayerName)
                    return -1;

                HandAction heroAction = playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                    .SkipWhile(x => x != raiseAction)
                    .FirstOrDefault( x => x != raiseAction && x.HandActionType != HandActionType.FOLD || x.PlayerName == playerstatistic.PlayerName);

                if (heroAction?.PlayerName == playerstatistic.PlayerName)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(raiseAction.Amount)
                        / playerstatistic.HandHistory.Actions.TakeWhile(x => x != heroAction).Sum(x => Math.Abs(x.Amount));
            }


            //calculations in this part are different  from flop, turn or river. 
            //here we take first hero action facing, excluding hero actions as BB or SB 
            if (targetStreet == Street.Preflop)     
                if (playerstatistic.FacingPreflop == EnumFacingPreflop.Raiser)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(playerstatistic.HandHistory.Actions.FirstOrDefault(x => x.HandActionType == HandActionType.RAISE).Amount)
                        / playerstatistic.HandHistory.Actions.TakeWhile(x => x.HandActionType != HandActionType.RAISE).Sum(x => Math.Abs(x.Amount));
  
            return -1;
        }

        public static decimal GetFacingBetSizePot(Playerstatistic playerstatistic, Street targetStreet)
        {
            if (targetStreet == Street.Flop || targetStreet == Street.Turn || targetStreet == Street.River)
            {
                HandAction betAction =
                    playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                                                       .SkipWhile(x => x.HandActionType != HandActionType.BET)
                                                       .TakeWhile(x => x.PlayerName != playerstatistic.PlayerName)
                                                       .LastOrDefault(x => x.HandActionType == HandActionType.BET || x.HandActionType == HandActionType.RAISE);

                if (betAction == null || betAction.PlayerName == playerstatistic.PlayerName)
                    return -1;

                HandAction heroAction = playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                                                                           .SkipWhile(x => x != betAction)
                                                                           .FirstOrDefault(x => x != betAction && x.HandActionType != HandActionType.FOLD || x.PlayerName == playerstatistic.PlayerName);


                if (heroAction?.PlayerName == playerstatistic.PlayerName)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(betAction.Amount)/ playerstatistic.HandHistory.Actions.TakeWhile(x => x != heroAction).Sum(x => Math.Abs(x.Amount));

            }

            return -1;
        }



        public static decimal GetFacingBetAndDidRaiseSizePot(Playerstatistic playerstatistic, Street targetStreet)
        {
            if (targetStreet == Street.Flop || targetStreet == Street.Turn || targetStreet == Street.River)
            {
                //HandAction betAction =
                //    playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                //                                       .FirstOrDefault(x => x.HandActionType == HandActionType.BET);
                HandAction betAction =
                    playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                                                       .SkipWhile(x => x.HandActionType != HandActionType.BET)
                                                       .TakeWhile(x => x.PlayerName != playerstatistic.PlayerName)
                                                       .LastOrDefault(x => x.HandActionType == HandActionType.BET || x.HandActionType == HandActionType.RAISE);

                if (betAction == null || betAction.PlayerName == playerstatistic.PlayerName)
                    return -1;

                HandAction heroAction = playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                                                                           .SkipWhile(x => x != betAction)
                                                                           .FirstOrDefault(x => x != betAction && x.HandActionType != HandActionType.FOLD || x.PlayerName == playerstatistic.PlayerName);

                if (heroAction != null && heroAction.PlayerName == playerstatistic.PlayerName && heroAction.HandActionType == HandActionType.RAISE)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(heroAction.Amount) / playerstatistic.HandHistory.Actions.TakeWhile(x => x != heroAction).Sum(x => Math.Abs(x.Amount));
            }


            //no bet here just check if facing limpers or unopened
            if (targetStreet == Street.Preflop)
            {
                HandAction raiseAction = null;
                if (playerstatistic.FacingPreflop == EnumFacingPreflop.Limper ||
                    playerstatistic.FacingPreflop == EnumFacingPreflop.MultipleLimpers ||
                    playerstatistic.FacingPreflop == EnumFacingPreflop.Unopened)
                    raiseAction = playerstatistic.HandHistory.Actions.FirstOrDefault(x => x.Street == Street.Preflop && x.HandActionType == HandActionType.RAISE);

                if (raiseAction != null && raiseAction.PlayerName == playerstatistic.PlayerName)
                    return ONE_HUNDRED_PERCENTS * Math.Abs(raiseAction.Amount) / playerstatistic.HandHistory.Actions.TakeWhile(x => x != raiseAction).Sum(x => Math.Abs(x.Amount));
            }

            return -1;
        }









    }
}
