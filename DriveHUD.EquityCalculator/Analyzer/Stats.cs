using System;
using System.Collections;
using System.Collections.Generic;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class Stats
    {
        internal String PlayerName;
        internal double VPIP, PFR, AF, _3Bet, Fold_3bet, NBHands, WTSD, Agg, WSSD, CBet;
        internal double OpenRaiseEP = 0, OpenRaiseMP = 0, OpenRaiseCO = 0, OpenRaiseBTN = 0, OpenRaiseSB = 0, OpenRaiseUTG = 0;
        internal List<double> OpenRaises = new List<double>();
        static Hashtable PlayersStats = new Hashtable();
        internal static void GetPlayersStats(HandHistory handHistory)
        {
        }
    }
}