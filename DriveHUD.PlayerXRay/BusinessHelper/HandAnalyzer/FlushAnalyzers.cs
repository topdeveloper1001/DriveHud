using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    public class FlushTwoCardNutAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards)) 
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.All(x => flushBoardCards.Any(f => f.CardSuit == x.CardSuit)))
                {
                    var nutCardSuit = flushBoardCards.First().CardSuit;
                    var nutCardRank = DataTypes.Card.CardRankList.First(x => boardCards.Where(b => b.CardSuit == nutCardSuit).All(b => b.Rank != (int) x));
                    return playerCards.Any(x => x.Rank == (int) nutCardRank);
                }
            }

            return false;
        }

    }

    public class FlushTwoCardHighAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.All(x => flushBoardCards.Any(f => f.CardSuit == x.CardSuit)))
                {
                    return playerCards.Any(x => HandAnalyzerHelpers.IsDecentKicker(x));
                }
            }
            return false;
        }
    }

    public class FlushTwoCardLowAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.All(x => flushBoardCards.Any(f => f.CardSuit == x.CardSuit)))
                {
                    return playerCards.All(x => HandAnalyzerHelpers.IsWeakKicker(x));
                }
            }
            return false;
        }
    }

    public class FlushOneCardNutAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.Any(x => flushBoardCards.Any(f => f.CardSuit == x.CardSuit))
                    && playerCards.Any(x => !flushBoardCards.Any(f => f.CardSuit == x.CardSuit)))
                {
                    var nutCardSuit = flushBoardCards.First().CardSuit;
                    var nutCardRank = DataTypes.Card.CardRankList.First(rank => boardCards.Where(b => b.CardSuit == nutCardSuit).All(b => b.Rank != (int) rank));
                    return playerCards.Any(x => x.Rank == (int) nutCardRank && x.CardSuit == nutCardSuit);
                }
            }
            return false;
        }
    }

    public class FlushOneCardHighAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.First(g => g.Count() >= 5).OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.Any(x => flushBoardCards.Any(f => f.CardSuit == x.CardSuit) && HandAnalyzerHelpers.IsDecentKicker(x))
                    && playerCards.Any(x => flushBoardCards.All(f => f.CardSuit != x.CardSuit)))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class FlushOneCardLowAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.First(g => g.Count() >= 5).OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.Any(x => flushBoardCards.Any(f => f.CardSuit == x.CardSuit) && HandAnalyzerHelpers.IsWeakKicker(x))
                    && playerCards.Any(x => flushBoardCards.All(f => f.CardSuit != x.CardSuit)))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class FlushOnBoardNutAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);   

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.First(g => g.Count() >= 5).OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.All(x => flushBoardCards.All(f => f.CardSuit != x.CardSuit)))
                {
                    return flushBoardCards.Max(x => x.Rank) == (int)DataTypes.Card.GetCardRank("A");
                }
            }
            return false;
        }    
    }

    public class FlushOnBoardHighAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.First(g => g.Count() >= 5).OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.All(x => flushBoardCards.All(f => f.CardSuit != x.CardSuit)))
                {
                    return HandAnalyzerHelpers.IsDecentKicker(flushBoardCards.First());
                }
            }
            return false;
        }       
    }

    public class FlushOnBoardLowAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var suitGroup = allCards.GroupBy(x => x.CardSuit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.First(g => g.Count() >= 5).OrderByDescending(x => x.Rank).Take(5);
                if (playerCards.All(x => flushBoardCards.All(f => f.CardSuit != x.CardSuit)))
                {
                    return HandAnalyzerHelpers.IsWeakKicker(flushBoardCards.First());
                }
            }
            return false;
        } 
    }
}
