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
using DriveHUD.Common.Log;
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

            var bindingText = new Binding(member)
            {
                StringFormat = "{0:n1}"
            };

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

        public static double GetColumnWidth(Telerik.Windows.Controls.GridViewColumn column)
        {
            var columnHeader = column.Header as string;

            if (string.IsNullOrEmpty(columnHeader))
            {
                var dataColumn = column as GridViewDataColumn;

                if (dataColumn == null)
                {
                    LogProvider.Log.Warn("Column header is null. Column type isn't data column type");
                }
                else
                {
                    LogProvider.Log.Warn($"Column header is null. Path: {dataColumn.DataMemberBinding?.Path?.Path}");
                }
            }

            return GetColumnHeaderWidth(columnHeader);
        }

        public static double GetColumnHeaderWidth(string header)
        {
            double minWidth = TextMeasurer.MesureString(header);

            if (string.IsNullOrEmpty(header) || header.Length <= 2)
            {
                minWidth += 40;
            }
            else if (header.Length <= 5)
            {
                minWidth += 20;
            }
            else if (header.Length > 15)
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
        /// <param name="columnsToShow">Collection of properties that should be shown from defaults list</param>
        protected virtual void AddDefaultStats(RadGridView gridView, params string[] columnsToShow)
        {
            Tuple<string, string, ColumnType>[] columns;

            if (columnsToShow != null && columnsToShow.Length > 0)
            {
                var defaultColumnsToShow = (from columnToShow in columnsToShow
                                            join defaultColumn in defaultColumns on columnToShow equals defaultColumn.Item2
                                            select defaultColumn).ToArray();

                columns = defaultColumnsToShow.Concat(defaultColumns.Except(defaultColumnsToShow)).ToArray();
            }
            else
            {
                columns = defaultColumns;
            }

            foreach (var columnTuple in columns)
            {
                var showColumn = columnsToShow?.Contains(columnTuple.Item2) ?? false;

                switch (columnTuple.Item3)
                {
                    case ColumnType.Regular:
                        gridView.Columns.Add(Add(columnTuple.Item1, columnTuple.Item2, showColumn));
                        break;
                    case ColumnType.Percentile:
                        gridView.Columns.Add(AddPercentile(columnTuple.Item1, columnTuple.Item2, showColumn));
                        break;
                    case ColumnType.Financial:
                        gridView.Columns.Add(AddFinancial(columnTuple.Item1, columnTuple.Item2, showColumn));
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
            // 3-bet based     
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet", nameof(Indicators.ThreeBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3BetCall", nameof(Indicators.ThreeBetCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_MP", nameof(Indicators.ThreeBet_MP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_CO", nameof(Indicators.ThreeBet_CO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_BTN", nameof(Indicators.ThreeBet_BN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_SB", nameof(Indicators.ThreeBet_SB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_BB", nameof(Indicators.ThreeBet_BB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_IP", nameof(Indicators.ThreeBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3Bet_OOP", nameof(Indicators.ThreeBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_3BetVsSteal", nameof(Indicators.ThreeBetVsSteal), ColumnType.Percentile),
            // 4-bet based
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet", nameof(Indicators.FourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_EP", nameof(Indicators.FourBetInEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_MP", nameof(Indicators.FourBetInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_CO", nameof(Indicators.FourBetInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_BTN", nameof(Indicators.FourBetInBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_SB", nameof(Indicators.FourBetInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_BB", nameof(Indicators.FourBetInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4BetRange", nameof(Indicators.FourBetRange), ColumnType.Percentile),
            // 5-bet based
            new Tuple<string, string, ColumnType>("Reports_Column_5Bet", nameof(Indicators.FiveBet), ColumnType.Percentile),
            // A
            new Tuple<string, string, ColumnType>("Reports_Column_AF", nameof(Indicators.Agg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_AggPercent", nameof(Indicators.AggPr), ColumnType.Percentile),
            // B
            new Tuple<string, string, ColumnType>("Reports_Column_BetFlopCalled3BetPreflopIp", nameof(Indicators.BetFlopCalled3BetPreflopIp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BetFoldFlopPfrRaiser", nameof(Indicators.BetFoldFlopPfrRaiser), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BetRiverOnBXLine", nameof(Indicators.BetRiverOnBXLine), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BetWhenCheckedTo", nameof(Indicators.BetWhenCheckedTo), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BlindsFoldToSteal", nameof(Indicators.BlindsFoldSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BlindsReRaiseSteal", nameof(Indicators.BlindsReraiseSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNDefendCORaise", nameof(Indicators.BTNDefendCORaise), ColumnType.Percentile),            
            // C
            new Tuple<string, string, ColumnType>("Reports_Column_CallFlopCBetIP", nameof(Indicators.CallFlopCBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CallFlopCBetOOP", nameof(Indicators.CallFlopCBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CallRiverRaise", nameof(Indicators.CallRiverRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet", nameof(Indicators.FlopCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_IP", nameof(Indicators.CBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_OOP", nameof(Indicators.CBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_MonotonePot", nameof(Indicators.FlopCBetMonotone), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_MWPot", nameof(Indicators.FlopCBetMW), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet_RagFlop", nameof(Indicators.FlopCBetRag), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBetVs1Opp", nameof(Indicators.FlopCBetVsOneOpp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBetVs2Opp", nameof(Indicators.FlopCBetVsTwoOpp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBetIn3BetPot", nameof(Indicators.FlopCBetInThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CBet4BetPot", nameof(Indicators.FlopCBetInFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledTurnCheckRaise", nameof(Indicators.CalledTurnCheckRaise), ColumnType.Percentile),

            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndXCOnTurnOOP", nameof(Indicators.CheckFlopAsPFRAndXCOnTurnOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndXFOnTurnOOP", nameof(Indicators.CheckFlopAsPFRAndXFOnTurnOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndCallOnTurn", nameof(Indicators.CheckFlopAsPFRAndCallOnTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldOnTurn", nameof(Indicators.CheckFlopAsPFRAndFoldOnTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndRaiseOnTurn", nameof(Indicators.CheckFlopAsPFRAndRaiseOnTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRaise", nameof(Indicators.CheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRaisedFlopCBet", nameof(Indicators.CheckRaisedFlopCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledCheckRaiseVsFlopCBet", nameof(Indicators.CalledCheckRaiseVsFlopCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCheckRaiseVsFlopCBet", nameof(Indicators.FoldedCheckRaiseVsFlopCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRiverAfterBBLine", nameof(Indicators.CheckRiverAfterBBLine), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRiverOnBXLine", nameof(Indicators.CheckRiverOnBXLine), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall", nameof(Indicators.ColdCall), ColumnType.Percentile),
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
            // D
            new Tuple<string, string, ColumnType>("Reports_Column_DelayedCBet", nameof(Indicators.DidDelayedTurnCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_DelayedTurnCBetIP", nameof(Indicators.DelayedTurnCBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_DelayedTurnCBetOOP", nameof(Indicators.DelayedTurnCBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_DelayedTurnCBetIn3BetPot", nameof(Indicators.DelayedTurnCBetIn3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_DonkBet", nameof(Indicators.DonkBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_DoubleBarrel", nameof(Indicators.TurnCBet), ColumnType.Percentile),
            // E
            new Tuple<string, string, ColumnType>("Reports_Column_EVDiff", nameof(Indicators.EVDiff), ColumnType.Financial),            
            // F
            new Tuple<string, string, ColumnType>("Reports_Column_FloatFlop", nameof(Indicators.FloatFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopCheckRaise", nameof(Indicators.FlopCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopCheckBehind", nameof(Indicators.FlopCheckBehind), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopAgg", nameof(Indicators.FlopAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeOneHalfOrLess", nameof(Indicators.FlopBetSizeOneHalfOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeOneQuarterOrLess", nameof(Indicators.FlopBetSizeOneQuarterOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeTwoThirdsOrLess", nameof(Indicators.FlopBetSizeTwoThirdsOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeThreeQuartersOrLess", nameof(Indicators.FlopBetSizeThreeQuartersOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeOneOrLess", nameof(Indicators.FlopBetSizeOneOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeMoreThanOne", nameof(Indicators.FlopBetSizeMoreThanOne), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3Bet", nameof(Indicators.FoldToThreeBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo4Bet", nameof(Indicators.FoldToFourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo5Bet", nameof(Indicators.FoldToFiveBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldCBet", nameof(Indicators.FoldCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDoubleBarrel", nameof(Indicators.FoldToTurnCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDoubleBarrelIn3BetPot", nameof(Indicators.FoldToTurnCBetIn3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCBet3BetPot", nameof(Indicators.FoldFlopCBetFromThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCBet4BetPot", nameof(Indicators.FoldFlopCBetFromFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDonkBet", nameof(Indicators.FoldToDonkBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopCheckRaise", nameof(Indicators.FoldToFlopCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopCBetIP", nameof(Indicators.FoldToFlopCBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopCBetOOP", nameof(Indicators.FoldToFlopCBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopRaise", nameof(Indicators.FoldToFlopRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToRiverCBet", nameof(Indicators.FoldToRiverCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToSqueez", nameof(Indicators.FoldToSqueez), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTurnCheckRaise", nameof(Indicators.FoldToTurnCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTurnRaise", nameof(Indicators.FoldToTurnRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToRiverCheckRaise", nameof(Indicators.FoldToRiverCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTurn", nameof(Indicators.FoldTurn), ColumnType.Percentile),
            // L
            new Tuple<string, string, ColumnType>("Reports_Column_Limp", nameof(Indicators.DidLimp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_EP", nameof(Indicators.LimpEp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_MP", nameof(Indicators.LimpMp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_CO", nameof(Indicators.LimpCo), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_BTN", nameof(Indicators.LimpBtn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_SB", nameof(Indicators.LimpSb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpCall", nameof(Indicators.DidLimpCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpFold", nameof(Indicators.DidLimpFold), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpReRaise", nameof(Indicators.DidLimpReraise), ColumnType.Percentile),                      
            // P    
            new Tuple<string, string, ColumnType>("Reports_Column_PFR", nameof(Indicators.PFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_BB", nameof(Indicators.PFRInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_BTN", nameof(Indicators.PFRInBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_CO", nameof(Indicators.PFRInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_EP", nameof(Indicators.PFRInEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_MP", nameof(Indicators.PFRInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_SB", nameof(Indicators.PFRInSB), ColumnType.Percentile),
            // R
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseCBet", nameof(Indicators.RaiseCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverBetSizeMoreThanOne", nameof(Indicators.RiverBetSizeMoreThanOne), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseFlop", nameof(Indicators.RaiseFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseTurn", nameof(Indicators.RaiseTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseRiver", nameof(Indicators.RaiseRiver), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseFreqFactor", nameof(Indicators.RaiseFrequencyFactor), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverAgg", nameof(Indicators.RiverAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverBet", nameof(Indicators.RiverBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverCallEffeciency", nameof(Indicators.RiverCallEffeciency), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverCheckCall", nameof(Indicators.RiverCheckCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverCheckFold", nameof(Indicators.RiverCheckFold), ColumnType.Percentile),
            // S
            new Tuple<string, string, ColumnType>("Reports_Column_Squeeze", nameof(Indicators.Squeeze), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Steal", nameof(Indicators.Steal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SeenTurn", nameof(Indicators.TurnSeen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SeenRiver", nameof(Indicators.RiverSeen), ColumnType.Percentile),
            // T
            new Tuple<string, string, ColumnType>("Reports_Column_TrueAgg", nameof(Indicators.TrueAggression), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnAF", nameof(Indicators.TurnAF), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnAgg", nameof(Indicators.TurnAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneHalfOrLess", nameof(Indicators.TurnBetSizeOneHalfOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneQuarterOrLess", nameof(Indicators.TurnBetSizeOneQuarterOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneThirdOrLess", nameof(Indicators.TurnBetSizeOneThirdOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeTwoThirdsOrLess", nameof(Indicators.TurnBetSizeTwoThirdsOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeThreeQuartersOrLess", nameof(Indicators.TurnBetSizeThreeQuartersOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneOrLess", nameof(Indicators.TurnBetSizeOneOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeMoreThanOne", nameof(Indicators.TurnBetSizeMoreThanOne), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnCheckRaise", nameof(Indicators.TurnCheckRaise), ColumnType.Percentile),
            // U
            new Tuple<string, string, ColumnType>("Reports_Column_UO_PFR_BB", nameof(Indicators.UO_PFR_BB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_UO_PFR_BTN", nameof(Indicators.UO_PFR_BN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_UO_PFR_CO", nameof(Indicators.UO_PFR_CO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_UO_PFR_EP", nameof(Indicators.UO_PFR_EP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_UO_PFR_MP", nameof(Indicators.UO_PFR_MP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_UO_PFR_SB", nameof(Indicators.UO_PFR_SB), ColumnType.Percentile),
            // V
            new Tuple<string, string, ColumnType>("Reports_Column_VPIP", nameof(Indicators.VPIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_VPIP_BB", nameof(Indicators.VPIP_BB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_VPIP_BTN", nameof(Indicators.VPIP_BN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_VPIP_CO", nameof(Indicators.VPIP_CO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_VPIP_EP", nameof(Indicators.VPIP_EP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_VPIP_MP", nameof(Indicators.VPIP_MP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_VPIP_SB", nameof(Indicators.VPIP_SB), ColumnType.Percentile),
            // W
            new Tuple<string, string, ColumnType>("Reports_Column_WTSD", nameof(Indicators.WTSD), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAfterCalling3Bet", nameof(Indicators.WTSDAfterCalling3Bet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAfterCallingPfr", nameof(Indicators.WTSDAfterCallingPfr), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAfterNotCBettingFlopAsPfr", nameof(Indicators.WTSDAfterNotCBettingFlopAsPfr), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAfterSeeingTurn", nameof(Indicators.WTSDAfterSeeingTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAsPF3Bettor", nameof(Indicators.WTSDAsPF3Bettor), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WSWSF", nameof(Indicators.WSWSF), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WSSD", nameof(Indicators.WSSD), ColumnType.Percentile),
        };

        #endregion
    }
}
