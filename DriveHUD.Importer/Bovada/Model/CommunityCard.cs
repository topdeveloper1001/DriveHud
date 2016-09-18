//-----------------------------------------------------------------------
// <copyright file="CommunityCard.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Community cards
    /// </summary>
    internal class CommunityCard
    {
        public int Rank { get; set; }
        public int Suit { get; set; }
        public string Card { get; set; }

        public CommunityCard()
        {
            this.Rank = -1;
            this.Suit = -1;
            this.Card = "";
        }

        private string GetCard(int rank, int suit)
        {
            string rankString = "";
            string suitString = "";
            switch (rank)
            {
                case 0:
                    rankString = "A";
                    break;
                case 1:
                    rankString = "2";
                    break;
                case 2:
                    rankString = "3";
                    break;
                case 3:
                    rankString = "4";
                    break;
                case 4:
                    rankString = "5";
                    break;
                case 5:
                    rankString = "6";
                    break;
                case 6:
                    rankString = "7";
                    break;
                case 7:
                    rankString = "8";
                    break;
                case 8:
                    rankString = "9";
                    break;
                case 9:
                    rankString = "T";
                    break;
                case 10:
                    rankString = "J";
                    break;
                case 11:
                    rankString = "Q";
                    break;
                case 12:
                    rankString = "K";
                    break;
                case 32896:
                    rankString = "";
                    break;
                default:
                    rankString = "Error ";
                    break;
            }

            switch (suit)
            {
                case 0:
                    suitString = "c";
                    break;
                case 1:
                    suitString = "d";
                    break;
                case 2:
                    suitString = "h";
                    break;
                case 3:
                    suitString = "s";
                    break;
                case 32896:
                    suitString = "";
                    break;
                default:
                    suitString = "Error";
                    break;
            }
            return rankString + suitString;
        }

        public override string ToString()
        {
            return string.Format("Card: {2}",
                Rank, Suit, Card);
        }
    }
}