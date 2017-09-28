using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcePokerSolutions.BusinessHelper.TextureHelpers;
using AcePokerSolutions.DataTypes;

namespace AcePokerSolutions.BusinessHelper.HandAnalyzer
{
    #region Two Card
    public class FlushDrawTwoCardNutAnalyzer 
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsTopKicker(playerCards.OrderByDescending(x => x.Rank).First(), boardCards.Where(b => b.CardSuit == playerCards.First().CardSuit));
        }    
    }

    public class FlushDrawTwoCardHighAnalyzer
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsDecentKicker(playerCards.OrderByDescending(x => x.Rank).First());
        }     
    }

    public class FlushDrawTwoCardLowAnalyzer 
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsWeakKicker(playerCards.OrderByDescending(x => x.Rank).First());
        }      
    }
    #endregion

    #region One Card
    public class FlushDrawOneCardNutAnalyzer
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1))
            {
                return false;
            }

            var drawSuit = boardCards.GroupBy(x => x.CardSuit).FirstOrDefault(x => x.Count() == 3)?.Key;

            return (drawSuit != null) && HandAnalyzerHelpers.IsTopKicker(playerCards.OrderByDescending(x => x.Rank).First(x => x.CardSuit == drawSuit), boardCards.Where(b => b.CardSuit == drawSuit));
        }      
    }

    public class FlushDrawOneCardHighAnalyzer
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1))
            {
                return false;
            }

            var drawSuit = boardCards.GroupBy(x => x.CardSuit).FirstOrDefault(x => x.Count() == 3)?.Key;
            return (drawSuit != null) && HandAnalyzerHelpers.IsDecentKicker(playerCards.OrderByDescending(x => x.Rank).First(x => x.CardSuit == drawSuit));
        }   
    }

    public class FlushDrawOneCardLowAnalyzer 
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1))
            {
                return false;
            }

            var drawSuit = boardCards.GroupBy(x => x.CardSuit).FirstOrDefault(x => x.Count() == 3)?.Key;
            return (drawSuit != null) && HandAnalyzerHelpers.IsWeakKicker(playerCards.OrderByDescending(x => x.Rank).First(x => x.CardSuit == drawSuit));
        }    
    }
    #endregion

    #region Backdoor
    public class FlushDrawBackdoorTwoCardNutAnalyzer 
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2, isBackdoor: true))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsTopKicker(playerCards.OrderByDescending(x => x.Rank).First(), boardCards.Where(b => b.CardSuit == playerCards.First().CardSuit));

        }       
    }

    public class FlushDrawBackdoorTwoCardAnalyzer 
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            return HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2, isBackdoor: true);
        }  
    }

    public class FlushDrawBackdoorOneCardNutAnalyzer 
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1, isBackdoor: true))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsTopKicker(playerCards.OrderByDescending(x => x.Rank).First(), boardCards.Where(b => b.CardSuit == playerCards.First().CardSuit));

        }       
    }

    #endregion

    public class FlushDrawNoFlushDrawAnalyzer
    {
        public bool Analyze(List<Card> playerCards, List<Card> allBoardCards, Street targetStreet)
        {
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            return !HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 0)
                && !HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1)
                && !HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2);
        }    
    }
}
