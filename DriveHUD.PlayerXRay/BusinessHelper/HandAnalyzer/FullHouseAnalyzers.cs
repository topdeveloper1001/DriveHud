using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    public class FullHousePocketPairNoTripsOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count != 2 || !boardCards.Any())
                return false;

            if (!HandAnalyzerHelpers.HasPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.Rank).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips != null)
            {
                if (groupedCards.Where(x => x.Key != trips.Key).Any(x => x.Count() >= 2))
                {
                    return trips.First().Rank == playerCards.First().Rank;
                }
            }

            return false;
        }
    }

    public class FullHousePocketPairTripsOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count != 2 || !boardCards.Any())
                return false;

            if (!HandAnalyzerHelpers.HasPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.Rank).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips != null)
            {
                var pair = groupedCards.Where(x => x.Key != trips.Key).FirstOrDefault(x => x.Count() >= 2);
                if (pair != null && pair.Key == playerCards.First().Rank)
                {
                    return true;
                }
            }

            return false;
        }                    
    }

    public class FullHouseNoPocketPairNoTripsOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count != 2 || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))       
                return false;

            if (boardCards.GroupBy(x => x.Rank).Any(x => x.Count() >= 3)) 
                return false;  

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            var groupedCards = allCards.GroupBy(x => x.Rank).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips != null)
            {
                var pair = groupedCards.Where(x => x.Key != trips.Key).Where(x => x.Count() >= 2);
                if (playerCards.Any(x => x.Rank == trips.Key) && playerCards.Any(x => pair.Any(y => y.Key == x.Rank)))
                {
                    return true;
                }
            }

            return false;
        }
    }


    public class FullHouseOneHoleCardTripsOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count != 2 || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
                return false;

            if (!HandAnalyzerHelpers.IsNoOfKind(boardCards,3))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.Rank).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips != null)
            {
                var pair = groupedCards.Where(x => x.Key != trips.Key).Where(x => x.Count() >= 2);
                
                if (playerCards.Any(x => pair.Any(y => y.Key == x.Rank)))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class FullHouseOneHoleCardTopPairNoTripsOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count != 2 || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
                return false;

            if (boardCards.GroupBy(x => x.Rank).Any(x => x.Count() >= 3))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            var groupedCards = allCards.GroupBy(x => x.Rank).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips != null && trips.FirstOrDefault()?.Rank == boardCards.OrderByDescending(x =>x.Rank).FirstOrDefault()?.Rank)
            {
                var pair = groupedCards.Where(x => x.Key != trips.Key).Where(x => x.Count() >= 2);
                
                if (playerCards.All(x => pair.All(y => y.Key != x.Rank)))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class FullHouseOneHoleCardLowPairNoTripsOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count != 2 || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
                return false;

            if (boardCards.GroupBy(x => x.Rank).Any(x => x.Count() >= 3))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            var groupedCards = allCards.GroupBy(x => x.Rank).OrderByDescending(x => x.Key);

            var trips = groupedCards.LastOrDefault(x => x.Count() == 3);
            if (trips != null && trips.FirstOrDefault()?.Rank == boardCards.OrderBy(x => x.Rank).FirstOrDefault()?.Rank)
            {
                var pair = groupedCards.Where(x => x.Key != trips.Key).Where(x => x.Count() >= 2);

                if (playerCards.All(x => pair.All(y => y.Key != x.Rank)))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class FullHouseNoHoleCardsOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (boardCards == null || !boardCards.Any())
                return false;

            var groupedCards = boardCards.GroupBy(x => x.Rank).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips == null)
            {
                return false;
            }

            var pair = groupedCards.Where(x => x.Key != trips.Key).FirstOrDefault(x => x.Count() >= 2);
            if (pair != null)
            {
                return !playerCards.Any(x => x.Rank == trips.Key || x.Rank == pair.Key);
            }

            return false;

        }
    }

}
