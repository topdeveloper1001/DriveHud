using System;
using System.Collections;
using System.Collections.Generic;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class Jacob
    {
        internal static int TranslateCard(char rank, char suit)
        {
            int value;

            switch (rank)
            {
                case 'A': value = 14; break;
                case 'K': value = 13; break;
                case 'Q': value = 12; break;
                case 'J': value = 11; break;
                case 'T': value = 10; break;
                case '9': value = 9; break;
                case '8': value = 8; break;
                case '7': value = 7; break;
                case '6': value = 6; break;
                case '5': value = 5; break;
                case '4': value = 4; break;
                case '3': value = 3; break;
                case '2': value = 2; break;
                default: return -1;
            }

            switch (suit)
            {
                case 'h': value += 100; break;
                case 'd': value += 200; break;
                case 'c': value += 300; break;
                case 's': value += 400; break;
                default: return -1;
            }

            return value;
        }
        internal static int TranslateCard1(char rank, char suit)
        {
            int value;

            switch (rank)
            {
                case 'A': value = 14; break;
                case 'K': value = 13; break;
                case 'Q': value = 12; break;
                case 'J': value = 11; break;
                case 'T': value = 10; break;
                case '9': value = 9; break;
                case '8': value = 8; break;
                case '7': value = 7; break;
                case '6': value = 6; break;
                case '5': value = 5; break;
                case '4': value = 4; break;
                case '3': value = 3; break;
                case '2': value = 2; break;
                default: return -1;
            }

            switch (suit)
            {
                case 'h': value += 0; break;
                case 'd': value += 13; break;
                case 'c': value += 13 * 2; break;
                case 's': value += 13 * 3; break;
                default: return -1;
            }

            return value;
        }


        internal static boardinfo AnalyzeHand(String boardCard1, String boardCard2, String boardCard3, String boardCard4, String boardCard5, String holeCard1, String holeCard2)
        {
            boardinfo info = new boardinfo();

            int[] holecard = new int[] { -1, -1 };
            int[] boardcard = new int[] { -1, -1, -1, -1, -1 };

            Hashtable suitOcc = new Hashtable(); //s, h, d, c
            suitOcc.Add('s', 0);
            suitOcc.Add('h', 0);
            suitOcc.Add('d', 0);
            suitOcc.Add('c', 0);

            //if (withHoleCards)
            {
                holecard[0] = Jacob.TranslateCard(holeCard1[0], holeCard1[1]);
                holecard[1] = Jacob.TranslateCard(holeCard2[0], holeCard2[1]);

                suitOcc[holeCard1[1]] = (int)suitOcc[holeCard1[1]] + 1;
                suitOcc[holeCard2[1]] = (int)suitOcc[holeCard2[1]] + 1;
            }
            if (boardCard1 != null)
            {
                boardcard[0] = Jacob.TranslateCard(boardCard1[0], boardCard1[1]);
                boardcard[1] = Jacob.TranslateCard(boardCard2[0], boardCard2[1]);
                boardcard[2] = Jacob.TranslateCard(boardCard3[0], boardCard3[1]);

                suitOcc[boardCard1[1]] = (int)suitOcc[boardCard1[1]] + 1;
                suitOcc[boardCard2[1]] = (int)suitOcc[boardCard2[1]] + 1;
                suitOcc[boardCard3[1]] = (int)suitOcc[boardCard3[1]] + 1;
            }

            if (boardCard4 != null)
            {
                boardcard[3] = Jacob.TranslateCard(boardCard4[0], boardCard4[1]);
                suitOcc[boardCard4[1]] = (int)suitOcc[boardCard4[1]] + 1;
            }

            if (boardCard5 != null)
            {
                boardcard[4] = Jacob.TranslateCard(boardCard5[0], boardCard5[1]);
                suitOcc[boardCard5[1]] = (int)suitOcc[boardCard5[1]] + 1;
            }
            Jacob.AnalyzeBoard(holecard[0], holecard[1], boardcard[0], boardcard[1], boardcard[2], boardcard[3], boardcard[4], info);

            if (info.madehand == PostFlopHand.kNoPair)
            {
                bool flush = false;
                foreach (char suit in suitOcc.Keys)
                {
                    if ((int)suitOcc[suit] == 5)
                    {
                        info.madehand = PostFlopHand.kFlush;
                        break;
                    }
                }
            }

            return info;
        }

        internal static boardinfo AnalyzeHandCustomHoleCards(HandHistory handHistory, int street, bool withHoleCards, String card1, String card2)
        {
            boardinfo info = new boardinfo();

            int[] holecard = new int[] { -1, -1 };
            int[] boardcard = new int[] { -1, -1, -1, -1, -1 };
            Hashtable suitOcc = new Hashtable(); //s, h, d, c
            suitOcc.Add('s', 0);
            suitOcc.Add('h', 0);
            suitOcc.Add('d', 0);
            suitOcc.Add('c', 0);

            if (withHoleCards)
            {
                holecard[0] = Jacob.TranslateCard(card1[0], card1[1]);
                holecard[1] = Jacob.TranslateCard(card2[0], card2[1]);

                suitOcc[card1[1]] = (int)suitOcc[card1[1]] + 1;
                suitOcc[card2[1]] = (int)suitOcc[card2[1]] + 1;
            }
            if (handHistory.CommunityCards[1] != null && handHistory.CommunityCards.Length > 0)
            {
                boardcard[0] = Jacob.TranslateCard(handHistory.CommunityCards[1][0], handHistory.CommunityCards[1][1]);
                boardcard[1] = Jacob.TranslateCard(handHistory.CommunityCards[1][2], handHistory.CommunityCards[1][3]);
                boardcard[2] = Jacob.TranslateCard(handHistory.CommunityCards[1][4], handHistory.CommunityCards[1][5]);

                suitOcc[handHistory.CommunityCards[1][1]] = (int)suitOcc[handHistory.CommunityCards[1][1]] + 1;
                suitOcc[handHistory.CommunityCards[1][3]] = (int)suitOcc[handHistory.CommunityCards[1][3]] + 1;
                suitOcc[handHistory.CommunityCards[1][5]] = (int)suitOcc[handHistory.CommunityCards[1][5]] + 1;
            }

            if (handHistory.CommunityCards[2] != null && handHistory.CommunityCards[2].Length > 0)
            {
                boardcard[3] = Jacob.TranslateCard(handHistory.CommunityCards[2][0], handHistory.CommunityCards[2][1]);

                suitOcc[handHistory.CommunityCards[2][1]] = (int)suitOcc[handHistory.CommunityCards[2][1]] + 1;
            }

            if (handHistory.CommunityCards[3] != null && handHistory.CommunityCards[3].Length > 0)
            {
                boardcard[4] = Jacob.TranslateCard(handHistory.CommunityCards[3][0], handHistory.CommunityCards[3][1]);
                suitOcc[handHistory.CommunityCards[3][1]] = (int)suitOcc[handHistory.CommunityCards[3][1]] + 1;
            }

            if (street == 1) boardcard[3] = boardcard[4] = -1;
            else if (street == 2) boardcard[4] = -1;

            Jacob.AnalyzeBoard(holecard[0], holecard[1], boardcard[0], boardcard[1], boardcard[2], boardcard[3], boardcard[4], info);

            if (info.madehand == PostFlopHand.kNoPair)
            {
                bool flush = false;
                foreach (char suit in suitOcc.Keys)
                {
                    if ((int)suitOcc[suit] == 5)
                    {
                        info.madehand = PostFlopHand.kFlush;
                        break;
                    }
                }
            }

            return info;
        }

        internal static boardinfo AnalyzeHand(HandHistory handHistory, int street, String card1, String card2)
        {
            boardinfo info = new boardinfo();

            int[] holecard = new int[] { -1, -1 };
            int[] boardcard = new int[] { -1, -1, -1, -1, -1 };
            Hashtable suitOcc = new Hashtable(); //s, h, d, c
            suitOcc.Add('s', 0);
            suitOcc.Add('h', 0);
            suitOcc.Add('d', 0);
            suitOcc.Add('c', 0);

            holecard[0] = Jacob.TranslateCard(card1[0], card1[1]);
            holecard[1] = Jacob.TranslateCard(card2[0], card2[1]);

            suitOcc[card1[1]] = (int)suitOcc[card1[1]] + 1;
            suitOcc[card2[1]] = (int)suitOcc[card2[1]] + 1;

            if (handHistory.CommunityCards[1] != null && handHistory.CommunityCards.Length > 0)
            {
                boardcard[0] = Jacob.TranslateCard(handHistory.CommunityCards[1][0], handHistory.CommunityCards[1][1]);
                boardcard[1] = Jacob.TranslateCard(handHistory.CommunityCards[1][2], handHistory.CommunityCards[1][3]);
                boardcard[2] = Jacob.TranslateCard(handHistory.CommunityCards[1][4], handHistory.CommunityCards[1][5]);

                suitOcc[handHistory.CommunityCards[1][1]] = (int)suitOcc[handHistory.CommunityCards[1][1]] + 1;
                suitOcc[handHistory.CommunityCards[1][3]] = (int)suitOcc[handHistory.CommunityCards[1][3]] + 1;
                suitOcc[handHistory.CommunityCards[1][5]] = (int)suitOcc[handHistory.CommunityCards[1][5]] + 1;
            }

            if (handHistory.CommunityCards[2] != null && handHistory.CommunityCards[2].Length > 0)
            {
                boardcard[3] = Jacob.TranslateCard(handHistory.CommunityCards[2][0], handHistory.CommunityCards[2][1]);

                suitOcc[handHistory.CommunityCards[2][1]] = (int)suitOcc[handHistory.CommunityCards[2][1]] + 1;
            }

            if (handHistory.CommunityCards[3] != null && handHistory.CommunityCards[3].Length > 0)
            {
                boardcard[4] = Jacob.TranslateCard(handHistory.CommunityCards[3][0], handHistory.CommunityCards[3][1]);
                suitOcc[handHistory.CommunityCards[3][1]] = (int)suitOcc[handHistory.CommunityCards[3][1]] + 1;
            }

            if (street == 1) boardcard[3] = boardcard[4] = -1;
            else if (street == 2) boardcard[4] = -1;

            Jacob.AnalyzeBoard(holecard[0], holecard[1], boardcard[0], boardcard[1], boardcard[2], boardcard[3], boardcard[4], info);

            if (info.madehand == PostFlopHand.kNoPair)
            {
                bool flush = false;
                foreach (char suit in suitOcc.Keys)
                {
                    if ((int)suitOcc[suit] == 5)
                    {
                        info.madehand = PostFlopHand.kFlush;
                        break;
                    }
                }
            }

            return info;
        }


        internal static boardinfo AnalyzeHand(HandHistory handHistory, int street, bool withHoleCards)
        {
            boardinfo info = new boardinfo();
            Player heroPlayer = handHistory.Players[handHistory.HeroName] as Player;

            int[] holecard = new int[] { -1, -1 };
            int[] boardcard = new int[] { -1, -1, -1, -1, -1 };
            Hashtable suitOcc = new Hashtable(); //s, h, d, c
            suitOcc.Add('s', 0);
            suitOcc.Add('h', 0);
            suitOcc.Add('d', 0);
            suitOcc.Add('c', 0);

            if (withHoleCards)
            {
                holecard[0] = Jacob.TranslateCard(heroPlayer.Cards[0], heroPlayer.Cards[1]);
                holecard[1] = Jacob.TranslateCard(heroPlayer.Cards[2], heroPlayer.Cards[3]);

                suitOcc[heroPlayer.Cards[1]] = (int)suitOcc[heroPlayer.Cards[1]] + 1;
                suitOcc[heroPlayer.Cards[3]] = (int)suitOcc[heroPlayer.Cards[3]] + 1;
            }
            if (handHistory.CommunityCards[1] != null && handHistory.CommunityCards.Length > 0)
            {
                boardcard[0] = Jacob.TranslateCard(handHistory.CommunityCards[1][0], handHistory.CommunityCards[1][1]);
                boardcard[1] = Jacob.TranslateCard(handHistory.CommunityCards[1][2], handHistory.CommunityCards[1][3]);
                boardcard[2] = Jacob.TranslateCard(handHistory.CommunityCards[1][4], handHistory.CommunityCards[1][5]);

                suitOcc[handHistory.CommunityCards[1][1]] = (int)suitOcc[handHistory.CommunityCards[1][1]] + 1;
                suitOcc[handHistory.CommunityCards[1][3]] = (int)suitOcc[handHistory.CommunityCards[1][3]] + 1;
                suitOcc[handHistory.CommunityCards[1][5]] = (int)suitOcc[handHistory.CommunityCards[1][5]] + 1;
            }

            if (handHistory.CommunityCards[2] != null && handHistory.CommunityCards[2].Length > 0)
            {
                boardcard[3] = Jacob.TranslateCard(handHistory.CommunityCards[2][0], handHistory.CommunityCards[2][1]);

                suitOcc[handHistory.CommunityCards[2][1]] = (int)suitOcc[handHistory.CommunityCards[2][1]] + 1;
            }

            if (handHistory.CommunityCards[3] != null && handHistory.CommunityCards[3].Length > 0)
            {
                boardcard[4] = Jacob.TranslateCard(handHistory.CommunityCards[3][0], handHistory.CommunityCards[3][1]);
                suitOcc[handHistory.CommunityCards[3][1]] = (int)suitOcc[handHistory.CommunityCards[3][1]] + 1;
            }

            if (street == 1) boardcard[3] = boardcard[4] = -1;
            else if (street == 2) boardcard[4] = -1;

            Jacob.AnalyzeBoard(holecard[0], holecard[1], boardcard[0], boardcard[1], boardcard[2], boardcard[3], boardcard[4], info);

            if (info.madehand == PostFlopHand.kNoPair)
            {
                bool flush = false;
                foreach (char suit in suitOcc.Keys)
                {
                    if ((int)suitOcc[suit] == 5)
                    {
                        info.madehand = PostFlopHand.kFlush;
                        break;
                    }
                }
            }

            return info;
        }
        internal static int AnalyzeBoard(int holecard1, int holecard2, int boardcard1, int boardcard2, int boardcard3, int boardcard4,
                    int boardcard5, boardinfo info)
        {
            lowlevel low = new lowlevel();


            int weight = IterateCombinations(holecard1, holecard2, boardcard1, boardcard2, boardcard3, boardcard4, boardcard5, ref low);

            info.weight = weight;
            info.holesused = 0;
            if (low.ifholeoneused)
                info.holesused++;
            if (low.ifholetwoused)
                info.holesused++;

            info.type = low.type;
            info.size = low.size;
            info.value1 = low.value1;
            info.value2 = low.value2;

            switch (weight / 100)
            {
                case 10:
                case 9:
                    info.madehand = PostFlopHand.kStraightFlush;
                    break;
                case 8:
                    info.madehand = PostFlopHand.k4ofKind;
                    break;
                case 7:
                    info.madehand = PostFlopHand.kFullHouse;
                    break;
                case 6:
                    info.madehand = PostFlopHand.kFlush;
                    if (info.holesused > 0)
                    {
                        int maxvalue = 0;
                        int flushsuit = 0;

                        if (info.holesused == 2)
                            if (holecard1 % 100 > holecard2 % 100)
                                maxvalue = holecard1 % 100;
                            else
                                maxvalue = holecard2 % 100;
                        else if (low.ifholeoneused)
                            maxvalue = holecard1 % 100;
                        else if (low.ifholetwoused)
                            maxvalue = holecard2 % 100;

                        if (low.ifholeoneused)
                            flushsuit = holecard1 / 100;
                        else if (low.ifholetwoused)
                            flushsuit = holecard2 / 100;

                        info.flushvalue = 14 - maxvalue;
                        if ((boardcard1 / 100 == flushsuit) && (boardcard1 % 100 > maxvalue))
                            info.flushvalue--;
                        if ((boardcard2 / 100 == flushsuit) && (boardcard2 % 100 > maxvalue))
                            info.flushvalue--;
                        if ((boardcard3 / 100 == flushsuit) && (boardcard3 % 100 > maxvalue))
                            info.flushvalue--;
                        if ((boardcard4 / 100 == flushsuit) && (boardcard4 % 100 > maxvalue))
                            info.flushvalue--;
                        if ((boardcard5 / 100 == flushsuit) && (boardcard5 % 100 > maxvalue))
                            info.flushvalue--;
                    }

                    break;
                case 5:
                    info.madehand = PostFlopHand.kStraight;
                    break;
                case 4:
                    info.madehand = PostFlopHand.k3ofKind;
                    break;
                case 3:
                    info.madehand = PostFlopHand.k2Pair;
                    break;
                case 2:
                    info.madehand = PostFlopHand.kPair;
                    break;
                case 1:
                    info.madehand = PostFlopHand.kNoPair;
                    break;
            }

            if (info.holesused == 0)
                info.type = -1;

            if (info.holesused == 0)
                if (holecard1 % 100 > holecard2 % 100)
                    info.kicker = holecard1 % 100;
                else
                    info.kicker = holecard2 % 100;
            else if (!low.ifholeoneused)
                info.kicker = holecard1 % 100;
            else if (!low.ifholetwoused)
                info.kicker = holecard2 % 100;

            info.boardhigher1 = 0;
            if (boardcard1 % 100 > holecard1 % 100)
                info.boardhigher1++;
            if (boardcard2 % 100 > holecard1 % 100)
                info.boardhigher1++;
            if (boardcard3 % 100 > holecard1 % 100)
                info.boardhigher1++;
            if (boardcard4 % 100 > holecard1 % 100)
                info.boardhigher1++;
            if (boardcard5 % 100 > holecard1 % 100)
                info.boardhigher1++;

            info.boardhigher2 = 0;
            if (boardcard1 % 100 > holecard2 % 100)
                info.boardhigher2++;
            if (boardcard2 % 100 > holecard2 % 100)
                info.boardhigher2++;
            if (boardcard3 % 100 > holecard2 % 100)
                info.boardhigher2++;
            if (boardcard4 % 100 > holecard2 % 100)
                info.boardhigher2++;
            if (boardcard5 % 100 > holecard2 % 100)
                info.boardhigher2++;

            info.straighthighest = 0;
            if (low.ifholeoneused && (holecard1 % 100 == low.value1))
                info.straighthighest = 1;
            if (low.ifholetwoused && (holecard2 % 100 == low.value1))
                info.straighthighest = 1;
            if ((info.straighthighest == 1) && low.ifholeoneused && (holecard1 % 100 == low.value1 - 1))
                info.straighthighest = 2;
            if ((info.straighthighest == 1) && low.ifholetwoused && (holecard2 % 100 == low.value1 - 1))
                info.straighthighest = 2;

            info.straightlowest = 0;
            if (low.ifholeoneused && ((holecard1 % 100 == low.value1 - 4) || ((low.value1 == 5) && (holecard1 % 100 == 14))))
                info.straightlowest = 1;
            if (low.ifholetwoused && ((holecard2 % 100 == low.value1 - 4) || ((low.value1 == 5) && (holecard2 % 100 == 14))))
                info.straightlowest = 1;
            if ((info.straightlowest == 1) && low.ifholeoneused && (holecard1 % 100 == low.value1 - 3))
                info.straightlowest = 2;
            if ((info.straightlowest == 1) && low.ifholetwoused && (holecard2 % 100 == low.value1 - 3))
                info.straightlowest = 2;

            info.ifstraightdraw = false;
            info.ifflushdraw = false;
            info.ifstraightflushdraw = false;
            info.drawtype = 0;

            if (boardcard5 == -1)
            {
                CheckDraw(boardcard1, boardcard2, boardcard3, holecard1, 1, (boardcard4 == -1), info);

                if (boardcard4 != -1)
                {
                    CheckDraw(boardcard1, boardcard2, boardcard4, holecard1, 1, false, info);

                    CheckDraw(boardcard1, boardcard3, boardcard4, holecard1, 1, false, info);

                    CheckDraw(boardcard2, boardcard3, boardcard4, holecard1, 1, false, info);
                }

                CheckDraw(boardcard1, boardcard2, boardcard3, holecard2, 1, (boardcard4 == -1), info);

                if (boardcard4 != -1)
                {
                    CheckDraw(boardcard1, boardcard2, boardcard4, holecard2, 1, false, info);

                    CheckDraw(boardcard1, boardcard3, boardcard4, holecard2, 1, false, info);

                    CheckDraw(boardcard2, boardcard3, boardcard4, holecard2, 1, false, info);
                }

                CheckDraw(boardcard1, boardcard2, holecard1, holecard2, 2, (boardcard4 == -1), info);

                CheckDraw(boardcard1, boardcard3, holecard1, holecard2, 2, (boardcard4 == -1), info);

                CheckDraw(boardcard2, boardcard3, holecard1, holecard2, 2, (boardcard4 == -1), info);

                if (boardcard4 != -1)
                {
                    CheckDraw(boardcard1, boardcard4, holecard1, holecard2, 2, false, info);

                    CheckDraw(boardcard2, boardcard4, holecard1, holecard2, 2, false, info);

                    CheckDraw(boardcard3, boardcard4, holecard1, holecard2, 2, false, info);
                }

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard1, boardcard2, boardcard3, boardcard4, boardcard5, 0, info);

                if (boardcard4 != -1)
                    CheckDoubleBellyBuster(boardcard1, boardcard2, boardcard3, boardcard4, holecard1, 1, info);

                //if (boardcard5 != -1)
                //	CheckDoubleBellyBuster (boardcard1, boardcard2, boardcard3, boardcard5, holecard1, 1, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard1, boardcard2, boardcard4, boardcard5, holecard1, 1, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard1, boardcard3, boardcard4, boardcard5, holecard1, 1, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard2, boardcard3, boardcard4, boardcard5, holecard1, 1, info);

                if (boardcard4 != -1)
                    CheckDoubleBellyBuster(boardcard1, boardcard2, boardcard3, boardcard4, holecard2, 1, info);

                //if (boardcard5 != -1)
                //	CheckDoubleBellyBuster (boardcard1, boardcard2, boardcard3, boardcard5, holecard2, 1, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard1, boardcard2, boardcard4, boardcard5, holecard2, 1, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard1, boardcard3, boardcard4, boardcard5, holecard2, 1, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard2, boardcard3, boardcard4, boardcard5, holecard2, 1, info);

                CheckDoubleBellyBuster(boardcard1, boardcard2, boardcard3, holecard1, holecard2, 2, info);

                if (boardcard4 != -1)
                    CheckDoubleBellyBuster(boardcard1, boardcard2, boardcard4, holecard1, holecard2, 2, info);

                if (boardcard4 != -1)
                    CheckDoubleBellyBuster(boardcard1, boardcard3, boardcard4, holecard1, holecard2, 2, info);

                if (boardcard4 != -1)
                    CheckDoubleBellyBuster(boardcard2, boardcard3, boardcard4, holecard1, holecard2, 2, info);

                //if (boardcard5 != -1)
                //	CheckDoubleBellyBuster (boardcard1, boardcard2, boardcard5, holecard1, holecard2, 2, info);

                //if (boardcard5 != -1)
                //	CheckDoubleBellyBuster (boardcard1, boardcard3, boardcard5, holecard1, holecard2, 2, info);

                //if (boardcard5 != -1)
                //	CheckDoubleBellyBuster (boardcard2, boardcard3, boardcard5, holecard1, holecard2, 2, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard1, boardcard4, boardcard5, holecard1, holecard2, 2, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard2, boardcard4, boardcard5, holecard1, holecard2, 2, info);

                //if ((boardcard4 != -1) && (boardcard5 != -1))
                //	CheckDoubleBellyBuster (boardcard3, boardcard4, boardcard5, holecard1, holecard2, 2, info);
            }

            AnalyzeDangers(boardcard1, boardcard2, boardcard3, boardcard4, boardcard5, info);
            return weight;
        }



        internal static int IterateCombinations(int holecard1, int holecard2, int boardcard1, int boardcard2, int boardcard3, int boardcard4,
                                 int boardcard5, ref lowlevel bestlow)
        {
            /*
            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard3, boardcard4, boardcard5, 0, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard4 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard3, boardcard4, holecard1, 1, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard5 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard3, boardcard5, holecard1, 1, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard4, boardcard5, holecard1, 1, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard1, boardcard3, boardcard4, boardcard5, holecard1, 1, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard2, boardcard3, boardcard4, boardcard5, holecard1, 1, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard4 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard3, boardcard4, holecard2, 2, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard5 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard3, boardcard5, holecard2, 2, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard4, boardcard5, holecard2, 2, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard1, boardcard3, boardcard4, boardcard5, holecard2, 2, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard2, boardcard3, boardcard4, boardcard5, holecard2, 2, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            weight = GetCombinationWeight(boardcard1, boardcard2, boardcard3, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard4 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard4, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard5 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard2, boardcard5, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard4 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard3, boardcard4, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if (boardcard5 != -1)
                weight = GetCombinationWeight(boardcard1, boardcard3, boardcard5, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard1, boardcard4, boardcard5, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard2, boardcard4, boardcard5, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }

            if ((boardcard4 != -1) && (boardcard5 != -1))
                weight = GetCombinationWeight(boardcard3, boardcard4, boardcard5, holecard1, holecard2, 3, low);
            if (bestweight < weight)
            {
                bestweight = weight;
                bestlow = low;
            }
            */
            int bestweight = 0;
            int weight = 0;

            lowlevel low = new lowlevel();

            {
                List<String> allCards = new List<String>();
                allCards.Add(boardcard1.ToString());
                allCards.Add(boardcard2.ToString());
                allCards.Add(boardcard3.ToString());
                if (boardcard4 != -1)
                    allCards.Add(boardcard4.ToString());
                if (boardcard5 != -1)
                {
                    allCards.Add(boardcard5.ToString());
                }
                allCards.Add("h1" + holecard1.ToString());
                allCards.Add("h2" + holecard2.ToString());

                String[] sAllCards = allCards.ToArray();
                Combination c = new Combination(allCards.Count, 5); // 52 cards, 5 at a time
                string[] cardsComb = new string[5];

                //if (allCards.Count > 5)
                {
                    while (c != null)
                    {
                        cardsComb = c.ApplyTo(sAllCards);
                        c = c.Successor();

                        int holecards = 0;
                        for (int i = 0; i < cardsComb.Length; i++)
                        {
                            if (cardsComb[i].StartsWith("h1"))
                            {
                                if (holecards > 0)
                                    holecards = 3;
                                else holecards = 1;
                            }
                            else if (cardsComb[i].StartsWith("h2"))
                            {
                                if (holecards > 0)
                                    holecards = 3;
                                else holecards = 2;
                            }
                            cardsComb[i] = cardsComb[i].Replace("h1", "").Replace("h2", "");
                        }

                        weight = GetCombinationWeight(int.Parse(cardsComb[0]), int.Parse(cardsComb[1]), int.Parse(cardsComb[2]), int.Parse(cardsComb[3]), int.Parse(cardsComb[4]), holecards, low);
                        if (weight == 512)
                        {
                        }
                        if (bestweight < weight)
                        {
                            bestweight = weight;
                            bestlow = new lowlevel();
                            bestlow.ifholeoneused = low.ifholeoneused;
                            bestlow.ifholetwoused = low.ifholetwoused;
                            bestlow.size = low.size;
                            bestlow.type = low.type;
                            bestlow.value1 = low.value1;
                            bestlow.value2 = low.value2;
                        }
                    }
                }
            }

            return bestweight;
        }



        internal static void AnalyzeDangers(int card1, int card2, int card3, int card4, int card5, boardinfo info)
        {
            int[] cardsuits = new int[5];
            int[] cardvalues = new int[15];
            int i;

            int maxsamesuits = 0;

            for (i = 0; i <= 14; i++)
                cardvalues[i] = 0;

            if (card1 > 0)
                cardvalues[card1 % 100]++;
            if (card2 > 0)
                cardvalues[card2 % 100]++;
            if (card3 > 0)
                cardvalues[card3 % 100]++;
            if (card4 > 0)
                cardvalues[card4 % 100]++;
            if (card5 > 0)
                cardvalues[card5 % 100]++;

            cardvalues[1] = cardvalues[14];

            for (i = 0; i <= 4; i++)
                cardsuits[i] = 0;

            if (card1 > 0)
                cardsuits[card1 / 100]++;
            if (card2 > 0)
                cardsuits[card2 / 100]++;
            if (card3 > 0)
                cardsuits[card3 / 100]++;
            if (card4 > 0)
                cardsuits[card4 / 100]++;
            if (card5 > 0)
                cardsuits[card5 / 100]++;

            info.samesuitcards = 0;
            info.samesuitpairs = 0;
            info.samevaluecards = 0;
            info.samevaluepairs = 0;
            info.ifmadestraights = false;
            info.ifstraightdraws = false;
            info.broadwaycards = 0;

            for (i = 2; i <= 14; i++)
            {
                if (info.samevaluecards <= cardvalues[i])
                    info.samevaluecards = cardvalues[i];

                if (cardvalues[i] == 2)
                    info.samevaluepairs++;

                if ((i >= 2) && (cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (cardvalues[i - 2] == 0) &&
                        ((i == 14) || (cardvalues[i + 1] == 0)))
                    info.similarvaluepairs++;

                if ((i >= 5) && (cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (cardvalues[i - 2] > 0) && (cardvalues[i - 3] > 0) &&
                        (cardvalues[i - 4] > 0) && (info.similarvaluecards < 5))
                    info.similarvaluecards = 5;

                if ((i >= 4) && (cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (cardvalues[i - 2] > 0) && (cardvalues[i - 3] > 0) &&
                        (info.similarvaluecards < 4))
                    info.similarvaluecards = 4;

                if ((i >= 3) && (cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (cardvalues[i - 2] > 0) && (info.similarvaluecards < 3))
                    info.similarvaluecards = 3;

                if ((i >= 2) && (cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (info.similarvaluecards < 2))
                    info.similarvaluecards = 2;

                // Tepon lisays (broadwaycards A-T)
                if (cardvalues[i] > 0 && i >= 10) info.broadwaycards++;

                if (i >= 5)
                {
                    int numstraightcards = 0;

                    if (cardvalues[i] > 0)
                        numstraightcards++;
                    if (cardvalues[i - 1] > 0)
                        numstraightcards++;
                    if (cardvalues[i - 2] > 0)
                        numstraightcards++;
                    if (cardvalues[i - 3] > 0)
                        numstraightcards++;
                    if (cardvalues[i - 4] > 0)
                        numstraightcards++;

                    if (numstraightcards >= 3)
                        info.ifmadestraights = true;
                    if (numstraightcards >= 2)
                        info.ifstraightdraws = true;
                }
            }

            for (i = 1; i <= 4; i++)
            {
                if (info.samesuitcards < cardsuits[i])
                    info.samesuitcards = cardsuits[i];

                if (cardsuits[i] == 2)
                    info.samesuitpairs++;
            }
        }

        internal static int GetCombinationWeight(int card1, int card2, int card3, int card4, int card5, int holecards, lowlevel low)
        {
            int[] cardsuits = new int[5];
            int[] cardvalues = new int[15];
            int i;

            int comb;

            int board11;
            int board12;
            int board13;
            int positive1;
            int negative1;

            int board21;
            int board22;
            int board23;
            int positive2;
            int negative2;

            //memset (low, 0, sizeof (low));

            for (i = 0; i <= 14; i++)
                cardvalues[i] = 0;

            if (card1 > 0)
                cardvalues[card1 % 100]++;
            if (card2 > 0)
                cardvalues[card2 % 100]++;
            if (card3 > 0)
                cardvalues[card3 % 100]++;
            if (card4 > 0)
                cardvalues[card4 % 100]++;
            if (card5 > 0)
                cardvalues[card5 % 100]++;

            cardvalues[1] = cardvalues[14];

            for (i = 0; i <= 4; i++)
                cardsuits[i] = 0;

            if (card1 > 0)
                cardsuits[card1 / 100]++;
            if (card2 > 0)
                cardsuits[card2 / 100]++;
            if (card3 > 0)
                cardsuits[card3 / 100]++;
            if (card4 > 0)
                cardsuits[card4 / 100]++;
            if (card5 > 0)
                cardsuits[card5 / 100]++;

            int maxsamevalues = 0;
            int numpairs = 0;
            int maxsamesuits = 0;
            int maxcardvalue = 0;
            int maxsamevalue = 0;
            int straightmaxvalue = 0;

            for (i = 2; i <= 14; i++)
            {
                if (maxsamevalues <= cardvalues[i])
                {
                    maxsamevalues = cardvalues[i];
                    maxsamevalue = i;
                }

                if (cardvalues[i] == 2)
                    numpairs++;

                if (cardvalues[i] > 0)
                    maxcardvalue = i;

                if ((i >= 5) && (cardvalues[i] == 1) && (cardvalues[i - 1] == 1) && (cardvalues[i - 2] == 1) &&
                        (cardvalues[i - 3] == 1) && (cardvalues[i - 4] == 1))
                    straightmaxvalue = i;
            }

            for (i = 1; i <= 4; i++)
                if (maxsamesuits < cardsuits[i])
                    maxsamesuits = cardsuits[i];

            if ((straightmaxvalue == 14) && (maxsamesuits == 5))
            {
                low.ifholeoneused = (holecards & 1) == 1;//ATT
                low.ifholetwoused = (holecards & 2) == 2;//ATT
                low.value1 = maxcardvalue;
                comb = 10;
            }
            else if ((maxsamesuits == 5) && (straightmaxvalue >= 4))
            {
                low.ifholeoneused = (holecards & 1) == 1;//ATT
                low.ifholetwoused = (holecards & 2) == 2;//ATT
                low.value1 = straightmaxvalue;
                comb = 9;
            }
            else if (maxsamevalues == 4)
            {
                low.value1 = maxsamevalue;

                if ((holecards == 3) && (card4 % 100 == card5 % 100))
                {
                    low.ifholeoneused = true;
                    low.ifholetwoused = true;
                }
                else if (((holecards == 3) && ((card4 % 100 == low.value1) || (card5 % 100 == low.value1))) ||
                    (((holecards == 1) || (holecards == 2)) && (card5 % 100 == low.value1)))
                {
                    if (holecards == 3)
                    {
                        low.ifholeoneused = (card4 % 100 == low.value1);
                        low.ifholetwoused = (card5 % 100 == low.value1);
                    }
                    else
                    {
                        low.ifholeoneused = (holecards & 1) == 1;//ATT
                        low.ifholetwoused = (holecards & 2) == 2;//ATT
                    }
                }
                else
                {
                    low.type = -1;
                    low.ifholeoneused = false;
                    low.ifholetwoused = false;
                }

                comb = 8;
            }
            else if ((maxsamevalues == 3) && (numpairs > 0))
            {
                low.ifholeoneused = (holecards & 1) == 1;//ATT
                low.ifholetwoused = (holecards & 2) == 2;//ATT
                low.value1 = maxsamevalue;

                for (i = 2; i <= 14; i++)
                    if (cardvalues[i] == 2)
                    {
                        low.value2 = i;
                        break;
                    }

                if (holecards == 3)
                    if (card4 % 100 == card5 % 100)
                        if (card4 % 100 == low.value1)
                            low.type = 2;
                        else
                            low.type = 3;
                    else
                        low.type = 1;

                comb = 7;
            }
            else if (maxsamesuits == 5)
            {
                low.ifholeoneused = (holecards & 1) == 1;//ATT
                low.ifholetwoused = (holecards & 2) == 2;//ATT
                low.value1 = maxcardvalue;
                comb = 6;
            }
            else if (straightmaxvalue >= 4)
            {
                low.ifholeoneused = (holecards & 1) == 1;
                low.ifholetwoused = (holecards & 2) == 2;
                low.value1 = straightmaxvalue;
                comb = 5;
            }
            else if (maxsamevalues == 3)
            {
                low.value1 = maxsamevalue;

                if ((holecards == 3) && (card4 % 100 == card5 % 100))
                {
                    low.type = 1;
                    low.ifholeoneused = true;
                    low.ifholetwoused = true;
                }
                else if (((holecards == 3) && ((card4 % 100 == low.value1) || (card5 % 100 == low.value1))) ||
                    (((holecards == 1) || (holecards == 2)) && (card5 % 100 == low.value1)))
                {
                    low.type = 2;
                    if (holecards == 3)
                    {
                        low.ifholeoneused = (card4 % 100 == low.value1);
                        low.ifholetwoused = (card5 % 100 == low.value1);
                    }
                    else
                    {
                        low.ifholeoneused = (holecards & 1) == 1;
                        low.ifholetwoused = (holecards & 2) == 2;
                    }
                }
                else
                {
                    low.type = -1;
                    low.ifholeoneused = false;
                    low.ifholetwoused = false;
                }

                comb = 4;
            }
            else if (numpairs > 1)
            {
                low.value1 = maxsamevalue;

                for (i = 2; i <= 14; i++)
                    if ((i != maxsamevalue) && (cardvalues[i] == 2))
                    {
                        low.value2 = i;
                        break;
                    }

                if ((holecards == 3) && (card4 % 100 == card5 % 100))
                {
                    low.type = 3;
                    low.ifholeoneused = true;
                    low.ifholetwoused = true;
                }
                else if ((holecards == 3) && (((card4 % 100 == low.value1) && (card5 % 100 == low.value2)) ||
                    ((card5 % 100 == low.value1) && (card4 % 100 == low.value2))))
                {
                    low.type = 1;
                    low.ifholeoneused = true;
                    low.ifholetwoused = true;
                }
                else if (((holecards == 3) && ((card4 % 100 == low.value1) || (card5 % 100 == low.value2) ||
                    (card5 % 100 == low.value1) || (card4 % 100 == low.value2))) ||
                    (((holecards == 1) || (holecards == 2)) && ((card5 % 100 == low.value1) || (card5 % 100 == low.value2))))
                {
                    low.type = 2;
                    if (holecards == 3)
                    {
                        low.ifholeoneused = (card4 % 100 == low.value1) || (card4 % 100 == low.value2);
                        low.ifholetwoused = (card5 % 100 == low.value1) || (card5 % 100 == low.value2);
                    }
                    else
                    {
                        low.ifholeoneused = (holecards & 1) == 1;
                        low.ifholetwoused = (holecards & 2) == 2;
                    }
                }
                else
                {
                    low.type = -1;
                    low.ifholeoneused = false;
                    low.ifholetwoused = false;
                }

                comb = 3;
            }
            else if (numpairs > 0)
            {
                low.value1 = maxsamevalue;

                if ((holecards == 3) && (card4 % 100 == card5 % 100))
                {
                    low.type = 1;
                    low.ifholeoneused = true;
                    low.ifholetwoused = true;
                }
                else if (((holecards == 3) && ((card4 % 100 == low.value1) || (card5 % 100 == low.value1))) ||
                    (((holecards == 1) || (holecards == 2)) && (card5 % 100 == low.value1)))
                {
                    low.type = 2;
                    if (holecards == 3)
                    {
                        low.ifholeoneused = (card4 % 100 == low.value1);
                        low.ifholetwoused = (card5 % 100 == low.value1);
                    }
                    else
                    {
                        low.ifholeoneused = (holecards & 1) == 1;
                        low.ifholetwoused = (holecards & 2) == 2;
                    }
                }
                else
                {
                    low.type = -1;
                    low.ifholeoneused = false;
                    low.ifholetwoused = false;
                }

                comb = 2;
            }
            else
            {
                low.ifholeoneused = false;
                low.ifholetwoused = false;
                low.value1 = maxcardvalue;
                comb = 1;
            }

            board11 = card1 % 100 - low.value1;
            board12 = card2 % 100 - low.value1;
            board13 = card3 % 100 - low.value1;

            positive1 = 0;
            negative1 = 0;
            if (board11 > 0)
                positive1++;
            if (board11 < 0)
                negative1++;
            if (board12 > 0)
                positive1++;
            if (board12 < 0)
                negative1++;
            if (board13 > 0)
                positive1++;
            if (board13 < 0)
                negative1++;

            board21 = card1 % 100 - low.value2;
            board22 = card2 % 100 - low.value2;
            board23 = card3 % 100 - low.value2;

            positive2 = 0;
            negative2 = 0;
            if (board21 > 0)
                positive2++;
            if (board21 < 0)
                negative2++;
            if (board22 > 0)
                positive2++;
            if (board22 < 0)
                negative2++;
            if (board23 > 0)
                positive2++;
            if (board23 < 0)
                negative2++;

            if (holecards == 3)
                switch (comb)
                {
                    case 4:
                        switch (low.type)
                        {
                            case 2:
                                if (positive1 - negative1 > 0)
                                    low.size = 2;
                                else
                                    low.size = 1;

                                break;
                            case 1:
                                if (positive1 - negative1 > 0)
                                    low.size = 3;
                                else if (positive1 - negative1 < 0)
                                    low.size = 1;
                                else
                                    low.size = 2;

                                break;
                        }

                        break;
                    case 3:
                        switch (low.type)
                        {
                            case 3:
                                if ((card4 % 100 >= low.value1) && (card4 % 100 >= low.value2))
                                    low.size = 1;
                                else
                                    low.size = 2;

                                break;
                            case 2:
                                if (positive1 - negative1 + positive2 - negative2 > 0)
                                    low.size = 2;
                                else
                                    low.size = 1;

                                break;
                            case 1:
                                if (positive1 - negative1 + positive2 - negative2 > 0)
                                    low.size = 3;
                                else if (positive1 - negative1 + positive2 - negative2 < 0)
                                    low.size = 1;
                                else
                                    low.size = 2;

                                break;
                        }

                        break;
                    case 2:
                        switch (low.type)
                        {
                            case 2:
                                if (positive1 - negative1 > 0)
                                    low.size = 3;
                                else if (positive1 - negative1 < 0)
                                    low.size = 1;
                                else
                                    low.size = 2;

                                break;
                            case 1:
                                if (positive1 - negative1 == 3)
                                    low.size = 4;
                                else if (positive1 - negative1 == 1)
                                    low.size = 3;
                                else if (positive1 - negative1 == -1)
                                    low.size = 2;
                                else if (positive1 - negative1 == -3)
                                    low.size = 1;
                                else
                                    low.size = -1;

                                break;
                        }

                        break;
                }

            return comb * 100 + low.value1;
        }

        internal static void CheckDraw(int card1, int card2, int card3, int card4, int numholes, bool ifthreeboards, boardinfo info)
        {
            int[] cardsuits = new int[5];
            int[] cardvalues = new int[15];
            int i;

            int maxsamesuits = 0;
            int maxsamesuit = 0;

            for (i = 0; i <= 14; i++)
                cardvalues[i] = 0;

            if (card1 > 0)
                cardvalues[card1 % 100]++;
            if (card2 > 0)
                cardvalues[card2 % 100]++;
            if (card3 > 0)
                cardvalues[card3 % 100]++;
            if (card4 > 0)
                cardvalues[card4 % 100]++;

            cardvalues[1] = cardvalues[14];

            for (i = 0; i <= 4; i++)
                cardsuits[i] = 0;

            if (card1 > 0)
                cardsuits[card1 / 100]++;
            if (card2 > 0)
                cardsuits[card2 / 100]++;
            if (card3 > 0)
                cardsuits[card3 / 100]++;
            if (card4 > 0)
                cardsuits[card4 / 100]++;

            for (i = 1; i <= 4; i++)
                if (maxsamesuits < cardsuits[i])
                {
                    maxsamesuit = i;
                    maxsamesuits = cardsuits[i];
                }

            bool processstraight = !info.ifstraightdraw;
            int newdrawtype = 10;
            int sthighest = 0;

            for (i = 4; i <= 14; i++)
                if ((cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (cardvalues[i - 2] > 0) && (cardvalues[i - 3] > 0))
                {
                    info.ifstraightdraw = true;

                    if ((i == 4) || (i == 14))
                        newdrawtype = 2;
                    else
                        newdrawtype = 1;

                    sthighest = i;

                    if (maxsamesuits == 4)
                    {
                        int numsamesuit = 0;
                        if ((card1 % 100 <= i) && (card1 % 100 >= i - 3) && (card1 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card2 % 100 <= i) && (card2 % 100 >= i - 3) && (card2 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card3 % 100 <= i) && (card3 % 100 >= i - 3) && (card3 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card4 % 100 <= i) && (card4 % 100 >= i - 3) && (card4 / 100 == maxsamesuit))
                            numsamesuit++;

                        if (numsamesuit == 4)
                            info.ifstraightflushdraw = true;
                    }
                }
                else if ((i >= 5) && (cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (cardvalues[i - 2] > 0) && (cardvalues[i - 4] > 0))
                {
                    info.ifstraightdraw = true;

                    newdrawtype = 2;

                    sthighest = i;

                    if (maxsamesuits == 4)
                    {
                        int numsamesuit = 0;
                        if ((card1 % 100 <= i) && (card1 % 100 >= i - 4) && (card1 % 100 != i - 3) && (card1 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card2 % 100 <= i) && (card2 % 100 >= i - 4) && (card2 % 100 != i - 3) && (card2 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card3 % 100 <= i) && (card3 % 100 >= i - 4) && (card3 % 100 != i - 3) && (card3 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card4 % 100 <= i) && (card4 % 100 >= i - 4) && (card4 % 100 != i - 3) && (card4 / 100 == maxsamesuit))
                            numsamesuit++;

                        if (numsamesuit == 4)
                            info.ifstraightflushdraw = true;
                    }
                }
                else if ((i >= 5) && (cardvalues[i] > 0) && (cardvalues[i - 2] > 0) && (cardvalues[i - 3] > 0) && (cardvalues[i - 4] > 0))
                {
                    info.ifstraightdraw = true;

                    newdrawtype = 2;

                    sthighest = i;

                    if (maxsamesuits == 4)
                    {
                        int numsamesuit = 0;
                        if ((card1 % 100 <= i) && (card1 % 100 >= i - 4) && (card1 % 100 != i - 1) && (card1 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card2 % 100 <= i) && (card2 % 100 >= i - 4) && (card2 % 100 != i - 1) && (card2 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card3 % 100 <= i) && (card3 % 100 >= i - 4) && (card3 % 100 != i - 1) && (card3 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card4 % 100 <= i) && (card4 % 100 >= i - 4) && (card4 % 100 != i - 1) && (card4 / 100 == maxsamesuit))
                            numsamesuit++;

                        if (numsamesuit == 4)
                            info.ifstraightflushdraw = true;
                    }
                }
                // Tepon lisays
                else if ((i >= 5) && (cardvalues[i] > 0) && (cardvalues[i - 1] > 0) && (cardvalues[i - 3] > 0) && (cardvalues[i - 4] > 0))
                {
                    info.ifstraightdraw = true;

                    newdrawtype = 2;

                    sthighest = i;

                    if (maxsamesuits == 4)
                    {
                        int numsamesuit = 0;
                        if ((card1 % 100 <= i) && (card1 % 100 >= i - 4) && (card1 % 100 != i - 2) && (card1 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card2 % 100 <= i) && (card2 % 100 >= i - 4) && (card2 % 100 != i - 2) && (card2 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card3 % 100 <= i) && (card3 % 100 >= i - 4) && (card3 % 100 != i - 2) && (card3 / 100 == maxsamesuit))
                            numsamesuit++;
                        if ((card4 % 100 <= i) && (card4 % 100 >= i - 4) && (card4 % 100 != i - 2) && (card4 / 100 == maxsamesuit))
                            numsamesuit++;

                        if (numsamesuit == 4)
                            info.ifstraightflushdraw = true;
                    }
                }

            if (info.ifstraightdraw && ((info.drawtype == 10) || (info.drawtype == 0) ||
                ((info.drawtype == 2) && ((newdrawtype == 3) || (newdrawtype == 1)))))
            {
                info.drawtype = newdrawtype;
                processstraight = true;
            }

            if (info.ifstraightdraw && processstraight)
            {
                info.drawstraightholesused = numholes;

                if ((sthighest < 7) && (card1 % 100 == 14))
                    card1 = (card1 / 100) * 100 + 1;
                if ((sthighest < 7) && (card2 % 100 == 14))
                    card2 = (card2 / 100) * 100 + 1;
                if ((sthighest < 7) && (card3 % 100 == 14))
                    card3 = (card3 / 100) * 100 + 1;
                if ((sthighest < 7) && (card4 % 100 == 14))
                    card4 = (card4 / 100) * 100 + 1;

                if (numholes == 2)
                {
                    info.drawstraighthighest = 0;

                    if ((card3 % 100 > card1 % 100) && (card3 % 100 > card2 % 100))
                        info.drawstraighthighest++;
                    if ((card4 % 100 > card1 % 100) && (card4 % 100 > card2 % 100))
                        info.drawstraighthighest++;
                }
                else
                    if ((card4 % 100 > card1 % 100) && (card4 % 100 > card2 % 100) && (card4 % 100 > card3 % 100))
                    info.drawstraighthighest = 1;
                else
                    info.drawstraighthighest = 0;
            }

            for (i = 1; i <= 4; i++)
                if ((cardsuits[i] == 4) || (ifthreeboards && (cardsuits[i] == 3) && (info.drawflushcardsmissing != 1)))
                {
                    info.drawhighest = 0;

                    if ((card4 / 100 == i) && (info.drawhighest < card4 % 100))
                        info.drawhighest = card4 % 100;
                    if ((numholes == 2) && (card3 / 100 == i) && (info.drawhighest < card3 % 100))
                        info.drawhighest = card3 % 100;

                    if (info.drawhighest > 0)
                    {
                        info.ifflushdraw = true;

                        info.drawflushholesused = 0;

                        if (card4 / 100 == i)
                            info.drawflushholesused++;
                        if ((numholes == 2) && (card3 / 100 == i))
                            info.drawflushholesused++;

                        if (cardsuits[i] == 4)
                            info.drawflushcardsmissing = 1;
                        else
                            info.drawflushcardsmissing = 2;
                    }
                }
        }



        internal static void CheckDoubleBellyBuster(int card1, int card2, int card3, int card4, int card5, int numholes, boardinfo info)
        {
            int[] cardsuits = new int[5];
            int[] cardvalues = new int[15];
            int i;

            int maxsamesuits = 0;

            for (i = 0; i <= 14; i++)
                cardvalues[i] = 0;

            if (card1 > 0)
                cardvalues[card1 % 100]++;
            if (card2 > 0)
                cardvalues[card2 % 100]++;
            if (card3 > 0)
                cardvalues[card3 % 100]++;
            if (card4 > 0)
                cardvalues[card4 % 100]++;
            if (card5 > 0)
                cardvalues[card5 % 100]++;

            cardvalues[1] = cardvalues[14];

            for (i = 0; i <= 4; i++)
                cardsuits[i] = 0;

            if (card1 > 0)
                cardsuits[card1 / 100]++;
            if (card2 > 0)
                cardsuits[card2 / 100]++;
            if (card3 > 0)
                cardsuits[card3 / 100]++;
            if (card4 > 0)
                cardsuits[card4 / 100]++;
            if (card5 > 0)
                cardsuits[card5 / 100]++;

            for (i = 1; i <= 4; i++)
                if (maxsamesuits < cardsuits[i])
                    maxsamesuits = cardsuits[i];

            bool processstraight = !info.ifstraightdraw;
            int sthighest = 0;

            for (i = 4; i <= 14; i++)
                if ((i >= 6) && (cardvalues[i] > 0) && (cardvalues[i - 2] > 0) && (cardvalues[i - 3] > 0) &&
                    (cardvalues[i - 4] > 0) && (cardvalues[i - 6] > 0))
                {
                    info.ifstraightdraw = true;
                    processstraight = true;
                    info.drawtype = 3;
                    sthighest = i;
                }

            if (info.ifstraightdraw && processstraight)
            {
                info.drawstraightholesused = numholes;

                if ((sthighest < 7) && (card1 % 100 == 14))
                    card1 = (card1 / 100) * 100 + 1;
                if ((sthighest < 7) && (card2 % 100 == 14))
                    card2 = (card2 / 100) * 100 + 1;
                if ((sthighest < 7) && (card3 % 100 == 14))
                    card3 = (card3 / 100) * 100 + 1;
                if ((sthighest < 7) && (card4 % 100 == 14))
                    card4 = (card4 / 100) * 100 + 1;

                if (numholes == 2)
                {
                    info.drawstraighthighest = 0;

                    if ((card3 % 100 > card1 % 100) && (card3 % 100 > card2 % 100))
                        info.drawstraighthighest++;
                    if ((card4 % 100 > card1 % 100) && (card4 % 100 > card2 % 100))
                        info.drawstraighthighest++;
                }
                else
                    if ((card4 % 100 > card1 % 100) && (card4 % 100 > card2 % 100) && (card4 % 100 > card3 % 100))
                    info.drawstraighthighest = 1;
                else
                    info.drawstraighthighest = 0;

                if (maxsamesuits == 4)
                    info.ifstraightflushdraw = true;
            }
        }

        internal static int GetBoardCoordination(int nComCard1, int nComCard2, int nComCard3, int nComCard4, int nComCard5, int weight)
        {
            int res = 0;

            var nComCards = new List<int> { nComCard1, nComCard2, nComCard3, nComCard4, nComCard5 };

            int firstEmptyIndex = 5;

            for (int i = 0; i < 5; i++)
            {
                if (nComCards[i] == 0)
                {
                    firstEmptyIndex = i;
                    break;
                }
            }

            for (int i = 0; i < firstEmptyIndex; i++)
            {
                int minDiff = 900000;

                for (int j = 0; j < firstEmptyIndex; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (Math.Abs(nComCards[i] % 13 - nComCards[j] % 13) < minDiff)
                    {
                        minDiff = Math.Abs(nComCards[i] % 13 - nComCards[j] % 13) * (nComCards[i] / 13 == nComCards[j] / 13 ? 1 : 3);
                    }
                }

                res += minDiff;
            }

            if (res == 0)
            {
                res = 1;
            }

            return 3 * weight / res;
        }
    }

    internal class boardinfo
    {
        internal PostFlopHand madehand;
        internal int weight;
        internal int holesused;
        internal int type;
        internal int size;
        internal int value1;
        internal int value2;
        internal int kicker;
        internal int boardhigher1;
        internal int boardhigher2;
        internal int straighthighest;
        internal int straightlowest;
        internal int flushvalue;

        internal bool ifstraightflushdraw;
        internal bool ifflushdraw;
        internal bool ifstraightdraw;
        internal int drawflushcardsmissing;
        internal int drawflushholesused;
        internal int drawstraightholesused;
        internal int drawtype;
        internal int drawstraighthighest;
        internal int drawhighest;

        internal int samesuitcards;
        internal int samesuitpairs;
        internal int samevaluecards;
        internal int samevaluepairs;
        internal int similarvaluecards;
        internal int similarvaluepairs;
        internal bool ifmadestraights;
        internal bool ifstraightdraws;
        internal int broadwaycards; // A-T
    };




    internal class lowlevel
    {
        internal bool ifholeoneused;
        internal bool ifholetwoused;
        internal int type;
        internal int size;
        internal int value1;
        internal int value2;
    };
}
