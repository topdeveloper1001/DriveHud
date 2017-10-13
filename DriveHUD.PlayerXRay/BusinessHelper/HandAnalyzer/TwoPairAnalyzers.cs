using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using HandHistories.Objects.Cards;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{

    #region Both Card Paired
    public class TwoPairBothCardPairedAnalyzer
    {
        public virtual bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNoOfKind(allCards, 3) || HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
                return false;

            return playerCards.All(x => boardCards.Any(y => x.Rank == y.Rank));
        }
    }

    public class TwoPairTopTwoPairAnalyzer : TwoPairBothCardPairedAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                var topTwo = boardCards.OrderByDescending(x => x.Rank).Take(2).Distinct();
                var topCard = topTwo.ElementAt(0);
                var secondTopCard = topTwo.ElementAt(1);
                return playerCards.Any(x => x.Rank == topCard.Rank) && playerCards.Any(x => x.Rank == secondTopCard.Rank);
            }
            //return playerCards.All(x => topTwo.Any(t => t.Rank == x.Rank));    

            return false;
        }
    }

    public class TwoPairTopPairPlusPairAnalyzer : TwoPairBothCardPairedAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                var topTwo = boardCards.OrderByDescending(x => x.Rank).Take(2).Distinct();
                var topCard = topTwo.ElementAt(0);
                var secondTopCard = topTwo.ElementAt(1);
                return playerCards.Any(x => x.Rank == topCard.Rank) && playerCards.All(x => x.Rank != secondTopCard.Rank);
            }

            return false;
        }
    }

    public class TwoPairBottomPairPlusMiddlePairAnalyzer : TwoPairBothCardPairedAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                var topCard = boardCards.OrderByDescending(x => x.Rank).FirstOrDefault();
                var bottomCard = boardCards.OrderBy(x => x.Rank).FirstOrDefault();

                return playerCards.Any(x => x.Rank == bottomCard?.Rank) && playerCards.All(x => x.Rank != topCard?.Rank);
            }

            return false;
        }
    }

    #endregion

    #region Paired Board

    public class TwoPairCardPairedAndPairedBoardAnalyzer
    {
        public virtual bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNoOfKind(allCards, 3) || HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
                return false;

            return playerCards.Count(x => boardCards.Any(y => x.Rank == y.Rank)) == 1
                && HandAnalyzerHelpers.HasPair(boardCards, 1);
        }
    }

    public class TwoPairPairedBoardTopPairAnalyzer : TwoPairCardPairedAndPairedBoardAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var topCard = boardCards.Max(x => x.Rank);
                return playerCards.Any(x => x.Rank == topCard);
            }

            return false;
        }
    }

    public class TwoPairPairedBoardSecondPairAnalyzer : TwoPairCardPairedAndPairedBoardAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var secondCardOnBoard = boardCards.OrderByDescending(x => x.Rank).Distinct().ElementAt(1).Rank;

                return playerCards.Any(x => x.Rank == secondCardOnBoard);
            }

            return false;
        }
    }

    public class TwoPairPairedBoardBottomPairAnalyzer : TwoPairCardPairedAndPairedBoardAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var topCard = boardCards.Min(x => x.Rank);
                return playerCards.Any(x => x.Rank == topCard);
            }

            return false;
        }
    }


    #endregion

    #region Pocket Pair  and Board Pair

    public class TwoPairPocketPairPlusPairedBoard
    {
        public virtual bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;  

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNoOfKind(allCards, 3) || HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
                return false;

            return HandAnalyzerHelpers.HasPair(playerCards, 1) && HandAnalyzerHelpers.HasPair(boardCards, 1);
        }
    }


    public class TwoPairPocketPairPlusPairedBoardOverPair : TwoPairPocketPairPlusPairedBoard
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

                return playerCards.All(p => boardCards.All(b => b.Rank < p.Rank));
            }

            return false;
        }

    }

    public class TwoPairPocketPairPlusPairedBoardGreaterPairOnBoard : TwoPairPocketPairPlusPairedBoard
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);
                List<DataTypes.Card> pairOnBoard = boardCards.GroupBy(x => x.Rank).FirstOrDefault(x => x.Count() == 2)?.ToList();

                if (pairOnBoard?.Count != 0)
                    return playerCards.ElementAt(0).Rank > pairOnBoard.ElementAt(0).Rank;
            }

            return false;
        }

    }

    public class TwoPairPocketPairPlusPairedBoardLowerPairOnBoard : TwoPairPocketPairPlusPairedBoard
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);
                List<DataTypes.Card> pairOnBoard = boardCards.GroupBy(x => x.Rank).FirstOrDefault(x => x.Count() == 2)?.ToList();

                if (pairOnBoard?.Count != 0)
                    return playerCards.ElementAt(0).Rank < pairOnBoard.ElementAt(0).Rank;
            }

            return false;
        }

    }

    #endregion

    public class TwoPairOnBoardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if ( boardCards == null || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNoOfKind(allCards, 3) || HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
                return false;

            return !HandAnalyzerHelpers.HasPair(playerCards, 1)
                   && !playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank))
                   && HandAnalyzerHelpers.HasPair(boardCards, 2);
        }
    }


}
