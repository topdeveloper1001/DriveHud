using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class Action
    {
        internal Action() { }
        internal Action(String playerName, String sAction, int amount)
        {
            this.PlayerName = playerName;
            this.SAction = sAction;
            this.Amount = amount;
        }
        internal int Street; // 0=preflop, 1=flop, 2=turn, 3=river, (4=showdown/end-of-hand, only used to store the final pot size)

        internal String PlayerName;
        internal String SAction;
        internal int Amount;

        internal Hashtable InHand = new Hashtable(); // Hashtable indicating which players are still in the hand BEFORE this action
        internal Hashtable LastStreetCommitment = new Hashtable(); // Hashtable of how much each player had committed to the pot at the end of previous street (does not get updated until the end of each street)
        internal Hashtable ThisStreetCommitment = new Hashtable(); // Hashtable of how much each player has committed to the pot in the current street only, BEFORE this action
        internal Hashtable LastStreetCall = new Hashtable();		 // Hashtable of if each player called as their last action on the precious street, and how much they called (if called at all) - this is used for the next bet-range calculation

        // Game Theory -information (BEFORE this action)
        internal String Attacker;	// Last raiser (initially the BB) -> Will turn into defender when someone else (re)raises
        internal String Defender;	// The defender of the pot, who closes the action for the street if he calls
        internal int AttackerRisk;	// How much did the attacker risk of his stack (needed for calculating the optimal folding range)


    }

    struct PreAction
    {
        public int Pot;
        public int To_call;
        public int TotalAnte;
        public int Eff_stacks;
        public int Players_ip;
        public int Players_oop;
    }

    struct RaiseAdvice
    {
        public int RaiseSize;
        public float OptimalRaiseRange;
        public float ActualRaiseRange;
        public float CallRange;
        public float GametheoryCall; // This is not an absolute range, but relative to existing range
        public GameRules.stack_depths StackDepth;
        public bool PocketPairOdds;
        public bool SuitedConnectorOdds;
        public bool SuitedGapperOdds;

        public String Debug;
    };


    class hand_distribution
    {
        public float hand_range;          // Range of made hands: 0-x, where zero is the nuts (top 0% hand)
                                          //float draw_range;          // Range of drawing hands: x-1, where 1 means 100% pot equity (post-flop only)
        public float[,] draw_matrix = new float[52, 52]; // Matrix of hands the player might still be drawing to - considering all betting rounds (post-flop only)
    };


    struct pot_equity
    {
        public float random_equity_all; // Pot equity against random opponents'
        public float deep_equity_all;   // Pot equity against given opponent percentiles (using deep stack jam-rankings)
        public float short_equity_all;  // Pot equity against given opponent percentiles (using short stack rankings)
        public float postflop_equity_all; // Pot equity against given opponent percentiles (for post-flop)
        public float suckout_equity_all;  // How often do we suck out to win the hand, against given opponent percentiles (for post-flop)

        public String strongest_man;
        public float random_equity_hup; // Pot equity against one random hand
        public float deep_equity_hup;   // Pot equity against the opponent with strongest percentile (using deep stack jam-rankings)
        public float short_equity_hup;  // Pot equity against the opponent with strongest percentile (using short stack rankings)
        public float postflop_equity_hup; // Pot equity against the opponent with strongest percentile (for post-flop)

        public bool MadeHands;
    };


    class suckout
    {
        public float potOdds;
        public float outs;
        public float[] prob = new float[2]; // Probability to hit w/ 1 and 2 cards to come
    };

    struct postflop_advice
    {
        public int raise_size;
        public bool OpponentCheckRaised;
        public int bet_size;
        public bool callAllIn;
        public bool moveAllIn;
        public float optimal_raise_range;
        public float optimal_bluff_range;
        public float actual_raise_range;
        public float actual_bluff_range;
        public float call_range;
        public float gametheory_call; // This is not an absolute range, but relative to existing range
        public String custom_advice;
        public String debug;
    };


    internal enum postflophand
    {
        kStraightFlush,
        k4ofKind,
        kFullHouse,
        kFlush,
        kStraight,
        k3ofKind,
        k2Pair,
        kPair,
        kNoPair
    };

}
