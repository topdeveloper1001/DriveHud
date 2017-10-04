using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.DataAccessHelper.DriveHUD;
using DriveHUD.PlayerXRay.DataTypes;

namespace DriveHUD.PlayerXRay.BusinessHelper.OtherAnalyzers
{
    public class NotClassifiedAnalyzers
    {
        public static bool WasRaisedAnalyzer(Playerstatistic playerstatistic, Street targetStreet)
        {
            HandAction raiseAction =
              playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                  .FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);

            if (raiseAction == null || raiseAction.PlayerName == playerstatistic.PlayerName)
                return false;

            return playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                .SkipWhile(x => x != raiseAction).Skip(1)
                .Any(x => x.PlayerName == playerstatistic.PlayerName);
        }

        public static bool WasBetIntoAnalyzer(Playerstatistic playerstatistic, Street targetStreet)
        {
            HandAction betAction =
              playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                  .FirstOrDefault(x => x.HandActionType == HandActionType.BET);

            if (betAction == null || betAction.PlayerName == playerstatistic.PlayerName)
                return false;

            return playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                .SkipWhile(x => x != betAction).Skip(1)
                .Any(x => x.PlayerName == playerstatistic.PlayerName);

        }

        public static bool WasCheckAndRaiseAnalyzer(Playerstatistic playerstatistic, Street targetStreet)
        {
            HandAction raiseAction =
              playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                  .FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);

            if (raiseAction == null || raiseAction.PlayerName == playerstatistic.PlayerName)
                return false;

            return playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                                                      .TakeWhile(x => x != raiseAction)
                                                      .Count(x => x.HandActionType == HandActionType.CHECK) == 1
                && playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet)
                                                      .SkipWhile(x => x != raiseAction).Skip(1)
                                                      .Any(x => x.PlayerName == playerstatistic.PlayerName);

        }
    }
}
