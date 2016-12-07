using DriveHUD.Common.Wpf.Converters;
using Model.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class TournamentStackSizeLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(GetMRatioColumn("M-Ratio", nameof(MRatioReportRecord.MRatio), GetColumnWidth("M-Ratio") + 10));
            gridView.Columns.Add(Add("Total Hands", nameof(MRatioReportRecord.TotalHands)));
            gridView.Columns.Add(AddFinancial("Net Won", nameof(MRatioReportRecord.TotalWon)));
            gridView.Columns.Add(Add("bb/100", nameof(MRatioReportRecord.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(MRatioReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(MRatioReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Agg", nameof(MRatioReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(MRatioReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("3-Bet%", nameof(MRatioReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(MRatioReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(MRatioReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(MRatioReportRecord.WSWSF)));

            gridView.Columns.Add(AddPercentile("4-Bet%", nameof(MRatioReportRecord.FourBet), false));
            gridView.Columns.Add(AddPercentile("Check-Raise%", nameof(MRatioReportRecord.CheckRaise), false));
            gridView.Columns.Add(AddPercentile("Cold Call%", nameof(MRatioReportRecord.ColdCall), false));
            gridView.Columns.Add(AddPercentile("Flop AGG%", nameof(MRatioReportRecord.FlopAgg), false));
            gridView.Columns.Add(AddPercentile("Fold to 3-Bet%", nameof(MRatioReportRecord.FoldToThreeBet), false));
            gridView.Columns.Add(AddPercentile("Fold to 4-Bet%", nameof(MRatioReportRecord.FoldToFourBet), false));
            gridView.Columns.Add(AddPercentile("River AGG%", nameof(MRatioReportRecord.RiverAgg), false));
            gridView.Columns.Add(AddPercentile("Squeeze%", nameof(MRatioReportRecord.Squeeze), false));
            gridView.Columns.Add(AddPercentile("Turn AGG%", nameof(MRatioReportRecord.TurnAgg), false));
            gridView.Columns.Add(AddPercentile("3-Bet BB%", nameof(MRatioReportRecord.ThreeBetInBB), false));
            gridView.Columns.Add(AddPercentile("3-Bet BTN%", nameof(MRatioReportRecord.ThreeBetInBTN), false));
            gridView.Columns.Add(AddPercentile("3-Bet CO%", nameof(MRatioReportRecord.ThreeBetInCO), false));
            gridView.Columns.Add(AddPercentile("3-Bet IP%", nameof(MRatioReportRecord.ThreeBetIP), false));
            gridView.Columns.Add(AddPercentile("3-Bet MP%", nameof(MRatioReportRecord.ThreeBetInMP), false));
            gridView.Columns.Add(AddPercentile("3-Bet OPP%", nameof(MRatioReportRecord.ThreeBetOOP), false));
            gridView.Columns.Add(AddPercentile("3-Bet SB%", nameof(MRatioReportRecord.ThreeBetInSB), false));
            gridView.Columns.Add(Add("3-Bet vs. Steal", nameof(MRatioReportRecord.ThreeBetVsSteal), false));
            gridView.Columns.Add(AddPercentile("C-Bet in 3-Bet Pot%", nameof(MRatioReportRecord.FlopCBetInThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 3-Bet Pot%", nameof(MRatioReportRecord.FoldFlopCBetFromThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("4-Bet BB%", nameof(MRatioReportRecord.FourBetInBB), false));
            gridView.Columns.Add(AddPercentile("4-Bet BTN%", nameof(MRatioReportRecord.FourBetInBTN), false));
            gridView.Columns.Add(AddPercentile("4-Bet CO%", nameof(MRatioReportRecord.FourBetInCO), false));
            gridView.Columns.Add(AddPercentile("4-Bet MP%", nameof(MRatioReportRecord.FourBetInMP), false));
            gridView.Columns.Add(AddPercentile("4-Bet SB%", nameof(MRatioReportRecord.FourBetInSB), false));
            gridView.Columns.Add(AddPercentile("C-Bet 4-Bet Pot%", nameof(MRatioReportRecord.FlopCBetInFourBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 4-Bet Pot%", nameof(MRatioReportRecord.FoldFlopCBetFromFourBetPot), false));
            gridView.Columns.Add(AddPercentile("C-Bet IP%", nameof(MRatioReportRecord.CBetIP), false));
            gridView.Columns.Add(AddPercentile("C-Bet OOP%", nameof(MRatioReportRecord.CBetOOP), false));
            gridView.Columns.Add(AddPercentile("C-Bet Monotone Pot%", nameof(MRatioReportRecord.FlopCBetMonotone), false));
            gridView.Columns.Add(AddPercentile("C-Bet MW Pot%", nameof(MRatioReportRecord.FlopCBetMW), false));
            gridView.Columns.Add(AddPercentile("C-Bet Rag Flop%", nameof(MRatioReportRecord.FlopCBetRag), false));
            gridView.Columns.Add(AddPercentile("C-Bet vs 1 Opp%", nameof(MRatioReportRecord.FlopCBetVsOneOpp), false));
            gridView.Columns.Add(AddPercentile("C-Bet vs 2 Opp%", nameof(MRatioReportRecord.FlopCBetVsTwoOpp), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 3-Bet Pot%", nameof(MRatioReportRecord.FoldFlopCBetFromThreeBetPot), false));
            gridView.Columns.Add(AddPercentile("Fold to C-Bet 4-Bet Pot%", nameof(MRatioReportRecord.FoldFlopCBetFromFourBetPot), false));
            gridView.Columns.Add(AddPercentile("Raise C-Bet%", nameof(MRatioReportRecord.RaiseCBet), false));
            gridView.Columns.Add(AddPercentile("Cold Call BB%", nameof(MRatioReportRecord.ColdCallInBB), false));
            gridView.Columns.Add(AddPercentile("Cold Call BTN%", nameof(MRatioReportRecord.ColdCallInBTN), false));
            gridView.Columns.Add(AddPercentile("Cold Call CO%", nameof(MRatioReportRecord.ColdCallInCO), false));
            gridView.Columns.Add(AddPercentile("Cold Call MP%", nameof(MRatioReportRecord.ColdCallInMP), false));
            gridView.Columns.Add(AddPercentile("Cold Call SB%", nameof(MRatioReportRecord.ColdCallInSB), false));
            gridView.Columns.Add(AddPercentile("Float Flop%", nameof(MRatioReportRecord.FloatFlop), false));
            gridView.Columns.Add(AddPercentile("Flop Check-Raise%", nameof(MRatioReportRecord.FlopCheckRaise), false));
            gridView.Columns.Add(AddPercentile("Raise Flop%", nameof(MRatioReportRecord.RaiseFlop), false));
            gridView.Columns.Add(AddPercentile("Delayed C-Bet%", nameof(MRatioReportRecord.DidDelayedTurnCBet), false));
            gridView.Columns.Add(AddPercentile("Raise Turn%", nameof(MRatioReportRecord.RaiseTurn), false));
            gridView.Columns.Add(AddPercentile("Seen Turn%", nameof(MRatioReportRecord.TurnSeen), false));
            gridView.Columns.Add(AddPercentile("Turn AGG%", nameof(MRatioReportRecord.TurnAgg), false));
            gridView.Columns.Add(AddPercentile("Turn Check-Raise%", nameof(MRatioReportRecord.TurnCheckRaise), false));
            gridView.Columns.Add(Add("Check River On BX Line%", nameof(MRatioReportRecord.CheckRiverOnBXLine), false));
            gridView.Columns.Add(AddPercentile("Raise River%", nameof(MRatioReportRecord.RaiseRiver), false));
            gridView.Columns.Add(AddPercentile("River AGG%", nameof(MRatioReportRecord.RiverAgg), false));
            gridView.Columns.Add(AddPercentile("Seen River%", nameof(MRatioReportRecord.RiverSeen), false));
            gridView.Columns.Add(AddPercentile("Limp Call%", nameof(MRatioReportRecord.DidLimpCall), false));
            gridView.Columns.Add(AddPercentile("Limp Fold%", nameof(MRatioReportRecord.DidLimpFold), false));
            gridView.Columns.Add(AddPercentile("Limp Reraise%", nameof(MRatioReportRecord.DidLimpReraise), false));
            gridView.Columns.Add(AddPercentile("Limp%", nameof(MRatioReportRecord.DidLimp), false));
            gridView.Columns.Add(AddPercentile("Donk Bet%", nameof(MRatioReportRecord.DonkBet), false));
            gridView.Columns.Add(AddPercentile("Raise Frequency Factor%", nameof(MRatioReportRecord.RaiseFrequencyFactor), false));
            gridView.Columns.Add(AddPercentile("True Aggression% (TAP)", nameof(MRatioReportRecord.TrueAggression), false));
        }

        private GridViewDataColumn GetMRatioColumn(string name, string member, GridViewLength width)
        {
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));

            var bindingText = new Binding(member);
            bindingText.Converter = new MRatioToTextConverter();
            fef.SetBinding(TextBlock.TextProperty, bindingText);
            fef.SetValue(TextBlock.WidthProperty, 40.0);
            fef.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);

            FrameworkElementFactory rect = new FrameworkElementFactory(typeof(Rectangle));
            rect.SetValue(Rectangle.WidthProperty, 20.0);
            var backgroundColorBinding = new Binding(member);
            backgroundColorBinding.Converter = new MRatioToColorConverter();
            rect.SetBinding(Rectangle.FillProperty, backgroundColorBinding);

            FrameworkElementFactory sp = new FrameworkElementFactory(typeof(StackPanel));
            sp.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            sp.AppendChild(fef);
            sp.AppendChild(rect);

            DataTemplate template = new DataTemplate { VisualTree = sp };
            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = name,
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
            };

            return column;
        }
    }
}
