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
using DriveHUD.Common.Resources;
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

        protected virtual GridViewDataColumn Add(string resourceKey, string member)
        {
            return Add(resourceKey, member, new GridViewLength(0));
        }

        protected virtual GridViewDataColumn Add(string resourceKey, string member, string stringFormat)
        {
            return Add(resourceKey, member, new GridViewLength(0), stringFormat: stringFormat);
        }

        protected virtual GridViewDataColumn Add(string resourceKey, string member, bool isVisible, IValueConverter converter = null, string stringFormat = null)
        {
            return Add(resourceKey, member, new GridViewLength(0), isVisible, converter, stringFormat);
        }

        protected virtual GridViewDataColumn Add(string resourceKey, string member, GridViewLength width, IValueConverter converter = null, string stringFormat = null)
        {
            return Add(resourceKey, member, width, true, converter, stringFormat);
        }

        protected virtual GridViewDataColumn Add(string resourceKey, string member, GridViewLength width, bool isVisible, IValueConverter converter, string stringFormat)
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
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
                DataMemberBinding = binding,
                Width = width.Value == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                UniqueName = member,
                IsVisible = isVisible
            };

            return column;
        }

        protected virtual GridViewDataColumn AddFinancial(string resourceKey, string member)
        {
            return AddFinancial(resourceKey, member, new GridViewLength(0), true);
        }

        protected virtual GridViewDataColumn AddFinancial(string resourceKey, string member, bool isVisible)
        {
            return AddFinancial(resourceKey, member, new GridViewLength(0), isVisible);
        }

        protected virtual GridViewDataColumn AddFinancial(string resourceKey, string member, GridViewLength width, bool isVisible)
        {
            var fef = new FrameworkElementFactory(typeof(TextBlock));

            var bindingText = new Binding(member);
            bindingText.StringFormat = "{0:c2}";

            fef.SetBinding(TextBlock.TextProperty, bindingText);

            var bindingForeground = new Binding(member);
            bindingForeground.Converter = new ValueToColorConverter();
            fef.SetBinding(TextBlock.ForegroundProperty, bindingForeground);

            var template = new DataTemplate();
            template.VisualTree = fef;
            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
                IsVisible = isVisible
            };

            return column;
        }

        protected virtual GridViewDataColumn AddPercentile(string resourceKey, string member)
        {
            return AddPercentile(resourceKey, member, new GridViewLength(0), true);
        }

        protected virtual GridViewDataColumn AddPercentile(string resourceKey, string member, bool isVisible)
        {
            return AddPercentile(resourceKey, member, new GridViewLength(0), isVisible);
        }

        protected virtual GridViewDataColumn AddPercentile(string resourceKey, string member, GridViewLength width, bool isVisible)
        {
            var fef = new FrameworkElementFactory(typeof(TextBlock));

            var bindingText = new Binding(member) { StringFormat = "{0:n1}" };
            fef.SetBinding(TextBlock.TextProperty, bindingText);

            DataTemplate template = new DataTemplate { VisualTree = fef };
            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
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
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet", nameof(Indicators.FourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRaise", nameof(Indicators.CheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall", nameof(Indicators.ColdCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopAgg", nameof(Indicators.FlopAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3Bet", nameof(Indicators.FoldToThreeBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo4Bet", nameof(Indicators.FoldToFourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverAgg", nameof(Indicators.RiverAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Squeeze", nameof(Indicators.Squeeze), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnAgg", nameof(Indicators.TurnAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_MP", nameof(Indicators.ThreeBet_MP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_CO", nameof(Indicators.ThreeBet_CO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_BTN", nameof(Indicators.ThreeBet_BN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_SB", nameof(Indicators.ThreeBet_SB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_BB", nameof(Indicators.ThreeBet_BB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_IP", nameof(Indicators.ThreeBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_OOP", nameof(Indicators.ThreeBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3BetVsSteal", nameof(Indicators.ThreeBetVsSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBetIn3BetPot", nameof(Indicators.FlopCBetInThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCBet3BetPot", nameof(Indicators.FoldFlopCBetFromThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_EP", nameof(Indicators.FourBetInEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_MP", nameof(Indicators.FourBetInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_CO", nameof(Indicators.FourBetInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_BTN", nameof(Indicators.FourBetInBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_SB", nameof(Indicators.FourBetInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_BB", nameof(Indicators.FourBetInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet4BetPot", nameof(Indicators.FlopCBetInFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCBet4BetPot", nameof(Indicators.FoldFlopCBetFromFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_IP", nameof(Indicators.CBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_OOP", nameof(Indicators.CBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_MonotonePot", nameof(Indicators.FlopCBetMonotone), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_MWPot", nameof(Indicators.FlopCBetMW), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_RagFlop", nameof(Indicators.FlopCBetRag), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBetVs1Opp", nameof(Indicators.FlopCBetVsOneOpp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBetVs2Opp", nameof(Indicators.FlopCBetVsTwoOpp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseCBet", nameof(Indicators.RaiseCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_EP", nameof(Indicators.ColdCall_EP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_MP", nameof(Indicators.ColdCall_MP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_CO", nameof(Indicators.ColdCall_CO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_BTN", nameof(Indicators.ColdCall_BN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_SB", nameof(Indicators.ColdCall_SB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_BB", nameof(Indicators.ColdCall_BB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_3Bet", nameof(Indicators.ColdCallThreeBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_4Bet", nameof(Indicators.ColdCallFourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCallVsBTNOpen", nameof(Indicators.ColdCallVsBtnOpen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCallVsSBOpen", nameof(Indicators.ColdCallVsSbOpen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCallVsCOOpen", nameof(Indicators.ColdCallVsCoOpen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FloatFlop", nameof(Indicators.FloatFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopCheckRaise", nameof(Indicators.FlopCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseFlop%", nameof(Indicators.RaiseFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_DelayedCBet", nameof(Indicators.DidDelayedTurnCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseTurn", nameof(Indicators.RaiseTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SeenTurn", nameof(Indicators.TurnSeen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnCheckRaise", nameof(Indicators.TurnCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRiverOnBXLine", nameof(Indicators.CheckRiverOnBXLine), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseRiver", nameof(Indicators.RaiseRiver), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SeenRiver", nameof(Indicators.RiverSeen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpCall", nameof(Indicators.DidLimpCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpFold", nameof(Indicators.DidLimpFold), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpReRaise", nameof(Indicators.DidLimpReraise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp", nameof(Indicators.DidLimp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_EP", nameof(Indicators.LimpEp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_MP", nameof(Indicators.LimpMp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_CO", nameof(Indicators.LimpCo), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_BTN", nameof(Indicators.LimpBtn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_SB", nameof(Indicators.LimpSb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_DonkBet", nameof(Indicators.DonkBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_RaiseFreqFactor", nameof(Indicators.RaiseFrequencyFactor), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_TrueAgg", nameof(Indicators.TrueAggression), ColumnType.Percentile)
        };

        #endregion
    }
}