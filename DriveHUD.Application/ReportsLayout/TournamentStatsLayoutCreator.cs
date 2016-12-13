using DriveHUD.Common.Wpf.Helpers;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class TournamentStatsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Table Type", nameof(TournamentReportRecord.TableType)));
            gridView.Columns.Add(Add("Game Type", nameof(TournamentReportRecord.GameType)));
            gridView.Columns.Add(Add("Speed", nameof(TournamentReportRecord.TournamentSpeed)));
            gridView.Columns.Add(AddFinancial("Buy-in", nameof(TournamentReportRecord.BuyIn)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(TournamentReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(TournamentReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(TournamentReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("Agg", nameof(TournamentReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("3-Bet%", nameof(TournamentReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("WTSD", nameof(TournamentReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(TournamentReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("WWSF", nameof(TournamentReportRecord.WSWSF)));
            gridView.Columns.Add(AddPercentile("Flop c-bet%", nameof(TournamentReportRecord.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Fold to Flop C-bet%", nameof(TournamentReportRecord.FoldCBet)));
            gridView.Columns.Add(AddPercentile("Steal %", nameof(TournamentReportRecord.Steal)));
            gridView.Columns.Add(AddPercentile("Blinds re-raise steal", nameof(TournamentReportRecord.BlindsReraiseSteal)));
            gridView.Columns.Add(AddPercentile("Blinds fold to steal", nameof(TournamentReportRecord.BlindsFoldSteal)));
            gridView.Columns.Add(AddPercentile("4-bet%", nameof(TournamentReportRecord.FourBet)));
            gridView.Columns.Add(AddPercentile("UO-PFR EP%", nameof(TournamentReportRecord.UO_PFR_EP)));
            gridView.Columns.Add(AddPercentile("UO-PFR MP%", nameof(TournamentReportRecord.UO_PFR_MP)));
            gridView.Columns.Add(AddPercentile("UO-PFR CO%", nameof(TournamentReportRecord.UO_PFR_CO)));
            gridView.Columns.Add(AddPercentile("UO-PFR BN%", nameof(TournamentReportRecord.UO_PFR_BN)));
            gridView.Columns.Add(AddPercentile("UO-PFR SB%", nameof(TournamentReportRecord.UO_PFR_SB)));

            gridView.Columns.Add(AddPercentile("4-Bet%", nameof(TournamentReportRecord.FourBet), false));
            gridView.Columns.Add(AddPercentile("Check-Raise%", nameof(TournamentReportRecord.CheckRaise), false));
            gridView.Columns.Add(AddPercentile("Cold Call%", nameof(TournamentReportRecord.ColdCall), false));
            gridView.Columns.Add(AddPercentile("Flop AGG%", nameof(TournamentReportRecord.FlopAgg), false));
            gridView.Columns.Add(AddPercentile("Fold to 3-Bet%", nameof(TournamentReportRecord.FoldToThreeBet), false));
            gridView.Columns.Add(AddPercentile("Fold to 4-Bet%", nameof(TournamentReportRecord.FoldToFourBet), false));
            gridView.Columns.Add(AddPercentile("River AGG%", nameof(TournamentReportRecord.RiverAgg), false));
            gridView.Columns.Add(AddPercentile("Squeeze%", nameof(TournamentReportRecord.Squeeze), false));
            gridView.Columns.Add(AddPercentile("Turn AGG%", nameof(TournamentReportRecord.TurnAgg), false));
            gridView.Columns.Add(AddPercentile("3-Bet BB%", nameof(TournamentReportRecord.ThreeBetInBB), false));
            gridView.Columns.Add(AddPercentile("3-Bet BTN%", nameof(TournamentReportRecord.ThreeBetInBTN), false));
            gridView.Columns.Add(AddPercentile("3-Bet CO%", nameof(TournamentReportRecord.ThreeBetInCO), false));
            gridView.Columns.Add(AddPercentile("3-Bet IP%", nameof(TournamentReportRecord.ThreeBetIP), false));
            gridView.Columns.Add(AddPercentile("3-Bet MP%", nameof(TournamentReportRecord.ThreeBetInMP), false));
            gridView.Columns.Add(AddPercentile("3-Bet OPP%", nameof(TournamentReportRecord.ThreeBetOOP), false));
            gridView.Columns.Add(AddPercentile("3-Bet SB%", nameof(TournamentReportRecord.ThreeBetInSB), false));
            gridView.Columns.Add(Add("3-Bet vs. Steal", nameof(TournamentReportRecord.ThreeBetVsSteal), false));
            gridView.Columns.Add(AddPercentile("C-Bet in 3-Bet Pot%", nameof(TournamentReportRecord.FlopCBetInThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 3-Bet Pot%", nameof(TournamentReportRecord.FoldFlopCBetFromThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("4-Bet BB%", nameof(TournamentReportRecord.FourBetInBB), false));
            gridView.Columns.Add(AddPercentile("4-Bet BTN%", nameof(TournamentReportRecord.FourBetInBTN), false));
            gridView.Columns.Add(AddPercentile("4-Bet CO%", nameof(TournamentReportRecord.FourBetInCO), false));
            gridView.Columns.Add(AddPercentile("4-Bet MP%", nameof(TournamentReportRecord.FourBetInMP), false));
            gridView.Columns.Add(AddPercentile("4-Bet SB%", nameof(TournamentReportRecord.FourBetInSB), false));
            gridView.Columns.Add(AddPercentile("C-Bet 4-Bet Pot%", nameof(TournamentReportRecord.FlopCBetInFourBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 4-Bet Pot%", nameof(TournamentReportRecord.FoldFlopCBetFromFourBetPot), false));
            gridView.Columns.Add(AddPercentile("C-Bet IP%", nameof(TournamentReportRecord.CBetIP), false));
            gridView.Columns.Add(AddPercentile("C-Bet OOP%", nameof(TournamentReportRecord.CBetOOP), false));
            gridView.Columns.Add(AddPercentile("C-Bet Monotone Pot%", nameof(TournamentReportRecord.FlopCBetMonotone), false));
            gridView.Columns.Add(AddPercentile("C-Bet MW Pot%", nameof(TournamentReportRecord.FlopCBetMW), false));
            gridView.Columns.Add(AddPercentile("C-Bet Rag Flop%", nameof(TournamentReportRecord.FlopCBetRag), false));
            gridView.Columns.Add(AddPercentile("C-Bet vs 1 Opp%", nameof(TournamentReportRecord.FlopCBetVsOneOpp), false));
            gridView.Columns.Add(AddPercentile("C-Bet vs 2 Opp%", nameof(TournamentReportRecord.FlopCBetVsTwoOpp), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 3-Bet Pot%", nameof(TournamentReportRecord.FoldFlopCBetFromThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 4-Bet Pot%", nameof(TournamentReportRecord.FoldFlopCBetFromFourBetPot), false));
            gridView.Columns.Add(AddPercentile("Raise C-Bet%", nameof(TournamentReportRecord.RaiseCBet), false));
            gridView.Columns.Add(AddPercentile("Cold Call BB%", nameof(TournamentReportRecord.ColdCallInBB), false));
            gridView.Columns.Add(AddPercentile("Cold Call BTN%", nameof(TournamentReportRecord.ColdCallInBTN), false));
            gridView.Columns.Add(AddPercentile("Cold Call CO%", nameof(TournamentReportRecord.ColdCallInCO), false));
            gridView.Columns.Add(AddPercentile("Cold Call MP%", nameof(TournamentReportRecord.ColdCallInMP), false));
            gridView.Columns.Add(AddPercentile("Cold Call SB%", nameof(TournamentReportRecord.ColdCallInSB), false));
            gridView.Columns.Add(AddPercentile("Float Flop%", nameof(TournamentReportRecord.FloatFlop), false));
            gridView.Columns.Add(AddPercentile("Flop Check-Raise%", nameof(TournamentReportRecord.FlopCheckRaise), false));
            gridView.Columns.Add(AddPercentile("Raise Flop%", nameof(TournamentReportRecord.RaiseFlop), false));
            gridView.Columns.Add(AddPercentile("Delayed C-Bet%", nameof(TournamentReportRecord.DidDelayedTurnCBet), false));
            gridView.Columns.Add(AddPercentile("Raise Turn%", nameof(TournamentReportRecord.RaiseTurn), false));
            gridView.Columns.Add(AddPercentile("Seen Turn%", nameof(TournamentReportRecord.TurnSeen), false));
            gridView.Columns.Add(AddPercentile("Turn AGG%", nameof(TournamentReportRecord.TurnAgg), false));
            gridView.Columns.Add(AddPercentile("Turn Check-Raise%", nameof(TournamentReportRecord.TurnCheckRaise), false));
            gridView.Columns.Add(Add("Check River On BX Line%", nameof(TournamentReportRecord.CheckRiverOnBXLine), false));
            gridView.Columns.Add(AddPercentile("Raise River%", nameof(TournamentReportRecord.RaiseRiver), false));
            gridView.Columns.Add(AddPercentile("River AGG%", nameof(TournamentReportRecord.RiverAgg), false));
            gridView.Columns.Add(AddPercentile("Seen River%", nameof(TournamentReportRecord.RiverSeen), false));
            gridView.Columns.Add(AddPercentile("Limp Call%", nameof(TournamentReportRecord.DidLimpCall), false));
            gridView.Columns.Add(AddPercentile("Limp Fold%", nameof(TournamentReportRecord.DidLimpFold), false));
            gridView.Columns.Add(AddPercentile("Limp Reraise%", nameof(TournamentReportRecord.DidLimpReraise), false));
            gridView.Columns.Add(AddPercentile("Limp%", nameof(TournamentReportRecord.DidLimp), false));
            gridView.Columns.Add(AddPercentile("Donk Bet%", nameof(TournamentReportRecord.DonkBet), false));
            gridView.Columns.Add(AddPercentile("Raise Frequency Factor%", nameof(TournamentReportRecord.RaiseFrequencyFactor), false));
            gridView.Columns.Add(AddPercentile("True Aggression% (TAP)", nameof(TournamentReportRecord.TrueAggression), false));

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column.Header as string);
            }
        }
    }
}
