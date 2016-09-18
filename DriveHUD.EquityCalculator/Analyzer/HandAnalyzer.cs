using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class HandAnalyzer
    {
        internal static float ModMultOpponentReRaised = 0.3F, ModMultOpponentSingleRaised = 0.3F, ModMultOpponentUnraised = 0.3F, ModMultOpponentThreeToAFlush = 0.3F, ModMultOpponentPairedBoard = 0.3F, ModMultOpponent4BetPreflop = 0.3F;
        static int PostflopEquitySims = 20000;
        int missed_bets = 0;
        List<List<int>> missed_bets_street = new List<List<int>>();
        List<List<int>> missed_bets_final = new List<List<int>>();
        List<List<String>> advice_street = new List<List<String>>();
        internal List<String> StrongestOpponentHands = new List<String>();
        internal String StrongestOpponentName;
        internal static bool reduceRaiseSize = false;
        bool debug_mode = false;
        bool skipCustom = false;
        void Debug(String msg)
        {
        }

        bool PlayerCheckedOnStreet(HandHistory handHistory, Player player, int street)
        {
            foreach (Action postflopAction in handHistory.PostflopActions[street])
            {
                if (postflopAction.PlayerName.Equals(player.PlayerName) && postflopAction.SAction.Equals("Checks"))
                {
                    return true;
                }
            }
            return false;
        }

        internal Hashtable PostflopAnalysis(HandHistory handHistory, int street, Hashtable collective_range)
        {
            if (handHistory.CommunityCards[street] == "") return collective_range; // Return immediately if this street was never reached


            int missed_bets_before = missed_bets;
            //GHADY
            bool calcMissedBets = true;
            //END GHADY


            String msg = "", player = "", action = "", amount = "", advice = "";

            switch (street)
            {
                case 1: player = "Flop"; break;
                case 2: player = "Turn"; break;
                case 3: player = "River"; break;
                default: player = "N/A"; break;
            }
            action = handHistory.CommunityCards[street];

            if ((handHistory.PotSizeByStreet[street] % 100) == 0)
                amount = (handHistory.PotSizeByStreet[street] / 100.0).ToString();
            else amount = (handHistory.PotSizeByStreet[street] / 100.0).ToString();
            advice = "";

            // Show card graphics
            // Calculate new collective hand range estimates based on the new board
            Hashtable collective = new Hashtable();
            Hashtable hand_weight = new Hashtable();
            foreach (String key in handHistory.Players.Keys)
            {
                collective.Add(key, new hand_distribution());
                (collective[key] as hand_distribution).hand_range = 0;
                hand_weight.Add(key, 0);

                for (int i = 0; i < 52; i++)
                {
                    for (int j = 0; j < 52; j++)
                    {
                        (collective[key] as hand_distribution).draw_matrix[i, j] = (collective_range[key] as hand_distribution).draw_matrix[i, j];
                    }
                }
            }

            float[,] hand_percentile = new float[52, 52]; // Store current hand percentiles

            if (street == 1) // Preflop->Flop
            {
                // Update hand rankings based on the flop dealt
                handHistory.update_absolute_percentiles(street);

                // Loop though all starting hands
                for (int i = 1; i < 52; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        String hand_group;

                        if (i / 13 == j / 13) hand_group = handHistory.preflop_group(Card.AllCards[i % 13].ToString() + "s" + Card.AllCards[j % 13].ToString() + "s"); // Suited
                        else hand_group = handHistory.preflop_group(Card.AllCards[i % 13].ToString() + "h" + Card.AllCards[j % 13].ToString() + "c"); // Offsuit

                        float jam_percentile = (float)Convert.ToDouble(HandHistory.jam_percentile[hand_group]);
                        float deep_percentile = (float)Convert.ToDouble(HandHistory.deepstack_percentile[hand_group]);
                        float short_percentile = (float)Convert.ToDouble(HandHistory.shortstack_percentile[hand_group]);

                        // Update players' hand ranges based on how strong each starting hand is on the given board (use all 3 preflop hand rankings to make the cutoffs smoother)
                        String hand_str = Card.CardName[i].ToString() + Card.CardSuit[i].ToString() + Card.CardName[j].ToString() + Card.CardSuit[j].ToString();
                        float hand_pct = (float)Convert.ToDouble(handHistory.absolute_percentile[hand_str]);
                        hand_percentile[i, j] = hand_percentile[j, i] = hand_pct;

                        foreach (String key in handHistory.Players.Keys)
                        {
                            (collective[key] as hand_distribution).draw_matrix[i, j] = 0.0f;

                            if (jam_percentile <= (collective_range[key] as hand_distribution).hand_range)
                            {
                                (collective[key] as hand_distribution).hand_range += hand_pct;
                                hand_weight[key] = Convert.ToDouble(hand_weight[key]) + 1.0f;
                                (collective[key] as hand_distribution).draw_matrix[i, j] += 1 / 3.0f;
                            }
                            if (deep_percentile <= (collective_range[key] as hand_distribution).hand_range)
                            {
                                (collective[key] as hand_distribution).hand_range += hand_pct;
                                hand_weight[key] = Convert.ToDouble(hand_weight[key]) + 1.0f;
                                (collective[key] as hand_distribution).draw_matrix[i, j] += 1 / 3.0f;
                            }
                            if (short_percentile <= (collective_range[key] as hand_distribution).hand_range)
                            {
                                (collective[key] as hand_distribution).hand_range += hand_pct;
                                hand_weight[key] = Convert.ToDouble(hand_weight[key]) + 1.0f;
                                (collective[key] as hand_distribution).draw_matrix[i, j] += 1 / 3.0f;
                            }
                            (collective[key] as hand_distribution).draw_matrix[j, i] = (collective[key] as hand_distribution).draw_matrix[i, j];
                        }
                    }
                }

                foreach (String key in handHistory.Players.Keys)
                {
                    (collective[key] as hand_distribution).hand_range *= 2;
                    if (Convert.ToDouble(hand_weight[key]) > 0) (collective[key] as hand_distribution).hand_range /= (float)Convert.ToDouble(hand_weight[key]);
                    else (collective[key] as hand_distribution).hand_range = 1.0f;
                }
            }
            else // Flop->Turn, or Turn->River
            {
                // Calculate previous street's hand percentiles
                float[,] last_percentile = new float[52, 52];
                handHistory.update_absolute_percentiles(street - 1);
                for (int i = 1; i < 52; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        String hand_str = Card.CardName[i].ToString() + Card.CardSuit[i].ToString() + Card.CardName[j].ToString() + Card.CardSuit[j].ToString();
                        last_percentile[i, j] = (float)Convert.ToDouble(handHistory.absolute_percentile[hand_str]);
                    }
                }

                // Update hand rankings based on the new card dealt
                handHistory.update_absolute_percentiles(street);

                // Loop though all starting hands
                for (int i = 1; i < 52; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Update players' hand ranges based on the new card
                        String hand_str = Card.CardName[i].ToString() + Card.CardSuit[i].ToString() + Card.CardName[j].ToString() + Card.CardSuit[j].ToString();
                        float hand_pct = (float)Convert.ToDouble(handHistory.absolute_percentile[hand_str]);

                        foreach (String key in handHistory.Players.Keys)
                        {
                            if (last_percentile[i, j] <= (collective_range[key] as hand_distribution).hand_range) // Made hands
                            {
                                (collective[key] as hand_distribution).hand_range += hand_pct;
                                hand_weight[key] = Convert.ToDouble(hand_weight[key]) + 1.0f;
                            }
                            else // Drawing hands
                            {
                                (collective[key] as hand_distribution).hand_range += (collective[key] as hand_distribution).draw_matrix[i, j] * hand_pct;
                                hand_weight[key] = Convert.ToDouble(hand_weight[key]) + (collective[key] as hand_distribution).draw_matrix[i, j] * 1.0f;
                            }
                        }
                        hand_percentile[i, j] = hand_percentile[j, i] = hand_pct;
                    }
                }
                foreach (String key in handHistory.Players.Keys)
                {
                    (collective[key] as hand_distribution).hand_range *= 2;
                    if (Convert.ToDouble(hand_weight[key]) > 0) (collective[key] as hand_distribution).hand_range /= (float)Convert.ToDouble(hand_weight[key]);
                    else (collective[key] as hand_distribution).hand_range = 1.0f;
                }
            }

            // Roughly approximate outs for all starting hands (needs to be done only once in the beginning of the street)
            int[,] outs = new int[52, 52];
            for (int i = 1; i < 52; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    String hand_str = Card.CardName[i].ToString() + Card.CardSuit[i].ToString() + Card.CardName[j].ToString() + Card.CardSuit[j].ToString();
                    if (street == 1) outs[i, j] = outs[j, i] = drawingOuts(handHistory.CommunityCards[1], hand_str, street); // Flop
                    else if (street == 2) outs[i, j] = outs[j, i] = drawingOuts(handHistory.CommunityCards[1] + handHistory.CommunityCards[2], hand_str, street); // Turn
                    else outs[i, j] = outs[j, i] = 0;
                }
            }

            if (handHistory.PostflopActions[street].Count > 0 && (bool)handHistory.PostflopActions[street][0].InHand[handHistory.HeroName])
            {

            }

            float squeeze_fold_p = 1.0f; // Probability that all non-defenders fold (needed for calculating the optimal folding range in multi-way pots)

            // Actions
            for (int i = 0; i < handHistory.PostflopActions[street].Count; i++)
            {
                advice = "";

                String hand_analysis = "", situational_analysis = "", conclusion = "";

                if (handHistory.PostflopActions[street][i].SAction != "Returns")
                {
                    PreAction basics = decisionSummary(handHistory, handHistory.PostflopActions[street][i]); // Size of the pot, position, and other basic data
                    List<String> advancedAdvices = new List<String>();
                    postflop_advice temp_advice = postflop_raise(handHistory, handHistory.PostflopActions[street][i], collective, squeeze_fold_p, advancedAdvices); // ABC Strategy



                    // Strategic adjustments (c-bets, donks, multi-way squeezes, etc.)

                    // Adjusting aggressiveness/passiveness of the ABC-strategy
                    int preflop_aggressor = 0, flop_aggressor = 0, turn_aggressor = 0; // 0 -> no aggressor, 1 -> hero is the aggressor, 2 -> villain is the aggressor
                    String aggressor_name = ""; // If applicable, store the aggressor here
                    for (int j = 0; j < handHistory.PreflopActions.Count; j++)
                    {
                        if (Convert.ToDouble(handHistory.PreflopActions[j].LastStreetCommitment[handHistory.PreflopActions[j].PlayerName]) < (handHistory.Players[handHistory.PreflopActions[j].PlayerName] as Player).StartingStack)
                        { // Don't make aggressor adjustements for players who went all-in in the previous street (we should actually consider "practical all-ins", e.g. 50%+ of effective stacks being in the pot already)
                            if (handHistory.PreflopActions[j].SAction == "Bets" || handHistory.PreflopActions[j].SAction == "Raises")
                            {
                                if (handHistory.PreflopActions[j].PlayerName == handHistory.PostflopActions[street][i].PlayerName) preflop_aggressor = 1;
                                else
                                {
                                    preflop_aggressor = 2;
                                    if (street == 1) aggressor_name = handHistory.PreflopActions[j].PlayerName;
                                }
                            }
                        }
                    }
                    for (int st = 1; st < street; st++)
                    {
                        for (int j = 0; j < handHistory.PostflopActions[st].Count; j++)
                        {
                            if (Convert.ToDouble(handHistory.PostflopActions[st][j].LastStreetCommitment[handHistory.PostflopActions[st][j].PlayerName]) < (handHistory.Players[handHistory.PostflopActions[st][j].PlayerName] as Player).StartingStack)
                            { // Don't make aggressor adjustements for players who went all-in in the previous street (we should actually consider "practical all-ins", e.g. 50%+ of effective stacks being in the pot already)
                                if (handHistory.PostflopActions[st][j].SAction == "Bets" || handHistory.PostflopActions[st][j].SAction == "Raises")
                                {
                                    if (handHistory.PostflopActions[st][j].PlayerName == handHistory.PostflopActions[street][i].PlayerName)
                                    {
                                        if (st == 1) flop_aggressor = 1;
                                        else if (st == 2) turn_aggressor = 1;
                                    }
                                    else
                                    {
                                        if (st == 1) flop_aggressor = 2;
                                        else if (st == 2) turn_aggressor = 2;
                                        if (street == st + 1) aggressor_name = handHistory.PostflopActions[st][j].PlayerName;
                                    }
                                }
                            }
                        }
                    }
                    // Calculate the strategic value of betting out from player's position (can be absolute position or relative to the aggressor)
                    float strategic_position = basics.Players_oop / (float)(basics.Players_oop + basics.Players_ip); // Default to absolute position (0.0-1.0)
                    if ((street == 1 && preflop_aggressor == 2) || (street == 2 && flop_aggressor == 2) || (street == 3 && turn_aggressor == 2))
                    {
                        if (inPosition(handHistory, aggressor_name, handHistory.PostflopActions[street][i].PlayerName)) // Player is OOP (relative to the aggressor)
                        {
                            // Count how many players are OOP relative to the aggressor
                            int oops = 0;
                            foreach (String key in handHistory.PostflopActions[street][i].ThisStreetCommitment.Keys)
                            {
                                if (!(bool)handHistory.PostflopActions[street][i].InHand[key]) continue; // Skip players already folded
                                if (inPosition(handHistory, aggressor_name, key)) oops++;
                            }

                            strategic_position = (oops - basics.Players_oop - 1) / (float)(basics.Players_oop + basics.Players_ip);
                        }
                        //else // Player is IP relative to the aggressor -> Use absolute position
                    }
                    float strategic_aggression = 0.0f; // -1 => never bet, +1 => always bet (x<0 can be considered an aggression multiplier of x+1, while x>0 is a passiveness multiplier of 1-x, so for example +0.25 would double a 20% bet-frequency to ~40%)
                    bool first_action = true;
                    for (int j = 0; j < handHistory.PostflopActions[street].Count; j++)
                    {
                        if (handHistory.PostflopActions[street][j].PlayerName == handHistory.PostflopActions[street][i].PlayerName)
                        {
                            if (j != i) first_action = false;
                            break;
                        }
                    }
                    if (first_action) // Player's first action for this street
                    {
                        if (basics.To_call == 0) // Check/bet -decision
                        {
                            if (street == 1) // Flop
                            {
                                if (preflop_aggressor == 0) // Opening Bet (following preflop limp with a flop bet)
                                {
                                    // Original: Cut 20% bet-% to 4% if OOP
                                    // New: Cut 20% bet-% to 10% if OOP
                                    // Double 20% bet-% to 40% if IP
                                    strategic_aggression = -0.50f * (1 - strategic_position) + // "OOP"	// Original: -0.80
                                                            0.25f * strategic_position;      // "IP"
                                }
                                else if (preflop_aggressor == 1) // Continuation Bet (following preflop raise with a flop bet)
                                {
                                    // Original: Double 20% bet-% to 40% for both positions
                                    // New: Double 20% bet-% to 40% if OOP
                                    // New: Quadruple 20% bet-% to 80% if IP
                                    strategic_aggression = 0.25f * (1 - strategic_position) + // "OOP"
                                                            0.75f * strategic_position;      // "IP"	// Original: 0.25
                                }
                                else if (preflop_aggressor == 2) // Donk Bet (following preflop call with a flop bet) -> This should be relative position to the raiser, right?
                                {
                                    // Cut 20% bet-% to 4% if OOP
                                    strategic_aggression = -0.80f * (1 - strategic_position) + // "OOP"
                                                            0.00f * strategic_position;      // "IP"
                                }
                            }
                            else if (street == 2) // Turn
                            {
                                if (preflop_aggressor == 0 && flop_aggressor == 0) // Opening Bet (following preflop limp and flop check with a turn bet)
                                {
                                    // Double 20% bet-% to 40% if IP
                                    strategic_aggression = 0.00f * (1 - strategic_position) + // "OOP"
                                                            0.25f * strategic_position;      // "IP"
                                }
                                else if (preflop_aggressor == 1 && flop_aggressor == 0) // Delayed Continuation Bet (following preflop raise and flop check with a turn bet)
                                {
                                    // Double 20% bet-% to 40% for both positions
                                    strategic_aggression = 0.25f * (1 - strategic_position) + // "OOP"
                                                            0.25f * strategic_position;      // "IP"
                                }
                                else if (flop_aggressor == 1) // 2nd Barrel (following flop bet with a turn bet)
                                {
                                    // Double 20% bet-% to 40% for both positions
                                    strategic_aggression = 0.25f * (1 - strategic_position) + // "OOP"
                                                            0.25f * strategic_position;      // "IP"
                                }
                                else if (flop_aggressor == 2) // Donk/Float (following flop call with a turn bet)
                                {
                                    // Original: Cut 20% bet-% to 4% if OOP
                                    // New: Cut 20% bet-% to 10% if OOP
                                    // Double 20% bet-% to 40% if IP
                                    strategic_aggression = -0.50f * (1 - strategic_position) + // "OOP"	// Original: -0.80
                                                            0.25f * strategic_position;      // "IP"
                                }
                            }
                            else if (street == 3) // River
                            {
                                if (turn_aggressor == 2) // Donk/Float (following turn call with a river bet)
                                {
                                    // Original: Cut 20% bet-% to 4% if OOP
                                    // New: No change if OOP
                                    // Double 20% bet-% to 40% if IP
                                    strategic_aggression = 0.00f * (1 - strategic_position) + // "OOP"	// Original: -0.80
                                                            0.25f * strategic_position;      // "IP"
                                }
                                else // Any situation where villain is not the turn aggressor (so we either bet and villain called, or turn went check-check)
                                {
                                    // Double 20% bet-% to 40% for both positions
                                    strategic_aggression = 0.25f * (1 - strategic_position) + // "OOP"
                                                            0.25f * strategic_position;      // "IP"
                                }
                            }
                        }
                    }
                    //swprintf(buf, 1024, "DEBUG: Strategic Aggression: %.2f Strategic Position: %.2f OOP: %i IP: %i", strategic_aggression, strategic_position, basics.players_oop, basics.players_ip);
                    //pFrame->AddEditWndMessage(buf);

                    // Is it Hero's turn
                    if (handHistory.PostflopActions[street][i].PlayerName == handHistory.HeroName)
                    {

                        missed_bets_before = missed_bets;
                        HandStreetResult handResult = new HandStreetResult();
                        handResult.Street = street;

                        //hero_action_counter ++;

                        if (street == 1) hand_analysis = String.Format("<u>Hand Analysis</u>: {0} + {1}<br/><br/>", (handHistory.Players[handHistory.HeroName] as Player).Cards, handHistory.CommunityCards[1]);
                        else if (street == 2) hand_analysis = String.Format("<u>Hand Analysis</u>: {0} + {1} {2}<br/><br/>", (handHistory.Players[handHistory.HeroName] as Player).Cards, handHistory.CommunityCards[1], handHistory.CommunityCards[2]);
                        else if (street == 3) hand_analysis = String.Format("<u>Hand Analysis</u>: {0} + {1} {2} {3}<br/><br/>", (handHistory.Players[handHistory.HeroName] as Player).Cards, handHistory.CommunityCards[1], handHistory.CommunityCards[2], handHistory.CommunityCards[3]);

                        situational_analysis = "<u>Recommended Strategy</u>:<br/><br/>";
                        conclusion = "<p align=\"center\"><u>Advice</u>:";

                        // Equity simulations
                        pot_equity ev_simulation = equitySimulation2(handHistory, handHistory.PostflopActions[street][i], collective, PostflopEquitySims, street, true);

                        //NEW CODE TO ADD

                        // Calculate conditional collective hand ranges based on their calling ranges
                        Hashtable conditional_collective = new Hashtable();
                        int temp_bet = (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName]));
                        float all_fold_p = 1;

                        foreach (String key in collective.Keys)
                        {//street
                            hand_distribution handDistribution = collective[key] as hand_distribution;

                            float fold_p = temp_bet / (float)(basics.Pot + temp_bet); // Replace with stats based fold probability (e.g. "fold-to-cbet" stats) when available
                            fold_p = (ev_simulation.postflop_equity_hup); //PotEquityOpponent

                            hand_distribution newHandDistribution = new hand_distribution();
                            newHandDistribution.hand_range = handDistribution.hand_range * (1 - fold_p);

                            for (int j = 0; j < 52; j++)
                            {
                                for (int k = 0; k < 52; k++)
                                {
                                    newHandDistribution.draw_matrix[j, k] = handDistribution.draw_matrix[j, k] * (1 - fold_p);
                                }
                            }
                            conditional_collective.Add(key, newHandDistribution);
                            all_fold_p *= fold_p;
                        }
                        // Calculate conditional pot equity
                        pot_equity ev_conditional = equitySimulation1(handHistory, handHistory.PostflopActions[street][i], conditional_collective, PostflopEquitySims, street);

                        float ev_call = ev_simulation.postflop_equity_all * basics.Pot - (1 - ev_simulation.postflop_equity_all) * basics.To_call;
                        float ev_raise = all_fold_p * basics.Pot + (1 - all_fold_p) * (ev_conditional.postflop_equity_all * (basics.Pot + temp_bet - basics.To_call) - (1 - ev_conditional.postflop_equity_all) * temp_bet);





                        //

                        //Console.WriteLine(ev_simulation.MadeHands.ToString());
                        if (debug_mode)
                        {
                            msg = String.Format("DEBUG: All opponents (random/postflop): {0}% / {1}%", GetLabelNumber(ev_simulation.random_equity_all * 100, 0, null), GetLabelNumber(ev_simulation.postflop_equity_all * 100, 0, null));
                            msg = String.Format("DEBUG: {0} (random/postflop): {1}% / {2}%", ev_simulation.strongest_man, GetLabelNumber(ev_simulation.random_equity_hup * 100, 0, null), GetLabelNumber(ev_simulation.postflop_equity_hup * 100, 0, null));
                            msg = temp_advice.debug;
                        }

                        handResult.TopHand = Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100;
                        handResult.TopHandMsg = String.Format("Top {0}% hand on given board", GetLabelNumber(Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100, 0, null));
                        hand_analysis = String.Format("{0}<bullet>  Top {1}% hand on given board</bullet>", hand_analysis, GetLabelNumber(Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100, 0, null));

                        if (ev_simulation.strongest_man != "")
                        {
                            hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against {2}</bullet>", hand_analysis, GetLabelNumber(ev_simulation.postflop_equity_hup * 100, 0, null), ev_simulation.strongest_man);
                            handResult.PotEquityOpponent = ev_simulation.postflop_equity_hup * 100;
                            handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against {1}", GetLabelNumber(ev_simulation.postflop_equity_hup * 100, 0, null), ev_simulation.strongest_man);
                        }
                        else
                        {
                            hand_analysis = String.Format("{0}<butllet> {1}% pot equity against one opponent</bullet>", hand_analysis, GetLabelNumber(ev_simulation.postflop_equity_hup * 100, 0, null));
                            handResult.PotEquityOpponent = ev_simulation.postflop_equity_hup * 100;
                            handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against one opponent", GetLabelNumber(ev_simulation.postflop_equity_hup * 100, 0, null));
                        }
                        int villains_left = GetActivePlayersNB(handHistory, handHistory.PostflopActions[street][i]) - 1;
                        if (villains_left > 1)
                        {
                            hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against all {2} remaining opponents</bullet>", hand_analysis, GetLabelNumber(ev_simulation.postflop_equity_all * 100, 0, null), villains_left);
                            handResult.PotEquityAll = ev_simulation.postflop_equity_all * 100;
                            handResult.PotEquityAllMsg = String.Format("{0}% pot equity against all {1} remaining opponents", GetLabelNumber(ev_simulation.postflop_equity_all * 100, 0, null), villains_left);
                        }

                        // Out calculations
                        //float live_outs = (float)drawingOuts(handHistory, handHistory.PostflopActions[street][i], street);
                        suckout live_draws = equity_to_outs(ev_simulation.suckout_equity_all, street);
                        if (live_draws.outs >= 1.0f)
                        {
                            hand_analysis = String.Format("{0}<bullet>  Approx. {1} live outs to draw to if behind</bullet>", hand_analysis, Math.Round(live_draws.outs));
                            handResult.OutsMsg = String.Format("Approx. {0} live outs to draw to if behind", Math.Round(live_draws.outs));
                        }

                        //pFrame->AddEditWndMessage(temp_advice.debug); // Debug output for postflop_raise function

                        //GHADY
                        Player heroPlayer = handHistory.Players[handHistory.HeroName] as Player;

                        situational_analysis = String.Format("{0}<bullet>  Raising range: Top {1}% (Raise to ${2})</bullet>", situational_analysis, GetLabelNumber(temp_advice.optimal_raise_range * 100, 0, null), GetLabelNumber((double)temp_advice.raise_size / 100, 2, null));
                        handResult.RaisingRangeTop = temp_advice.optimal_raise_range * 100;
                        handResult.RaisingRangeRaiseTo = (double)temp_advice.raise_size / 100;
                        handResult.RaisingRangeMsg = String.Format("Raising range: Top {0}% (Raise to ${1})", GetLabelNumber(temp_advice.optimal_raise_range * 100, 0, null), GetLabelNumber((double)temp_advice.raise_size / 100, 2, null));


                        if (temp_advice.call_range < 1.0f)
                        {
                            situational_analysis = String.Format("{0}<bullet>  Calling range: Top {1}%", situational_analysis, GetLabelNumber(temp_advice.call_range * 100, 0, null));
                            handResult.CallingRangeTop = temp_advice.call_range * 100;
                            handResult.CallingRangeMsg = String.Format("Calling range: Top {0}%", GetLabelNumber(temp_advice.call_range * 100, 0, null));
                        }



                        float postflop_percentile = (float)Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]);

                        // Aply strategic_aggression to basic strategy to properly mix up the play
                        float real_bluff_range = temp_advice.optimal_bluff_range; // Mixed strategy not used for bluff range, so shrink the size of the range if needed
                        float mix_nonvalue_bets = 0, mix_value_calls = 0; // These will tell how often to bet when checking is the standard play, and when the check when value-bet would be the standard play
                        if (strategic_aggression > 0) mix_nonvalue_bets = strategic_aggression;
                        else if (strategic_aggression < 0)
                        {
                            mix_value_calls = -strategic_aggression;
                            real_bluff_range *= strategic_aggression + 1;
                        }



                        bool abc_raise = false, value_raise = false, bluff_raise = false, value_call = false, gametheory_call = false, easy_fold = false;
                        if (postflop_percentile <= temp_advice.optimal_raise_range) abc_raise = true; // We don't really raise with this range, but use it for explanations
                        if (postflop_percentile <= temp_advice.optimal_raise_range -
                                                   temp_advice.optimal_bluff_range) value_raise = true; // This is the real value-raise range
                        if (postflop_percentile <= temp_advice.call_range &&
                            postflop_percentile >= temp_advice.call_range -
                                                   real_bluff_range) bluff_raise = true; // This is the real bluff-range (not including semi-bluffs though)
                        if (postflop_percentile <= temp_advice.call_range) value_call = true;
                        if (postflop_percentile > 1.2 * Math.Max(temp_advice.call_range, temp_advice.optimal_raise_range)) easy_fold = true;
                        if (postflop_percentile / (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range <= temp_advice.gametheory_call)
                        {
                            //street
                            //GHADY IF OPPONENT CHECKRAISED SKIP THIS VARIABLE
                            if (!temp_advice.OpponentCheckRaised)
                                gametheory_call = true;

                            //If opponent is fish/whale with AF of 2.5 or less,
                            //then bypass the game theory call calculation.
                            if (gametheory_call)
                            {
                                if (AllOpponentsAre(handHistory, handHistory.PostflopActions[street][i], new String[] { "fish", "whale" }))
                                {
                                    bool allOpponentLowAgg = true;
                                    foreach (Player opponent in GetPlayersInHand(handHistory, handHistory.PostflopActions[street][i], false))
                                    {
                                        if (opponent.Stats.Agg > 2.5)
                                        {
                                            allOpponentLowAgg = false;
                                            break;
                                        }
                                    }
                                    if (allOpponentLowAgg)
                                    {
                                        //gametheory_call = false;
                                        //Debug("If opponent is fish/whale with AF of 2.5 or less, then bypass the game theory call calculation.");
                                    }
                                }
                            }
                        }
                        // Output recommended plays here
                        //GHADY POSTFLOP ADD

                        int handRange = (int)Math.Round(Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100);

                        int stackSize = GetPlayerStackOnStreet(handHistory, handHistory.PostflopActions[street][i].PlayerName, handHistory.PostflopActions[street][i]);
                        bool customAdviceSet = false;
                        if (!skipCustom)
                        {
                            //DON'T BLUFF ON THE RIVER AGAINST FISH/WHALE/MANIAC
                            //IF CALL RANGE-HAND RANGE>=4
                            if ((street == 2 || street == 3) && bluff_raise)
                            {
                                bool allOpponentsAreFishWhaleManiacs = true;
                                foreach (String playerName in handHistory.Players.Keys)
                                {
                                    if (playerName.Equals(handHistory.HeroName)) continue;
                                    if (!(bool)handHistory.PostflopActions[street][i].InHand[playerName]) continue;
                                    String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, handHistory.Players[playerName] as Player).ToLower();

                                    if (!opponentModel.Equals("fish") && !opponentModel.Equals("whale") && !opponentModel.Equals("maniac"))
                                    {
                                        allOpponentsAreFishWhaleManiacs = false;
                                        break;
                                    }
                                }

                                if (allOpponentsAreFishWhaleManiacs || street == 2)
                                {
                                    if (temp_advice.call_range * 100 - handRange >= 0)
                                    {
                                        bluff_raise = false;
                                    }
                                }
                            }
                            //




                            if (bluff_raise)
                            {
                                //let's just put an additional check to qualify APC valuing this as a bluff raise
                                //[2:19:22 AM] Ace Poker Solutions: can we add a condition where if raising range AND calling range are > hand value %
                                //[2:19:43 AM] Ace Poker Solutions: then we do not register this as a bluff, but follow APC advice
                                if (temp_advice.call_range * 100 > handRange && temp_advice.raise_size * 100 > handRange)
                                {
                                    bluff_raise = false;
                                }

                                if (bluff_raise && temp_advice.raise_size < basics.Pot + basics.To_call)//ATT
                                {
                                    if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                        temp_advice.custom_advice = "check";
                                    else
                                        temp_advice.custom_advice = "fold";
                                    Debug("Bluff Raise=true BUT raise size<Pot -> " + temp_advice.custom_advice.ToUpper());
                                }
                                else
                                {
                                    //we should only be bluffing if hero can make at least a 3/4ths pot size raise.
                                    //Meaning, both hero and the opponent need to have that much money left in their stack.
                                    //Otherwise, we should bypass the bluff advice and proceed to the next advice.
                                    double minimumAmount = (double)3 / (double)4 * basics.Pot;
                                    bool allPlayersHaveThatMuchMoneyLeft = true;
                                    foreach (Player streetPlayer in GetPlayersInHand(handHistory, handHistory.PostflopActions[street][i], true))
                                    {
                                        if (GetPlayerStackOnStreet(handHistory, streetPlayer.PlayerName, handHistory.PostflopActions[street][i]) < minimumAmount)
                                        {
                                            allPlayersHaveThatMuchMoneyLeft = false;
                                            break;
                                        }
                                    }
                                    if (allPlayersHaveThatMuchMoneyLeft)
                                    {
                                        bluff_raise = false;
                                        Debug("AI advised to bluff, but we should only be bluffing if hero can make at least a 3/4ths pot size raise.");
                                    }
                                }
                            }

                            if (temp_advice.custom_advice != null)
                            {
                                if (temp_advice.custom_advice.Equals("fold"))
                                {
                                    advice = "Fold"; // Default play
                                    missed_bets -= !calcMissedBets ? 0 : handHistory.PostflopActions[street][i].Amount;
                                    conclusion = "Fold";
                                    handResult.Advice = "Fold";
                                    customAdviceSet = true;
                                }
                                else if (temp_advice.custom_advice.Equals("bet") && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                {
                                    advice = "Bet"; // Default play
                                    missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    customAdviceSet = true;
                                }

                                else if (temp_advice.custom_advice.Equals("raise"))
                                {
                                    advice = "Raise"; // Default play
                                    missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;

                                    conclusion = String.Format("{0} <b> Raise - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    customAdviceSet = true;
                                }
                                else if (temp_advice.custom_advice.Equals("check") && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                {
                                    advice = "Check";
                                    missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                    handResult.Advice = "Check";
                                    customAdviceSet = true;
                                }
                            }

                            if (street == 1)
                            {
                                /*
                                if (temp_advice.custom_advice != null && temp_advice.custom_advice.Equals("bet"))
                                {
                                    if (temp_advice.custom_advice.Equals("bet"))
                                    {
                                        advice = "Bet"; // Default play
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;

                                        conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        customAdviceSet = true;
                                    }
                                }
                                else if (temp_advice.custom_advice != null && temp_advice.custom_advice.Equals("check"))
                                {
                                    advice = "Check";
                                    missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                    handResult.Advice = "Check";
                                    customAdviceSet = true;
                                }
                                else
                                 */
                                if (HeroRaisedLimperPreflop(handHistory) && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]) && !HandAnalyzer.BoardIsCoordinated(handHistory, 300, street, false)) //ATT 49335450
                                {
                                    Debug("Hero raised limper preflop + is first to act or is checked to + board not coordinated -> Bet");
                                    advice = "Bet"; // Default play
                                    missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    customAdviceSet = true;
                                }
                            }
                            else if (street == 2)
                            {
                                Action turnHeroAction = null;
                                foreach (Action postflopAction in handHistory.PostflopActions[2])
                                {
                                    if (postflopAction.PlayerName.Equals(handHistory.HeroName))
                                    {
                                        turnHeroAction = postflopAction;
                                        break;
                                    }
                                }
                                if (turnHeroAction != null && FlopChecked_HeroBetsTurn_Paired_Scenario(turnHeroAction, handHistory, collective_range) && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                {
                                    Debug("We need to add logic that if hero has a pocket pair or any pair to the board, and the flop is checked, then hero bets the turn if hero has => 51% pot equity in the hand, and the board is now paired. EX: Hero has: 22  - board is  Q87Q - the flop was checked, and hero is first to act (or it can be checked to hero) - then hero bets 100% of the time.");

                                    advice = "Bet"; // Default play 
                                    missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    conclusion = String.Format("{0} <b> Bet</b></p>", conclusion);

                                    handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    customAdviceSet = true;
                                }

                                /*
                            else if (temp_advice.custom_advice != null)
                            {
                                //(handHistory.PreflopActions[0] as Action).Amount
                                if (temp_advice.custom_advice.Equals("bet"))
                                {
                                    advice = "Bet"; // Default play
                                    //(handHistory.Players[0] as Player).sta
                                    missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    customAdviceSet = true;
                                }
                            }
                                 */
                            }

                            /*
                             *there should be some area of the AI code that says on the flop, turn and river
    that what the made hand value is for HERO
    and basically if there is no made hand, but the equity in the hand is 50% or >
    and the advice is bet... then the text that is shown for the advice should say
    Profitable bluffing situation, bet as a bluff: Bet x$
    instead of what it says now which is, Bet for value 
                             * 
                            */
                            if (street == 1 || street == 2 || street == 3)
                            {
                                boardinfo boardInfo = Jacob.AnalyzeHand(handHistory, street, true);
                                if (boardInfo.madehand == postflophand.kNoPair)
                                {
                                    if (temp_advice.custom_advice != null && temp_advice.custom_advice.Equals("bet"))
                                    {
                                        if (ev_simulation.postflop_equity_hup * 100 > 50 && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                        //if (Math.Round(Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100) >= 50)
                                        {
                                            //Debug(@"on the flop, turn and river if there is no made hand, but the equity in the hand is 50% or > and the advice is bet... then the text that is shown for the advice should say Profitable bluffing situation, bet as a bluff: Bet x$instead of what it says now which is, Bet for value");
                                            advice = "Bet"; // Default play
                                            //(handHistory.Players[0] as Player).sta
                                            missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;

                                            //conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory.BigBlindAmount));
                                            //handResult.Advice = String.Format("Profitable bluffing situation, bet as a bluff - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory.BigBlindAmount));

                                            conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));


                                            customAdviceSet = true;
                                        }
                                    }
                                }
                            }
                            //End Ghady

                            //GHADY
                            //#21734593 - advice is to check / call the flop.
                            //If you have no pair, and no draw you should never be checking and calling.
                            //Should be in THIS particular scenario to check / fold the flop.
                            if (!customAdviceSet)
                            {
                                if (street == 1)
                                {
                                    boardinfo boardInfo = Jacob.AnalyzeHand(handHistory, street, true);
                                    if (boardInfo.madehand == postflophand.kNoPair)
                                    {
                                        if (boardInfo.drawtype == 0)
                                        {
                                            if (PlayerCheckedBeforeAction(handHistory, handHistory.HeroName, handHistory.PostflopActions[street][i]))
                                            {
                                                if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                                {
                                                    Debug("If you have no pair, and no draw you should never be checking and calling.");
                                                    advice = "Check";
                                                    missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                                    conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                                    handResult.Advice = "Check";
                                                    customAdviceSet = true;
                                                }
                                                else
                                                {
                                                    Debug("If you have no pair, and no draw you should never be checking and calling. Should be in THIS particular scenario to check / fold the flop.");
                                                    advice = "Fold"; // Default play
                                                    missed_bets -= !calcMissedBets ? 0 : handHistory.PostflopActions[street][i].Amount;

                                                    conclusion = "Fold";
                                                    handResult.Advice = "Fold";
                                                    customAdviceSet = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (street == 3)
                                {
                                    //#1918359371 - JhKh : Rule - If hero does not have at least one pair using his hole cards (in this case KJ), then do not value bet on the river unless hero has an ace in his hand (his whole cards might be AK instead). 
                                    boardinfo boardInfo = Jacob.AnalyzeHand(handHistory, street, true);
                                    if (boardInfo.madehand == postflophand.kNoPair

                                        && !heroPlayer.Cards[0].Equals('A')
                                        && !heroPlayer.Cards[1].Equals('A')

                                        //&& 
                                        )
                                    {
                                        Debug("If hero does not have at least one pair using his hole cards, then do not value bet on the river unless hero has an ace in his hand.");

                                        if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                        {
                                            advice = "Check";
                                            missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                            conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                            handResult.Advice = "Check";
                                            customAdviceSet = true;
                                        }
                                        else
                                        {
                                            advice = "Fold"; // Default play
                                            missed_bets -= !calcMissedBets ? 0 : handHistory.PostflopActions[street][i].Amount;
                                            conclusion = "Fold";
                                            handResult.Advice = "Fold";
                                            customAdviceSet = true;
                                        }

                                    }
                                }
                            }
                        }
                        //END GHADY

                        bool foldIfNotCallingAllIn = false;
                        //AI POSTFLOP START
                        bool canRaise = CanRaisePostflop(handHistory, handHistory.PostflopActions[street][i]);
                        if (!customAdviceSet && value_raise) // Raise
                        {
                            if (basics.To_call > 0)
                            {
                                if (mix_value_calls <= 0.5 && canRaise)
                                {
                                    advice = "Raise"; // Default play
                                    missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                }
                                else
                                {
                                    advice = "Call"; // Deceptive play
                                    missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                }

                                if (canRaise && mix_value_calls < 0.01) handResult.Advice = String.Format("Raise for value - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else if (canRaise && mix_value_calls < 0.17) handResult.Advice = String.Format("Raise for value - Raise to ${0} \r\n(Occasionally smooth-call)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else if (canRaise && mix_value_calls < 0.33) handResult.Advice = String.Format("Usually raise for value - Raise to ${0} \r\n(Sometimes smooth-call)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else if (canRaise && mix_value_calls <= 0.50) handResult.Advice = String.Format("Sometimes raise for value - Raise to ${0} \r\n(Sometimes smooth-call)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.67) handResult.Advice = String.Format("Sometimes smooth-call \r\n(Sometimes raise for value - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.83) handResult.Advice = String.Format("Usually smooth-call \r\n(Sometimes raise for value - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.99) handResult.Advice = String.Format("Smooth-call \r\n(Occasionally raise for value - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else handResult.Advice = String.Format("Smooth-call for deception");
                            }
                            else
                            {
                                if (mix_value_calls <= 0.5)
                                {
                                    advice = "Bet"; // Default play 
                                    missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                }
                                else
                                {
                                    advice = "Check"; // Deceptive play
                                    missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                }

                                if (mix_value_calls < 0.01)
                                    handResult.Advice = String.Format("Bet for value - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.17) handResult.Advice = String.Format("Bet for value - bet ${0} \r\n(Occasionally check for deception)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.33) handResult.Advice = String.Format("Usually bet for value - bet ${0} \r\n(Sometimes check for deception)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else if (mix_value_calls <= 0.50) handResult.Advice = String.Format("Sometimes bet for value - bet ${0} \r\n(Sometimes check for deception)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.67) handResult.Advice = String.Format("Sometimes check for deception \r\n(Sometimes bet for value - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.83) handResult.Advice = String.Format("Usually check for deception \r\n(Sometimes bet for value - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else if (mix_value_calls < 0.99) handResult.Advice = String.Format("Check for deception \r\n(Occasionally bet for value - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else handResult.Advice = String.Format("Check for deception");
                            }
                        }
                        else if (canRaise && !customAdviceSet && bluff_raise) // Raise
                        {
                            if (basics.To_call > 0)
                            {
                                advice = "Raise";
                                if (!abc_raise) handResult.Advice = String.Format("Raise as a bluff - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                else handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));

                                missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                            }
                            else
                            {
                                advice = "Bet";
                                if (!abc_raise) handResult.Advice = String.Format("Bet as a bluff - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                else handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));

                                missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                            }
                        }
                        else if (canRaise && !customAdviceSet && street == 1 && live_draws.prob[1] >= 0.5) // Value-raise w/ a drawing hand
                        {
                            if (basics.To_call > 0)
                            {
                                advice = "Raise";
                                handResult.Advice = String.Format("Raise - Raise to ${0} \r\n(Monster-draws can be raised for value)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));

                                missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                            }
                            else
                            {
                                advice = "Bet";
                                handResult.Advice = String.Format("Raise - Raise to ${0} \r\n(Monster-draws can be bet for value)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                            }
                        }
                        else if (!customAdviceSet)// Don't raise
                        {
                            if (value_call) // Call
                            {
                                if (basics.To_call > 0)
                                {
                                    if (mix_nonvalue_bets <= 0.5)
                                    {
                                        advice = "Call"; // Default play
                                        missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    }
                                    else
                                    {
                                        advice = "Raise"; // Deceptive play
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    }

                                    if (abc_raise)
                                    {
                                        if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Call to keep the pot small", conclusion);
                                        else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Call to keep the pot small \r\n(Occasionally raise)", conclusion);
                                        else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually call to keep the pot small \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes call to keep the pot small \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes raise \r\n(Sometimes call to keep the pot small)");
                                        else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually raise \r\n(Sometimes call to keep the pot small)");
                                        else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Raise - Raise to ${0} \r\n(Occasionally call to keep the pot small)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else handResult.Advice = String.Format("Raise", conclusion);
                                    }
                                    else
                                    {
                                        if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Call");
                                        else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Call \r\n(Occasionally raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually call \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes call \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes raise - Raise to ${0} \r\n(Sometimes call)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually raise \r\n(Sometimes call)");
                                        else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Raise \r\n(Occasionally call)");
                                        else handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    }
                                }
                                else
                                {
                                    if (mix_nonvalue_bets <= 0.5)
                                    {
                                        advice = "Check";// Default play
                                        missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    }
                                    else
                                    {
                                        advice = "Bet"; // Deceptive play
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    }

                                    if (abc_raise)
                                    {
                                        if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Check to keep the pot small");
                                        else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Check to keep the pot small \r\n(Occasionally bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually check to keep the pot small \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes check to keep the pot small \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes bet - bet ${0} \r\n(Sometimes check to keep the pot small)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually bet - bet ${0} \r\n(Sometimes check to keep the pot small)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Bet - bet ${0} \r\n(Occasionally check to keep the pot small)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    }
                                    else
                                    {
                                        if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Check");
                                        else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Check \r\n(Occasionally bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually check \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes check \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes bet - bet ${0} \r\n(Sometimes check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually bet - bet ${0} \r\n(Sometimes check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Bet - bet ${0} \r\n(Occasionally check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        else handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    }
                                }
                            }
                            else if (gametheory_call) // Game theory based call
                            {
                                if (basics.To_call > 0)
                                {
                                    if (mix_nonvalue_bets <= 0.5)
                                    {
                                        advice = "Call"; // Default play
                                        missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    }
                                    else
                                    {
                                        advice = "Raise"; // Deceptive play
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    }

                                    if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Call to avoid being exploitable");
                                    else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Call to avoid being exploitable \r\n(Occasionally raise - Raise to {1})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually call to avoid being exploitable \r\n(Sometimes raise - Raise to {1})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes call to avoid being exploitable \r\n(Sometimes raise - Raise to {1})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes raise - Raise to ${0} \r\n(Sometimes call to avoid being exploitable)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually raise - Raise to ${0} \r\n(Sometimes call to avoid being exploitable)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Raise - Raise to ${0} \r\n(Occasionally call to avoid being exploitable)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    else handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                }
                                else
                                {
                                    if (mix_nonvalue_bets <= 0.5)
                                    {
                                        advice = "Check"; // Default play
                                        missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    }
                                    else
                                    {
                                        advice = "Bet"; // Deceptive play
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    }

                                    if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Check");
                                    else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Check \r\n(Occasionally bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually check \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes check \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes bet - bet ${0} \r\n(Sometimes check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually bet - bet ${0} \r\n(Sometimes check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Bet - bet ${0} \r\n(Occasionally check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                }
                            }
                            else
                            {
                                if (basics.To_call > 0)
                                {
                                    int all_in_call = (int)(basics.Eff_stacks - (Convert.ToDouble(handHistory.PostflopActions[street][i].LastStreetCommitment[handHistory.HeroName]) + Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])));
                                    float implied_bet = 0;
                                    if (street == 1) implied_bet = (float)(2 * (Math.Sqrt(2.0f) - 1) * basics.Players_ip + 3 * (Math.Sqrt(2.0f) - 1) * basics.Players_oop) / (basics.Players_ip + basics.Players_oop); // Extra money to be won if the draw hits (between 0.83 and 1.24 as a multiple of the pot) - Flop
                                    else if (street == 2) implied_bet = (float)(1 * (Math.Sqrt(2.0f) - 1) * basics.Players_ip + 2 * (Math.Sqrt(2.0f) - 1) * basics.Players_oop) / (basics.Players_ip + basics.Players_oop); // Extra money to be won if the draw hits (between 0.41 and 0.83 as a multiple of the pot) - Turn
                                    if (street == 1 && live_draws.prob[1] >= all_in_call / (float)(all_in_call + basics.Pot + (all_in_call - basics.To_call))) // Enough outs to call flop all-in w/ 2 cards to come
                                    {
                                        if (mix_nonvalue_bets <= 0.5)
                                        {
                                            advice = "Call"; // Default play
                                            missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                        }
                                        else
                                        {
                                            advice = "Raise"; // Deceptive play
                                            missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                        }

                                        if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Call due to good enough pot odds for drawing 2 more cards");
                                        else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Call due to good pot odds \r\n(Occasionally raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually call due to good pot odds \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes call due to good pot odds \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes raise - Raise to ${0} \r\n(Sometimes call due to good pot odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually raise - Raise to ${0} \r\n(Sometimes call due to good pot odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Raise - Raise to ${0} \r\n(Occasionally call due to good pot odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    }
                                    else if (live_draws.prob[0] >= basics.To_call / (float)(basics.To_call + basics.Pot)) // Enough outs for 1-card pot odds call?	
                                    {
                                        if (mix_nonvalue_bets <= 0.5)
                                        {
                                            advice = "Call"; // Default play
                                            missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                        }
                                        else
                                        {
                                            advice = "Raise"; // Deceptive play
                                            missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                        }

                                        if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Call due to good enough pot odds for drawing 1 more card");
                                        else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Call due to good pot odds \r\n(Occasionally raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually call due to good pot odds \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes call due to good pot odds \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes raise - Raise to ${0} \r\n(Sometimes call due to good pot odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually raise - Raise to ${0} \r\n(Sometimes call due to good pot odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Raise - Raise to ${0} \r\n(Occasionally call due to good pot odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    }
                                    //else if (live_draws.prob[0] >= basics.to_call/(float)((basics.to_call+basics.pot)*(1.0f+implied_bet))) // Enough outs for 1-card implied odds call?	
                                    else if (live_draws.prob[0] >= basics.To_call / (float)((basics.To_call + basics.Pot) + Math.Min(implied_bet * (basics.To_call + basics.Pot), all_in_call - basics.To_call))) // Enough outs for 1-card implied odds call?
                                    {
                                        if (mix_nonvalue_bets <= 0.5)
                                        {
                                            advice = "Call"; // Default play
                                            missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                        }
                                        else
                                        {
                                            advice = "Raise"; // Deceptive play
                                            missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                        }

                                        if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Call due to good enough implied odds for drawing 1 more card");
                                        else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Call due to implied odds \r\n(Occasionally raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually call due to implied odds \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes call due to implied odds \r\n(Sometimes raise - Raise to ${0})", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes raise - Raise to ${0} \r\n(Sometimes call due to implied odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually raise - Raise to ${0} \r\n(Sometimes call due to implied odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Raise - Raise to ${0} \r\n(Occasionally call due to implied odds)", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                        else handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                    }
                                    else // Fold
                                    {
                                        //GHADY
                                        //so all we need to is look at what the pot really is,
                                        //and then what hero needs to call, make this a percentage,
                                        //and if it's > the equity we have against our opponent, we call.
                                        //ELSE WE FOLD.
                                        if (((double)basics.To_call / ((double)basics.To_call + (double)basics.Pot)) * 100 < handResult.PotEquityOpponent)
                                        {
                                            //handHistory.GameNumber
                                            //"205530086","205526740","205511989"
                                            foldIfNotCallingAllIn = true;
                                            advice = "Call";
                                            missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                            conclusion = String.Format("{0} <b> Call </b></p>", conclusion);
                                            handResult.Advice = "Call";
                                        }
                                        //END GHADY

                                        else
                                        {
                                            advice = "Fold";
                                            /*if (easy_fold) conclusion=String.Format("{0} <b> Easy fold</b>", conclusion);
                                            else*/
                                            conclusion = String.Format("{0} <b> Fold</b></p>", conclusion);
                                            handResult.Advice = "Fold";
                                            missed_bets -= !calcMissedBets ? 0 : handHistory.PostflopActions[street][i].Amount;
                                        }
                                    }
                                }
                                else
                                {
                                    if (mix_nonvalue_bets <= 0.5)
                                    {
                                        advice = "Check"; // Default play
                                        missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    }
                                    else
                                    {
                                        advice = "Bet"; // Deceptive play
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    }

                                    if (mix_nonvalue_bets < 0.01) handResult.Advice = String.Format("Check");
                                    else if (mix_nonvalue_bets < 0.17) handResult.Advice = String.Format("Check \r\n(Occasionally bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.33) handResult.Advice = String.Format("Usually check \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets <= 0.50) handResult.Advice = String.Format("Sometimes check \r\n(Sometimes bet - bet ${0})", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.67) handResult.Advice = String.Format("Sometimes bet - bet ${0} \r\n(Sometimes check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.83) handResult.Advice = String.Format("Usually bet - bet ${0} \r\n(Sometimes check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else if (mix_nonvalue_bets < 0.99) handResult.Advice = String.Format("Bet - bet ${0} \r\n(Occasionally check)", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                    else handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                }
                            }
                        }


                        if (!skipCustom)
                        {
                            //On river if hand strength is not =< 5%,
                            //and opponent has bet the river, then call instead of raise.
                            if (street == 3 && advice.Equals("Raise") && handRange >= 5)
                            {
                                advice = "Call";
                                missed_bets = missed_bets_before;
                                missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                conclusion = String.Format("{0} <b> Call </b></p>", conclusion);
                                handResult.Advice = "Call";
                            }



                            //#1918377055 - JsTc : Rule - Against fish/whales/gambler we can call flop and/or turn bets
                            //against them with open ended straight draws if their bet is the size of
                            //the pot or less AND they have at least 4.5x the bet amount they are
                            //betting left in their stack.
                            if ((street == 1 || street == 2) && advice.Equals("Fold"))
                            {
                                Player opponentWhoBet = handHistory.Players[handHistory.PostflopActions[street][i].Attacker] as Player;
                                if (!Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                                {
                                    if (PlayerHasOESD(handHistory, handHistory.HeroName, street))
                                    {
                                        if (basics.To_call > 0)
                                        {
                                            String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, opponentWhoBet).ToLower();

                                            if (opponentModel.Equals("fish") || opponentModel.Equals("whale") || opponentModel.Equals("gambler"))
                                            {
                                                int opponentStacks = GetPlayerStackOnStreet(handHistory, opponentWhoBet.PlayerName, handHistory.PostflopActions[street][i]);
                                                if (basics.To_call <= basics.Pot && opponentStacks >= 4.5 * basics.To_call)
                                                {
                                                    Debug("Rule - Against fish/whales/gambler we can call flop and/or turn bets against them with open ended straight draws if their bet is the size of the pot or less AND they have at least 4.5x the bet amount they are betting left in their stack.");
                                                    handResult.AdvancedAdvices.Add("Against players that offer high implied odds such as fish, whales, or gamblers, you can with open ended straight draws more liberally.");

                                                    missed_bets = missed_bets_before;

                                                    advice = "Call";
                                                    missed_bets = missed_bets_before;
                                                    missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                                    conclusion = String.Format("{0} <b> Call </b></p>", conclusion);
                                                    handResult.Advice = "Call";
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            // Need a rule that if hand is top 2% or less and hero is not the PFR,
                            //then they call instead of raise, unless board is very cordinated.
                            //2214992429
                            if (advice.Equals("Raise") && handRange <= 2
                                && !PlayerIsPreflopRaiser(handHistory, heroPlayer, false) //hero is not PFR
                                && !BoardIsCoordinated(handHistory, 200, street, false))
                            {
                                advice = "Call";
                                missed_bets = missed_bets_before;
                                missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                conclusion = String.Format("{0} <b> Call </b></p>", conclusion);
                                handResult.Advice = "Call";
                            }


                            //MOVE ALL IN / CALL ALL IN
                            temp_advice.moveAllIn = true;
                            int amountToUse = advice.Equals("Call") ? basics.To_call : advice.Equals("Raise") ? temp_advice.raise_size : advice.Equals("Bet") ? temp_advice.bet_size : 0;
                            foreach (String playerName in handHistory.Players.Keys)
                            {
                                if (playerName.Equals(handHistory.HeroName)) continue;
                                if (!(bool)handHistory.PostflopActions[street][i].InHand[playerName]) continue;
                                int opponentStack = GetPlayerStackOnStreet(handHistory, playerName, handHistory.PostflopActions[street][i]);

                                if (amountToUse < opponentStack)
                                {
                                    temp_advice.moveAllIn = false;
                                    break;
                                }
                            }

                            int heroStack = GetPlayerStackOnStreet(handHistory, handHistory.HeroName, handHistory.PostflopActions[street][i]);
                            if (!temp_advice.moveAllIn && advice.Equals("Raise"))
                            {
                                int heroAmount = handHistory.PostflopActions[street][i].Amount;

                                if (heroAmount == heroStack && heroAmount <= temp_advice.raise_size)
                                {
                                    temp_advice.moveAllIn = true;
                                }
                            }
                            //#49341650 - 2h2c : Player has no more money left (and no more players left to act). Should be call all-in.
                            bool opponentsHaveNoMoreMoney = false;
                            foreach (String playerName in handHistory.Players.Keys)
                            {
                                if (!playerName.Equals(handHistory.HeroName))
                                {
                                    if ((bool)handHistory.PostflopActions[street][i].InHand[playerName])
                                    {
                                        int opponentStack = GetPlayerStackOnStreet(handHistory, playerName, handHistory.PostflopActions[street][i]);
                                        opponentsHaveNoMoreMoney = opponentStack == 0;
                                        if (!opponentsHaveNoMoreMoney) break;
                                    }
                                }
                            }
                            if (opponentsHaveNoMoreMoney)
                            {
                                int heroStacks = GetPlayerStackOnStreet(handHistory, handHistory.HeroName, handHistory.PostflopActions[street][i]);
                                Debug("Player has no more money left (and no more players left to act). Should be call all-in.");
                                //temp_advice.bet_size = temp_advice.raise_size = heroStacks;
                                temp_advice.callAllIn = true;
                            }
                            //

                            //CALL ALL IN
                            if ((advice.Equals("Call") || advice.Equals("Raise")) && basics.To_call > heroStack)
                            {
                                temp_advice.callAllIn = true;
                            }


                            //


                            //GHADY POSTFLOP OVERRIDING AI ADVICE

                            //ON RIVER ONLY: If hand value % is => 25% AND calling range is within +/- 3 percentage points of the hand value 
                            //percentage, and opponent is facing a bet, AND the advice is to move all-in, then call instead.
                            if (street == 3)
                            {
                                if (temp_advice.moveAllIn && handRange >= 25 && (temp_advice.call_range * 100 >= handRange - 3 && temp_advice.call_range * 100 <= handRange + 3) && GetOpponentBettingPostflop(handHistory, handHistory.PostflopActions[street][i]) != null)
                                {
                                    Debug("ON RIVER ONLY: If hand value % is => 25% AND calling range is within +/- 3 percentage points of the hand value percentage, and opponent is facing a bet, AND the advice is to move all-in, then call instead.");

                                    advice = "Call";
                                    missed_bets = missed_bets_before;
                                    missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    conclusion = String.Format("{0} <b> Call </b></p>", conclusion);
                                    handResult.Advice = "Call";
                                }
                            }


                            //On the river, if APC advice is usually check / sometimes bet AND Hero
                            //has at least a pair (using at least one of his hole cards),
                            //AND Hero is against fish/whale/gambler, then change the advice to bet
                            //(and use the bet sizing it's recommending). EX hand: #49321325 - QdKh.
                            //Here Hero made a pair of kings on the river (using one of his whole cards).
                            //This important because we don't want to change the advice if there is a pair
                            //on the board (not using hero's hole cards). And just so you dont' get confused,
                            //there can be a pair AND hero can also have a pair using one of his hole cards.
                            //EX: Hero has KQ ... board on river is 5 5 T 9 K .... here hero has 2 pair.
                            //55's and Kings... this would qualify for this rule too.
                            //EX HAND: #86844590 - 8hTs
                            if (street == 3 && !Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                            {
                                if (handResult.Advice.ToLower().Contains("usually check") && handResult.Advice.ToLower().Contains("sometimes bet"))
                                {
                                    String sHeroCards = (handHistory.Players[handHistory.HeroName] as Player).Cards;
                                    char[] heroCards = new char[] { sHeroCards[0], sHeroCards[2] };
                                    String sCommunityCards = handHistory.CommunityCards[1] + handHistory.CommunityCards[2] + handHistory.CommunityCards[3];
                                    char[] commCards = new char[] { sCommunityCards[0], sCommunityCards[2], sCommunityCards[4], sCommunityCards[6], sCommunityCards[8] };
                                    bool hasPairsUsingHolecards = false;
                                    foreach (char heroCard in heroCards)
                                    {
                                        int nbSame = 0;
                                        foreach (char commCard in commCards)
                                        {
                                            if (heroCard.Equals(commCard))
                                            {
                                                nbSame++;
                                                if (nbSame > 1) break;
                                            }
                                        }
                                        if (nbSame == 1)
                                        {
                                            hasPairsUsingHolecards = true;
                                            break;
                                        }
                                    }

                                    if (hasPairsUsingHolecards)
                                    {
                                        if (handRange <= 26 && AllOpponentsAre(handHistory, handHistory.PostflopActions[street][i], new String[] { "fish", "whale", "gambler" }))
                                        {

                                            Debug("On the river, if APC advice is usually check / sometimes bet AND Hero has at least a pair (using at least one of his hole cards), AND Hero is against fish/whale/gambler, AND Hero's hand value is 26% or less, then change the advice to bet");
                                            missed_bets = missed_bets_before;
                                            advice = "Bet";
                                            missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                            conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));

                                            handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        }
                                    }
                                }
                            }

                            if (!temp_advice.callAllIn && foldIfNotCallingAllIn)
                            {
                                missed_bets = missed_bets_before;
                                conclusion = String.Format("{0} <b> Fold</b></p>", conclusion);
                                handResult.Advice = "Fold";
                                missed_bets -= handHistory.PreflopActions[i].Amount;
                            }
                            else if (foldIfNotCallingAllIn)
                            {
                                Debug("If pot odds<pot equity -> CALL");
                            }

                            if (temp_advice.callAllIn && !advice.Equals("Fold") && !advice.Equals("Check"))
                            {
                                advice = "Call";
                                missed_bets = missed_bets_before;
                                missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                conclusion = String.Format("{0} <b> Call All In</b></p>", conclusion);
                                handResult.Advice = "Call All In";
                            }
                            else if ((temp_advice.moveAllIn) && !advice.Equals("Fold") && !advice.Equals("Check"))
                            {
                                Debug("APC ADVICE: " + advice + " + " + advice + " amount=" + ((double)amountToUse / 100) + " > opponents Stack");

                                if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                    advice = "Bet";
                                else
                                    advice = "Raise";
                                missed_bets = missed_bets_before;
                                int missedValue = 0;
                                if (heroStack != handHistory.PostflopActions[street][i].Amount)
                                    missedValue = !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                if (missedValue > 0)
                                    missed_bets += missedValue;

                                conclusion = String.Format("{0} <b> Move all-in for ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                                handResult.Advice = String.Format((bluff_raise ? "Profitable bluffing situation: " : "") + "Move all-in for ${0}", GetLabelNumber((double)temp_advice.raise_size / 100, 2, handHistory));
                            }
                            else if (advice.Equals("Bet"))
                            {
                                //#1918312576 - Jc8s : General rule - If hand analysis value is 30% or less (meaning 31-100) on the flop or later streets, and the AI advice is to bet... then we just say bet, and not bet for value. This hand is saying bet for value, but it should just say bet on the turn.
                                if (handRange > 30)
                                {
                                    if (handResult.Advice.ToLower().Contains("bet for value"))
                                    {
                                        conclusion = String.Format("{0} <b> Bet - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        handResult.Advice = String.Format("Bet - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                        Debug("General rule - If hand analysis value is 30% or less (meaning 31-100) on the flop or later streets, and the AI advice is to bet... then we just say bet, and not bet for value. This hand is saying bet for value, but it should just say bet on the turn.");
                                    }
                                }
                            }
                            else
                            {

                                //if (!Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                                {
                                    //AI returns bluff as yes against a fish or whale, then we need to change this to check
                                    if (handResult.Advice.ToLower().Contains("bluff") && (!handResult.Advice.ToLower().Contains("sometimes")
                                        || (handResult.Advice.ToLower().Contains("sometimes") && handResult.Advice.ToLower().IndexOf("sometimes") > handResult.Advice.ToLower().IndexOf("bluff"))))
                                    {
                                        bool allOpponentsAreFishOrWhale = true;
                                        foreach (String playerName in handHistory.Players.Keys)
                                        {
                                            if (playerName.Equals(heroPlayer.PlayerName)) continue;
                                            if (!(bool)handHistory.PostflopActions[street][i].InHand[playerName]) continue;

                                            Player opponent = handHistory.Players[playerName] as Player;
                                            String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, handHistory.Players[playerName] as Player);

                                            if (Settings.CurrentSettings.TurnOffPlayerModelAdjustments) opponentModel = "";

                                            if ((!opponentModel.ToLower().Equals("fish") && !opponentModel.ToLower().Equals("whale")) || Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                                            {
                                                allOpponentsAreFishOrWhale = false;
                                                break;
                                            }
                                        }

                                        if (allOpponentsAreFishOrWhale && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, handHistory.PostflopActions[street][i]))
                                        {
                                            Debug("AI returns bluff as yes against a fish or whale, then we need to change this to check");
                                            missed_bets = missed_bets_before;
                                            advice = "Check";
                                            missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                            conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                            handResult.Advice = "Check";
                                        }
                                    }


                                    //On the flop
                                    //if hero is facing a fish,whale or maniac heads up, or all opponents in the pot are fish/whale
                                    //and if the advice is sometimes bet... swap the sometimes as the primary advice
                                    //just on the flop, and just when the advice comes up as usually check, sometimes bet
                                    if (street == 1 && advice.Equals("Check"))
                                    {
                                        if (handResult.Advice.ToLower().Contains("usually check") && handResult.Advice.ToLower().Contains("sometimes bet"))
                                        {
                                            Player headsUpOpponent = HeadsUpOrOneOpponent(handHistory, handHistory.PostflopActions[street][i]);

                                            bool applyRule = false;
                                            if (headsUpOpponent != null)
                                            {
                                                String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, headsUpOpponent).ToLower();
                                                if (opponentModel.Equals("fish") || opponentModel.Equals("whale") || opponentModel.Equals("maniac"))
                                                    applyRule = true;
                                            }
                                            else
                                            {
                                                bool allOpponentsAreFishOrWhale = true;
                                                foreach (String playerName in handHistory.Players.Keys)
                                                {
                                                    if (playerName.Equals(heroPlayer.PlayerName)) continue;
                                                    if (!(bool)handHistory.PostflopActions[street][i].InHand[playerName]) continue;

                                                    Player opponent = handHistory.Players[playerName] as Player;
                                                    String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, handHistory.Players[playerName] as Player);


                                                    if ((!opponentModel.ToLower().Equals("fish") && !opponentModel.ToLower().Equals("whale")) || Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                                                    {
                                                        allOpponentsAreFishOrWhale = false;
                                                        break;
                                                    }
                                                }
                                                applyRule = allOpponentsAreFishOrWhale;
                                            }

                                            if (applyRule)
                                            {
                                                Debug("On the flop, if hero is facing a fish,whale or maniac heads up, or all opponents in the pot are fish/whale, and if the advice is sometimes bet... swap the sometimes as the primary advice.");
                                                missed_bets = missed_bets_before;
                                                advice = "Bet"; // Default play
                                                missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;

                                                conclusion = String.Format("{0} <b> Bet for value - bet ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                                handResult.Advice = String.Format("Bet for value - bet ${0}", GetLabelNumber((double)temp_advice.bet_size / 100, 2, handHistory));
                                            }
                                        }
                                    }
                                }
                            }
                        }



                        advice_street[street].Add(advice);

                        //GHADY
                        //MISSED VALUE

                        if (advice.Equals("Bet"))
                        {
                            String oldAdvice = handResult.Advice.ToLower().Contains("bet for value") ? "Bet for value" : "Bet";
                            conclusion = String.Format("{0} <b> " + oldAdvice + " - bet ${1}-${2}</b></p>", conclusion, GetLabelNumber(((double)temp_advice.bet_size - handHistory.BigBlindAmount) / 100, 2, handHistory), GetLabelNumber(((double)temp_advice.bet_size + handHistory.BigBlindAmount) / 100, 2, handHistory));
                            handResult.Advice = String.Format(oldAdvice + " - bet ${0}-${1}", GetLabelNumber(((double)temp_advice.bet_size - handHistory.BigBlindAmount) / 100, 2, handHistory), GetLabelNumber(((double)temp_advice.bet_size + handHistory.BigBlindAmount) / 100, 2, handHistory));
                        }


                        handResult.ev_raise = ev_raise;

                        Action currentPostflopAction = handHistory.PostflopActions[street][i];
                        String sAction = currentPostflopAction.SAction;

                        if ((advice.Equals("Bet") && sAction.Equals("Bets"))
                            || (advice.Equals("Raise") && sAction.Equals("Raises")))
                        {
                            bool allOpponentsFoldedAfter = true;
                            bool afterHero = false;
                            foreach (Action postflopAction in handHistory.PostflopActions[street])
                            {
                                if (afterHero && !postflopAction.PlayerName.Equals(handHistory.HeroName) && !postflopAction.SAction.Equals("Folds"))
                                {
                                    allOpponentsFoldedAfter = false;
                                    break;
                                }
                                if (postflopAction == handHistory.PostflopActions[street][i])
                                    afterHero = true;
                            }
                            if (allOpponentsFoldedAfter)
                            {
                                missed_bets = missed_bets_before;
                                calcMissedBets = false;
                            }
                        }




                        int streetz = street;
                        if (!sAction.ToLower().Contains(advice.ToLower()))
                        {
                            if (handResult.Advice.ToLower().Contains("sometimes"))
                            {
                                if (handResult.Advice.ToLower().Contains(sAction.Substring(0, sAction.Length - 1).ToLower()))
                                {
                                    missed_bets = missed_bets_before;
                                    if (sAction.Equals("Bets"))
                                    {
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.bet_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    }
                                    else if (sAction.Equals("Raises"))
                                    {
                                        missed_bets += !calcMissedBets ? 0 : (int)(temp_advice.raise_size - Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PostflopActions[street][i].Amount;
                                    }
                                    else if (sAction.Equals("Checks"))
                                    {
                                        missed_bets += !calcMissedBets ? 0 : basics.To_call - handHistory.PostflopActions[street][i].Amount;
                                    }
                                }
                            }
                            else
                            {
                                if (sAction.Equals("Checks") && advice.Equals("Bet"))
                                {
                                    missed_bets = missed_bets_before;
                                    missed_bets += (int)(Math.Abs(ev_raise));

                                    //River only EV rule: if APC recommends bet, but hero checks...
                                    //then we only run EV raise if opponents hand value is 30% or less
                                    if (street == 3 && Settings.CurrentSettings.MissedEVCalculation_StreetByStreet)
                                    {
                                        bool useEvRaise = true;
                                        foreach (Player opponent in GetPlayersInHand(handHistory, handHistory.PostflopActions[street][i], false))
                                        {
                                            if ((collective_range[opponent.PlayerName] as hand_distribution).hand_range * 100 > 30)
                                            {
                                                useEvRaise = false;
                                                break;
                                            }
                                        }
                                        if (!useEvRaise) missed_bets = missed_bets_before;
                                    }
                                }
                                else if (sAction.Equals("Folds") && advice.Equals("Call"))
                                {
                                    missed_bets = missed_bets_before;
                                    missed_bets += (int)(Math.Abs(ev_call));
                                }
                                else if (sAction.Equals("Folds") && advice.Equals("Raise"))
                                {
                                    missed_bets = missed_bets_before;
                                    missed_bets += (int)(Math.Abs(ev_raise));
                                }
                                else if (sAction.Equals("Calls") && advice.Equals("Raise"))
                                {
                                    missed_bets = missed_bets_before;
                                    missed_bets += (int)(Math.Abs(ev_raise));
                                }
                                //else if (sAction.Equals("Checks") && advice.Equals("Raise"))
                                //{
                                //   missed_bets = missed_bets_before;
                                //    missed_bets += (int)(Math.Abs(ev_raise));
                                //}
                            }
                        }
                        else
                        {
                            //override missed_value
                            if (handHistory.handResults[handHistory.handResults.Count - 1].Street == currentPostflopAction.Street)
                            {
                                Action previousPostflopAction = null;
                                foreach (Action postflopAction in handHistory.PostflopActions[street])
                                {
                                    if (postflopAction.Equals(currentPostflopAction)) break;
                                    if (postflopAction.PlayerName.Equals(handHistory.HeroName))
                                    {
                                        previousPostflopAction = postflopAction;
                                    }
                                }

                                if (previousPostflopAction != null && (handHistory.handResults[handHistory.handResults.Count - 1].CorrectAction.Equals("Bet") || handHistory.handResults[handHistory.handResults.Count - 1].CorrectAction.Equals("Raise")) && previousPostflopAction.SAction.Equals("Checks"))
                                {
                                    missed_bets_before -= missed_bets_street[street][missed_bets_street[street].Count - 1];
                                    missed_bets_street[street][missed_bets_street[street].Count - 1] = 0;

                                    missed_bets = missed_bets_before;


                                    //int missed_value= (int)handHistory.handResults[handHistory.handResults.Count - 1].BetSize-(int)currentPostflopAction.Amount;
                                    int missed_value = Math.Abs((int)handHistory.handResults[handHistory.handResults.Count - 1].ev_raise) - (int)currentPostflopAction.Amount;
                                    if (missed_value > 0) missed_bets += missed_value;
                                }
                            }
                            //
                        }

                        //RISK AMOUNT CANNOT BE MORE THAN OPPONENT HAS
                        int minStack = -1;
                        int missedVal = missed_bets - missed_bets_before;

                        if (missedVal < 0)
                        {
                            missedVal *= -1;
                            foreach (String opponent in handHistory.Players.Keys)
                            {
                                if (opponent.Equals(handHistory.HeroName)) continue;
                                if (!(bool)handHistory.PostflopActions[street][i].InHand[opponent]) continue;
                                double opponentStack = GetPlayerStackOnStreet(handHistory, opponent, handHistory.PostflopActions[street][i]);
                                if (handHistory.PostflopActions[street][i].Amount > opponentStack && minStack == -1 || opponentStack < minStack)
                                {
                                    minStack = (int)opponentStack;
                                }
                            }
                            if (minStack != -1)
                            {
                                int adviceAmount = advice.Equals("Bet") ? temp_advice.bet_size : temp_advice.raise_size;
                                missed_bets += handHistory.PostflopActions[street][i].Amount - minStack;
                            }
                        }


                        //If hero's hand strength is 2% or less, and hero raises or bets more than APC recommends
                        //AND opponent calls the bet or raise, then potential risked EV = 0.
                        //EX: #69037153 - 6c6d
                        if (handRange <= 2)
                        {
                            if ((sAction.Equals("Bets") || sAction.Equals("Raises")) && missed_bets < 0)
                            {
                                bool opponentCalledBetRaise = false;
                                foreach (Action PostflopAction in GetAllPostflopActionsOnStreetAfter(handHistory, handHistory.PostflopActions[street][i]))
                                {
                                    if (PostflopAction.PlayerName.Equals(handHistory.HeroName))
                                        break;
                                    if (PostflopAction.SAction.Equals("Calls"))
                                    {
                                        opponentCalledBetRaise = true;
                                        break;
                                    }
                                }
                                if (opponentCalledBetRaise)
                                {
                                    missed_bets = missed_bets_before;
                                }
                            }
                        }



                        handResult.HeroAction = handHistory.PostflopActions[street][i].SAction;
                        handResult.Action = handHistory.PostflopActions[street][i];
                        handResult.BetSize = temp_advice.bet_size;
                        handResult.RaiseSize = temp_advice.raise_size;
                        handResult.CorrectAction = advice;


                        missed_bets_final[street].Add(missed_bets - missed_bets_before);
                        //Final Result EV
                        if (!Settings.CurrentSettings.MissedEVCalculation_StreetByStreet)
                        {
                            //If APC advice is check, but hero bets AND opponent folds, then = 0 EV.
                            //If APC advice is to fold, but HERO raises AND opponent folds then = 0 EV.
                            if ((handResult.CorrectAction.Equals("Check") && handResult.HeroAction.Equals("Check"))
                                || (handResult.CorrectAction.Equals("Fold") && handResult.HeroAction.Equals("Raises")))
                            {
                                bool allOpponentsFolded = true;
                                foreach (Action actionAfterHero in GetAllPostflopActionsOnStreetAfter(handHistory, handResult.Action))
                                {
                                    if (actionAfterHero.PlayerName.Equals(handHistory.HeroName) || !actionAfterHero.SAction.Equals("Folds"))
                                    {
                                        allOpponentsFolded = false;
                                        break;
                                    }
                                }

                                if (allOpponentsFolded)
                                {
                                    missed_bets = missed_bets_before;
                                }
                            }

                            //Ex: #86848010 - AhQc
                            if (handResult.Street == 3)
                            {
                                if (HeroMovedAllIn(handHistory, handHistory.PostflopActions[street][i]))
                                {
                                    int actionIndex = 0;
                                    int lastStreet = -1;
                                    foreach (HandStreetResult handStreetResult in handHistory.handResults)
                                    {
                                        if (handStreetResult.Street == 3) continue;
                                        if (handStreetResult.Street != lastStreet)
                                            actionIndex = 0;
                                        else actionIndex++;

                                        //if (handStreetResult.Advice.ToLower().Contains("all-in") && (handStreetResult.HeroAction.Equals("Raises") || handStreetResult.HeroAction.Equals("Calls")))
                                        if (missed_bets_final[handStreetResult.Street][actionIndex] > 0)
                                        {
                                            //if (!HeroMovedAllIn(handHistory, handStreetResult.Action))
                                            {
                                                missed_bets_final[handStreetResult.Street][actionIndex] = 0;
                                                //missed_bets_final[street][missed_bets_final[street].Count - 1] = 0;
                                                //break;
                                            }
                                        }
                                        actionIndex++;
                                    }
                                }
                            }
                        }


                        missed_bets_street[street].Add(missed_bets - missed_bets_before);
                        handResult.AdvancedAdvices.AddRange(advancedAdvices);
                        handHistory.handResults.Add(handResult);
                    }

                    if (debug_mode)
                    {
                        msg = String.Format("{0} ({1}) is {2}%: {3}% -> ${4} and {5}% -> Call", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PostflopActions[street][i].PlayerName] as Player).Position], handHistory.PostflopActions[street][i].PlayerName, GetLabelNumber((collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range / 2 * 100, 0, null), GetLabelNumber(temp_advice.optimal_raise_range * 100, 0, null), GetLabelNumber((double)temp_advice.raise_size / 100.0f, 2, null), GetLabelNumber(temp_advice.call_range * 100, 0, null));
                    }

                    // Calculate a raw probability for the player to semi-bluff (before normalizing the semi-bluff probability to match the game-theoretic bluffing probability)
                    int all_in_calls = (int)(basics.Eff_stacks - (Convert.ToDouble(handHistory.PostflopActions[street][i].LastStreetCommitment[handHistory.PostflopActions[street][i].PlayerName]) + Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.PostflopActions[street][i].PlayerName])));
                    float raw_semibluff_p = 0.0f, raw_semibluff_w = 0.0f;
                    for (int a = 0; a < 52; a++)
                    {
                        for (int b = 0; b < 52; b++)
                        {
                            if (a == b) continue;

                            raw_semibluff_w += (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b];

                            if (hand_percentile[a, b] <= temp_advice.optimal_raise_range - temp_advice.optimal_bluff_range) continue; // Skip value-raising hands
                            if ((street == 1 && GameRules.drawing_outs[outs[a, b], 1] >= all_in_calls / (float)(basics.Pot - basics.To_call + 2 * all_in_calls)) ||
                                (street == 2 && GameRules.drawing_outs[outs[a, b], 0] >= all_in_calls / (float)(basics.Pot - basics.To_call + 2 * all_in_calls)))
                            {
                                // Assume 100% semi-bluffs from drawing hands that are pot-committed on the flop (could even be favorites to win in showdown)
                                raw_semibluff_p += (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b];
                            }
                            else
                            {
                                // Tried to use probablity to hit the draw as a proxy for the semi-bluff probability for non-committed drawing hands, but pot-equity calculations against really strong lines were way too optimistic
                                //raw_semibluff_p += handHistory.drawing_outs[outs[a][b]][0] * collective[handHistory.PostflopActions[street][i].PlayerName].draw_matrix[a][b];
                            }
                        }
                    }
                    if (raw_semibluff_w > 0) raw_semibluff_p /= raw_semibluff_w;





                    // Update hand range estimates (and one game theory variable)
                    if (handHistory.PostflopActions[street][i].SAction == "Raises" || handHistory.PostflopActions[street][i].SAction == "Bets")
                    {
                        // Normalize the semi-bluff probability by game theoretic bluff probability
                        float optimal_bluff_p = temp_advice.actual_bluff_range / (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range;
                        float semi_bluff_multiplier = 1.0f;
                        if (raw_semibluff_p > 0) semi_bluff_multiplier = optimal_bluff_p / raw_semibluff_p;

                        // If strategic situation warrant more aggressive play (e.g. c-bets), use wider hand range for hand reading too
                        float temp_raise_range = temp_advice.actual_raise_range;
                        if (strategic_aggression > 0) temp_raise_range = 1 - (1 - temp_raise_range) * (1 - strategic_aggression);

                        // Use the tightest range we have seen so far 
                        (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range = Math.Min((collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range, temp_raise_range);
                        // Reset the squeeze_fold_p
                        squeeze_fold_p = 1.0f;

                        // Update drawing hand matrix, and make sure the semi-bluff probability is in line with game theoretic bluffing probability
                        for (int a = 0; a < 52; a++)
                        {
                            for (int b = 0; b < 52; b++)
                            {
                                if (a == b) continue;
                                if (hand_percentile[a, b] <= temp_advice.actual_raise_range - temp_advice.optimal_bluff_range) continue; // Don't adjust draw-matrix for hands raised for value
                                if ((street == 1 && GameRules.drawing_outs[outs[a, b], 1] >= all_in_calls / (float)(basics.Pot - basics.To_call + 2 * all_in_calls)) ||
                                    (street == 2 && GameRules.drawing_outs[outs[a, b], 0] >= all_in_calls / (float)(basics.Pot - basics.To_call + 2 * all_in_calls)))
                                {
                                    (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b] = Math.Min(
                                        (semi_bluff_multiplier * (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b]), 1.0f);
                                }
                                else
                                {
                                    // Tried to use probablity to hit the draw as a proxy for the semi-bluff probability for non-committed drawing hands, but pot-equity calculations against really strong lines were way too optimistic
                                    (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b] = 0;
                                    /*collective[handHistory.PostflopActions[street][i].PlayerName].draw_matrix[a][b] = min(handHistory.drawing_outs[outs[a][b]][0] * 
                                        semi_bluff_multiplier * collective[handHistory.PostflopActions[street][i].PlayerName].draw_matrix[a][b], 1.0f);*/
                                }
                            }
                        }

                        if (debug_mode)
                        {
                            if (handHistory.PostflopActions[street][i].SAction == "Bets")
                                msg = String.Format("{0} ({1}) bet -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PostflopActions[street][i].PlayerName] as Player).Position], handHistory.PostflopActions[street][i].PlayerName, GetLabelNumber((collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range / 2 * 100, 0, null));
                            else if (handHistory.PostflopActions[street][i].SAction == "Raises")
                                msg = String.Format("{0} ({1}) raised -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PostflopActions[street][i].PlayerName] as Player).Position], handHistory.PostflopActions[street][i].PlayerName, GetLabelNumber((collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range / 2 * 100, 0, null));
                        }
                    }
                    else if (handHistory.PostflopActions[street][i].SAction == "Calls" || handHistory.PostflopActions[street][i].SAction == "Checks")
                    {
                        // Normalize the semi-bluff probability by game theoretic bluff probability
                        float optimal_bluff_p = temp_advice.optimal_bluff_range / (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range;
                        float semi_bluff_multiplier = 1.0f;
                        if (raw_semibluff_p > 0) semi_bluff_multiplier = optimal_bluff_p / raw_semibluff_p;

                        // If strategic situation warrant more aggressive play (e.g. c-bets), use wider hand range for hand reading too
                        float temp_raise_range = temp_advice.optimal_raise_range;
                        if (strategic_aggression > 0) temp_raise_range = 1 - (1 - temp_raise_range) * (1 - strategic_aggression);

                        // Calculate the new range
                        (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range =
                            Math.Min(temp_raise_range, (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range) +
                            Math.Min(temp_advice.call_range, (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range);

                        // Update drawing hand matrix, and make sure the semi-bluff probability is in line with game theoretic bluffing probability
                        float implied_bet = (float)((Math.Sqrt(2.0f) - 1) * basics.Players_ip + 2 * (Math.Sqrt(2.0f) - 1) * basics.Players_oop) / (basics.Players_ip + basics.Players_oop); // Extra money to be won if the draw hits (between 0.41 and 0.83 as a multiple of the pot)
                        for (int a = 0; a < 52; a++)
                        {
                            for (int b = 0; b < 52; b++)
                            {
                                if (a == b) continue;
                                if (hand_percentile[a, b] <= temp_advice.optimal_raise_range - temp_advice.optimal_bluff_range) (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b] = 0.0f; // Value-raise hand that did not raise -> weight to zero
                                else if ((street == 1 && GameRules.drawing_outs[outs[a, b], 1] >= all_in_calls / (float)(all_in_calls + basics.Pot + (all_in_calls - basics.To_call))) ||
                                         (street == 2 && GameRules.drawing_outs[outs[a, b], 0] >= all_in_calls / (float)(all_in_calls + basics.Pot + (all_in_calls - basics.To_call))))
                                {
                                    (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b] = Math.Max(
                                        (1.0f - semi_bluff_multiplier) * (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b], 0.0f);
                                }
                                else if (GameRules.drawing_outs[outs[a, b], 0] < basics.To_call / (float)((basics.Pot + basics.To_call) * (1.0f + implied_bet)))
                                {
                                    // Other drawing hands (not worth a call) -> weight to zero
                                    (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).draw_matrix[a, b] = 0.0f;
                                }
                                /*else if (handHistory.drawing_outs[outs[a][b]][0] >= basics.to_call/(float)((basics.pot+basics.to_call)*(1.0f+implied_bet)))
                                {
                                    // Tried to use probablity to hit the draw as a proxy for the semi-bluff probability for non-committed drawing hands, but pot-equity calculations against really strong lines were way too optimistic
                                    collective[handHistory.PostflopActions[street][i].PlayerName].draw_matrix[a][b] = max(
                                        (1.0f - handHistory.drawing_outs[outs[a][b]][0] * semi_bluff_multiplier) *
                                        collective[handHistory.PostflopActions[street][i].PlayerName].draw_matrix[a][b], 0.0f); // Never fold a draw that has sufficient implied odds to call and see 1 more card
                                }
                                else collective[handHistory.PostflopActions[street][i].PlayerName].draw_matrix[a][b] = 0.0f;*/
                                // Other drawing hands (not worth a call) -> weight to zero
                            }
                        }

                        if (debug_mode)
                        {
                            if (handHistory.PostflopActions[street][i].SAction == "Checks")
                                msg = String.Format("{0} ({1}) checked -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PostflopActions[street][i].PlayerName] as Player).Position], handHistory.PostflopActions[street][i].PlayerName, GetLabelNumber(((collective[handHistory.PostflopActions[street][i].PlayerName]) as hand_distribution).hand_range / 2 * 100, 0, null));
                            else if (handHistory.PostflopActions[street][i].SAction == "Calls")
                                msg = String.Format("{0} ({1}) called -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PostflopActions[street][i].PlayerName] as Player).Position], handHistory.PostflopActions[street][i].PlayerName, GetLabelNumber(((collective[handHistory.PostflopActions[street][i].PlayerName]) as hand_distribution).hand_range / 2 * 100, 0, null));
                        }
                    }
                    else if (handHistory.PostflopActions[street][i].SAction == "Folds")
                    {
                        //squeeze_fold_p *= 1-min(max(temp_advice.optimal_raise_range,temp_advice.call_range)/hand_range[handHistory.PreflopActions[i].PlayerName], 1.0f);
                        squeeze_fold_p *= 1 - Math.Min(Math.Max(temp_advice.optimal_raise_range, temp_advice.call_range) / (collective[handHistory.PostflopActions[street][i].Attacker] as hand_distribution).hand_range, 1.0f);
                        // Using above calculation instead, since we can't assume all other players to call based on EV and then take the remaining responsibility of pot defence, or otherwise the call-range would be way too loose
                        // More correct way is to assume that the other players would defend ~41% of the time too (against pot sized bets) assuming the attacker raised 100% of his range
                    }

                    // For Villains' actions, show hand reading %-range in the "advice" column
                    if (handHistory.PostflopActions[street][i].PlayerName != handHistory.HeroName)
                    {
                        advice = String.Format("~{0}%", (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range / 2 * 100);
                    }


                    //CHANGE HAND RANGE
                    //GHADY SEARCH POSTFLOP
                    if (handHistory.PostflopActions[street][i].PlayerName != handHistory.HeroName)
                    {
                        bool isFirstActionOnStreetForOpponent = false;
                        foreach (Action postflopAction in handHistory.PostflopActions[street])
                        {
                            if (postflopAction.PlayerName.Equals(handHistory.PostflopActions[street][i].PlayerName))
                            {
                                isFirstActionOnStreetForOpponent = postflopAction == handHistory.PostflopActions[street][i];
                                break;
                            }
                        }
                        if (isFirstActionOnStreetForOpponent)
                        {
                            //Plus if there is 3 to a flush on board, lets multiply by 1.2
                            //[9:50:52 PM] Ace Poker Solutions: Same w paired board, but by 1.3
                            bool threeToAFlush = handHistory.CommunityCards[1][1].Equals(handHistory.CommunityCards[1][3])
                                || handHistory.CommunityCards[1][1].Equals(handHistory.CommunityCards[1][5])
                                || handHistory.CommunityCards[1][3].Equals(handHistory.CommunityCards[1][5]);

                            int nbRaisesPreflop = 0;
                            foreach (Action preflopAction in handHistory.PreflopActions)
                            {
                                if (preflopAction.SAction.Equals("Raises"))
                                    nbRaisesPreflop++;
                            }
                            bool _4BetPreflop = nbRaisesPreflop == 3;


                            bool pairedBoard = handHistory.CommunityCards[1][0].Equals(handHistory.CommunityCards[1][2])
                                || handHistory.CommunityCards[1][0].Equals(handHistory.CommunityCards[1][4])
                                || handHistory.CommunityCards[1][2].Equals(handHistory.CommunityCards[1][4]);

                            if (_4BetPreflop)
                            {
                                (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range *= ModMultOpponent4BetPreflop;
                            }

                            if (threeToAFlush)
                            {
                                (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range *= ModMultOpponentThreeToAFlush;
                            }

                            if (pairedBoard)
                            {
                                (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range *= ModMultOpponentPairedBoard;
                            }

                            bool IsMinAmount = false;
                            float mult = PotIsReraisedPreflop(handHistory, out IsMinAmount) ? ModMultOpponentReRaised : PotIsUnraisedPreflop(handHistory) ? ModMultOpponentUnraised : ModMultOpponentSingleRaised;

                            (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range *= mult;

                            if ((collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range > 1)
                                (collective[handHistory.PostflopActions[street][i].PlayerName] as hand_distribution).hand_range = 1;
                        }
                    }
                    //

                }

                /*if (handHistory.flop_actions[i].PlayerName == handHistory.HeroName) hero_action_counter ++;
                if (handHistory.flop_actions[i].PlayerName == handHistory.HeroName) swprintf(buf_player, 128, "<%i> %s", hero_action_counter, handHistory.flop_actions[i].PlayerName);
                else*/
                player = String.Format("{0} - {1}", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PostflopActions[street][i].PlayerName] as Player).Position], handHistory.PostflopActions[street][i].PlayerName);
                action = String.Format("{0}", handHistory.PostflopActions[street][i].SAction);
                if (handHistory.PostflopActions[street][i].Amount > 0)
                {
                    if ((handHistory.PostflopActions[street][i].Amount % 100) == 0)
                    {
                        if (handHistory.PostflopActions[street][i].SAction == "Raises")
                            amount = ((handHistory.PostflopActions[street][i].Amount + Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.PostflopActions[street][i].PlayerName])) / 100).ToString(); // Raise To
                        else
                            amount = (handHistory.PostflopActions[street][i].Amount / 100).ToString();
                    }
                    else
                    {
                        if (handHistory.PostflopActions[street][i].SAction == "Raises")
                            amount = (handHistory.PostflopActions[street][i].Amount + Convert.ToDouble(handHistory.PostflopActions[street][i].ThisStreetCommitment[handHistory.PostflopActions[street][i].PlayerName]) / 100.0f).ToString(); // Raise To
                        else amount = (handHistory.PostflopActions[street][i].Amount / 100.0f).ToString();
                    }
                }
                else amount = "";
                int hh_formatting = 0;
                if (handHistory.PostflopActions[street][i].SAction == "Folds" || handHistory.PostflopActions[street][i].SAction == "Returns") hh_formatting = (int)GameRules.format_types.FORMAT_FOLD;
                else if (handHistory.PostflopActions[street][i].SAction == "Calls" ||
                         handHistory.PostflopActions[street][i].SAction == "Checks") hh_formatting = (int)GameRules.format_types.FORMAT_CALL;
                else if (handHistory.PostflopActions[street][i].SAction == "Raises" ||
                         handHistory.PostflopActions[street][i].SAction == "Bets") hh_formatting = (int)GameRules.format_types.FORMAT_RAISE;

                //pFrame->AddListBoxMessage(buf_player, buf_action, buf_amount, buf_advice, hh_formatting, handHistory.PostflopActions[street][i].PlayerName == handHistory.HeroName);
            }

            //missed_bets_street[street].Add(missed_bets - missed_bets_before);
            return collective; // Return hashtable of the players' estimated hand percentiles, drawing ranges, and a matrix of remaining drawing hands
        }

        PreAction decisionSummary(HandHistory handHistory, Action action)
        {
            PreAction summary = new PreAction();
            summary.Pot = 0;
            summary.TotalAnte = 0;
            summary.To_call = 0;
            summary.Eff_stacks = 0;
            summary.Players_ip = 0;
            summary.Players_oop = 0;

            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                summary.Pot += Math.Min((int)(Convert.ToDouble(action.LastStreetCommitment[key]) + Convert.ToDouble(action.ThisStreetCommitment[key])), (handHistory.Players[action.PlayerName] as Player).StartingStack); // Don't count other players' side pots

                if (PlayerIsInPreflop(handHistory, key))
                    summary.Pot += (int)(handHistory.Ante);
                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                if (key == action.PlayerName) continue; // Skip hero

                if (inPosition(handHistory, action.PlayerName, key)) summary.Players_oop++;
                else summary.Players_ip++;

                if ((handHistory.Players[key] as Player).StartingStack > summary.Eff_stacks) summary.Eff_stacks = (handHistory.Players[key] as Player).StartingStack;
                if (Convert.ToDouble(action.ThisStreetCommitment[key]) > summary.To_call) summary.To_call = (int)Convert.ToDouble(action.ThisStreetCommitment[key]);
            }
            summary.Eff_stacks = Math.Min(summary.Eff_stacks, (handHistory.Players[action.PlayerName] as Player).StartingStack);
            summary.To_call -= (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]);

            return summary;
        }

        suckout equity_to_outs(float ev, int street)
        {
            suckout result = new suckout();
            result.outs = 0;
            result.prob[0] = 0;
            result.prob[1] = 0;

            if (street < 1 || street > 2) return result; // Invalid street (only flop and turn are valid streets for out calculations)
            if (ev <= 0) return result; // No equity -> no outs

            for (int outs = 1; outs < 20; outs++)
            {
                if (ev <= GameRules.drawing_outs[outs, 2 - street])
                {
                    float a = (GameRules.drawing_outs[outs, 2 - street] - ev) / (GameRules.drawing_outs[outs, 2 - street] - GameRules.drawing_outs[outs - 1, 2 - street]);
                    result.potOdds = GameRules.drawing_outs[outs, 2 - street];

                    result.outs = a * (outs - 1) + (1 - a) * outs;
                    result.prob[0] = a * GameRules.drawing_outs[outs - 1, 0] + (1 - a) * GameRules.drawing_outs[outs, 0];
                    result.prob[1] = a * GameRules.drawing_outs[outs - 1, 1] + (1 - a) * GameRules.drawing_outs[outs, 1];
                    return result;
                }
            }
            result.outs = 19.0f;
            result.prob[0] = GameRules.drawing_outs[19, 0];
            result.prob[1] = GameRules.drawing_outs[19, 1];
            return result; // 19+ outs
        }

        bool PlayerCheckedBeforeAction(HandHistory handHistory, String playerName, Action action)
        {
            foreach (Action postflopAction in GetAllPostflopActionsOnStreetBefore(handHistory, action))
            {
                if (postflopAction.PlayerName.Equals(playerName) && postflopAction.SAction.Equals("Checks"))
                {
                    return true;
                }
            }
            return false;
        }

        postflop_advice postflop_raise(HandHistory handHistory, Action action, Hashtable collective_range, float squeeze_fold_p, List<String> advancedAdvices)//,out float postflop_raise_betsize)
        {
            if (action.PlayerName == handHistory.HeroName)
            {
            }
            postflop_advice post_advice = new postflop_advice();
            post_advice.debug = "";

            post_advice.gametheory_call = 0; // By default game theory calls are not used (since it only applies to pot defenders)

            // Find player's relative position, count the pot, to call -amount, effective stacks, and calculate collective hand range for opponents

            float collective_hand_range = 0;

            int players_oop = 0, players_ip = 0, eff_stacks = 0, pot = 0, to_call = 0;
            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                pot += (int)Math.Min((Convert.ToDouble(action.LastStreetCommitment[key]) + Convert.ToDouble(action.ThisStreetCommitment[key])), (handHistory.Players[action.PlayerName] as Player).StartingStack); // Don't count other players' side pots

                if (PlayerIsInPreflop(handHistory, key))
                    pot += (int)(handHistory.Ante);

                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                if (key == action.PlayerName) continue; // Skip hero

                String temp_debug = String.Format("{0} range: {1}", key, GetLabelNumber((collective_range[key] as hand_distribution).hand_range, 2, null));
                post_advice.debug += temp_debug;

                if (inPosition(handHistory, action.PlayerName, key)) players_oop++;
                else players_ip++;

                if ((handHistory.Players[key] as Player).StartingStack > eff_stacks) eff_stacks = (handHistory.Players[key] as Player).StartingStack;
                if (Convert.ToDouble(action.ThisStreetCommitment[key]) > to_call) to_call = (int)Convert.ToDouble(action.ThisStreetCommitment[key]);

                //collective_hand_range += 2.0008f/((collective_range[key] as hand_distribution).hand_range+0.0008f)-1; // How many random hands equals the range
                if (collective_hand_range == 0 || (collective_range[key] as hand_distribution).hand_range + 0.0008f < collective_hand_range) collective_hand_range = (collective_range[key] as hand_distribution).hand_range + 0.0008f;
            }
            eff_stacks = Math.Min(eff_stacks, (handHistory.Players[action.PlayerName] as Player).StartingStack);
            to_call -= (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]);
            //collective_hand_range = 2.0f/(collective_hand_range+1);

            //GHADY


            //ON FLOP ONLY: IF facing a bet from opponent that is 24% of the pot size or less,
            //then we ignore this as a bet from our opponent. We treat it as though our opponent
            //has checked. EX hand: #88282166 - 9s9d
            Action changedBetAction = null;
            if (action.Street == 1 && action.PlayerName == handHistory.HeroName)
            {
                if (action.Attacker != null && action.Attacker != "")
                {
                    foreach (Action actionBefore in GetAllPostflopActionsOnStreetBefore(handHistory, action))
                    {
                        if (actionBefore.SAction.Equals("Bets") && actionBefore.PlayerName.Equals(action.Attacker))
                        {
                            changedBetAction = actionBefore;
                        }
                    }
                    if (changedBetAction != null)
                    {
                        if (changedBetAction.Amount <= 0.24 * pot)
                        {
                            Debug("If facing a bet from opponent that is 24% of the pot size or less, then we ignore this as a bet from our opponent. We treat it as though our opponent has checked.");
                            changedBetAction.SAction = "Checks";
                            //changedBetAction.ThisStreetCommitment;
                        }
                    }
                }
            }

            //END GHADY


            // Debug the hand ranges
            //post_advice.debug.Format(L"%.2f vs. %.2f", collective_range[action.playerName], collective_hand_range);

            // How much stack would be left if we call
            int stack_left = (int)(eff_stacks - to_call - Convert.ToDouble(action.LastStreetCommitment[action.PlayerName]) - Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]));
            if (stack_left < 0) // If someone raised more than we have stack left
            {
                to_call += stack_left;
                action.AttackerRisk += stack_left;
                stack_left = 0;
            }

            // Apply pot odds to calling range (assuming a single opponent)
            if (to_call > 0) post_advice.call_range = collective_hand_range * (1 - 1.0f / (pot / (float)to_call + 1));
            else post_advice.call_range = 1; // If no bet, the whole hand range is the calling range

            // If player is not facing an all-in decision, and is not closing the action on the river
            if (stack_left > 0 && !(action.Street == 3 && action.Defender == action.PlayerName))
            {
                if (to_call > 0) post_advice.call_range *= (float)(2 * (Math.Sqrt(2.0f) - 1)); // Tighten up the calling range for the risk of getting raised (and for future betting)
                // Above formula should take into account how much there is left to call, since a bet of 99% of remaining stacks is practically the same as all-in

                // Apply position adjustment if at least sqrt(2) times the pot is still left for later
                if (stack_left - to_call >= Math.Sqrt(2.0f) * (pot + to_call))
                {
                    // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP
                    if (to_call > 0) post_advice.call_range *= (float)Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip));
                }
            }
            if (post_advice.call_range > 1.0f) post_advice.call_range = 1.0f;




            //

            // Game theory - call
            if (action.Defender == action.PlayerName) // Player is closing the action
            {
                float virtual_risk = (float)Math.Min(Math.Sqrt(2.0f) * to_call, (float)stack_left + to_call) - to_call; // Tighten up the range based on the risk of future betting (assuming enough stackis still left)
                virtual_risk *= players_ip / (float)(players_oop + players_ip); // Use tighter range only when playing OOP, because we can't allow other OOP players being able to turn the hand +EV for them 
                if (action.Street == 3 || stack_left == 0) virtual_risk = 0; // Don't tighter the range if this is the last action in the hand (i.e. we are either on the river, or we are making a decision for the rest of our stack)

                // Villain is technically risking only 86-100% of his bet since even his bluffs will have ~14% pot equity on the flop and 7% equity on the turn (although he might also get re-raised and have to fold, therefore using lower % when the raise is less than all-in)
                if (stack_left == 0) // All-in
                {
                    if (action.Street == 1) virtual_risk -= action.AttackerRisk * 0.14f; // Flop
                    else if (action.Street == 2) virtual_risk -= action.AttackerRisk * 0.07f; // Turn
                }
                else // Less than all-in
                {
                    if (action.Street == 1) virtual_risk -= action.AttackerRisk * 0.10f; // Flop
                    else if (action.Street == 2) virtual_risk -= action.AttackerRisk * 0.05f; // Turn
                }

                // Villain was technically risking 80-95% of his call in the previous street (if he called when behind)
                float virtual_last_call = (float)Convert.ToDouble(action.LastStreetCall[action.Attacker]);
                if (action.Street == 1) virtual_last_call *= 1 - 0.20f; // Preflop calls have at least 20% pot equity even if behind
                else if (action.Street == 2) virtual_last_call *= 1 - 0.10f; // Flop    calls have at least 10% pot equity even if behind
                else if (action.Street == 3) virtual_last_call *= 1 - 0.05f; // Turn    calls have at least  5% pot equity even if behind

                // If attackers first bet, raise or check-raise this street, then include his possible (virtual) call-amount from previous street in the current risk-calculation
                if (action.AttackerRisk == (int)Convert.ToDouble(action.ThisStreetCommitment[action.Attacker]))
                    post_advice.gametheory_call = 1 - (action.AttackerRisk + virtual_risk + virtual_last_call) / (pot + virtual_risk);
                else post_advice.gametheory_call = 1 - (action.AttackerRisk + virtual_risk) / (pot + virtual_risk);

                if (squeeze_fold_p > 0) post_advice.gametheory_call = Math.Max(1 - (1 - post_advice.gametheory_call) / squeeze_fold_p, 0.0f); // Multi-way game theory formula
            }

            // How much to raise, if we raise
            float bet_pot_multiple = (float)1.0; // Start from a pot sized raises
            // Calculate how many pot sized would have been needed to build the pot to it's current size (not perfectly accurate due varying preflop pot size, numer of players, etc)
            float pot_sized_bets = (float)(Math.Log((pot + to_call) / (float)((players_oop + players_ip + 1) * handHistory.BigBlindAmount + handHistory.SmallBlindAmount)) / Math.Log(players_oop + players_ip + 2.0f)); // Notice the (players+1) based logarithm
            bet_pot_multiple -= 0.2f * pot_sized_bets; // Deacrease the bet and raises sizes as the pot gets bigger
            if (bet_pot_multiple < Math.Sqrt(2.0f) - 1) bet_pot_multiple = (float)(Math.Sqrt(2.0f) - 1); // Minimum bet/raise in any situation is ~0.41xPot
            if (to_call > 0) // Additional rules for raises
            {
                bet_pot_multiple += 0.2f * players_ip / (float)(players_oop + players_ip); // Use larger *raises* from OOP
                if (action.Street == 3 && bet_pot_multiple < 0.6f) bet_pot_multiple = 0.6f; // Minimum raise on the river is ~0.6xPot
            }
            else // Additional rules for bets
            {
                // Strategic situation (c-bets, donk-bets, etc)
                int aggressor = 0; // 0 -> no aggressor, 1 -> hero is the aggressor, 2 -> villain is the aggressor
                if (action.Street == 1)
                {
                    for (int j = 0; j < handHistory.PreflopActions.Count; j++)
                    {
                        if (handHistory.PreflopActions[j].SAction == "Bets" || handHistory.PreflopActions[j].SAction == "Raises")
                        {
                            if (handHistory.PreflopActions[j].PlayerName == action.PlayerName) aggressor = 1;
                            else aggressor = 2;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < handHistory.PostflopActions[action.Street - 1].Count; j++)
                    {
                        if (handHistory.PostflopActions[action.Street - 1][j].SAction == "Bets" || handHistory.PostflopActions[action.Street - 1][j].SAction == "Raises")
                        {
                            if (handHistory.PostflopActions[action.Street - 1][j].PlayerName == action.PlayerName) aggressor = 1;
                            else aggressor = 2;
                        }
                    }
                }
                bool first_action = true;
                for (int j = 0; j < handHistory.PostflopActions[action.Street].Count; j++)
                    if (handHistory.PostflopActions[action.Street][j].PlayerName == action.PlayerName) first_action = false;

                // Adjust bet size based on the strategic situation
                if (first_action) // Player's first action for this street
                {
                    if (aggressor == 1) bet_pot_multiple += 0.2f * players_oop / (float)(players_oop + players_ip); // Larger bets from BTN for C-bets, 2nd barrels, etc.
                    else if (aggressor == 2) bet_pot_multiple += 0.2f * players_ip / (float)(players_oop + players_ip); // Larger donk-bets from OOP (because villain's hand range is stronger)
                }

                //if (action.street == 3) bet_pot_multiple += 0.2f * players_oop/(float)(players_oop+players_ip); // On the river use larger bets from the BTN because it re-opens the betting, NO because c-bets already have similar effect (though not exactly the same)
            }
            int standard_raise_risk = (int)Math.Round(to_call + (pot + to_call) * bet_pot_multiple); // Adjust the standard bet/raise size based on above considerations

            //GHADY POSTFLOP RAISE
            if (action.Street == 3 && handHistory.PostflopActions[3].Count == 2 && action.PlayerName == handHistory.HeroName)
            {
                String opponentPlayerName = handHistory.PostflopActions[3][0].PlayerName.Equals(handHistory.HeroName) ? handHistory.PostflopActions[3][1].PlayerName : handHistory.PostflopActions[3][0].PlayerName;
                Stats opponentStats = (handHistory.Players[opponentPlayerName] as Player).Stats;
                double multPot = 0;
                if (!Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                {
                    if (Math.Round(opponentStats.VPIP) >= 36 && Math.Round(opponentStats.VPIP) <= 44)
                    {
                        multPot = (double)3 / (double)5;
                    }
                    else if (Math.Round(opponentStats.VPIP) >= 45 && Math.Round(opponentStats.VPIP) <= 60)
                    {
                        multPot = (double)2 / (double)3;
                    }
                    else if (Math.Round(opponentStats.VPIP) >= 61)
                    {
                        multPot = (double)3 / (double)4;
                    }
                    standard_raise_risk = (int)Math.Round(to_call + (pot * (1 + multPot) + to_call) * bet_pot_multiple); // Adjust the standard bet/raise size based on above considerations
                }
            }
            // END GHADY


            float optimal_value_range = 0;
            float optimal_gametheory_range = 0;
            float actual_value_range = 0;
            float actual_gametheory_range = 0;

            // Player was technically risking 80-95% of his call in the previous street (if he called when behind)
            float virtual_last_calls = (float)Convert.ToDouble(action.LastStreetCall[action.PlayerName]);
            if (action.Street == 1) virtual_last_calls *= 1 - 0.20f; // Preflop calls have at least 20% pot equity even if behind
            else if (action.Street == 2) virtual_last_calls *= 1 - 0.10f; // Flop    calls have at least 10% pot equity even if behind
            else if (action.Street == 3) virtual_last_calls *= 1 - 0.05f; // Turn    calls have at least  5% pot equity even if behind

            // Debug the virtual calls
            //post_advice.debug.Format(L"Virtual call: %.2f", virtual_last_call);

            // Rough estimate of the board texture
            float board_texture = 0; // [0..1], where 0 = dry and 1 = wet
            if (action.Street == 1) // Flop
            {
                int suited = 0;
                if (handHistory.CommunityCards[1][1] == handHistory.CommunityCards[1][3] &&
                    handHistory.CommunityCards[1][3] == handHistory.CommunityCards[1][5]) suited = 3; // 3 suited cards
                else if (handHistory.CommunityCards[1][1] == handHistory.CommunityCards[1][3] ||
                    handHistory.CommunityCards[1][1] == handHistory.CommunityCards[1][5] ||
                    handHistory.CommunityCards[1][3] == handHistory.CommunityCards[1][5]) suited = 2; // 2 suited cards

                if (Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][0].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[1][2].ToString()]) == 1 ||
                    Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][0].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[1][4].ToString()]) == 1 ||
                    Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][2].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[1][4].ToString()]) == 1)
                {
                    // At least 2 connectors

                    if (suited == 3) board_texture = 0.75f;
                    else if (suited == 2) board_texture = 1.00f;
                    else board_texture = 0.25f;
                }
                else // No connectors
                {
                    if (suited == 3) board_texture = 0.50f;
                    else if (suited == 2) board_texture = 0.75f;
                    else board_texture = 0.00f;
                }
            }
            else if (action.Street == 2) // Turn
            {

                int suited = 0;
                if (handHistory.CommunityCards[1][1] == handHistory.CommunityCards[1][3] &&
                    handHistory.CommunityCards[1][3] == handHistory.CommunityCards[1][5] &&
                    handHistory.CommunityCards[1][5] == handHistory.CommunityCards[2][1]) suited = 4; // 4 suited cards
                else if (handHistory.CommunityCards[1][1] == handHistory.CommunityCards[1][3] &&
                    handHistory.CommunityCards[1][3] == handHistory.CommunityCards[1][5]) suited = 3; // 3 suited cards on the flop
                else if (handHistory.CommunityCards[1][1] == handHistory.CommunityCards[1][3] ||
                    handHistory.CommunityCards[1][1] == handHistory.CommunityCards[1][5] ||
                    handHistory.CommunityCards[1][3] == handHistory.CommunityCards[1][5]) suited = 2; // 2 suited cards on the flop

                if (Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][0].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[1][2].ToString()]) == 1 ||
                    Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][0].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[1][4].ToString()]) == 1 ||
                    Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][0].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[2][0].ToString()]) == 1 ||
                    Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][2].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[1][4].ToString()]) == 1 ||
                    Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][2].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[2][0].ToString()]) == 1 ||
                    Math.Abs((int)Card.CardValues[handHistory.CommunityCards[1][4].ToString()] - (int)Card.CardValues[handHistory.CommunityCards[2][0].ToString()]) == 1)
                {
                    // At least 2 connectors

                    if (suited == 4) board_texture = 0.25f;
                    else if (suited == 3) board_texture = 0.50f;
                    else if (suited == 2) board_texture = 0.75f;
                    else board_texture = 0.25f;
                }
                else // No connectors
                {
                    if (suited == 4) board_texture = 0.00f;
                    else if (suited == 3) board_texture = 0.25f;
                    else if (suited == 2) board_texture = 0.50f;
                    else board_texture = 0.00f;
                }
            }

            bool potSizeRaise = true;
            // Would the villain be able to push all-in for less than sqrt(2) times the pot if we make a standard pot sized raise?
            if (stack_left - to_call < (Math.Sqrt(2.0f) * 3 + 1) * (pot + to_call))
            {
                // If we have stacks left for less than 2*sqrt(2), or actually just sqrt(2) times the pot sized raise, then we should just move all-in (or fold)
                // Actually that criteria needs to be adjusted (2*sqrt(2) may be fine for wet flops, but not for turns, or dry flops, and especially not for river!)
                if (stack_left - to_call < /* 2* */ Math.Sqrt(2.0f) * (pot + to_call))
                {
                    // Calculate raise range based on all-in
                    optimal_value_range = collective_hand_range * (1 - 1.0f / (pot / (float)(to_call + stack_left) + 1));
                    actual_value_range = optimal_value_range; // Actual raise range is based on the assumption that the player is willing to risk the whole stack anyways
                    //if (to_call > 0) post_advice.call_range = optimal_value_range; // No calling, as we either move all-in or fold
                    // If we did use the all-in or fold rule (i.e. never called), then the stack criteria should be a lot lower than 2*sqrt(2), and we should set call_range = 0 to indicate no calling!

                    //float gametheory_range   = 2.0f/(players_oop+players_ip+1);
                    float gametheory_range = 0.5f * (collective_hand_range + 0.0008f) / ((collective_range[action.PlayerName] as hand_distribution).hand_range + 0.0008f);
                    // If players first bet, raise or check-raise this street, then include his possible (virtual) call-amount from previous street in the current risk-calculation
                    if (Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) == 0)
                        optimal_gametheory_range = gametheory_range * (1 - 1.0f / ((pot - virtual_last_calls) / (float)(to_call + stack_left + virtual_last_calls) + 1));
                    else optimal_gametheory_range = gametheory_range * (1 - 1.0f / (pot / (float)(to_call + stack_left) + 1));
                    actual_gametheory_range = optimal_gametheory_range; // Actual raise range is based on the assumption that the player is willing to risk the whole stack anyways

                    // Recommend all-in
                    post_advice.raise_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + to_call + stack_left; // "Raise to"
                    potSizeRaise = false;
                    //GHADY REDUCE POT SIZE RAISE
                    //post_advice.raise_size = reduceRaiseSize?(int)Math.Round((double)3 / (double)4 * post_advice.raise_size):post_advice.raise_size;
                }
                else // Otherwise make a pot sized raise, but use a tighter range due to the fact that we'd be committed to call the push too (although that might not apply on the river)
                {
                    // Calculate raise range based on all-in
                    //optimal_value_range = collective_hand_range * (1-1.0f/(pot/(float)(to_call+stack_left)+1));
                    optimal_value_range =
                        (1 - board_texture) * collective_hand_range * (1 - 1.0f / (pot / (float)(to_call + stack_left) + 1)) +
                        board_texture * collective_hand_range * (1 - 1.0f / (pot / (float)standard_raise_risk + 1)); // On wet boards don't worry about getting raised all-in, because villain might do it with a draw

                    // Calculate raise-range based on the actual raise size chosen by the player
                    actual_value_range = collective_hand_range * (1 - 1.0f / (pot / (float)action.Amount + 1));

                    //float gametheory_range   = 2.0f/(players_oop+players_ip+1);
                    float gametheory_range = 0.5f * (collective_hand_range + 0.0008f) / ((collective_range[action.PlayerName] as hand_distribution).hand_range + 0.0008f);
                    // If players first bet, raise or check-raise this street, then include his possible (virtual) call-amount from previous street in the current risk-calculation
                    if (Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) == 0)
                    {
                        optimal_gametheory_range = gametheory_range * (1 - 1.0f / ((pot - virtual_last_calls) / (float)(to_call + stack_left + virtual_last_calls) + 1));
                        actual_gametheory_range = gametheory_range * (1 - 1.0f / ((pot - virtual_last_calls) / (float)(action.Amount + virtual_last_calls) + 1));
                    }
                    else
                    {
                        optimal_gametheory_range = gametheory_range * (1 - 1.0f / (pot / (float)(to_call + stack_left) + 1));
                        actual_gametheory_range = gametheory_range * (1 - 1.0f / (pot / (float)(action.Amount) + 1));
                    }

                    //GHADY
                    /*also with that TT hand we had looked at before ona draw heavy board... we need to still increase the raise size.... hand #1918398751... hero has TT, the board is: 7 T 8
however it calculates raise size in this situation, it needs to be increase slightly
so if condition if true for draw heavy board, and hand strangth is top 5% or less, then the raise size needs to be larger
roughly 15% more than what it's currently recommended
                     */


                    if (action.PlayerName == handHistory.HeroName && BoardIsCoordinated(handHistory, 200, action.Street, true) && Math.Round(Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100) <= 5)
                    {
                        Debug("If draw heavy board, and hand strangth is top 5% or less, then the raise size needs to be larger roughly 15% more than what it's currently recommended");
                        standard_raise_risk = (int)Math.Round(standard_raise_risk + standard_raise_risk * 0.15);
                    }
                    //END GHADY

                    // Recommend pot sized raise (despite the range being tight enough for all-in) //ToCheck
                    post_advice.raise_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + standard_raise_risk; // "Raise to"
                }
            }
            else // If stacks are deep enough to ignore the above considerations
            {
                // Calculate raise range based on standard pot sized raise
                optimal_value_range = collective_hand_range * (1 - 1.0f / (pot / (float)standard_raise_risk + 1));
                //optimal_value_range *= 2*(sqrt(2.0f)-1); // Adjust by the risk of getting re-raised (and for future betting)		
                optimal_value_range *= (float)Math.Pow(2 * (Math.Sqrt(2.0f) - 1), 1 - board_texture); // Adjust by the risk of getting re-raised (and for future betting), but on dry boards only
                if (action.Street < 3) optimal_value_range *= (float)Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip)); // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP (does not apply on the river!)

                // Calculate raise-range based on the actual raise size chosen by the player
                actual_value_range = collective_hand_range * (1 - 1.0f / (pot / (float)action.Amount + 1));
                //actual_value_range *= 2*(sqrt(2.0f)-1); // Adjust by the risk of getting re-raised (and for future betting)
                actual_value_range *= (float)Math.Pow(2 * (Math.Sqrt(2.0f) - 1), 1 - board_texture); // Adjust by the risk of getting re-raised (and for future betting), but on dry boards only
                if (action.Street < 3) actual_value_range *= (float)Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip)); // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP (does not apply on the river!)

                //float gametheory_range   = 2.0f/(players_oop+players_ip+1);
                float gametheory_range = (float)(Math.Sqrt(2.0f) - 1) * (collective_hand_range + 0.0008f) / ((collective_range[action.PlayerName] as hand_distribution).hand_range + 0.0008f);
                // If players first bet, raise or check-raise this street, then include his possible (virtual) call-amount from previous street in the current risk-calculation
                if (Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) == 0)
                {
                    optimal_gametheory_range = gametheory_range * (1 - 1.0f / ((pot - virtual_last_calls) / (float)(standard_raise_risk + virtual_last_calls) + 1));
                    actual_gametheory_range = gametheory_range * (1 - 1.0f / ((pot - virtual_last_calls) / (float)(action.Amount + virtual_last_calls) + 1));
                }
                else
                {
                    optimal_gametheory_range = gametheory_range * (1 - 1.0f / (pot / (float)(standard_raise_risk) + 1));
                    actual_gametheory_range = gametheory_range * (1 - 1.0f / (pot / (float)(action.Amount) + 1));
                }
                // Recommend pot sized raise
                post_advice.raise_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + standard_raise_risk; // "Raise to"
                //GHADY REDUCE POT SIZE RAISE
                //post_advice.raise_size = (int)Math.Round((int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + ((double)2/(double)3)*standard_raise_risk); // "Raise to"
            }

            post_advice.optimal_raise_range = (players_oop * optimal_value_range + players_ip * optimal_gametheory_range) / (players_oop + players_ip);
            post_advice.actual_raise_range = (players_oop * actual_value_range + players_ip * actual_gametheory_range) / (players_oop + players_ip);



            //GHADY Postflop Raise added
            /*
            */
            #region Custom
            if (true || !skipCustom)
            {
                Player heroPlayer = handHistory.Players[handHistory.HeroName] as Player;

                boardinfo allBoardInfo = Jacob.AnalyzeHand(handHistory, action.Street, true);
                boardinfo boardInfo = Jacob.AnalyzeHand(handHistory, action.Street, false);

                if (action.PlayerName == handHistory.HeroName)
                {
                    Debug(action.Street == 1 ? "FLOP:" : action.Street == 2 ? "TURN:" : "RIVER:");
                }


                post_advice.bet_size = post_advice.raise_size;
                //GHADY REDUCE POT SIZE RAISE
                if (potSizeRaise)
                    post_advice.raise_size = (int)Math.Round(((double)3 / (double)4 * (pot + to_call))); // "Raise to"

                bool saveBet = false;
                bool shouldMultPot = false;

                double handRange = Math.Round(Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100);

                if ((action.Street == 1 || action.Street == 2) && action.PlayerName == handHistory.HeroName)
                {
                    //RULE for FLOP AND TURN ONLY
                    //If hand value is top 6% or less, AND the board is very draw heavy (the higher range of your draw heavy calculation), AND bet sizing facing hero is 40% of the pot or less, then RAISE.

                    //EX hand: #86885272 - ThTd
                    bool crdnt = BoardIsCoordinated(handHistory, 200, action.Street, false);

                    if (handRange <= 6 && BoardIsCoordinated(handHistory, 200, action.Street, false) && to_call <= 0.4 * pot)
                    {
                    }
                }

                #region FLOP
                if (action.Street == 1 && action.PlayerName == handHistory.HeroName)
                {
                    double multPot = 0;
                    //PAIRED BOARD RULE


                    //it should be the one that IF hero is the pre-flop raiser and there's no re-raise
                    //and the flop is single broadway
                    //A,K,Q, or J
                    //then bet 100% of the time on the flop

                    //let's also include this if they have small pocket pairs 88-22 unless they flop 3 of a kind... l
                    if (PlayerIsPreflopRaiser(handHistory, handHistory.Players[handHistory.HeroName] as Player, true))
                    {
                        if (FlopIsSingleBroadway(handHistory) && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action))
                        {
                            bool hasSmallPairs = heroPlayer.Cards[0].Equals(heroPlayer.Cards[2]);
                            bool hasThreeOfAKind = false;

                            if (hasSmallPairs)
                            {
                                if (heroPlayer.Cards[0].Equals(handHistory.CommunityCards[1][0]) ||
                                    heroPlayer.Cards[0].Equals(handHistory.CommunityCards[1][2]) ||
                                    heroPlayer.Cards[0].Equals(handHistory.CommunityCards[1][4]))
                                    hasThreeOfAKind = true;
                            }

                            if (allBoardInfo.madehand == postflophand.kNoPair || (hasSmallPairs && !hasThreeOfAKind))
                            {
                                bool minimumRaise = false;
                                if (!PotIsReraisedPreflop(handHistory, out minimumRaise))
                                {
                                    Debug("hero is the pre-flop raiser and there's no re-raise and the flop is single broadway -> BET 100% of the time");
                                    post_advice.custom_advice = "bet";
                                }
                            }
                        }
                    }

                    //If hand is heads-up, and board is paired, AND no one bet the flop, then Hero bet's 60% of the pot on the turn if first to act OR opponent has checked to hero.
                    //this is an example hand of it: #1918307834 - Ks8c
                    if (HeadsUp(handHistory, action) != null && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action) && (boardInfo.madehand == postflophand.kPair || boardInfo.madehand == postflophand.k2Pair || boardInfo.madehand == postflophand.k3ofKind || boardInfo.madehand == postflophand.k4ofKind))
                    {
                        multPot = 0.6;
                        Debug("If hand is heads-up, and board is paired, AND no one bet the flop, then Hero bet's 60% of the pot on the turn if first to act OR opponent has checked to hero.");
                        saveBet = true;
                        post_advice.custom_advice = "bet";
                    }

                    //Rule: If Hero is in position on the flop only, and HERO has a straight or flush draw, AND opponent bets the size of the pot or less, then hero calls
                    String sPosition = GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.HeroName] as Player).Position];

                    if (!sPosition.Equals("SB") && !sPosition.Equals("BB"))
                    {
                        bool opponentBetPotOrLess = false;
                        if (allBoardInfo.madehand == postflophand.kFlush || allBoardInfo.madehand == postflophand.kStraightFlush || allBoardInfo.madehand == postflophand.kStraight)
                        {
                            foreach (Action postflopAction in GetAllPostflopActionsOnStreetBefore(handHistory, action))
                            {
                                if (postflopAction.PlayerName.Equals(handHistory.HeroName)) continue;
                                if (postflopAction.SAction.Equals("Bets") && postflopAction.Amount <= pot)
                                {
                                    Debug("Rule: If Hero is in position on the flop only, and HERO has a straight or flush draw, AND opponent bets the size of the pot or less, then hero calls");
                                    post_advice.custom_advice = "Call";
                                }
                            }
                        }
                    }


                    bool raisedPreflop = false;
                    foreach (Action preflopAction in handHistory.PreflopActions)
                    {
                        if (preflopAction.PlayerName == handHistory.HeroName)
                        {
                            raisedPreflop = preflopAction.SAction.Equals("Raises");
                        }
                    }

                    if (!saveBet)
                    {
                        if (boardInfo.madehand == postflophand.kPair || boardInfo.madehand == postflophand.k2Pair || boardInfo.madehand == postflophand.k3ofKind || boardInfo.madehand == postflophand.k4ofKind)
                        {
                            multPot = (double)2 / (double)3;
                            saveBet = true;



                            //HERO RAISED PREFLOP? If yes, he should bet 100% of the time
                            if (raisedPreflop && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action))
                            {
                                Debug("HERO raised preflop + Is first to act or is checked to + PAIRED BOARD -> BET");
                                post_advice.custom_advice = "bet";
                            }
                        }
                    }

                    if (!saveBet)
                    {
                        //#49340892 - 7sTs : This should be a bet on the flop.
                        //When need a rule that if hero is the pre-flop raiser,
                        //and there is a single high card on the flop w/ any two other cards,/
                        //then hero is betting the flop as the pre-flop raiser 100% of the time.
                        //EX: flop is - Q 4 6, or K 2 9, or A 7 4, or J 9 2. 
                        //+ IS FIRST ACTION FOR HERO
                        //+ No PAIRS
                        //+HERO DID NOT MAKE A PAIR OR GREATER
                        if (raisedPreflop)
                        {
                            int nbHighCards = 0;
                            for (int i = 0; i <= 4; i += 2)
                            {
                                if (handHistory.CommunityCards[1][i].Equals('J') || handHistory.CommunityCards[1][i].Equals('Q') || handHistory.CommunityCards[1][i].Equals('K') || handHistory.CommunityCards[1][i].Equals('A'))
                                    nbHighCards++;
                            }

                            bool madePairs = false;
                            if (allBoardInfo.madehand == postflophand.kPair || allBoardInfo.madehand == postflophand.k2Pair || allBoardInfo.madehand == postflophand.k3ofKind || allBoardInfo.madehand == postflophand.k4ofKind)
                            {
                                madePairs = true;
                            }
                            if (!handHistory.CommunityCards[1][0].Equals(handHistory.CommunityCards[1][2])
                                && !handHistory.CommunityCards[1][0].Equals(handHistory.CommunityCards[1][4])
                                && !handHistory.CommunityCards[1][2].Equals(handHistory.CommunityCards[1][4])

                                &&
                                 nbHighCards == 1 && !madePairs)
                            {
                                if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action))
                                {
                                    Debug("if hero is the pre-flop raiser, and there is a single high card on the flop w/ any two other cards, then hero is betting the flop as the pre-flop raiser 100% of the time.");
                                    post_advice.custom_advice = "bet";
                                }
                            }
                        }
                    }

                    /*
                     * Bet sizing on the flop needs to be altered. If the hand value is =< top 3% then the bet sizing needs to increase to 70% of the pot. If it's 4 or 5%, then 66% of the pot. 
                    */
                    if (!saveBet)
                    {

                        if (!BoardIsCoordinated(handHistory, 200, action.Street, true))
                        {
                            if (handRange <= 3)
                            {
                                multPot = 0.7;
                                saveBet = true;
                            }
                            else if (handRange == 4 || handRange == 5)
                            {
                                multPot = 0.66;
                                saveBet = true;
                            }
                        }
                        else
                        {
                            if (handRange <= 3)
                            {
                                multPot = 0.8;
                                saveBet = true;
                            }
                            else if (handRange == 4 || handRange == 5)
                            {
                                multPot = 0.75;
                                saveBet = true;
                            }
                        }
                    }

                    //If hero is the pre-flop raiser and is facing 2 opponents or less on the flop,
                    //and has an OESD or flush draw, then bet 2/3rds of the pot. On the flop only.
                    if (!saveBet)
                    {
                        if (PlayerIsPreflopRaiser(handHistory, handHistory.Players[handHistory.HeroName] as Player, true))
                        {
                            if (GetActivePlayersNB(handHistory, action) - 1 <= 2)
                            {
                                if (allBoardInfo.madehand == postflophand.kFlush || allBoardInfo.madehand == postflophand.kStraightFlush || PlayerHasOESD(handHistory, handHistory.HeroName, action.Street))
                                {
                                    advancedAdvices.Add("This is generally a profitable semi-bluffing situation with your draw. Increase your EV in this hand by betting 2/3rds of the pot. The combination of your current equity, and fold equity in this hand makes it a profitable play.");
                                    saveBet = true;
                                    multPot = (double)2 / (double)3;
                                }
                            }
                        }
                    }

                    //If hero is pre-flop raiser, and we're heads up,
                    //we need to be betting the flop with flush draws and OESD
                    //#214806594 - QsAs
                    //#214828675 - KsKh
                    if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action)
                        && PlayerIsPreflopRaiser(handHistory, heroPlayer, false)
                        && GetActivePlayersNB(handHistory, action) == 2)
                    {
                        if (allBoardInfo.ifflushdraw || PlayerHasOESD(handHistory, handHistory.HeroName, action.Street))
                        {
                            Debug("If hero is pre-flop raiser, and we're heads up, we need to be betting the flop with flush draws and OESD.");
                            post_advice.custom_advice = "bet";
                        }
                    }

                    //Need to look at AF of opponents on an A high flop in a 3-bet pot.
                    //We should be c/f if agg is =<2.4.
                    //#214828675 - KsKh
                    if (PotIsReraisedPostflopBeforeAction(handHistory, action, true) //3Bet
                        && (handHistory.CommunityCards[1][0].Equals('A') || handHistory.CommunityCards[1][2].Equals('A') || handHistory.CommunityCards[1][4].Equals('A')
                        && boardInfo.madehand == postflophand.kNoPair))
                    {
                        List<Player> opponents = GetPlayersInHand(handHistory, action, false);
                        bool allOpponentHaveLowAgg = true;
                        foreach (Player opponent in opponents)
                        {
                            if (opponent.Stats.Agg > 2.4)
                            {
                                allOpponentHaveLowAgg = false;
                                break;
                            }
                        }

                        if (allOpponentHaveLowAgg)
                        {
                            Debug("Need to look at AF of opponents on an A high flop in a 3-bet pot. We should be c/f if agg is =<2.4.");
                            post_advice.custom_advice = PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action) ? "check" : "fold";
                        }
                    }



                    if (saveBet)
                        post_advice.bet_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + (int)Math.Round(to_call + (pot * (multPot) + to_call));
                }
                #endregion

                #region RIVER
                if (action.Street == 3 && action.PlayerName == handHistory.HeroName)
                {

                    //DEFAULT RIVER BET SIZING
                    double multPot = 0;

                    saveBet = true;
                    if (handRange <= 2)
                    {
                        multPot = 0.8;
                        if (AllOpponentsAre(handHistory, action, new String[] { "gambler", "whale" }))
                            multPot += 0.15;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "fish" }))
                            multPot += 0.08;
                    }
                    else if (handRange <= 6)
                    {
                        multPot = 0.72;
                        if (AllOpponentsAre(handHistory, action, new String[] { "gambler", "whale" }))
                            multPot += 0.10;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "fish" }))
                            multPot += 0.05;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "nit" }))
                            multPot -= 0.05;
                    }
                    else if (handRange <= 10)
                    {
                        multPot = 0.66;
                        if (AllOpponentsAre(handHistory, action, new String[] { "gambler", "whale" }))
                            multPot += 0.08;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "fish" }))
                            multPot += 0.04;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "nit" }))
                            multPot -= 0.06;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "tight tag" }))
                            multPot -= 0.04;
                    }
                    else if (handRange <= 16)
                    {
                        multPot = 0.60;
                        if (AllOpponentsAre(handHistory, action, new String[] { "whale" }))
                            multPot += 0.04;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "nit" }))
                            multPot -= 0.06;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "tight tag" }))
                            multPot -= 0.02;
                    }
                    else if (handRange <= 22)
                    {
                        multPot = 0.52;
                        if (AllOpponentsAre(handHistory, action, new String[] { "nit" }))
                            multPot -= 0.08;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "tight tag" }))
                            multPot -= 0.02;
                    }
                    else if (handRange <= 28)
                    {
                        multPot = 0.42;
                        if (AllOpponentsAre(handHistory, action, new String[] { "nit" }))
                            multPot -= 0.06;
                        else if (AllOpponentsAre(handHistory, action, new String[] { "tight tag" }))
                            multPot -= 0.02;
                    }
                    else saveBet = false;
                    if (saveBet)
                    {
                        Debug("Default Bet Sizing: " + multPot + " of the pot.");
                        post_advice.bet_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + (int)Math.Round(to_call + (pot * (multPot) + to_call));
                    }

                    if (false && GetActivePlayersNB(handHistory, action) == 2)
                    {
                        String opponentPlayerName = handHistory.PostflopActions[3][0].PlayerName.Equals(handHistory.HeroName) ? handHistory.PostflopActions[3][1].PlayerName : handHistory.PostflopActions[3][0].PlayerName;
                        Player opponentPlayer = handHistory.Players[opponentPlayerName] as Player;

                        if (!Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                        {
                            Stats opponentStats = (handHistory.Players[opponentPlayerName] as Player).Stats;
                            multPot = 0;

                            //int handRange = (int)Math.Round(Convert.ToDouble(handHistory.absolute_percentile[(handHistory.Players[handHistory.HeroName] as Player).Cards]) * 100);
                            String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, opponentPlayer).ToLower();
                            if (handRange <= 3 && (opponentModel.Equals("whale") || opponentModel.Equals("fish")))
                            {
                                Debug("2 Players on the river + handRange<=3 AND opponent is WHALE/FISH");
                                multPot = 0.8;
                                saveBet = true;
                            }
                            else if (Math.Round(opponentStats.VPIP) >= 36 && Math.Round(opponentStats.VPIP) <= 44)
                            {
                                Debug("2 Players on the river + VPIP>=36 AND VPIP<=44");

                                multPot = (double)3 / (double)5;
                                saveBet = true;
                            }
                            else if (Math.Round(opponentStats.VPIP) >= 45 && Math.Round(opponentStats.VPIP) <= 60)
                            {
                                Debug("2 Players on the river + VPIP>=45 AND VPIP<=60");

                                multPot = (double)2 / (double)3;
                                saveBet = true;
                            }
                            else if (Math.Round(opponentStats.VPIP) >= 61)
                            {
                                Debug("2 Players on the river + VPIP>=61");
                                multPot = (double)3 / (double)4;
                                saveBet = true;
                            }

                        }
                    }
                }
                #endregion

                #region TURN
                if (action.Street == 2 && action.PlayerName == handHistory.HeroName)
                {
                    //Heads up only... If pot is un-raised pre-flop, and flop is paired,
                    //AND no one bets on the flop, then hero bets the turn 63% of pot if first to act on the turn,
                    //or is checked to.
                    if (HeadsUpOrOneOpponent(handHistory, action) != null)
                    {
                        if (PotIsUnraisedPreflop(handHistory))
                        {
                            if (FlopIsPaired(handHistory))
                            {
                                if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action))
                                {
                                    post_advice.bet_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + (int)Math.Round(to_call + (pot * (0.63) + to_call));
                                    post_advice.custom_advice = "bet";
                                    Debug("Heads up only... If pot is un-raised pre-flop, and flop is paired, AND no one bets on the flop, then hero bets the turn 63% of pot if first to act on the turn, or is checked to.");
                                    advancedAdvices.Add("Since no one has shown interest in this paired board, then it's profitable for you to take a stab by betting slightly less than 2/3rds of the pot in spots like these.");
                                }
                            }
                        }


                        //
                    }


                    //new turn rule: If pot is raised, or re-rasied pre-flop,
                    //AND hero has OESD (open ended straight draw),
                    //and opponent checked to hero on the turn, then hero bets
                    bool potIsRaised = false;
                    foreach (Action preflopAction in handHistory.PreflopActions)
                    {
                        if (preflopAction.SAction.Equals("Raises"))
                        {
                            potIsRaised = true;
                            break;
                        }
                    }

                    if (potIsRaised)
                    {
                        if (PlayerHasOESD(handHistory, handHistory.HeroName, action.Street))
                        {
                            bool opponentCheckedToHero = false;
                            foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
                            {
                                if (postflopAction.Equals(action)) break;
                                if (!postflopAction.PlayerName.Equals(handHistory.HeroName))
                                    opponentCheckedToHero = postflopAction.SAction.Equals("Checks");
                            }
                            opponentCheckedToHero = opponentCheckedToHero && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action);

                            if (opponentCheckedToHero)
                            {
                                Debug("If pot is raised, or re-rasied pre-flop, AND hero has OESD (open ended straight draw), and opponent checked to hero on the turn, then hero bets");
                                post_advice.custom_advice = "bet";
                            }
                        }
                    }



                    double multPot = 0;
                    //We need to add logic that if hero has a pocket pair or any pair to the board, and the flop is checked, then hero bets the turn if hero has => 51% pot equity in the hand, and the board is now paired. EX: Hero has: 22  - board is  Q87Q - the flop was checked, and hero is first to act (or it can be checked to hero) - then hero bets 3/5ths of the pot.
                    if (FlopChecked_HeroBetsTurn_Paired_Scenario(action, handHistory, collective_range))
                    {
                        Debug("We need to add logic that if hero has a pocket pair or any pair to the board, and the flop is checked, then hero bets the turn if hero has => 51% pot equity in the hand, and the board is now paired. EX: Hero has: 22  - board is  Q87Q - the flop was checked, and hero is first to act (or it can be checked to hero) - then hero bets 3/5ths of the pot.");
                        multPot = (double)3 / (double)5;
                        saveBet = true;
                    }

                    //#1918295984 - Th9s : Rule - if hero is against one opponent on the flop and hero has an OESD
                    //on the flop or turn, the pot was not raised pre-flop, or it was just raised once,
                    //and both players check the flop. If hero is first to act, or opponent checks to hero,
                    //hero bets 2/3rds of the pot. 
                    if (!saveBet)
                    {
                        int nbOpponents = 0;
                        String opponentName = null;

                        foreach (String opponent in handHistory.Players.Keys)
                        {
                            if ((bool)handHistory.PostflopActions[1][0].InHand[opponent] && !opponent.Equals(handHistory.HeroName))
                            {
                                nbOpponents++;
                                opponentName = opponent;
                            }
                        }

                        //#1918295984 - Th9s : Rule - if hero is against one opponent on the flop and hero has an OESD
                        //on the flop or turn, the pot was not raised pre-flop, or it was just raised once,
                        //and both players check the flop. If hero is first to act, or opponent checks to hero,
                        //hero bets 2/3rds of the pot. 
                        if (nbOpponents == 1) //ONE OPPONENT ON THE FLOP
                        {
                            if (PlayerHasOESD(handHistory, handHistory.HeroName, 2)) //HAS OESD
                            {
                                int nbRaisesOnPreflop = 0;
                                foreach (Action preflopAction in handHistory.PreflopActions)
                                {
                                    if (preflopAction.SAction.Equals("Raises"))
                                    {
                                        nbRaisesOnPreflop++;
                                    }
                                }

                                if (nbRaisesOnPreflop <= 1) //NO OR ONLY ONE PREFLOP RAISE
                                {
                                    bool bothCheckedTheFlop = true;
                                    foreach (Action flopAction in handHistory.PostflopActions[1])
                                    {
                                        if (!flopAction.SAction.Equals("Checks"))
                                        {
                                            bothCheckedTheFlop = false;
                                            break;
                                        }
                                    }

                                    if (bothCheckedTheFlop)
                                    {
                                        if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action))
                                        {
                                            post_advice.custom_advice = "bet";
                                            multPot = (double)2 / (double)3;
                                            saveBet = true;
                                            advancedAdvices.Add("Since no one has shown interest in this pot, and you have an open ended straight draw with lots of outs, semi-bluffing is very profitable in this situation.");
                                            Debug("Rule - if hero is against one opponent on the flop and hero has an OESD on the flop or turn, the pot was not raised pre-flop, or it was just raised once, and both players check the flop. If hero is first to act, or opponent checks to hero, hero bets 2/3rds of the pot");
                                        }
                                    }
                                }
                            }
                        }



                        //If flop checked, and you turn a flush draw or OESD then bet
                        //#214820691 - 9cQc
                        bool heroCheckedFlop = false;
                        foreach (Action flopAction in handHistory.PostflopActions[1])
                        {
                            if (flopAction.PlayerName.Equals(handHistory.HeroName))
                                heroCheckedFlop = flopAction.SAction.Equals("Checks");
                        }
                        if (PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action)
                            && (allBoardInfo.ifflushdraw || PlayerHasOESD(handHistory, handHistory.HeroName, 2)))
                        {
                            Debug("If flop checked, and you turn a flush draw or OESD then bet");
                            post_advice.custom_advice = "bet";
                        }
                    }

                    //Bet sizing on the turn needs to be altered. If the hand value is =< top 3% then the bet sizing needs to increase to 70% of the pot. If it's 4 or 5%, then 66% of the pot. 
                    if (handRange <= 3)
                    {
                        Debug("TURN + HANDRANGE<=3: BET 70% of the pot");
                        multPot = 0.7;
                        saveBet = true;
                    }
                    else if (handRange == 4 || handRange == 5)
                    {
                        Debug("TURN + HANDRANGE=4 OR 5: BET 66% of the pot");
                        multPot = 0.66;
                        saveBet = true;
                    }

                    bool firstToAct = true;
                    foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
                    {
                        if (postflopAction.Equals(action))
                        {
                            break;
                        }
                        if (postflopAction.SAction.Equals("Bets"))
                        {
                            firstToAct = false;
                            break;
                        }
                    }

                    if (firstToAct)
                    {
                        Player opponentOnTurn = null;
                        List<String> playersOnTheTurn = new List<string>();
                        foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
                        {
                            if (!playersOnTheTurn.Contains(postflopAction.PlayerName))
                            {
                                playersOnTheTurn.Add(postflopAction.PlayerName);
                                if (postflopAction.PlayerName != handHistory.HeroName)
                                {
                                    opponentOnTurn = handHistory.Players[postflopAction.PlayerName] as Player;
                                }
                            }
                        }

                        if (playersOnTheTurn.Count == 2)
                        {
                            //GHADY if the hero is the first to act on the turn (or was checked to hero), and there are only 2 players + Board is coordinated + Opponent is Fish or Whale..hero bets 75% of the pot.

                            String opponentOnTurnModel = Player.GetPlayerModel(handHistory.Is6Max, opponentOnTurn).ToLower();

                            if (!Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                            {
                                if (!BoardIsCoordinated(handHistory, 100, action.Street, true))
                                {
                                    if ((opponentOnTurnModel.Equals("gambler") || opponentOnTurnModel.Equals("fish") || opponentOnTurnModel.Equals("whale")) && handRange >= 18 && handRange <= 24)
                                    {
                                        Debug("hero is the first to act on the turn (or was checked to hero), and there are only 2 players + Board is NOT coordinated + Opponent is Gambler/Fish/Whale + handRange>=18 AND handRange<=24: BET 50%");
                                        multPot = 0.5;
                                        saveBet = true;
                                    }
                                    else if ((opponentOnTurnModel.Equals("gambler") || opponentOnTurnModel.Equals("fish") || opponentOnTurnModel.Equals("whale")) && handRange >= 12 && handRange <= 17)
                                    {
                                        Debug("hero is the first to act on the turn (or was checked to hero), and there are only 2 players + Board is NOT coordinated + Opponent is Gambler/Fish/Whale + handRange>=12 AND handRange<17: BET 60%");
                                        multPot = 0.60;
                                        saveBet = true;
                                    }
                                    else if ((opponentOnTurnModel.Equals("gambler") || opponentOnTurnModel.Equals("fish") || opponentOnTurnModel.Equals("whale")) && handRange >= 6 && handRange <= 11)
                                    {
                                        Debug("hero is the first to act on the turn (or was checked to hero), and there are only 2 players + Board is NOT coordinated + Opponent is Gambler/Fish/Whale + handRange>=6 AND handRange<=11: BET 66%");
                                        multPot = 0.66;
                                        saveBet = true;
                                    }
                                    else if ((opponentOnTurnModel.Equals("gambler") || opponentOnTurnModel.Equals("fish") || opponentOnTurnModel.Equals("whale")) && handRange <= 5)
                                    {
                                        Debug("hero is the first to act on the turn (or was checked to hero), and there are only 2 players + Board is NOT coordinated + Opponent is Gambler/Fish/Whale + handRange<=5: BET 76%");
                                        multPot = 0.76;
                                        saveBet = true;
                                    }
                                }
                                else
                                {
                                    if (opponentOnTurnModel.Equals("fish") || opponentOnTurnModel.Equals("whale"))
                                    {
                                        Debug("hero is the first to act on the turn (or was checked to hero), and there are only 2 players + Board is coordinated + Opponent is Fish/Whale: BET 80%");
                                        multPot = 0.80;
                                        saveBet = true;
                                    }
                                    else
                                    {
                                        Debug("hero is the first to act on the turn (or was checked to hero), and there are only 2 players + Board is coordinated + Opponent is NOT Fish/Whale: BET 2/3");
                                        multPot = (double)2 / (double)3;
                                        saveBet = true;
                                    }
                                }
                            }
                        }
                    }

                    if (shouldMultPot || saveBet)
                        post_advice.bet_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + (int)Math.Round(to_call + (pot * (multPot) + to_call));
                }
                #endregion

                if (action.PlayerName == handHistory.HeroName)
                {
                    int heroStacks = GetPlayerStackOnStreet(handHistory, action.PlayerName, action);

                    post_advice.bet_size = post_advice.bet_size > heroStacks ? heroStacks : post_advice.bet_size;
                    post_advice.raise_size = post_advice.raise_size > heroStacks ? heroStacks : post_advice.raise_size;
                }
                //END GHADY



                // Calculate optimal bluff-range based on value-range and standard raise size
                post_advice.optimal_bluff_range = post_advice.optimal_raise_range * (standard_raise_risk - to_call) / (float)(standard_raise_risk - to_call + (pot + standard_raise_risk));
                // Calculate bluff-range based on value-range and the actual raise size chosen by player
                post_advice.actual_bluff_range = post_advice.actual_raise_range * (action.Amount - to_call) / (float)(action.Amount - to_call + (pot + action.Amount));




                //GHADY
                //SHOULD CHANGE BLUFFING RANGE
                if (action.PlayerName.Equals(handHistory.HeroName))
                {
                    if (action.Street == 3)
                    {
                        //ON RIVER ONLY: that is opponent is fish/whale...
                        //and there is 4 to a straight or 4 to a flush
                        Player playerAgainst = GetPlayerAgainstOrBetting(handHistory, action);
                        if (playerAgainst != null)
                        {
                            String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, playerAgainst).ToLower();
                            if (opponentModel.Equals("fish") || opponentModel.Equals("whale"))
                            {
                                if (PlayerHas4ToAFlush(handHistory, action.Street, false) || PlayerHas4ToAStraight(handHistory, action.Street, false))
                                {
                                    Debug("Rule: If opponent is fish/whale and there is 4 to a straight or 4 to a flush, multiply gametheory calc by 0.6");
                                    post_advice.gametheory_call *= 0.6F;
                                }
                            }
                        }

                        //if hand is top 16% or higher (16-100) AND there is 4 to a straight on the board
                        // OR 4 to a flush (and hero doesn't have a flush or straight),
                        //then check, UNLESS Hero has a pocket pair that is an overpair
                        //to the board.
                        //#70521719 - JsAd
                        if (!HeroHasOverPair(handHistory, action.Street) && (PlayerHas4ToAStraight(handHistory, 3, false) || PlayerHas4ToAFlush(handHistory, 3, false)))
                        {
                            if (allBoardInfo.madehand != postflophand.kFlush && allBoardInfo.madehand != postflophand.kStraight && allBoardInfo.madehand != postflophand.kStraightFlush)
                            {
                                Debug("if hand is top 16% or higher (16-100) AND there is 4 to a straight on the board OR 4 to a flush (and hero doesn't have a flush or straight), then check, UNLESS Hero has a pocket pair that is an overpair to the board.");
                                if (handRange >= 16) post_advice.custom_advice = "check";
                            }
                        }

                        //Reduce calling and and raising range on RIVER ONLY against nits by .8.
                        if (action.Attacker != null)
                        {
                            Player attacker = handHistory.Players[action.Attacker] as Player;
                            if (Player.GetPlayerModel(handHistory.Is6Max, attacker).ToLower().Equals("nit"))
                            {
                                double multRanges = 0.8;
                                post_advice.optimal_raise_range *= (float)multRanges;
                                post_advice.actual_raise_range *= (float)multRanges;
                                post_advice.call_range *= (float)multRanges;
                                post_advice.optimal_bluff_range *= (float)multRanges;
                                post_advice.gametheory_call *= (float)multRanges;

                                Debug("Reduce calling and and raising range on RIVER ONLY against nits by .8.");
                            }
                        }

                        //REDUCE CALLING/RAISING RANGES WHEN OPPONENT IS CHECK/RAISING
                        Player opponentCheckRaising = null;
                        Hashtable opponentsChecking = new Hashtable();
                        foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
                        {
                            if (postflopAction.Equals(action)) break;
                            if (postflopAction.PlayerName.Equals(handHistory.HeroName)) continue;
                            if (postflopAction.SAction.Equals("Checks"))
                                opponentsChecking.Add(postflopAction.PlayerName, null);
                            else if (postflopAction.SAction.Equals("Raises"))
                            {
                                if (opponentsChecking.ContainsKey(postflopAction.PlayerName))
                                {
                                    opponentCheckRaising = handHistory.Players[postflopAction.PlayerName] as Player;
                                    break;
                                }
                            }
                        }

                        if (opponentCheckRaising != null)
                        {
                            Debug("REDUCE CALLING/RAISING RANGES WHEN OPPONENT IS CHECK/RAISING");
                            double multPot = 1;
                            String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, opponentCheckRaising).ToLower();

                            if (opponentModel.Equals("tight tag") || opponentModel.Equals("tag"))
                                multPot = 0.2;
                            else if (opponentModel.Equals("fish") || opponentModel.Equals("whale"))
                                multPot = 0.3;
                            else if (opponentModel.Equals("lag") || opponentModel.Equals("gambler"))
                                multPot = 0.35;


                            post_advice.OpponentCheckRaised = true;
                            post_advice.optimal_raise_range *= (float)multPot;
                            post_advice.actual_raise_range *= (float)multPot;
                            post_advice.call_range *= (float)multPot;
                            post_advice.optimal_bluff_range *= (float)multPot;
                        }

                        //If fish/whale bet on the river, then we decrease the calling and raising range by .7.
                        Player fishWhaleOpponentBettingOnRiver = null;
                        foreach (Action postflopAction in GetAllPostflopActionsOnStreetBefore(handHistory, action))
                        {
                            if (postflopAction.PlayerName.Equals(handHistory.HeroName)) continue;
                            if (postflopAction.SAction.Equals("Bets"))
                            {
                                Player opponent = handHistory.Players[postflopAction.PlayerName] as Player;
                                String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, opponent).ToLower();
                                if (opponentModel.Equals("fish") || opponentModel.Equals("whale"))
                                    fishWhaleOpponentBettingOnRiver = opponent;
                            }
                        }

                        if (fishWhaleOpponentBettingOnRiver != null && !PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action))
                        {
                            double multPot = 0.7;
                            post_advice.optimal_raise_range *= (float)multPot;
                            post_advice.actual_raise_range *= (float)multPot;
                            post_advice.call_range *= (float)multPot;
                            post_advice.optimal_bluff_range *= (float)multPot;

                            Debug("If fish/whale bet on the river, then we decrease the calling and raising range by .7.");

                            //When a fish/whale with an AF of 2.5 or less bets on the river, reduce hero's calling range by .8.
                            if (fishWhaleOpponentBettingOnRiver.CanUseStats && fishWhaleOpponentBettingOnRiver.Stats.AF <= 2.5)
                            {
                                post_advice.call_range *= 0.8F;
                                advancedAdvices.Add("The opponent you are against is fairly tight and also pretty fishy. You should tighten up your calling range slightly against these opponents, unless they start to show that they are more aggressive. Passive opponents are passive because they don't get adequate value from their good hands, and also very rarely bluff.");
                                Debug("When a fish/whale with an AF of 2.5 or less bets on the river, reduce hero's calling range by .8.");
                            }
                        }
                        //


                        {
                            //Against fish/whales on the river increase the raising range (first betting range only) by .8.
                            //Do not apply this if villain has already bet into hero.
                            bool allOpponentsAreFishOrWhales = true;
                            foreach (String opponentName in handHistory.Players.Keys)
                            {
                                if (opponentName.Equals(handHistory.HeroName)) continue;
                                if (!(bool)action.InHand[opponentName]) continue;
                                String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, handHistory.Players[opponentName] as Player).ToLower();
                                if (!opponentModel.Equals("fish") && !opponentModel.Equals("whale"))
                                {
                                    allOpponentsAreFishOrWhales = false;
                                    break;
                                }
                            }

                            if (allOpponentsAreFishOrWhales && PlayerIsFirstToActOrIsCheckedToPostFlop(handHistory, action))
                            {
                                double multPot = 1, multRanges = 1;
                                saveBet = false;
                                if (handRange == 0)
                                {
                                    //Debug("Against fish/whales on the river + handRange=0 -> 0.9 of the pot.");
                                    multPot = 0.9;
                                    saveBet = true;
                                }
                                else if (handRange <= 3)
                                {
                                    //Debug("Against fish/whales on the river + handRange<=3 -> 80% of the pot.");
                                    multPot = 0.8;
                                    saveBet = true;
                                }
                                else if (handRange <= 6)
                                {
                                    //Debug("Against fish/whales on the river + handRange<=6 -> 70% of the pot.");
                                    multPot = 0.7;
                                    saveBet = true;
                                }
                                else if (handRange <= 10)
                                {
                                    Debug("Against fish/whales on the river increase the raising range (x 1.4)");// + handRange<=10 -> 66% of the pot.");
                                    multPot = 0.66;
                                    saveBet = true;
                                    multRanges = 1.4;
                                }
                                else if (handRange <= 16)
                                {
                                    Debug("Against fish/whales on the river increase the raising range (x 1.3)");// + handRange<=16 -> 60% of the pot.");
                                    multPot = 0.60;
                                    saveBet = true;
                                    multRanges = 1.3;
                                }
                                else if (handRange <= 22)
                                {
                                    Debug("Against fish/whales on the river increase the raising range (x 1.2)");// + handRange<=22 -> 50% of the pot.");
                                    multPot = 0.50;
                                    saveBet = true;
                                    multRanges = 1.2;
                                }
                                else if (handRange <= 28)
                                {
                                    Debug("Against fish/whales on the river increase the raising range (x 1.1)");// + handRange<=28 -> 40% of the pot.");
                                    multPot = 0.40;
                                    saveBet = true;
                                    multRanges = 1.1;
                                }
                                saveBet = false;
                                if (saveBet)
                                    post_advice.bet_size = (int)Convert.ToDouble(action.ThisStreetCommitment[action.PlayerName]) + (int)Math.Round(to_call + (pot * (multPot) + to_call));


                                post_advice.optimal_raise_range *= (float)multRanges;
                                post_advice.actual_raise_range *= (float)multRanges;
                                post_advice.call_range *= (float)multRanges;
                                post_advice.optimal_bluff_range *= (float)multRanges;
                                post_advice.gametheory_call *= (float)multRanges;
                            }
                        }
                    }

                    bool potWasRaisedMinimumAmount = false;
                    if (PotIsReraisedPreflop(handHistory, out potWasRaisedMinimumAmount) && potWasRaisedMinimumAmount && action.Street != 3)
                    {
                        float multRanges = 0.75F;

                        Debug("POT IS RERAISED PREFLOP " + (potWasRaisedMinimumAmount ? "(+min raise)" : "") + ", Multiply ranges by " + multRanges);

                        post_advice.optimal_raise_range *= multRanges;
                        post_advice.actual_raise_range *= multRanges;
                        post_advice.call_range *= multRanges;
                        post_advice.optimal_bluff_range *= multRanges;
                        post_advice.gametheory_call *= multRanges;
                    }
                    //If hero's range =<2 then multiply by .9, =<5 .85, =< 7 .75, =<10 .65
                    else if (PotIsReraisedPreflop(handHistory, out potWasRaisedMinimumAmount) && handRange < 10.65)
                    {
                        int minRange = -1;
                        float multRanges = 1;
                        if (handRange <= 3)
                        {
                            multRanges = 0.92F;
                            minRange = 3;
                        }
                        else if (handRange <= 5)
                        {
                            multRanges = 0.85F;
                            minRange = 5;
                        }
                        else if (handRange <= 7)
                        {
                            multRanges = 0.76F;
                            minRange = 7;
                        }
                        else if (handRange <= 10)
                        {
                            multRanges = 0.75F;
                            minRange = 10;
                        }
                        else
                        {
                            multRanges = 0.68F;
                        }

                        Debug("Hero's range " + (minRange != -1 ? ("=< " + minRange.ToString()) : ">10") + " then multiply by " + multRanges.ToString());
                        post_advice.optimal_raise_range *= multRanges;
                        post_advice.actual_raise_range *= multRanges;
                        post_advice.call_range *= multRanges;
                        post_advice.optimal_bluff_range *= multRanges;
                        post_advice.gametheory_call *= multRanges;

                    }
                    else
                    {
                        //IF Hero still have players that have decisions left to act (and it's multi-way)
                        //then we need to tighten up the calling and raising ranges like we did for re-rasied pots

                        List<Player> inHandPlayers = GetPlayersInHand(handHistory, action, true);
                        bool isMultiWay = inHandPlayers.Count > 2;
                        if (isMultiWay)
                        {
                            List<Action> postflopActions = handHistory.PostflopActions[action.Street];
                            bool isLastToAct = HeroIsLastToActPostflop(handHistory, action);// postflopActions[postflopActions.Count - 1].PlayerName.Equals(handHistory.HeroName);

                            // Multi-way:  handRange>6 and handRange<=12 + Last to act -> 0.8
                            //handRange>6 and handRange<=12 + not Last to act -> 0.75
                            //handRange>12 + Last to act -> 0.70
                            //handRange>12 + Last to act -> 0.65
                            //and if multiway + whale is betting: we multiply ranges by 1.25
                            if (handRange > 6 && TempConfig.EnableMultiwayMultiplier)
                            {
                                float multRange = handRange <= 12 ? (isLastToAct ? (float)TempConfig.MultiwayMultiplier_Inf_Last : (float)TempConfig.MultiwayMultiplier_Inf_NotLast) : (isLastToAct ? (float)TempConfig.MultiwayMultiplier_Sup_Last : (float)TempConfig.MultiwayMultiplier_Sup_NotLast);
                                Debug("Multiway+handRange>6, Multiply ranges by " + multRange.ToString());

                                post_advice.optimal_raise_range *= multRange;
                                post_advice.actual_raise_range *= multRange;
                                post_advice.call_range *= multRange;
                                post_advice.optimal_bluff_range *= multRange;
                                post_advice.gametheory_call *= multRange; //added
                            }

                            //WHALE IS BETTING? INCREASE THE RANGES
                            bool isWhaleOpponentBetting = false;
                            foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
                            {
                                if (postflopAction.Equals(action)) break;
                                if (postflopAction.SAction.Equals("Raises") || postflopAction.SAction.Equals("Bets"))
                                {
                                    if (!Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                                        isWhaleOpponentBetting = Player.GetPlayerModel(handHistory.Is6Max, (handHistory.Players[postflopAction.PlayerName] as Player)).ToLower().Equals("whale");
                                }
                            }


                            if (isWhaleOpponentBetting)
                            {
                                float multRange = 1.25F;
                                Debug("Multiway+WhaleOpponentIsBetting, Multiply ranges by " + multRange.ToString());

                                post_advice.optimal_raise_range *= multRange;
                                post_advice.actual_raise_range *= multRange;
                                post_advice.call_range *= multRange;
                                post_advice.optimal_bluff_range *= multRange;
                                post_advice.gametheory_call *= multRange; //added
                            }
                        }
                    }




                    if (action.Street == 1)
                    {
                        //action.Amount
                        //to_call
                        //FLOP ONLY
                        //If hero has a set (3 of a kind using both hole cards), and there's a re-raise on the flop,
                        //then manually change raising range to 3% and calling range to 6%.
                        String heroCards = (handHistory.Players[handHistory.HeroName] as Player).Cards;
                        if (heroCards[0].Equals(heroCards[2])
                            && (heroCards[0].Equals(handHistory.CommunityCards[1][0]) ||
heroCards[0].Equals(handHistory.CommunityCards[1][2]) ||
heroCards[0].Equals(handHistory.CommunityCards[1][4]))
                            )
                        {
                            if (PotIsReraisedPostflopBeforeAction(handHistory, action, false))
                            {
                                Debug("If hero has a set (3 of a kind using both hole cards), and there's a re-raise on the flop, then manually change raising range to 3% and calling range to 6%.");
                                post_advice.optimal_raise_range = 0.03F;
                                post_advice.actual_raise_range = 0.03F;
                                post_advice.call_range = 0.06F;
                            }
                        }

                        //if HERO flops trips... this means if hero has 3 of a kind on the flop using only ONE of heros' hole cards
                        //then X the raising and calling range by 2
                        //#1918343816 - 5h4c
                        if (allBoardInfo.madehand == postflophand.k3ofKind
                            && !heroCards[0].Equals(heroCards[2])
                            )
                        {
                            Debug("if HERO flops trips... this means if hero has 3 of a kind on the flop using only ONE of heros' hole cards, then X the raising and calling range by 2.");
                            post_advice.optimal_raise_range *= 2;
                            post_advice.actual_raise_range *= 2;
                            post_advice.call_range *= 2;
                            post_advice.optimal_bluff_range *= 2;
                            post_advice.gametheory_call *= 2; //added
                        }

                        //#49330913 - 7d7s : Calling range when hero is not the pre-flop raiser,
                        //and is out of position needs to be slightly reduced. We likely need
                        //to multiply this by .8 for bet and raise because this should be a check / fold.
                        if (!PlayerIsPreflopRaiser(handHistory, handHistory.Players[handHistory.HeroName] as Player, false))
                        {
                            if (PlayerIsOutOfPosition(handHistory, handHistory.HeroName, action.Street))
                            {
                                //Debug("Calling range when hero is not the pre-flop raiser,and is out of position needs to be slightly reduced. We likely needto multiply this by .8 for bet and raise because this should be a check / fold");

                                //post_advice.optimal_raise_range *= 0.5F;
                                //post_advice.call_range *= 0.5F;
                                //post_advice.optimal_bluff_range *= 0.5F;
                            }
                        }
                    }
                    else if (action.Street == 2)
                    {
                        //NIT Bets on the turn, multiply calling and raising ranges by .7.
                        //IF heros' hand value is > 22%, then multiply by .5.
                        //EX: #100057066 - 7c7s
                        Player opponentBetting = GetOpponentBettingPostflop(handHistory, action);
                        if (opponentBetting != null && Player.GetPlayerModel(handHistory.Is6Max, opponentBetting).ToLower().Equals("nit"))
                        {
                            advancedAdvices.Add("When a really tight player like a NIT bets on the turn, they usually have a good hand. You should reduce your calling and raising ranges in situations like these.");
                            float multPot = 0.7F;
                            if (handRange > 22)
                            {
                                multPot = 0.5F;
                                Debug("NIT Bets on the turn + Hand Range>22%, multiply calling and raising ranges by .5");
                            }
                            else
                                Debug("NIT Bets on the turn, multiply calling and raising ranges by .7");

                            post_advice.optimal_raise_range *= multPot;
                            post_advice.actual_raise_range *= multPot;
                            post_advice.call_range *= multPot;
                            post_advice.optimal_bluff_range *= multPot;
                            post_advice.gametheory_call *= multPot;
                        }
                    }
                }


                if (post_advice.optimal_raise_range > 1) post_advice.optimal_raise_range = 1;
                if (post_advice.actual_raise_range > 1) post_advice.actual_raise_range = 1;
                if (post_advice.call_range > 1) post_advice.call_range = 1;
                if (post_advice.optimal_bluff_range > 1) post_advice.optimal_bluff_range = 1;
                if (post_advice.gametheory_call > 1) post_advice.gametheory_call = 1;
            }
            #endregion

            if (true)
            {
                double multTemp = (action.PlayerName.Equals(handHistory.HeroName) ? TempConfig.CorrectionAHero : TempConfig.CorrectionAOpp);

                if (!TempConfig.SkipCorrections)
                {
                    float orig = post_advice.optimal_raise_range;
                    post_advice.optimal_raise_range = orig;

                    //post_advice.optimal_raise_range *= (float)Math.Pow((players_oop + players_ip + 1) / 2.0, multTemp);
                    post_advice.optimal_raise_range = (float)Math.Pow(post_advice.optimal_raise_range, 1 + (players_oop + players_ip - 1) * multTemp);

                    post_advice.actual_raise_range = (float)Math.Pow(post_advice.actual_raise_range, 1 + (players_oop + players_ip - 1) * multTemp);

                    //post_advice.call_range *= (float)Math.Pow((players_oop + players_ip + 1) / 2.0, multTemp);
                    post_advice.call_range = (float)Math.Pow(post_advice.call_range, 1 + (players_oop + players_ip - 1) * multTemp);
                    if (action.PlayerName.Equals(handHistory.HeroName))
                    {

                    }

                    //post_advice.gametheory_call *= (float)Math.Pow((players_oop + players_ip + 1) / 2.0, multTemp);
                    post_advice.gametheory_call = (float)Math.Pow(post_advice.gametheory_call, 1 + (players_oop + players_ip - 1) * multTemp);

                    //post_advice.optimal_bluff_range *= (float)Math.Pow((players_oop + players_ip + 1) / 2.0, multTemp);
                    post_advice.optimal_bluff_range = (float)Math.Pow(post_advice.optimal_bluff_range, 1 + (players_oop + players_ip - 1) * multTemp);
                }
            }

            //NEW CODE TO ADD 2
            if (!TempConfig.SkipCorrections)
            {
                if (post_advice.optimal_raise_range > 0 && post_advice.optimal_raise_range < 1)
                {
                    double logit = (action.PlayerName.Equals(handHistory.HeroName) ? TempConfig.CorrectionBHero : TempConfig.CorrectionBOpp) * Math.Log(post_advice.optimal_raise_range / (1 - post_advice.optimal_raise_range)); // Find a better value for 0.8
                    post_advice.optimal_raise_range = (float)(Math.Exp(logit) / (Math.Exp(logit) + 1));
                }
                if (post_advice.actual_raise_range > 0 && post_advice.actual_raise_range < 1)
                {
                    double logit = (action.PlayerName.Equals(handHistory.HeroName) ? TempConfig.CorrectionBHero : TempConfig.CorrectionBOpp) * Math.Log(post_advice.actual_raise_range / (1 - post_advice.actual_raise_range)); // Find a better value for 0.8
                    post_advice.actual_raise_range = (float)(Math.Exp(logit) / (Math.Exp(logit) + 1));
                }

                int str = action.Street;
                if (post_advice.call_range > 0 && post_advice.call_range < 1)
                {
                    if (!TempConfig.SkipCorrections)
                    {
                        double logit = (action.PlayerName.Equals(handHistory.HeroName) ? TempConfig.CorrectionBHero : TempConfig.CorrectionBOpp) * Math.Log(post_advice.call_range / (1 - post_advice.call_range)); // Find a better value for 0.8
                        post_advice.call_range = (float)(Math.Exp(logit) / (Math.Exp(logit) + 1));
                    }
                }
            }

            if (changedBetAction != null)
            {
                changedBetAction.SAction = "Bets";
                if (post_advice.custom_advice != null && post_advice.custom_advice.Equals("bet"))
                    post_advice.custom_advice = "raise";
            }

            return post_advice;
        }

        bool FlopChecked_HeroBetsTurn_Paired_Scenario(Action action, HandHistory handHistory, Hashtable collective_range)
        {
            return false;
            //GHADY
            /*
             * We need to add logic that if hero has a pocket pair or any pair to the board, and the flop is checked, then hero bets the turn if hero has => 51% pot equity in the hand, and the board is now paired. EX: Hero has: 22  - board is  Q87Q - the flop was checked, and hero is first to act (or it can be checked to hero) - then hero bets 3/5ths of the pot.
             * 
            */

            if (action.PlayerName.Equals(handHistory.HeroName))
            {
                Player heroPlayer = handHistory.Players[handHistory.HeroName] as Player;
                bool flopChecked = true;
                foreach (Action flopAction in handHistory.PostflopActions[1])
                {
                    if (!flopAction.SAction.Equals("Checks"))
                    {
                        flopChecked = false;
                        break;
                    }
                }

                if (flopChecked)
                {
                    if (heroPlayer.Cards[0].Equals(heroPlayer.Cards[2])
                        || heroPlayer.Cards[0].Equals(handHistory.CommunityCards[1][0])
                        || heroPlayer.Cards[0].Equals(handHistory.CommunityCards[1][2])
                        || heroPlayer.Cards[0].Equals(handHistory.CommunityCards[1][4])

                        || heroPlayer.Cards[1].Equals(handHistory.CommunityCards[1][0])
                        || heroPlayer.Cards[1].Equals(handHistory.CommunityCards[1][2])
                        || heroPlayer.Cards[1].Equals(handHistory.CommunityCards[1][4]))
                    {
                        if (IsTablePaired(handHistory, action.Street))
                        {
                            //Get Turn Hero Action
                            Action turnHeroAction = new Action();
                            foreach (Action turnAction in handHistory.PostflopActions[2])
                            {
                                if (turnAction.PlayerName.Equals(handHistory.HeroName))
                                {
                                    turnHeroAction = turnAction;
                                    break;
                                }
                            }

                            if (turnHeroAction != action) return false;
                            pot_equity potEquityHero = equitySimulation2(handHistory, turnHeroAction, collective_range, 10000, 2, false);
                            if (potEquityHero.postflop_equity_hup >= 0.51)
                            {
                                //double multPot = (double)3 / (double)5;
                                //standard_raise_risk = (int)Math.Round(to_call + (pot * (1 + multPot) + to_call) * bet_pot_multiple); // Adjust the standard bet/raise size based on above considerations
                                return true;

                            }
                        }
                    }
                }
            }
            return false;

            //END GHADY
        }


        internal Hashtable PreflopAnalysis(HandHistory handHistory)
        {
            Debug("PREFLOP:");
            missed_bets_street.Clear();
            missed_bets_final.Clear();

            missed_bets_street.Add(new List<int>()); //PREFLOP
            missed_bets_street.Add(new List<int>()); //FLOP
            missed_bets_street.Add(new List<int>()); //TURN
            missed_bets_street.Add(new List<int>()); //RIVER

            missed_bets_final.Add(new List<int>()); //PREFLOP
            missed_bets_final.Add(new List<int>()); //FLOP
            missed_bets_final.Add(new List<int>()); //TURN
            missed_bets_final.Add(new List<int>()); //RIVER

            advice_street.Clear();
            advice_street.Add(new List<String>()); //PREFLOP
            advice_street.Add(new List<String>()); //FLOP
            advice_street.Add(new List<String>()); //TURN
            advice_street.Add(new List<String>()); //RIVER

            // Hero's name, hole cards, and starting stack
            Player heroPlayer = handHistory.Players[handHistory.HeroName] as Player;
            String player = handHistory.HeroName;
            String action = heroPlayer.Cards;
            String msg = "";
            String amount = "";
            //if (heroPlayer.StartingStack % 100 == 0)
            amount = GetLabelNumber((double)heroPlayer.StartingStack / (double)100, 2, null);

            String advice = "";

            foreach (String key in handHistory.Players.Keys)
            {
                if (key != handHistory.HeroName)
                {
                    if (PlayerIsInPreflop(handHistory, key) && !PlayerFoldedOnPreflop(handHistory, key))
                    {
                    }
                }
            }

            // Show card graphics 

            Hashtable collective = new Hashtable();
            // Small Blind
            player = handHistory.SBName;
            action = "SB";
            //if ((handHistory.SmallBlindAmount % 100) == 0)
            amount = GetLabelNumber(handHistory.SmallBlindAmount / (double)100, 2, null);
            //else
            //    amount = (handHistory.SmallBlindAmount / 100).ToString("0.00");

            advice = "";

            // Big Blind
            player = handHistory.BBName;
            action = "BB";

            //if ((handHistory.BigBlindAmount % 100) == 0)
            amount = GetLabelNumber(handHistory.BigBlindAmount / (double)100, 2, null);
            //else
            //amount = (handHistory.BigBlindAmount / 100).ToString("0.00");

            advice = "";



            // Preflop
            player = "Preflop";
            action = "";

            if ((handHistory.PotSizeByStreet[0] % 100) == 0) amount = (handHistory.PotSizeByStreet[0] / 100.0).ToString();
            else amount = (handHistory.PotSizeByStreet[0] / 100.0).ToString("0.00");
            advice = "";

            int hero_action_counter = 0; // Intended to give an id number for each action if user clicks one

            Hashtable hand_range = new Hashtable(); // Estimated hand ranges for all players (everyone starts w/ 1.0 which means the whole range)
            foreach (String key in handHistory.Players.Keys)
            {
                hand_range.Add(key, 1);
            }

            float squeeze_fold_p = 1.0f; // Probability that all non-defenders fold (needed for calculating the optimal folding range in multi-way pots)

            // Preflop Actions
            int limpers = 0, raisers = 0, callers = 0;
            int /*max_commitment = handHistory.bigBlindAmount,*/ pot = (int)(handHistory.SmallBlindAmount + handHistory.BigBlindAmount + handHistory.Players.Count * handHistory.Ante);
            for (int i = 0; i < handHistory.PreflopActions.Count; i++)
            {
                int missed_bets_before = missed_bets;
                advice = "";
                String hand_analysis = "", situational_analysis = "", conclusion = "";

                PreAction basics = DecisionSummary(handHistory, handHistory.PreflopActions[i]); // Size of the pot, position, and other basic data

                if (!handHistory.PreflopActions[i].SAction.Equals("Returns")) // Returned is not a real action, but needs to be shown so that the (showdown) win amounts will make sense
                {
                    bool heroOpenRaisesFromButton = false;
                    // Is it Hero's turn
                    if (handHistory.PreflopActions[i].PlayerName == handHistory.HeroName)
                    {
                        HandStreetResult handResult = new HandStreetResult();
                        handResult.Street = 0;

                        hero_action_counter++;

                        int test_cases = 0;
                        //test_cases = 169;
                        Player HeroName = handHistory.Players[handHistory.HeroName] as Player;
                        for (int hypothetical = 0; hypothetical <= test_cases; hypothetical++)
                        {
                            if (hypothetical > 0) // In dev_mode we loop through hypothetical starting hands and analyze them all
                            {
                                int hypo1 = (hypothetical - 1) / 13;
                                int hypo2 = (hypothetical - 1) % 13;
                                if (hypo1 > hypo2) (handHistory.Players[handHistory.HeroName] as Player).Cards = (Card.AllCards[hypo1]) + "s" + Card.AllCards[hypo2] + "s";
                                else (handHistory.Players[handHistory.HeroName] as Player).Cards = Card.AllCards[hypo1] + "h" + Card.AllCards[hypo2] + "c";
                            }

                            hand_analysis = "<u>Hand Analysis</u>: " + handHistory.preflop_group((heroPlayer as Player).Cards) + "<br/><br/>";
                            situational_analysis = "<br/><u>Recommended Strategy</u>:<br/><br/>";
                            conclusion = "<p align=\"center\"><u>Advice</u>:";

                            bool pocket_pair = false, suited_connector = false, suited_gapper = false;
                            if (heroPlayer.Cards[0] == HeroName.Cards[2]) pocket_pair = true;
                            else if (heroPlayer.Cards[1] == heroPlayer.Cards[3]) // Suited
                            {
                                if ((int)Card.CardValues[heroPlayer.Cards[0].ToString()] == ((int)Card.CardValues[heroPlayer.Cards[2].ToString()]) + 1 ||
                                    (int)Card.CardValues[heroPlayer.Cards[2].ToString()] == ((int)Card.CardValues[heroPlayer.Cards[0].ToString()]) + 1) suited_connector = true;
                                else if ((int)Card.CardValues[heroPlayer.Cards[0].ToString()] == (int)Card.CardValues[heroPlayer.Cards[2].ToString()] + 2 ||
                                    (int)Card.CardValues[heroPlayer.Cards[2].ToString()] == (int)Card.CardValues[heroPlayer.Cards[0].ToString()] + 2) suited_gapper = true;
                            }

                            RaiseAdvice temp_advice;
                            if (raisers == 0) temp_advice = open_raise(handHistory, handHistory.PreflopActions[i]); // Unraised pot
                            else temp_advice = re_raise(handHistory, handHistory.PreflopActions[i], hand_range, squeeze_fold_p); // Raised pot


                            //GHADY
                            //If a fish/whale raise pre-flop (single raised pot),
                            //then we need to increase hero's calling range by the following. In position,
                            //when hero is not in the SB or BB, increase by 1.4. Out of position, (sb or bb),
                            //increase hero's calling percentage by 1.35.
                            //Against GAMBLER, increase hero's calling range in position by 1.2.
                            //Do not increase out of position.
                            if (raisers == 1)
                            {
                                Player raisePlayer = null;
                                foreach (Action preflopAction in handHistory.PreflopActions)
                                {
                                    if (preflopAction.SAction.Equals("Raises"))
                                    {
                                        raisePlayer = handHistory.Players[preflopAction.PlayerName] as Player;
                                        break;
                                    }
                                }
                                String raisePlayerModel = Player.GetPlayerModel(handHistory.Is6Max, raisePlayer).ToLower();
                                String sPosition = GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.HeroName] as Player).Position];
                                double multPot;
                                if (raisePlayerModel.Equals("fish"))
                                {
                                    multPot = sPosition.Equals("SB") || sPosition.Equals("BB") ? 1.4 : 1.45;
                                    temp_advice.CallRange *= (float)multPot;
                                    Debug("fish raised pre-flop (single raised pot) +  HERO IS " + (sPosition.Equals("SB") || sPosition.Equals("BB") ? " OUT OF POSITION" : "IN POSITION") + " -> Increase CALLING RANGE by " + multPot);
                                    handResult.AdvancedAdvices.Add("Against weak opponents, it's profitable to expand our calling ranges pre-flop. Looking for situations to get into pots consistently against the weaker players at the table is a highly EV play, as long as you're not over doing it and playing any two cards.");
                                }
                                else if (raisePlayerModel.Equals("whale"))
                                {
                                    multPot = sPosition.Equals("SB") || sPosition.Equals("BB") ? 1.45 : 1.55;
                                    temp_advice.CallRange *= (float)multPot;
                                    Debug("whale raised pre-flop (single raised pot) +  HERO IS " + (sPosition.Equals("SB") || sPosition.Equals("BB") ? " OUT OF POSITION" : "IN POSITION") + " -> Increase CALLING RANGE by " + multPot);
                                    handResult.AdvancedAdvices.Add("Against weak opponents, it's profitable to expand our calling ranges pre-flop. Looking for situations to get into pots consistently against the weaker players at the table is a highly EV play, as long as you're not over doing it and playing any two cards.");
                                }
                                else if (raisePlayerModel.Equals("gambler"))
                                {
                                    if (!sPosition.Equals("SB") && !sPosition.Equals("BB"))
                                    {
                                        temp_advice.CallRange *= 1.2F;
                                        Debug("gambler raised pre-flop (single raised pot) +  HERO IS IN POSITION -> Increase CALLING RANGE by 1.2");
                                        handResult.AdvancedAdvices.Add("Against weak opponents, it's profitable to expand our calling ranges pre-flop. Looking for situations to get into pots consistently against the weaker players at the table is a highly EV play, as long as you're not over doing it and playing any two cards.");
                                    }
                                }
                                else if (raisePlayerModel.Equals("tight tag"))
                                {
                                    temp_advice.CallRange *= 0.8F;
                                    temp_advice.ActualRaiseRange *= 0.8F;
                                    temp_advice.OptimalRaiseRange *= 0.8F;
                                    Debug("If opponent is tight tag, reduce calling and raising range by .8.");
                                }
                                else if (raisePlayerModel.Equals("nit"))
                                {
                                    temp_advice.CallRange *= 0.8F;
                                    temp_advice.ActualRaiseRange *= 0.8F;
                                    temp_advice.OptimalRaiseRange *= 0.8F;
                                    Debug("If opponent is NIT, reduce calling and raising range by .7.");
                                }

                                if (raisePlayerModel.Equals("tight tag") || raisePlayerModel.Equals("nit"))
                                {
                                }
                            }

                            //If an opponent raises the minimum amount pre-flop (this is double the bb amount). So if the BB amount is 2, the minimum is 4. If this occurs, then HERO's raising range should increase to =<6%. This is only for minimum. So we need extra logic to account for this, but keep the current logic in tact otherwise. Ex hand: #1918333616
                            //+ pot is not re-raised
                            bool potWasReraisedMinimumAmount = false;
                            int _3betAmount = 0;
                            if (!PotIsReraisedPreflopBeforeAction(handHistory, handHistory.PreflopActions[i], out potWasReraisedMinimumAmount, out _3betAmount))
                            {
                                foreach (Action preflopAction in handHistory.PreflopActions)
                                {
                                    if (preflopAction.PlayerName.Equals(handHistory.HeroName)) break;
                                    if (preflopAction.SAction.Equals("Raises") && preflopAction.Amount == handHistory.BigBlindAmount * 2)
                                    {
                                        temp_advice.OptimalRaiseRange += (float)0.06;
                                        Debug("If an opponent raises the minimum amount pre-flop (this is double the bb amount). So if the BB amount is 2, the minimum is 4. If this occurs, then HERO's raising range should increase to =<6%. This is only for minimum. So we need extra logic to account for this, but keep the current logic in tact otherwise.");
                                        //temp_advice.OptimalRaiseRange = temp_advice.OptimalRaiseRange * (float)(1.06);
                                        break;
                                    }
                                }
                            }
                            //GHADY

                            if (debug_mode)
                            {
                                if (raisers == 0) msg = String.Format("DEBUG: {0} ({1}): {2}% -> ${3} and {4}% -> Limp", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(temp_advice.OptimalRaiseRange * 100, 0, null), GetLabelNumber(temp_advice.RaiseSize / 100.0f, 2, null), GetLabelNumber(temp_advice.CallRange * 100, 0, null));
                                else msg = String.Format("DEBUG: {0} ({1}): {2}% -> ${3} and {4}% -> Call ({5}%)", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(temp_advice.OptimalRaiseRange * 100, 0, null), GetLabelNumber(temp_advice.RaiseSize / 100.0f, 1, null), GetLabelNumber(temp_advice.CallRange * 100, 0, null), GetLabelNumber(temp_advice.GametheoryCall * 100, 0, null));
                            }




                            situational_analysis = String.Format("{0}<bullet>  Raising range: Top {1}% (Raise to ${2})</bullet>", situational_analysis, GetLabelNumber(temp_advice.OptimalRaiseRange * 100, 0, null), GetLabelNumber(temp_advice.RaiseSize / 100, 2, null));
                            handResult.RaisingRangeTop = temp_advice.OptimalRaiseRange * 100;
                            handResult.RaisingRangeRaiseTo = temp_advice.RaiseSize / 100.0f;
                            handResult.RaisingRangeMsg = String.Format("Raising range: Top {0}% (Raise to ${1})", GetLabelNumber(temp_advice.OptimalRaiseRange * 100, 0, null), GetLabelNumber(temp_advice.RaiseSize / 100, 2, null));


                            if (temp_advice.CallRange > temp_advice.OptimalRaiseRange)
                            {
                                situational_analysis = String.Format("{0}<bullet>  Calling range: Top {1}%</bullet>", situational_analysis, GetLabelNumber(temp_advice.CallRange * 100, 0, null));
                                handResult.CallingRangeTop = temp_advice.CallRange * 100;
                                handResult.CallingRangeMsg = String.Format("Calling range: Top {0}%", GetLabelNumber(temp_advice.CallRange * 100, 0, null));
                            }

                            bool short_raise = false, short_call = false, short_easy_fold = false;
                            bool deep_jam = false, deep_raise = false, deep_call = false, deep_easy_fold = false;
                            bool medium_raise = false, medium_call = false, medium_easy_fold = false;

                            float jam_percentile = (float)HandHistory.jam_percentile[handHistory.preflop_group(heroPlayer.Cards)];
                            float deep_percentile = (float)HandHistory.deepstack_percentile[handHistory.preflop_group(heroPlayer.Cards)];
                            float short_percentile = (float)HandHistory.shortstack_percentile[handHistory.preflop_group(heroPlayer.Cards)];
                            float medium_percentile = (deep_percentile + short_percentile) / 2;

                            if (short_percentile <= temp_advice.OptimalRaiseRange) short_raise = true;
                            if (short_percentile <= temp_advice.CallRange) short_call = true;
                            if (short_percentile > 1.2 * temp_advice.CallRange) short_easy_fold = true;
                            if (jam_percentile <= temp_advice.OptimalRaiseRange) deep_jam = true;
                            if (deep_percentile <= temp_advice.OptimalRaiseRange) deep_raise = true;
                            if (deep_percentile <= temp_advice.CallRange) deep_call = true;
                            if (deep_percentile > 1.2 * temp_advice.CallRange) deep_easy_fold = true;

                            if (medium_percentile <= temp_advice.OptimalRaiseRange) medium_raise = true;
                            if (medium_percentile <= temp_advice.CallRange) medium_call = true;
                            if (medium_percentile > 1.2 * temp_advice.CallRange) medium_easy_fold = true;

                            bool implied_odds = false;
                            if ((pocket_pair && temp_advice.PocketPairOdds) ||
                                (suited_connector && temp_advice.SuitedConnectorOdds) ||
                                (suited_gapper && temp_advice.SuitedGapperOdds))
                            {
                                implied_odds = true;
                            }

                            bool short_gametheory_call = false, medium_gametheory_call = false, deep_gametheory_call = false;
                            if (raisers > 0) // Raised pot
                            {
                                if (short_percentile / Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) <= temp_advice.GametheoryCall) short_gametheory_call = true;
                                if (medium_percentile / Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) <= temp_advice.GametheoryCall) medium_gametheory_call = true;
                                if (deep_percentile / Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) <= temp_advice.GametheoryCall) deep_gametheory_call = true;
                            }

                            // Equity simulations
                            pot_equity ev_simulation = equitySimulation1(handHistory, handHistory.PreflopActions[i], hand_range, PostflopEquitySims, 0);


                            //NEW CODE TO ADD

                            // Calculate conditional collective hand ranges based on their calling ranges
                            Hashtable conditional_collective = new Hashtable();
                            int temp_bet = (int)(temp_advice.RaiseSize - Convert.ToDouble(handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]));
                            float all_fold_p = 1;

                            foreach (String key in hand_range.Keys)
                            {//street
                                //hand_distribution handDistribution = collective[key] as hand_distribution;

                                float fold_p = temp_bet / (float)(basics.Pot + temp_bet); // Replace with stats based fold probability (e.g. "fold-to-cbet" stats) when available
                                fold_p = (ev_simulation.postflop_equity_hup); //PotEquityOpponent

                                //conditional_collective.Add(key, float.Parse(hand_range[key]) * (1 - fold_p));

                                all_fold_p *= fold_p;
                            }
                            // Calculate conditional pot equity
                            //pot_equity ev_conditional = equitySimulation1(handHistory, handHistory.PreflopActions[i], conditional_collective, 10000, 0);

                            //float ev_call = ev_simulation.postflop_equity_all * basics.Pot - (1 - ev_simulation.postflop_equity_all) * basics.To_call;
                            //float ev_raise = all_fold_p * basics.Pot + (1 - all_fold_p) * (ev_conditional.postflop_equity_all * (basics.Pot + temp_bet - basics.To_call) - (1 - ev_conditional.postflop_equity_all) * temp_bet);
                            float ev_call = 0, ev_raise = 0;




                            //


                            // Heat Map Calculation (not exact, but approximate)
                            float[,] raise_heat = new float[13, 13];
                            float[,] call_heat = new float[13, 13];
                            for (int hole_i = 0; hole_i < 169; hole_i++)
                            {
                                int hole_x = hole_i / 13;
                                int hole_y = hole_i % 13;
                                String hole_cardstr;
                                if (hole_x > hole_y)
                                { // Suited
                                    hole_cardstr = Card.AllCards[hole_x] + "s" + Card.AllCards[hole_y] + "s";
                                    if (hole_x == hole_y + 1 && temp_advice.SuitedConnectorOdds) call_heat[hole_x, hole_y] += 2 / 3.0f; // Suited connectors w/ implied odds
                                    else if (hole_x == hole_y + 2 && temp_advice.SuitedGapperOdds)
                                        call_heat[hole_x, hole_y] += 1 / 3.0f; // Suited 1-gappers w/ implied odds
                                }
                                else
                                { // Offsuit
                                    hole_cardstr = Card.AllCards[hole_x] + "h" + Card.AllCards[hole_y] + "c";
                                    if (hole_x == hole_y && temp_advice.PocketPairOdds) call_heat[hole_x, hole_y] += 2 / 3.0f; // Pocket pair w/ implied odds
                                }

                                //if (handHistory.jam_percentile[hole_cardstr]        <= temp_advice.optimal_raise_range) raise_heat[hole_x][hole_y] += 1/3.0;
                                if (HandHistory.deepstack_percentile.ContainsKey(hole_cardstr) && (float)HandHistory.deepstack_percentile[hole_cardstr] <= temp_advice.OptimalRaiseRange) raise_heat[hole_x, hole_y] += 1 / 2.0f;
                                if (HandHistory.shortstack_percentile.ContainsKey(hole_cardstr) && (float)HandHistory.shortstack_percentile[hole_cardstr] <= temp_advice.OptimalRaiseRange) raise_heat[hole_x, hole_y] += 1 / 2.0f;

                                //if (handHistory.jam_percentile[hole_cardstr]        <= temp_advice.call_range) call_heat[hole_x][hole_y] += 1/3.0;
                                if (HandHistory.deepstack_percentile.ContainsKey(hole_cardstr) && (float)HandHistory.deepstack_percentile[hole_cardstr] <= temp_advice.CallRange) call_heat[hole_x, hole_y] += 1 / 2.0f;
                                if (HandHistory.shortstack_percentile.ContainsKey(hole_cardstr) && (float)HandHistory.shortstack_percentile[hole_cardstr] <= temp_advice.CallRange) call_heat[hole_x, hole_y] += 1 / 2.0f;
                            }
                            // Heat map has been calculated, next we need to print it out... still missing


                            if (debug_mode)
                            {
                                msg = String.Format("DEBUG: All opponents (random/deep/short): {0}% / {1}% / {2}%", GetLabelNumber(ev_simulation.random_equity_all * 100, 0, null), GetLabelNumber(ev_simulation.deep_equity_all * 100, 0, null), GetLabelNumber(ev_simulation.short_equity_all * 100, 0, null));
                                msg = String.Format("DEBUG: {0} (random/deep/short): {1}% / {2}% / {3}%", ev_simulation.strongest_man, GetLabelNumber(ev_simulation.random_equity_hup * 100, 0, null), GetLabelNumber(ev_simulation.deep_equity_hup * 100, 0, null), GetLabelNumber(ev_simulation.short_equity_hup * 100, 0, null));
                            }

                            if (temp_advice.StackDepth == GameRules.stack_depths.SHORT_STACK) // Short stack rankings
                            {
                                hand_analysis = String.Format("{0}<bullet>  Top {1}% hand based on pre-flop playability</bullet>", hand_analysis, short_percentile * 100);
                                handResult.TopHand = short_percentile * 100;
                                handResult.TopHandMsg = String.Format("Top {0}% hand based on pre-flop playability", short_percentile * 100);

                                if (ev_simulation.strongest_man != "")
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against {2}</bullet>", hand_analysis, GetLabelNumber(ev_simulation.short_equity_hup * 100, 0, null), ev_simulation.strongest_man);
                                    handResult.PotEquityOpponent = ev_simulation.short_equity_hup * 100;
                                    handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against {1}", GetLabelNumber(ev_simulation.short_equity_hup * 100, 0, null), ev_simulation.strongest_man);
                                }
                                else
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against one opponent</bullet>", hand_analysis, GetLabelNumber(ev_simulation.short_equity_hup * 100, 0, null));
                                    handResult.PotEquityOpponent = ev_simulation.short_equity_hup * 100;
                                    handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against one opponent", GetLabelNumber(ev_simulation.short_equity_hup * 100, 0, null));
                                }

                                int villains_left = GetActivePlayersNB(handHistory, handHistory.PreflopActions[i]) - 1;
                                if (villains_left > 1)
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against all {2} remaining opponents</bullet>", hand_analysis, GetLabelNumber(ev_simulation.short_equity_all * 100, 0, null), villains_left);
                                    handResult.PotEquityAll = ev_simulation.short_equity_all * 100;
                                    handResult.PotEquityAllMsg = String.Format("{0}% pot equity against all {1} remaining opponents", GetLabelNumber(ev_simulation.short_equity_all * 100, 0, null), villains_left);
                                }
                            }
                            else if (temp_advice.StackDepth == GameRules.stack_depths.DEEP_STACK) // Deep stack rankings
                            {
                                hand_analysis = String.Format("{0}<bullet>  Top {1}% hand based on post-flop playability</bullet>", hand_analysis, GetLabelNumber(deep_percentile * 100, 0, null));
                                handResult.TopHand = deep_percentile * 100;
                                handResult.TopHandMsg = String.Format("Top {0}% hand based on post-flop playability", GetLabelNumber(deep_percentile * 100, 0, null));

                                if (ev_simulation.strongest_man != "")
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against {2}</bullet>", hand_analysis, GetLabelNumber(ev_simulation.deep_equity_hup * 100, 0, null), ev_simulation.strongest_man);
                                    handResult.PotEquityOpponent = ev_simulation.deep_equity_hup * 100;
                                    handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against {1}", GetLabelNumber(ev_simulation.deep_equity_hup * 100, 0, null), ev_simulation.strongest_man);
                                }
                                else
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against one opponent</bullet>", hand_analysis, GetLabelNumber(ev_simulation.deep_equity_hup * 100, 0, null));
                                    handResult.PotEquityOpponent = ev_simulation.deep_equity_hup * 100;
                                    handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against one opponent", GetLabelNumber(ev_simulation.deep_equity_hup * 100, 0, null));
                                }


                                int villains_left = GetActivePlayersNB(handHistory, handHistory.PreflopActions[i]) - 1;
                                if (villains_left > 1)
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against all {2} remaining opponents</bullet>", hand_analysis, GetLabelNumber(ev_simulation.deep_equity_all * 100, 0, null), villains_left);
                                    handResult.PotEquityAll = ev_simulation.deep_equity_all * 100;
                                    handResult.PotEquityAllMsg = String.Format("{0}% pot equity against all {1} remaining opponents", GetLabelNumber(ev_simulation.deep_equity_all * 100, 0, null), villains_left);
                                }
                            }
                            else if (temp_advice.StackDepth == GameRules.stack_depths.MEDIUM_STACK) // Medium stack rankings
                            {
                                hand_analysis = String.Format("{0}<bullet>  Top {1}% hand based on pre-flop and post-flop playability</bullet>", hand_analysis, GetLabelNumber(medium_percentile * 100, 0, null));
                                handResult.TopHand = medium_percentile * 100;
                                handResult.TopHandMsg = String.Format("Top {0}% hand based on pre-flop and post-flop playability", GetLabelNumber(medium_percentile * 100, 0, null));

                                if (ev_simulation.strongest_man != "")
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against {2}</bullet>", hand_analysis, GetLabelNumber((ev_simulation.short_equity_hup + ev_simulation.deep_equity_hup) / 2 * 100, 0, null), ev_simulation.strongest_man);
                                    handResult.PotEquityOpponent = (ev_simulation.short_equity_hup + ev_simulation.deep_equity_hup) / 2 * 100;
                                    handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against {1}", GetLabelNumber((ev_simulation.short_equity_hup + ev_simulation.deep_equity_hup) / 2 * 100, 0, null), ev_simulation.strongest_man);
                                }
                                else
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against one opponent</bullet>", hand_analysis, GetLabelNumber((ev_simulation.short_equity_hup + ev_simulation.deep_equity_hup) / 2 * 100, 0, null));
                                    handResult.PotEquityOpponent = (ev_simulation.short_equity_hup + ev_simulation.deep_equity_hup) / 2 * 100;
                                    handResult.PotEquityOpponentMsg = String.Format("{0}% pot equity against one opponent", GetLabelNumber((ev_simulation.short_equity_hup + ev_simulation.deep_equity_hup) / 2 * 100, 0, null));
                                }


                                int villains_left = GetActivePlayersNB(handHistory, handHistory.PreflopActions[i]) - 1;
                                if (villains_left > 1)
                                {
                                    hand_analysis = String.Format("{0}<bullet>  {1}% pot equity against all {2} remaining opponents</bullet>", hand_analysis, GetLabelNumber((ev_simulation.short_equity_all + ev_simulation.deep_equity_all) / 2 * 100, 0, null), villains_left);
                                    handResult.PotEquityAll = (ev_simulation.short_equity_all + ev_simulation.deep_equity_all) / 2 * 100;
                                    handResult.PotEquityAllMsg = String.Format("{0}% pot equity against all {1} remaining opponents", GetLabelNumber((ev_simulation.short_equity_all + ev_simulation.deep_equity_all) / 2 * 100, 0, null), villains_left);
                                }
                            }

                            if (!short_easy_fold && (!deep_raise || !short_raise)) // If not an "easy fold", and not a "value raise"
                            {
                                if (HandHistory.trouble_hands.ContainsKey(handHistory.preflop_group(heroPlayer.Cards)) && (bool)HandHistory.trouble_hands[handHistory.preflop_group(heroPlayer.Cards)]) // Trouble hand (that is not as strong in deep stack play as it may appear)
                                {
                                    hand_analysis = String.Format("{0}<bullet>  Trouble hand that plays better with less money behind</bullet>", hand_analysis);
                                }
                                else if (HandHistory.semibluff_hands.ContainsKey(handHistory.preflop_group(heroPlayer.Cards)) && (bool)HandHistory.semibluff_hands[handHistory.preflop_group(heroPlayer.Cards)]) // Semi-bluffable hand (that plays better with deep stacks than hand rank suggests)
                                {
                                    hand_analysis = String.Format("{0}<bullet>  Drawing hand that plays better with more money behind</bullet>", hand_analysis);
                                }
                            }

                            if (raisers == 0) // Unraised pot
                            {
                                if (temp_advice.StackDepth == GameRules.stack_depths.SHORT_STACK) // Short stack rankings
                                {
                                    if (short_raise) // Raise
                                    {
                                        advice = "Raise";

                                        if (deep_raise)
                                        {
                                            conclusion = String.Format("{0}<b> Raise for value - Raise to ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Raise for value - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        }
                                        else
                                        {
                                            conclusion = String.Format("{0} <b> Raise due to short effective stacks - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            conclusion = String.Format("{0}<br/>(With deeper stacks this might not be worth a raise)</p>", conclusion);
                                            handResult.Advice = String.Format("Raise due to short effective stacks - Raise to ${0}\r\n(With deeper stacks this might not be worth a raise)", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        }

                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else // Don't raise
                                    {
                                        // We shouldn't allow open-calls though !!!

                                        if (basics.To_call == 0) // Checking is allowed (no calling or folding)
                                        {
                                            advice = "Check";
                                            conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                            handResult.Advice = "Check";
                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else
                                        {
                                            if (short_call) // Call
                                            {
                                                advice = "Call";

                                                if (deep_raise)
                                                {
                                                    conclusion = String.Format("{0} <b> Call due to short effective stacks<b/>", conclusion);
                                                    conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a raise)</p>", conclusion);

                                                    handResult.Advice = "Call due to short effective stacks\r\n(With deeper stacks this might be worth a raise)";
                                                }
                                                else
                                                {
                                                    if (deep_call)
                                                    {
                                                        conclusion = String.Format("{0} <b> Call</b></p>", conclusion);
                                                        handResult.Advice = "Call";
                                                    }
                                                    else
                                                    {
                                                        conclusion = String.Format("{0} <b> Marginal call</b></p>", conclusion);
                                                        handResult.Advice = "Marginal Call";
                                                    }
                                                }

                                                missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                            }
                                            else // Fold
                                            {
                                                if (limpers > 0 && implied_odds)
                                                {
                                                    advice = "Call";
                                                    conclusion = String.Format("{0} <b> Call due to high implied odds</b></p>", conclusion);
                                                    handResult.Advice = "Call due to high implied odds";
                                                    missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                                }
                                                else
                                                {
                                                    advice = "Fold";

                                                    if (deep_raise)
                                                    {
                                                        conclusion = String.Format("{0} <b> Fold due to short effective stacks</b>", conclusion);
                                                        conclusion = String.Format("{0}(With deeper stacks this might be worth a raise)</p>", conclusion);
                                                        handResult.Advice = "Fold due to short effective stacks\r\n(With deeper stacks this might be worth a raise)";
                                                    }
                                                    else if (deep_call)
                                                    {
                                                        conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                                        handResult.Advice = "Marginal fold";
                                                        if (limpers > 0)
                                                        {
                                                            conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a call)</p>", conclusion);
                                                            handResult.Advice += "\r\n(With deeper stacks this might be worth a call)";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*if (short_easy_fold && deep_easy_fold) conclusion=String.Format("{0} <b> Easy fold</b>", conclusion);
                                                        else*/
                                                        conclusion = String.Format("{0} <b> Fold</b></p>", conclusion);
                                                        handResult.Advice = "Fold";
                                                    }

                                                    missed_bets -= handHistory.PreflopActions[i].Amount;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (temp_advice.StackDepth == GameRules.stack_depths.DEEP_STACK) // Deep stack rankings
                                {
                                    if (deep_raise && short_raise)
                                    {
                                        advice = "Raise";
                                        conclusion = String.Format("{0} <b> Raise for value - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        handResult.Advice = String.Format("Raise for value - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (deep_raise && deep_jam)
                                    {
                                        advice = "Raise";
                                        conclusion = String.Format("{0} <b> Raise due to deep effective stacks - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        conclusion = String.Format("{0}<br/>(With shorter stacks this might not be worth a raise)</p>", conclusion);
                                        handResult.Advice = String.Format("Raise due to deep effective stacks - Raise to ${0}\r\n(With shorter stacks this might not be worth a raise)", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (deep_raise)
                                    {
                                        if (limpers >= 2) // If 2+ limpers already in the pot
                                        {
                                            advice = "Call";
                                            conclusion = String.Format("{0} <b> Limp behind to keep the pot small</b></p>", conclusion);
                                            handResult.Advice = "Limp behind to keep the pot small";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else if (limpers == 1) // If a single limper only in the pot
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Raise to isolate the open-limper</b></p>", conclusion);
                                            handResult.Advice = "Raise to isolate the open-limper";

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                        else // If unopened pot
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Marginal raise</b></p>", conclusion);
                                            handResult.Advice = "Marginal Raise";

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else if (deep_jam)
                                    {
                                        if (limpers >= 3) // If 3+ limpers already in the pot
                                        {
                                            advice = "Call";
                                            conclusion = String.Format("{0} <b> Limp behind to see a cheap flop</b></p>", conclusion);
                                            handResult.Advice = "Limp behind to see a cheap flop";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else // If unopened pot, or 1-2 limpers
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Raise as a semi-bluff</b></p>", conclusion);
                                            handResult.Advice = "Raise as a semi-bluff";

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else if (deep_call) // Call
                                    {
                                        // We shouldn't allow open-calls though !!!

                                        if (basics.To_call == 0) // Checking is allowed (no calling or folding)
                                        {
                                            advice = "Check";
                                            conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                            handResult.Advice = "Check";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else
                                        {
                                            advice = "Call";

                                            if (short_raise)
                                            {
                                                conclusion = String.Format("{0} <b> Call for pot control</b>", conclusion);
                                                conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a raise)</p>", conclusion);
                                                handResult.Advice = "Call for pot control \r\n(With shorter stacks this might be worth a raise)";
                                            }
                                            else
                                            {
                                                if (short_call)
                                                {
                                                    conclusion = String.Format("{0} <b> Call</b>", conclusion);
                                                    handResult.Advice = "Call";
                                                }
                                                else
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal call</b></p>", conclusion);
                                                    handResult.Advice = "Marginal Call";
                                                }
                                            }

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else // Fold
                                    {
                                        if (basics.To_call == 0) // Checking is allowed (no calling or folding)
                                        {
                                            advice = "Check";
                                            conclusion = String.Format("{0} <b> Check</b></p>", conclusion);
                                            handResult.Advice = "Check";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else
                                        {
                                            if (limpers > 0 && implied_odds)
                                            {
                                                advice = "Call";
                                                conclusion = String.Format("{0} <b> Call due to high implied odds</b></p>", conclusion);
                                                handResult.Advice = "Call due to high implied odds";

                                                missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                            }
                                            else
                                            {
                                                advice = "Fold";

                                                if (short_raise)
                                                {
                                                    conclusion = String.Format("{0} <b> Fold due to deep effective stacks</b>", conclusion);
                                                    conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a raise)", conclusion);
                                                    handResult.Advice = "Fold due to deep effective stacks \r\n(With shorter stacks this might be worth a raise)";
                                                }
                                                else if (short_call)
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                                    if (limpers > 0) conclusion = String.Format("{0}<br/>(With shorter stacks this might worth a call)</p>", conclusion);
                                                    handResult.Advice = "Marginal fold \r\n(With shorter stacks this might worth a call)";
                                                }
                                                else
                                                {
                                                    /*if (deep_easy_fold && short_easy_fold) conclusion=String.Format("{0} <b> Easy fold</b>", conclusion);
                                                    else*/
                                                    conclusion = String.Format("{0} <b> Fold</b>", conclusion);
                                                    handResult.Advice = "Fold";
                                                }

                                                missed_bets -= handHistory.PreflopActions[i].Amount;
                                            }
                                        }
                                    }
                                }
                                else if (temp_advice.StackDepth == GameRules.stack_depths.MEDIUM_STACK) // Medium stack rankings
                                {
                                    if (deep_raise && short_raise)
                                    {
                                        advice = "Raise";
                                        conclusion = String.Format("{0} <b> Raise for value - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        handResult.Advice = String.Format("Raise for value - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (medium_raise)
                                    {
                                        if (limpers >= 1) // If limpers already in the pot
                                        {
                                            if (basics.To_call == 0) // Checking is allowed (no calling or folding)
                                            {
                                                advice = "Check";
                                                conclusion = String.Format("{0} <b> Check</b>", conclusion);
                                                handResult.Advice = "Check";

                                                missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                            }
                                            else
                                            {
                                                advice = "Call";
                                                if (deep_raise)
                                                {
                                                    conclusion = String.Format("{0} <b> Limp behind to see a cheap flop</b>", conclusion);
                                                    handResult.Advice = "Limp behind to see a cheap flop";
                                                }
                                                else
                                                {
                                                    conclusion = String.Format("{0} <b> Limp behind to keep the pot small</b>", conclusion);
                                                    handResult.Advice = "Limp behind to keep the pot small";
                                                }

                                                missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                            }
                                        }
                                        else // If unopened pot
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Raise</b>", conclusion);
                                            handResult.Advice = "Raise";

                                            if (deep_raise)
                                            {
                                                conclusion = String.Format("{0}<br/>(With shorter stacks this might not be worth a raise)", conclusion);
                                                handResult.Advice += "\r\n(With shorter stacks this might not be worth a raise)";
                                            }
                                            else
                                            {
                                                conclusion = String.Format("{0}<br/>(With deeper stacks this might not be worth a raise)", conclusion);
                                                handResult.Advice += "\r\n(With deeper stacks this might not be worth a raise)";
                                            }

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else if (medium_call) // Call
                                    {
                                        // We shouldn't allow open-calls though !!!

                                        if (basics.To_call == 0) // Checking is allowed (no calling or folding)
                                        {
                                            advice = "Check";
                                            conclusion = String.Format("{0} <b> Check</b>", conclusion);
                                            handResult.Advice = "Check";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else
                                        {
                                            advice = "Call";

                                            if (deep_raise)
                                            {
                                                conclusion = String.Format("{0} <b> Call to see the flop</b>", conclusion);
                                                conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a raise)", conclusion);
                                                handResult.Advice = "Call to see the flop \r\n(With deeper stacks this might be worth a raise)";
                                            }
                                            else if (short_raise)
                                            {
                                                conclusion = String.Format("{0} <b> Call for pot control</b>", conclusion);
                                                conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a raise)", conclusion);
                                                handResult.Advice = "Call for pot control \r\n(With shorter stacks this might be worth a raise)";
                                            }
                                            else
                                            {
                                                if (deep_call && short_call)
                                                {
                                                    conclusion = String.Format("{0} <b> Call</b>", conclusion);
                                                    handResult.Advice = "Call";
                                                }
                                                else
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal call</b>", conclusion);
                                                    handResult.Advice = "Marginal Call";
                                                }
                                            }

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else // Fold
                                    {
                                        if (basics.To_call == 0) // Checking is allowed (no calling or folding)
                                        {
                                            advice = "Check";
                                            conclusion = String.Format("{0} <b> Check</b>", conclusion);
                                            handResult.Advice = "Check";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else
                                        {
                                            if (limpers > 0 && implied_odds)
                                            {
                                                advice = "Call";
                                                conclusion = String.Format("{0} <b> Call due to high implied odds</b>", conclusion);
                                                handResult.Advice = "Call due to high implied odds";

                                                missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                            }
                                            else
                                            {
                                                advice = "Fold";

                                                if (deep_raise)
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                                    conclusion = String.Format("{0}<br/>(With deeper stacks this might have been worth a raise)", conclusion);
                                                    handResult.Advice = "Marginal Fold \r\n(With deeper stacks this might have been worth a raise)";
                                                }
                                                else if (short_raise)
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                                    conclusion = String.Format("{0}<br/>(With shorter stacks this might have been worth a raise)", conclusion);
                                                    handResult.Advice = "Marginal Fold \r\n(With shorter stacks this might have been worth a raise)";
                                                }
                                                else if (deep_call)
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                                    handResult.Advice = "Marginal Fold";
                                                    if (limpers > 0)
                                                    {
                                                        conclusion = String.Format("{0}<br/>(With deeper stacks this might have been worth a call)", conclusion);
                                                        handResult.Advice += "\r\n(With deeper stacks this might have been worth a call)";
                                                    }
                                                }
                                                else if (short_call)
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                                    handResult.Advice = "Marginal Fold";
                                                    if (limpers > 0)
                                                    {
                                                        conclusion = String.Format("{0}<br/>(With shorter stacks this might have been worth a call)", conclusion);
                                                        handResult.Advice += "\r\n(With shorter stacks this might have been worth a call)";
                                                    }
                                                }
                                                else
                                                {
                                                    conclusion = String.Format("{0} <b> Fold</b>", conclusion);
                                                    handResult.Advice = "Fold";
                                                }

                                                missed_bets -= handHistory.PreflopActions[i].Amount;
                                            }
                                        }
                                    }
                                }

                                // Update hand range estimates (and the game theory variable)
                                if (handHistory.PreflopActions[i].SAction == "Raises" || handHistory.PreflopActions[i].SAction == "Bets")
                                {
                                    hand_range[handHistory.PreflopActions[i].PlayerName] = temp_advice.ActualRaiseRange;
                                    squeeze_fold_p = 1.0f;
                                }
                                else if (handHistory.PreflopActions[i].SAction == "Calls" || handHistory.PreflopActions[i].SAction == "Checks")
                                {
                                    hand_range[handHistory.PreflopActions[i].PlayerName] = temp_advice.OptimalRaiseRange + temp_advice.CallRange;
                                }
                                else if (handHistory.PreflopActions[i].SAction == "Folds")
                                {
                                    //squeeze_fold_p *= 1-max(temp_advice.optimal_raise_range, temp_advice.call_range);
                                    squeeze_fold_p *= 1 - Math.Min(Math.Max(temp_advice.OptimalRaiseRange, temp_advice.CallRange) / (float)Convert.ToDouble(hand_range[handHistory.PreflopActions[i].Attacker]), 1.0f);
                                    // Using above calculation instead, since we can't assume all other players to call based on EV and then take the remaining responsibility of pot defence, or otherwise the call-range would be way too loose
                                    // More correct way is to assume that the other players would defend ~41% of the time too (against pot sized bets) assuming the attacker raised 100% of his range
                                }
                            }
                            else // Raised pot
                            {

                                //GHADY
                                /*
                                 * This next issue is also fairly serious. If Hero raises pre-flop, and they are re-raised only the minimum amount, then they need to call (or the minimum amount + 25%).
                                 * So to figure the minimum amount, you need to take the big blind amount and subtract it from  the raise of HERO. Then take that amount and add it to HERO's raise.  If that equals the raise size of the opponent, then that's the minimum re-raise and should be a call. EX: Hero raises 3.5x the big blind to $7. The small blind re-raises to $12, and hero calls. So the big blind is $2. 2 - 7 (hero's raise) = 5. 5 + 7 (hero's raise size) = 12, which is the small blinds raise... 
                                */
                                bool heroRaised = false;
                                double heroRaise = 0;
                                foreach (Action preflopAction in handHistory.PreflopActions)
                                {
                                    if (heroRaised && preflopAction.SAction.Equals("Raises"))
                                    {
                                        double raiseAmount = preflopAction.Amount + Convert.ToDouble(preflopAction.ThisStreetCommitment[preflopAction.PlayerName]);
                                        if (2 * heroRaise - handHistory.BigBlindAmount == raiseAmount)
                                        {
                                            deep_call = short_call = true;
                                        }
                                    }
                                    if (preflopAction.PlayerName.Equals(handHistory.HeroName) && preflopAction.SAction.Equals("Raises"))
                                    {
                                        heroRaised = true;
                                        heroRaise = preflopAction.Amount + Convert.ToDouble(preflopAction.ThisStreetCommitment[preflopAction.PlayerName]);
                                    }
                                }

                                //GHADY
                                bool canRaise = CanRaisePreflop(handHistory, handHistory.PreflopActions[i]);
                                if (temp_advice.StackDepth == GameRules.stack_depths.SHORT_STACK) // Short stack rankings
                                {
                                    if (short_raise && canRaise) // Raise
                                    {
                                        advice = "Raise";

                                        if (deep_raise)
                                        {
                                            conclusion = String.Format("{0} <b> Raise for value - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Raise for value - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        }
                                        else
                                        {
                                            conclusion = String.Format("{0} <b> Raise due to short effective stacks - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            conclusion = String.Format("{0}<br/>(With deeper stacks this might not be worth a raise)", conclusion);
                                            handResult.Advice = String.Format("Raise due to short effective stacks - Raise to ${0}\r\n(With deeper stacks this might not be worth a raise)", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        }

                                        handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName] = 5300;
                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else // Don't raise
                                    {
                                        if (short_call) // Call
                                        {
                                            advice = "Call";

                                            if (deep_raise)
                                            {
                                                conclusion = String.Format("{0} <b> Call due to short effective stacks</b>", conclusion);
                                                conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a raise)", conclusion);
                                                handResult.Advice = "Call due to short effective stacks \r\n(With deeper stacks this might be worth a raise)";
                                            }
                                            else
                                            {
                                                if (deep_call)
                                                {
                                                    conclusion = String.Format("{0} <b> Call</b>", conclusion);
                                                    handResult.Advice = "Call";
                                                }
                                                else
                                                {
                                                    conclusion = String.Format("{0} <b> Marginal call</b>", conclusion);
                                                    handResult.Advice = "Marginal Call";
                                                }
                                            }

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else if (short_gametheory_call) // Game theory based call
                                        {
                                            advice = "Call";
                                            conclusion = String.Format("{0} <b> Call to avoid being exploitable</b>", conclusion);
                                            handResult.Advice = "Call to avoid being exploitable";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else if (implied_odds) // Implied odds call
                                        {
                                            advice = "Call";
                                            conclusion = String.Format("{0} <b> Call due to high implied odds</b>", conclusion);
                                            handResult.Advice = "Call due to high implied odds";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else // Fold
                                        {
                                            advice = "Fold";

                                            if (deep_raise)
                                            {
                                                conclusion = String.Format("{0} <b> Fold due to short effective stacks</b>", conclusion);
                                                conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a raise)", conclusion);
                                                handResult.Advice = "Fold due to short effective stacks \r\n(With deeper stacks this might be worth a raise)";
                                            }
                                            else if (deep_call)
                                            {
                                                conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                                conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a call)", conclusion);
                                                handResult.Advice = "Marginal Fold \r\n(With deeper stacks this might be worth a call)";
                                            }
                                            else
                                            {
                                                /*if (short_easy_fold && deep_easy_fold) conclusion=String.Format("{0} <b> Easy fold</b>", conclusion);
                                                else*/
                                                conclusion = String.Format("{0} <b> Fold</b>", conclusion);
                                                handResult.Advice = "Fold";
                                            }

                                            missed_bets -= handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                }
                                else if (temp_advice.StackDepth == GameRules.stack_depths.DEEP_STACK) // Deep stack rankings
                                {
                                    if (canRaise && deep_raise && short_raise)
                                    {
                                        advice = "Raise";
                                        conclusion = String.Format("{0} <b> Raise for value - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        handResult.Advice = String.Format("Raise for value - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (canRaise && deep_raise && deep_jam)
                                    {
                                        advice = "Raise";
                                        conclusion = String.Format("{0} <b> Raise due to deep effective stacks - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        conclusion = String.Format("{0}<br/>(With shorter stacks this might not be worth a raise)", conclusion);
                                        handResult.Advice = String.Format("Raise due to deep effective stacks - Raise to ${0}\r\n(With shorter stacks this might not be worth a raise)", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (canRaise && deep_raise)
                                    {
                                        if (callers >= 2) // If 2+ callers already in the pot
                                        {
                                            advice = "Call";
                                            conclusion = String.Format("{0} <b> Call behind to keep the pot small</b>", conclusion);
                                            handResult.Advice = "Call behind to keep the pot small";

                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else if (callers == 1) // If 1 caller already in the pot
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Raise to isolate one of the players - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Raise to isolate one of the players - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                        else // No callers yet -> Raise for value against single opponent
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Marginal raise - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Marginal raise - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else if (canRaise && deep_jam && callers < 2) // Notice, we won't try to semi-bluff a raiser + 2 callers
                                    {
                                        if (callers >= 1) // If a single caller only in the pot (since we already filtere out 2+ callers)
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Raise as a squeeze play - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Raise as a squeeze play - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                        else // No callers yet -> Semi-bluff against single opponent
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Raise as a semi-bluff - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Raise as a semi-bluff - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else if (deep_call) // Call
                                    {
                                        advice = "Call";

                                        if (short_raise)
                                        {
                                            conclusion = String.Format("{0} <b> Call for pot control</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a raise)", conclusion);
                                            handResult.Advice = "Call for pot control \r\n(With shorter stacks this might be worth a raise)";
                                        }
                                        else
                                        {
                                            if (short_call)
                                            {
                                                conclusion = String.Format("{0} <b> Call</b>", conclusion);
                                                handResult.Advice = "Call";
                                            }
                                            else
                                            {
                                                conclusion = String.Format("{0} <b> Marginal call</b>", conclusion);
                                                handResult.Advice = "Marginal Call";
                                            }
                                        }

                                        missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (deep_gametheory_call) // Game theory based call
                                    {
                                        advice = "Call";
                                        conclusion = String.Format("{0} <b> Call to avoid being exploitable</b>", conclusion);
                                        handResult.Advice = "Call to avoid being exploitable";

                                        missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (implied_odds) // Implied odds call
                                    {
                                        advice = "Call";
                                        conclusion = String.Format("{0} <b> Call due to high implied odds</b>", conclusion);
                                        handResult.Advice = "Call due to high implied odds";

                                        missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                    }
                                    else // Fold
                                    {
                                        advice = "Fold";

                                        if (short_raise)
                                        {
                                            conclusion = String.Format("{0} <b> Fold due to deep effective stacks</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a raise)", conclusion);
                                            handResult.Advice = "Fold due to deep effective stacks \r\n(With shorter stacks this might be worth a raise)";
                                        }
                                        else if (short_call)
                                        {
                                            conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With shorter stacks this might worth a call)", conclusion);
                                            handResult.Advice = "Marginal Fold \r\n(With shorter stacks this might worth a call)";
                                        }
                                        else
                                        {
                                            conclusion = String.Format("{0} <b> Fold</b>", conclusion);
                                            handResult.Advice = "Fold";
                                        }

                                        missed_bets -= handHistory.PreflopActions[i].Amount;
                                    }
                                }
                                else if (temp_advice.StackDepth == GameRules.stack_depths.MEDIUM_STACK) // Medium stack rankings
                                {
                                    if (canRaise && deep_raise && short_raise)
                                    {
                                        advice = "Raise";
                                        conclusion = String.Format("{0} <b> Raise for value - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                        handResult.Advice = String.Format("Raise for value - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                        missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (canRaise && medium_raise)
                                    {
                                        if (callers > 0) // If callers already in the pot -> Try to stay out of trouble by playing small ball
                                        {
                                            advice = "Call";
                                            if (deep_raise)
                                            {
                                                conclusion = String.Format("{0} <b> Call behind to to see a cheap flop</b>", conclusion);
                                                handResult.Advice = "Call behind to to see a cheap flop";
                                            }
                                            else
                                            {
                                                conclusion = String.Format("{0} <b> Call behind to keep the pot small</b>", conclusion);
                                                handResult.Advice = "Call behind to keep the pot small";
                                            }
                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                        else // No callers yet -> Raise against single opponent
                                        {
                                            advice = "Raise";
                                            conclusion = String.Format("{0} <b> Raise - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                            handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));


                                            if (deep_raise)
                                            {
                                                conclusion = String.Format("{0}<br/>(With shorter stacks this might not be worth a raise)", conclusion);
                                                handResult.Advice += "\r\n(With shorter stacks this might not be worth a raise)";
                                            }
                                            else
                                            {
                                                conclusion = String.Format("{0}<br/>(With deeper stacks this might not be worth a raise)", conclusion);
                                                handResult.Advice += "\r\n(With deeper stacks this might not be worth a raise)";
                                            }

                                            missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                    else if (medium_call) // Call
                                    {
                                        advice = "Call";

                                        if (deep_raise)
                                        {
                                            conclusion = String.Format("{0} <b> Call for pot control</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a raise)", conclusion);
                                            handResult.Advice = "Call for pot control \r\n(With deeper stacks this might be worth a raise)";
                                        }
                                        else if (short_raise)
                                        {
                                            conclusion = String.Format("{0} <b> Call for pot control</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a raise)", conclusion);
                                            handResult.Advice = "Call for pot control \r\n(With shorter stacks this might be worth a raise)";
                                        }
                                        else
                                        {
                                            if (deep_call && short_call)
                                            {
                                                conclusion = String.Format("{0} <b> Call</b>", conclusion);
                                                handResult.Advice = "Call";
                                            }
                                            else
                                            {
                                                conclusion = String.Format("{0} <b> Marginal call</b>", conclusion);
                                                handResult.Advice = "Marginal Call";
                                            }
                                        }

                                        missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (medium_gametheory_call) // Game theory based call
                                    {
                                        advice = "Call";
                                        conclusion = String.Format("{0} <b> Call to avoid being exploitable</b>", conclusion);
                                        handResult.Advice = "Call to avoid being exploitable";

                                        missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                    }
                                    else if (implied_odds) // Implied odds call
                                    {
                                        advice = "Call";
                                        conclusion = String.Format("{0} <b> Call due to high implied odds</b>", conclusion);
                                        handResult.Advice = "Call due to high implied odds";

                                        missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                    }
                                    else // Fold
                                    {
                                        advice = "Fold";

                                        if (deep_raise)
                                        {
                                            conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a raise)", conclusion);
                                            handResult.Advice = "Marginal Fold \r\n(With deeper stacks this might be worth a raise)";
                                        }
                                        else if (short_raise)
                                        {
                                            conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a raise)", conclusion);
                                            handResult.Advice = "Marginal Fold \r\n(With shorter stacks this might be worth a raise)";
                                        }
                                        else if (deep_call)
                                        {
                                            conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With deeper stacks this might be worth a call)", conclusion);
                                            handResult.Advice = "Marginal Fold \r\n(With deeper stacks this might be worth a call)";
                                        }
                                        else if (short_call)
                                        {
                                            conclusion = String.Format("{0} <b> Marginal fold</b>", conclusion);
                                            conclusion = String.Format("{0}<br/>(With shorter stacks this might be worth a call)", conclusion);
                                            handResult.Advice = "Marginal Fold \r\n(With shorter stacks this might be worth a call)";
                                        }
                                        else
                                        {
                                            conclusion = String.Format("{0} <b> Fold</b>", conclusion);
                                            handResult.Advice = "Fold";
                                        }

                                        missed_bets -= handHistory.PreflopActions[i].Amount;
                                    }
                                }

                                // Update hand range estimates (and one game theory variable)
                                if (handHistory.PreflopActions[i].SAction == "Raises" || handHistory.PreflopActions[i].SAction == "Bets")
                                {
                                    // Use the tightest range we have seen so far 
                                    hand_range[handHistory.PreflopActions[i].PlayerName] = Math.Min(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]), temp_advice.ActualRaiseRange);

                                    squeeze_fold_p = 1.0f;
                                }
                                else if (handHistory.PreflopActions[i].SAction == "Calls" || handHistory.PreflopActions[i].SAction == "Checks")
                                {
                                    // Calculate the new range
                                    hand_range[handHistory.PreflopActions[i].PlayerName] =
                                        Math.Min(temp_advice.OptimalRaiseRange, Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName])) +
                                        Math.Min(temp_advice.CallRange, Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]));
                                }
                                else if (handHistory.PreflopActions[i].SAction == "Folds")
                                {
                                    //squeeze_fold_p *= 1-min(max(temp_advice.optimal_raise_range,temp_advice.call_range)/hand_range[handHistory.PreflopActions[i].PlayerName], 1.0f);
                                    squeeze_fold_p *= 1 - Math.Min(Math.Max(temp_advice.OptimalRaiseRange, temp_advice.CallRange) / (float)Convert.ToDouble(hand_range[handHistory.PreflopActions[i].Attacker]), 1.0f);
                                    // Using above calculation instead, since we can't assume all other players to call based on EV and then take the remaining responsibility of pot defence, or otherwise the call-range would be way too loose
                                    // More correct way is to assume that the other players would defend ~41% of the time too (against pot sized bets) assuming the attacker raised 100% of his range
                                }
                            }

                            //GHADY
                            //anytime we have AKo or AKs
                            //so first check is, does hero have 60bb's or less... if yes, then raise
                            //[12:48:33 AM] Ace Poker Solutions: if no, then check opponent(s) stack size... if one opponent... then raise
                            //[12:48:39 AM] Ghady Diab: ok
                            //[12:48:42 AM] Ace Poker Solutions: if two, then both need to be under 60bbs
                            //[12:48:51 AM] Ghady Diab: ok
                            //[12:49:07 AM] Ghady Diab: if more than 2, only one should have under 60bbs?
                            //[12:49:27 AM] Ace Poker Solutions: if more than 2 opponents, then all of them need to be under 60bbs
                            bool shouldRaise = false;
                            String heroCards = (handHistory.Players[handHistory.HeroName] as Player).Cards;
                            if ((heroCards[0].Equals('A') && heroCards[2].Equals('K'))
                                || (heroCards[0].Equals('K') && heroCards[2].Equals('A')))
                            {
                                int heroStacks = GetPlayerStackOnStreet(handHistory, handHistory.HeroName, handHistory.PreflopActions[i]);
                                if (heroStacks <= 60 * handHistory.BigBlindAmount)
                                    shouldRaise = true;
                                else
                                {
                                    List<Player> opponents = GetPlayersInHand(handHistory, handHistory.PreflopActions[i], false);
                                    if (opponents.Count == 1) shouldRaise = true;
                                    else
                                    {
                                        shouldRaise = true;
                                        foreach (Player opponent in opponents)
                                        {
                                            int opponentStack = GetPlayerStackOnStreet(handHistory, opponent.PlayerName, handHistory.PreflopActions[i]);
                                            if (opponentStack > 60 * handHistory.BigBlindAmount)
                                            {
                                                shouldRaise = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (shouldRaise)
                            {
                                Debug("If hero has 60bb's or less, OR opponent(s) have 60bb's or less, then hero should raise w/ AK. Override the existing advice");
                                missed_bets = missed_bets_before;

                                advice = "Raise";
                                conclusion = String.Format("{0} <b> Raise - Raise to ${1}</b>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                handResult.Advice = String.Format("Raise - Raise to ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));

                                missed_bets += (temp_advice.RaiseSize - (int)handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName]) - handHistory.PreflopActions[i].Amount;
                            }

                            //PREFLOP OVERRIDE ADVICE
                            //
                            //Cannot fold to a 3-bet in this situation.
                            //If hero and opponent have at least 70bbs,
                            //and the re-raise is less than 3x hero's open raise size, then you must call.
                            if (advice.Equals("Fold")
                                && PlayerIsPreflopRaiser(handHistory, heroPlayer, false)
                                && PotIsReraisedPreflopBeforeAction(handHistory, handHistory.PreflopActions[i], out potWasReraisedMinimumAmount, out _3betAmount)
                                && GetPlayerStackOnStreet(handHistory, handHistory.HeroName, handHistory.PreflopActions[i]) > 7 * handHistory.BigBlindAmount
                                )
                            {
                                Player opponent = HeadsUp(handHistory, handHistory.PreflopActions[i]);

                                if (opponent != null && GetPlayerStackOnStreet(handHistory, opponent.PlayerName, handHistory.PreflopActions[i]) > 7 * handHistory.BigBlindAmount)
                                {
                                    Debug("Cannot fold to a 3-bet in this situation. If hero and opponent have at least 70bbs, and the re-raise is less than 3x hero's open raise size, then you must call.");


                                    advice = "Call";
                                    conclusion = String.Format("{0} <b> Call</b>", conclusion);
                                    handResult.Advice = "Call";

                                    missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                }
                                //ATTz
                            }


                            //GHADY
                            handResult.RaiseSize = temp_advice.RaiseSize;
                            handResult.CorrectAction = advice;
                            handResult.HeroAction = handHistory.PreflopActions[i].SAction;


                            //MOVE ALL IN / CALL ALL IN
                            bool moveAllIn = true;
                            bool callAllIn = false;
                            int amountToUse = advice.Equals("Call") ? basics.To_call : advice.Equals("Raise") ? temp_advice.RaiseSize : advice.Equals("Bet") ? temp_advice.RaiseSize : 0;
                            foreach (String playerName in handHistory.Players.Keys)
                            {
                                if (playerName.Equals(handHistory.HeroName)) continue;
                                if (!(bool)handHistory.PreflopActions[i].InHand[playerName]) continue;
                                int opponentStack = GetPlayerStackOnStreet(handHistory, playerName, handHistory.PreflopActions[i]);

                                if (amountToUse < opponentStack)
                                {
                                    moveAllIn = false;
                                    break;
                                }
                            }

                            int heroStack = GetPlayerStackOnStreet(handHistory, handHistory.HeroName, handHistory.PreflopActions[i]);
                            if (!moveAllIn && advice.Equals("Raise"))
                            {
                                int heroAmount = handHistory.PreflopActions[i].Amount;

                                if (heroAmount == heroStack && heroAmount <= temp_advice.RaiseSize)
                                {
                                    moveAllIn = true;
                                }
                            }

                            //#49341650 - 2h2c : Player has no more money left (and no more players left to act). Should be call all-in.
                            bool opponentsHaveNoMoreMoney = false;
                            foreach (String playerName in handHistory.Players.Keys)
                            {
                                if (!playerName.Equals(handHistory.HeroName))
                                {
                                    if ((bool)handHistory.PreflopActions[i].InHand[playerName])
                                    {
                                        int opponentStack = GetPlayerStackOnStreet(handHistory, playerName, handHistory.PreflopActions[i]);
                                        opponentsHaveNoMoreMoney = opponentStack == 0;
                                        if (!opponentsHaveNoMoreMoney) break;
                                    }
                                }
                            }

                            if (opponentsHaveNoMoreMoney)
                            {
                                int heroStacks = GetPlayerStackOnStreet(handHistory, handHistory.HeroName, handHistory.PreflopActions[i]);
                                Debug("Player has no more money left (and no more players left to act). Should be call all-in.");
                                //temp_advice.bet_size = temp_advice.raise_size = heroStacks;
                                callAllIn = true;
                            }
                            //


                            if (callAllIn && !advice.Equals("Fold") && !advice.Equals("Check"))
                            {
                                advice = "Call";
                                missed_bets = missed_bets_before;
                                missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                conclusion = String.Format("{0} <b> Call All In</b></p>", conclusion);
                                handResult.Advice = "Call All In";
                            }
                            else if ((moveAllIn) && !advice.Equals("Fold") && !advice.Equals("Check"))
                            {
                                Debug("APC ADVICE: " + advice + " + " + advice + " amount=" + ((double)amountToUse / 100) + " > opponents Stack");

                                advice = "Raise";
                                missed_bets = missed_bets_before;
                                if (heroStack != handHistory.PreflopActions[i].Amount)
                                    missed_bets += (int)(temp_advice.RaiseSize - Convert.ToDouble(handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PreflopActions[i].Amount;
                                conclusion = String.Format("{0} <b> Move all-in for ${1}</b></p>", conclusion, GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                                handResult.Advice = String.Format("Move all-in for ${0}", GetLabelNumber((double)temp_advice.RaiseSize / 100, 2, handHistory));
                            }
                            //
                            //END GHADY


                            //MISSED VALUE
                            handResult.ev_raise = ev_raise;

                            Action currentPreflopAction = handHistory.PreflopActions[i];
                            String sAction = currentPreflopAction.SAction;

                            if (false && !sAction.ToLower().Contains(advice.ToLower()))
                            {
                                if (handResult.Advice.ToLower().Contains("sometimes"))
                                {
                                    if (handResult.Advice.ToLower().Contains(sAction.Substring(0, sAction.Length - 1).ToLower()))
                                    {
                                        missed_bets = missed_bets_before;
                                        if (sAction.Equals("Raises"))
                                        {
                                            missed_bets += (int)(temp_advice.RaiseSize - Convert.ToDouble(handHistory.PreflopActions[i].ThisStreetCommitment[handHistory.HeroName])) - handHistory.PreflopActions[i].Amount;
                                        }
                                        else if (sAction.Equals("Checks"))
                                        {
                                            missed_bets += basics.To_call - handHistory.PreflopActions[i].Amount;
                                        }
                                    }
                                }
                                else
                                {
                                    if (sAction.Equals("Checks") && advice.Equals("Bet"))
                                    {
                                        missed_bets = missed_bets_before;
                                        missed_bets += (int)(Math.Abs(ev_raise));
                                    }
                                    else if (sAction.Equals("Folds") && advice.Equals("Call"))
                                    {
                                        missed_bets = missed_bets_before;
                                        missed_bets += (int)(Math.Abs(ev_call));
                                    }
                                    else if (sAction.Equals("Folds") && advice.Equals("Raise"))
                                    {
                                        missed_bets = missed_bets_before;
                                        missed_bets += (int)(Math.Abs(ev_raise));
                                    }
                                    else if (sAction.Equals("Calls") && advice.Equals("Raise"))
                                    {
                                        missed_bets = missed_bets_before;
                                        missed_bets += (int)(Math.Abs(ev_raise));
                                    }
                                }
                            }
                            else
                            {
                                //override missed_value
                                if (handHistory.handResults.Count > 0 && handHistory.handResults[handHistory.handResults.Count - 1].Street == currentPreflopAction.Street)
                                {
                                    Action previousPreflopAction = null;
                                    foreach (Action preflopAction in handHistory.PreflopActions)
                                    {
                                        if (preflopAction.Equals(currentPreflopAction)) break;
                                        if (preflopAction.PlayerName.Equals(handHistory.HeroName))
                                        {
                                            previousPreflopAction = preflopAction;
                                        }
                                    }

                                    if (previousPreflopAction != null && (handHistory.handResults[handHistory.handResults.Count - 1].CorrectAction.Equals("Bet") || handHistory.handResults[handHistory.handResults.Count - 1].CorrectAction.Equals("Raise")) && previousPreflopAction.SAction.Equals("Checks"))
                                    {
                                        missed_bets = missed_bets_before;
                                        int missed_value = (int)handHistory.handResults[handHistory.handResults.Count - 1].BetSize - (int)currentPreflopAction.Amount;
                                        //int missed_value = (int)currentPostflopAction.Amount - (int)handHistory.handResults[handHistory.handResults.Count - 1].ev_raise;
                                        if (missed_value > 0) missed_bets += missed_value;
                                    }
                                }
                                //
                            }

                            //RISK AMOUNT CANNOT BE MORE THAN OPPONENT HAS
                            int minStack = -1;
                            int missedVal = missed_bets - missed_bets_before;

                            if (missedVal < 0)
                            {
                                missedVal *= -1;
                                foreach (String opponent in handHistory.Players.Keys)
                                {
                                    if (opponent.Equals(handHistory.HeroName)) continue;
                                    if (!(bool)handHistory.PreflopActions[i].InHand[opponent]) continue;
                                    double opponentStack = GetPlayerStackOnStreet(handHistory, opponent, handHistory.PreflopActions[i]);
                                    if (handHistory.PreflopActions[i].Amount > opponentStack && minStack == -1 || opponentStack < minStack)
                                    {
                                        minStack = (int)opponentStack;
                                    }
                                }
                                if (minStack != -1)
                                {
                                    int adviceAmount = temp_advice.RaiseSize;
                                    missed_bets += handHistory.PreflopActions[i].Amount - minStack;
                                }
                            }

                            //



                            //ALL OPPONENTS FOLDED AFTER HERO?
                            if ((advice.Equals("Bet") && handHistory.PreflopActions[i].SAction.Equals("Bets"))
                                || (advice.Equals("Raise") && handHistory.PreflopActions[i].SAction.Equals("Raises")))
                            {
                                bool allOpponentsFoldedAfter = true;
                                bool afterHero = false;
                                foreach (Action preflopAction in handHistory.PreflopActions)
                                {
                                    if (afterHero && !preflopAction.PlayerName.Equals(handHistory.HeroName) && !preflopAction.SAction.Equals("Folds"))
                                    {
                                        allOpponentsFoldedAfter = false;
                                        break;
                                    }
                                    if (preflopAction == handHistory.PreflopActions[i])
                                        afterHero = true;
                                }
                                if (allOpponentsFoldedAfter)
                                {
                                    missed_bets = missed_bets_before;
                                }
                            }


                            //Pre-flop only: If someone open raises between 3 - 4 times the big blind amount,
                            //then we exclude this from missed value. The default AI advice is to open 3.5x BB.
                            //If someone open raising 3x, then we do not count this .5.
                            if (advice.Equals("Raise"))
                            {
                                if (raisers == 0)
                                {
                                    if (temp_advice.RaiseSize >= handHistory.BigBlindAmount * 3 && temp_advice.RaiseSize <= handHistory.BigBlindAmount * 4)
                                    {
                                        Debug("If someone open raises between 3 - 4 times the big blind amount, then we exclude this from missed value.");
                                        missed_bets = missed_bets_before;
                                    }
                                }
                            }
                            //

                            handResult.HeroAction = handHistory.PreflopActions[i].SAction;
                            advice_street[0].Add(advice);

                            missed_bets_street[0].Add(missed_bets - missed_bets_before);
                            missed_bets_final[0].Add(missed_bets - missed_bets_before);

                        }
                        handHistory.handResults.Add(handResult);
                    }



                    #region added
                    else //VILLAIN ACTED
                    {
                        //GHADY
                        /*
                         *if our hero is open raising on the button (this means that positions in front of him folded pre-flop), then we want to increase opponents raising range to:
60% if there's a fish or a whale in the Big blind or small blind. 
                        */
                        String opponentName = handHistory.PreflopActions[i].PlayerName;
                        String bbPlayerModel = Player.GetPlayerModel(handHistory.Is6Max, handHistory.Players[handHistory.BBName] as Player).ToLower();
                        String sbPlayerModel = Player.GetPlayerModel(handHistory.Is6Max, handHistory.Players[handHistory.SBName] as Player).ToLower();

                        bool heroOpenRaisesFromButtonAndPlayerIsFishOrWhale = heroOpenRaisesFromButton && (bbPlayerModel.Equals("fish") || bbPlayerModel.Equals("whale") || (sbPlayerModel.Equals("fish") || sbPlayerModel.Equals("whale")));
                        if (heroOpenRaisesFromButtonAndPlayerIsFishOrWhale)
                        {

                        }
                        //END GHADY

                        if (raisers == 0) // Unraised pot
                        {
                            RaiseAdvice temp_advice = open_raise(handHistory, handHistory.PreflopActions[i]);


                            if (debug_mode)
                            {
                                msg = String.Format("DEBUG: {0} ({1}): {2}% -> ${3} and {4}% -> Limp", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(temp_advice.OptimalRaiseRange * 100, 0, null), GetLabelNumber(temp_advice.RaiseSize / 100.0f, 2, null), GetLabelNumber(temp_advice.CallRange * 100, 0, null));
                            }

                            // Update hand range estimates (and one game theory variable)
                            if (handHistory.PreflopActions[i].SAction == "Raises" || handHistory.PreflopActions[i].SAction == "Bets")
                            {
                                hand_range[handHistory.PreflopActions[i].PlayerName] = temp_advice.ActualRaiseRange;

                                // Show hand reading %-range in the "advice" column
                                if (heroOpenRaisesFromButtonAndPlayerIsFishOrWhale)
                                {
                                    //hand_range[handHistory.PreflopActions[i].PlayerName] = 1.2;//ATT
                                }
                                advice = String.Format("~{0}%", GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.CallRange * 100, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.ActualRaiseRange * 100, 0, null));

                                Debug(temp_advice.Debug);
                                if (debug_mode)
                                {
                                    if (handHistory.PreflopActions[i].SAction == "Bets") msg = String.Format("{0} ({1}) bet -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber((float)Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                    else if (handHistory.PreflopActions[i].SAction == "Raises") msg = String.Format("{0} ({1}) raised -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber((float)Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                }


                                squeeze_fold_p = 1.0f;
                            }
                            else if (handHistory.PreflopActions[i].SAction == "Calls" || handHistory.PreflopActions[i].SAction == "Checks")
                            {
                                hand_range[handHistory.PreflopActions[i].PlayerName] = temp_advice.OptimalRaiseRange + temp_advice.CallRange;


                                if (debug_mode)
                                {
                                    if (handHistory.PreflopActions[i].SAction == "Checks") msg = String.Format("{0} ({1}) checked -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                    else if (handHistory.PreflopActions[i].SAction == "Calls") msg = String.Format("{0} ({1}) called -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                }
                                // Show hand reading %-range in the "advice" column
                                if (heroOpenRaisesFromButtonAndPlayerIsFishOrWhale)
                                {
                                    hand_range[handHistory.PreflopActions[i].PlayerName] = 1.2;
                                }
                                advice = String.Format("~{0}%", GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.CallRange, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.ActualRaiseRange, 0, null));
                            }
                            else if (handHistory.PreflopActions[i].SAction == "Folds")
                            {
                                //squeeze_fold_p *= 1-min(max(temp_advice.optimal_raise_range,temp_advice.call_range)/hand_range[handHistory.PreflopActions[i].PlayerName], 1.0f);
                                squeeze_fold_p *= 1 - Math.Min(Math.Max(temp_advice.OptimalRaiseRange, temp_advice.CallRange) / (float)Convert.ToDouble(hand_range[handHistory.PreflopActions[i].Attacker]), 1.0f);
                                // Using above calculation instead, since we can't assume all other players to call based on EV and then take the remaining responsibility of pot defence, or otherwise the call-range would be way too loose
                                // More correct way is to assume that the other players would defend ~41% of the time too (against pot sized bets) assuming the attacker raised 100% of his range
                            }
                        }
                        else // Raised pot
                        {
                            RaiseAdvice temp_advice = re_raise(handHistory, handHistory.PreflopActions[i], hand_range, squeeze_fold_p);

                            if (debug_mode)
                            {
                                msg = String.Format("{0} ({1}): {2}% -> ${3} and {4}% -> Call", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(temp_advice.OptimalRaiseRange * 100, 0, null), GetLabelNumber(temp_advice.RaiseSize / 100.0f, 2, null), GetLabelNumber(temp_advice.CallRange * 100, 0, null));
                            }

                            // Update hand range estimates (and one game theory variable)
                            if (handHistory.PreflopActions[i].SAction == "Raises" || handHistory.PreflopActions[i].SAction == "Bets")
                            {
                                // Use the tightest range we have seen so far 
                                hand_range[handHistory.PreflopActions[i].PlayerName] = Math.Min(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]), temp_advice.ActualRaiseRange);
                                // Reset the squeeze_fold_p
                                squeeze_fold_p = 1.0f;

                                // Show hand reading %-range in the "advice" column
                                if (heroOpenRaisesFromButtonAndPlayerIsFishOrWhale)
                                {
                                    hand_range[handHistory.PreflopActions[i].PlayerName] = 1.2;
                                }
                                advice = String.Format("~{0}%", GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.CallRange, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.ActualRaiseRange, 0, null));

                                Debug(temp_advice.Debug);
                                if (debug_mode)
                                {
                                    if (handHistory.PreflopActions[i].SAction == "Bets") msg = String.Format("{0} ({1}) bet -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                    else if (handHistory.PreflopActions[i].SAction == "Raises")
                                    {
                                        msg = String.Format("{0} ({1}) raised -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                    }
                                }
                            }
                            else if (handHistory.PreflopActions[i].SAction == "Calls" || handHistory.PreflopActions[i].SAction == "Checks")
                            {
                                // Calculate the new range
                                hand_range[handHistory.PreflopActions[i].PlayerName] =
                                    Math.Min(temp_advice.OptimalRaiseRange, Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName])) +
                                    Math.Min(temp_advice.CallRange, Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]));

                                // Show hand reading %-range in the "advice" column
                                if (heroOpenRaisesFromButtonAndPlayerIsFishOrWhale)
                                {
                                    hand_range[handHistory.PreflopActions[i].PlayerName] = 1.2;
                                }
                                advice = String.Format("~{0}%", GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.CallRange, 0, null));
                                advice += String.Format(" - ~{0}%", GetLabelNumber(temp_advice.ActualRaiseRange, 0, null));

                                if (debug_mode)
                                {
                                    if (handHistory.PreflopActions[i].SAction == "Checks") msg = String.Format("{0} ({1}) checked -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                    else if (handHistory.PreflopActions[i].SAction == "Calls") msg = String.Format("{0} ({1}) called -> avg. {2}%", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[handHistory.PreflopActions[i].PlayerName] as Player).Position], handHistory.PreflopActions[i].PlayerName, GetLabelNumber(Convert.ToDouble(hand_range[handHistory.PreflopActions[i].PlayerName]) / 2 * 100, 0, null));
                                }
                            }
                            else if (handHistory.PreflopActions[i].SAction == "Folds")
                            {
                                //squeeze_fold_p *= 1-min(max(temp_advice.optimal_raise_range,temp_advice.call_range)/hand_range[handHistory.PreflopActions[i].PlayerName], 1.0f);
                                squeeze_fold_p *= 1 - Math.Min(Math.Max(temp_advice.OptimalRaiseRange, temp_advice.CallRange) / ((float)Convert.ToDouble(hand_range[handHistory.PreflopActions[i].Attacker])), 1.0f);
                                // Using above calculation instead, since we can't assume all other players to call based on EV and then take the remaining responsibility of pot defence, or otherwise the call-range would be way too loose
                                // More correct way is to assume that the other players would defend ~41% of the time too (against pot sized bets) assuming the attacker raised 100% of his range
                            }
                        }
                    }

                    #endregion

                    pot += (handHistory.PreflopActions[i] as Action).Amount;
                    //if ((handHistory.PreflopActions[i] as Action).Amount > max_commitment) max_commitment = (handHistory.PreflopActions[i] as Action).Amount; this is incorrect, because "amount" has only the additional amount, not total commitment
                }

                /*if ((handHistory.PreflopActions[i] as Action).PlayerName == handHistory.HeroName) swprintf(buf_player, 128, "(%s) <%i> %s", handHistory.position_names[handHistory.player.size()][(handHistory.Players[(handHistory.PreflopActions[0] as Action).PlayerName] as Player).position], hero_action_counter, (handHistory.PreflopActions[i] as Action).PlayerName);
                else*/
                player = String.Format("{0} - {1}", GameRules.position_names[handHistory.Players.Count, (handHistory.Players[(handHistory.PreflopActions[i] as Action).PlayerName] as Player).Position], (handHistory.PreflopActions[i] as Action).PlayerName);
                action = handHistory.PreflopActions[i].SAction;
                if ((handHistory.PreflopActions[i] as Action).Amount > 0)
                {
                    if (((handHistory.PreflopActions[i] as Action).Amount % 100) == 0)
                    {
                        if (handHistory.PreflopActions[i].SAction == "Raises")
                            amount = (((handHistory.PreflopActions[i] as Action).Amount + (int)(handHistory.PreflopActions[i] as Action).ThisStreetCommitment[(handHistory.PreflopActions[i] as Action).PlayerName]) / 100).ToString(); // Raise To
                        else amount = ((handHistory.PreflopActions[i] as Action).Amount / 100).ToString();
                    }
                    else
                    {
                        if (handHistory.PreflopActions[i].SAction == "Raises")
                            amount = String.Format("{0}", ((double)((handHistory.PreflopActions[i] as Action).Amount + (int)(handHistory.PreflopActions[i] as Action).ThisStreetCommitment[(handHistory.PreflopActions[i] as Action).PlayerName])) / 100).ToString(); // Raise To
                        else amount = String.Format("{0}", (double)((handHistory.PreflopActions[i] as Action).Amount / 100));
                    }
                }

                else amount = "";
                int hh_formatting = 0;
                if (handHistory.PreflopActions[i].SAction == "Folds" || handHistory.PreflopActions[i].SAction == "Returns") hh_formatting = (int)GameRules.format_types.FORMAT_FOLD;
                else if (handHistory.PreflopActions[i].SAction == "Calls" ||
                         handHistory.PreflopActions[i].SAction == "Checks") hh_formatting = (int)GameRules.format_types.FORMAT_CALL;
                else if (handHistory.PreflopActions[i].SAction == "Raises" ||
                    handHistory.PreflopActions[i].SAction == "Bets") hh_formatting = (int)GameRules.format_types.FORMAT_RAISE;

                //Console.WriteLine(conclusion);

                if (handHistory.PreflopActions[i].SAction == "Raises") raisers++;
                else if (handHistory.PreflopActions[i].SAction == "Calls")
                {
                    if (raisers == 0) limpers++;
                    else callers++;
                }
            }

            return hand_range; // Return hashtable of the players' estimated hand percentiles
        }

        bool inPosition(HandHistory handHistory, String sPlayer1, String sPlayer2)
        {
            Player player1 = handHistory.Players[sPlayer1] as Player;
            Player player2 = handHistory.Players[sPlayer2] as Player;

            if (player1.Position == handHistory.Players.Count - 2) return false; // SB is always OOP
            else if (player2.Position == handHistory.Players.Count - 2) return true;  // SB is always OOP
            else if (player1.Position == handHistory.Players.Count - 1) return false; // BB is OOP, if opponent is not in SB
            else if (player2.Position == handHistory.Players.Count - 1) return true;  // BB is OOP, if opponent is not in SB
            else if (player1.Position > player2.Position) return true;

            return false;
        }

        bool HeroMovedAllIn(HandHistory handHistory, Action action)
        {
            bool movedAllIn = true;
            int amountUsed = action.Amount;

            foreach (String playerName in handHistory.Players.Keys)
            {
                if (playerName.Equals(handHistory.HeroName)) continue;
                if (!(bool)action.InHand[playerName]) continue;
                int opponentStack = GetPlayerStackOnStreet(handHistory, playerName, action);

                if (amountUsed < opponentStack)
                {
                    movedAllIn = false;
                    break;
                }
            }
            return movedAllIn;
        }

        internal static int GetActivePlayersNB(HandHistory handHistory, Action action)
        {
            int active = 0;
            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                active++;
            }
            return active;
        }

        internal static List<Player> GetPlayersInHand(HandHistory handHistory, Action action, bool withHero)
        {
            List<Player> activePlayers = new List<Player>();
            foreach (String opponent in handHistory.Players.Keys)
            {
                if (!(bool)action.InHand[opponent]) continue; // Skip players already folded
                if (!withHero && opponent.Equals(handHistory.HeroName)) continue;
                activePlayers.Add(handHistory.Players[opponent] as Player);
            }
            return activePlayers;
        }

        PreAction DecisionSummary(HandHistory handHistory, Action action)//ATT
        {
            PreAction summary = new PreAction();
            summary.Pot = 0;
            summary.To_call = 0;
            summary.Eff_stacks = 0;
            summary.Players_ip = 0;
            summary.Players_oop = 0;
            summary.TotalAnte = 0;

            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                summary.Pot += Math.Min((int)action.LastStreetCommitment[key] + (int)action.ThisStreetCommitment[key], (handHistory.Players[action.PlayerName] as Player).StartingStack); // Don't count other players' side pots

                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                if (key == action.PlayerName) continue; // Skip hero

                if (inPosition(handHistory, action.PlayerName, key)) summary.Players_oop++;
                else summary.Players_ip++;

                if ((handHistory.Players[key] as Player).StartingStack > summary.Eff_stacks) summary.Eff_stacks = (handHistory.Players[key] as Player).StartingStack;
                if ((int)action.ThisStreetCommitment[key] > (int)summary.To_call) summary.To_call = (int)action.ThisStreetCommitment[key];
            }
            summary.Eff_stacks = Math.Min(summary.Eff_stacks, (handHistory.Players[action.PlayerName] as Player).StartingStack);
            summary.To_call -= (int)action.ThisStreetCommitment[action.PlayerName];

            return summary;
        }

        internal int GetPlayerStackOnStreet(HandHistory handHistory, String playerName, Action action)
        {
            Player player = handHistory.Players[playerName] as Player;
            int startingStack = player.StartingStack;
            if (handHistory.BBName.Equals(playerName))
                startingStack -= (int)handHistory.BigBlindAmount;
            else if (handHistory.SBName.Equals(playerName))
                startingStack -= (int)handHistory.SmallBlindAmount;

            bool stop = false;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction == action)
                {
                    stop = true;
                    break;
                }
                if (preflopAction.PlayerName.Equals(playerName))
                {
                    if (preflopAction != action)
                    {
                        startingStack -= preflopAction.Amount;
                    }
                }
            }

            if (!stop)
            {
                foreach (List<Action> postflopActions in handHistory.PostflopActions)
                {
                    foreach (Action postflopAction in postflopActions)
                    {
                        if (postflopAction == action)
                        {
                            stop = true;
                            break;
                        }
                        if (postflopAction.PlayerName.Equals(playerName))
                        {
                            if (postflopAction != action)
                            {
                                startingStack -= postflopAction.Amount;
                            }
                        }
                    }
                    if (stop)
                        break;
                }
            }
            return startingStack;
        }

        RaiseAdvice open_raise(HandHistory handHistory, Action action)
        {
            Player player = handHistory.Players[action.PlayerName] as Player;

            RaiseAdvice open_raise_advice = new RaiseAdvice();
            open_raise_advice.Debug = "";

            open_raise_advice.StackDepth = GameRules.stack_depths.DEEP_STACK; // By default use deep stack rankings
            open_raise_advice.PocketPairOdds = false; // By default assume drawing hands don't get sufficient implied odds
            open_raise_advice.SuitedConnectorOdds = false;
            open_raise_advice.SuitedGapperOdds = false;

            // Find player's relative position, and count the pot

            int players_oop = 0, players_ip = 0, pot = 0, eff_stacks = 0, to_call = 0;
            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                pot += Math.Min((int)action.LastStreetCommitment[key] + (int)action.ThisStreetCommitment[key], (handHistory.Players[action.PlayerName] as Player).StartingStack); // Don't count other players' side pots

                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                if (key == action.PlayerName) continue; // Skip hero

                if (inPosition(handHistory, action.PlayerName, key)) players_oop++;
                else players_ip++;

                if ((handHistory.Players[key] as Player).StartingStack > eff_stacks) eff_stacks = (handHistory.Players[key] as Player).StartingStack;
                if ((int)action.ThisStreetCommitment[key] > to_call) to_call = (int)action.ThisStreetCommitment[key];
            }
            eff_stacks = Math.Min(eff_stacks, (handHistory.Players[action.PlayerName] as Player).StartingStack);
            to_call -= (int)action.ThisStreetCommitment[action.PlayerName];
            //open_raise_advice.debug.Format(L"%i pot", pot);

            // How much stack would be left if we call
            int stack_left = eff_stacks - to_call - (int)action.LastStreetCommitment[action.PlayerName] - (int)action.ThisStreetCommitment[action.PlayerName];
            if (stack_left < 0) // If someone raised more than we have stack left
            {
                to_call += stack_left;
                stack_left = 0;
            }

            // See if we have sufficient implied odds to play weak drawing hands
            float implied_odds = to_call / (float)(pot + to_call + stack_left);
            float position_advantage = players_oop / (float)(players_oop + players_ip);
            if (implied_odds <= position_advantage / 14 + (1 - position_advantage) / 20) open_raise_advice.PocketPairOdds = true; //  ~14x remaining stacks (or  ~20x from OOP) required for calling w/ pocket pairs
            if (implied_odds <= position_advantage / 50 + (1 - position_advantage) / 70) open_raise_advice.SuitedConnectorOdds = true; //  ~50x remaining stacks (or  ~70x from OOP) required for calling w/ suited connectors
            if (implied_odds <= position_advantage / 100 + (1 - position_advantage) / 140) open_raise_advice.SuitedGapperOdds = true; // ~100x remaining stacks (or ~140x from OOP) required for calling w/ suited 1-gappers

            // Find short stackers (~15xBB in empty pots), so we won't open-raise as wide and risk getting re-raised all-in
            // And medium stackers, so we know which hand rankings to use

            int short_stack_size = (int)(15 * handHistory.BigBlindAmount); // ~15xBB is where villain can push all in for sqrt(2) times the pot if we make a standard pot sized open-raise
            int medium_stack_size = (int)(45 * handHistory.BigBlindAmount); // ~45xBB is where after a standard pot sized open-raise there is left for one pot-sized re-raise, followed by all-in for sqrt(2) times the pot

            int short_stackers = 0, medium_stackers = 0;
            float avg_short_stack = 0;

            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                if (key == action.PlayerName) continue; // Skip hero

                if (Math.Min((handHistory.Players[key] as Player).StartingStack, (handHistory.Players[action.PlayerName] as Player).StartingStack) / (float)(pot + 2 * handHistory.BigBlindAmount) <=
                    short_stack_size / (float)(handHistory.SmallBlindAmount + handHistory.BigBlindAmount + 2 * handHistory.BigBlindAmount))
                {
                    short_stackers++;
                    avg_short_stack = (float)Math.Min((handHistory.Players[key] as Player).StartingStack, (handHistory.Players[action.PlayerName] as Player).StartingStack);
                }
                else if (Math.Min((handHistory.Players[key] as Player).StartingStack, (handHistory.Players[action.PlayerName] as Player).StartingStack) / (float)(pot + 2 * handHistory.BigBlindAmount) <=
                    medium_stack_size / (float)(handHistory.SmallBlindAmount + handHistory.BigBlindAmount + 2 * handHistory.BigBlindAmount))
                {
                    medium_stackers++;
                }
            }
            if (short_stackers > 0) avg_short_stack /= short_stackers;

            // Start calculating the final raise size and hand range advice
            int standard_raise = (int)(pot + 2 * handHistory.BigBlindAmount); // "Raise to standard_raise" would be a pot sized raise
            //GHADY REDUCE POT SIZE RAISE PREFLOP
            //standard_raise = reduceRaiseSize?(int)Math.Round(((double)3 / (double)4) * standard_raise):standard_raise;

            //GHADY OPEN RAISE
            /*
            *If HERO open raises from the button position. This means that everyone has folded pre-flop, and HERO is on the button. Raise to 3x the big blind amount as default, but if one of the opponents in the blinds (small or big blind) has a VPIP => 50%, then raise to 3.5x the big blind.
            */
            Player heroPlayer = handHistory.Players[handHistory.HeroName] as Player;
            String sPosition = GameRules.position_names[handHistory.Players.Count, (handHistory.Players[action.PlayerName] as Player).Position];
            if (!skipCustom)
            {
                if (sPosition.Equals("CO"))
                {

                }
                if (action.PlayerName == handHistory.HeroName)
                {
                    int nbLimpers = 0;
                    foreach (Action preflopAction in handHistory.PreflopActions)
                    {
                        if (preflopAction.Equals(action))
                            break;

                        if (preflopAction.SAction.Equals("Calls"))
                        {
                            nbLimpers++;
                        }
                    }

                    if (sPosition.Equals("SB") || sPosition.Equals("BB"))
                    {
                        Debug("if advice is to raise, then it needs to be slightly larger out of position. Current hand says to raise 3X from the BB. It should be raise 3.5x + 1BB for every limper (person who has called the big blind pre-flop). In general, out of position raises should be slightly larger. (HERO)");
                        standard_raise = (int)(standard_raise + handHistory.BigBlindAmount * nbLimpers);
                    }

                    if (sPosition == "BTN")
                    {
                        bool noOneLimpedOrRaised = true;
                        foreach (Action preflopAction in handHistory.PreflopActions)
                        {
                            if (preflopAction == action)
                                break;
                            else if (!preflopAction.PlayerName.Equals(handHistory.HeroName) && (preflopAction.SAction.Equals("Raises") || preflopAction.SAction.Equals("Calls")))
                            {
                                noOneLimpedOrRaised = false;
                                break;
                            }
                        }

                        if (noOneLimpedOrRaised)
                        {
                            if (!Settings.CurrentSettings.TurnOffPlayerModelAdjustments)
                            {
                                Debug("If HERO open raises from the button position. This means that everyone has folded pre-flop, and HERO is on the button. Raise to 3x the big blind amount as default, but if one of the opponents in the blinds (small or big blind) has a VPIP => 50%, then raise to 3.5x the big blind.");
                                //                      if (action.SAction.Equals("Raises") || action.SAction.Equals("Calls") || action.SAction.Equals("Bets"))
                                //                            heroOpenRaisesFromButton = true;
                                if ((handHistory.Players[handHistory.BBName] as Player).Stats.VPIP > 50 || (handHistory.Players[handHistory.SBName] as Player).Stats.VPIP > 50)
                                    standard_raise = (int)(((double)3.5 * (double)handHistory.BigBlindAmount)); // "Raise to standard_raise" would be a pot sized raise
                                else
                                    standard_raise = (int)((3 * handHistory.BigBlindAmount)); // "Raise to standard_raise" would be a pot sized raise
                            }
                        }
                    }
                }
            }
            //END GHADY



            // Use standard pot sized raise over 10xBB stack (or ~14xBB if we are OOP after the flop)
            float jam_or_fold_stack = (float)(10 * players_oop / (float)(players_oop + players_ip) + 10 * Math.Sqrt(2.0f) * players_ip / (float)(players_oop + players_ip));
            if ((handHistory.Players[action.PlayerName] as Player).StartingStack > jam_or_fold_stack * handHistory.BigBlindAmount)
                open_raise_advice.RaiseSize = standard_raise;
            else open_raise_advice.RaiseSize = (handHistory.Players[action.PlayerName] as Player).StartingStack; // Otherwise move-all in or fold, since we would be close to being pot committed and might have to call an all-in anyways

            // Calculate hand range for a standard pot sized opening raise based on position and number of opponents

            float simple_raise_range = 1.0f / (players_oop + players_ip + 1); // Starting point is 1/k, where k is the number of players

            int actual_raise_amount = action.Amount + (int)action.ThisStreetCommitment[action.PlayerName]; // Notice! XML format contains only the amount added (which mathematically makes a lot of sense), not the more typical "raise" or "raise to" amount that is shown by poker rooms and raw hand histories
            open_raise_advice.Debug = "";

            if (open_raise_advice.RaiseSize == (handHistory.Players[action.PlayerName] as Player).StartingStack) // If moving all-in
            {
                // No risk of getting re-raised
                // No positional advantage or disadvantage
                // No worrying about short stacks

                // Use short stack rankings if all-in is small enough for BB to be about pot committed
                if (open_raise_advice.RaiseSize < pot + 2 * Math.Sqrt(2.0f) * handHistory.BigBlindAmount)
                {
                    //open_raise_advice.short_stack_rankings = true;
                    open_raise_advice.StackDepth = GameRules.stack_depths.SHORT_STACK;
                }
                // Otherwise we always use deep stack rankings because we need the card removal effect (e.g. A5s) for optimal semi-bluffing

                // Update all-in -range based on the all-in size
                open_raise_advice.OptimalRaiseRange = simple_raise_range * (1 - open_raise_advice.RaiseSize / (float)(open_raise_advice.RaiseSize + pot)) / (1 - standard_raise / (float)(standard_raise + pot));
                open_raise_advice.ActualRaiseRange = open_raise_advice.OptimalRaiseRange; // Actual raise range is based on the assumption that the player is willing to risk all his stack anyways


                // No limping range for non-blinds, because we play all-in or fold here
                open_raise_advice.CallRange = open_raise_advice.OptimalRaiseRange;
                // For SB calculate limp -range based on the standard raise range without the all-in adjustment
                if ((handHistory.Players[action.PlayerName] as Player).Position == handHistory.Players.Count - 2) open_raise_advice.CallRange = simple_raise_range;
                else if ((handHistory.Players[action.PlayerName] as Player).Position == handHistory.Players.Count - 1) open_raise_advice.CallRange = 1; // BB can check for free with his full range


                //GHADY
                String playerPosition = GameRules.position_names[handHistory.Players.Count, player.Position];
                //EP, MP, CO, BTN, SB and UTG
                //action.Street
                //The open raise calcs should effect the equity calculation for the hands, but not seeing enough of a difference in these numbers.
                //double openRaise = playerPosition.Equals("EP") ? player.Stats.OpenRaiseEP : playerPosition.Equals("MP") ? player.Stats.OpenRaiseMP : playerPosition.Equals("CO") ? player.Stats.OpenRaiseCO : playerPosition.Equals("BTN") ? player.Stats.OpenRaiseBTN : playerPosition.Equals("SB") ? player.Stats.OpenRaiseSB : playerPosition.Equals("UTG") ? player.Stats.OpenRaiseUTG : -1;
                //double openRaise = (playerPosition.Equals("EP") || playerPosition.Equals("MP") || playerPosition.Equals("CO") || playerPosition.Equals("BTN") || playerPosition.Equals("SB") || playerPosition.Equals("UTG"))?player.Stats.OpenRaises[player.Position]:-1;
                double openRaise = -1;
                switch (playerPosition)
                {
                    case "EP": openRaise = player.Stats.OpenRaiseEP; break;
                    case "MP": openRaise = player.Stats.OpenRaiseMP; break;
                    case "CO": openRaise = player.Stats.OpenRaiseCO; break;
                    case "BTN": openRaise = player.Stats.OpenRaiseBTN; break;
                    case "SB": openRaise = player.Stats.OpenRaiseSB; break;
                    case "UTG": openRaise = player.Stats.OpenRaiseUTG; break;
                    default: openRaise = -1; break;
                }
                if (openRaise != -1 && !player.PlayerName.Equals(handHistory.HeroName) && player.CanUseStats)
                {
                    open_raise_advice.Debug = "The open raise calcs should effect the equity calculation for the hands: " + player.PlayerName + " (Open Raise from " + playerPosition + ": " + (openRaise * 100) + "%)";
                    open_raise_advice.OptimalRaiseRange = (float)(openRaise);
                    open_raise_advice.ActualRaiseRange = (float)(openRaise);
                }
                else
                    open_raise_advice.Debug = "AI OPEN RAISE for " + player.PlayerName + ": " + (double)open_raise_advice.ActualRaiseRange * 100;

                player.OpenRaiseRange = open_raise_advice.ActualRaiseRange;
                //END GHADY

                return open_raise_advice;
            }
            else // If not moving all-in
            {
                // Use short stack rankings when there are more short stacks than all other stacks
                if (short_stackers > players_oop + players_ip - short_stackers)
                {
                    //open_raise_advice.short_stack_rankings = true;
                    open_raise_advice.StackDepth = GameRules.stack_depths.SHORT_STACK;
                }
                else if (medium_stackers > players_oop + players_ip - medium_stackers)
                {
                    // Use medium stack rankings when there are more medium stacks than all other stacks
                    open_raise_advice.StackDepth = GameRules.stack_depths.MEDIUM_STACK;
                }
                else if (short_stackers > players_oop + players_ip - short_stackers - medium_stackers)
                {
                    // Also use medium stack rankings when there are more short stacks than deep stacks
                    open_raise_advice.StackDepth = GameRules.stack_depths.MEDIUM_STACK;
                }
                /*else if (short_stackers == players_oop+players_ip-short_stackers)
                {
                    // Use player in BB as a tie breaker, as he's most likely to get involved
                    if (handHistory.player[handHistory.bbName].startingStack / (float)(pot+2*handHistory.bigBlindAmount) <=
                        short_stack_size/(float)(handHistory.smallBlindAmount+handHistory.bigBlindAmount+2*handHistory.bigBlindAmount))
                    {
                        open_raise_advice.short_stack_rankings = true;
                    }
                }*/

                // Apply multiplier of 2*(sqrt(2)-1) to count for the risk of getting raised
                simple_raise_range *= (float)(2 * (Math.Sqrt(2.0f) - 1));
                // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP
                float standard_raise_range = (float)(simple_raise_range * Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip)));


                standard_raise_range = standard_raise_range * 1.15f; // Added a 1.15 multiplier because John thought the advice was too tight on the button
                if (sPosition.Equals("BTN"))
                {
                }
                // Calculate alternative raise range we'd use against short stackers (no positional advantage, as they either fold or move all-in)
                float shortstack_raise_range = simple_raise_range * (1 - avg_short_stack / (float)(avg_short_stack + pot)) / (1 - standard_raise / (float)(standard_raise + pot));

                // Calculate the final raise range based on the risk of getting pushed all-in by a short stacker

                open_raise_advice.OptimalRaiseRange = (shortstack_raise_range * short_stackers + standard_raise_range * (players_oop + players_ip - short_stackers)) / (float)(players_oop + players_ip);

                String playerPosition = GameRules.position_names[handHistory.Players.Count, player.Position];


                //EP, MP, CO, BTN, SB and UTG
                //action.Street
                //The open raise calcs should effect the equity calculation for the hands, but not seeing enough of a difference in these numbers.
                //double openRaise = playerPosition.Equals("EP") ? player.Stats.OpenRaiseEP : playerPosition.Equals("MP") ? player.Stats.OpenRaiseMP : playerPosition.Equals("CO") ? player.Stats.OpenRaiseCO : playerPosition.Equals("BTN") ? player.Stats.OpenRaiseBTN : playerPosition.Equals("SB") ? player.Stats.OpenRaiseSB : playerPosition.Equals("UTG") ? player.Stats.OpenRaiseUTG : -1;
                //double openRaise = (playerPosition.Equals("EP") || playerPosition.Equals("MP") || playerPosition.Equals("CO") || playerPosition.Equals("BTN") || playerPosition.Equals("SB") || playerPosition.Equals("UTG"))?player.Stats.OpenRaises[player.Position]:-1;
                double openRaise = -1;
                switch (playerPosition)
                {
                    case "EP": openRaise = player.Stats.OpenRaiseEP; break;
                    case "MP": openRaise = player.Stats.OpenRaiseMP; break;
                    case "CO": openRaise = player.Stats.OpenRaiseCO; break;
                    case "BTN": openRaise = player.Stats.OpenRaiseBTN; break;
                    case "SB": openRaise = player.Stats.OpenRaiseSB; break;
                    case "UTG": openRaise = player.Stats.OpenRaiseUTG; break;
                    default: openRaise = -1; break;
                }
                if (openRaise != -1 && !player.PlayerName.Equals(handHistory.HeroName) && player.CanUseStats)
                {
                    open_raise_advice.Debug = "The open raise calcs should effect the equity calculation for the hands: " + player.PlayerName + " (Open Raise from " + playerPosition + ": " + (openRaise * 100) + "%)";
                    open_raise_advice.OptimalRaiseRange = (float)(openRaise);
                    open_raise_advice.ActualRaiseRange = (float)(openRaise);
                }
                else
                {
                    if (sPosition.Equals("MP"))
                    {
                    }
                    // Calculate raise-range based on the actual raise size chosen by the player
                    open_raise_advice.ActualRaiseRange = simple_raise_range * (1 - actual_raise_amount / (float)(actual_raise_amount + pot)) / (1 - standard_raise / (float)(standard_raise + pot));
                    open_raise_advice.ActualRaiseRange *= (float)(Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip))); // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP
                    open_raise_advice.ActualRaiseRange = (shortstack_raise_range * short_stackers + open_raise_advice.ActualRaiseRange * (players_oop + players_ip - short_stackers)) / (float)(players_oop + players_ip);


                    {
                        //open_raise_advice.Debug = "Not Using Open Raise Stat for " + player.PlayerName + "." + (Settings.CurrentSettings.UseOpponentSpecificStats ? "Total Hands (" + player.Stats.NBHands.ToString() + ") < " + Settings.CurrentSettings.OpponentXHands.ToString() : "") + ". AI OPEN RAISE: " + (double)open_raise_advice.ActualRaiseRange * 100;
                        open_raise_advice.Debug = "AI OPEN RAISE for " + player.PlayerName + ": " + (double)open_raise_advice.ActualRaiseRange * 100;
                    }
                    //openRaise = open_raise_advice.ActualRaiseRange;
                }
                // Calculate limp -range based on standard_raise_range but ignoring limpers (unless there are short stackers in which case use simple_raise_range, or actually don't eve consider short stackers because that could cause the formula to suggest open-limping)
                int players_yet_to_act = handHistory.Players.Count - (handHistory.Players[action.PlayerName] as Player).Position - 1;

                open_raise_advice.CallRange = Math.Min(standard_raise_range * (players_oop + players_ip + 1) / (players_yet_to_act + 1), 1.0f);
                if ((handHistory.Players[action.PlayerName] as Player).Position == handHistory.Players.Count - 1) open_raise_advice.CallRange = 1; // BB can check for free with his full range

                if (handHistory.Players.Count >= 7 && (sPosition.Equals("EP") || sPosition.Equals("MP")))
                {
                    if (sPosition.Equals("EP"))
                    {
                        Debug("players>7: Multiply Open Raising Ranges by 0.65 : " + player.PlayerName);
                        open_raise_advice.OptimalRaiseRange *= 0.65F;
                        open_raise_advice.ActualRaiseRange *= 0.65F;
                    }
                    else if (sPosition.Equals("MP"))
                    {
                        Debug("players>7: Multiply Open Raising Ranges by 0.8 : " + player.PlayerName);
                        open_raise_advice.OptimalRaiseRange *= 0.8F;
                        open_raise_advice.ActualRaiseRange *= 0.8F;
                    }
                }

                //if limped pot (un-raised pot), and hero is OOP, the raising range should be tighter (x 1.25)
                if (action.PlayerName == handHistory.HeroName && (sPosition.Equals("BB") || sPosition.Equals("SB")))
                {
                    bool isLimped = false;
                    foreach (Action postflopAction in handHistory.PreflopActions)
                    {
                        if (postflopAction == action)
                            break;

                        if (postflopAction.SAction.Equals("Calls"))
                            isLimped = true;

                        if (postflopAction.SAction.Equals("Raises"))
                        {
                            isLimped = false;
                            break;
                        }
                    }
                    if (isLimped)
                    {
                        //Debug("if limped pot (un-raised pot), and hero is OOP, the raising range should be tighter (x 1.25)");
                        //open_raise_advice.OptimalRaiseRange *= 1.25F;
                        //open_raise_advice.ActualRaiseRange *= 1.25F;
                    }
                }

                if (open_raise_advice.OptimalRaiseRange > 1) open_raise_advice.OptimalRaiseRange = 1;
                if (open_raise_advice.ActualRaiseRange > 1) open_raise_advice.ActualRaiseRange = 1;

                player.OpenRaiseRange = open_raise_advice.ActualRaiseRange;
                return open_raise_advice;
            }
        }

        RaiseAdvice re_raise(HandHistory handHistory, Action action, Hashtable collective_range, float squeeze_fold_p)
        {
            if (action.PlayerName == handHistory.HeroName)
            {
            }
            String sPosition = GameRules.position_names[handHistory.Players.Count, (handHistory.Players[action.PlayerName] as Player).Position];

            RaiseAdvice re_raise_advice = new RaiseAdvice();
            re_raise_advice.Debug = "";

            re_raise_advice.GametheoryCall = 0; // By default game theory calls are not used (since it only applies to pot defenders)
            re_raise_advice.StackDepth = GameRules.stack_depths.DEEP_STACK; // By default use deep stack rankings
            re_raise_advice.PocketPairOdds = false; // By default assume drawing hands don't get sufficient implied odds
            re_raise_advice.SuitedConnectorOdds = false;
            re_raise_advice.SuitedGapperOdds = false;

            // Find player's relative position, count the pot, to call -amount, effective stacks, and calculate collective hand range for opponents

            float collective_hand_range = 0;

            int players_oop = 0, players_ip = 0, eff_stacks = 0, pot = 0, to_call = 0;

            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                pot += Math.Min((int)action.LastStreetCommitment[key] + (int)action.ThisStreetCommitment[key], (handHistory.Players[action.PlayerName] as Player).StartingStack); // Don't count other players' side pots

                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                if (key == action.PlayerName) continue; // Skip hero

                if (inPosition(handHistory, action.PlayerName, key)) players_oop++;
                else players_ip++;

                if ((handHistory.Players[key] as Player).StartingStack > eff_stacks) eff_stacks = (handHistory.Players[key] as Player).StartingStack;
                if ((int)action.ThisStreetCommitment[key] > to_call) to_call = (int)action.ThisStreetCommitment[key];

                //collective_hand_range += 2.0008f/(collective_range[key]+0.0008f)-1; // How many random hands equals the range
                if (collective_hand_range == 0 || (float)Convert.ToDouble(collective_range[key]) + 0.0008f < collective_hand_range) collective_hand_range = (float)Convert.ToDouble(collective_range[key]) + 0.0008f;
            }
            eff_stacks = Math.Min(eff_stacks, (handHistory.Players[action.PlayerName] as Player).StartingStack);
            to_call -= (int)action.ThisStreetCommitment[action.PlayerName];
            //collective_hand_range = 2.0f/(collective_hand_range+1);

            // How much stack would be left if we call
            int stack_left = eff_stacks - to_call - (int)action.LastStreetCommitment[action.PlayerName] - (int)action.ThisStreetCommitment[action.PlayerName];
            if (stack_left < 0) // If someone raised more than we have stack left
            {
                to_call += stack_left;
                action.AttackerRisk += stack_left;
                stack_left = 0;
            }

            // Apply pot odds to calling range (assuming a single opponent)
            re_raise_advice.CallRange = collective_hand_range * (1 - 1.0f / (pot / (float)to_call + 1));

            if (action.Defender == action.PlayerName) // Player is closing the action
            {
                float virtual_risk = (float)Math.Min(Math.Sqrt(2.0f) * to_call, (float)stack_left + to_call) - to_call; // Tighten up the range based on the risk of future betting (assuming enough stackis still left)
                virtual_risk *= players_ip / (float)(players_oop + players_ip); // Use tighter range only when playing OOP, because we can't allow other OOP players being able to turn the hand +EV for them 
                if (action.Street == 3 || stack_left == 0) virtual_risk = 0; // Don't tighter the range if this is the last action in the hand (i.e. we are either on the river, or we are making a decision for the rest of our stack)

                // Villain is technically risking only 75-80% of his bet since even his bluffs will have ~25% pot equity (although he might also get re-raised and have to fold, therefore using only 20% when the raise is less than all-in)
                if (stack_left == 0) virtual_risk -= action.AttackerRisk * 0.25f; // All-in
                else virtual_risk -= action.AttackerRisk * 0.20f; // Less than all-in

                re_raise_advice.GametheoryCall = 1 - (action.AttackerRisk + virtual_risk) / (pot + virtual_risk);
                if (squeeze_fold_p > 0) re_raise_advice.GametheoryCall = Math.Max(1 - (1 - re_raise_advice.GametheoryCall) / squeeze_fold_p, 0.0f); // Multi-way game theory formula
            }
            else // Player is not closing the action
            {
                if (stack_left > 0) re_raise_advice.CallRange *= (float)(2 * (Math.Sqrt(2.0f) - 1)); // Tighten up the calling range for the risk of getting raised (and for future betting), unless it's an all-in call
            }

            // Apply position adjustment if at least sqrt(2) times the pot is still left for later
            if (stack_left - to_call >= Math.Sqrt(2.0f) * (pot + to_call))
            {
                // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP
                re_raise_advice.CallRange *= (float)Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip));
            }
            if (re_raise_advice.CallRange > 1.0f) re_raise_advice.CallRange = 1.0f;

            // See if we have sufficient implied odds to play weak drawing hands
            float implied_odds = to_call / (float)(pot + to_call + stack_left);
            float position_advantage = players_oop / (float)(players_oop + players_ip);
            if (implied_odds <= position_advantage / 14 + (1 - position_advantage) / 20) re_raise_advice.PocketPairOdds = true; //  ~14x remaining stacks (or  ~20x from OOP) required for calling w/ pocket pairs
            if (implied_odds <= position_advantage / 50 + (1 - position_advantage) / 70) re_raise_advice.SuitedConnectorOdds = true; //  ~50x remaining stacks (or  ~70x from OOP) required for calling w/ suited connectors
            if (implied_odds <= position_advantage / 100 + (1 - position_advantage) / 140) re_raise_advice.SuitedGapperOdds = true; // ~100x remaining stacks (or ~140x from OOP) required for calling w/ suited 1-gappers

            // How much to raise, if we raise
            int standard_raise_risk = pot + 2 * to_call; // start with a pot sized raises (although sqrt(2) would be good when raising from OOP)

            // Would the villain be able to push all-in for less than sqrt(2) times the pot if we make a standard pot sized raise?
            if (stack_left - to_call < (Math.Sqrt(2.0f) * 3 + 1) * (pot + to_call))
            {
                // If we have stacks left for less than 2*sqrt(2) times the pot sized raise, then we should just move all-in (or fold)
                if (stack_left - to_call < 2 * Math.Sqrt(2.0f) * (pot + to_call))
                {
                    // Calculate raise range based on all-in
                    re_raise_advice.OptimalRaiseRange = collective_hand_range * (1 - 1.0f / (pot / (float)(to_call + stack_left) + 1));
                    re_raise_advice.ActualRaiseRange = re_raise_advice.OptimalRaiseRange; // Actual raise range is based on the assumption that the player is willing to risk all his stack anyways
                    re_raise_advice.CallRange = re_raise_advice.OptimalRaiseRange; // No calling, as we either move all-in or fold

                    // Recommend all-in
                    re_raise_advice.RaiseSize = (int)action.ThisStreetCommitment[action.PlayerName] + to_call + stack_left; // "Raise to"

                    // Use short stack rankings if our all-in raise is less than sqrt(2) times the pot (assuming opponent is about pot committed)
                    //if (stack_left < sqrt(2.0f)*(pot+to_call)) re_raise_advice.short_stack_rankings = true;
                    if (stack_left < Math.Sqrt(2.0f) * (pot + to_call)) re_raise_advice.StackDepth = GameRules.stack_depths.SHORT_STACK;
                    // Otherwise we always use deep stack rankings because we need the card removal effect (e.g. A5s) for optimal semi-bluffing
                }
                else // Otherwise make a pot sized raise, but use a tighter range due to the fact that we'd be committed to call the push too
                {
                    // Use medium stack rankings to count for both possibilities, either we end-up calling an all-in preflop, or we have stack left for one flop all-in decision
                    re_raise_advice.StackDepth = GameRules.stack_depths.MEDIUM_STACK;

                    // Calculate raise range based on all-in
                    re_raise_advice.OptimalRaiseRange = collective_hand_range * (1 - 1.0f / (pot / (float)(to_call + stack_left) + 1));

                    // Calculate raise-range based on the actual raise size chosen by the player
                    re_raise_advice.ActualRaiseRange = collective_hand_range * (1 - 1.0f / (pot / (float)action.Amount + 1));

                    // Recommend pot sized raise (despite the range being tight enough for all-in)
                    re_raise_advice.RaiseSize = (int)action.ThisStreetCommitment[action.PlayerName] + standard_raise_risk; // "Raise to"
                    //GHADY REDUCE POT SIZE RAISE PREFLOP RERAISE
                    //re_raise_advice.RaiseSize = reduceRaiseSize?(int)Math.Round(((double)3 / (double)4) * re_raise_advice.RaiseSize):re_raise_advice.RaiseSize;
                }
            }
            else // If stacks are deep enough to ignore the above considerations
            {
                // Calculate raise range based on standard pot sized raise
                re_raise_advice.OptimalRaiseRange = collective_hand_range * (1 - 1.0f / (pot / (float)standard_raise_risk + 1));
                // Adjust by the risk of getting re-raised (and for future betting)
                re_raise_advice.OptimalRaiseRange *= (float)(2 * (Math.Sqrt(2.0f) - 1));
                // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP
                re_raise_advice.OptimalRaiseRange *= (float)Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip));

                // Calculate raise-range based on the actual raise size chosen by the player
                re_raise_advice.ActualRaiseRange = collective_hand_range * (1 - 1.0f / (pot / (float)action.Amount + 1));
                re_raise_advice.ActualRaiseRange *= (float)(2 * (Math.Sqrt(2.0f) - 1)); // Adjust by the risk of getting re-raised (and for future betting)
                re_raise_advice.ActualRaiseRange *= (float)Math.Pow(Math.Pow(Math.Sqrt(2.0f), players_oop) / Math.Pow(Math.Sqrt(2.0f), players_ip), 1.0f / (players_oop + players_ip)); // Apply multipliers for positional advantage, sqrt(2) if IP, 1/sqrt(2) if OOP

                // Recommend pot sized raise
                re_raise_advice.RaiseSize = (int)action.ThisStreetCommitment[action.PlayerName] + standard_raise_risk; // "Raise to"
                //GHADY REDUCE POT SIZE RAISE PREFLOP RERAISE
                //re_raise_advice.RaiseSize = reduceRaiseSize?(int)Math.Round(((double)3 / (double)4) * re_raise_advice.RaiseSize):re_raise_advice.RaiseSize;
            }


            //GHADY RE-RAISE ADD
            //INCREASE RAISING RANGE IF 4-bet
            int nbRaises = 0;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction == action) break;
                if (preflopAction.SAction.Equals("Raises"))
                    nbRaises++;
            }
            if (nbRaises >= 2)
            {
                re_raise_advice.OptimalRaiseRange *= 2.6F;
                re_raise_advice.ActualRaiseRange *= 2.6F;
            }
            //

            return re_raise_advice;
        }

        pot_equity equitySimulation1(HandHistory handHistory, Action action, Hashtable collective_range, int simulations, int street)
        // Overloaded function
        {
            // Convert hand_range to hand_distribution structure for equitySimulation function
            Hashtable hand_collective = new Hashtable();
            foreach (String key in collective_range.Keys)
            {
                hand_collective.Add(key, new hand_distribution()); //ATT
                try
                {
                    if (collective_range[key] is int)
                        (hand_collective[key] as hand_distribution).hand_range = (int)collective_range[key];
                    else

                        (hand_collective[key] as hand_distribution).hand_range = (float)Convert.ToDouble(collective_range[key]);
                }
                catch (Exception exc)
                {
                }
                // draw_range and draw_matrix don't matter yet, because they will be initialized in the next function when moving to the flop
            }
            return equitySimulation2(handHistory, action, hand_collective, simulations, street, false);
        }

        int GetNumberByCard(String card, int[] deck)
        {
            int temp_suit = 0;
            if (card[1] == 's') temp_suit = (int)GameRules.suit.SPADE;
            else if (card[1] == 'h') temp_suit = (int)GameRules.suit.HEART;
            else if (card[1] == 'd') temp_suit = (int)GameRules.suit.DIAMOND;
            else if (card[1] == 'c') temp_suit = (int)GameRules.suit.CLUB;
            int temp_location = FastEvaluator.find_card((int)Card.CardValues[card[0].ToString()], temp_suit, deck);
            return deck[temp_location];
        }

        String GetCardByNumber(int n, int[] deck)
        {
            String[] allCards = new String[] { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };
            String[] allSuits = new String[] { "s", "h", "c", "d" };

            foreach (String card in allCards)
            {
                foreach (String suit in allSuits)
                {
                    if (GetNumberByCard(card + suit, deck) == n)
                        return card + suit;
                }
            }
            return "";
        }

        pot_equity equitySimulation2(HandHistory handHistory, Action action, Hashtable collective_range, long simulations, int street, bool calculateAPDEquity)
        {
            StrongestOpponentHands = new List<String>();
            //return new pot_equity();
            Hashtable opponentCards = new Hashtable();
            //map<CString,hand_distribution> collective_range
            int[] deck = new int[52];
            FastEvaluator.init_deck(deck);

            // Move player's hole cards to the beginning of the deck
            for (int i = 0; i < 2; i++)
            {
                int temp_suit = 0;
                if ((handHistory.Players[action.PlayerName] as Player).Cards[2 * i + 1] == 's') temp_suit = (int)GameRules.suit.SPADE;
                else if ((handHistory.Players[action.PlayerName] as Player).Cards[2 * i + 1] == 'h') temp_suit = (int)GameRules.suit.HEART;
                else if ((handHistory.Players[action.PlayerName] as Player).Cards[2 * i + 1] == 'd') temp_suit = (int)GameRules.suit.DIAMOND;
                else if ((handHistory.Players[action.PlayerName] as Player).Cards[2 * i + 1] == 'c') temp_suit = (int)GameRules.suit.CLUB;
                int temp_location = FastEvaluator.find_card((int)Card.CardValues[(handHistory.Players[action.PlayerName] as Player).Cards[2 * i].ToString()], temp_suit, deck);
                int temp_value = deck[temp_location];
                deck[temp_location] = deck[i];
                deck[i] = temp_value;
            }

            // Move any board cards in the beginning of the deck as well
            String board_cards = "";
            if (street >= 1) board_cards += handHistory.CommunityCards[1]; // Flop cards
            if (street >= 2) board_cards += handHistory.CommunityCards[2]; // Turn cards
            if (street >= 3) board_cards += handHistory.CommunityCards[3]; // River cards
            for (int i = 0; i < board_cards.Length / 2; i++)
            {
                int temp_suit = 0;
                if (board_cards[2 * i + 1] == 's') temp_suit = (int)GameRules.suit.SPADE;
                else if (board_cards[2 * i + 1] == 'h') temp_suit = (int)GameRules.suit.HEART;
                else if (board_cards[2 * i + 1] == 'd') temp_suit = (int)GameRules.suit.DIAMOND;
                else if (board_cards[2 * i + 1] == 'c') temp_suit = (int)GameRules.suit.CLUB;
                int temp_location = FastEvaluator.find_card((int)Card.CardValues[board_cards[2 * i].ToString()], temp_suit, deck);
                int temp_value = deck[temp_location];
                deck[temp_location] = deck[2 + i];
                deck[2 + i] = temp_value;
            }

            pot_equity result = new pot_equity();

            // Maximum of 10 opponents allowed
            bool[,,] short_weight = new bool[10, 52, 52];
            bool[,,] deep_weight = new bool[10, 52, 52];
            float[,,] postflop_weight = new float[10, 52, 52];
            int strongest_villain = 0, villain_counter = 0;
            float strongest_range = -1;
            foreach (String key in action.ThisStreetCommitment.Keys)
            {
                if (!(bool)action.InHand[key]) continue; // Skip players already folded
                if (key == action.PlayerName) continue; // Skip hero

                float collective_hand_range = (collective_range[key] as hand_distribution).hand_range;

                if ((collective_range[key] as hand_distribution).hand_range <= 1) // If opponent's hand range is stronger than random hand, don't count the weakest possible hands
                {
                    if (street == 0) // Preflop
                    {
                        for (int i = 1; i < 13; i++)
                        {
                            if ((float)HandHistory.deepstack_percentile[Card.AllCards[i] + Card.AllCards[i]] <= collective_hand_range) deep_weight[villain_counter, i, i] = true;
                            if ((float)HandHistory.shortstack_percentile[Card.AllCards[i] + Card.AllCards[i]] <= collective_hand_range) short_weight[villain_counter, i, i] = true;
                            for (int j = 0; j < i; j++)
                            {
                                if ((float)HandHistory.deepstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "s"] <= collective_hand_range) deep_weight[villain_counter, i, j] = true;
                                if ((float)HandHistory.deepstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "o"] <= collective_hand_range) deep_weight[villain_counter, j, i] = true;
                                if ((float)HandHistory.shortstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "s"] <= collective_hand_range) short_weight[villain_counter, i, j] = true;
                                if ((float)HandHistory.shortstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "o"] <= collective_hand_range) short_weight[villain_counter, j, i] = true;
                            }
                        }
                    }
                    else // Post-flop
                    {
                        String playerCards = (handHistory.Players[handHistory.HeroName] as Player).Cards;
                        bool heroHasMadeHand = false;
                        if ((float)handHistory.absolute_percentile[playerCards] <= collective_hand_range) // Made hands
                        {
                            heroHasMadeHand = true;
                        }
                        result.MadeHands = heroHasMadeHand;
                        for (int i = 1; i < 52; i++)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                String card_str = Card.CardName[i].ToString() + Card.CardSuit[i].ToString() + Card.CardName[j].ToString() + Card.CardSuit[j].ToString();
                                if ((float)handHistory.absolute_percentile[card_str] <= collective_hand_range) // Made hands
                                {
                                    postflop_weight[villain_counter, i, j] = 1.0f;
                                    postflop_weight[villain_counter, j, i] = 1.0f;
                                }
                                else // Drawing hands
                                {
                                    postflop_weight[villain_counter, i, j] = (collective_range[key] as hand_distribution).draw_matrix[i, j];
                                    postflop_weight[villain_counter, j, i] = (collective_range[key] as hand_distribution).draw_matrix[j, i];
                                }
                            }
                        }
                    }
                }
                else // If opponent's hand range is weaker than random hand, don't count the strongest possible hands
                {
                    if (street == 0) // Preflop
                    {
                        for (int i = 1; i < 13; i++)
                        {
                            if ((float)HandHistory.deepstack_percentile[Card.AllCards[i] + Card.AllCards[i]] >= 1 - 1.0f / collective_hand_range) deep_weight[villain_counter, i, i] = true;
                            if ((float)HandHistory.shortstack_percentile[Card.AllCards[i] + Card.AllCards[i]] >= 1 - 1.0f / collective_hand_range) short_weight[villain_counter, i, i] = true;
                            for (int j = 0; j < i; j++)
                            {
                                if ((float)HandHistory.deepstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "s"] >= 1 - 1.0f / collective_hand_range) deep_weight[villain_counter, i, j] = true;
                                if ((float)HandHistory.deepstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "o"] >= 1 - 1.0f / collective_hand_range) deep_weight[villain_counter, j, i] = true;
                                if ((float)HandHistory.shortstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "s"] >= 1 - 1.0f / collective_hand_range) short_weight[villain_counter, i, j] = true;
                                if ((float)HandHistory.shortstack_percentile[Card.AllCards[i] + Card.AllCards[j] + "o"] >= 1 - 1.0f / collective_hand_range) short_weight[villain_counter, j, i] = true;
                            }
                        }
                    }
                    else // Post-flop
                    {
                        String playerCards = (handHistory.Players[handHistory.HeroName] as Player).Cards;
                        bool heroHasMadeHand = false;
                        if ((float)handHistory.absolute_percentile[playerCards] >= 1 - 1.0f / collective_hand_range)
                        {
                            heroHasMadeHand = true;
                        }
                        result.MadeHands = heroHasMadeHand;
                        for (int i = 1; i < 52; i++)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                String card_str = Card.CardName[i].ToString() + Card.CardSuit[i].ToString() + Card.CardName[j].ToString() + Card.CardSuit[j].ToString();
                                if ((float)handHistory.absolute_percentile[card_str] >= 1 - 1.0f / collective_hand_range) // Made hands
                                {
                                    postflop_weight[villain_counter, i, j] = 1.0f;
                                    postflop_weight[villain_counter, j, i] = 1.0f;
                                }
                                else // Drawing hands
                                {
                                    postflop_weight[villain_counter, i, j] = (collective_range[key] as hand_distribution).draw_matrix[i, j];
                                    postflop_weight[villain_counter, j, i] = (collective_range[key] as hand_distribution).draw_matrix[j, i];
                                }
                            }
                        }
                    }
                }

                if (collective_hand_range < strongest_range || strongest_range < 0)
                {
                    strongest_villain = villain_counter;
                    strongest_range = collective_hand_range;
                    result.strongest_man = key;
                }
                else if (strongest_range == collective_hand_range) result.strongest_man = ""; // Don't show the name if the strongest opponent is not unique

                villain_counter++;
            }

            int skip = 2 + board_cards.Length / 2; // How many cards in the deck are known (hero's hole cards, and board cards as needed)

            long wins_all = 0, deep_trials_all = 0, short_trials_all = 0, deep_wins_all = 0, short_wins_all = 0, postflop_trials_all = 0, postflop_wins_all = 0, suckout_trials_all = 0, suckout_wins_all = 0;
            long wins_hup = 0, deep_trials_hup = 0, short_trials_hup = 0, deep_wins_hup = 0, short_wins_hup = 0, postflop_trials_hup = 0, postflop_wins_hup = 0;



            Player strongestOpponentPlayer = handHistory.Players[result.strongest_man] as Player;
            if (strongestOpponentPlayer == null)
            {
                int lastStreet = handHistory.PostflopActions[3].Count > 1 ? 3 : handHistory.PostflopActions[2].Count > 1 ? 2 : 1;
                List<Player> allOpponents = GetPlayersInHand(handHistory, handHistory.PostflopActions[0].Count == 0 ? handHistory.PreflopActions[0] : handHistory.PostflopActions[lastStreet][0], false);
                strongestOpponentPlayer = allOpponents[0];
            }


            StrongestOpponentName = strongestOpponentPlayer.PlayerName;


            bool isLastAction = true;// IsLastActionInHand(handHistory, action) && calculateAPDEquity;

            bool strongestOpponentIsPreflopRaiser = false, strongestOpponentDid3BetPreflop = false, strongestOpponentDid4BetPreflop = false,
                strongestOpponentCalled3BetPreflop = false, strongestOpponentCalled4BetPreflop = false, strongestOpponentLimpedThenCall = false;
            Player opponentNotStrongestPreflopRaiser = null;

            List<int[]> simsDecks = new List<int[]>();
            List<String> handsToAddToRange = new List<String>();
            List<String> opponentHandRange = new List<String>();
            List<String> handsToTryBluffingWith = new List<String>();
            bool preflopIs3Bet = false, preflopIs4Bet = false;
            bool strongestOpponentBetOnTheFlop = false, strongestOpponentBetOnTheTurn = false, strongestOpponentBetOnTheRiver = false;
            bool strongestOpponentBetOrRaisedOnTheLastStreet = false;
            bool strongestOpponentCheckCalledOnTheFlop = false, strongestOpponentCheckCalledOnTheTurn = false, strongestOpponentCheckCalledOnTheRiver = false;
            int nbStreetsStrongestOpponentBetOn = 0;
            bool strongestOpponentCalledOffHisStackOnTheFlop = false, strongestOpponentCalledOffHisStackOnTheTurn = false, strongestOpponentCalledOffHisStackOnTheRiver = false;

            List<String> handsRemovedOnTheFlop = new List<String>(), handsRemovedOnTheTurn = new List<String>();

            int nbStraightsPossibleOnTheFlop = 0, nbFlushsPossibleOnTheFlop = 0, nbFlushsPossibleOnTheTurn = 0, nbStraightsPossibleOnTheTurn = 0;
            if (isLastAction)
            {

                strongestOpponentIsPreflopRaiser = strongestOpponentPlayer == null ? false : PlayerIsPreflopRaiser(handHistory, strongestOpponentPlayer, false);
                strongestOpponentDid3BetPreflop = strongestOpponentPlayer == null ? false : PlayerDid3BetPreflop(handHistory, strongestOpponentPlayer);
                strongestOpponentDid4BetPreflop = strongestOpponentPlayer == null ? false : PlayerDid4BetPreflop(handHistory, strongestOpponentPlayer);
                strongestOpponentCalled3BetPreflop = strongestOpponentPlayer == null ? false : PlayerCalled3BetPreflop(handHistory, strongestOpponentPlayer);
                strongestOpponentCalled4BetPreflop = strongestOpponentPlayer == null ? false : PlayerCalled4BetPreflop(handHistory, strongestOpponentPlayer);
                opponentNotStrongestPreflopRaiser = strongestOpponentPlayer == null ? null : PlayerCalledOpenRaisePreflop(handHistory, strongestOpponentPlayer);
                strongestOpponentLimpedThenCall = strongestOpponentPlayer == null ? false : PlayerLimpedThenCalledPreflop(handHistory, strongestOpponentPlayer);




                preflopIs3Bet = strongestOpponentDid3BetPreflop || strongestOpponentCalled3BetPreflop;
                preflopIs4Bet = strongestOpponentDid4BetPreflop || strongestOpponentCalled4BetPreflop;

                opponentHandRange = GetOpponentHandRange(handHistory, strongestOpponentPlayer,
                                        opponentNotStrongestPreflopRaiser,
                                        strongestOpponentDid3BetPreflop,
                                        strongestOpponentDid4BetPreflop,
                                        strongestOpponentIsPreflopRaiser,
                                        strongestOpponentCalled3BetPreflop,
                                        strongestOpponentCalled4BetPreflop,
                                        strongestOpponentLimpedThenCall);

                handsToAddToRange.AddRange(UngroupHands(opponentHandRange, handHistory));

                if (strongestOpponentIsPreflopRaiser)
                {
                    handsToTryBluffingWith = UngroupHands(GetHandsToOpenRaiseWith()["CO"] as List<String>, handHistory);
                    handsToAddToRange.AddRange(handsToTryBluffingWith);
                }
                else handsToTryBluffingWith.AddRange(handsToAddToRange);

                simulations = handsToAddToRange.Count;



                if (handHistory.PostflopActions[0].Count > 0)
                {
                    int[] flushsStraightsPossibleOnTheFlop = GetNbFlushStraightsPossible(handHistory, 1);
                    nbFlushsPossibleOnTheFlop = flushsStraightsPossibleOnTheFlop[0];
                    nbStraightsPossibleOnTheFlop = flushsStraightsPossibleOnTheFlop[1];

                    int[] flushsStraightsPossibleOnTheTurn = GetNbFlushStraightsPossible(handHistory, 2);
                    nbFlushsPossibleOnTheTurn = flushsStraightsPossibleOnTheTurn[0];
                    nbStraightsPossibleOnTheTurn = flushsStraightsPossibleOnTheTurn[1];

                    strongestOpponentBetOnTheFlop = PlayerBetOnStreet(handHistory, strongestOpponentPlayer, 1);
                    strongestOpponentBetOnTheTurn = PlayerBetOnStreet(handHistory, strongestOpponentPlayer, 2);
                    strongestOpponentBetOnTheRiver = PlayerBetOnStreet(handHistory, strongestOpponentPlayer, 3);



                    strongestOpponentCheckCalledOnTheFlop = PlayerCheckCalledOnStreet(handHistory, strongestOpponentPlayer, 1);
                    strongestOpponentCheckCalledOnTheTurn = PlayerCheckCalledOnStreet(handHistory, strongestOpponentPlayer, 2);
                    strongestOpponentCheckCalledOnTheRiver = PlayerCheckCalledOnStreet(handHistory, strongestOpponentPlayer, 3);

                    strongestOpponentBetOrRaisedOnTheLastStreet = PlayerBetOrRaisedOnStreet(handHistory, strongestOpponentPlayer, action.Street);

                    strongestOpponentCalledOffHisStackOnTheFlop = PlayerCalledOffHisStack(handHistory, strongestOpponentPlayer, 1);
                    strongestOpponentCalledOffHisStackOnTheTurn = PlayerCalledOffHisStack(handHistory, strongestOpponentPlayer, 2);
                    strongestOpponentCalledOffHisStackOnTheRiver = PlayerCalledOffHisStack(handHistory, strongestOpponentPlayer, 3);

                    nbStreetsStrongestOpponentBetOn = 0;
                    if (strongestOpponentBetOnTheFlop) nbStreetsStrongestOpponentBetOn++;
                    if (strongestOpponentBetOnTheTurn) nbStreetsStrongestOpponentBetOn++;
                    if (strongestOpponentBetOnTheRiver) nbStreetsStrongestOpponentBetOn++;
                }
            }
            Hashtable handsAlreadyAnalyzed = new Hashtable();
            double drawBluffPrct = strongestOpponentCalled3BetPreflop || strongestOpponentDid3BetPreflop ? 0.5 : 0.75;
            for (long kloops = 0; kloops < simulations; kloops++)
            {
                FastEvaluator.shuffle_deck(deck, skip); // Skip hero's hole cards since those are known (and board cards as needed)
                List<int> simsDeck = new List<int>(deck);
                simsDecks.Add(simsDeck.ToArray());

                //START TEST
                int[] hand_hero = new int[] { deck[0], deck[1], deck[2], deck[3], deck[4], deck[5], deck[6] };
                short rank_hero = FastEvaluator.eval_7hand(hand_hero);

                short suckout_rank_flop_hero = 0, suckout_rank_turn_hero = 0;
                if (street == 1) suckout_rank_flop_hero = FastEvaluator.eval_5hand(hand_hero); // Flop
                if (street == 1 || street == 2) suckout_rank_turn_hero = FastEvaluator.eval_6hand(hand_hero); // Turn

                int[] hand_villain = new int[] { 0, 0, deck[2], deck[3], deck[4], deck[5], deck[6] };
                short rank_villain = 9999, suckout_rank_flop_villain = 9999, suckout_rank_turn_villain = 9999;
                bool deep_valid = true, short_valid = true, postflop_valid = true; // Is this iteration valid with the given hand range criteria

                for (int i = 0; i < villain_counter; i++)
                {
                    hand_villain[0] = deck[7 + i * 2];
                    hand_villain[1] = deck[8 + i * 2];

                    if (i == strongest_villain)// && kloops > simulations - handsToAddToRange.Count)
                    {
                        hand_villain[0] = GetNumberByCard(handsToAddToRange[(int)kloops].Substring(0, 2), deck);// (int)((int)kloops - ((int)simulations - (int)handsToAddToRange.Count))].Substring(0, 2), deck);
                        hand_villain[1] = GetNumberByCard(handsToAddToRange[(int)kloops].Substring(2, 2), deck);//(int)kloops - ((int)simulations - handsToAddToRange.Count)].Substring(2, 2), deck);
                    }

                    short temp_rank = FastEvaluator.eval_7hand(hand_villain);
                    if (temp_rank < rank_villain) rank_villain = temp_rank;

                    if (street == 1)
                    {
                        short temp_suckout = FastEvaluator.eval_5hand(hand_villain); // Flop
                        if (temp_suckout < suckout_rank_flop_villain) suckout_rank_flop_villain = temp_suckout;
                    }
                    if (street == 1 || street == 2)
                    {
                        short temp_suckout = FastEvaluator.eval_6hand(hand_villain); // Turn
                        if (temp_suckout < suckout_rank_turn_villain) suckout_rank_turn_villain = temp_suckout;
                    }

                    // See if villain's hand was within the given percentile for weighted equity calculations (preflop)
                    int villain1, villain2;
                    if (FastEvaluator.SUIT(hand_villain[0]) == FastEvaluator.SUIT(hand_villain[1])) // Villain has a suited hand
                    {
                        villain1 = Math.Max(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                        villain2 = Math.Min(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                    }
                    else // Villain has offsuit hand or pocket pair
                    {
                        villain1 = Math.Min(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                        villain2 = Math.Max(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                    }

                    // (post-flop)
                    int post_villain1 = FastEvaluator.RANK(hand_villain[0]);
                    int post_villain2 = FastEvaluator.RANK(hand_villain[1]);
                    switch (FastEvaluator.SUIT(hand_villain[0]))
                    {
                        case (int)GameRules.suit.DIAMOND: post_villain1 += 13; break;
                        case (int)GameRules.suit.CLUB: post_villain1 += 26; break;
                        case (int)GameRules.suit.SPADE: post_villain1 += 39; break;
                    }
                    switch (FastEvaluator.SUIT(hand_villain[1]))
                    {
                        case (int)GameRules.suit.DIAMOND: post_villain2 += 13; break;
                        case (int)GameRules.suit.CLUB: post_villain2 += 26; break;
                        case (int)GameRules.suit.SPADE: post_villain2 += 39; break;
                    }

                    if (!deep_weight[i, villain1, villain2]) deep_valid = false;
                    if (!short_weight[i, villain1, villain2]) short_valid = false;
                    if (postflop_weight[i, post_villain1, post_villain2] == 0.0f) postflop_valid = false; //ATT

                    if (i == strongest_villain)
                    {
                        // Update equity against one random hand
                        if (rank_hero < temp_rank) wins_hup += 2; // Hero won
                        else if (rank_hero == temp_rank) wins_hup += 1; // Split pot

                        if (isLastAction)
                        {
                            String oppCard1 = GetCardByNumber(hand_villain[0], deck);
                            String oppCard2 = GetCardByNumber(hand_villain[1], deck);

                            if (handsAlreadyAnalyzed.ContainsKey(oppCard1 + oppCard2) || handsAlreadyAnalyzed.ContainsKey(oppCard2 + oppCard1))
                                continue;

                            handsAlreadyAnalyzed.Add(oppCard1 + oppCard2, "");


                            bool canTryBluffingWithHandOnTheFlop = (handsToTryBluffingWith.Contains(oppCard1 + oppCard2) || handsToTryBluffingWith.Contains(oppCard2 + oppCard1)) && !CheckCheckScenarioOnStreet(handHistory, strongestOpponentPlayer, 1);
                            bool canTryBluffingWithHandOnTheTurn = (handsToTryBluffingWith.Contains(oppCard1 + oppCard2) || handsToTryBluffingWith.Contains(oppCard2 + oppCard1)) && !CheckCheckScenarioOnStreet(handHistory, strongestOpponentPlayer, 2);
                            bool canTryBluffingWithHandOnTheRiver = (handsToTryBluffingWith.Contains(oppCard1 + oppCard2) || handsToTryBluffingWith.Contains(oppCard2 + oppCard1)) && PlayerBetOrRaisedOnStreet(handHistory, strongestOpponentPlayer, 3);
                            if (strongestOpponentCalledOffHisStackOnTheFlop) canTryBluffingWithHandOnTheFlop = false;
                            if (strongestOpponentCalledOffHisStackOnTheTurn)
                            {
                                canTryBluffingWithHandOnTheFlop = false;
                                canTryBluffingWithHandOnTheTurn = false;
                            }
                            if (strongestOpponentCalledOffHisStackOnTheRiver)
                            {
                                canTryBluffingWithHandOnTheFlop = false;
                                canTryBluffingWithHandOnTheTurn = false;
                                canTryBluffingWithHandOnTheRiver = false;
                            }
                            //if (street == 3) canTryBluffingWithHandOnTheFlop = canTryBluffingWithHandOnTheTurn = canTryBluffingWithHandOnTheRiver;
                            //else if (street == 2) canTryBluffingWithHandOnTheFlop = canTryBluffingWithHandOnTheTurn;

                            bool canTryBluffingWithHand = (canTryBluffingWithHandOnTheFlop || canTryBluffingWithHandOnTheTurn || canTryBluffingWithHandOnTheRiver) && (handsToTryBluffingWith.Contains(oppCard1 + oppCard2) || handsToTryBluffingWith.Contains(oppCard2 + oppCard1));

                            String groupedHand1 = oppCard1[0].ToString() + oppCard2[0].ToString() + (oppCard1[0].Equals(oppCard2[0]) ? "" : oppCard1[1].Equals(oppCard2[1]) ? "s" : "o");
                            String groupedHand2 = oppCard2[0].ToString() + oppCard1[0].ToString() + (oppCard1[0].Equals(oppCard2[0]) ? "" : oppCard1[1].Equals(oppCard2[1]) ? "s" : "o");


                            bool dontRemoveHand = false;
                            if (street > 0 &&
                                (
                                 //((deep_weight[i, villain1, villain2]
                                 //|| short_weight[i, villain1, villain2]
                                 //|| postflop_weight[i, post_villain1, post_villain2] == 1))
                                 opponentHandRange.Contains(groupedHand1) || opponentHandRange.Contains(groupedHand2)
                                ||
                                 canTryBluffingWithHand
                                ))
                            {
                                if (groupedHand1.Length == 2)
                                {
                                }
                                if (canTryBluffingWithHand)
                                {
                                }
                                if (!canTryBluffingWithHand && !opponentHandRange.Contains(groupedHand1) && !opponentHandRange.Contains(groupedHand2))
                                {
                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 0);
                                }
                                else
                                {
                                    String topCard;

                                    //Rule: Opponents won't bet two streets with small under pairs on draw heavy boards,
                                    //unless it creates an OESD. Smal pairs like 22-88 should not be in opponents hand range here.
                                    //He wouldn't bet 33 on the flop. So it shouldn't list on the turn.
                                    if (nbStreetsStrongestOpponentBetOn == 2 && (nbFlushsPossibleOnTheTurn > 16 || nbStraightsPossibleOnTheTurn > 16))
                                    {
                                        String cardThatMadeAPair = null;
                                        if ((cardThatMadeAPair = HandIsPaired(handHistory, oppCard1, oppCard2, 1)) != null)
                                        {
                                            if (Card.AllCardsList.IndexOf(cardThatMadeAPair[0].ToString()) <= Card.AllCardsList.IndexOf("8") && (!canMakeOESDWithHand(handHistory, oppCard1, oppCard2, 1) || strongestOpponentCalledOffHisStackOnTheFlop))
                                            {
                                                if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                    handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                            }
                                            else if (Card.AllCardsList.IndexOf(cardThatMadeAPair[0].ToString()) <= Card.AllCardsList.IndexOf("8"))
                                            {
                                            }
                                        }
                                        else if (!HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false))
                                        {
                                            if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                        }
                                    }


                                    //if they bet all 3 streets
                                    //then top pair hands that would have been made from the flop on
                                    if (action.Street == 3 && (nbStreetsStrongestOpponentBetOn == 3 || (strongestOpponentBetOnTheTurn && strongestOpponentBetOnTheRiver)))
                                    {
                                        drawBluffPrct = 0.35;

                                        if ((nbStreetsStrongestOpponentBetOn == 3
                                            && HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 1, false, false, out topCard, false, false)
                                            && HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false)
                                            && HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 3, false, false, out topCard, false, false))
                                            //||
                                            //(strongestOpponentBetOnTheTurn && strongestOpponentBetOnTheRiver
                                            //&& handIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, out topCard,false)
                                            //&& handIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 3, out topCard,false))
                                            )
                                        {

                                            dontRemoveHand = true;
                                        }
                                        else
                                        {
                                            //if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                            //   handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, "");
                                        }

                                        if (canTryBluffingWithHandOnTheFlop && canMakeDrawWithHand(handHistory, oppCard1, oppCard2, 1))
                                        {
                                            if (!bluffHands.ContainsKey(oppCard1 + oppCard2))
                                            {
                                                bluffHands.Add(oppCard1 + oppCard2, 1);
                                            }
                                        }
                                    }

                                    if (groupedHand1.Equals("AJo"))
                                    {
                                    }

                                    #region FLOP CUSTOM EQUITY
                                    //FLOP
                                    if (canTryBluffingWithHandOnTheFlop && canMakeDrawWithHand(handHistory, oppCard1, oppCard2, 1))
                                    {
                                        if (!bluffHands.ContainsKey(oppCard1 + oppCard2))
                                        {
                                            bluffHands.Add(oppCard1 + oppCard2, 1);
                                        }
                                    }

                                    bool checkCheckScenarioOnFlop = CheckCheckScenarioOnStreet(handHistory, strongestOpponentPlayer, 1);
                                    bool checkCheckScenarioOnTurn = CheckCheckScenarioOnStreet(handHistory, strongestOpponentPlayer, 2);
                                    bool checkCheckScenarioOnRiver = CheckCheckScenarioOnStreet(handHistory, strongestOpponentPlayer, 3);

                                    bool dontBluffWithHand = false;

                                    //in a 3-bet pot... opponent/ hero can call one bet on the flop with a 50% of their draws,
                                    //and an under pair (pocket pair) thats' TT+, and middle pair
                                    //[10:15:41 PM] John Anhalt: everything else should be excluded
                                    if (preflopIs3Bet && !checkCheckScenarioOnFlop)
                                    {
                                        drawBluffPrct = 0.5;
                                        if ((Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf("T")
                                            && oppCard1[0].Equals(oppCard2[0]))
                                            ||
                                            (HandHasMiddlePairOnStreet(handHistory, 1, oppCard1, oppCard2))
                                            )
                                        {

                                        }
                                        else
                                        {
                                            if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);

                                            if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                                bluffHands.Remove(oppCard1 + oppCard2);
                                        }
                                    }

                                    //if opponent WAS NOT the pre-flop aggressor... BUT CHECKED TO HERO ON THE FLOP
                                    //[1:25:47 AM] John Anhalt: then they CAN have turned draws or pairs in their range
                                    else if (!PlayerIsPreflopAggressor(handHistory, strongestOpponentPlayer)
                                        && checkCheckScenarioOnFlop && handHistory.PostflopActions[1][0].PlayerName.Equals(strongestOpponentPlayer.PlayerName))
                                    {
                                    }
                                    else
                                        //CHECK CHECK SCENARIO ON THE FLOP
                                        if (checkCheckScenarioOnFlop)
                                    {
                                        String comCards = handHistory.CommunityCards[1];

                                        //REMOVE TOP PAIR OR BETTER
                                        topCard = null;
                                        if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 1, false, false, out topCard, false, false))
                                        {
                                            //if the top pair was an ace
                                            //[9:30:54 PM] John Anhalt: I would not have opponents range include AT+ on the turn
                                            //[9:31:06 PM] John Anhalt: sometimes opponents will check weak top pair hands... like A4 for example
                                            //[9:31:17 PM] John Anhalt: so we'd have to include A9-A2 on the turn
                                            //[9:31:36 PM] John Anhalt: in the check/check scenario
                                            bool removeHand = true;
                                            if (topCard != null)
                                            {
                                                String otherCard = oppCard1[0].ToString().Equals(topCard) ? oppCard2[0].ToString() : oppCard1[0].ToString();
                                                if (Card.AllCardsList.IndexOf(otherCard) <= Card.AllCardsList.IndexOf("9"))
                                                {
                                                    removeHand = false;
                                                }
                                            }

                                            // if hero checks the flop... and the flop doesn't contain 3 or 2 of the same suit, or lots of straight draw type hands on the flop
                                            //that opponent can check 3 of a kind hands
                                            boardinfo boardInfo = new boardinfo();
                                            if (
                                                ((
                                                handHistory.CommunityCards[1][1] != handHistory.CommunityCards[1][3] &&
                                                handHistory.CommunityCards[1][1] != handHistory.CommunityCards[1][5] &&
                                                handHistory.CommunityCards[1][3] != handHistory.CommunityCards[1][5])
                                                && nbStraightsPossibleOnTheFlop < 16) &&
                                                (boardInfo = Jacob.AnalyzeHandCustomHoleCards(handHistory, 1, true, oppCard1, oppCard2)).madehand == postflophand.k3ofKind
                                                && boardInfo.holesused > 0)
                                            {
                                                removeHand = false;
                                            }

                                            if (removeHand && !dontRemoveHand)
                                            {
                                                if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                    handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 1);

                                                if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                                    bluffHands.Remove(oppCard1 + oppCard2);
                                            }
                                        }
                                    }
                                    else //OPPONENT RAISED, CALLED, BET
                                    {
                                        //if opponent is the pre-flop aggressor... meaning they raised, re-raised... they were the person with the last aggressive action
                                        //[1:25:27 AM] John Anhalt: and they bet the flop, hero calls
                                        //[1:25:47 AM] John Anhalt: then they CAN have turned draws or pairs in their range
                                        bool strongestPlayerIsPreflopAggressorAndBetCallScenarioOnTheFlop = (PlayerIsPreflopAggressor(handHistory, strongestOpponentPlayer) && BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, true, true));
                                        if (strongestPlayerIsPreflopAggressorAndBetCallScenarioOnTheFlop)
                                        {
                                            drawBluffPrct = 0.25;
                                        }
                                        else drawBluffPrct = 0.5;


                                        //on paired flops, with no draws... we need to add all pairs 22-AA + AJo+/AJs+ to opponents calling range (if they call a bet on the flop)
                                        List<String> handsToCallWithOnPairedFlopsWithNoDraws = new List<String>(new String[]{
                                                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA",
                                                "AJo","AQo","AKo",
                                                "AJs","AQs","AKs"
                                            });

                                        topCard = null;

                                        if ((handHistory.CommunityCards[1][0].Equals(handHistory.CommunityCards[1][2])
                                         || handHistory.CommunityCards[1][0].Equals(handHistory.CommunityCards[1][4])
                                            || handHistory.CommunityCards[1][2].Equals(handHistory.CommunityCards[1][4])) //PAIRED BOARD
                                            && !strongestOpponentBetOnTheFlop && !canMakeDrawWithHand(handHistory, oppCard1, oppCard2, 1)
                                            && (handsToCallWithOnPairedFlopsWithNoDraws.Contains(groupedHand1) || handsToCallWithOnPairedFlopsWithNoDraws.Contains(groupedHand2))) //CALLED FLOP AND NO DRAWS
                                        {

                                        }


                                        //if there was bet/call... or check/bet call... or anything that involved a bet or raise
                                        //[1:14:32 AM] John Anhalt: the hand range for opponent would need to include top pair or better... some middlepair ++ pocket pairs and bottom set... and 50% of draws on the turn/river
                                        //[1:14:57 AM] John Anhalt: this is IF there was a bet and call, or raise on the flop
                                        //[1:15:03 AM] John Anhalt: anything except for check/check

                                        //50% DRAWS
                                        else if (canTryBluffingWithHandOnTheFlop && canMakeDrawWithHand(handHistory, oppCard1, oppCard2, 1))
                                        {
                                            if (!bluffHands.ContainsKey(oppCard1 + oppCard2))
                                            {
                                                bluffHands.Add(oppCard1 + oppCard2, 1);
                                            }
                                        }//SMALL PAIRS
                                        else if (oppCard1[0].Equals(oppCard2[0]) &&
                                            (
                                            (GetMiddleCardOnStreet(handHistory, 1) != null && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf(GetMiddleCardOnStreet(handHistory, 1)[0].ToString()))
                                            ||
                                            (GetBottomCardOnStreet(handHistory, 1) != null && oppCard1[0].Equals(GetBottomCardOnStreet(handHistory, 1)[0]))
                                            )
                                           )
                                        {

                                        }
                                        else if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 1, false, false, out topCard, false, false))
                                        {

                                        }
                                        else //REMOVE HAND
                                        {
                                            //if opponent is the pre-flop aggressor... meaning they raised, re-raised... they were the person with the last aggressive action
                                            //[1:25:27 AM] John Anhalt: and they bet the flop, hero calls
                                            //[1:25:47 AM] John Anhalt: then they CAN have turned draws or pairs in their range

                                            if (strongestPlayerIsPreflopAggressorAndBetCallScenarioOnTheFlop && oppCard1[0].Equals(oppCard2[0]))
                                            {

                                            }
                                            //so in a 3-bet pot... let's also include that TT+ would call ONE bet on the flop
                                            //ONLY IF CHECK/CHECK ON THE TURN AND RIVER
                                            else if ((preflopIs3Bet || preflopIs4Bet) && oppCard1[0].Equals(oppCard2[0]) && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf("T")
                                                && ((action.Street > 2 && checkCheckScenarioOnTurn && checkCheckScenarioOnRiver) || (action.Street == 2 && checkCheckScenarioOnTurn))
                                                )
                                            {

                                            }
                                            //rule: single raised pots pre-flop...
                                            //opponent will call 1 bet on the flop with pocket pairs that are middle pair plus.
                                            else if (PotIsSingleRaisedPreflop(handHistory)
                                                && BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, false, true)  //OPPONENT CALLED A BETON THE FLOP
                                                && oppCard1[0].Equals(oppCard2[0])
                                                && GetMiddleCardOnStreet(handHistory, 1) != null
                                                && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf(GetMiddleCardOnStreet(handHistory, 1)[0].ToString())
                                                )
                                            {
                                            }
                                            else
                                            {
                                                if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2) && !dontRemoveHand)
                                                    handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 1);
                                            }
                                        }

                                    }
                                    #endregion


                                    #region TURN CUSTOM EQUITY
                                    //TURN
                                    if (action.Street >= 2)
                                    {
                                        if (canTryBluffingWithHandOnTheTurn && canMakeDrawWithHand(handHistory, oppCard1, oppCard2, 2))
                                        {
                                            if (!bluffHands.ContainsKey(oppCard1 + oppCard2))
                                            {
                                                bluffHands.Add(oppCard1 + oppCard2, 2);
                                            }
                                        }
                                        if (checkCheckScenarioOnTurn)
                                        {
                                            String comCards = handHistory.CommunityCards[1];
                                            comCards += handHistory.CommunityCards[2];

                                            //REMOVE TOP PAIR OR BETTER
                                            topCard = null;

                                            bool removeHand = true;


                                            //if the action is bet, raise, re-raise, call on the flop.
                                            //And then goes ceck /check on the turn.
                                            //Opponent can have a good top pair hand AQ+,
                                            //and two pair (in this case AK).
                                            //Also, as an additional rule...
                                            //if the board pairs on the turn in a situation like this,
                                            //opponent can have a set (full house or quads)...
                                            //so KK,AA and 99 would be in the range also.
                                            if (RaiseReraiseCallOnStreet(handHistory, 1))
                                            {
                                                //GOOD TOP PAIRS
                                                String highestBoardCard = GetHighestCardOnTheBoard(handHistory, 1);
                                                if (highestBoardCard != null && (oppCard1[0].Equals(highestBoardCard[0]) || oppCard2[0].Equals(highestBoardCard[0])))
                                                {
                                                    if (oppCard1[0] != oppCard2[0])
                                                    {
                                                        if (oppCard1[0].Equals(highestBoardCard[0]) && Card.AllCardsList.IndexOf(oppCard2[0].ToString()) >= Card.AllCardsList.IndexOf("Q"))
                                                        {
                                                            dontRemoveHand = true;
                                                        }
                                                        else if (oppCard2[0].Equals(highestBoardCard[0]) && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf("Q"))
                                                        {
                                                            dontRemoveHand = true;
                                                        }
                                                    }
                                                }


                                                //2 PAIRS
                                                boardinfo allBoardInfoOnTheFlop = Jacob.AnalyzeHand(handHistory, 1, oppCard1, oppCard2);
                                                if (allBoardInfoOnTheFlop.madehand == postflophand.k2Pair)
                                                {
                                                    dontRemoveHand = true;
                                                }


                                                boardinfo boardInfoOnTheFlop = Jacob.AnalyzeHand(handHistory, 1, false);
                                                boardinfo boardInfoOnTheTurn = Jacob.AnalyzeHand(handHistory, 2, false);
                                                if (boardInfoOnTheFlop.madehand == postflophand.kNoPair && boardInfoOnTheTurn.madehand == postflophand.kPair)
                                                {
                                                    boardinfo allBoardInfoOnTheTurn = Jacob.AnalyzeHand(handHistory, 2, oppCard1, oppCard2);
                                                    if (allBoardInfoOnTheTurn.madehand == postflophand.kFullHouse || allBoardInfoOnTheTurn.madehand == postflophand.k4ofKind)
                                                    {
                                                        dontRemoveHand = true;
                                                    }
                                                }

                                                if (!dontRemoveHand)
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                }
                                            }


                                            //If opponent raises, and there's no reasonable draws on the flop,
                                            //then they won't check hands like JJ+ below.
                                            //Hand they may check are top pair + if the turn card is higher than
                                            //the highest flop card (like in this example), they might check over
                                            //pairs like 88-TT. So there would be a lot more mid pairs +
                                            //7x and a couple of pure bluffs (if the action was bet / raise/call...
                                            //and then check/check). Pure bluffs we could add are bit Ax hands like AK-AJ.
                                            //
                                            //What I mean is, on the turn, the highest card has no become to a J.
                                            //I meant to say, they would check TT and below (unless they had a set),
                                            //and they wouldn't check JJ+ on the turn.
                                            //So if they did check the turn, their range would become a lot more pairs
                                            //that are under JJ, 7x, some pure bluffs.
                                            else if (PlayerRaisedOnStreet(handHistory, strongestOpponentPlayer, 1))
                                            {
                                                //OPPONENT RAISED ON THE FLOP AND CHECKED ON THE TURN    
                                                if (nbStraightsPossibleOnTheFlop == 0 && nbFlushsPossibleOnTheFlop == 0)
                                                {
                                                    if (!HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false)
                                                        )
                                                    {
                                                        String highestCardOnTheTurn = GetHighestCardOnTheBoard(handHistory, 2);
                                                        String highestCardOnTheFlop = GetHighestCardOnTheBoard(handHistory, 1);

                                                        if (highestCardOnTheTurn != null && highestCardOnTheTurn.Equals(handHistory.CommunityCards[2]))
                                                        {
                                                            //TOP PAIR ON THE FLOP
                                                            if (!oppCard1[0].Equals(oppCard2[0]) && (oppCard1[0].Equals(highestCardOnTheFlop[0]) || oppCard2[0].Equals(highestCardOnTheFlop[0])))
                                                            {
                                                                dontRemoveHand = true;
                                                            }//POCKET PAIR UNDER JJ
                                                            else if (oppCard1[0].Equals(oppCard2[0]) && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) < Card.AllCardsList.IndexOf(highestCardOnTheTurn[0].ToString()))
                                                            {
                                                                dontRemoveHand = true;
                                                            }//MIDDLE PAIRS
                                                            else if (HandIsPaired(handHistory, oppCard1, oppCard2, 1) != null)
                                                            {
                                                                dontRemoveHand = true;
                                                            }
                                                            else if (//GetRandom(1, 12) < 5
                                                                 (
                                                                (oppCard1[0].Equals('A') && Card.AllCardsList.IndexOf(oppCard2[0].ToString()) >= Card.AllCardsList.IndexOf("J"))
                                                                ||
                                                                (oppCard2[0].Equals('A') && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf("J"))
                                                                )
                                                                )//PURE BLUFF
                                                            {
                                                                dontRemoveHand = true;
                                                            }
                                                            else
                                                            {
                                                                //REMOVE HAND

                                                                if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                                    handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                    }
                                                }

                                                if (dontRemoveHand)
                                                {
                                                    if (handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Remove(oppCard1 + oppCard2);
                                                    if (handsToRemoveFromEquityCalculation.ContainsKey(oppCard2 + oppCard1))
                                                        handsToRemoveFromEquityCalculation.Remove(oppCard2 + oppCard2);

                                                }
                                            }
                                            //




                                            if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false))
                                            {
                                                //if the top pair was an ace
                                                //[9:30:54 PM] John Anhalt: I would not have opponents range include AT+ on the turn
                                                //[9:31:06 PM] John Anhalt: sometimes opponents will check weak top pair hands... like A4 for example
                                                //[9:31:17 PM] John Anhalt: so we'd have to include A9-A2 on the turn
                                                //[9:31:36 PM] John Anhalt: in the check/check scenario
                                                if (topCard != null)
                                                {
                                                    String otherCard = oppCard1[0].ToString().Equals(topCard) ? oppCard2[0].ToString() : oppCard1[0].ToString();
                                                    if (Card.AllCardsList.IndexOf(otherCard) <= Card.AllCardsList.IndexOf("9"))
                                                    {
                                                        removeHand = false;
                                                    }

                                                }


                                                //An opponent or hero will check the turn with middle or bottom pair IF they called
                                                //a bet on the flop. So we need to include middle and bottoms pairs that fit the pre-flop range.
                                                if (removeHand && (HandHasBottomPairOnStreet(handHistory, 2, oppCard1, oppCard2) || HandHasMiddlePairOnStreet(handHistory, 1, oppCard1, oppCard2))
                                                    &&
                                                    BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, false, true))
                                                {
                                                    removeHand = false;
                                                }


                                                //If an opponent is out of position,
                                                //they can check/call the flop and then check the turn with top pair
                                                //or better, but won't do this on the river.
                                                if (removeHand && handHistory.PostflopActions[2][0].PlayerName.Equals(strongestOpponentPlayer.PlayerName) //OPPONENT IS OUT OF POSITION
                                                    && PlayerCheckCalledOnStreet(handHistory, strongestOpponentPlayer, 1))
                                                {
                                                    removeHand = false;
                                                }

                                                //In a 3-bet pot... If the flop is not draw heavy,
                                                //then an opponent could check the turn with top pair or better.
                                                if (removeHand && (preflopIs3Bet || preflopIs4Bet)
                                                    && nbStraightsPossibleOnTheTurn < 16
                                                    && nbFlushsPossibleOnTheTurn < 16)
                                                {
                                                    removeHand = false;
                                                }


                                                //If hero bets the flop, opponent calls, and hero checks turn, and opponent checks turn
                                                //opponent can have top pair or better in the range.
                                                //However, if the turn makes the board very draw heavy
                                                //then they would not have 3 of a kind in their range.
                                                //It would be top pair, middle pair, draws, or straights and flushes
                                                //that they made on the turn.
                                                if (removeHand && !checkCheckScenarioOnFlop && BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, false, true))
                                                {
                                                    boardinfo boardInfo;
                                                    if (nbStraightsPossibleOnTheTurn > 16 && nbFlushsPossibleOnTheTurn > 16
                                                        && (nbStraightsPossibleOnTheTurn > nbStraightsPossibleOnTheFlop || nbFlushsPossibleOnTheTurn > nbFlushsPossibleOnTheFlop)
                                                        && (boardInfo = Jacob.AnalyzeHandCustomHoleCards(handHistory, 2, true, oppCard1, oppCard2)).madehand == postflophand.k3ofKind
                                                        && boardInfo.holesused > 0)
                                                    {
                                                        removeHand = true;
                                                    }
                                                    else removeHand = false;
                                                }


                                                //if the flop is re-raised... it's ok to have pairs if the turn is checked
                                                if (!removeHand && PostflopStreetIsReraised(handHistory, 1)) removeHand = false;




                                                if (removeHand && !dontRemoveHand)
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);

                                                    if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                                        bluffHands.Remove(oppCard1 + oppCard2);
                                                }
                                            }
                                        }//END IF CHECK CHECK ON THE TURN


                                        if (checkCheckScenarioOnFlop)
                                        {
                                            if ((BetCallScenarioOnStreet(handHistory, 2, strongestOpponentPlayer, true, false) || BetCallScenarioOnStreet(handHistory, 2, strongestOpponentPlayer, false, false)) && GetSuitWithXOccurrencesOnTheBoard(handHistory, 2, 3) != null)
                                            {
                                                //There actually should be more 9x hands in an opponent range (middle and bottom pair hands)
                                                //when the action goes check/check on the flop, and then bet/call on the turn,
                                                //than hands that complete a 4 board flush. A 4 board flush means that a player
                                                //has only one card in their hand to make a flush, and there are 4 of the
                                                //flush cards on the board. Hands like As2x etc... would make more sense
                                                //than AJo for example. So if we can add some logic in that would account for
                                                //this that would be good.


                                                List<String> middleBoardCards = GetMiddleBoardCardsBetweenTopAndBottom(handHistory, 2);
                                                if (!HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false))
                                                {
                                                    foreach (String middleBoardCard in middleBoardCards)
                                                    {
                                                        if ((oppCard1[0] == middleBoardCard[0] || oppCard2[0] == middleBoardCard[0]) && !oppCard1.Equals(oppCard2))
                                                        {
                                                            dontRemoveHand = true;
                                                            break;
                                                        }
                                                    }
                                                    if (!dontRemoveHand)
                                                    {
                                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                    }
                                                }
                                                else
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                }
                                            }
                                            else if (PlayerCheckRaisedOnStreet(handHistory, handHistory.Players[handHistory.HeroName] as Player, 2))
                                            {
                                                //if the action goes check/check on the flop... and then check/bet/raise on the turn
                                                //[1:09:26 AM] John Anhalt: Then we would limit the range (because we are elininating sets, top pair on the flop)
                                                //[1:10:13 AM] John Anhalt: we would change the range though to, the two bottom sets that are possible on the flop...  so 22,99
                                                //[1:10:41 AM] John Anhalt: and turned two pairs that don't include top pair... so 89
                                                //[1:11:09 AM] John Anhalt: straights, like TJ
                                                //[1:11:23 AM] John Anhalt: and pair plus nut flush draw... like A2s,A9s,A8s
                                                //[1:11:34 AM] John Anhalt: does that make sense w/o getting too confusing?
                                                //[1:11:57 AM] Ghady Diab: what's nut again?
                                                //[1:13:14 AM] John Anhalt: nut flush draw is a hand that has the A that would make the flush
                                                //[1:13:23 AM] John Anhalt: so in this hand it's the As
                                                //[1:13:37 AM] John Anhalt: if the board was: Qc 2s 9c 8c
                                                //[1:13:39 AM] John Anhalt: it would be the Ac
                                                bool bottomSet = false;
                                                List<String> bottomBoardCards = GetBottomCardsOnFlop(handHistory);
                                                foreach (String bottomCard in bottomBoardCards)
                                                {
                                                    if (oppCard1[0].Equals(oppCard2[0]) && oppCard1[0].Equals(bottomCard[0]))
                                                    {
                                                        bottomSet = true;
                                                        break;
                                                    }
                                                }

                                                if (opponentHandRange.Contains("98") || opponentHandRange.Contains("89"))
                                                {
                                                }
                                                if (groupedHand1.StartsWith("89") || groupedHand1.StartsWith("98"))
                                                {
                                                }
                                                bool turned2PairsThatDontIncludeTopPairs = false;
                                                if (!HandIsTopPairOrBetter(handHistory, oppCard1, oppCard1, 2, false, false, out topCard, false, false))
                                                {
                                                    String turnCard = handHistory.CommunityCards[2];
                                                    if ((oppCard1[0].Equals(turnCard[0]) || oppCard2[0].Equals(turnCard[0]))
                                                        && (
                                                            (oppCard1[0].Equals(handHistory.CommunityCards[0][0]) || oppCard1[0].Equals(handHistory.CommunityCards[0][2]) || oppCard1[0].Equals(handHistory.CommunityCards[0][4]))
                                                          ||
                                                            (oppCard2[0].Equals(handHistory.CommunityCards[0][0]) || oppCard2[0].Equals(handHistory.CommunityCards[0][2]) || oppCard2[0].Equals(handHistory.CommunityCards[0][4]))
                                                        ))
                                                    {
                                                        turned2PairsThatDontIncludeTopPairs = true;
                                                    }
                                                }
                                                bool madeHands = HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, street, false, false, out topCard, false, false);
                                                bool straight = HandIsStraight(handHistory, oppCard1, oppCard2, 2);

                                                String board = handHistory.CommunityCards[0] + handHistory.CommunityCards[1];
                                                bool flushNotWithPair = (board.Contains(oppCard1[0].ToString()) || board.Contains(oppCard2[0].ToString())) && HandIsNutFlushDraw(handHistory, oppCard1, oppCard2, 2);

                                                if (groupedHand1.Contains("33"))
                                                {
                                                }
                                                dontBluffWithHand = true;
                                                if (bottomSet || turned2PairsThatDontIncludeTopPairs || madeHands || straight || flushNotWithPair)
                                                {
                                                    dontRemoveHand = true;
                                                }
                                                else
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                }
                                            }
                                            //flop went check/check
                                            //[1:23:15 AM] John Anhalt: then the turn was 6h
                                            //[1:23:23 AM] John Anhalt: and hero bet, opponent called
                                            //[1:23:34 AM] John Anhalt: there would be 50% of oesd and flush draws
                                            //[1:23:37 AM] John Anhalt: in the range
                                            //[1:23:42 AM] John Anhalt: plus some hands that include a 6
                                            //[1:23:53 AM] John Anhalt: like 67o, etc....
                                            else if (BetCallScenarioOnStreet(handHistory, 2, strongestOpponentPlayer, true, true))
                                            {
                                                drawBluffPrct = 0.5;

                                                String turnCard = handHistory.CommunityCards[2][0].ToString();
                                                if (oppCard1[0].ToString().Equals(turnCard) || oppCard2[0].ToString().Equals(turnCard))
                                                {
                                                    if (GetRandom(1, 3) == 1 && !dontRemoveHand)
                                                    {
                                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                else
                                                {
                                                    //if HERO was the PFR (or the last person to raise)
                                                    //[9:29:18 PM] John Anhalt: and the flop went check / check... and hero had position
                                                    //[9:29:32 PM] John Anhalt: then opponent's range will stay the same... they can check top pair, sets, everything
                                                    if (PlayerIsPreflopRaiserOrLastToRaisePreflop(handHistory, handHistory.HeroName)
                                                        && !PlayerIsOutOfPosition(handHistory, handHistory.HeroName, 2)
                                                        )
                                                    {
                                                    }
                                                    else
                                                    {
                                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                    }
                                                }
                                            }
                                            else if (strongestOpponentBetOnTheTurn)
                                            {
                                                //Hand range for opponent if they check the flop, but bet the turn:
                                                //It can include sets sometimes, if the board is not really draw heavy.
                                                //It can also include the nuts, or close to it.
                                                //Hands like flopped straights.

                                                //ok, fine... top pair or better
                                                //[9:47:47 PM] John Anhalt: but the board should not be real draw heavy
                                                //[9:47:51 PM] John Anhalt: if the board is draw heavy
                                                //[9:47:52 PM] Ghady Diab: ok
                                                //[9:47:55 PM] John Anhalt: like the hand example
                                                //[9:48:05 PM] John Anhalt: then it should only include flopped straights or flushes

                                                if (
                                                    (nbFlushsPossibleOnTheTurn < 16 && nbStraightsPossibleOnTheTurn < 16
                                                    && HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false))
                                                    ||
                                                    ((nbFlushsPossibleOnTheTurn > 16 || nbStraightsPossibleOnTheTurn > 16)
                                                    && HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 1, false, false, out topCard, false, true))
                                                   )
                                                {
                                                    //DONT REMOVE HAND     
                                                    handsToRemoveFromEquityCalculation.Remove(oppCard1 + oppCard2);
                                                    handsToRemoveFromEquityCalculation.Remove(oppCard2 + oppCard1);
                                                }
                                                else
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                }
                                            }


                                            //CHECK ON THE FLOP AND ON THE TURN
                                            if (PlayerCheckedOnStreet(handHistory, strongestOpponentPlayer, 1))
                                            {
                                                //So if opponent IS PFR,
                                                //and we have check/check on the flop
                                                //(opponent checked to hero), and checked again on
                                                //the turn -> range=middle pair hands (8x,7x),
                                                //draws, and 25% of the weaker top pair hands?
                                                //if (strongestOpponentIsPreflopRaiser)
                                                {
                                                    if (PlayerCheckedOnStreet(handHistory, strongestOpponentPlayer, 2))
                                                    {
                                                        String middleCard = GetMiddleCardOnStreet(handHistory, 2);
                                                        String highestCard = GetHighestCardOnTheBoard(handHistory, 2);

                                                        if (middleCard != null && (oppCard1[0].Equals(middleCard[0]) || oppCard2[0].Equals(middleCard[0]))
                                                            && !oppCard1[0].Equals(oppCard2[0]))
                                                        {

                                                        }
                                                        else if (highestCard != null && (oppCard1[0].Equals(highestCard[0]) || oppCard2[0].Equals(highestCard[0]))
                                                            && GetRandom(1, 101) <= 25
                                                            && !oppCard1[0].Equals(oppCard2[0]))
                                                        {

                                                        }
                                                        else
                                                        {
                                                            if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                                handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                        }
                                                    }
                                                }
                                            }


                                        }
                                        else
                                        {

                                            //the rule means that they won't bet pairs that are under the highest pair on the board twice
                                            //they will on the flop, but if they bet again on the turn
                                            //zzzz
                                            if (strongestOpponentBetOnTheFlop && strongestOpponentBetOnTheTurn)
                                            {
                                                if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false))
                                                {

                                                }
                                                else
                                                {
                                                    String highestCardOnTheBoard = GetHighestCardOnTheBoard(handHistory, 2);

                                                    if (
                                                        (Card.AllCardsList.IndexOf(highestCardOnTheBoard[0].ToString()) == Card.AllCardsList.IndexOf(oppCard1[0].ToString()) + 1)
                                                        ||
                                                        (Card.AllCardsList.IndexOf(highestCardOnTheBoard[0].ToString()) == Card.AllCardsList.IndexOf(oppCard2[0].ToString()) + 1)
                                                        )
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                    }
                                                }
                                            }

                                            //more importantly the hand range in a situation that hero bets, opponent calls,
                                            //and now hero checks the turn and opponent bets.
                                            //This hand range in this hand is close, but opponent won't bet betting hands that aren't middle pair or better on the turn (except for the bluff hands).
                                            if (BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, false, true)
                                                && strongestOpponentBetOnTheTurn)
                                            {
                                                bool temp = false;
                                                if (PotIsReraisedPreflop(handHistory, out temp))
                                                    drawBluffPrct = 0.5;

                                                if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, true, out topCard, false, false))
                                                {

                                                }
                                                else
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                }
                                            }
                                            else if (BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, true, true)
                                                && strongestOpponentBetOnTheTurn)
                                            {
                                                if (groupedHand1.Equals("22"))
                                                {
                                                }
                                                if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false))
                                                {

                                                }
                                                else
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);
                                                }
                                            }
                                        }

                                        //If opponent, check/calls the flop, and then bets the turn...
                                        //they can have draws in their range. If they are raised on the turn,
                                        //and the raise they need to call is =< 2.8x their bet amount
                                        //then they will need to call w/ big draws
                                        if (PlayerCheckCalledOnStreet(handHistory, strongestOpponentPlayer, 1))
                                        {
                                            if (PlayerBetOnStreet(handHistory, strongestOpponentPlayer, 2) && PlayerWasRaisedOnStreet(handHistory, strongestOpponentPlayer, street))
                                            {
                                                int strongestOpponentBetAmount = 0, raiseAmount = 0;
                                                foreach (Action turnActions in handHistory.PostflopActions[2])
                                                {
                                                    if (turnActions.SAction.Equals("Bets")) strongestOpponentBetAmount = turnActions.Amount;
                                                    else if (turnActions.SAction.Equals("Raises")) raiseAmount = turnActions.Amount;
                                                }

                                                if (raiseAmount <= 2.8 * strongestOpponentBetAmount)
                                                {
                                                    if (
                                                        ((oppCard1[0].Equals('A') && Card.AllCardsList.IndexOf(oppCard2[0].ToString()) >= Card.AllCardsList.IndexOf("T") && groupedHand1.EndsWith("s"))
                                                        || (oppCard2[0].Equals('A') && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf("T") && groupedHand1.EndsWith("s")))
                                                        && canMakeDrawWithHand(handHistory, oppCard1, oppCard2, 2))
                                                    {
                                                        dontRemoveHand = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region RIVER CUSTOM EQUITY
                                    //RIVER
                                    if (action.Street == 3)
                                    {
                                        if (checkCheckScenarioOnRiver)
                                        {
                                            String comCards = handHistory.CommunityCards[1];
                                            comCards += handHistory.CommunityCards[2];

                                            //REMOVE TOP PAIR OR BETTER
                                            topCard = null;
                                            if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, 2, false, false, out topCard, false, false))
                                            {
                                                //if the top pair was an ace
                                                //[9:30:54 PM] John Anhalt: I would not have opponents range include AT+ on the turn
                                                //[9:31:06 PM] John Anhalt: sometimes opponents will check weak top pair hands... like A4 for example
                                                //[9:31:17 PM] John Anhalt: so we'd have to include A9-A2 on the turn
                                                //[9:31:36 PM] John Anhalt: in the check/check scenario
                                                bool removeHand = true;
                                                if (topCard != null)
                                                {
                                                    String otherCard = oppCard1[0].ToString().Equals(topCard) ? oppCard2[0].ToString() : oppCard1[0].ToString();
                                                    if (Card.AllCardsList.IndexOf(otherCard) <= Card.AllCardsList.IndexOf("9"))
                                                    {
                                                        removeHand = false;
                                                    }
                                                }


                                                if (removeHand && !dontRemoveHand)
                                                {
                                                    if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                                        handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 3);

                                                    if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                                        bluffHands.Remove(oppCard1 + oppCard2);
                                                }
                                            }
                                        }
                                    }
                                    #endregion


                                    #region ALL STREETS CUSTOM EQUITY
                                    if (!dontRemoveHand && action.Street >= 1 && PlayerRaisedOnStreet(handHistory, strongestOpponentPlayer, 1)
                                        && !handIsTopPairOrBetterOrOESDOrFlushDraw(handHistory, oppCard1, oppCard2, 1, true))
                                    {
                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 1);

                                        if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                            bluffHands.Remove(oppCard1 + oppCard2);
                                    }
                                    else if (!dontRemoveHand && action.Street >= 2 && PlayerRaisedOnStreet(handHistory, strongestOpponentPlayer, 2)
                                        && !handIsTopPairOrBetterOrOESDOrFlushDraw(handHistory, oppCard1, oppCard2, 2, true))
                                    {
                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 2);

                                        if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                            bluffHands.Remove(oppCard1 + oppCard2);
                                    }
                                    else if (!dontRemoveHand && action.Street >= 3 && PlayerRaisedOnStreet(handHistory, strongestOpponentPlayer, 3)
                                        && !handIsTopPairOrBetterOrOESDOrFlushDraw(handHistory, oppCard1, oppCard2, 3, true))
                                    {
                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 3);

                                        if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                            bluffHands.Remove(oppCard1 + oppCard2);
                                    }


                                    //An opponent won't check/call two streets w/o at least top pair.
                                    //So the small pairs need to be eliminated, and also the small aces
                                    //(thought we had a rule for this) need to be taken out
                                    //(unless it's a hand that makes two pair on the flop or turn).
                                    //So A9s-A2s need to be taken out. JJ-22 need to go as well. 
                                    int nbStreetsStrongestOpponentCheckedCallOn = 0;
                                    if (strongestOpponentCheckCalledOnTheFlop) nbStreetsStrongestOpponentCheckedCallOn++;
                                    if (strongestOpponentCheckCalledOnTheTurn) nbStreetsStrongestOpponentCheckedCallOn++;
                                    if (strongestOpponentCheckCalledOnTheRiver) nbStreetsStrongestOpponentCheckedCallOn++;

                                    if (nbStreetsStrongestOpponentCheckedCallOn >= 2)
                                    {
                                        if (HandIsTopPairOrBetter(handHistory, oppCard1, oppCard2, action.Street, true, false, out topCard, false, false))
                                        {
                                        }
                                        else
                                        {
                                            if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                            {
                                                handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, action.Street);
                                            }
                                        }
                                    }
                                    #endregion

                                    //could we add some rule in a 3-bet pot that opponent will bet or call with TT+ if there is a single broadway on the flop?
                                    //[12/7/2010 11:51:30 PM] John Anhalt: but we need to make sure that if opponent has a set, and there are two or more broadways
                                    //[12/7/2010 11:51:41 PM] John Anhalt: that it still counts this range in
                                    //[12/7/2010 11:52:02 PM] John Anhalt: meaning, don't just exclude TT+ if the flop is : K Q 4... and opponent has QQ or something 
                                    if ((strongestOpponentDid3BetPreflop || strongestOpponentCalled3BetPreflop)
                                        && oppCard1[0].Equals(oppCard2[0])
                                        && (oppCard1[0].ToString().Equals("T") || oppCard1[0].ToString().Equals("J") || oppCard1[0].ToString().Equals("Q") || oppCard1[0].ToString().Equals("K") || oppCard1[0].ToString().Equals("A"))
                                        && GetBroadwayNBOnBoard(handHistory) >= 1
                                        )
                                    {

                                    }
                                    else if (ShouldRemoveFromEquityCalculation(handHistory, strongestOpponentPlayer,
                                      oppCard1 + oppCard2,
                                      opponentNotStrongestPreflopRaiser,
                                      strongestOpponentDid3BetPreflop,
                                      strongestOpponentDid4BetPreflop,
                                      strongestOpponentIsPreflopRaiser,
                                      strongestOpponentCalled3BetPreflop,
                                      strongestOpponentCalled4BetPreflop,
                                       strongestOpponentLimpedThenCall
                                      ))
                                    {
                                        if (!handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                            handsToRemoveFromEquityCalculation.Add(oppCard1 + oppCard2, 0);
                                    }

                                    if (dontRemoveHand)
                                    {
                                        if (handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2))
                                            handsToRemoveFromEquityCalculation.Remove(oppCard1 + oppCard2);
                                        if (handsToRemoveFromEquityCalculation.ContainsKey(oppCard2 + oppCard1))
                                            handsToRemoveFromEquityCalculation.Remove(oppCard2 + oppCard1);
                                    }
                                    if (dontBluffWithHand)
                                    {
                                        if (bluffHands.ContainsKey(oppCard1 + oppCard2))
                                            bluffHands.Remove(oppCard1 + oppCard2);
                                        if (bluffHands.ContainsKey(oppCard2 + oppCard1))
                                            bluffHands.Remove(oppCard2 + oppCard1);

                                    }
                                }
                            }
                        }
                    }
                }
                //END TEST
            }

            #region Change Bluff Prct

            bool noGutshotDraw = false;
            if (isLastAction)
            {
                //In a 3-bet pot - if opponent bets flop and hero just calls,
                //and opponent then check/calls the turn - then we should only list 20% of the highest ranked
                //draws in their range. Meaning draws with big aces (AKs, AQs, etc...).
                //The rest should be taken out of the range. There should be ZERO gut shot draws in their range.
                //Same situation as above... but this time opponent bets the flop AND the turn,
                //then we can include 80% of draws in their range.
                if (street >= 2 && (preflopIs3Bet || preflopIs4Bet))
                {
                    if (BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, true, true)
                        && PlayerCheckCalledOnStreet(handHistory, strongestOpponentPlayer, 2))
                    {
                        drawBluffPrct = 0.2;
                        noGutshotDraw = true;
                    }
                    else if (PlayerBetOnStreet(handHistory, strongestOpponentPlayer, 1)
                        && PlayerBetOnStreet(handHistory, strongestOpponentPlayer, 2))
                    {
                        drawBluffPrct = 0.8;
                        noGutshotDraw = true;
                    }

                }

                //In a single raised or 3-bet pot. If hero bets the flop, and opponent calls.
                //And then hero checks the turn.... opponent can bet 80% of draws that are in their range. 
                if ((PotIsSingleRaisedPreflop(handHistory) || preflopIs3Bet) && BetCallScenarioOnStreet(handHistory, 1, strongestOpponentPlayer, false, true)
                    && PlayerBetOnStreet(handHistory, strongestOpponentPlayer, 2))
                {
                    drawBluffPrct = 0.8;
                }

                //If hero is PFR and flop goes check/check
                //then opponent can bet draws 100% of the time as bluff hands
                if (PlayerIsPreflopRaiser(handHistory, handHistory.Players[handHistory.HeroName] as Player, false)
                    && CheckCheckScenarioOnStreet(handHistory, strongestOpponentPlayer, 1)
                    && strongestOpponentBetOnTheTurn)
                {
                    drawBluffPrct = 1;
                }

                if (strongestOpponentCalledOffHisStackOnTheFlop || strongestOpponentCalledOffHisStackOnTheTurn || strongestOpponentCalledOffHisStackOnTheRiver)
                    drawBluffPrct = 0;
            }
            #endregion


            #region Filter BluffHands
            //IF BLUFF HAND WAS REMOVED ON A PREVIOUS STREET DON'T USE AS BLUFF
            Hashtable filteredBluffList = new Hashtable();
            foreach (String bluffHand1 in bluffHands.Keys)
            {
                String bluffHand2 = bluffHand1.Substring(2, 2) + bluffHand1.Substring(0, 2);
                int removedOnStreet = -1;
                if (handsToRemoveFromEquityCalculation.ContainsKey(bluffHand1))
                {
                    removedOnStreet = (int)handsToRemoveFromEquityCalculation[bluffHand1];
                }
                else if (handsToRemoveFromEquityCalculation.ContainsKey(bluffHand2))
                {
                    removedOnStreet = (int)handsToRemoveFromEquityCalculation[bluffHand2];
                }

                int bluffOnStreet = (int)bluffHands[bluffHand1];
                if (removedOnStreet >= bluffOnStreet || removedOnStreet == -1)
                {
                    filteredBluffList.Add(bluffHand1, bluffOnStreet);
                }
            }
            bluffHands = filteredBluffList;

            FilterBluffHands(handHistory, drawBluffPrct, action.Street, noGutshotDraw, strongestOpponentPlayer);
            #endregion

            for (int kloops = 0; kloops < simulations; kloops++)
            {
                if (handsToAddToRange.Count == 0) handsToAddToRange.Add("");
                //foreach (String handToAdd in handsToAddToRange)
                {
                    deck = simsDecks[kloops];
                    //ATT EQUITY
                    int[] hand_hero = new int[] { deck[0], deck[1], deck[2], deck[3], deck[4], deck[5], deck[6] };
                    short rank_hero = FastEvaluator.eval_7hand(hand_hero);

                    short suckout_rank_flop_hero = 0, suckout_rank_turn_hero = 0;
                    if (street == 1) suckout_rank_flop_hero = FastEvaluator.eval_5hand(hand_hero); // Flop
                    if (street == 1 || street == 2) suckout_rank_turn_hero = FastEvaluator.eval_6hand(hand_hero); // Turn

                    int[] hand_villain = new int[] { 0, 0, deck[2], deck[3], deck[4], deck[5], deck[6] };
                    short rank_villain = 9999, suckout_rank_flop_villain = 9999, suckout_rank_turn_villain = 9999;
                    bool deep_valid = true, short_valid = true, postflop_valid = true; // Is this iteration valid with the given hand range criteria


                    bool skipCards = false;
                    for (int i = 0; i < villain_counter; i++)
                    {
                        hand_villain[0] = deck[7 + i * 2];
                        hand_villain[1] = deck[8 + i * 2];

                        if (i == strongest_villain)// && handsToAddToRange.Count > 1)// && kloops > simulations - handsToTryBluffingWith.Count)
                        {
                            try
                            {
                                hand_villain[0] = GetNumberByCard(handsToAddToRange[kloops].Substring(0, 2), deck);
                                hand_villain[1] = GetNumberByCard(handsToAddToRange[kloops].Substring(2, 2), deck);
                            }
                            catch (Exception exc)
                            {
                            }
                        }

                        short temp_rank = FastEvaluator.eval_7hand(hand_villain);
                        if (temp_rank < rank_villain) rank_villain = temp_rank;

                        if (street == 1)
                        {
                            short temp_suckout = FastEvaluator.eval_5hand(hand_villain); // Flop
                            if (temp_suckout < suckout_rank_flop_villain) suckout_rank_flop_villain = temp_suckout;
                        }
                        if (street == 1 || street == 2)
                        {
                            short temp_suckout = FastEvaluator.eval_6hand(hand_villain); // Turn
                            if (temp_suckout < suckout_rank_turn_villain) suckout_rank_turn_villain = temp_suckout;
                        }

                        // See if villain's hand was within the given percentile for weighted equity calculations (preflop)
                        int villain1, villain2;
                        if (FastEvaluator.SUIT(hand_villain[0]) == FastEvaluator.SUIT(hand_villain[1])) // Villain has a suited hand
                        {
                            villain1 = Math.Max(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                            villain2 = Math.Min(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                        }
                        else // Villain has offsuit hand or pocket pair
                        {
                            villain1 = Math.Min(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                            villain2 = Math.Max(FastEvaluator.RANK(hand_villain[0]), FastEvaluator.RANK(hand_villain[1]));
                        }

                        // (post-flop)
                        int post_villain1 = FastEvaluator.RANK(hand_villain[0]);
                        int post_villain2 = FastEvaluator.RANK(hand_villain[1]);
                        switch (FastEvaluator.SUIT(hand_villain[0]))
                        {
                            case (int)GameRules.suit.DIAMOND: post_villain1 += 13; break;
                            case (int)GameRules.suit.CLUB: post_villain1 += 26; break;
                            case (int)GameRules.suit.SPADE: post_villain1 += 39; break;
                        }
                        switch (FastEvaluator.SUIT(hand_villain[1]))
                        {
                            case (int)GameRules.suit.DIAMOND: post_villain2 += 13; break;
                            case (int)GameRules.suit.CLUB: post_villain2 += 26; break;
                            case (int)GameRules.suit.SPADE: post_villain2 += 39; break;
                        }

                        if (!deep_weight[i, villain1, villain2]) deep_valid = false;
                        if (!short_weight[i, villain1, villain2]) short_valid = false;
                        if (postflop_weight[i, post_villain1, post_villain2] == 0.0f) postflop_valid = false; //ATT

                        if (i == strongest_villain)
                        {
                            // Update equity against one random hand
                            if (rank_hero < temp_rank) wins_hup += 2; // Hero won
                            else if (rank_hero == temp_rank) wins_hup += 1; // Split pot


                            String oppCard1 = null, oppCard2 = null;
                            if (isLastAction)
                            {
                                skipCards = true;
                                oppCard1 = GetCardByNumber(hand_villain[0], deck);
                                oppCard2 = GetCardByNumber(hand_villain[1], deck);

                                String groupedHand1 = oppCard1[0].ToString() + oppCard2[0].ToString() + (oppCard1[0].Equals(oppCard2[0]) ? "" : oppCard1[1].Equals(oppCard2[1]) ? "s" : "o");
                                String groupedHand2 = oppCard2[0].ToString() + oppCard1[0].ToString() + (oppCard1[0].Equals(oppCard2[0]) ? "" : oppCard1[1].Equals(oppCard2[1]) ? "s" : "o");




                                if (
                                         (bluffHands.ContainsKey(oppCard1 + oppCard2) || bluffHands.ContainsKey(oppCard2 + oppCard1)
                                         || opponentHandRange.Contains(groupedHand1) || opponentHandRange.Contains(groupedHand2)
                                         )
                                     )
                                {

                                    if (groupedHand1.Contains("33"))
                                    {
                                    }

                                    skipCards = false;
                                    if ((handsToRemoveFromEquityCalculation.ContainsKey(oppCard1 + oppCard2) || handsToRemoveFromEquityCalculation.ContainsKey(oppCard2 + oppCard1)) && (!bluffHands.ContainsKey(oppCard1 + oppCard2) && !bluffHands.ContainsKey(oppCard2 + oppCard1)))
                                    {
                                        skipCards = true;
                                    }
                                    else
                                    {
                                        if (groupedHand1.StartsWith("AKo"))
                                        {
                                        }
                                    }


                                    if (!skipCards && !opponentCards.ContainsKey(oppCard1 + oppCard2) && !opponentCards.ContainsKey(oppCard2 + oppCard1))
                                    {
                                        opponentCards.Add(oppCard1 + oppCard2, "");
                                        StrongestOpponentHands.Add(oppCard1 + oppCard2);
                                    }
                                }
                            }

                            if (!skipCards)
                            {
                                bool isInEquity = false;

                                if (isLastAction && !isInEquity)
                                {
                                    postflop_valid = true;
                                    postflop_trials_hup++;
                                    if (rank_hero < temp_rank)
                                        postflop_wins_hup += 2; // Hero won
                                    else if (rank_hero == temp_rank) postflop_wins_hup += 1; // Split pot
                                }
                                else
                                {

                                    // Update equity against the strongest opponent
                                    if (deep_weight[i, villain1, villain2])
                                    {
                                        deep_trials_hup++;
                                        if (rank_hero < temp_rank) deep_wins_hup += 2; // Hero won
                                        else if (rank_hero == temp_rank) deep_wins_hup += 1; // Split pot
                                        isInEquity = true;
                                    }
                                    if (short_weight[i, villain1, villain2])
                                    {
                                        short_trials_hup++;
                                        if (rank_hero < temp_rank) short_wins_hup += 2; // Hero won
                                        else if (rank_hero == temp_rank) short_wins_hup += 1; // Split pot
                                        isInEquity = true;
                                    }
                                    if (postflop_weight[i, post_villain1, post_villain2] == 1)
                                    {
                                        postflop_trials_hup++;
                                        if (rank_hero < temp_rank)
                                            postflop_wins_hup += 2; // Hero won
                                        else if (rank_hero == temp_rank)
                                            postflop_wins_hup += 1; // Split pot
                                        isInEquity = true;
                                    }
                                }

                            }
                        }
                    }

                    if (!skipCards)
                    {
                        // Update equity against random hands
                        if (rank_hero < rank_villain) wins_all += 2; // Hero won
                        else if (rank_hero == rank_villain) wins_all += 1; // Split pot

                        // Update equity against given hand ranges
                        if (deep_valid)
                        {
                            deep_trials_all++;
                            if (rank_hero < rank_villain) deep_wins_all += 2; // Hero won
                            else if (rank_hero == rank_villain) deep_wins_all += 1; // Split pot
                        }
                        if (short_valid)
                        {
                            short_trials_all++;
                            if (rank_hero < rank_villain) short_wins_all += 2; // Hero won
                            else if (rank_hero == rank_villain) short_wins_all += 1; // Split pot
                        }
                        if (postflop_valid)
                        {
                            postflop_trials_all++;
                            if (rank_hero < rank_villain) postflop_wins_all += 2; // Hero won
                            else if (rank_hero == rank_villain) postflop_wins_all += 1; // Split pot
                        }



                        if (street == 1)
                        {
                            if (postflop_valid && suckout_rank_flop_hero > suckout_rank_flop_villain)
                            {
                                suckout_trials_all++;
                                if (suckout_rank_turn_hero < suckout_rank_turn_villain) suckout_wins_all += 2; // Hero won
                                else if (suckout_rank_turn_hero == suckout_rank_turn_villain) suckout_wins_all += 1; // Split pot
                            }
                        }
                        else if (street == 2)
                        {
                            if (postflop_valid && suckout_rank_turn_hero > suckout_rank_turn_villain)
                            {
                                suckout_trials_all++;
                                if (rank_hero < rank_villain) suckout_wins_all += 2; // Hero won
                                else if (rank_hero == rank_villain) suckout_wins_all += 1; // Split pot
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool PlayerWasRaisedOnStreet(HandHistory handHistory, Player player, int street)
        {
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (!player.PlayerName.Equals(action.PlayerName) && action.SAction.Equals("Raises"))
                    return true;
            }
            return false;
        }

        private bool RaiseReraiseCallOnStreet(HandHistory handHistory, int street)
        {
            bool raise = false, reraise = false;
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (action.SAction.Equals("Raises"))
                {
                    if (raise) reraise = true;
                    else raise = true;
                }
                else if (action.SAction.Equals("Calls"))
                {
                    if (reraise) return true;
                }
            }
            return false;
        }

        private bool PlayerCheckRaisedOnStreet(HandHistory handHistory, Player player, int street)
        {
            bool playerChecked = false;
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (action.PlayerName.Equals(player.PlayerName))
                {
                    if (action.SAction.Equals("Checks")) playerChecked = true;
                    else if (playerChecked && action.SAction.Equals("Raises")) return true;
                    else playerChecked = false;
                }
            }
            return false;
        }

        private String GetHighestCardOnTheBoard(HandHistory handHistory, int street)
        {
            String topCard = null;
            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;

            //GET TOP CARD
            List<String> boardCards = new List<string>();
            boardCards.Add(boardCard1);
            boardCards.Add(boardCard2);
            boardCards.Add(boardCard3);
            if (boardCard4 != null)
                boardCards.Add(boardCard4);
            if (boardCard5 != null)
                boardCards.Add(boardCard5);
            foreach (String boardCard in boardCards)
            {
                if (topCard != null && boardCard[0] == topCard[0])
                {
                    //topCard = null;
                    //break;
                }

                if (topCard == null) topCard = boardCard;
                else if (Card.AllCardsList.IndexOf(boardCard[0].ToString()) > Card.AllCardsList.IndexOf(topCard[0].ToString()))
                    topCard = boardCard;
            }
            return topCard;
        }
        bool PlayerCalledOffHisStack(HandHistory handHistory, Player player, int street)
        {
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (player.PlayerName.Equals(player.PlayerName) && action.SAction.Equals("Calls"))
                {
                    int plrStack = GetPlayerStackOnStreet(handHistory, player.PlayerName, action);
                    if (plrStack <= action.Amount)
                        return true;
                }
            }
            return false;
        }
        bool PlayerRaisedOnStreet(HandHistory handHistory, Player player, int street)
        {
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (action.SAction.Equals("Raises")) return true;
            }
            return false;
        }

        private bool PostflopStreetIsReraised(HandHistory handHistory, int street)
        {
            int nbRaises = 0;
            foreach (Action postflopAction in handHistory.PostflopActions[street])
            {
                if (postflopAction.SAction.Equals("Raises") || postflopAction.SAction.Equals("Bets")) nbRaises++;
            }
            return nbRaises > 1;
        }

        private String GetBottomCardOnStreet(HandHistory handHistory, int street)
        {
            List<String> allBoardCards = new List<String>();
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(0, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(2, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(4, 2));
            if (street >= 2)
                allBoardCards.Add(handHistory.CommunityCards[2].Substring(0, 2));
            if (street >= 3)
                allBoardCards.Add(handHistory.CommunityCards[3].Substring(0, 2));

            String minBoardCard = allBoardCards[0];
            foreach (String boardCard in allBoardCards)
            {
                int boardCardIndex = Card.AllCardsList.IndexOf(boardCard[0].ToString());
                int boardCardMinIndex = Card.AllCardsList.IndexOf(minBoardCard[0].ToString());
                if (boardCardIndex < boardCardMinIndex)
                    minBoardCard = boardCard;
            }

            int minCardOccurences = 0;
            foreach (String boardCard in allBoardCards)
            {
                if (boardCard[0].Equals(minBoardCard[0])) minCardOccurences++;
            }

            if (minCardOccurences == 1) return minBoardCard;
            else return null;
        }

        List<String> GetMiddleBoardCardsBetweenTopAndBottom(HandHistory handHistory, int street)
        {
            List<String> allBoardCards = new List<String>();
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(0, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(2, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(4, 2));
            if (street >= 2)
                allBoardCards.Add(handHistory.CommunityCards[2].Substring(0, 2));
            if (street >= 3)
                allBoardCards.Add(handHistory.CommunityCards[3].Substring(0, 2));

            List<String> distinctBoardCardsWithoutSuits = new List<String>();
            foreach (String boardCard in allBoardCards)
            {
                if (!distinctBoardCardsWithoutSuits.Contains(boardCard[0].ToString()))
                    distinctBoardCardsWithoutSuits.Add(boardCard[0].ToString());
            }


            //ORDER BOARD CARDS BY MIN VALUE
            for (int i = 0; i < distinctBoardCardsWithoutSuits.Count - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < distinctBoardCardsWithoutSuits.Count; j++)
                {
                    if (Card.AllCardsList.IndexOf(distinctBoardCardsWithoutSuits[j]) < Card.AllCardsList.IndexOf(distinctBoardCardsWithoutSuits[minIndex]))
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)
                {
                    String temp = distinctBoardCardsWithoutSuits[minIndex];
                    distinctBoardCardsWithoutSuits[minIndex] = distinctBoardCardsWithoutSuits[i];
                    distinctBoardCardsWithoutSuits[i] = temp;
                }
            }
            List<String> middleCards = new List<String>();
            if (distinctBoardCardsWithoutSuits.Count <= 2) return new List<String>();

            for (int i = 1; i < distinctBoardCardsWithoutSuits.Count - 1; i++)
            {
                middleCards.Add(distinctBoardCardsWithoutSuits[i]);
            }


            for (int i = 0; i < middleCards.Count; i++)
            {
                String middleCard = middleCards[i];
                int middleCardOccurences = 0;
                foreach (String boardCard in allBoardCards)
                {
                    if (boardCard[0].Equals(middleCard[0]))
                    {
                        middleCard = boardCard;
                        middleCardOccurences++;
                    }
                }
                if (middleCardOccurences > 1)
                {
                    middleCards.RemoveAt(i);
                    i--;
                }
            }

            return middleCards;
        }


        List<String> GetBottomCardsOnFlop(HandHistory handHistory)
        {
            List<String> allBoardCards = new List<String>();
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(0, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(2, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(4, 2));

            List<String> distinctBoardCardsWithoutSuits = new List<String>();
            foreach (String boardCard in allBoardCards)
            {
                if (!distinctBoardCardsWithoutSuits.Contains(boardCard[0].ToString()))
                    distinctBoardCardsWithoutSuits.Add(boardCard[0].ToString());
            }


            //ORDER BOARD CARDS BY MIN VALUE
            for (int i = 0; i < distinctBoardCardsWithoutSuits.Count - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < distinctBoardCardsWithoutSuits.Count; j++)
                {
                    if (Card.AllCardsList.IndexOf(distinctBoardCardsWithoutSuits[j]) < Card.AllCardsList.IndexOf(distinctBoardCardsWithoutSuits[minIndex]))
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)
                {
                    String temp = distinctBoardCardsWithoutSuits[minIndex];
                    distinctBoardCardsWithoutSuits[minIndex] = distinctBoardCardsWithoutSuits[i];
                    distinctBoardCardsWithoutSuits[i] = temp;
                }
            }
            List<String> bottomCards = new List<String>();
            if (distinctBoardCardsWithoutSuits.Count < 1) return new List<String>();

            for (int i = 0; i < distinctBoardCardsWithoutSuits.Count - 1; i++)
            {
                bottomCards.Add(distinctBoardCardsWithoutSuits[i]);
            }


            for (int i = 0; i < bottomCards.Count; i++)
            {
                String bottomCard = bottomCards[i];
                int bottomCardOccurences = 0;
                foreach (String boardCard in allBoardCards)
                {
                    if (boardCard[0].Equals(bottomCard[0]))
                    {
                        bottomCard = boardCard;
                        bottomCardOccurences++;
                    }
                }
                if (bottomCardOccurences > 1)
                {
                    bottomCards.RemoveAt(i);
                    i--;
                }
            }

            return bottomCards;
        }

        private String GetMiddleCardOnStreet(HandHistory handHistory, int street)
        {
            List<String> allBoardCards = new List<String>();
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(0, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(2, 2));
            allBoardCards.Add(handHistory.CommunityCards[1].Substring(4, 2));
            if (street >= 2)
                allBoardCards.Add(handHistory.CommunityCards[2].Substring(0, 2));
            if (street >= 3)
                allBoardCards.Add(handHistory.CommunityCards[3].Substring(0, 2));

            List<String> distinctBoardCardsWithoutSuits = new List<String>();
            foreach (String boardCard in allBoardCards)
            {
                if (!distinctBoardCardsWithoutSuits.Contains(boardCard[0].ToString()))
                    distinctBoardCardsWithoutSuits.Add(boardCard[0].ToString());
            }

            //ORDER BOARD CARDS BY MIN VALUE
            for (int i = 0; i < distinctBoardCardsWithoutSuits.Count - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < distinctBoardCardsWithoutSuits.Count; j++)
                {
                    if (Card.AllCardsList.IndexOf(distinctBoardCardsWithoutSuits[j]) < Card.AllCardsList.IndexOf(distinctBoardCardsWithoutSuits[minIndex]))
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)
                {
                    String temp = distinctBoardCardsWithoutSuits[minIndex];
                    distinctBoardCardsWithoutSuits[minIndex] = distinctBoardCardsWithoutSuits[i];
                    distinctBoardCardsWithoutSuits[i] = temp;
                }
            }

            String middleCardWithoutSuit = distinctBoardCardsWithoutSuits[(int)(Math.Floor((double)distinctBoardCardsWithoutSuits.Count / (double)2))];


            String middleCard = null;
            int middleCardOccurences = 0;
            foreach (String boardCard in allBoardCards)
            {
                if (boardCard[0].Equals(middleCardWithoutSuit[0]))
                {
                    middleCard = boardCard;
                    middleCardOccurences++;
                }
            }

            if (middleCardOccurences == 1) return middleCard;
            else return null;
        }

        private bool PotIsSingleRaisedPreflop(HandHistory handHistory)
        {
            int nbRaises = 0;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                    nbRaises++;
            }
            return nbRaises == 1;
        }


        bool PlayerBetOrRaisedOnStreet(HandHistory handHistory, Player player, int street)
        {
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (action.PlayerName.Equals(player.PlayerName))
                    return action.SAction.Equals("Bets") || action.SAction.Equals("Raises");
            }
            return false;
        }


        bool PlayerCheckCalledOnStreet(HandHistory handHistory, Player player, int street)
        {
            bool playerChecked = false;
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (action.PlayerName.Equals(player.PlayerName))
                {
                    if (action.SAction.Equals("Checks")) playerChecked = true;
                    if (playerChecked && action.SAction.Equals("Calls")) return true;
                }
            }
            return false;
        }

        bool PlayerBetOnStreet(HandHistory handHistory, Player player, int street)
        {
            foreach (Action action in handHistory.PostflopActions[street])
            {
                if (action.PlayerName.Equals(player.PlayerName))
                    return action.SAction.Equals("Bets");
            }
            return false;
        }

        private bool PlayerIsPreflopAggressor(HandHistory handHistory, Player player)
        {
            String lastAggressivePlayerName = null;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                    lastAggressivePlayerName = preflopAction.PlayerName;
            }
            return lastAggressivePlayerName != null && lastAggressivePlayerName.Equals(player.PlayerName);
        }

        private bool CheckCheckScenarioOnStreet(HandHistory handHistory, Player strongestOpponentPlayer, int street)
        {
            bool heroChecked = false;
            bool opponentChecked = false;
            foreach (Action flopAction in handHistory.PostflopActions[street])
            {
                if (flopAction.PlayerName.Equals(handHistory.HeroName))
                {
                    heroChecked = flopAction.SAction.Equals("Checks");
                    if (heroChecked && opponentChecked) return true;
                }
                else if (flopAction.PlayerName.Equals(strongestOpponentPlayer.PlayerName))
                {
                    opponentChecked = flopAction.SAction.Equals("Checks");
                    if (heroChecked && opponentChecked) return true;
                }
            }
            return false;
        }

        bool HandHasMiddlePairOnStreet(HandHistory handHistory, int street, String oppCard1, String oppCard2)
        {
            String middleBoardCard;
            if ((middleBoardCard = GetMiddleCardOnStreet(handHistory, street)) != null && (oppCard1[0].Equals(middleBoardCard[0]) || oppCard2[0].Equals(middleBoardCard[0])))
            {
                return true;
            }
            return false;
        }

        bool HandHasBottomPairOnStreet(HandHistory handHistory, int street, String oppCard1, String oppCard2)
        {
            String bottomBoardCard;
            if ((bottomBoardCard = GetBottomCardOnStreet(handHistory, street)) != null && (oppCard1[0].Equals(bottomBoardCard[0]) || oppCard2[0].Equals(bottomBoardCard[0])))
            {
                return true;
            }
            return false;
        }

        private bool BetCallScenarioOnStreet(HandHistory handHistory, int street, Player strongestOpponentPlayer, bool heroCalled, bool betOrRaised)
        {
            bool heroBet = false, opponentBet = false;
            foreach (Action postflopAction in handHistory.PostflopActions[street])
            {
                if ((postflopAction.PlayerName.Equals(handHistory.HeroName) || postflopAction.PlayerName.Equals(strongestOpponentPlayer.PlayerName)))
                {
                    String sAction = postflopAction.SAction;
                    if (opponentBet && postflopAction.PlayerName.Equals(handHistory.HeroName) && sAction.Equals("Calls") && heroCalled)
                        return true;
                    else if (heroBet && postflopAction.PlayerName.Equals(strongestOpponentPlayer.PlayerName) && sAction.Equals("Calls") && !heroCalled)
                        return true;

                    if (postflopAction.PlayerName.Equals(handHistory.HeroName))
                    {
                        heroBet = sAction.Equals("Bets") || (betOrRaised && sAction.Equals("Raises"));
                        opponentBet = false;
                    }
                    else
                    {
                        opponentBet = sAction.Equals("Bets") || (betOrRaised && sAction.Equals("Raises"));
                        heroBet = false;
                    }
                }
            }
            return false;
        }


        internal List<String> GetEquityCards(HandHistory handHistory, int action)
        {
            List<String> equityCards = new List<String>();

            return equityCards;
        }

        int drawingOuts(String board_cards, String hole_cards, int street)
        {
            boardinfo info = new boardinfo();
            int[] holecard = new int[2];
            int[] boardcard = new int[5];

            holecard[0] = Jacob.TranslateCard(hole_cards[0], hole_cards[1]);
            holecard[1] = Jacob.TranslateCard(hole_cards[2], hole_cards[3]);

            if (street == 1)
            { // Flop
                if (board_cards != null && board_cards.Length >= 1)
                    boardcard[0] = Jacob.TranslateCard(board_cards[0], board_cards[1]);
                if (board_cards != null && board_cards.Length >= 3)
                    boardcard[1] = Jacob.TranslateCard(board_cards[2], board_cards[3]);
                if (board_cards != null && board_cards.Length >= 5)
                    boardcard[2] = Jacob.TranslateCard(board_cards[4], board_cards[5]);
                boardcard[3] = -1;
                boardcard[4] = -1;
            }
            else if (street == 2)
            { // Turnif(board_cards.Length>=1)
                if (board_cards != null && board_cards.Length >= 1)
                    boardcard[0] = Jacob.TranslateCard(board_cards[0], board_cards[1]);
                if (board_cards != null && board_cards.Length >= 3)
                    boardcard[1] = Jacob.TranslateCard(board_cards[2], board_cards[3]);
                if (board_cards != null && board_cards.Length >= 5)
                    boardcard[2] = Jacob.TranslateCard(board_cards[4], board_cards[5]);
                if (board_cards != null && board_cards.Length >= 7)
                    boardcard[3] = Jacob.TranslateCard(board_cards[6], board_cards[7]);
                boardcard[4] = -1;
            }
            else return 0; // River -> No outs (same for Preflop)

            if (holecard[0] == boardcard[0] || holecard[0] == boardcard[1] || holecard[0] == boardcard[2] || holecard[0] == boardcard[3] ||
                holecard[1] == boardcard[0] || holecard[1] == boardcard[1] || holecard[1] == boardcard[2] || holecard[1] == boardcard[3]) return 0; // Invalid hole cards given

            Jacob.AnalyzeBoard(holecard[0], holecard[1], boardcard[0], boardcard[1], boardcard[2], boardcard[3], boardcard[4], info);

            // Count all outs as live outs (and start splitting them by hand type, for debugging purposes)
            int outs_full = 0; // Outs to a full house (or quads) -> beats a flush
            int outs_flush = 0; // Outs to a flush -> beats a straight
            int outs_straight = 0; // Outs to a straight -> beats pairs, two pairs, and trips
            int outs_overpair = 0; // Outs to an over pair (or two pair or trips)

            // Outs to a full house (requires a straight or flush threat on the board required)
            if (info.samesuitcards >= 3 || info.ifmadestraights)
            { // If possible straight or flush on board
                if (info.madehand == postflophand.k3ofKind)
                {
                    if (street == 1) outs_full = 7;  // Flop:  7 outs to a full house (or quads), both with set and with trips
                    else if (street == 2) outs_full = 10; // Turn: 10 outs to a full house (or quads), both with set and with trips
                }
                else if (info.madehand == postflophand.k2Pair)
                { // 2-Pair
                    outs_full = 4; // 4 outs to full house, same with hitting both unpaired hole cards, hitting one pair and paired board, or pocket pair on paired board
                }
            }

            // Outs to a flush and a straight
            if (info.madehand != postflophand.kStraightFlush && info.madehand != postflophand.k4ofKind && info.madehand != postflophand.kFullHouse && info.madehand != postflophand.kFlush && info.samesuitcards < 4)
            {
                if ((info.ifstraightflushdraw && info.drawtype != 2) || // Open Ended Straight-Flush Draw
                    (info.ifflushdraw && info.drawflushcardsmissing == 1 && info.ifstraightdraw && info.drawtype != 2)) // OESD+Flush-draw (15 outs total)
                {
                    outs_flush = 9; // 9 flush-outs
                    if (info.madehand != postflophand.kStraight && info.samesuitcards < 3) outs_straight = 6; // 6 straight-outs since flush-outs are already counted for (and don't count straight-outs if 3-flush on board already)
                }
                else if ((info.ifstraightflushdraw && info.drawtype == 2) || // Gut-Shot Straight-Flush Draw
                         (info.ifflushdraw && info.drawflushcardsmissing == 1 && info.ifstraightdraw && info.drawtype == 2)) // Gutshot+Flush-draw (12 outs total)
                {
                    outs_flush = 9; // 9 flush-outs
                    if (info.madehand != postflophand.kStraight && info.samesuitcards < 3) outs_straight = 3; // 3 straight-outs since flush-outs are already counted for (and don't count straight-outs if 3-flush on board already)
                }
                else // No straight-flush draws, or straight + flush combo-draws
                {
                    if (info.ifflushdraw && info.drawflushcardsmissing == 1) outs_flush = 9; // Flush Draw, 9 outs
                    else if (info.ifstraightdraw && info.drawtype != 2 && info.madehand != postflophand.kStraight && info.samesuitcards < 3) outs_straight = 8; // OESD, 8 outs (and don't count straight-outs if 3-flush on board already)
                    else if (info.ifstraightdraw && info.drawtype == 2 && info.madehand != postflophand.kStraight && info.samesuitcards < 3) outs_straight = 4; // Gut-Shot, 4 outs (and don't count straight-outs if 3-flush on board already)
                }
            }

            // Outs to a set, trips, 2-pair or overpair
            if (info.madehand == postflophand.kPair)
            { // Pair
                if (info.holesused == 2) outs_overpair = 2; // Pocket pair, 2 outs to a set
                else if (info.holesused == 1) outs_overpair = 5; // Flopped a pair, 5 outs to trips or a 2-pair
            }
            else if (info.madehand == postflophand.kNoPair)
            {
                if (info.boardhigher1 == 0 && info.boardhigher2 == 0) outs_overpair = 6; // 2 overcards, 6 outs to an overpair
                else if (info.boardhigher1 == 0 || info.boardhigher2 == 0) outs_overpair = 3; // 1 overcard,  3 outs to an overpair
            }

            // Discount "overpair" outs based on possible flush-draws and made straights on the board
            // Even though I wanted to use objective out counting, flush outs and overpair outs can't be counted independently as they may overlap!
            if (info.samesuitcards == 4) outs_overpair = 0;
            else if (info.samesuitcards == 3) outs_overpair -= 2;
            else if (info.samesuitcards == 2) outs_overpair -= 1;
            if (info.ifmadestraights) outs_overpair -= 1;
            if (outs_overpair < 0) outs_overpair = 0;

            return Math.Min(outs_full + outs_flush + outs_straight + outs_overpair, 21); // Cap outs to 21 in Texas Hold'em - our outs->equity array is only up to 21 outs (and it may be the limit in Texas Holdem anyways)
        }

        bool HeroHasOverPair(HandHistory handHistory, int street)
        {
            String sHeroCards = (handHistory.Players[handHistory.HeroName] as Player).Cards;
            char[] heroCards = new char[] { sHeroCards[0], sHeroCards[2] };

            if (!heroCards[0].Equals(heroCards[1])) return false;

            List<char> communityCards = new List<char>();
            if (street >= 1)
            {
                communityCards.Add(handHistory.CommunityCards[1][0]);
                communityCards.Add(handHistory.CommunityCards[1][2]);
                communityCards.Add(handHistory.CommunityCards[1][4]);
            }
            if (street >= 2)
            {
                communityCards.Add(handHistory.CommunityCards[2][0]);
            }
            if (street >= 3)
            {
                communityCards.Add(handHistory.CommunityCards[3][0]);
            }

            foreach (char communityCard in communityCards)
            {
                if ((int)Card.CardValues[communityCard.ToString()] > (int)Card.CardValues[heroCards[0].ToString()])
                    return false;
            }
            return true;
        }

        Player GetOpponentBettingPostflop(HandHistory handHistory, Action action)
        {
            Player opponentBetting = null;
            foreach (Action postflopAction in GetAllPostflopActionsOnStreetBefore(handHistory, action))
            {
                if (postflopAction.PlayerName.Equals(handHistory.HeroName)) continue;
                if (postflopAction.SAction.Equals("Bets") || postflopAction.SAction.Equals("Raises"))
                    opponentBetting = handHistory.Players[postflopAction.PlayerName] as Player;
            }
            return opponentBetting;
        }

        bool HeroIsLastToActPostflop(HandHistory handHistory, Action action)
        {
            Hashtable playersInHand = new Hashtable();
            foreach (String playerName in handHistory.Players.Keys)
            {
                if ((bool)action.InHand[playerName])
                    playersInHand.Add(playerName, 0);
            }

            foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
            {
                if (postflopAction == action)
                {
                    foreach (String playerName in playersInHand.Keys)
                    {
                        if (playerName.Equals(action.PlayerName)) continue;
                        if ((int)playersInHand[playerName] <= (int)playersInHand[action.PlayerName])
                            return false;
                    }
                }
                else
                {
                    if (playersInHand.Contains(postflopAction.PlayerName))
                    {
                        playersInHand[postflopAction.PlayerName] = (int)playersInHand[postflopAction.PlayerName] + 1;
                    }
                }
            }
            return true;
        }

        bool PlayerIsOutOfPosition(HandHistory handHistory, String playerName, int street)
        {
            return (handHistory.PostflopActions[street][0].PlayerName == playerName);
        }

        static bool IsTablePaired(HandHistory handHistory, int street)
        {
            boardinfo boardInfo = Jacob.AnalyzeHand(handHistory, street, false);
            return boardInfo.madehand == postflophand.kPair || boardInfo.madehand == postflophand.k2Pair || boardInfo.madehand == postflophand.k3ofKind || boardInfo.madehand == postflophand.k4ofKind;
        }
        static internal bool HeroRaisedLimperPreflop(HandHistory handHistory)
        {
            bool raisedPreflop = false;
            bool hasLimpers = false;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.PlayerName.Equals(handHistory.HeroName) && hasLimpers && preflopAction.SAction.Equals("Raises"))
                {
                    raisedPreflop = true;
                }

                if (!preflopAction.PlayerName.Equals(handHistory.HeroName) && preflopAction.SAction.Equals("Raises"))
                {
                    raisedPreflop = false;
                }

                if (!preflopAction.PlayerName.Equals(handHistory.HeroName))
                {
                    if (preflopAction.SAction.Equals("Calls") && preflopAction.Amount == handHistory.BigBlindAmount)
                        hasLimpers = true;
                }
            }
            return raisedPreflop;
        }

        internal static Player HeadsUpOrOneOpponent(HandHistory handHistory, Action action)
        {
            List<String> playersLeft = new List<String>();
            List<String> opponentsInHand = new List<String>();
            bool currentActionFound = false;
            foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
            {
                if (currentActionFound && !postflopAction.PlayerName.Equals(handHistory.HeroName))
                {
                    if (!playersLeft.Contains(postflopAction.PlayerName))
                        playersLeft.Add(postflopAction.PlayerName);
                }
                else if (postflopAction.Equals(action))
                {
                    currentActionFound = true;
                }
                if (!opponentsInHand.Contains(postflopAction.PlayerName) && !postflopAction.PlayerName.Equals(handHistory.HeroName))
                    opponentsInHand.Add(postflopAction.PlayerName);
            }

            Player opponent = null;
            if (playersLeft.Count == 1 || opponentsInHand.Count == 1)
                opponent = handHistory.Players[playersLeft.Count == 1 ? playersLeft[0] : opponentsInHand[0]] as Player;

            return opponent;
        }

        internal static Player HeadsUp(HandHistory handHistory, Action action)
        {
            List<String> opponentsInHand = new List<String>();
            if (action.Street == 0)
            {
                foreach (Action preflopAction in handHistory.PreflopActions)
                {
                    if (!opponentsInHand.Contains(preflopAction.PlayerName) && !preflopAction.PlayerName.Equals(handHistory.HeroName))
                        opponentsInHand.Add(preflopAction.PlayerName);
                    if (preflopAction.SAction.Equals("Folds") && opponentsInHand.Contains(preflopAction.PlayerName))
                    {
                        opponentsInHand.Remove(preflopAction.PlayerName);
                    }
                }
            }
            else
            {
                foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
                {
                    if (!opponentsInHand.Contains(postflopAction.PlayerName) && !postflopAction.PlayerName.Equals(handHistory.HeroName))
                        opponentsInHand.Add(postflopAction.PlayerName);
                    if (postflopAction.SAction.Equals("Folds") && opponentsInHand.Contains(postflopAction.PlayerName))
                    {
                        opponentsInHand.Remove(postflopAction.PlayerName);
                    }
                }
            }

            Player opponent = null;
            if (opponentsInHand.Count == 1)
                opponent = handHistory.Players[opponentsInHand[0]] as Player;

            return opponent;
        }

        static bool IsLastActionInHand(HandHistory handHistory, Action action)
        {
            Action lastAction = null;
            if (handHistory.PostflopActions[1].Count == 0)
                lastAction = handHistory.PreflopActions[handHistory.PreflopActions.Count - 1];
            else if (handHistory.PostflopActions[2].Count == 0)
                lastAction = handHistory.PostflopActions[1][handHistory.PostflopActions[1].Count - 1];
            else if (handHistory.PostflopActions[3].Count == 0)
                lastAction = handHistory.PostflopActions[2][handHistory.PostflopActions[2].Count - 1];
            else
                lastAction = handHistory.PostflopActions[3][handHistory.PostflopActions[3].Count - 1];

            return action == lastAction;
        }
        static List<Action> GetAllPostflopActionsOnStreetAfter(HandHistory handHistory, Action action)
        {
            List<Action> postflopActionsAfter = new List<Action>();
            bool actionFound = false;
            foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
            {
                if (actionFound)
                {
                    postflopActionsAfter.Add(postflopAction);
                }
                else if (postflopAction.Equals(action))
                {
                    actionFound = true;
                }
            }
            return postflopActionsAfter;
        }


        static List<Action> GetAllPreflopActionsOnStreetAfter(HandHistory handHistory, Action action)
        {
            List<Action> preflopActionsAfter = new List<Action>();
            bool actionFound = false;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (actionFound)
                {
                    preflopActionsAfter.Add(preflopAction);
                }
                else if (preflopAction.Equals(action))
                {
                    actionFound = true;
                }
            }
            return preflopActionsAfter;
        }

        bool CanRaisePostflop(HandHistory handHistory, Action action)
        {
            if (!PotIsReraisedPostflopBeforeAction(handHistory, action, false)) return true;
            List<Action> raiseActions = new List<Action>();
            int nbRaises = 0;
            int lastRaise = 0;
            bool allInOk = false;
            foreach (Action postflopAction in GetAllPostflopActionsOnStreetBefore(handHistory, action))
            {
                if (allInOk && postflopAction.SAction.Equals("Calls"))
                {
                    return false;
                }
                if (postflopAction.SAction.Equals("Raises"))
                {
                    if (nbRaises > 0)
                    {
                        int playerStack = GetPlayerStackOnStreet(handHistory, postflopAction.PlayerName, postflopAction);
                        if (playerStack == postflopAction.Amount)
                        {
                            if (postflopAction.Amount >= 2 * lastRaise)
                            {
                                allInOk = true;
                            }
                        }
                        lastRaise = postflopAction.Amount;
                    }
                    nbRaises++;
                }
            }
            return true;
        }


        bool CanRaisePreflop(HandHistory handHistory, Action action)
        {
            bool minimumRaise = false;
            int _3betAmount = 0;
            if (!PotIsReraisedPreflopBeforeAction(handHistory, action, out minimumRaise, out _3betAmount)) return true;
            List<Action> raiseActions = new List<Action>();
            int nbRaises = 0;
            int lastRaise = 0;
            bool allInOk = false;
            foreach (Action preflopAction in GetAllPreflopActionsOnStreetBefore(handHistory, action))
            {
                if (allInOk && preflopAction.SAction.Equals("Calls"))
                {
                    return false;
                }
                if (preflopAction.SAction.Equals("Raises"))
                {
                    if (nbRaises > 0)
                    {
                        int playerStack = GetPlayerStackOnStreet(handHistory, preflopAction.PlayerName, preflopAction);
                        if (playerStack == preflopAction.Amount)
                        {
                            if (preflopAction.Amount >= 2 * lastRaise)
                            {
                                allInOk = true;
                            }
                        }
                        lastRaise = preflopAction.Amount;
                    }
                    nbRaises++;
                }
            }
            return true;
        }

        static List<Action> GetAllPreflopActionsOnStreetBefore(HandHistory handHistory, Action action)
        {
            List<Action> preflopActionsBefore = new List<Action>();
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.Equals(action)) break;
                preflopActionsBefore.Add(preflopAction);
            }
            return preflopActionsBefore;
        }

        static List<Action> GetAllPostflopActionsOnStreetBefore(HandHistory handHistory, Action action)
        {
            List<Action> postflopActionsBefore = new List<Action>();
            foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
            {
                if (postflopAction.Equals(action)) break;
                postflopActionsBefore.Add(postflopAction);
            }
            return postflopActionsBefore;
        }

        //GHADY
        internal static bool BoardIsCoordinated(HandHistory handHistory, int valToCompare, int street, bool withHeroCards)
        {
            String heroCards = (handHistory.Players[handHistory.HeroName] as Player).Cards;
            int nCard1 = withHeroCards ? Jacob.TranslateCard(heroCards[0], heroCards[1]) : -1;
            int nCard2 = withHeroCards ? Jacob.TranslateCard(heroCards[2], heroCards[3]) : -1;

            String comCards = handHistory.CommunityCards[1] + handHistory.CommunityCards[2] + handHistory.CommunityCards[3];

            int nComCard1 = comCards.Length < 2 ? 0 : Jacob.TranslateCard1(comCards[0], comCards[1]);
            int nwComCard1 = comCards.Length < 2 ? -1 : Jacob.TranslateCard(comCards[0], comCards[1]);

            int nComCard2 = comCards.Length < 4 ? 0 : Jacob.TranslateCard1(comCards[2], comCards[3]);
            int nwComCard2 = comCards.Length < 4 ? -1 : Jacob.TranslateCard(comCards[2], comCards[3]);

            int nComCard3 = comCards.Length < 6 ? 0 : Jacob.TranslateCard1(comCards[4], comCards[5]);
            int nwComCard3 = comCards.Length < 6 ? -1 : Jacob.TranslateCard(comCards[4], comCards[5]);

            int nComCard4 = street < 2 ? 0 : comCards.Length < 8 ? 0 : Jacob.TranslateCard1(comCards[6], comCards[7]);
            int nwComCard4 = street < 2 ? -1 : comCards.Length < 8 ? -1 : Jacob.TranslateCard(comCards[6], comCards[7]);

            int nComCard5 = street < 3 ? 0 : comCards.Length < 10 ? 0 : Jacob.TranslateCard1(comCards[8], comCards[9]);
            int nwComCard5 = street < 3 ? -1 : comCards.Length < 10 ? -1 : Jacob.TranslateCard(comCards[8], comCards[9]);


            boardinfo info = new boardinfo();
            int weight = Jacob.AnalyzeBoard(nCard1, nCard2, nwComCard1, nwComCard2, nwComCard3, nwComCard4, nwComCard5, info);
            int boardCoordination = Jacob.GetBoardCoordination(nComCard1, nComCard2, nComCard3, nComCard4, nComCard5, weight);
            if (weight > valToCompare * 2) return true;
            return 3 * weight / boardCoordination > valToCompare;
        }

        internal static bool PlayerFoldedOnPreflop(HandHistory handHistory, String playerName)
        {
            foreach (Action action in handHistory.PreflopActions)
            {
                if (action.PlayerName.Equals(playerName) && !action.SAction.Equals("Folds"))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool PlayerIsInPreflop(HandHistory handHistory, String playerName)
        {
            foreach (Action action in handHistory.PreflopActions)
            {
                if (action.PlayerName.Equals(playerName))// && action.SAction!="Folds")
                    return true;
            }
            return false;
        }

        static bool PotIsReraisedPreflop(HandHistory handHistory, out bool potWasRaisedMinimumAmount)
        {
            potWasRaisedMinimumAmount = false;
            int nbRaises = 0;

            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))// && preflopAction.Amount > handHistory.BigBlindAmount * 2)
                {
                    nbRaises++;
                    if (preflopAction.Amount <= handHistory.BigBlindAmount * 2)
                        potWasRaisedMinimumAmount = true;
                }
            }
            return nbRaises > 1;
        }

        static bool PotIsReraisedPostflopBeforeAction(HandHistory handHistory, Action beforePostflopAction, bool checkAllStreets)
        {
            int nbRaises = 0;
            foreach (Action postflopAction in handHistory.PostflopActions[beforePostflopAction.Street])
            {
                if (postflopAction.Equals(beforePostflopAction)) break;
                if (postflopAction.SAction.Equals("Raises"))
                {
                    nbRaises++;
                }
            }
            if (checkAllStreets && beforePostflopAction.Street > 1)
            {
                for (int i = 1; i < beforePostflopAction.Street; i++)
                {
                    foreach (Action postflopAction in handHistory.PostflopActions[i])
                    {
                        if (postflopAction.SAction.Equals("Raises"))
                        {
                            nbRaises++;
                        }
                    }
                }
            }

            //CHECK PREFLOP
            if (checkAllStreets)
            {
                foreach (Action preflopAction in handHistory.PreflopActions)
                {
                    if (preflopAction.SAction.Equals("Raises"))
                    {
                        nbRaises++;
                    }
                }
            }
            return nbRaises > 1;
        }


        static bool PotIsReraisedPreflopBeforeAction(HandHistory handHistory, Action beforePreflopAction, out bool potWasRaisedMinimumAmount, out int _3BetAmount)
        {
            potWasRaisedMinimumAmount = false;
            _3BetAmount = 0;
            int nbRaises = 0;

            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.Equals(beforePreflopAction)) break;
                if (preflopAction.SAction.Equals("Raises"))// && preflopAction.Amount > handHistory.BigBlindAmount * 2)
                {
                    nbRaises++;
                    _3BetAmount = preflopAction.Amount;
                    if (preflopAction.Amount <= handHistory.BigBlindAmount * 2)
                        potWasRaisedMinimumAmount = true;
                }
            }
            return nbRaises > 1;
        }

        static bool FlopIsPaired(HandHistory handHistory)
        {
            String flopCards = handHistory.CommunityCards[1];

            return (flopCards[0].Equals(flopCards[2])
            || flopCards[0].Equals(flopCards[4])

            || flopCards[1].Equals(flopCards[2])
            || flopCards[1].Equals(flopCards[4]));
        }

        static bool PotIsUnraisedPreflop(HandHistory handHistory)
        {
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool PlayerIsFirstToActOrIsCheckedToPostFlop(HandHistory handHistory, Action action)
        {
            bool firstToAct = true;
            foreach (Action postflopAction in handHistory.PostflopActions[action.Street])
            {
                if (postflopAction.PlayerName.Equals(handHistory.HeroName) && postflopAction.Equals(action))
                {
                    break;
                }
                if (postflopAction.SAction.Equals("Bets") || postflopAction.SAction.Equals("Raises"))
                {
                    firstToAct = false;
                    break;
                }
            }
            return firstToAct;
        }

        bool FlopIsSingleBroadway(HandHistory handHistory)
        {
            String flopCards = handHistory.CommunityCards[1];
            String[] cards = new String[] { "J", "Q", "K", "A" };
            Hashtable cardsToSearch = new Hashtable();
            cardsToSearch.Add("J", 0);
            cardsToSearch.Add("Q", 0);
            cardsToSearch.Add("K", 0);
            cardsToSearch.Add("A", 0);

            foreach (String cardToSearch in cards)
            {
                if (flopCards.Contains(cardToSearch))
                    cardsToSearch[cardToSearch] = (int)cardsToSearch[cardToSearch] + 1;
            }

            int nbCardsAppearingOnce = 0;
            foreach (String cardToSearch in cardsToSearch.Keys)
            {
                if ((int)cardsToSearch[cardToSearch] == 1)
                    nbCardsAppearingOnce++;
            }

            return nbCardsAppearingOnce == 1;
        }

        Player GetPlayerAgainstOrBetting(HandHistory handHistory, Action action)
        {
            Player playerAgainst = HeadsUpOrOneOpponent(handHistory, action);
            if (playerAgainst == null)
            {
                if (action.Attacker != null && !action.Attacker.Equals(""))
                    return handHistory.Players[action.Attacker] as Player;
            }
            return playerAgainst;
        }
        bool AllOpponentsAre(HandHistory handHistory, Action action, String[] models)
        {
            foreach (String opponent in handHistory.Players.Keys)
            {
                if (opponent.Equals(handHistory.HeroName)) continue;
                if (!(bool)action.InHand[opponent]) continue;

                String opponentModel = Player.GetPlayerModel(handHistory.Is6Max, handHistory.Players[opponent] as Player).ToLower();
                bool opponentIsFromModels = false;
                foreach (String model in models)
                {
                    if (model.ToLower().Equals(opponentModel))
                    {
                        opponentIsFromModels = true;
                        break;
                    }
                }
                if (!opponentIsFromModels) return false;
            }
            return true;
        }

        bool PlayerHasOESD(HandHistory handHistory, String playerName, int street)
        {
            Player player = handHistory.Players[playerName] as Player;
            List<int> allCardValues = new List<int>();
            allCardValues.Add((int)Card.CardValues[player.Cards[0].ToString()]);
            allCardValues.Add((int)Card.CardValues[player.Cards[2].ToString()]);

            allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[1][0].ToString()]);
            allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[1][2].ToString()]);
            allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[1][4].ToString()]);

            if (street >= 2)
                allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[2][0].ToString()]);

            if (street == 3)
                allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[3][0].ToString()]);

            bool hasOESD = false;

            foreach (int cardValue1 in allCardValues)
            {
                int numberToSearch = cardValue1 + 1;

                for (int i = 1; i < 5; i++)
                {
                    foreach (int CardValue2 in allCardValues)
                    {
                        if (CardValue2 == numberToSearch)
                        {
                            numberToSearch++;
                        }
                    }
                    if (numberToSearch == cardValue1 + 4)
                        hasOESD = true;
                }
                if (hasOESD) break;
            }
            return hasOESD;
        }

        bool PlayerHas4ToAFlush(HandHistory handHistory, int street, bool withHeroCards)
        {
            String cards = "";
            if (withHeroCards)
            {
                cards += (handHistory.Players[handHistory.HeroName] as Player).Cards[1] + (handHistory.Players[handHistory.HeroName] as Player).Cards[3];
            }
            cards += (handHistory.CommunityCards[1][1]);
            cards += (handHistory.CommunityCards[1][3]);
            cards += (handHistory.CommunityCards[1][5]);

            if (street >= 2)
                cards += (handHistory.CommunityCards[2][1]);

            if (street == 3)
                cards += (handHistory.CommunityCards[3][1]);

            foreach (char card in cards)
            {
                String tmpCards = cards;
                if (tmpCards.Replace(card.ToString(), "").Length <= cards.Length - 4)
                {
                    return true;
                }
            }

            return false;
        }

        bool PlayerHas4ToAStraight(HandHistory handHistory, int street, bool withHeroCards)
        {
            Player player = handHistory.Players[handHistory.HeroName] as Player;
            List<int> allCardValues = new List<int>();
            if (withHeroCards)
            {
                allCardValues.Add((int)Card.CardValues[player.Cards[0].ToString()]);
                allCardValues.Add((int)Card.CardValues[player.Cards[2].ToString()]);
            }

            allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[1][0].ToString()]);
            allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[1][2].ToString()]);
            allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[1][4].ToString()]);

            if (street >= 2)
                allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[2][0].ToString()]);

            if (street == 3)
                allCardValues.Add((int)Card.CardValues[handHistory.CommunityCards[3][0].ToString()]);

            bool hasStraight = false;

            foreach (int cardValue1 in allCardValues)
            {
                int numberToSearch = cardValue1 + 1;

                bool gave1Chance = false;
                while (true)
                {
                    bool nextNumberFound = false;
                    foreach (int card2 in allCardValues)
                    {
                        if (card2 == numberToSearch)
                        {
                            nextNumberFound = true;
                            numberToSearch++;
                            break;
                        }
                    }
                    if (!nextNumberFound && !gave1Chance)
                    {
                        gave1Chance = true;
                        numberToSearch++;
                    }
                    else if (!nextNumberFound) break;

                    if (numberToSearch == cardValue1 + 5)
                    {
                        hasStraight = true;
                        break;
                    }
                }
                if (hasStraight) break;
            }
            return hasStraight;
        }


        internal static bool PlayerHasTopPair(HandHistory handHistory, Action action, Player player)
        {
            List<String> boardCards = new List<String>();
            boardCards.Add(handHistory.CommunityCards[1][0].ToString());
            boardCards.Add(handHistory.CommunityCards[1][2].ToString());
            boardCards.Add(handHistory.CommunityCards[1][4].ToString());

            int maxCardNo = -1;
            foreach (String boardCard in boardCards)
            {
                for (int i = 0; i < Card.CardName.Length; i++)
                {
                    String cardName = Card.CardName[i].ToString();
                    if (boardCard.ToLower().Equals(cardName.ToLower()))
                    {
                        if (i > maxCardNo)
                            maxCardNo = i;
                        break;
                    }
                }
            }
            String maxCard = Card.CardName[maxCardNo].ToString();
            if (player.Cards[0].ToString().ToLower().Equals(maxCard.ToLower())
             || player.Cards[2].ToString().ToLower().Equals(maxCard.ToLower()))
            {
                return true;
            }
            return false;
        }

        internal static bool PlayerIsPreflopRaiserOrLastToRaisePreflop(HandHistory handHistory, String playerName)
        {
            String lastPlayerToRaise = null;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                {
                    lastPlayerToRaise = preflopAction.PlayerName;
                }
            }
            if (lastPlayerToRaise == null) return false;
            else if (lastPlayerToRaise.Equals(playerName)) return true;
            return false;
        }
        internal static bool PlayerIsPreflopRaiser(HandHistory handHistory, Player player, bool noReraise)
        {
            bool raisedPreflop = false;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                {
                    raisedPreflop = true;
                    if (!noReraise && preflopAction.PlayerName == player.PlayerName)
                        return true;
                    else if (!preflopAction.PlayerName.Equals(player.PlayerName))
                        return false;
                }

                else if (raisedPreflop && noReraise && preflopAction.SAction.Equals("Raises"))
                {
                    raisedPreflop = false;
                    break;
                }
            }
            return raisedPreflop;
        }

        internal static String GetLabelNumber(double d, int v, HandHistory handHistory)
        {
            if (handHistory != null)
            {
                if (handHistory.SmallBlindAmount < 100)
                {
                    d = d / (handHistory.SmallBlindAmount / 100);
                    d = Math.Round(d, MidpointRounding.AwayFromZero);
                    d *= (handHistory.SmallBlindAmount / 100);
                    return d.ToString("0.00");
                }
                if (handHistory.BigBlindAmount > 100)
                {
                    v = 0;
                }
            }
            int n = 0;
            if (int.TryParse(d.ToString(), out n))
                return n.ToString();
            if (v == 0)
                return Math.Round(d).ToString();
            else if (v == 2)
                return Math.Round(d, v).ToString("0.00");
            else return Math.Round(d, v).ToString();
        }


        List<String> GetHandsTo3BetWith()
        {
            return new List<String>(new String[] { "77", "88", "99", "TT", "JJ", "QQ", "KK", "AA", "A2s", "A3s", "A4s", "A5s", "A6s", "A7s", "A8s", "A9s", "ATs", "AJs", "AQs", "AKs", "K7s", "K8s", "K9s", "KTs", "KJs", "KQs", "Q7s", "Q8s", "Q9s", "QTs", "QJs", "J8s", "J9s", "JTs", "T8s", "T9s", "97s", "98s", "86s", "87s", "76s", "A2o", "A3o", "A4o", "A5o", "A6o", "A7o", "A8o", "A9o", "ATo", "AJo", "AQo", "AKo", "K9o", "KTo", "KJo", "KQo", "QTo", "QJo", "JTo", "T8o", "T9o", "97o", "98o", "86o", "87o", "76o" });
        }

        List<String> GetHandsTo4BetWith()
        {
            return new List<String>(new String[] { "99", "TT", "JJ", "QQ", "KK", "AA", "A2s", "A3s", "A4s", "A5s", "A6s", "A7s", "A8s", "A9s", "ATs", "AJs", "AQs", "AKs", "K9s", "KTs", "KJs", "KQs", "QTs", "QJs", "A2o", "A3o", "A4o", "A5o", "A6o", "A7o", "A8o", "A9o", "ATo", "AJo", "AQo", "AKo", "KTo", "KJo", "KQo", "QTo", "QJo" });
        }


        List<String> UngroupHands(List<String> groupedHands, HandHistory handHistory)
        {
            String[] suits = new String[] { "c", "s", "h", "d" };
            List<String> ungroupedHands = new List<String>();
            foreach (String groupedHand in groupedHands)
            {
                List<String> handsToTry = new List<String>();
                if (groupedHand.EndsWith("s"))
                {
                    foreach (String suit in suits)
                        handsToTry.Add(groupedHand[0].ToString() + suit + groupedHand[1].ToString() + suit);
                }
                else
                {
                    foreach (String suit1 in suits)
                    {
                        foreach (String suit2 in suits)
                        {
                            if (!suit1.Equals(suit2))
                            {
                                handsToTry.Add(groupedHand[0].ToString() + suit1 + groupedHand[1].ToString() + suit2);
                            }
                        }
                    }
                }

                foreach (String handToTry in handsToTry)
                {
                    String card1 = handToTry.Substring(0, 2);
                    String card2 = handToTry.Substring(2, 2);

                    String comCards = "";
                    if (handHistory.CommunityCards[1] != null) comCards += handHistory.CommunityCards[1];
                    if (handHistory.CommunityCards[2] != null) comCards += handHistory.CommunityCards[2];
                    if (handHistory.CommunityCards[3] != null) comCards += handHistory.CommunityCards[3];
                    comCards += (handHistory.Players[handHistory.HeroName] as Player).Cards;

                    if (!comCards.Contains(card1) && !comCards.Contains(card2))
                        ungroupedHands.Add(card1 + card2);
                }
            }
            return ungroupedHands;
        }
        Hashtable GetHandsToOpenRaiseWith()
        {
            Hashtable HandsToOpenRaiseWith = new Hashtable();

            HandsToOpenRaiseWith.Add("UTG", new List<String>(
                new String[]{
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","76s","65s","54s","43s","A9o","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo","T9o","97o","98o","87o","76o","65o"
                }));

            HandsToOpenRaiseWith.Add("EP", new List<String>(
                new String[]{
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","76s","65s","54s","43s","A9o","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo","T9o","97o","98o","87o","76o","65o"
                }));

            HandsToOpenRaiseWith.Add("MP", new List<String>(
                new String[]{
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","65s","54s","43s","A8o","A9o","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","64o","65o","54"
                }));

            HandsToOpenRaiseWith.Add("HJ", new List<String>(
                new String[]{
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","65s","54s","43s","A8o","A9o","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","64o","65o","54"
                }));

            HandsToOpenRaiseWith.Add("CO", new List<String>(
                new String[]{
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K9s","KTs","KJs","KQs","Q9s","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","65s","54s","43s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","64o","65o","54o"
                }));

            HandsToOpenRaiseWith.Add("BTN", new List<String>(
                new String[]{
                    "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K2s","K3s","K4s","K5s","K6s","K7s","K8s","K9s","KTs","KJs","KQs","Q6s","Q7s","Q8s","Q9s","QTs","QJs","J6s","J7s","J8s","J9s","JTs","T6s","T7s","T8s","T9s","96s","97s","98s","85s","86s","87s","75s","76s","64s","65s","53s","54s","43s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K2o","K3o","K4o","K5o","K6o","K7o","K8o","K9o","KTo","KJo","KQo","Q8o","Q9o","QTo","QJo","J7o","J8o","J9o","JTo","T7o","T8o","T9o","96o","97o","98o","85o","86o","87o","75o","76o","64o","65o","54"
                }));

            HandsToOpenRaiseWith.Add("SB", new List<String>(
                new String[]{
                    "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K6s","K7s","K8s","K9s","KTs","KJs","KQs","Q7s","Q8s","Q9s","QTs","QJs","J7s","J8s","J9s","JTs","T7s","T8s","T9s","96s","97s","98s","85s","86s","87s","75s","76s","64s","65s","54s","43s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","QTo","QJo","J8o","J9o","JTo","T7o","T8o","T9o","96o","97o","98o","85o","86o","87o","75o","76o","64o","65o","54o"
                }));

            return HandsToOpenRaiseWith;
        }

        bool PlayerDid3BetPreflop(HandHistory handHistory, Player player)
        {
            int nbRaises = 0;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                {
                    nbRaises++;
                    if (preflopAction.PlayerName.Equals(player.PlayerName) && nbRaises == 2)
                        return true;
                }
            }
            return false;
        }

        bool PlayerDid4BetPreflop(HandHistory handHistory, Player player)
        {
            int nbRaises = 0;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                {
                    nbRaises++;
                    if (preflopAction.PlayerName.Equals(player.PlayerName) && nbRaises == 3)
                        return true;
                }
            }
            return false;
        }



        Player PlayerCalledOpenRaisePreflop(HandHistory handHistory, Player player)
        {
            Player preflopRaiser = null;
            if (!PlayerIsPreflopRaiser(handHistory, player, false))
            {
                foreach (Action action in handHistory.PreflopActions)
                {
                    if (action.SAction.Equals("Raises") && preflopRaiser == null && action.PlayerName != player.PlayerName)
                        preflopRaiser = handHistory.Players[action.PlayerName] as Player;

                    if (preflopRaiser != null && action.PlayerName == player.PlayerName && action.SAction.Equals("Calls"))
                    {
                        return preflopRaiser;
                    }
                }
            }
            return null;
        }

        bool PlayerCalled4BetPreflop(HandHistory handHistory, Player player)
        {
            int nbRaises = 0;
            //if (!PlayerIsPreflopRaiser(handHistory, player, false))
            {
                foreach (Action action in handHistory.PreflopActions)
                {
                    if (action.SAction.Equals("Raises"))
                        nbRaises++;

                    if (nbRaises == 3 && action.PlayerName == player.PlayerName && action.SAction.Equals("Calls"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool PlayerCalled3BetPreflop(HandHistory handHistory, Player player)
        {
            int nbRaises = 0;
            //if (!PlayerIsPreflopRaiser(handHistory, player, false))
            {
                foreach (Action action in handHistory.PreflopActions)
                {
                    if (action.SAction.Equals("Raises"))
                        nbRaises++;

                    if (nbRaises == 2 && action.PlayerName == player.PlayerName && action.SAction.Equals("Calls"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        List<String> GetHandsToCall4BetWith()
        {
            return new List<String>(new String[]{
                "TT","JJ","QQ","KK","AA","AQs","AKs","AQo","AKo"
            });
        }

        List<String> GetHandsToCall3BetWithIP()
        {
            return new List<String>(new String[]{
                "66","77","88","99","TT","JJ","QQ","KK","AA","ATs","AJs","AQs","AKs","KJs","KQs","QJs","JTs","T9s","98s","87s","76s","AJo","AQo","AKo","KJo","KQo"
            });
        }

        List<String> GetHandsToCall3BetWithOOP()
        {
            return new List<String>(new String[]{
                //88+, ATs+, KJs+, QJs, JTs, T9s, 98s, 87s, AJo+, KJo+, QJ
                "88","99","TT","JJ","QQ","KK","AA","ATs","AJs","AQs","AKs","QJs", "JTs", "T9s", "98s", "87s", "AJo","AQo","AKo","KJo","KQo","QJ"
            });
        }


        Hashtable GetHandsToCallWithWhenBlindsOpenRaised()
        {
            Hashtable handsToCallWith = new Hashtable();
            //"UTG",  "EP",  "MP",  "MP",  "HJ",  "CO", "BTN",  "SB",  "BB"

            handsToCallWith.Add("UTG", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
            }));

            handsToCallWith.Add("EP", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
            }));

            handsToCallWith.Add("MP", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","87o","76o","65o","54o"
            }));

            handsToCallWith.Add("HJ", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","87o","76o","65o","54o"
            }));

            handsToCallWith.Add("CO", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","43s","A8o","A9o","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","87o","76o","65o","54o"
            }));

            handsToCallWith.Add("BTN", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K9s","KTs","KJs","KQs","QTs","QJs","J8s","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","98o","87o","76o","65o","54o"
            }));

            handsToCallWith.Add("SB", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K9s","KTs","KJs","KQs","Q9s","QTs","QJs","J8s","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","65o","54o"
            }));

            handsToCallWith.Add("BB", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","K9s","KTs","KJs","KQs","Q9s","QTs","QJs","J8s","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o","ATo","AJo","AQo","AKo","K9o","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","65o","54o"
            }));
            return handsToCallWith;
        }



        Hashtable GetHandsToCallWithWhenNonBlindsOpenRaised()
        {
            Hashtable handsToCallWith = new Hashtable();


            //"UTG",  "EP",  "MP",  "MP",  "HJ",  "CO", "BTN",  "SB",  "BB"

            handsToCallWith.Add("UTG", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
            }));

            handsToCallWith.Add("EP", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo"
            }));

            handsToCallWith.Add("MP", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","T9s","98s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo","T9o","98o"
            }));

            handsToCallWith.Add("HJ", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","JTs","T9s","98s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","JTo","T9o","98o"
            }));

            handsToCallWith.Add("CO", new List<String>(new String[] {
                "22","33","44","55","66","77","88","99","TT","JJ","QQ","KK","AA","A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs","AJs","AQs","AKs","KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","43s","ATo","AJo","AQo","AKo","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","87o","76o","65o","54o"
            }));
            return handsToCallWith;
        }

        List<String> GetHandsToLimpCallWith()
        {
            return new List<String>(new String[]{
                "22","33","44","55","66","77","88","99",
            "A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs",
                "KTs","KJs","KQs","QTs","QJs","J9s","JTs","T8s","T9s","97s","98s","86s","87s","75s","76s","64s","65s","54s","KTo","KJo","KQo","QTo","QJo","J9o","JTo","T9o","98o","86o","87o","76o","65o","54o"
            });
        }

        List<String> GetHandsCallUnraisedPotWith()
        {
            return new List<String>(new String[]{
                //88-22, ATs-A2s, KTs, QTs+, J8s+, T7s+, 96s+, 86s+, 75s+, 65s, 54s,
                //A9o-A2o, KJo-KTo, QTo+, J8o+, T8o+, 97o+, 86o+, 75o+, 65o
                "22","33","44","55","66","77","88",
                "A2s","A3s","A4s","A5s","A6s","A7s","A8s","A9s","ATs",
                "KTs",
                "QTs","QJs","J8s","J9s","JTs","T7s","T8s","T9s","96s","97s","98s","86s","87s","75s","76s","65s","54s",
                "A2o","A3o","A4o","A5o","A6o","A7o","A8o","A9o",
                "KTo","KJo",
                "QTo","QJo","J8o","J9o","JTo","T8o","T9o","97o","98o","86o","87o","75o","76o","65o"
            });
        }

        bool PlayerLimpedThenCalledPreflop(HandHistory handHistory, Player player)
        {
            bool playerLimped = false, potRaised = false;
            foreach (Action preflopAction in handHistory.PreflopActions)
            {
                if (preflopAction.SAction.Equals("Raises"))
                {
                    if (!playerLimped) return false;
                    potRaised = true;
                }

                else if (preflopAction.SAction.Equals("Calls") && preflopAction.PlayerName.Equals(player.PlayerName))
                {
                    if (!potRaised) playerLimped = true;
                    else if (playerLimped) return true;
                }
            }
            return false;
        }

        #region FlushDraw Helper delagetes
        private static bool Dimonds(String suite)
        {
            if (suite == "d")
                return true;
            return false;
        }

        private static bool Herts(String suite)
        {
            if (suite == "h")
                return true;
            return false;
        }

        private static bool Clubs(String suite)
        {
            if (suite == "c")
                return true;
            return false;
        }

        private static bool Spades(String suite)
        {
            if (suite == "s")
                return true;
            return false;
        }
        #endregion

        private bool PlayerHasFlushDraw(HandHistory handHistory, String oppCard1, String oppCard2)
        {
            List<String> hand = new List<String>();
            hand.Add(oppCard1[1].ToString());
            hand.Add(oppCard2[1].ToString());

            List<String> allCardValues = new List<String>();
            allCardValues.Add(handHistory.CommunityCards[1][1].ToString());
            allCardValues.Add(handHistory.CommunityCards[1][3].ToString());
            allCardValues.Add(handHistory.CommunityCards[1][5].ToString());

            if (handHistory.CommunityCards[2] != null)
                allCardValues.Add(handHistory.CommunityCards[2][1].ToString());

            if (handHistory.CommunityCards[3] != null)
                allCardValues.Add(handHistory.CommunityCards[3][1].ToString());

            List<String> allBoardClubs = allCardValues.FindAll(Clubs);
            List<String> allBoardDimonds = allCardValues.FindAll(Dimonds);
            List<String> allBoardHearts = allCardValues.FindAll(Herts);
            List<String> allBoardSpades = allCardValues.FindAll(Spades);

            List<String> handClubs = hand.FindAll(Clubs);
            List<String> handDimonds = hand.FindAll(Dimonds);
            List<String> handHearts = hand.FindAll(Herts);
            List<String> handSpades = hand.FindAll(Spades);

            //use one from the hand, and have exactly 4 suites 
            if (
                ((handClubs.Count > 0) && (allBoardClubs.Count + handClubs.Count == 4)) ||
                ((handDimonds.Count > 0) && (allBoardDimonds.Count + handDimonds.Count == 4)) ||
                ((handHearts.Count > 0) && (allBoardHearts.Count + handHearts.Count == 4)) ||
                ((handSpades.Count > 0) && (allBoardSpades.Count + handSpades.Count == 4))
                )
                return true;

            return false;
        }

        bool handIsTopPairOrBetterOrOESDOrFlushDraw(HandHistory handHistory, String oppCard1, String oppCard2, int street, bool orOverPair)
        {
            if (orOverPair && oppCard1[0].Equals(oppCard2[0]) && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf("J"))
            {
                return true;
            }

            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;
            boardinfo boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, oppCard1, oppCard2);

            if (boardInfo.madehand == postflophand.kPair)
            {
                List<String> allCards = new List<String>();
                if (handHistory.CommunityCards[1] != null)
                {
                    allCards.Add(handHistory.CommunityCards[1][0].ToString());
                    allCards.Add(handHistory.CommunityCards[1][2].ToString());
                    allCards.Add(handHistory.CommunityCards[1][4].ToString());
                }

                if (handHistory.CommunityCards[2] != null)
                    allCards.Add(handHistory.CommunityCards[2][0].ToString());
                if (handHistory.CommunityCards[3] != null)
                    allCards.Add(handHistory.CommunityCards[3][0].ToString());

                allCards.Add(oppCard1[0].ToString());
                allCards.Add(oppCard2[0].ToString());

                int maxCardNo = -1;
                foreach (String card in allCards)
                {
                    for (int i = 0; i < Card.CardName.Length; i++)
                    {
                        String cardName = Card.CardName[i].ToString();
                        if (card.ToLower().Equals(cardName.ToLower()))
                        {
                            if (i > maxCardNo)
                                maxCardNo = i;
                            break;
                        }
                    }
                }
                String maxCard = Card.CardName[maxCardNo].ToString();
                if (oppCard1[0].ToString().ToLower().Equals(maxCard.ToLower())
                 || oppCard2[0].ToString().ToLower().Equals(maxCard.ToLower()))
                {
                    return true;
                }
            }

            if ((boardInfo.madehand == postflophand.k2Pair)
                || boardInfo.madehand == postflophand.k3ofKind
                || boardInfo.madehand == postflophand.k4ofKind
                || boardInfo.madehand == postflophand.kFlush
                || boardInfo.madehand == postflophand.kFullHouse
                || boardInfo.madehand == postflophand.kStraight
                || boardInfo.madehand == postflophand.kStraightFlush) return true; //TOP PAIR OR BETTER

            if (boardInfo.drawflushcardsmissing == 1 && boardInfo.drawflushholesused > 0) return true;

            return (boardInfo.ifstraightdraw && boardInfo.drawtype != 2);
        }



        private int[] GetNbFlushStraightsPossible(HandHistory handHistory, int street)
        {
            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;

            int nbStraights = 0, nbFlush = 0;
            String[] suits = new String[] { "s", "c", "h", "d" };

            Hashtable cardsUsedForStraight = new Hashtable();
            Hashtable cardsUsedForFlush = new Hashtable();
            foreach (String card1WithoutSuit in Card.AllCardsList)
            {
                foreach (String suit1 in suits)
                {
                    String card1 = card1WithoutSuit + suit1;
                    foreach (String card2WithoutSuit in Card.AllCardsList)
                    {
                        foreach (String suit2 in suits)
                        {
                            if (card1WithoutSuit.Equals("Q") && card2WithoutSuit.Equals("J"))
                            {
                            }
                            String card2 = card2WithoutSuit + suit2;
                            if (card1.Equals(card2)) continue;

                            boardinfo boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, card1, card2);
                            if ((boardInfo.madehand == postflophand.kStraight || boardInfo.madehand == postflophand.kStraightFlush) && boardInfo.holesused > 0)
                            {
                                if (!cardsUsedForStraight.ContainsKey(card1 + card2) && !cardsUsedForStraight.ContainsKey(card2 + card1))
                                {
                                    cardsUsedForStraight.Add(card1 + card2, "");
                                    nbStraights++;
                                }
                            }
                            else if (boardInfo.madehand == postflophand.kFlush)
                            {
                                if (!cardsUsedForFlush.ContainsKey(card1 + card2) && !cardsUsedForFlush.ContainsKey(card2 + card1))
                                {
                                    cardsUsedForFlush.Add(card1 + card2, "");
                                    nbFlush++;
                                }
                            }
                        }
                    }
                }
            }
            return new int[] { nbFlush, nbStraights };
        }



        String HandIsPaired(HandHistory handHistory, String oppCard1, String oppCard2, int street)
        {
            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;
            String board = boardCard1 + boardCard2 + boardCard3 + (boardCard4 == null ? "" : boardCard4) + (boardCard5 == null ? "" : boardCard5);

            boardinfo boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, oppCard1, oppCard2);

            if (boardInfo.madehand == postflophand.kPair && boardInfo.holesused > 0)
            {
                return board.Contains(oppCard1[0].ToString()) ? oppCard1 : oppCard2;
            }
            return null;
        }

        bool HandIsStraight(HandHistory handHistory, String oppCard1, String oppCard2, int street)
        {
            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;
            String board = boardCard1 + boardCard2 + boardCard3 + (boardCard4 == null ? "" : boardCard4) + (boardCard5 == null ? "" : boardCard5);

            boardinfo boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, oppCard1, oppCard2);

            if ((boardInfo.madehand == postflophand.kStraight || boardInfo.madehand == postflophand.kStraightFlush) && boardInfo.holesused > 0)
            {
                return true;
            }
            return false;
        }

        bool HandIsNutFlushDraw(HandHistory handHistory, String oppCard1, String oppCard2, int street)
        {
            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;
            String board = boardCard1 + boardCard2 + boardCard3 + (boardCard4 == null ? "" : boardCard4) + (boardCard5 == null ? "" : boardCard5);

            boardinfo boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, oppCard1, oppCard2);

            if (boardInfo.ifflushdraw && boardInfo.drawflushcardsmissing == 1 && boardInfo.drawflushholesused > 0 && boardInfo.madehand != postflophand.kFlush && boardInfo.madehand != postflophand.kStraightFlush)
            {
                return oppCard1[0].Equals('A') || oppCard2[0].Equals('A');
            }
            return false;
        }

        bool HandIsTopPairOrBetter(HandHistory handHistory, String oppCard1, String oppCard2, int street, bool betterThanTopPair, bool betterThanMiddlePair, out String topCard, bool orOverPair, bool onlyStraightOrFlush)
        {
            topCard = null;
            if (orOverPair && oppCard1[0].Equals(oppCard2[0]) && Card.AllCardsList.IndexOf(oppCard1[0].ToString()) >= Card.AllCardsList.IndexOf("J"))
            {
                return true;
            }
            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;
            boardinfo boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, oppCard1, oppCard2);


            //GET TOP CARD
            List<String> boardCards = new List<string>();
            boardCards.Add(boardCard1);
            boardCards.Add(boardCard2);
            boardCards.Add(boardCard3);
            if (boardCard4 != null)
                boardCards.Add(boardCard4);
            if (boardCard5 != null)
                boardCards.Add(boardCard5);
            foreach (String boardCard in boardCards)
            {
                if (topCard != null && boardCard[0] == topCard[0])
                {
                    topCard = null;
                    break;
                }

                if (topCard == null) topCard = boardCard;
                else if (Card.AllCardsList.IndexOf(boardCard[0].ToString()) > Card.AllCardsList.IndexOf(topCard[0].ToString()))
                    topCard = boardCard;
            }
            //

            if (!betterThanTopPair && !onlyStraightOrFlush && (boardInfo.madehand == postflophand.kPair || (boardInfo.madehand == postflophand.k2Pair && oppCard1[0].Equals(oppCard2[0]))) && boardInfo.holesused > 0)
            {
                List<String> allCardsOnTheBoard = new List<String>();
                if (handHistory.CommunityCards[1] != null)
                {
                    allCardsOnTheBoard.Add(handHistory.CommunityCards[1][0].ToString());
                    allCardsOnTheBoard.Add(handHistory.CommunityCards[1][2].ToString());
                    allCardsOnTheBoard.Add(handHistory.CommunityCards[1][4].ToString());
                }

                if (handHistory.CommunityCards[2] != null && street >= 2)
                    allCardsOnTheBoard.Add(handHistory.CommunityCards[2][0].ToString());
                if (handHistory.CommunityCards[3] != null && street == 3)
                    allCardsOnTheBoard.Add(handHistory.CommunityCards[3][0].ToString());

                List<String> allCards = new List<String>(Card.AllCards);

                int highestBoardCardIndex = -1;
                foreach (String cardOnTheBoard in allCardsOnTheBoard)
                {
                    if (allCards.IndexOf(cardOnTheBoard) > highestBoardCardIndex) highestBoardCardIndex = allCards.IndexOf(cardOnTheBoard);
                }
                int middleBoardCardIndex = -1;
                String middleCard = null;
                if (betterThanMiddlePair)
                {
                    middleCard = GetMiddleCardOnStreet(handHistory, street);
                    if (middleCard != null) middleBoardCardIndex = Card.AllCardsList.IndexOf(middleCard[0].ToString());
                }
                int holeCard1Index = allCards.IndexOf(oppCard1[0].ToString());
                int holeCard2Index = allCards.IndexOf(oppCard2[0].ToString());

                if ((highestBoardCardIndex == holeCard1Index || highestBoardCardIndex == holeCard2Index ||
                    (holeCard1Index > highestBoardCardIndex && holeCard1Index == holeCard2Index)
                    )
                    ||
                    (middleBoardCardIndex != -1 &&
                    (middleBoardCardIndex == holeCard1Index || middleBoardCardIndex == holeCard2Index ||
                    (holeCard1Index > middleBoardCardIndex && holeCard1Index == holeCard2Index)
                    )
                    )
                    )
                {
                    int topCardIndex = (holeCard1Index > highestBoardCardIndex && holeCard1Index == holeCard2Index) ? holeCard1Index : highestBoardCardIndex;
                    topCard = allCards[topCardIndex];

                    return oppCard1.StartsWith(topCard) || oppCard2.StartsWith(topCard);
                }
            }

            if (onlyStraightOrFlush)
            {
                if ((boardInfo.madehand == postflophand.kFlush
                   || boardInfo.madehand == postflophand.kStraight
                   || boardInfo.madehand == postflophand.kStraightFlush) && boardInfo.holesused > 0)
                    return true; //STRAIGHT OR FLUSH
                else return false;
            }
            if (((boardInfo.madehand == postflophand.k2Pair && oppCard1[0] != oppCard2[0])
                || boardInfo.madehand == postflophand.k3ofKind
                || boardInfo.madehand == postflophand.k4ofKind
                || boardInfo.madehand == postflophand.kFlush
                || boardInfo.madehand == postflophand.kFullHouse
                || boardInfo.madehand == postflophand.kStraight
                || boardInfo.madehand == postflophand.kStraightFlush) && boardInfo.holesused > 0)
                return true; //TOP PAIR OR BETTER

            return false;
        }

        int GetBroadwayNBOnBoard(HandHistory handHistory)
        {
            List<String> comCards = new List<String>();
            if (handHistory.CommunityCards[1] != null)
            {
                comCards.Add(handHistory.CommunityCards[1][0].ToString());
                comCards.Add(handHistory.CommunityCards[1][2].ToString());
                comCards.Add(handHistory.CommunityCards[1][4].ToString());
            }
            if (handHistory.CommunityCards[2] != null)
            {
                comCards.Add(handHistory.CommunityCards[2][0].ToString());
            }
            if (handHistory.CommunityCards[3] != null)
            {
                comCards.Add(handHistory.CommunityCards[3][0].ToString());
            }

            Hashtable cardsToSearch = new Hashtable();
            cardsToSearch.Add("J", 0);
            cardsToSearch.Add("Q", 0);
            cardsToSearch.Add("K", 0);
            cardsToSearch.Add("A", 0);

            foreach (String comCard in comCards)
            {
                if (cardsToSearch.ContainsKey(comCard))
                {
                    cardsToSearch[comCard] = (int)cardsToSearch[comCard] + 1;
                }
            }

            int nbBroadways = 0;
            foreach (String cardToSearch in cardsToSearch.Keys)
            {
                if ((int)cardsToSearch[cardToSearch] == 1) nbBroadways++;
            }

            return nbBroadways;
        }



        Hashtable bluffHands = new Hashtable();
        bool canMakeDrawWithHand(HandHistory handHistory, String oppCard1, String oppCard2, int street)
        {
            if (handHistory.CommunityCards[1] == null) return false;

            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;

            boardinfo boardinfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, oppCard1, oppCard2);
            if ((boardinfo.ifflushdraw && boardinfo.drawflushcardsmissing == 1 && boardinfo.drawflushholesused > 0 && boardinfo.madehand != postflophand.kFlush && boardinfo.madehand != postflophand.kStraightFlush)
                || (boardinfo.ifstraightdraw && boardinfo.drawstraightholesused > 0 && boardinfo.madehand != postflophand.kStraight && boardinfo.madehand != postflophand.kStraightFlush))
            {
                return true;
            }
            return false;
        }

        bool canMakeOESDWithHand(HandHistory handHistory, String oppCard1, String oppCard2, int street)
        {
            if (handHistory.CommunityCards[1] == null) return false;

            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;

            boardinfo boardinfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, oppCard1, oppCard2);

            if (boardinfo.ifstraightdraw && boardinfo.drawtype != 2)
            {
                return true;
            }
            return false;
        }

        List<String> GetBoardCards(HandHistory handHistory, int street)
        {
            String boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            String boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            String boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            String boardCard4 = street >= 2 ? handHistory.CommunityCards[2] : null;
            String boardCard5 = street >= 3 ? handHistory.CommunityCards[3] : null;

            List<String> boardCards = new List<String>();
            boardCards.Add(boardCard1);
            boardCards.Add(boardCard2);
            boardCards.Add(boardCard3);
            if (boardCard4 != null)
                boardCards.Add(boardCard4);
            if (boardCard5 != null)
                boardCards.Add(boardCard5);

            return boardCards;
        }
        void FilterBluffHands(HandHistory handHistory, double drawBluffPrct, int street, bool noGutshotDraw, Player strongestOpponentPlayer)
        {
            if (bluffHands.Count == 0) return;
            List<String> filteredBluffHands = new List<String>();
            List<String> allBluffHands = new List<String>();
            foreach (String bluffHand in bluffHands.Keys)
            {
                //String groupedHand = bluffHand[0].ToString() + bluffHand[2].ToString() + (bluffHand[0].Equals(bluffHand[2]) ? "" : bluffHand[1].Equals(bluffHand[3]) ? "s" : "o");
                //allBluffHands.Add(groupedHand);
                allBluffHands.Add(bluffHand);
            }



            //allBluffHands = UngroupHands(allBluffHands, handHistory);

            bool checkCheckOnTheFlop = CheckCheckScenarioOnStreet(handHistory, strongestOpponentPlayer, 1);
            bool HeroBetOpponentCalledOnTheTurn = BetCallScenarioOnStreet(handHistory, 2, strongestOpponentPlayer, false, false);
            bool OpponentBetHeroCalledOnTheTurn = BetCallScenarioOnStreet(handHistory, 2, strongestOpponentPlayer, true, false);
            bool strongestOpponentRaisedOnTheRiver = PlayerRaisedOnStreet(handHistory, strongestOpponentPlayer, 3);
            String suitAppearingThreeTimesOnTheBoard = GetSuitWithXOccurrencesOnTheBoard(handHistory, street, 3);


            String all = "";

            List<String> bluffHandsToRemove = new List<String>();
            //SORT STRONGEST FIRST
            for (int i = 0; i < allBluffHands.Count - 1; i++)
            {
                int maxWeightIndex = i;
                for (int j = i + 1; j < allBluffHands.Count; j++)
                {
                    String bluffHand = allBluffHands[j];

                    int handWeight = Card.AllCardsList.IndexOf(allBluffHands[j][0].ToString()) + Card.AllCardsList.IndexOf(allBluffHands[j][2].ToString());

                    if (canMakeOESDWithHand(handHistory, allBluffHands[j].Substring(0, 2), allBluffHands[j].Substring(2, 2), street))
                        handWeight += 50;

                    String hand = allBluffHands[j];

                    if (checkCheckOnTheFlop && HeroBetOpponentCalledOnTheTurn && suitAppearingThreeTimesOnTheBoard != null && !strongestOpponentRaisedOnTheRiver && (allBluffHands[j][1].ToString().Equals(suitAppearingThreeTimesOnTheBoard) || allBluffHands[j][3].ToString().Equals(suitAppearingThreeTimesOnTheBoard)))// && !allBluffHands[j][0].Equals(allBluffHands[j][2]))
                    {
                        if (HandIsPaired(handHistory, allBluffHands[j].Substring(0, 2), allBluffHands[j].Substring(2, 2), street) != null)
                        {
                            handWeight += 80;
                            all += hand + ",";
                        }
                        else if (!bluffHandsToRemove.Contains(hand))
                            bluffHandsToRemove.Add(hand);
                    }
                    else if (!bluffHandsToRemove.Contains(hand))
                        bluffHandsToRemove.Add(hand);


                    int maxWeight = Card.AllCardsList.IndexOf(allBluffHands[maxWeightIndex][0].ToString()) + Card.AllCardsList.IndexOf(allBluffHands[maxWeightIndex][2].ToString());
                    if (handWeight > maxWeight)
                    {
                        maxWeightIndex = j;
                    }
                }

                String temp = allBluffHands[i];
                allBluffHands[i] = allBluffHands[maxWeightIndex];
                allBluffHands[maxWeightIndex] = temp;
            }

            if (allBluffHands.Count - bluffHandsToRemove.Count > 0)
            {
                foreach (String hand in bluffHandsToRemove)
                {
                    allBluffHands.Remove(hand);
                }
            }
            for (int i = 0; i < drawBluffPrct * allBluffHands.Count; i++)
            {
                if (noGutshotDraw && !allBluffHands[i].Contains("A"))
                {
                    continue;
                }
                filteredBluffHands.Add(allBluffHands[i].ToString());
            }

            bluffHands = new Hashtable();
            foreach (String bluffHand in filteredBluffHands)
            {
                if (!bluffHands.ContainsKey(bluffHand))
                {
                    bluffHands.Add(bluffHand, 0);
                }
            }
        }


        List<String> GetOpponentHandRange(HandHistory handHistory, Player player,
            Player opponentPreflopRaiser,
            bool playerDid3BetPreflop,
            bool playerDid4BetPreflop,
            bool playerIsPreflopRaiser,
            bool playerCalled3BetPreflop,
            bool playerCalled4BetPreflop,
            bool playerLimpedThenCallPreflop
            )
        {
            if (player == null) return new List<String>();

            String playerPosition = GameRules.position_names[handHistory.Players.Count, player.Position];

            //CALLED 4Bet RANGES
            if (playerCalled4BetPreflop)
            {
                List<String> handsToCall4BetWith = GetHandsToCall4BetWith();
                return handsToCall4BetWith;
            }
            //4Bet RANGES
            else if (playerDid4BetPreflop)
            {
                List<String> handsTo4BetWith = GetHandsTo4BetWith();
                return handsTo4BetWith;
            }
            //CALLED 3Bet RANGES
            else if (playerCalled3BetPreflop)
            {
                bool IP = playerPosition.Equals("SB") || playerPosition.Equals("BB");
                List<String> handsToCall3BetWith = IP ? GetHandsToCall3BetWithIP() : GetHandsToCall3BetWithOOP();
                return handsToCall3BetWith;
            }
            //3Bet Ranges
            else if (playerDid3BetPreflop)
            {
                List<String> handsTo3BetWith = GetHandsTo3BetWith();
                return handsTo3BetWith;
            }

            //OPEN RAISING RANGES
            else if (playerIsPreflopRaiser)
            {
                Hashtable handsToOpenRaiseWithPos = GetHandsToOpenRaiseWith();
                if (handsToOpenRaiseWithPos.ContainsKey(playerPosition))
                {
                    List<String> handsToOpenRaiseWith = handsToOpenRaiseWithPos[playerPosition] as List<String>;
                    return handsToOpenRaiseWith;
                }
            }
            else if (playerLimpedThenCallPreflop)
            {
                List<String> handsToLimpThenCallWith = GetHandsToLimpCallWith();
                return handsToLimpThenCallWith;
            }
            else if (PotIsUnraisedPreflop(handHistory))
            {
                List<String> handsToCallUnraisedPotWith = GetHandsCallUnraisedPotWith();
                return handsToCallUnraisedPotWith;
            }
            //CALLED OPEN RAISE
            else if (opponentPreflopRaiser != null)
            {
                String opponentPreflopRaiserPosition = GameRules.position_names[handHistory.Players.Count, opponentPreflopRaiser.Position];
                if (playerPosition.Equals("SB") || playerPosition.Equals("BB")) //Calling ranges... for opponents that are in the blinds, against an...
                {
                    Hashtable handsToCallWithWhenBlindsOpenRaisedPos = GetHandsToCallWithWhenBlindsOpenRaised();
                    if (handsToCallWithWhenBlindsOpenRaisedPos.ContainsKey(opponentPreflopRaiserPosition))
                    {
                        List<String> handsToCallWithWhenBlindsOpenRaised = handsToCallWithWhenBlindsOpenRaisedPos[opponentPreflopRaiserPosition] as List<String>;
                        return handsToCallWithWhenBlindsOpenRaised;
                    }
                }
                else //Calling ranges... for opponents that are NOT in the blinds, against an...
                {
                    Hashtable handsToCallWithWhenNonBlindsOpenRaisedPos = GetHandsToCallWithWhenNonBlindsOpenRaised();
                    if (handsToCallWithWhenNonBlindsOpenRaisedPos.ContainsKey(opponentPreflopRaiserPosition))
                    {
                        List<String> handsToCallWithWhenNonBlindsOpenRaised = handsToCallWithWhenNonBlindsOpenRaisedPos[opponentPreflopRaiserPosition] as List<String>;
                        return handsToCallWithWhenNonBlindsOpenRaised;
                    }
                    else
                    {
                    }
                }
            }
            else
            {
            }
            return new List<String>();
        }

        String GetSuitWithXOccurrencesOnTheBoard(HandHistory handHistory, int street, int nbOccurences)
        {
            List<String> boardCards = GetBoardCards(handHistory, street);
            String[] suits = new String[] { "h", "c", "s", "d" };

            foreach (String suit in suits)
            {
                int occ = 0;
                foreach (String boardCard in boardCards)
                {
                    if (boardCard.EndsWith(suit)) occ++;
                }
                if (occ == nbOccurences)
                {
                    return suit;
                }
            }
            return null;
        }

        Hashtable handsToRemoveFromEquityCalculation = new Hashtable();
        bool ShouldRemoveFromEquityCalculation(HandHistory handHistory, Player player, String hand,
            Player opponentPreflopRaiser,
            bool playerDid3BetPreflop,
            bool playerDid4BetPreflop,
            bool playerIsPreflopRaiser,
            bool playerCalled3BetPreflop,
            bool playerCalled4BetPreflop,
            bool playerLimpedThenCalledPreflop
            )
        {

            if (player == null) return false;

            List<String> handsToUse = new List<String>();
            String oppCard1 = hand.Substring(0, 2);
            String oppCard2 = hand.Substring(2, 2);

            String groupedHand1 = oppCard1[0].ToString() + oppCard2[0].ToString() + (oppCard1[0].Equals(oppCard2[0]) ? "" : oppCard1[1].Equals(oppCard2[1]) ? "s" : "o");
            String groupedHand2 = oppCard2[0].ToString() + oppCard1[0].ToString() + (oppCard1[0].Equals(oppCard2[0]) ? "" : oppCard1[1].Equals(oppCard2[1]) ? "s" : "o");

            bool shouldRemove = false;

            String playerPosition = GameRules.position_names[handHistory.Players.Count, player.Position];

            //CALLED 4Bet RANGES
            if (playerCalled4BetPreflop)
            {
                List<String> handsToCall4BetWith = GetHandsToCall4BetWith();
                if (!handsToCall4BetWith.Contains(groupedHand1) && !handsToCall4BetWith.Contains(groupedHand2))
                    shouldRemove = true;
            }
            //4Bet RANGES
            else if (playerDid4BetPreflop)
            {
                List<String> handsTo4BetWith = GetHandsTo4BetWith();
                if (!handsTo4BetWith.Contains(groupedHand1) && !handsTo4BetWith.Contains(groupedHand2))
                    shouldRemove = true;
            }
            //CALLED 3Bet RANGES
            else if (playerCalled3BetPreflop)
            {
                bool IP = playerPosition.Equals("SB") || playerPosition.Equals("BB");

                List<String> handsToCall3BetWith = IP ? GetHandsToCall3BetWithIP() : GetHandsToCall3BetWithOOP();
                if (!handsToCall3BetWith.Contains(groupedHand1) && !handsToCall3BetWith.Contains(groupedHand2))
                    shouldRemove = true;
            }
            //3Bet Ranges
            else if (playerDid3BetPreflop)
            {
                List<String> handsTo3BetWith = GetHandsTo3BetWith();
                if (!handsTo3BetWith.Contains(groupedHand1) && !handsTo3BetWith.Contains(groupedHand2))
                    shouldRemove = true;
            }

            //OPEN RAISING RANGES
            else if (playerIsPreflopRaiser)
            {
                Hashtable handsToOpenRaiseWithPos = GetHandsToOpenRaiseWith();
                if (handsToOpenRaiseWithPos.ContainsKey(playerPosition))
                {
                    List<String> handsToOpenRaiseWith = handsToOpenRaiseWithPos[playerPosition] as List<String>;
                    if (!handsToOpenRaiseWith.Contains(groupedHand1) && !handsToOpenRaiseWith.Contains(groupedHand2))
                        shouldRemove = true;
                }
            }
            else if (playerLimpedThenCalledPreflop)
            {
                List<String> handsToLimpThenCallWith = GetHandsToLimpCallWith();
                if (!handsToLimpThenCallWith.Contains(groupedHand1) && !handsToLimpThenCallWith.Contains(groupedHand2))
                    shouldRemove = true;
            }
            else if (PotIsUnraisedPreflop(handHistory))
            {
                List<String> handsToCallUnraisedPotWith = GetHandsCallUnraisedPotWith();
                if (!handsToCallUnraisedPotWith.Contains(groupedHand1) && !handsToCallUnraisedPotWith.Contains(groupedHand2))
                    shouldRemove = true;
            }
            //CALLED OPEN RAISE
            else if (opponentPreflopRaiser != null)
            {
                String opponentPreflopRaiserPosition = GameRules.position_names[handHistory.Players.Count, opponentPreflopRaiser.Position];
                if (playerPosition.Equals("SB") || playerPosition.Equals("BB")) //Calling ranges... for opponents that are in the blinds, against an...
                {
                    Hashtable handsToCallWithWhenBlindsOpenRaisedPos = GetHandsToCallWithWhenBlindsOpenRaised();
                    if (handsToCallWithWhenBlindsOpenRaisedPos.ContainsKey(opponentPreflopRaiserPosition))
                    {
                        List<String> handsToCallWithWhenBlindsOpenRaised = handsToCallWithWhenBlindsOpenRaisedPos[opponentPreflopRaiserPosition] as List<String>;
                        if (!handsToCallWithWhenBlindsOpenRaised.Contains(groupedHand1) && !handsToCallWithWhenBlindsOpenRaised.Contains(groupedHand2))
                            shouldRemove = true;
                    }
                }
                else //Calling ranges... for opponents that are NOT in the blinds, against an...
                {
                    Hashtable handsToCallWithWhenNonBlindsOpenRaisedPos = GetHandsToCallWithWhenNonBlindsOpenRaised();
                    if (handsToCallWithWhenNonBlindsOpenRaisedPos.ContainsKey(opponentPreflopRaiserPosition))
                    {
                        List<String> handsToCallWithWhenNonBlindsOpenRaised = handsToCallWithWhenNonBlindsOpenRaisedPos[opponentPreflopRaiserPosition] as List<String>;
                        if (!handsToCallWithWhenNonBlindsOpenRaised.Contains(groupedHand1) && !handsToCallWithWhenNonBlindsOpenRaised.Contains(groupedHand2))
                            shouldRemove = true;
                    }
                }
            }
            else
            {

            }

            if (shouldRemove)
            {
                //bool canBluffWithHandz = canBluffWithHand(handHistory, oppCard1, oppCard2);
                //if (canBluffWithHandz)
                //{
                //}
            }
            return shouldRemove;
        }

        Random rand = new Random();
        int GetRandom(int min, int max)
        {
            return rand.Next(min, max + 1);
        }
    }
}
