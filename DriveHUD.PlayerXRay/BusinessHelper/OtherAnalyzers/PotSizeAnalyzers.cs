using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.DataAccessHelper.DriveHUD;
using AcePokerSolutions.DataTypes;

namespace AcePokerSolutions.BusinessHelper.OtherAnalyzers
{
    public class PotSizeAnalyzers
    {
        private static decimal PotSize(List<HandAction> handActions)
        {
            return Math.Abs(handActions.Sum(x => x.Amount));
        }


        public static List<Playerstatistic> BBPotSizeisBiggerThan(List<Playerstatistic> incomingPlayerstatistics, decimal potValue, Street targetStreet)
        {
            List<Playerstatistic> result = new List<Playerstatistic>();
            foreach (Playerstatistic playerstatistic in incomingPlayerstatistics)
            {
                HandAction firstOnStreetAction =
                    playerstatistic.HandHistory.Actions.FirstOrDefault(
                        x => x.Street == targetStreet && x.PlayerName == playerstatistic.PlayerName);
                if (firstOnStreetAction == null)
                    continue;

                if (PotSize(playerstatistic.HandHistory.Actions.TakeWhile(x => x != firstOnStreetAction).ToList()) > (decimal)potValue * playerstatistic.BigBlind)
                    result.Add(playerstatistic);
            }
            return result;
        }


        public static List<Playerstatistic> BBPotSizeisLessThan(List<Playerstatistic> incomingPlayerstatistics, decimal potValue, Street targetStreet)
        {
            List<Playerstatistic> result = new List<Playerstatistic>();
            foreach (Playerstatistic playerstatistic in incomingPlayerstatistics)
            {
                HandAction firstOnStreetAction =
                    playerstatistic.HandHistory.Actions.FirstOrDefault(
                        x => x.Street == targetStreet && x.PlayerName == playerstatistic.PlayerName);
                if (firstOnStreetAction == null)
                    continue;

                if (PotSize(playerstatistic.HandHistory.Actions.TakeWhile(x => x != firstOnStreetAction).ToList()) < (decimal)potValue * playerstatistic.BigBlind)
                    result.Add(playerstatistic);
            }
            return result;
        }


        public static decimal StackPotRatio(Playerstatistic playerstatistic, Street targetStreet)
        {
            List<HandAction> streetActions = playerstatistic.HandHistory.Actions.Where(x => x.Street == targetStreet).ToList();
            List<string> playerNames = streetActions.GroupBy(x => x.PlayerName).Select(x => x.Key).ToList();  

            if (playerNames.All(x => x != playerstatistic.PlayerName))  
                return 0;  
           
            decimal potAmount = 0;
            
            switch (targetStreet)
            {
                case Street.Flop:
                    potAmount = Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Preflop).Sum(x => x.Amount));
                    break;
                case Street.Turn:
                    potAmount = Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Preflop).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Flop).Sum(x => x.Amount));
                    break;
                case Street.River:
                    potAmount = Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Preflop).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Flop).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Turn).Sum(x => x.Amount));
                    break;
            }

            decimal playerPutInAmount = 0;
            List<decimal> playerStreetStackAmount = new List<decimal>();

            foreach (string player in playerNames)
            { 
                switch (targetStreet)
                {
                    case Street.Flop: 
                        playerPutInAmount = Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Preflop && x.PlayerName == player).Sum(x => x.Amount));
                        break;
                    case Street.Turn:
                        playerPutInAmount = Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Preflop && x.PlayerName == player).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Flop && x.PlayerName == player).Sum(x => x.Amount));
                        break;
                    case Street.River:  
                        playerPutInAmount = Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Preflop && x.PlayerName == player).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Flop && x.PlayerName == player).Sum(x => x.Amount)) + Math.Abs(playerstatistic.HandHistory.Actions.Where(x => x.Street == Street.Turn && x.PlayerName == player).Sum(x => x.Amount));
                        break;
                }

                DataAccessHelper.DriveHUD.Player playerHand = playerstatistic.HandHistory.Players.FirstOrDefault(x => x.PlayerName == player);
                if (playerHand == null)
                    return 0;

                playerStreetStackAmount.Add(playerHand.StartingStack - playerPutInAmount);
            }

            decimal effectiveStackSize = playerStreetStackAmount.Min();

            if (potAmount != 0)
                return effectiveStackSize/ potAmount;

            return 0;
        }
    }
}
