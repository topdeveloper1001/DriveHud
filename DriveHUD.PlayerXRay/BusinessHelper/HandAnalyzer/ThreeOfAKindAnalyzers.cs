using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using HandHistories.Objects.Cards;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    #region Three of a kind
    public class ThreeOfAKindTopSetAnalyzer
    {  
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (!HandAnalyzerHelpers.HasPair(playerCards, 1) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            var topBoardCardRank = boardCards.Max(x => x.Rank);

            return playerCards.Any(x => x.Rank == topBoardCardRank);
        }
    }

    public class ThreeOfAKindSecondSetAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (!HandAnalyzerHelpers.HasPair(playerCards, 1) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            var secondBoardCardRank = boardCards.OrderByDescending(x => x.Rank).Distinct().ElementAt(1).Rank;  

            return playerCards.Any(x => x.Rank == secondBoardCardRank);
        }
    }

    public class ThreeOfAKindLowSetAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (!HandAnalyzerHelpers.HasPair(playerCards, 1) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            var lowCardRank = boardCards.Min(x => x.Rank);

            return playerCards.Any(x => x.Rank == lowCardRank);
        }
    }
    #endregion

    #region Trips
    public class ThreeOfAKindTripsTopSetHighKickerAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        { 
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || HandAnalyzerHelpers.IsNoOfKind(boardCards, 3) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            if (boardCards.Count(x => x.Rank == boardCards.Max(b => b.Rank)) == 2
                && playerCards.Any(x => x.Rank == boardCards.Max(b => b.Rank)))
            {
                var kicker = playerCards.FirstOrDefault(p => boardCards.All(b => b.Rank != p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(kicker);
                }
            }

            return false;
        }        
    }

    public class ThreeOfAKindTripsTopSetWeakKickerAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || HandAnalyzerHelpers.IsNoOfKind(boardCards, 3) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            if (boardCards.Count(x => x.Rank == boardCards.Max(b => b.Rank)) == 2
                && playerCards.Any(x => x.Rank == boardCards.Max(b => b.Rank)))
            {
                var kicker = playerCards.FirstOrDefault(p => boardCards.All(b => b.Rank != p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(kicker);
                }
            }

            return false;
        }
    }

    public class ThreeOfAKindTripsSecondSetHighKickerAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || HandAnalyzerHelpers.IsNoOfKind(boardCards, 3) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            var secondSetCard = boardCards.OrderByDescending(x => x.Rank).ElementAt(1);
            if (boardCards.Count(x => x.Rank == secondSetCard.Rank) == 2
                && playerCards.Any(x => x.Rank == secondSetCard.Rank))
            {
                var kicker = playerCards.FirstOrDefault(p => boardCards.All(b => b.Rank != p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(kicker);
                }
            }

            return false;
        }
    }

    public class ThreeOfAKindTripsSecondSetWeakKickerAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || HandAnalyzerHelpers.IsNoOfKind(boardCards, 3) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            var secondSetCard = boardCards.OrderByDescending(x => x.Rank).ElementAt(1);
            if (boardCards.Count(x => x.Rank == secondSetCard.Rank) == 2
                && playerCards.Any(x => x.Rank == secondSetCard.Rank))
            {
                var kicker = playerCards.FirstOrDefault(p => boardCards.All(b => b.Rank != p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(kicker);
                }
            }

            return false;
        }
    }

    public class ThreeOfAKindTripsLowSetHighKickerAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

           if (HandAnalyzerHelpers.HasPair(playerCards, 1) || HandAnalyzerHelpers.IsNoOfKind(boardCards, 3) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            var lowSetCardRank = boardCards.Min(x => x.Rank);
            if (boardCards.Count(x => x.Rank == lowSetCardRank) == 2
                && playerCards.Any(x => x.Rank == lowSetCardRank))
            {
                var kicker = playerCards.FirstOrDefault(p => boardCards.All(b => b.Rank != p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(kicker);
                }
            }

            return false;
        }
    }

    public class ThreeOfAKindTripsLowSetWeakKickerAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

           
            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || HandAnalyzerHelpers.IsNoOfKind(boardCards, 3) || !HandAnalyzerHelpers.IsNoOfKind(allCards, 3))
            {
                return false;
            }

            var lowSetCardRank = boardCards.Min(x => x.Rank);
            if (boardCards.Count(x => x.Rank == lowSetCardRank) == 2
                && playerCards.Any(x => x.Rank == lowSetCardRank))
            {
                var kicker = playerCards.FirstOrDefault(p => boardCards.All(b => b.Rank != p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(kicker);
                }
            }

            return false;
        }
    }


    #endregion


    public class ThreeOfAKindOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (boardCards == null || !boardCards.Any())
                return false;

            return HandAnalyzerHelpers.IsNoOfKind(boardCards, 3);
        }    
    }
}
