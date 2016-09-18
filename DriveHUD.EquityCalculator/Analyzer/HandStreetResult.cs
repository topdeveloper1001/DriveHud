using System;
using System.Collections.Generic;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class HandStreetResult
    {
        internal int Street;
        internal String PotEquityOpponentMsg;
        internal double PotEquityOpponent;
        internal String PotEquityAllMsg;
        internal double PotEquityAll;
        internal double CallingRangeTop;
        internal String CallingRangeMsg;
        internal double RaisingRangeTop;
        internal double RaisingRangeRaiseTo;
        internal String RaisingRangeMsg;
        internal double TopHand;
        internal String TopHandMsg;
        internal String Advice;
        internal String OutsMsg;
        internal String FinalAdvancedAdvice;
        internal List<String> AdvancedAdvices = new List<String>();
        internal double RaiseSize;
        internal double BetSize;
        internal String CorrectAction;
        internal String HeroAction;
        internal Action Action;
        internal float ev_raise;
        internal List<List<String>> Summary = new List<List<String>>();

        internal enum Advices
        {
            Check,
            CheckAsBluff,
            Raise,
            RaiseBluff,
            RaiseSemiBluff,
            Bet,
            BetAsBluff,
            Call,
            Fold,
            None
        }

        internal Advices GetAdviceType()
        {
            String correctAction = CorrectAction.ToLower();
            String sAdvice = Advice.ToLower();

            if (correctAction.Equals("check"))
            {
                if (Advice.ToLower().Contains("bluff"))
                    return Advices.CheckAsBluff;
                return Advices.CheckAsBluff;
            }

            if (correctAction.Equals("bet"))
            {
                if (Advice.ToLower().Contains("bluff"))
                    return Advices.BetAsBluff;
                return Advices.Bet;
            }

            if (correctAction.Equals("call"))
            {
                return Advices.Call;
            }

            if (correctAction.Equals("raise"))
            {
                if (Advice.ToLower().Contains("semi"))
                    return Advices.RaiseSemiBluff;
                else if (Advice.ToLower().Contains("bluff"))
                    return Advices.RaiseBluff;
                return Advices.Raise;
            }

            if (correctAction.Equals("fold"))
                return Advices.Fold;

            return Advices.None;
        }

        internal HandStreetResult()
        {
            Summary.Add(new List<String>()); //PREFLOP
            Summary.Add(new List<String>()); //FLOP
            Summary.Add(new List<String>()); //TURN
            Summary.Add(new List<String>()); //RIVER
        }
    }
}