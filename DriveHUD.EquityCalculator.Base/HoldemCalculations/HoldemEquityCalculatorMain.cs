using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.Calculations
{
    public class HoldemEquityCalculator
    {
        internal static long[] maskLookup = new long[]{
    0,
    //2        3       4       5        6      7        8       9       10     J         Q       K     
    1,      2,      4,      8,      16,     32,     64,     128,    256,    512,    1024,   2048,       4096,
    1<<16,  2<<16,  4<<16,  8<<16,  16<<16, 32<<16, 64<<16, 128<<16,256<<16,512<<16,1024<<16,2048<<16,  4096<<16,
    0x100000000,    0x200000000,    0x400000000,    0x800000000,    0x1000000000,   0x2000000000,   0x4000000000,   0x8000000000,   0x10000000000,  0x20000000000,  0x40000000000,  0x80000000000,      0x100000000000,
    0x1000000000000,    0x2000000000000,    0x4000000000000,    0x8000000000000,    0x10000000000000,   0x20000000000000,   0x40000000000000,   0x80000000000000,   0x100000000000000,  0x200000000000000,  0x400000000000000,  0x800000000000000,      0x1000000000000000,
        };

        public static event EventHandler PartialResult;

        private String mConfusedBy;
        private int mHandCount;
        private int mBoardCount;
        private int mHoleCardCount = 0;
        private int cWeightCount = 1326;
        private int NUM_PLAYERS;
        private long mBoard;
        private long mDead = 0;
        private double mWeightedCount;
        private bool mPreFlop = false;
        private bool mBig;
        private List<Player> mPlayer = new List<Player>();

        public static async Task<List<double[]>> CalculateEquityAsync(IEnumerable<string> ranges, string sBoard, bool isHoldem6Plus, CancellationToken ct)
        {
            var hd = new HoldemEquityCalculator();

            var board = sBoard;

            hd.GetBoardCards(board, ref hd.mBoard, ref hd.mBoardCount, 5);

            if (isHoldem6Plus)
            {
                hd.GetHoldem6PlusDeadCards(ref hd.mDead);
            }

            var players = new List<Player>();

            foreach (var range in ranges)
            {
                players.Add(new Player(range));
            }

            hd.mPlayer = players;
            hd.NUM_PLAYERS = players.Count;

            await Task.Run(() =>
            {
                hd.OnCalc(ct);
            }, ct);

            var eqs = new List<double[]>();

            foreach (var player in players)
            {
                eqs.Add(new double[] { player.Equity, player.WinPrct, player.TiePrct });
            }

            return eqs;
        }

        private void GetHoldem6PlusDeadCards(ref long dead)
        {
            dead = 0;

            for (var rank = 0; rank < 4; rank++)
            {
                for (var suit = 0; suit < 4; suit++)
                {
                    long mask = WeightTable.GetCardMask(suit * 13 + rank);

                    dead |= mask;
                }
            }
        }

        private void GetBoardCards(String boardString, ref long board, ref int count, int max)
        {
            board = 0;
            count = 0;

            int pos = 0;

            while (!boardString.Equals(""))
            {
                pos += boardString.Length;
                TrimLeft(ref boardString, " ,.");
                pos -= boardString.Length;

                if (boardString.Equals(""))
                    break;

                int rank = GetRank(boardString[0]);
                if (rank < 0)
                {
                    throw new ArgumentException("Can't parse rank value", "boardString");
                }

                boardString = boardString.Substring(1);
                pos += 1;

                if (boardString.Equals(""))
                    break;

                int suit = GetSuit(boardString[0]);
                if (suit < 0)
                {
                    throw new ArgumentException("Can't parse suit value", "boardString");
                }

                boardString = boardString.Substring(1);
                pos += 1;

                long mask = WeightTable.GetCardMask(suit * 13 + rank);
                if ((mask & board) > 0)
                {
                    throw new ArgumentException("Board contains duplicate cards", "boardString");
                }

                board |= mask;

                count++;
            }

            if (count > max)
            {
                throw new ArgumentOutOfRangeException("boardString", count, "Board contains too many cards");
            }
        }

        private void OnCalc(CancellationToken ct)
        {
            int activeCount = 0;
            foreach (Player plyr in mPlayer)
            {
                plyr.mActive = false;
                plyr.mHoleTable.Reset();

                String holeString = plyr.Range;

                TrimLeft(ref holeString, " .,");

                if (holeString.Equals(""))
                    continue;

                if (ReadHoleCombos(holeString, ref plyr.mHoleTable, ref plyr.mFoldTable) < 0)
                {
                    return;
                }

                if (plyr.mHoleTable.ClearIntersects(0) == 0)
                {
                    return;
                }

                if ((plyr.mHoleCount = plyr.mHoleTable.ClearIntersects(mBoard | mDead)) == 0)
                {
                    return;
                }

                plyr.mActive = true;
                activeCount++;
            }

            if (activeCount < 2)
            {
                throw new ArgumentOutOfRangeException("mPlayer", activeCount, "Need at least 2 players with valid hands for showdown");
            }

            FoldRange();

            mPreFlop = (activeCount == 2) && (mBoardCount == 0) && (mDead == 0);

            if (!mPreFlop && (mBoardCount == 0))
            {
                mBig = true;
            }
            else
            {
                mBig = false;
            }

            EvaluatePlayerHand(0, mDead, ct);

            foreach (Player player in mPlayer)
            {
                player.Equity = player.mEquity * 100.0 / mWeightedCount;
                player.WinPrct = player.mWins * 100.0 / mWeightedCount;
                player.TiePrct = player.mTies * 100.0 / mWeightedCount;
            }
        }

        private void EvaluatePlayerHand(int plrIndex, long dead, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (plrIndex == NUM_PLAYERS)
            {
                mHoleCardCount += 1;

                if (mPreFlop)
                    PreFlopLookup();
                else
                    EvaluateHands(mBoard, dead, mBoardCount, 0);

                if (mBig)
                {
                    OutputPartial();
                }

                return;
            }

            Player plyr = mPlayer[plrIndex];
            if (!plyr.mActive)
            {
                EvaluatePlayerHand(plrIndex + 1, dead, ct);
                return;
            }

            for (int i = 0; i < cWeightCount; i++)
            {
                if (plyr.mHoleTable.mWeights[i].mWeight == 0.0f)
                    continue;

                //does this combo intersect with (board, dead cards)
                if ((plyr.mHoleTable.mWeights[i].mCards & (mBoard | dead)) > 0) //ATT
                {
                    if (mBig)
                    {
                        //find out how many we are combos we are skipping
                        int skipping = 1;
                        for (int otherPlrIndex = plrIndex + 1; otherPlrIndex < NUM_PLAYERS; otherPlrIndex++)
                        {
                            Player tPlyr = mPlayer[otherPlrIndex];
                            if (tPlyr.mActive)
                                skipping *= tPlyr.mHoleTable.ClearIntersects(0);
                        }

                        mHoleCardCount += skipping;
                    }
                    continue;
                }

                plyr.mCurrentWeight = plyr.mHoleTable.mWeights[i].mWeight;
                plyr.mCurrentHand = plyr.mHoleTable.mWeights[i].mCards;



                EvaluatePlayerHand(plrIndex + 1, (dead | plyr.mCurrentHand), ct);
            }
        }

        private void OutputPartial()
        {
            if (PartialResult != null)
            {
                List<double> eqs = new List<double>();
                for (int p = 0; p < NUM_PLAYERS; p++)
                {
                    if (mPlayer[p].mActive)
                    {
                        eqs.Add(mPlayer[p].mEquity * 100.0f / mWeightedCount);
                    }
                }
                PartialResult(eqs, null);
            }
        }

        private void EvaluateHands(long board, long dead, int boardCount, int lastCardIndex)
        {
            //is board complete?
            if (boardCount == 5)
            {
                //each player should have a sorted array containing the hole cards and the board cards
                EvaluateComplete(board);
                return;
            }

            long cardMask;
            int cardIndex;

            for (cardIndex = lastCardIndex + 1; cardIndex < 53; cardIndex++)
            {
                cardMask = (long)maskLookup[cardIndex];

                if ((cardMask & (dead | board)) > 0)//ATT
                    continue;

                EvaluateHands(board | cardMask, dead | cardMask, boardCount + 1, cardIndex);
            }
        }

        private void EvaluateComplete(long board)
        {
            int[] winners = new int[NUM_PLAYERS];
            int winnerCount = 0;
            long best = 0;

            //we have a mask for each active player

            //we have weight for each player

            //we have a complete board

            double weight = 1.0;

            //evaluate hands
            int i;
            for (i = 0; i < NUM_PLAYERS; i++)
            {
                if (mPlayer[i].mCurrentHand == 0)
                    continue;

                weight *= mPlayer[i].mCurrentWeight;

                int score = (int)Evalz.Eval((long)(mPlayer[i].mCurrentHand | board), 7);

                //gen mask table for cards

                if (score > best)
                {
                    winners[0] = i;
                    winnerCount = 1;
                    best = (long)score;
                }
                else if (score == best)
                {
                    winners[winnerCount++] = i;
                }
            }

            //award winnings
            if (winnerCount == 1)
            {
                mPlayer[winners[0]].mEquity += weight;
                mPlayer[winners[0]].mWins += weight;
            }
            else
            {
                for (i = 0; i < winnerCount; i++)
                {
                    mPlayer[winners[i]].mEquity += weight / (float)(winnerCount);
                    mPlayer[winners[i]].mTies += weight / (float)(winnerCount);
                }
            }

            mWeightedCount += weight;
            mHandCount += 1;
        }

        private void PreFlopLookup()
        {
            //get the two masks
            long mask0 = 0;
            long mask1 = 0;

            Player plyr0 = null;
            foreach (Player player in mPlayer)
            {
                if (player.mActive)
                {
                    mask0 = player.mCurrentHand;
                    plyr0 = player;
                    break;
                }
            }

            Player plyr1 = null;
            bool afterPlyr0 = false;
            foreach (Player player in mPlayer)// (plyr1 = plyr0+1; plyr1<mPlayer+NUM_PLAYERS; plyr1++)
            {
                if (player == plyr0)
                {
                    afterPlyr0 = true;
                    continue;
                }
                if (!afterPlyr0) continue;

                if (player.mActive)
                {
                    mask1 = player.mCurrentHand;
                    plyr1 = player;
                    break;
                }
            }

            double weight = plyr0.mCurrentWeight * plyr1.mCurrentWeight;

            int vsIdx = GetVsIndex(mask0, mask1);

            if (vsIdx > 0)
            {
                plyr1.mWins += GetVsValue(vsIdx, 0) * weight;
                plyr1.mTies += GetVsValue(vsIdx, 1) * weight;
                plyr1.mEquity += (GetVsValue(vsIdx, 0) * weight) + (GetVsValue(vsIdx, 1) * weight);

                plyr0.mWins += (1712304 - GetVsValue(vsIdx, 0) - GetVsValue(vsIdx, 1) * 2) * weight;
                plyr0.mTies += GetVsValue(vsIdx, 1) * weight;
                plyr0.mEquity += (1712304 - GetVsValue(vsIdx, 0) - GetVsValue(vsIdx, 1)) * weight;
            }
            else
            {
                plyr0.mWins += GetVsValue(-vsIdx, 0) * weight;
                plyr0.mTies += GetVsValue(-vsIdx, 1) * weight;
                plyr0.mEquity += (GetVsValue(-vsIdx, 0) + GetVsValue(-vsIdx, 1)) * weight;

                plyr1.mWins += (1712304 - GetVsValue(-vsIdx, 0) - GetVsValue(-vsIdx, 1) * 2) * weight;
                plyr1.mTies += GetVsValue(-vsIdx, 1) * weight;
                plyr1.mEquity += (1712304 - GetVsValue(-vsIdx, 0) - GetVsValue(-vsIdx, 1)) * weight;
            }

            mHandCount += 1712304;
            mWeightedCount += weight * 1712304;
        }

        private void FoldRange()
        {
            foreach (Player plyr in mPlayer)// (plyr = mPlayer; plyr<mPlayer+NUM_PLAYERS; plyr++)
            {
                if (!plyr.mActive)
                    continue;

                RangeWeightExcluding(0, plyr, (long)(mBoard | mDead), 1.0F);

                float foldWt = plyr.mFoldTable.GetTotalCumWeight();

                float callWt = plyr.mHoleTable.GetTotalCumWeight();
                plyr.mFolds = foldWt / (foldWt + callWt);
            }
        }

        private void RangeWeightExcluding(int plrIndex, Player pExcludedPlyr, long dead, float combinedWeight)
        {
            if (plrIndex == NUM_PLAYERS)
            {
                //we now have a valid combo of all the other players included in the dead mask

                //go through the exluded players weights. 
                //if they dont intersect add the current combined weight to each weights cumulative total

                for (int i = 0; i < cWeightCount; i++)
                {
                    if ((pExcludedPlyr.mHoleTable.mWeights[i].mCards & dead) == 0)
                    {
                        pExcludedPlyr.mHoleTable.mWeights[i].mCumWeight += pExcludedPlyr.mHoleTable.mWeights[i].mWeight * combinedWeight;
                    }

                    if ((pExcludedPlyr.mFoldTable.mWeights[i].mCards & dead) == 0)
                    {
                        pExcludedPlyr.mFoldTable.mWeights[i].mCumWeight += pExcludedPlyr.mFoldTable.mWeights[i].mWeight * combinedWeight;
                    }
                }

                return;
            }

            Player plyr = mPlayer[plrIndex];
            //skip inactive or excluded plyrs
            if (!plyr.mActive || (plyr == pExcludedPlyr))
            {
                RangeWeightExcluding(plrIndex + 1, pExcludedPlyr, dead, combinedWeight);
                return;
            }


            for (int i = 0; i < cWeightCount; i++)
            {
                if (plyr.mHoleTable.mWeights[i].mWeight == 0.0f)
                    continue;

                //does this combo intersect with (board, dead cards)
                if ((plyr.mHoleTable.mWeights[i].mCards & dead) > 0)
                    continue;

                //recurse onto next plyr
                RangeWeightExcluding(plrIndex + 1,
                                    pExcludedPlyr,
                                    (dead | plyr.mHoleTable.mWeights[i].mCards),
                                    combinedWeight * plyr.mHoleTable.mWeights[i].mWeight);
            }

        }

        private int ReadHoleCombos(String handString, ref WeightTable pWTbl, ref WeightTable pFoldTbl)
        {
            String subString = "", weightString, originalString;

            originalString = handString;

            pWTbl.Reset();
            pFoldTbl.Reset();

            while (!handString.Equals(""))
            {
                if (handString[0] == '/')
                {
                    if (pWTbl == pFoldTbl)
                    {
                        mConfusedBy = "/";
                        return -1;
                    }

                    pWTbl = pFoldTbl;
                    TrimLeft(ref handString, " /");
                }

                float weight = 1.0f;

                int limit = FindOneOf(handString, ". ,(/");
                if (limit > 0)
                {
                    subString = handString.Substring(0, limit);
                    handString = handString.Substring(limit);//ATT

                    if (!handString.Equals("") && handString[0] == '(')
                    {
                        limit = handString.IndexOf(")");
                        weightString = handString.Substring(1, limit - 1);
                        handString = handString.Substring(limit + 1);

                        weight = (float)Convert.ToDouble(weightString) / 100.0f;
                    }

                    TrimLeft(ref handString, ", .(");

                }
                else if (limit == 0)
                {
                }
                else
                {
                    subString = handString;
                    handString = "";
                }

                //process the substring and add the weight indices generated to a list

                //1st char should be a rank

                pWTbl.ResetFlags();

                String originalSubstring = subString;


                int rank11 = 0, rank12 = 0;
                int suit11, suit12;
                int rank21 = 0, rank22 = 0;
                int suit21, suit22;
                int suited1, suited2;

                //suited=1, offsuited=2, both=0
                suited1 = suited2 = 0;

                //default to no specified suit
                suit11 = suit12 = -1;
                suit21 = suit22 = -1;

                if (InterpretHoleString(ref subString, ref rank11, ref rank12, ref suit11, ref suit12, ref suited1) < 0)
                {
                    int gap = -1;
                    int suit = 0;

                    //special cases
                    if (String.Compare(subString, "scc", true) == 0)
                    {
                        gap = 1;
                        suit = 0;
                    }
                    else if (String.Compare(subString, "scd", true) == 0)
                    {
                        gap = 1;
                        suit = 1;
                    }
                    else if (String.Compare(subString, "sch", true) == 0)
                    {
                        gap = 1;
                        suit = 2;
                    }
                    else if (String.Compare(subString, "scs", true) == 0)
                    {
                        gap = 1;
                        suit = 3;
                    }
                    else if (String.Compare(subString, "ogc", true) == 0)
                    {
                        gap = 2;
                        suit = 0;
                    }
                    else if (String.Compare(subString, "ogd", true) == 0)
                    {
                        gap = 2;
                        suit = 1;
                    }
                    else if (String.Compare(subString, "ogh", true) == 0)
                    {
                        gap = 2;
                        suit = 2;
                    }
                    else if (String.Compare(subString, "ogs", true) == 0)
                    {
                        gap = 2;
                        suit = 3;
                    }

                    if (gap >= 1)
                    {
                        for (rank11 = 8; rank11 > 2; rank11--)
                        {
                            FlagWeight(pWTbl, rank11, rank11 - gap, suit, suit, 0);
                        }
                    }
                    else
                    {
                        mConfusedBy = originalSubstring;
                        return -1;
                    }
                }

                if (subString.Equals(""))
                {
                    FlagWeight(pWTbl, rank11, rank12, suit11, suit12, suited1);
                    //add all weights with rank11 rank12        
                }
                else if (subString[0] == '+')
                {
                    //pairs
                    if (rank11 == rank12)
                    {
                        do
                        {
                            FlagWeight(pWTbl, rank11, rank12, suit11, suit12, suited1);
                            rank11++;
                            rank12++;
                        } while (rank11 < 13);
                    }
                    else
                    {
                        do
                        {
                            FlagWeight(pWTbl, rank11, rank12, suit11, suit12, suited1);
                            rank12++;
                        } while (rank12 < rank11);
                    }
                }
                else if (subString[0] == '-')
                {
                    subString = subString.Substring(1);
                    if (InterpretHoleString(ref subString, ref rank21, ref rank22, ref suit21, ref suit22, ref suited2) == 0)
                    {
                        mConfusedBy = originalSubstring;
                        return -1;
                    }

                    //need to check that suit11 == suit21
                    //need to check that suit12 == suit22
                    //need to check that suited1 == suited2

                    if ((suited1 != suited2) || (suit11 != suit21) || (suit12 != suit22))
                    {
                        mConfusedBy = originalSubstring;
                        return -1;
                    }

                    //pairs?
                    if (rank11 == rank12)
                    {
                        //1st cards are pairs
                        if (rank21 != rank22)
                        {
                            mConfusedBy = originalSubstring;
                            return -1;
                        }

                        int step = (rank21 > rank11) ? 1 : -1;
                        int count = (rank21 - rank11) * step;

                        do
                        {
                            FlagWeight(pWTbl, rank11, rank12, suit11, suit12, suited1);
                            rank11 += step;
                            rank12 = rank11;
                            count--;
                        } while (count >= 0);
                    }
                    //non pairs so 1st card should be the same ?
                    else if (rank11 == rank21)
                    {
                        //add all combos inbetween rank11, rank12-rank22

                        int step = (rank22 > rank12) ? 1 : -1;
                        int count = (rank22 - rank12) * step;

                        do
                        {
                            FlagWeight(pWTbl, rank11, rank12, suit11, suit12, suited1);
                            rank12 += step;
                            count--;
                        } while (count >= 0);
                    }
                    else if ((rank11 - rank12) == (rank21 - rank22))
                    {
                        int step = (rank22 > rank12) ? 1 : -1;
                        do
                        {
                            FlagWeight(pWTbl, rank11, rank12, suit11, suit12, suited1);
                            rank11 += step;
                            rank12 += step;
                        } while ((rank11 - rank21) * step <= 0);
                    }
                    else
                    {
                        mConfusedBy = originalSubstring;
                        return -1;
                    }
                }

                pWTbl.SetFlaggedWeight(weight);
            };

            pWTbl.DumpFlagged();


            return 1;
        }

        private void FlagWeight(WeightTable pTbl, int rank1, int rank2, int suit1, int suit2, int suited)
        {
            int card1, card2, index = 0;

            //add indices
            if ((suit1 >= 0) && (suit2 >= 0))
            {
                card1 = rank1 + (suit1 * 13);
                card2 = rank2 + (suit2 * 13);
                if (card1 == card2)
                    return;

                if (card1 < card2)
                {
                    index = card2 * (card2 - 1) / 2;
                    index += card1;
                }
                else if (card1 > card2)
                {
                    index = card1 * (card1 - 1) / 2;
                    index += card2;
                }

                pTbl.mWeights[index].mFlag = true;
            }

            if (suit1 == -1)
            {
                for (suit1 = 0; suit1 < 4; suit1++)
                {
                    if ((suited > 0) && (suit2 > 0))
                    {
                        if ((suit1 == suit2) ^ (suited == 1))
                            continue;
                    }

                    FlagWeight(pTbl, rank1, rank2, suit1, suit2, suited);
                }
            }
            else if (suit2 == -1)
            {
                for (suit2 = 0; suit2 < 4; suit2++)
                {
                    if (suited > 0)
                    {
                        if ((suit1 == suit2) ^ (suited == 1))
                            continue;
                    }

                    FlagWeight(pTbl, rank1, rank2, suit1, suit2, suited);
                }
            }
        }

        private int InterpretHoleString(ref String hole, ref int rank1, ref int rank2, ref int suit1, ref int suit2, ref int suited)
        {
            suited = 0;

            rank1 = GetRank(hole[0]);
            if (rank1 == -1)
                return -1;

            //chop off this char
            hole = hole.Substring(1);

            if (hole.Equals(""))
                return -1;


            rank2 = GetRank(hole[0]);
            if (rank2 == -1)
            {
                //if not a rank, do we have a suit?
                suit1 = GetSuit(hole[0]);

                if (suit1 == -1)
                    return -1;

                if (hole.Equals(""))
                    return -1;

                hole = hole.Substring(1);
                if (hole.Equals(""))
                    return -1;

                rank2 = GetRank(hole[0]);
                if (rank2 == -1)
                    return -1;
            }

            hole = hole.Substring(1);
            if (hole.Equals(""))
                return 1;

            //cant get 2nd suit if 1st not specified because 's' could be suited combo
            if (suit1 >= 0)
            {
                suit2 = GetSuit(hole[0]);
                if (suit2 != -1)
                {
                    hole = hole.Substring(1);
                    if (hole.Equals(""))
                        return 1;
                }
            }

            //last char should be 's', or 'o'
            if ((hole[0] == 's') || (hole[0] == 'S'))
            {
                suited = 1;
                hole = hole.Substring(1);

            }
            else if ((hole[0] == 'o') || (hole[0] == 'O'))
            {
                suited = 2;
                hole = hole.Substring(1);
            }

            //shouldnt have anything else left
            //if (hole.Equals(""))
            return 1;

            //return -1;
        }

        private void TrimLeft(ref String s, String toFind)
        {
            char[] toRemove = toFind.ToCharArray();
            bool startsWithToFind = true;
            while (startsWithToFind)
            {
                startsWithToFind = false;
                foreach (char toRem in toRemove)
                {
                    if (s.StartsWith(toRem.ToString()))
                    {
                        s = s.Substring(1);
                        startsWithToFind = true;
                        break;
                    }
                }
            }
        }

        private int FindOneOf(String s, String toFind)
        {
            int minIndex = -1;
            foreach (Char cToFind in toFind)
            {
                int index = -1;
                if ((index = s.IndexOf(cToFind.ToString())) != -1)
                    if (minIndex == -1 || index < minIndex) minIndex = index;
            }
            return minIndex;
        }

        private int GetVsValue(int row, int col)
        {
            return VSTable.vsTbl[row * 2 + col];
        }

        private int GetVsIndex(long mask0, long mask1)
        {
            int index;
            Weight pWt = mPlayer[0].mHoleTable.GetWeight(mask0);
            if (pWt == null)
                return -10000000;    //error
            int index0 = mPlayer[0].mHoleTable.mWeights.IndexOf(pWt);// pWt.mWeight - mPlayer[0].mHoleTable.mWeights[0].mWeight; //ATT

            pWt = mPlayer[0].mHoleTable.GetWeight(mask1);
            if (pWt == null)
                return -10000000;    //error

            int index1 = mPlayer[0].mHoleTable.mWeights.IndexOf(pWt);//ATT


            if (index0 > index1)
            {
                return index1 * (cWeightCount - 2) - (index1 * (index1 - 1)) / 2 + index0;
            }
            else
            {
                index = index0 * (cWeightCount - 2) - (index0 * (index0 - 1)) / 2 + index1;
                return -index;
            }
        }

        private int GetRank(char c)
        {
            switch (c)
            {
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return (c - '2');

                case 't':
                case 'T':
                    return 8;

                case 'J':
                case 'j':
                    return 9;

                case 'Q':
                case 'q':
                    return 10;

                case 'K':
                case 'k':
                    return 11;

                case 'A':
                case 'a':
                    return 12;
            }

            return -1;
        }

        private int GetSuit(char c)
        {
            switch (c)
            {
                case 'c':
                case 'C':
                    return 0;

                case 'd':
                case 'D':
                    return 1;

                case 'h':
                case 'H':
                    return 2;

                case 's':
                case 'S':
                    return 3;
            }

            return -1;
        }
    }
}
