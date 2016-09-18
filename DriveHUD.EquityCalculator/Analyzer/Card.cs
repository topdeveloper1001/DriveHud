using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class Card
    {
        internal static String[] AllCards = new String[] { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };
        internal static List<String> AllCardsList = new List<String>(AllCards);
        internal static Hashtable CardValues = new Hashtable();

        internal static char[] CardSuit = {
      'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h',
      'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd',
      'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c',
      's', 's', 's', 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' };

        internal static char[] CardName = {
      '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
      '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
      '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
      '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

        internal static String GetCardFromID(String cardID, String correctCard)
        {
            int nCardID = int.Parse(cardID);


            int typeID = 4 - (int)Math.Ceiling((decimal)nCardID / 13);
            int n = nCardID - (int)Math.Floor((double)nCardID / (double)13) * 13;
            if (nCardID == 26)
            {

            }

            int col = nCardID - (int)Math.Floor((decimal)nCardID / 13) * 13;
            if (n == 13)
            {
            }
            String type = typeID == 0 || typeID == 1 ? "s" : typeID == 2 ? "h" : typeID == 3 ? "c" : "d";
            type = typeID == 0 ? "s" : typeID == 1 ? "h" : typeID == 2 ? "c" : "d";

            if (n == 0) return "K" + type;
            if (n == 1) return "A" + type;
            return AllCards[n - 2] + type;
        }


        internal static void Init()
        {
            for (int i = 0; i < 13; i++) CardValues.Add(AllCards[i], i);
        }
    }
}
