using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Model
{
    public class PreDefinedRangeModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, KeyValuePair<List<String>, List<String>>> _ranges = new Dictionary<string, KeyValuePair<List<String>, List<String>>>();

        public Dictionary<string, KeyValuePair<List<String>, List<String>>> Ranges
        {
            get
            {
                return _ranges;
            }

            set
            {
                _ranges = value;
            }
        }

        public PreDefinedRangeModel()
        {
        }

        public static PreDefinedRangeModel GetDefaultHandsTo3BetWith()
        {
            var s = new String[] { "77", "88", "99", "TT", "JJ", "QQ", "KK", "AA", "A2s", "A3s", "A4s", "A5s", "A6s", "A7s", "A8s", "A9s", "ATs", "AJs", "AQs", "AKs", "K7s", "K8s", "K9s", "KTs", "KJs", "KQs", "Q7s", "Q8s", "Q9s", "QTs", "QJs", "J8s", "J9s", "JTs", "T8s", "T9s", "97s", "98s", "86s", "87s", "76s", "A2o", "A3o", "A4o", "A5o", "A6o", "A7o", "A8o", "A9o", "ATo", "AJo", "AQo", "AKo", "K9o", "KTo", "KJo", "KQo", "QTo", "QJo", "JTo", "T8o", "T9o", "97o", "98o", "86o", "87o", "76o" };
            PreDefinedRangeModel model = new PreDefinedRangeModel();

            model.Ranges.Add("3Bet", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));
            return model;
        }

        public static PreDefinedRangeModel GetDefaultHandsTo4BetWith()
        {
            var s = new String[] { "99", "TT", "JJ", "QQ", "KK", "AA", "A2s", "A3s", "A4s", "A5s", "A6s", "A7s", "A8s", "A9s", "ATs", "AJs", "AQs", "AKs", "K9s", "KTs", "KJs", "KQs", "QTs", "QJs", "A2o", "A3o", "A4o", "A5o", "A6o", "A7o", "A8o", "A9o", "ATo", "AJo", "AQo", "AKo", "KTo", "KJo", "KQo", "QTo", "QJo" };
            PreDefinedRangeModel model = new PreDefinedRangeModel();

            model.Ranges.Add("4Bet", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));
            return model;
        }

        public static PreDefinedRangeModel GetDefaultHandsToOpenRaiseWith()
        {
            var s = new String[]{
                "55","66","77","88","99","TT","JJ","QQ","KK","AA","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","T9s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
                };
            PreDefinedRangeModel model = new PreDefinedRangeModel();
            model.Ranges.Add("UTG", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[]{
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T9s","98s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
                };
            model.Ranges.Add("MP",
                 new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[]{
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K9s","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T9s","98s","87s","76s","65s","A9o","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo","T9o","98o","87o"
                };


            model.Ranges.Add("CO",
                 new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[]{
                    "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K5s","K6s","K7s","K8s","K9s","KTs","KJs","KQs","Q6s","Q7s","Q8s","Q9s","QTs","QJs","J6s","J7s","J8s","J9s","JTs","T7s","T8s","T9s","97s","98s","87s","76s","65s","54s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K8o","K9o","KTo","KJo","KQo","Q8o","Q9o","QTo","QJo","J8o","J9o","JTo","T8o","T9o","97o","98o","86o","87o","76o","65o"
                };


            model.Ranges.Add("BTN",
                  new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[]{
                    "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K6s","K7s","K8s","K9s","KTs","KJs","KQs","Q7s","Q8s","Q9s","QTs","QJs","J8s","J9s","JTs","T8s","T9s","98s","87s","76s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","Q9o","QTo","QJo","J9o","JTo","T8o","T9o","97o","98o","87o","76o"
                };


            model.Ranges.Add("SB",
                new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            return model;
        }

        public static PreDefinedRangeModel GetDefaultHandsToCallWithWhenNonBlindsOpenRaised()
        {
            var s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
            };

            PreDefinedRangeModel model = new PreDefinedRangeModel();

            model.Ranges.Add("UTG", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","T9s","98s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo","T9o","98o"
            };


            model.Ranges.Add("MP", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","43s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","87o","76o","65o","54o"
            };


            model.Ranges.Add("CO", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));
            return model;
        }

        public static PreDefinedRangeModel GetDefaultHandsToCallWithWhenBlindsOpenRaised()
        {

            var s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
            };
            PreDefinedRangeModel model = new PreDefinedRangeModel();

            model.Ranges.Add("UTG", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","87o","76o","65o","54o"
            };

            model.Ranges.Add("MP", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","43s","A8o","A9o","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","87o","76o","65o","54o"
            };

            model.Ranges.Add("CO", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K9s","KTs","KJs","KQs","QTs","QJs","J8s","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","98o","87o","76o","65o","54o"
            };

            model.Ranges.Add("BTN", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            s = new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K9s","KTs","KJs","KQs","Q9s","QTs","QJs","J8s","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","65o","54o"
            };

            model.Ranges.Add("SB", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));

            return model;
        }

        public static PreDefinedRangeModel GetDefaultHandsCallUnraisedPotWith()
        {
            var s = new String[] {
                //88-22, ATs-A2s, KTs, QTs+, J8s+, T7s+, 96s+, 86s+, 75s+, 65s, 54s,
                //A9o-A2o, KJo-KTo, QTo+, J8o+, T8o+, 97o+, 86o+, 75o+, 65o
                "22","33","44","55","66","77","88",
                "A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs",
                "KTs",
                "QTs","QJs","J8s","J9s","JTs","T7s","T8s","T9s","96s","97s","98s","86s","87s","75s","76s","65s","54s",
                "A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o",
                "KTo","KJo",
                "QTo","QJo","J8o","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","65o"
            };

            PreDefinedRangeModel model = new PreDefinedRangeModel();

            model.Ranges.Add("Limped Pot", new KeyValuePair<List<String>, List<String>>(s.ToList(), GetHandsFormatted(s.ToList())));
            return model;
        }

        public static PreDefinedRangeModel GetTop_4_7_Percent_Range()
        {
            var s = new String[] { "TT", "JJ", "QQ", "KK", "AA", "AQs", "AKs", "AQo", "AKo" };

            PreDefinedRangeModel model = new PreDefinedRangeModel();

            model.Ranges.Add("Top_4_7_Percent_Range", new KeyValuePair<List<string>, List<string>>(s.ToList(), GetHandsFormatted(s.ToList())));
            return model;
        }

        public static PreDefinedRangeModel GetTop_19_5_Percent_Range()
        {
            var s = new String[] {
                "22", "33", "44", "55", "66", "77", "88", "99", "TT", "JJ", "QQ", "KK", "AA",
                "A9s", "ATs", "AJs", "AQs", "AKs",
                "ATo", "AJo", "AQo", "AKo",
                "KTs", "KJs", "KQs",
                "KTo", "KJo", "KQo",
                "QTs", "QJs",
                "JTs", "T9s", "98s", "87s", "76s", "JTo",
                "QTo", "QJo"
            };

            PreDefinedRangeModel model = new PreDefinedRangeModel();

            model.Ranges.Add("Top_19_5_Percent_Range", new KeyValuePair<List<string>, List<string>>(s.ToList(), GetHandsFormatted(s.ToList())));
            return model;
        }

        private static List<String> GetHandsFormatted(List<String> hands)
        {
            List<char> cards = new List<char>(new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' });

            List<List<String>> allGroups = new List<List<String>>();


            List<String> handsToRemove = new List<String>();
            for (int i = 0; i < hands.Count; i++)
            {
                if (hands[i].Equals("")) continue;
                if (handsToRemove.Contains(hands[i])) continue;
                List<String> group = new List<String>();
                String firstHand = hands[i];
                group.Add(firstHand);

                String lastHand = firstHand;

                bool newCardAdded = true;
                while (newCardAdded)
                {
                    int countBefore = group.Count;
                    foreach (String hand in hands)
                    {
                        if (lastHand[0] == lastHand[1] && hand[0] == hand[1] && hand != firstHand && hand != lastHand)
                        {
                            if (cards.IndexOf(lastHand[0]) == cards.IndexOf(hand[0]) - 1)
                            {
                                group.Add(hand);
                            }
                            else if (cards.IndexOf(firstHand[0]) == cards.IndexOf(hand[0]) + 1)
                            {
                                group.Insert(0, hand);
                            }
                        }
                        else if (hand != firstHand && hand != lastHand && hand.Length > 2 && firstHand.Length > 2 && hand[2] == firstHand[2])
                        {
                            if (hand[0].Equals(lastHand[0]) && cards.IndexOf(lastHand[1]) == cards.IndexOf(hand[1]) - 1)
                            {
                                group.Add(hand);
                            }
                            else if (hand[0].Equals(firstHand[0]) && cards.IndexOf(firstHand[1]) == cards.IndexOf(hand[1]) + 1)
                            {
                                group.Insert(0, hand);
                            }
                        }
                        lastHand = group[group.Count - 1];
                        firstHand = group[0];
                    }
                    newCardAdded = countBefore != group.Count;
                }

                allGroups.Add(group);
                foreach (String hand in group)
                {
                    handsToRemove.Add(hand);
                }
            }

            List<String> res = new List<string>();
            foreach (List<String> group in allGroups)
            {
                if (group.Count > 1)
                {
                    if ((group[0][0] != group[0][1] && cards.IndexOf(group[group.Count - 1][1]) == cards.IndexOf(group[0][0]) - 1)
                        || (group[0][0] == group[0][1] && group[group.Count - 1][0] == 'A'))
                        res.Add(group[0] + "+");
                    else res.Add(group[0] + "-" + group[group.Count - 1]);
                }
                else res.Add(group[0]);
            }
            return res;
        }
    }
}
