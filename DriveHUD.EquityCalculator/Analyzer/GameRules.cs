using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class GameRules
    {
        internal enum format_types
        {
            FORMAT_BLIND = 1,
            FORMAT_DEAL = 2,
            FORMAT_FOLD = 3,
            FORMAT_CALL = 4,
            FORMAT_RAISE = 5,
            FORMAT_SHOWDOWN = 6
        };



        internal static float[,] drawing_outs = { // 1 card and 2 card odds
     {0.000f, 0.000f},  // 0 outs
     {0.021f, 0.043f},  // 1 out
     {0.043f, 0.084f},  // 2 outs
     {0.064f, 0.125f},  // 3 outs
     {0.085f, 0.165f},  // 4 outs
     {0.106f, 0.204f},  // 5 outs
     {0.128f, 0.241f},  // 6 outs
     {0.149f, 0.278f},  // 7 outs
     {0.170f, 0.315f},  // 8 outs
     {0.192f, 0.350f},  // 9 outs
     {0.213f, 0.384f},  // 10 outs
     {0.234f, 0.417f},  // 11 outs
     {0.255f, 0.450f},  // 12 outs
     {0.277f, 0.481f},  // 13 outs
     {0.298f, 0.512f},  // 14 outs 
     {0.319f, 0.541f},  // 15 outs 
     {0.340f, 0.570f},  // 16 outs
     {0.362f, 0.598f},  // 17 outs
     {0.383f, 0.624f},  // 18 outs
     {0.404f, 0.650f},  // 19 outs
	 {0.426f, 0.675f},  // 20 outs
	 {0.447f, 0.699f}}; // 21 outs


        internal static String[,] position_names = {
    {   "",    "",    "",    "",    "",    "",    "",    "",    "",    "",    ""}, // 0-handed table
	{   "",    "",    "",    "",    "",    "",    "",    "",    "",    "",    ""}, // 1-handed table
	{ "SB",  "BB",    "",    "",    "",    "",    "",    "",    "",    "",    ""}, // 2-handed table
	{"BTN",  "SB",  "BB",    "",    "",    "",    "",    "",    "",    "",    ""}, // 3-handed table
	{ "CO", "BTN",  "SB",  "BB",    "",    "",    "",    "",    "",    "",    ""}, // 4-handed table
	{ "HJ",  "CO", "BTN",  "SB",  "BB",    "",    "",    "",    "",    "",    ""}, // 5-handed table
	{"UTG",  "HJ",  "CO", "BTN",  "SB",  "BB",    "",    "",    "",    "",    ""}, // 6-handed table
	{"UTG",  "MP",  "HJ",  "CO", "BTN",  "SB",  "BB",    "",    "",    "",    ""}, // 7-handed table
	{"UTG",  "MP",  "MP",  "HJ",  "CO", "BTN",  "SB",  "BB",    "",    "",    ""}, // 8-handed table
	{"UTG",  "EP",  "MP",  "MP",  "HJ",  "CO", "BTN",  "SB",  "BB",    "",    ""}, // 9-handed table
	{"UTG",  "EP",  "EP",  "MP",  "MP",  "HJ",  "CO", "BTN",  "SB",  "BB",    ""}, // 10-handed table
	{"UTG",  "EP",  "EP",  "EP",  "MP",  "MP",  "HJ",  "CO", "BTN",  "SB",  "BB"} // 11-handed table (should not exists in online poker)
};



        internal enum stack_depths { SHORT_STACK, MEDIUM_STACK, DEEP_STACK };

        internal enum suit
        {
            CLUB = 0x8000,
            DIAMOND = 0x4000,
            HEART = 0x2000,
            SPADE = 0x1000
        }




    }
}
