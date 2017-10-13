using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    public class FourOfAKindPocketPairAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            if (!HandAnalyzerHelpers.HasPair(playerCards, 1))
            {
                return false;
            }

            return boardCards.GroupBy(x => x.Rank).Any(g => (g.Count() == 2) && (g.Key == playerCards.First().Rank));
        }
    }

    public class FourOfAKindNoPocketPairAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || !boardCards.Any())
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
            {
                return false;
            }

            var trips = boardCards.GroupBy(x => x.Rank).FirstOrDefault(x => x.Count() == 3);
            if (trips != null)
            {
                return playerCards.Any(x => x.Rank == trips.Key);
            }     

            return false;
        }            
    }


    public class FourOfAKindOnBoardAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {  
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (boardCards == null || boardCards.Count == 0)
            {
                return false;
            }

            return HandAnalyzerHelpers.IsNoOfKind(boardCards, 4);
        }
    }
}
