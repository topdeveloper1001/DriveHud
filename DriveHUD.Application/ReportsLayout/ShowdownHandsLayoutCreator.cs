using DriveHUD.Common.Reflection;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class ShowdownHandsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Showdown Hand", nameof(ShowdownHandsReportRecord.ShowdownHandString), GetColumnWidth("Straight Flush: With 2 Pocket Cards")));
            gridView.Columns.Add(Add("Total Hands", nameof(ShowdownHandsReportRecord.TotalHands)));
            gridView.Columns.Add(AddPercentile("Won%", nameof(ShowdownHandsReportRecord.WonHandProc)));
            gridView.Columns.Add(AddFinancial("Net Won", ReflectionHelper.GetPath<ShowdownHandsReportRecord>(o => o.Source.NetWon)));
            gridView.Columns.Add(Add("bb/100", nameof(ShowdownHandsReportRecord.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(ShowdownHandsReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(ShowdownHandsReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(ShowdownHandsReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("Agg", nameof(ShowdownHandsReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("3Bet%", nameof(ShowdownHandsReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(ShowdownHandsReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(ShowdownHandsReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(ShowdownHandsReportRecord.WSWSF)));

            gridView.Columns.Add(AddPercentile("4-Bet%", nameof(Indicators.FourBet), false));
            gridView.Columns.Add(AddPercentile("Check-Raise%", nameof(Indicators.CheckRaise), false));
            gridView.Columns.Add(AddPercentile("Cold Call%", nameof(Indicators.ColdCall), false));
            gridView.Columns.Add(AddPercentile("Flop AGG%", nameof(Indicators.FlopAgg), false));
            gridView.Columns.Add(AddPercentile("Fold to 3-Bet%", nameof(Indicators.FoldToThreeBet), false));
            gridView.Columns.Add(AddPercentile("Fold to 4-Bet%", nameof(Indicators.FoldToFourBet), false));
            gridView.Columns.Add(AddPercentile("River AGG%", nameof(Indicators.RiverAgg), false));
            gridView.Columns.Add(AddPercentile("Squeeze%", nameof(Indicators.Squeeze), false));
            gridView.Columns.Add(AddPercentile("Turn AGG%", nameof(Indicators.TurnAgg), false));
            gridView.Columns.Add(AddPercentile("3-Bet BB%", nameof(Indicators.ThreeBetInBB), false));
            gridView.Columns.Add(AddPercentile("3-Bet BTN%", nameof(Indicators.ThreeBetInBTN), false));
            gridView.Columns.Add(AddPercentile("3-Bet CO%", nameof(Indicators.ThreeBetInCO), false));
            gridView.Columns.Add(AddPercentile("3-Bet IP%", nameof(Indicators.ThreeBetIP), false));
            gridView.Columns.Add(AddPercentile("3-Bet MP%", nameof(Indicators.ThreeBetInMP), false));
            gridView.Columns.Add(AddPercentile("3-Bet OPP%", nameof(Indicators.ThreeBetOOP), false));
            gridView.Columns.Add(AddPercentile("3-Bet SB%", nameof(Indicators.ThreeBetInSB), false));
            gridView.Columns.Add(Add("3-Bet vs. Steal", nameof(Indicators.ThreeBetVsSteal), false));
            gridView.Columns.Add(AddPercentile("C-Bet in 3-Bet Pot%", nameof(Indicators.FlopCBetInThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 3-Bet Pot%", nameof(Indicators.FoldFlopCBetFromThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("4-Bet BB%", nameof(Indicators.FourBetInBB), false));
            gridView.Columns.Add(AddPercentile("4-Bet BTN%", nameof(Indicators.FourBetInBTN), false));
            gridView.Columns.Add(AddPercentile("4-Bet CO%", nameof(Indicators.FourBetInCO), false));
            gridView.Columns.Add(AddPercentile("4-Bet MP%", nameof(Indicators.FourBetInMP), false));
            gridView.Columns.Add(AddPercentile("4-Bet SB%", nameof(Indicators.FourBetInSB), false));
            gridView.Columns.Add(AddPercentile("C-Bet 4-Bet Pot%", nameof(Indicators.FlopCBetInFourBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 4-Bet Pot%", nameof(Indicators.FoldFlopCBetFromFourBetPot), false));
            gridView.Columns.Add(AddPercentile("C-Bet IP%", nameof(Indicators.CBetIP), false));
            gridView.Columns.Add(AddPercentile("C-Bet OOP%", nameof(Indicators.CBetOOP), false));
            gridView.Columns.Add(AddPercentile("C-Bet Monotone Pot%", nameof(Indicators.FlopCBetMonotone), false));
            gridView.Columns.Add(AddPercentile("C-Bet MW Pot%", nameof(Indicators.FlopCBetMW), false));
            gridView.Columns.Add(AddPercentile("C-Bet Rag Flop%", nameof(Indicators.FlopCBetRag), false));
            gridView.Columns.Add(AddPercentile("C-Bet vs 1 Opp%", nameof(Indicators.FlopCBetVsOneOpp), false));
            gridView.Columns.Add(AddPercentile("C-Bet vs 2 Opp%", nameof(Indicators.FlopCBetVsTwoOpp), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 3-Bet Pot%", nameof(Indicators.FoldFlopCBetFromThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 4-Bet Pot%", nameof(Indicators.FoldFlopCBetFromFourBetPot), false));
            gridView.Columns.Add(AddPercentile("Raise C-Bet%", nameof(Indicators.RaiseCBet), false));
            gridView.Columns.Add(AddPercentile("Cold Call BB%", nameof(Indicators.ColdCallInBB), false));
            gridView.Columns.Add(AddPercentile("Cold Call BTN%", nameof(Indicators.ColdCallInBTN), false));
            gridView.Columns.Add(AddPercentile("Cold Call CO%", nameof(Indicators.ColdCallInCO), false));
            gridView.Columns.Add(AddPercentile("Cold Call MP%", nameof(Indicators.ColdCallInMP), false));
            gridView.Columns.Add(AddPercentile("Cold Call SB%", nameof(Indicators.ColdCallInSB), false));
            gridView.Columns.Add(AddPercentile("Float Flop%", nameof(Indicators.FloatFlop), false));
            gridView.Columns.Add(AddPercentile("Flop Check-Raise%", nameof(Indicators.FlopCheckRaise), false));
            gridView.Columns.Add(AddPercentile("Raise Flop%", nameof(Indicators.RaiseFlop), false));
            gridView.Columns.Add(AddPercentile("Delayed C-Bet%", nameof(Indicators.DidDelayedTurnCBet), false));
            gridView.Columns.Add(AddPercentile("Raise Turn%", nameof(Indicators.RaiseTurn), false));
            gridView.Columns.Add(AddPercentile("Seen Turn%", nameof(Indicators.TurnSeen), false));
            gridView.Columns.Add(AddPercentile("Turn AGG%", nameof(Indicators.TurnAgg), false));
            gridView.Columns.Add(AddPercentile("Turn Check-Raise%", nameof(Indicators.TurnCheckRaise), false));
            gridView.Columns.Add(Add("Check River On BX Line%", nameof(Indicators.CheckRiverOnBXLine), false));
            gridView.Columns.Add(AddPercentile("Raise River%", nameof(Indicators.RaiseRiver), false));
            gridView.Columns.Add(AddPercentile("River AGG%", nameof(Indicators.RiverAgg), false));
            gridView.Columns.Add(AddPercentile("Seen River%", nameof(Indicators.RiverSeen), false));
            gridView.Columns.Add(AddPercentile("Limp Call%", nameof(Indicators.DidLimpCall), false));
            gridView.Columns.Add(AddPercentile("Limp Fold%", nameof(Indicators.DidLimpFold), false));
            gridView.Columns.Add(AddPercentile("Limp Reraise%", nameof(Indicators.DidLimpReraise), false));
            gridView.Columns.Add(AddPercentile("Limp%", nameof(Indicators.DidLimp), false));
            gridView.Columns.Add(AddPercentile("Donk Bet%", nameof(Indicators.DonkBet), false));
            gridView.Columns.Add(AddPercentile("Raise Frequency Factor%", nameof(Indicators.RaiseFrequencyFactor), false));
            gridView.Columns.Add(AddPercentile("True Aggression% (TAP)", nameof(Indicators.TrueAggression), false));

            for (int i = 1; i < gridView.Columns.Count; i++)
            {
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i].Header as string) + 10;
            }
        }
    }
}
