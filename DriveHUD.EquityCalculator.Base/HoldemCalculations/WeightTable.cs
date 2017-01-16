using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.Calculations
{
    class WeightTable
    {

        internal List<Weight> mWeights = new List<Weight>(new Weight[1326]);//[cWeightCount];
        internal float mTotalWeight;

        //////////////////////////////////////////////////////////////////////
        // Construction/Destruction
        //////////////////////////////////////////////////////////////////////

        internal WeightTable()
        {
            Init();
        }


        internal void Init()
        {
            int card1Index, card2Index;

            //mpParent = pPlyr;
            for (int i = 0; i < mWeights.Count; i++)
            {
                mWeights[i] = new Weight();
            }
            for (card1Index = 0; card1Index < 51; card1Index++)
            {
                for (card2Index = card1Index + 1; card2Index < 52; card2Index++)
                {
                    GetWeight(card1Index, card2Index).mCards = GetCardMask(card1Index) | GetCardMask(card2Index);
                    if (GetWeight(card1Index, card2Index).mCards == 0)
                    {
                    }

                    GetWeight(card1Index, card2Index).mCard0 = card1Index + 1;
                    GetWeight(card1Index, card2Index).mCard1 = card2Index + 1;
                }
            }


            Reset();
        }


        internal void CheckTable()
        {
        }


        internal void Reset()
        {
            for (int i = 0; i < cWeightCount; i++)
            {
                mWeights[i].Reset();
            }
        }


        internal void ResetFlags()
        {
            for (int i = 0; i < cWeightCount; i++)
            {
                mWeights[i].mFlag = false;
            }
        }


        internal void DumpFlagged()
        {
        }


        internal void SetFlaggedWeight(float wt)
        {
            for (int i = 0; i < cWeightCount; i++)
            {
                if (mWeights[i].mFlag)
                {
                    mWeights[i].mWeight = wt;
                }
            }
        }






        int[] suitedIndex = new int[]{
    0,0,
    13, 13,
    26, 26,
    39, 39,
    100
};

        int[] pairIndex = new int[]{
    0, 13,
    0, 26,
    0, 39,
    13,26,
    13,39,
    26,39,
    100
};

        int[] offIndex = new int[]{
    0, 13,
    0, 26,
    0, 39,

    13,0,
    13,26,
    13,39,

    26,0,
    26,13,
    26, 39,

    39,0,
    39,13,
    39,26,
    100
};
        internal static long GetCardMask(int cardIndex)
        {
            long val = HoldemEquityCalculator.maskLookup[cardIndex + 1];
            return val;
        }


        internal int GetCardIndex(long cardMask, ref int card1, ref int card2)
        {
            //given mask return card indexes 
            card1 = 0;
            card2 = 0;      //error

            if (cardMask == 0)
                return 0;

            while ((cardMask & 1) == 0)
            {
                card1++;
                cardMask >>= 1;

                if (card1 == 13)
                    cardMask >>= 3;
                else if (card1 == 26)
                    cardMask >>= 3;
                else if (card1 == 39)
                    cardMask >>= 3;
            };

            if (cardMask <= 1)
                return 0;

            card2 = card1;
            do
            {
                card2++;
                cardMask >>= 1;

                if (card2 == 13)
                    cardMask >>= 3;
                else if (card2 == 26)
                    cardMask >>= 3;
                else if (card2 == 39)
                    cardMask >>= 3;

            } while ((cardMask & 1) == 0);

            return 1;
        }


        internal Weight GetWeight(int cardIndex1, int cardIndex2)
        {
            int index;

            if (cardIndex2 > cardIndex1)
            {
                index = cardIndex2 * (cardIndex2 - 1) / 2;
                index += cardIndex1;
            }
            else
            {
                index = cardIndex1 * (cardIndex1 - 1) / 2;
                index += cardIndex2;
            }

            return mWeights[index];
        }



        internal Weight GetWeight(long holeMask)
        {
            int card1 = 0, card2 = 0;
            if (GetCardIndex(holeMask, ref card1, ref card2) == 0)
                return null;

            Weight pWt = GetWeight(card1, card2);

            return pWt;
        }


        int cWeightCount = 1326;
        internal void Normalize(long deadCards)
        {
            float sum = 0;
            int count = 0;
            int w;

            for (w = 0; w < cWeightCount; w++)
            {
                Weight pWt = mWeights[w];

                if ((pWt.mCards & deadCards) > 0)
                    continue;

                if (pWt.mWeight > 0)
                {
                    count++;
                    sum += pWt.mWeight;
                }
            }

            float avgDiv = count / sum;

            for (w = 0; w < cWeightCount; w++)
            {
                Weight pWt = mWeights[w];
                pWt.mWeight *= avgDiv;
            }
        }





        internal int ClearIntersects(long mask)
        {
            int active = 0;

            for (int w = 0; w < cWeightCount; w++)
            {
                if ((mWeights[w].mCards & mask) > 0)//ATT
                {
                    mWeights[w].mWeight = 0;
                }
                else if (mWeights[w].mWeight != 0)
                {
                    active++;
                }
            }

            return active;
        }


        internal float GetTotalWeights()
        {
            float total = 0;
            for (int w = 0; w < cWeightCount; w++)
            {
                total += mWeights[w].mWeight;
            }

            mTotalWeight = total;
            return total;
        }

        internal float GetTotalCumWeight()
        {
            float total = 0;

            for (int w = 0; w < cWeightCount; w++)
            {
                total += mWeights[w].mCumWeight;
            }

            return total;
        }

    }
}
