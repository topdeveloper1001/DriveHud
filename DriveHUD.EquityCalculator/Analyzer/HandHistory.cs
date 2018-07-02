using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class HandHistory
    {
        internal String PokerHandID;
        internal String SHandHistory;
        internal String FileName;
        internal String HandDate;
        internal String SiteName;
        internal String PokerGame;
        internal String PokerGameType;
        internal String TableName;
        internal String GameNumber;
        internal int MaxSeats;
        internal int ButtonSeat;
        internal double SmallBlindAmount;
        internal double BigBlindAmount;
        internal double Ante;
        internal double MissedBets;

        internal List<HandStreetResult> handResults = new List<HandStreetResult>();
        internal abs_value[] abs_rank = new abs_value[1326];
        internal struct abs_value { internal int card1, card2, value; }; // Used for absolute post-flop hand rankings and percentiles

        internal Hashtable Players = new Hashtable();

        internal bool Is6Max;
        internal bool IsFR;

        internal static Hashtable jam_rank = new Hashtable();
        internal static Hashtable deepstack_rank = new Hashtable();
        internal static Hashtable shortstack_rank = new Hashtable();

        internal Hashtable absolute_percentile = new Hashtable(); //map<CString, float>// Absolute post-flop hand ranking percentile (ignoring draws)


        internal static Hashtable jam_percentile = new Hashtable();
        internal static Hashtable deepstack_percentile = new Hashtable();
        internal static Hashtable shortstack_percentile = new Hashtable();

        internal List<Action> PreflopActions = new List<Action>();
        internal List<Action>[] PostflopActions = new List<Action>[] { new List<Action>(), new List<Action>(), new List<Action>(), new List<Action>() };

        internal String HeroName;
        internal String SBName;
        internal String BBName;
        internal bool HeroIsOnFlop;

        internal String[] CommunityCards = new String[4]; // Flop-River: 1-3
        internal int[] PotSizeByStreet = new int[5]; // Pot size in the beginning of each street, and the final pot size at the end of the hand. (only used for printing it on the screen)


        static internal Hashtable trouble_hands = new Hashtable();
        static internal Hashtable semibluff_hands = new Hashtable();

        #region Arr
        static internal String[] trouble_handsArr = new String[]{
    "QQ",
    "JJ",
    "TT",
    "99",
    "88",
    "AQo",
    "AJo",
    "ATo",
    "A9o",
    "A8o",
    "A7o",
    "A6o",
    "A5o",
    "A4o",
    "A3o",
    "A2o",
    "KJo",
    "KTo",
    "K9o",
    "K8o",
    "K7o",
    "K6o",
    "K5o",
    "K4o",
    "K3o",
    "K2o",
    "Q9o",
    "Q8o",
    "Q7o",
    "Q6o",
    "Q5o",
    "Q4o",
    "Q3o",
    "Q2o",
    "J8o",
    "J7o",
    "J6o",
    "J5o",
    "J4o",
    "J3o",
    "J2o"};

        static String[] semibluff_handsArr = new String[] {
    "ATs",
    "A5s",
    "A4s",
    "A3s",
    "KJs",
    "KTs",
    "QJs",
    "QTs",
    "Q9s",
    "Q8s",
    "JTs",
    "J9s",
    "J8s",
    "J7s",
    "T9s",
    "T8s",
    "T7s",
    "T6s",
    "98s",
    "97s",
    "96s",
    "87s",
    "86s",
    "85s",
    "76s",
    "75s",
    "74s",
    "65s",
    "64s",
    "54s"};




        static String[] jam = new String[]{
    "AA",
    "AKs",
    "A5s",
    "AKo",
    "ATs",
    "KK",
    "A4s",
    "QQ",
    "AQs",
    "TT",
    "JJ",
    "AJs",
    "A3s",
    "KQs",
    "AQo",
    "66",
    "KJs",
    "QJs",
    "KTs",
    "99",
    "AJo",
    "88",
    "QTs",
    "JTs",
    "77",
    "A9s",
    "55",
    "A7s",
    "KQo",
    "KJo",
    "K9s",
    "44",
    "A8s",
    "J9s",
    "T9s",
    "Q9s",
    "33",
    "ATo",
    "22",
    "K8s",
    "QJo",
    "98s",
    "T8s",
    "A6s",
    "Q8s",
    "KTo",
    "J8s",
    "K7s",
    "A2s",
    "JTo",
    "A9o",
    "QTo",
    "A8o",
    "87s",
    "A7o",
    "A5o",
    "K6s",
    "T7s",
    "97s",
    "76s",
    "A6o",
    "A4o",
    "K5s",
    "A3o",
    "J7s",
    "T9o",
    "86s",
    "Q7s",
    "Q6s",
    "A2o",
    "J9o",
    "65s",
    "96s",
    "K4s",
    "T6s",
    "K9o",
    "Q5s",
    "Q9o",
    "75s",
    "54s",
    "98o",
    "K3s",
    "K2s",
    "K8o",
    "J6s",
    "85s",
    "T8o",
    "Q4s",
    "J5s",
    "K7o",
    "64s",
    "87o",
    "K6o",
    "Q3s",
    "J4s",
    "K5o",
    "95s",
    "74s",
    "J8o",
    "Q2s",
    "Q8o",
    "K4o",
    "K3o",
    "T5s",
    "K2o",
    "J3s",
    "T4s",
    "97o",
    "76o",
    "Q7o",
    "84s",
    "T7o",
    "Q6o",
    "Q5o",
    "J2s",
    "J7o",
    "Q4o",
    "T3s",
    "Q3o",
    "86o",
    "Q2o",
    "94s",
    "T2s",
    "J6o",
    "J5o",
    "T6o",
    "J4o",
    "96o",
    "J3o",
    "93s",
    "J2o",
    "T5o",
    "T4o",
    "92s",
    "95o",
    "T3o",
    "85o",
    "T2o",
    "83s",
    "94o",
    "75o",
    "73s",
    "82s",
    "93o",
    "53s",
    "65o",
    "63s",
    "84o",
    "43s",
    "92o",
    "72s",
    "54o",
    "74o",
    "52s",
    "62s",
    "64o",
    "83o",
    "42s",
    "53o",
    "73o",
    "82o",
    "32s",
    "63o",
    "43o",
    "72o",
    "52o",
    "62o",
    "42o",
    "32o"};

        static String[] deepstack = new String[]{
    "AA",
    "AKs",
    "KK",
    "QQ",
    "AQs",
    "AKo",
    "JJ",
    "TT",
    "AJs",
    "99",
    "ATs",
    "88",
    "KQs",
    "AQo",
    "77",
    "A9s",
    "KJs",
    "A8s",
    "66",
    "A7s",
    "AJo",
    "KTs",
    "55",
    "A6s",
    "A5s",
    "QJs",
    "ATo",
    "A4s",
    "A3s",
    "KQo",
    "QTs",
    "K9s",
    "44",
    "A2s",
    "A9o",
    "A8o",
    "JTs",
    "K8s",
    "Q9s",
    "33",
    "A7o",
    "K7s",
    "KJo",
    "J9s",
    "K6s",
    "Q8s",
    "K5s",
    "KTo",
    "T9s",
    "K4s",
    "A6o",
    "A5o",
    "J8s",
    "Q7s",
    "K3s",
    "K2s",
    "22",
    "QJo",
    "A4o",
    "Q6s",
    "K9o",
    "T8s",
    "A3o",
    "Q5s",
    "J7s",
    "Q4s",
    "QTo",
    "A2o",
    "98s",
    "Q3s",
    "T7s",
    "K8o",
    "Q2s",
    "J6s",
    "97s",
    "J5s",
    "JTo",
    "K7o",
    "87s",
    "Q9o",
    "T6s",
    "J4s",
    "K6o",
    "96s",
    "J3s",
    "86s",
    "J2s",
    "76s",
    "K5o",
    "J9o",
    "T5s",
    "T4s",
    "Q8o",
    "95s",
    "65s",
    "K4o",
    "75s",
    "85s",
    "T3s",
    "K3o",
    "54s",
    "T2s",
    "T9o",
    "K2o",
    "64s",
    "94s",
    "Q7o",
    "74s",
    "84s",
    "93s",
    "J8o",
    "53s",
    "Q6o",
    "92s",
    "63s",
    "43s",
    "83s",
    "73s",
    "52s",
    "Q5o",
    "T8o",
    "82s",
    "42s",
    "J7o",
    "Q4o",
    "72s",
    "62s",
    "32s",
    "98o",
    "Q3o",
    "Q2o",
    "T7o",
    "J6o",
    "J5o",
    "97o",
    "J4o",
    "87o",
    "T6o",
    "J3o",
    "96o",
    "J2o",
    "86o",
    "76o",
    "T5o",
    "T4o",
    "95o",
    "65o",
    "T3o",
    "85o",
    "75o",
    "T2o",
    "54o",
    "94o",
    "64o",
    "84o",
    "74o",
    "93o",
    "53o",
    "92o",
    "63o",
    "43o",
    "83o",
    "73o",
    "82o",
    "52o",
    "62o",
    "42o",
    "72o",
    "32o"};

        static String[] shortstack = new String[] {
    "AA",
    "KK",
    "AKs",
    "QQ",
    "AKo",
    "JJ",
    "TT",
    "AQs",
    "99",
    "AQo",
    "88",
    "AJs",
    "77",
    "AJo",
    "ATs",
    "KQs",
    "66",
    "ATo",
    "A9s",
    "KQo",
    "KJs",
    "55",
    "A8s",
    "A9o",
    "A7s",
    "A8o",
    "KTs",
    "44",
    "A6s",
    "A5s",
    "QJs",
    "A7o",
    "KJo",
    "A4s",
    "A3s",
    "QTs",
    "K9s",
    "KTo",
    "A2s",
    "33",
    "A6o",
    "A5o",
    "QJo",
    "A4o",
    "JTs",
    "K8s",
    "K9o",
    "A3o",
    "Q9s",
    "QTo",
    "A2o",
    "K7s",
    "22",
    "J9s",
    "K6s",
    "K8o",
    "Q8s",
    "K5s",
    "JTo",
    "K7o",
    "T9s",
    "K4s",
    "Q9o",
    "J8s",
    "Q7s",
    "K3s",
    "K2s",
    "K6o",
    "Q6s",
    "K5o",
    "J9o",
    "Q8o",
    "T8s",
    "K4o",
    "Q5s",
    "J7s",
    "K3o",
    "Q4s",
    "T9o",
    "98s",
    "K2o",
    "Q7o",
    "Q3s",
    "J8o",
    "T7s",
    "Q6o",
    "Q2s",
    "J6s",
    "97s",
    "J5s",
    "Q5o",
    "T8o",
    "87s",
    "J7o",
    "T6s",
    "Q4o",
    "J4s",
    "98o",
    "Q3o",
    "96s",
    "J3s",
    "86s",
    "J2s",
    "Q2o",
    "T7o",
    "76s",
    "J6o",
    "T5s",
    "T4s",
    "J5o",
    "95s",
    "97o",
    "65s",
    "75s",
    "85s",
    "T3s",
    "J4o",
    "87o",
    "T6o",
    "54s",
    "T2s",
    "J3o",
    "64s",
    "94s",
    "96o",
    "J2o",
    "74s",
    "84s",
    "93s",
    "86o",
    "53s",
    "76o",
    "T5o",
    "92s",
    "63s",
    "43s",
    "T4o",
    "95o",
    "65o",
    "83s",
    "73s",
    "52s",
    "T3o",
    "85o",
    "75o",
    "82s",
    "T2o",
    "54o",
    "42s",
    "72s",
    "62s",
    "32s",
    "94o",
    "64o",
    "84o",
    "74o",
    "93o",
    "53o",
    "92o",
    "63o",
    "43o",
    "83o",
    "73o",
    "82o",
    "52o",
    "62o",
    "42o",
    "72o",
    "32o"};
        #endregion

        internal static void Init()
        {
            float jam_cumulative = 0, deepstack_cumulative = 0, shortstack_cumulative = 0;
            for (int i = 0; i < 169; i++)
            {
                deepstack_rank.Add(deepstack[i], i);
                shortstack_rank.Add(shortstack[i], i);

                jam_percentile.Add(jam[i], jam_cumulative);
                deepstack_percentile.Add(deepstack[i], deepstack_cumulative);
                shortstack_percentile.Add(shortstack[i], shortstack_cumulative);

                if (jam[i][0] == jam[i][1]) jam_cumulative += 6 / 1326.0f; // Pocket Pair
                else if (jam[i][2] == 's') jam_cumulative += 4 / 1326.0f; // Suited
                else jam_cumulative += 12 / 1326.0f; // Offsuit

                if (deepstack[i][0] == deepstack[i][1]) deepstack_cumulative += 6 / 1326.0f; // Pocket Pair
                else if (deepstack[i][2] == 's') deepstack_cumulative += 4 / 1326.0f; // Suited
                else deepstack_cumulative += 12 / 1326.0f; // Offsuit

                if (shortstack[i][0] == shortstack[i][1]) shortstack_cumulative += 6 / 1326.0f; // Pocket Pair
                else if (shortstack[i][2] == 's') shortstack_cumulative += 4 / 1326.0f; // Suited
                else shortstack_cumulative += 12 / 1326.0f; // Offsuit

            }



            for (int i = 0; i < trouble_handsArr.Length; i++) trouble_hands[trouble_handsArr[i]] = true;
            for (int i = 0; i < semibluff_handsArr.Length; i++) semibluff_hands[semibluff_handsArr[i]] = true;

        }

        internal HandHistory()
        {
        }

        internal String preflop_group(String hole)
        {
            String hand;
            if (hole[0] == hole[2]) return hole[0].ToString() + hole[2].ToString(); // Pocket Pair
            else if ((int)Card.CardValues[hole[0].ToString()] > (int)Card.CardValues[hole[2].ToString()]) hand = hole[0].ToString() + hole[2].ToString();
            else hand = hole[2].ToString() + hole[0].ToString();
            if (hole[1] == hole[3]) return hand + "s"; // Suited
            else return hand + "o"; // Offsuit
        }

        internal static int fastCard(char rank, char suit)
        {
            int rank_value, suit_value;

            switch (rank)
            {
                case 'A': rank_value = 12; break;
                case 'K': rank_value = 11; break;
                case 'Q': rank_value = 10; break;
                case 'J': rank_value = 9; break;
                case 'T': rank_value = 8; break;
                case '9': rank_value = 7; break;
                case '8': rank_value = 6; break;
                case '7': rank_value = 5; break;
                case '6': rank_value = 4; break;
                case '5': rank_value = 3; break;
                case '4': rank_value = 2; break;
                case '3': rank_value = 1; break;
                case '2': rank_value = 0; break;
                default: return -1;
            }

            switch (suit)
            {
                case 'h': suit_value = 0x2000; break;
                case 'd': suit_value = 0x4000; break;
                case 'c': suit_value = 0x8000; break;
                case 's': suit_value = 0x1000; break;
                default: return -1;
            }

            var c1 = FastEvaluator.primes[rank_value];
            var c2 = (rank_value << 8);
            var c3 = suit_value;
            var c4 = (1 << (16 + rank_value));
     
            return c1 | c2 | c3 | c4;
        }

        internal void update_absolute_percentiles(int street)
        {
            // Update absolute hand value table based on the new board
            int[] hand = new int[7];

            if (street >= 1)
            {
                if (CommunityCards[1] != null)
                {
                    hand[2] = fastCard(CommunityCards[1][0], CommunityCards[1][1]);
                    hand[3] = fastCard(CommunityCards[1][2], CommunityCards[1][3]);
                    hand[4] = fastCard(CommunityCards[1][4], CommunityCards[1][5]);
                }
            }

            if (street >= 2)
            {
                if (CommunityCards[2] != null)
                    hand[5] = fastCard(CommunityCards[2][0], CommunityCards[2][1]);
            }

            if (street >= 3)
            {
                if (CommunityCards[3] != null)
                    hand[6] = fastCard(CommunityCards[3][0], CommunityCards[3][1]);
            }

            // Enumrate all possible hole cards and find the absolute hand rankings (ignoring draws)
            int valid_combos = 0;

            for (int i = 1, k = 0; i < 52; i++)
            {
                hand[0] = fastCard(Card.CardName[i], Card.CardSuit[i]); // Hole 1

                for (int j = 0; j < i; j++, k++)
                {
                    hand[1] = fastCard(Card.CardName[j], Card.CardSuit[j]); // Hole 2

                    abs_rank[k].card1 = i;
                    abs_rank[k].card2 = j;
                    abs_rank[k].value = 9999; // Use 9999 as a default value, that will be left for hands that are not possible on the given board

                    // Check if these hole cards are possible on the given board
                    if (hand[0] == hand[2] || hand[0] == hand[3] || hand[0] == hand[4] ||
                        hand[1] == hand[2] || hand[1] == hand[3] || hand[1] == hand[4])
                    {
                        continue; // Not a possible holding on this board (flop)
                    }

                    if (street == 1) // Flop
                    {
                        abs_rank[k].value = FastEvaluator.eval_5hand(hand);
                    }
                    else if (street == 2) // Turn
                    {
                        if (hand[0] == hand[5] || hand[1] == hand[5]) continue; // Not a possible holding on this board (turn)
                        abs_rank[k].value = FastEvaluator.eval_6hand(hand);
                    }
                    else if (street == 3) // River
                    {
                        if (hand[0] == hand[5] || hand[1] == hand[5] || hand[0] == hand[6] || hand[1] == hand[6]) continue; // Not a possible holding on this board (turn+river)
                        abs_rank[k].value = FastEvaluator.eval_7hand(hand);
                    }
                    else
                    {
                        return; // Invalid street
                    }

                    valid_combos++;
                }
            }

            // Sort the hands by the absolute hand rankings (ignoring draws)
            //sort(abs_rank, abs_rank + 1326); //ATT!
            for (int i = 0; i < abs_rank.Length - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < abs_rank.Length; j++)
                {
                    if (abs_rank[j].value < abs_rank[minIndex].value) minIndex = j;
                }

                abs_value tmp = abs_rank[i];
                abs_rank[i] = abs_rank[minIndex];
                abs_rank[minIndex] = tmp;
            }
          
            int combo = 0, last_valid = -1;

            float last_percentile = 0;

            for (int i = 0; i < 1326; i++) // 52*51/2 = 1326
            {
                String hand_str1 = Card.CardName[abs_rank[i].card1].ToString() + Card.CardSuit[abs_rank[i].card1].ToString() + Card.CardName[abs_rank[i].card2].ToString() + Card.CardSuit[abs_rank[i].card2].ToString();
                String hand_str2 = Card.CardName[abs_rank[i].card2].ToString() + Card.CardSuit[abs_rank[i].card2].ToString() + Card.CardName[abs_rank[i].card1].ToString() + Card.CardSuit[abs_rank[i].card1].ToString();

                // Default percentile to 100% (nut-low) -> left only for hands that are not possible on the given board
                if (!absolute_percentile.ContainsKey(hand_str1))
                {
                    absolute_percentile.Add(hand_str1, 1.0f);//?
                }
                else
                {
                    absolute_percentile[hand_str1] = 1.0f;
                }

                if (!absolute_percentile.ContainsKey(hand_str2))
                {
                    absolute_percentile.Add(hand_str2, 1.0f);
                }
                else
                {
                    absolute_percentile[hand_str2] = 1.0f;
                }

                if (abs_rank[i].value < 9999)
                {
                    absolute_percentile[hand_str1] = absolute_percentile[hand_str2] = combo / (float)valid_combos;

                    // Make sure that all hands w/ same hands also have the same percentile
                    if (last_valid >= 0 && (abs_rank[i].value == abs_rank[last_valid].value))
                    {
                        absolute_percentile[hand_str1] = absolute_percentile[hand_str2] = last_percentile;
                    }
                    else
                    {
                        last_valid = i;
                        last_percentile = (float)Convert.ToDouble(absolute_percentile[hand_str1]);
                    }

                    combo++;
                }
            }
        }

        /// <summary>
        /// Convert from DriveHUD style <see cref="HandHistories.Objects.Hand.HandHistory"/> to 
        /// EquityCalculator style <see cref="HandHistory"/> format.
        /// </summary>
        /// <param name="history">DriveHUD style history</param>
        /// <returns></returns>
        internal void ConverToEquityCalculatorFormat(HandHistories.Objects.Hand.HandHistory history, HandHistories.Objects.Cards.Street currentStreet)
        {
            //try
            //{
            BBName = null;

            //  XmlElement xeHandDetails = (XmlElement)xd.GetElementsByTagName("HandDetails")[0];
            // Read general hand information
            this.HandDate = history.DateOfHandUtc.ToString();
            this.SiteName = history.GameDescription.Site.ToString();
            this.PokerGame = history.GameDescription.GameType.ToString();
            this.PokerGameType = history.GameDescription.PokerFormat.ToString();
            this.TableName = history.TableName;
            this.MaxSeats = history.GameDescription.SeatType.MaxPlayers;

            this.ButtonSeat = history.DealerButtonPosition;

            this.SmallBlindAmount = (int)Math.Round(history.HandActions.First(x => x.HandActionType == HandHistories.Objects.Actions.HandActionType.SMALL_BLIND).Amount * 100);
            if (this.SmallBlindAmount < 0)
                this.SmallBlindAmount *= -1;
            this.BigBlindAmount = (int)Math.Round(history.HandActions.First(x => x.HandActionType == HandHistories.Objects.Actions.HandActionType.BIG_BLIND).Amount * 100);
            if (this.BigBlindAmount < 0)
                this.BigBlindAmount *= -1;
            this.Ante = 0;
            if (history.HandActions.Any(x => x.HandActionType == HandHistories.Objects.Actions.HandActionType.ANTE))
            {
                this.Ante = (int)Math.Round(history.HandActions.First(x => x.HandActionType == HandHistories.Objects.Actions.HandActionType.ANTE).Amount * 100);
                if (this.Ante < 0)
                    this.Ante *= -1;
            }

            // Track players still in the pot, and how much they have committed to the pot
            Hashtable in_hand = new Hashtable();
            Hashtable acted_preflop = new Hashtable();  // This is needed because sometimes there can be players on the player list that are not dealt in!
            Hashtable last_commit = new Hashtable();// How much each player had put in the pot at the end of previous steet (doesn't get updated until the end of each street)
            Hashtable this_commit = new Hashtable();    // How much each player has put in the pot in the current street only
            Hashtable last_call = new Hashtable();  // Previous street call-amounts (to be used for game theory optimal betting)
            Hashtable this_call = new Hashtable();  // Current street call-amounts (to update the last_call hashtable at the end of the street)
                                                    // Track the attacker/defender information for game theory optimal folding
            String attacker = null;
            String defender = null;
            int attacker_risk = 0;
            // Track the street
            int street = -1;
            // Track the size of the pot
            int pot = 0;


            //hero?

            if (history.Hero != null)
            {
                HeroName = history.Hero.PlayerName;
            }
            else
            {
                HeroName = history.Players.First(x => x.hasHoleCards).PlayerName;
            }

            //player is in preflop?
            foreach (var preflopAction in history.PreFlop)
            {
                if (!Players.Contains(preflopAction.PlayerName))
                {
                    HandHistories.Objects.Players.Player p = history.Players.First(x => preflopAction.PlayerName == x.PlayerName);
                    Players.Add(p.PlayerName, new Player());
                    (Players[p.PlayerName] as Player).SeatNumber = p.SeatNumber;
                    (Players[p.PlayerName] as Player).StartingStack = (int)Math.Round(decimal.ToDouble(p.StartingStack) * 100);
                    (Players[p.PlayerName] as Player).Showdown = false;
                    (Players[p.PlayerName] as Player).Wins = 0;
                    (Players[p.PlayerName] as Player).PlayerName = p.PlayerName;
                    if (p.hasHoleCards)
                    {
                        (Players[p.PlayerName] as Player).Cards = p.HoleCards.ToString();
                    }
                    in_hand[p.PlayerName] = true;
                    last_commit[p.PlayerName] = 0;
                    this_commit[p.PlayerName] = 0;
                    last_call[p.PlayerName] = 0;
                    this_call[p.PlayerName] = 0;
                    pot += (int)Ante;
                }
            }

            foreach (var action in history.HandActions.Where(x => (int)x.Street <= (int)currentStreet))
            {

                String plr = "";
                plr = action.PlayerName;

                // Dealer actions
                if (action.Street == HandHistories.Objects.Cards.Street.Preflop && street != 0)
                {
                    street = 0; // Start of the preflop round
                    PotSizeByStreet[0] = pot;
                }

                #region DealingFlop
                if (action.Street == HandHistories.Objects.Cards.Street.Flop && street != 1)
                {
                    street = 1;
                    CommunityCards[street] = string.Join("", history.CommunityCards.GetBoardOnStreet(HandHistories.Objects.Cards.Street.Flop));
                    PotSizeByStreet[street] = pot;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        last_commit[key] = (int)last_commit[key] + (int)this_commit[key];
                        this_commit[key] = 0;
                        last_call[key] = this_call[key];
                        this_call[key] = 0;
                    }


                    // Button (or the player last to act) is the initial post-flop "attacker"
                    for (int i = this.MaxSeats; i > this.ButtonSeat; i--)
                    {
                        foreach (String key in Players.Keys)
                        {
                            if (acted_preflop.Contains(key) && !((bool)acted_preflop[key])) in_hand[key] = false; // If player never acted preflop, he cannot be in the hand
                            if ((Players[key] as Player).SeatNumber == i)
                            {
                                if ((bool)in_hand[key])
                                {
                                    attacker = key;
                                    break;
                                }
                            }
                        }
                    }
                    for (int i = this.ButtonSeat; i >= 1; i--)
                    {
                        foreach (String key in Players.Keys)
                        {
                            if ((Players[key] as Player).SeatNumber == i)
                            {
                                if ((bool)in_hand[key])
                                {
                                    attacker = key;
                                    break;
                                }
                            }
                        }
                    }
                    defender = "";  // No defender until someone bets
                }
                #endregion

                #region DealingTurn
                if (action.Street == HandHistories.Objects.Cards.Street.Turn && street != 2)
                {
                    street = 2;
                    CommunityCards[street] = history.CommunityCards.GetBoardOnStreet(HandHistories.Objects.Cards.Street.Turn).Last().ToString();
                    PotSizeByStreet[street] = pot;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        last_commit[key] = (int)last_commit[key] + (int)this_commit[key];
                        this_commit[key] = 0;
                        last_call[key] = this_call[key];
                        this_call[key] = 0;
                    }

                    // Button (or the player last to act) is the initial post-flop "attacker"
                    for (int i = this.MaxSeats; i > this.ButtonSeat; i--)
                    {
                        foreach (String key in Players.Keys)
                            if ((Players[key] as Player).SeatNumber == i)
                            {
                                if ((bool)in_hand[key])
                                {
                                    attacker = key;
                                    break;
                                }
                            }
                    }

                    for (int i = this.ButtonSeat; i >= 1; i--)
                    {
                        foreach (String key in Players.Keys)
                            if ((Players[key] as Player).SeatNumber == i)
                            {
                                if ((bool)in_hand[key])
                                {
                                    attacker = key;
                                    break;
                                }
                            }
                    }

                    defender = "";  // No defender until someone bets
                }
                #endregion

                #region DealingRiver


                if (action.Street == HandHistories.Objects.Cards.Street.River && street != 3)
                {
                    street = 3;
                    CommunityCards[street] = history.CommunityCards.GetBoardOnStreet(HandHistories.Objects.Cards.Street.River).Last().ToString();
                    PotSizeByStreet[street] = pot;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        last_commit[key] = (int)last_commit[key] + (int)this_commit[key];
                        this_commit[key] = 0;
                        last_call[key] = this_call[key];
                        this_call[key] = 0;
                    }

                    // Button (or the player last to act) is the initial post-flop "attacker"
                    for (int i = this.MaxSeats; i > this.ButtonSeat; i--)
                    {
                        foreach (String key in Players.Keys)
                            if ((Players[key] as Player).SeatNumber == i)
                            {
                                if ((bool)in_hand[key])
                                {
                                    attacker = key;
                                    break;
                                }
                            }
                    }
                    for (int i = this.ButtonSeat; i >= 1; i--)
                    {
                        foreach (String key in Players.Keys)
                            if ((Players[key] as Player).SeatNumber == i)
                            {
                                if ((bool)in_hand[key])
                                {
                                    attacker = key;
                                    break;
                                }
                            }
                    }
                    defender = "";  // No defender until someone bets
                }
                #endregion

                #region Player Actions
                // Player actions 

                if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.SMALL_BLIND)
                {
                    var SBPlayer = history.HandActions.First(x => x.HandActionType == HandHistories.Objects.Actions.HandActionType.SMALL_BLIND);
                    SBName = SBPlayer.PlayerName;
                    int SBAmount = (int)Math.Round(SBPlayer.Amount * 100);
                    if (SBAmount < 0)
                        SBAmount *= -1;

                    this_commit[SBName] = SBAmount;
                    pot += SBAmount;
                }

                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.BIG_BLIND)
                {
                    var BBPlayer = history.HandActions.First(x => x.HandActionType == HandHistories.Objects.Actions.HandActionType.BIG_BLIND);
                    BBName = BBPlayer.PlayerName;

                    var BBAmount = (int)Math.Round(BBPlayer.Amount * 100);
                    if (BBAmount < 0)
                        BBAmount *= -1;

                    this_commit[BBName] = BBAmount;
                    attacker = BBName; // Big blind is the initial "attacker"
                    defender = ""; // No defender in unraised pots

                    pot += BBAmount;

                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.FOLD)
                {
                    Action act = new Action();
                    act.Street = street;
                    act.PlayerName = plr;
                    act.SAction = "Folds";
                    act.Amount = 0;
                    act.Attacker = attacker;
                    act.Defender = defender;
                    act.AttackerRisk = attacker_risk;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        act.InHand[key] = in_hand[key];
                        act.ThisStreetCommitment[key] = this_commit[key];
                        act.LastStreetCommitment[key] = last_commit[key];
                        act.LastStreetCall[key] = last_call[key];
                    }
                    this_call[plr] = 0;
                    in_hand[plr] = false;
                    if (street == 0)
                        acted_preflop[plr] = true; // Make sure we only count players who were dealt in the hand

                    if (street == 0) PreflopActions.Add(act);
                    else PostflopActions[street].Add(act);
                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.CHECK)
                {
                    Action act = new Action();
                    act.Street = street;
                    act.PlayerName = plr;
                    act.SAction = "Checks";
                    act.Amount = 0;
                    act.Attacker = attacker;
                    act.Defender = defender;
                    act.AttackerRisk = attacker_risk;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        act.InHand[key] = in_hand[key];
                        act.ThisStreetCommitment[key] = this_commit[key];
                        act.LastStreetCommitment[key] = last_commit[key];
                        act.LastStreetCall[key] = last_call[key];
                    }
                    this_call[plr] = 0;
                    if (street == 0) acted_preflop[plr] = true; // Make sure we only count players who were dealt in the hand

                    if (street == 0) PreflopActions.Add(act);
                    else PostflopActions[street].Add(act);

                    // Game theory updates
                    defender = ""; // No-one needs to defend the pot when another non-raising player is also in (i.e. the hand won't end even if defender folds)
                    attacker = plr; // Last caller/checker before the raiser gets the role of pot defender
                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.CALL)
                {
                    Action act = new Action();
                    act.Street = street;
                    act.PlayerName = plr;
                    act.SAction = "Calls";
                    var curAmout = action.Amount < 0 ? action.Amount * -1 : action.Amount;
                    act.Amount = (int)Math.Round(curAmout * 100);
                    act.Attacker = attacker;
                    act.Defender = defender;
                    act.AttackerRisk = attacker_risk;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        act.InHand[key] = in_hand[key];
                        act.ThisStreetCommitment[key] = this_commit[key];
                        act.LastStreetCommitment[key] = last_commit[key];
                        act.LastStreetCall[key] = last_call[key];
                    }
                    this_call[plr] = act.Amount;
                    this_commit[plr] = (int)this_commit[plr] + (int)act.Amount;
                    pot += act.Amount;
                    if (street == 0) acted_preflop[plr] = true; // Make sure we only count players who were dealt in the hand

                    if (street == 0) PreflopActions.Add(act);
                    else PostflopActions[street].Add(act);

                    // Game theory updates
                    defender = ""; // No-one needs to defend the pot when another non-raising player is also in (i.e. the hand won't end even if defender folds)
                    attacker = plr; // Last caller/checker before the raiser gets the role of pot defender
                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.BET)
                {
                    Action act = new Action();
                    act.Street = street;
                    act.PlayerName = plr;
                    act.SAction = "Bets";
                    var curAmout = action.Amount < 0 ? action.Amount * -1 : action.Amount;
                    act.Amount = (int)Math.Round(curAmout * 100);
                    act.Attacker = attacker;
                    act.Defender = defender;
                    act.AttackerRisk = attacker_risk;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        act.InHand[key] = in_hand[key];
                        act.ThisStreetCommitment[key] = this_commit[key];
                        act.LastStreetCommitment[key] = last_commit[key];
                        act.LastStreetCall[key] = last_call[key];
                    }
                    this_call[plr] = 0;
                    this_commit[plr] = (int)this_commit[plr] + (int)act.Amount;
                    pot += act.Amount;
                    if (street == 0) acted_preflop[plr] = true; // Make sure we only count players who were dealt in the hand

                    if (street == 0) PreflopActions.Add(act);
                    else PostflopActions[street].Add(act);

                    // Game theory updates
                    defender = attacker; // The previous attacker is the new defender
                    attacker = plr;      // The bettor is the new attacker
                    attacker_risk = act.Amount;
                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.RAISE)
                {
                    Action act = new Action();
                    act.Street = street;
                    act.PlayerName = plr;
                    act.SAction = "Raises";
                    var curAmout = action.Amount < 0 ? action.Amount * -1 : action.Amount;
                    act.Amount = (int)Math.Round(curAmout * 100);
                    act.Attacker = attacker;
                    act.Defender = defender;
                    act.AttackerRisk = attacker_risk;

                    List<string> keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry de in this_commit)
                        keys.Add(de.Key.ToString());

                    foreach (String key in keys)
                    {
                        act.InHand[key] = in_hand[key];
                        act.ThisStreetCommitment[key] = this_commit[key];
                        act.LastStreetCommitment[key] = last_commit[key];
                        act.LastStreetCall[key] = last_call[key];
                    }
                    this_call[plr] = 0;
                    this_commit[plr] = (int)this_commit[plr] + (int)act.Amount;
                    pot += act.Amount /*- action.this_street_commitment[plr]*/; // Notice! XML format contains only the amount added (which mathematically makes a lot of sense), not the more typical "raise" or "raise to" amount that is shown by poker rooms and raw hand histories
                    if (street == 0) acted_preflop[plr] = true; // Make sure we only count players who were dealt in the hand

                    if (street == 0) PreflopActions.Add(act);
                    else PostflopActions[street].Add(act);

                    // Game theory updates
                    defender = attacker; // The previous attacker is the new defender
                    attacker = plr;      // The bettor is the new attacker
                    attacker_risk = act.Amount /*- action.this_street_commitment[plr]*/; // Notice! XML format contains only the amount added (which mathematically makes a lot of sense), not the more typical "raise" or "raise to" amount that is shown by poker rooms and raw hand histories
                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.UNCALLED_BET)// Update the pot size, but don't store as an action
                {
                    Action act = new Action();
                    act.Street = street;
                    act.PlayerName = plr;
                    act.SAction = "Returns";
                    var curAmout = action.Amount < 0 ? action.Amount * -1 : action.Amount;
                    act.Amount = (int)Math.Round(curAmout * 100);
                    this_commit[plr] = (int)this_commit[plr] - (int)act.Amount;
                    pot -= act.Amount;
                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.SHOW)
                {
                    (Players[plr] as Player).Showdown = true;
                }
                else if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.WINS)
                {
                    PotSizeByStreet[4] = pot; // Update the final pot size
                    var curAmout = action.Amount < 0 ? action.Amount * -1 : action.Amount;
                    (Players[plr] as Player).Wins = (int)Math.Round(curAmout * 100);
                }
            }

            #endregion

            List<String> playersNotInPreflop = new List<String>();

            foreach (String playerName in this.Players.Keys)
            {
                bool playerInHand = false;
                foreach (Action preflopAction in this.PreflopActions)
                {
                    if (preflopAction.PlayerName.Equals(playerName))
                    {
                        playerInHand = true;
                        break;
                    }
                }
                if (!playerInHand)
                {
                    playersNotInPreflop.Add(playerName);
                }
            }

            // Update the position numbers for the players (should find a better way to do this than by looping)
            int pos_id = 0;

            for (int i = (Players[BBName] as Player).SeatNumber + 1; i <= MaxSeats; i++)
            {
                foreach (String key in Players.Keys)
                {
                    if ((Players[key] as Player).SeatNumber == i)
                    {
                        (Players[key] as Player).Position = pos_id;
                        pos_id++;
                        break;
                    }
                }
            }

            for (int i = 1; i <= (Players[BBName] as Player).SeatNumber; i++)
            {
                foreach (String key in Players.Keys)
                {
                    if ((Players[key] as Player).SeatNumber == i)
                    {
                        (Players[key] as Player).Position = pos_id;
                        pos_id++;
                        break;
                    }
                }
            }

            //if ((Players[HeroName] as Player).Cards.ToLower().Equals("9dtc") || (Players[HeroName] as Player).Cards.ToLower().Equals("tc9d"))
            //{
            //}

            List<String> comCardsArr = new List<String>();

            if (CommunityCards[1] != null)
            {
                comCardsArr.Add(CommunityCards[1].Substring(0, 2));
                comCardsArr.Add(CommunityCards[1].Substring(2, 2));
                comCardsArr.Add(CommunityCards[1].Substring(4, 2));
            }
            else
            {
                comCardsArr.Add(null);
            }

            comCardsArr.Add(CommunityCards[2]);
            comCardsArr.Add(CommunityCards[3]);

            int nbPairs = 0;
            bool hasPairs = true;

            while (hasPairs)
            {
                hasPairs = false;
                for (int i = 0; i < comCardsArr.Count - 1; i++)
                {
                    if (comCardsArr[i] == null) continue;
                    for (int j = i + 1; j < comCardsArr.Count; j++)
                    {
                        if (comCardsArr[j] != null && comCardsArr[i][0].Equals(comCardsArr[j][0]))
                        {
                            nbPairs++;
                            hasPairs = true;
                            comCardsArr.RemoveAt(i);
                            break;
                        }
                    }
                    if (hasPairs) break;
                }
            }

            if (this.Players.Count >= 7)
            {
                this.Is6Max = false;
                this.IsFR = true;
            }
            else if (this.Players.Count >= 5)
            {
                this.Is6Max = true;
                this.IsFR = false;
            }


            if (this.SBName == null)
            {

            }

            if (this.PostflopActions.Length > 0)
            {
                foreach (Action postflopAction in this.PostflopActions[1])
                {
                    if (postflopAction.PlayerName.Equals(this.HeroName))
                    {
                        HeroIsOnFlop = true;
                        break;
                    }
                }
            }
        }
    }
}
