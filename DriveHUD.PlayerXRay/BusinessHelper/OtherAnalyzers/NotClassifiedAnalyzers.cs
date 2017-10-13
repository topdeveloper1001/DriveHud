using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.OtherAnalyzers
{
    public class NotClassifiedAnalyzers
    {
        public static bool WasRaisedAnalyzer(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            HandAction raiseAction =
              playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                  .FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);

            if (raiseAction == null || raiseAction.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                return false;

            return playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                .SkipWhile(x => x != raiseAction).Skip(1)
                .Any(x => x.PlayerName == playerstatistic.Playerstatistic.PlayerName);
        }

        public static bool WasBetIntoAnalyzer(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            HandAction betAction =
              playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                  .FirstOrDefault(x => x.HandActionType == HandActionType.BET);

            if (betAction == null || betAction.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                return false;

            return playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                .SkipWhile(x => x != betAction).Skip(1)
                .Any(x => x.PlayerName == playerstatistic.Playerstatistic.PlayerName);

        }

        public static bool WasCheckAndRaiseAnalyzer(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            HandAction raiseAction =
              playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                  .FirstOrDefault(x => x.HandActionType == HandActionType.RAISE);

            if (raiseAction == null || raiseAction.PlayerName == playerstatistic.Playerstatistic.PlayerName)
                return false;

            return playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                                                      .TakeWhile(x => x != raiseAction)
                                                      .Count(x => x.HandActionType == HandActionType.CHECK) == 1
                && playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet)
                                                      .SkipWhile(x => x != raiseAction).Skip(1)
                                                      .Any(x => x.PlayerName == playerstatistic.Playerstatistic.PlayerName);

        }
    }
}
