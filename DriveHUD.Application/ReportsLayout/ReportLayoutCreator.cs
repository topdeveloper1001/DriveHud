//-----------------------------------------------------------------------
// <copyright file="ReportLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ValueConverters;
using DriveHUD.Common.Wpf.Helpers;
using Model.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public abstract class ReportLayoutCreator
    {
        public virtual void Create(RadGridView gridView)
        {
        }

        protected virtual GridViewDataColumn Add(string name, string member)
        {
            return Add(name, member, new GridViewLength(0));
        }

        protected virtual GridViewDataColumn Add(string name, string member, string stringFormat)
        {
            return Add(name, member, new GridViewLength(0), stringFormat: stringFormat);
        }

        protected virtual GridViewDataColumn Add(string name, string member, bool isVisible, IValueConverter converter = null, string stringFormat = null)
        {
            return Add(name, member, new GridViewLength(0), isVisible, converter, stringFormat);
        }

        protected virtual GridViewDataColumn Add(string name, string member, GridViewLength width, IValueConverter converter = null, string stringFormat = null)
        {
            return Add(name, member, width, true, converter, stringFormat);
        }

        protected virtual GridViewDataColumn Add(string name, string member, GridViewLength width, bool isVisible, IValueConverter converter, string stringFormat)
        {
            var binding = new Binding(member);

            if (stringFormat != null)
            {
                binding.StringFormat = stringFormat;
            }

            if (converter != null)
            {
                binding.Converter = converter;
            }

            var column = new GridViewDataColumn
            {
                Header = name,
                DataMemberBinding = binding,
                Width = width.Value == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                UniqueName = member,
                IsVisible = isVisible
            };

            return column;
        }

        protected virtual GridViewDataColumn AddFinancial(string name, string member)
        {
            return AddFinancial(name, member, new GridViewLength(0), true);
        }

        protected virtual GridViewDataColumn AddFinancial(string name, string member, bool isVisible)
        {
            return AddFinancial(name, member, new GridViewLength(0), isVisible);
        }

        protected virtual GridViewDataColumn AddFinancial(string name, string member, GridViewLength width, bool isVisible)
        {
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
            var bindingText = new Binding(member);
            bindingText.StringFormat = "{0:c2}";
            fef.SetBinding(TextBlock.TextProperty, bindingText);

            var bindingForeground = new Binding(member);
            bindingForeground.Converter = new ValueToColorConverter();
            fef.SetBinding(TextBlock.ForegroundProperty, bindingForeground);

            DataTemplate template = new DataTemplate();
            template.VisualTree = fef;
            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = name,
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
                IsVisible = isVisible
            };

            return column;
        }

        protected virtual GridViewDataColumn AddPercentile(string name, string member)
        {
            return AddPercentile(name, member, new GridViewLength(0), true);
        }

        protected virtual GridViewDataColumn AddPercentile(string name, string member, bool isVisible)
        {
            return AddPercentile(name, member, new GridViewLength(0), isVisible);
        }

        protected virtual GridViewDataColumn AddPercentile(string name, string member, GridViewLength width, bool isVisible)
        {
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
            var bindingText = new Binding(member) { StringFormat = "{0:n1}" };
            fef.SetBinding(TextBlock.TextProperty, bindingText);

            DataTemplate template = new DataTemplate { VisualTree = fef };
            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = name,
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
                IsVisible = isVisible
            };

            return column;
        }

        public static double GetColumnWidth(string text)
        {
            double minWidth = TextMeasurer.MesureString(text);
            if (text.Length <= 2)
            {
                minWidth += 40;
            }
            else if (text.Length <= 5)
            {
                minWidth += 20;
            }
            else if (text.Length > 15)
            {
                minWidth -= 20;
            }
            return minWidth;
        }

        /// <summary>
        /// Populates grid  with default columns (hidden by default)
        /// </summary>
        /// <param name="gridView">Grid to populate</param>
        protected virtual void AddDefaultStats(RadGridView gridView)
        {
            AddDefaultStats(gridView, new string[] { });
        }

        /// <summary>
        /// Populates grid with default columns (hidden by default)
        /// </summary>
        /// <param name="gridView">Grid to populate</param>
        /// <param name="columnsToSkip">Collection of properties that should be skipped from defaults list</param>
        protected virtual void AddDefaultStats(RadGridView gridView, params string[] columnsToSkip)
        {
            foreach (var columnTuple in defaultColumns)
            {
                if (columnsToSkip?.Contains(columnTuple.Item2) ?? false)
                {
                    continue;
                }

                switch (columnTuple.Item3)
                {
                    case ColumnType.Regular:
                        gridView.Columns.Add(Add(columnTuple.Item1, columnTuple.Item2, false));
                        break;
                    case ColumnType.Percentile:
                        gridView.Columns.Add(AddPercentile(columnTuple.Item1, columnTuple.Item2, false));
                        break;
                    case ColumnType.Financial:
                        gridView.Columns.Add(AddFinancial(columnTuple.Item1, columnTuple.Item2, false));
                        break;
                }
            }
        }

        #region Default Columns

        private enum ColumnType { Regular, Percentile, Financial };

        /// <summary>
        /// Item1 - Column name, item2 - property name, item3 - isPercentile
        /// </summary>
        private Tuple<string, string, ColumnType>[] defaultColumns = new Tuple<string, string, ColumnType>[]
        {
            new Tuple<string, string, ColumnType>("4-Bet%", nameof(Indicators.FourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Check-Raise%", nameof(Indicators.CheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call%", nameof(Indicators.ColdCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Flop AGG%", nameof(Indicators.FlopAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Fold to 3-Bet%", nameof(Indicators.FoldToThreeBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Fold to 4-Bet%", nameof(Indicators.FoldToFourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("River AGG%", nameof(Indicators.RiverAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Squeeze%", nameof(Indicators.Squeeze), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Turn AGG%", nameof(Indicators.TurnAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet MP%", nameof(Indicators.ThreeBet_MP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet CO%", nameof(Indicators.ThreeBet_CO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet BTN%", nameof(Indicators.ThreeBet_BN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet SB%", nameof(Indicators.ThreeBet_SB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet BB%", nameof(Indicators.ThreeBet_BB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet IP%", nameof(Indicators.ThreeBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet OPP%", nameof(Indicators.ThreeBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("3-Bet vs. Steal", nameof(Indicators.ThreeBetVsSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet in 3-Bet Pot%", nameof(Indicators.FlopCBetInThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Fold to C-Bet 3-Bet Pot%", nameof(Indicators.FoldFlopCBetFromThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("4-Bet EP%", nameof(Indicators.FourBetInEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("4-Bet MP%", nameof(Indicators.FourBetInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("4-Bet CO%", nameof(Indicators.FourBetInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("4-Bet BTN%", nameof(Indicators.FourBetInBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("4-Bet SB%", nameof(Indicators.FourBetInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("4-Bet BB%", nameof(Indicators.FourBetInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet 4-Bet Pot%", nameof(Indicators.FlopCBetInFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Fold to C-Bet 4-Bet Pot%", nameof(Indicators.FoldFlopCBetFromFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet IP%", nameof(Indicators.CBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet OOP%", nameof(Indicators.CBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet Monotone Pot%", nameof(Indicators.FlopCBetMonotone), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet MW Pot%", nameof(Indicators.FlopCBetMW), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet Rag Flop%", nameof(Indicators.FlopCBetRag), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet vs 1 Opp%", nameof(Indicators.FlopCBetVsOneOpp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("C-Bet vs 2 Opp%", nameof(Indicators.FlopCBetVsTwoOpp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Fold to C-Bet 3-Bet Pot%", nameof(Indicators.FoldFlopCBetFromThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Fold to C-Bet 4-Bet Pot%", nameof(Indicators.FoldFlopCBetFromFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Raise C-Bet%", nameof(Indicators.RaiseCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call EP%", nameof(Indicators.ColdCall_EP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call MP%", nameof(Indicators.ColdCall_MP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call CO%", nameof(Indicators.ColdCall_CO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call BTN%", nameof(Indicators.ColdCall_BN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call SB%", nameof(Indicators.ColdCall_SB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call BB%", nameof(Indicators.ColdCall_BB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call 3-Bet%", nameof(Indicators.ColdCallThreeBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call 4-Bet%", nameof(Indicators.ColdCallFourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call vs BTN open%", nameof(Indicators.ColdCallVsBtnOpen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call vs SB open%", nameof(Indicators.ColdCallVsSbOpen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Cold Call vs CO open%", nameof(Indicators.ColdCallVsCoOpen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Float Flop%", nameof(Indicators.FloatFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Flop Check-Raise%", nameof(Indicators.FlopCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Raise Flop%", nameof(Indicators.RaiseFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Delayed C-Bet%", nameof(Indicators.DidDelayedTurnCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Raise Turn%", nameof(Indicators.RaiseTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Seen Turn%", nameof(Indicators.TurnSeen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Turn AGG%", nameof(Indicators.TurnAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Turn Check-Raise%", nameof(Indicators.TurnCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Check River On BX Line%", nameof(Indicators.CheckRiverOnBXLine), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Raise River%", nameof(Indicators.RaiseRiver), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("River AGG%", nameof(Indicators.RiverAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Seen River%", nameof(Indicators.RiverSeen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp Call%", nameof(Indicators.DidLimpCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp Fold%", nameof(Indicators.DidLimpFold), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp Reraise%", nameof(Indicators.DidLimpReraise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp%", nameof(Indicators.DidLimp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp EP%", nameof(Indicators.LimpEp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp MP%", nameof(Indicators.LimpMp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp CO%", nameof(Indicators.LimpCo), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp BTN%", nameof(Indicators.LimpBtn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Limp SB%", nameof(Indicators.LimpSb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Donk Bet%", nameof(Indicators.DonkBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Raise Frequency Factor%", nameof(Indicators.RaiseFrequencyFactor), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("True Aggression% (TAP)", nameof(Indicators.TrueAggression), ColumnType.Percentile)
        };

        #endregion
    }
}