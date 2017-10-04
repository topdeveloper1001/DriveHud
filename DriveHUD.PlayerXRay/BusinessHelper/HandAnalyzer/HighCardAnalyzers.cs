﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    public class HighCardAnalyzer
    {
        public virtual bool Analyze(List<Card> playerCards, List<Card> allboardCards, Street targetStreet)
        {
            //retain only that card that we need for flop, turn or river calculations
            List<Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allboardCards, targetStreet);
            
            if (playerCards == null || boardCards == null || playerCards.Count != 2 || boardCards.Count < 3)
            {
                return false;
            }

            List<Card> allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (!HandAnalyzerHelpers.HasPair(allCards, 1) && !HandAnalyzerHelpers.HasPair(allCards, 2) && 
                !HandAnalyzerHelpers.IsNoOfKind(allCards, 3) && !HandAnalyzerHelpers.IsNoOfKind(allCards, 4))
            {
                if (!allCards.GroupBy(x => x.CardSuit).Any(x => x.Count() >= 5))
                {
                    return true;
                }
            }

            return false;
        }

        public class HighCardNoOvercardAnalyzer : HighCardAnalyzer
        {
            public override bool Analyze(List<Card> playerCards, List<Card> boardCards, Street targetStreet)
            { 
                if (base.Analyze(playerCards, boardCards, targetStreet))
                {
                    return HandAnalyzerHelpers.IsNoOvercards(playerCards, boardCards);
                }

                return false;
            }
        }

        public class HighCardOneOvercardAnalyzer : HighCardAnalyzer
        {
            public override bool Analyze(List<Card> playerCards, List<Card> boardCards, Street targetStreet)
            {
                if (base.Analyze(playerCards, boardCards, targetStreet))
                {
                    return HandAnalyzerHelpers.IsOneOvercard(playerCards, boardCards);
                }

                return false;
            }   
        }

        public class HighCardTwoOvercardAnalyzer : HighCardAnalyzer
        {
            public override bool Analyze(List<Card> playerCards, List<Card> boardCards, Street targetStreet)
            {
                if (base.Analyze(playerCards, boardCards, targetStreet))
                {
                    return HandAnalyzerHelpers.IsTwoOvercards(playerCards, boardCards);
                }

                return false;
            }
        }
    }
}
