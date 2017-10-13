using DriveHUD.Entities;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.OtherAnalyzers
{
    public class PotSizeAnalyzers
    {
        private static decimal PotSize(List<HandAction> handActions)
        {
            return Math.Abs(handActions.Sum(x => x.Amount));
        }


        public static List<PlayerstatisticExtended> BBPotSizeisBiggerThan(List<PlayerstatisticExtended> incomingPlayerstatistics, decimal potValue, Street targetStreet)
        {
            List<PlayerstatisticExtended> result = new List<PlayerstatisticExtended>();

            foreach (var playerstatistic in incomingPlayerstatistics)
            {
                HandAction firstOnStreetAction =
                    playerstatistic.HandHistory.HandActions.FirstOrDefault(
                        x => x.Street == targetStreet && x.PlayerName == playerstatistic.Playerstatistic.PlayerName);
                if (firstOnStreetAction == null)
                    continue;

                if (PotSize(playerstatistic.HandHistory.HandActions.TakeWhile(x => x != firstOnStreetAction).ToList()) > (decimal)potValue * playerstatistic.Playerstatistic.BigBlind)
                    result.Add(playerstatistic);
            }
            return result;
        }


        public static List<PlayerstatisticExtended> BBPotSizeisLessThan(List<PlayerstatisticExtended> incomingPlayerstatistics, decimal potValue, Street targetStreet)
        {
            List<PlayerstatisticExtended> result = new List<PlayerstatisticExtended>();

            foreach (var playerstatistic in incomingPlayerstatistics)
            {
                HandAction firstOnStreetAction =
                    playerstatistic.HandHistory.HandActions.FirstOrDefault(
                        x => x.Street == targetStreet && x.PlayerName == playerstatistic.Playerstatistic.PlayerName);
                if (firstOnStreetAction == null)
                    continue;

                if (PotSize(playerstatistic.HandHistory.HandActions.TakeWhile(x => x != firstOnStreetAction).ToList()) < (decimal)potValue * playerstatistic.Playerstatistic.BigBlind)
                    result.Add(playerstatistic);
            }
            return result;
        }


        public static decimal StackPotRatio(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            List<HandAction> streetActions = playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet).ToList();
            List<string> playerNames = streetActions.GroupBy(x => x.PlayerName).Select(x => x.Key).ToList();

            if (playerNames.All(x => x != playerstatistic.Playerstatistic.PlayerName))
                return 0;

            decimal potAmount = 0;

            switch (targetStreet)
            {
                case Street.Flop:
                    potAmount = Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Preflop).Sum(x => x.Amount));
                    break;
                case Street.Turn:
                    potAmount = Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Preflop).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Flop).Sum(x => x.Amount));
                    break;
                case Street.River:
                    potAmount = Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Preflop).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Flop).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Turn).Sum(x => x.Amount));
                    break;
            }

            decimal playerPutInAmount = 0;
            List<decimal> playerStreetStackAmount = new List<decimal>();

            foreach (string player in playerNames)
            {
                switch (targetStreet)
                {
                    case Street.Flop:
                        playerPutInAmount = Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Preflop && x.PlayerName == player).Sum(x => x.Amount));
                        break;
                    case Street.Turn:
                        playerPutInAmount = Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Preflop && x.PlayerName == player).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Flop && x.PlayerName == player).Sum(x => x.Amount));
                        break;
                    case Street.River:
                        playerPutInAmount = Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Preflop && x.PlayerName == player).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Flop && x.PlayerName == player).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.HandActions.Where(x => x.Street == Street.Turn && x.PlayerName == player).Sum(x => x.Amount));
                        break;
                }

                var playerHand = playerstatistic.HandHistory.Players.FirstOrDefault(x => x.PlayerName == player);

                if (playerHand == null)
                {
                    return 0;
                }

                playerStreetStackAmount.Add(playerHand.StartingStack - playerPutInAmount);
            }

            decimal effectiveStackSize = playerStreetStackAmount.Min();

            if (potAmount != 0)
                return effectiveStackSize / potAmount;

            return 0;
        }
    }
}
