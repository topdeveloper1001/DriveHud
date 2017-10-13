using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{

    #region Pocket Pair
    public class PocketPairOverpairAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            List<DataTypes.Card> consideredCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);


            if (HandAnalyzerHelpers.HasPair(playerCards, 1) && HandAnalyzerHelpers.HasPair(consideredCards, 1))
            {
                if (boardCards.All(x => x.Rank < playerCards.First().Rank))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class PocketPairSecondPairAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            List<DataTypes.Card> consideredCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) && HandAnalyzerHelpers.HasPair(consideredCards, 1))
            {
                if (boardCards.Count(x => x.Rank > playerCards.First().Rank) == 1)
                {
                    return true;
                }
            }
            return false;
        }

    }


    public class PocketPairLowPairAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            List<DataTypes.Card> consideredCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1)
                && HandAnalyzerHelpers.HasPair(consideredCards, 1)
                && !new PocketPairOverpairAnalyzer().Analyze(playerCards, consideredCards, targetStreet)
                && !new PocketPairSecondPairAnalyzer().Analyze(playerCards, consideredCards, targetStreet))
                return true;

            return false;

        }
    }
    #endregion

    #region Top Pair

    public class TopPairAnalyzer
    {
        public virtual bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            List<DataTypes.Card> consideredCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || !HandAnalyzerHelpers.HasPair(consideredCards, 1))
            {
                return false;
            }

            if (playerCards.Any(c => c.Rank == boardCards.Max(x => x.Rank)))
            {
                return true;
            }

            return false;
        }
    }

    public class TopPairTopKickerAnalyzer : TopPairAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> boardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsTopKicker(unpairedCard, boardCards);
                }
            }

            return false;
        }
    }

    public class TopPairGoodKickerAnalyzer : TopPairAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> boardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(unpairedCard);
                }
            }

            return false;
        }

    }

    public class TopPairWeakKickerAnalyzer : TopPairAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> boardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(unpairedCard);
                }
            }

            return false;
        }

    }
    #endregion

    #region Second Pair

    public class SecondPairAnalyzer
    {
        public virtual bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            List<DataTypes.Card> consideredCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || !HandAnalyzerHelpers.HasPair(consideredCards, 1))
            {
                return false;
            }

            var secondCardRank = boardCards.OrderByDescending(x => x.Rank).ElementAt(1).Rank;
            if (playerCards.Any(c => c.Rank == secondCardRank))
            {
                return true;
            }

            return false;
        }
    }

    public class SecondPairAceKickerAnalyzer : SecondPairAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> boardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null && unpairedCard.CardValue == CardRank.Ace)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class SecondPairNonAceKickerAnalyzer : SecondPairAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> boardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, boardCards, targetStreet))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null && unpairedCard.CardValue != CardRank.Ace)
                {
                    return true;
                }
            }

            return false;
        }
    }

    #endregion

    #region Bottom Pair

    public class BottomPairAnalyzer
    {
        public virtual bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            List<DataTypes.Card> consideredCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.HasPair(playerCards, 1) || !HandAnalyzerHelpers.HasPair(consideredCards, 1))
            {
                return false;
            }

            if (playerCards.Any(c => c.Rank == boardCards.Min(x => x.Rank)))
            {
                return true;
            }

            return false;
        }

        public class BottomPairAceKickerAnalyzer : BottomPairAnalyzer
        {
            public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> boardCards, Street targetStreet)
            {
                if (base.Analyze(playerCards, boardCards, targetStreet))
                {
                    var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                    if (unpairedCard != null && unpairedCard.CardValue == CardRank.Ace)
                    {
                        return true;
                    }
                }   
                return false;
            }
        }

        public class BottomPairNonAceKickerAnalyzer : BottomPairAnalyzer
        {
            public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> boardCards, Street targetStreet)
            {
                if (base.Analyze(playerCards, boardCards, targetStreet))
                {
                    var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                    if (unpairedCard != null && unpairedCard.CardValue != CardRank.Ace)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

    }

    #endregion

    #region Paired Board

    public class PairedBoardAnalyzer
    {
        public virtual bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;
            
            return !HandAnalyzerHelpers.HasPair(playerCards, 1)
                && !playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank))
                && HandAnalyzerHelpers.HasPair(boardCards, 1); ;
        }
    }

    public class PairedBoardNoOvercardsAnalyzer : PairedBoardAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                return HandAnalyzerHelpers.IsNoOvercards(playerCards, allBoardCards);
            }

            return false;
        }    
    }

    public class PairedBoardOneOvercardAnalyzer : PairedBoardAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                return HandAnalyzerHelpers.IsOneOvercard(playerCards, allBoardCards);
            }

            return false;
        }
    }

    public class PairedBoardTwoOvercardsAnalyzer : PairedBoardAnalyzer
    {
        public override bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            if (base.Analyze(playerCards, allBoardCards, targetStreet))
            {
                return HandAnalyzerHelpers.IsTwoOvercards(playerCards, allBoardCards);
            }

            return false;
        }
    }



    #endregion

}
