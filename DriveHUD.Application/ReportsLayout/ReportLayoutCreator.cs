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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
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

            if (stringFormat != null)
            {
                column.DataFormatString = stringFormat;
            }

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

            var bindingText = new Binding(member)
            {
                StringFormat = "{0:c2}"
            };

            fef.SetBinding(TextBlock.TextProperty, bindingText);

            var bindingForeground = new Binding(member)
            {
                Converter = new ValueToColorConverter()
            };

            fef.SetBinding(TextBlock.ForegroundProperty, bindingForeground);

            var template = new DataTemplate
            {
                VisualTree = fef
            };

            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                DataFormatString = "{0:c2}",
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
            var column = new GridViewDataColumn
            {
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                DataFormatString = "{0:n1}",
                UniqueName = member,
                IsVisible = isVisible
            };

            return column;
        }

        public virtual GridViewDataColumn AddPlayerType(string resourceKey, string member)
        {
            var stringReader = new StringReader(@"<DataTemplate 
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                 xmlns:dh=""http://www.acepokersolutions.com/winfx/2015/xaml/presentation"">
                    <StackPanel Orientation=""Horizontal"">
                        <Image 
                            Source=""{Binding PlayerType.Image, Converter={dh:StringToImageSourceConverter}}"" 
                            Visibility=""{Binding PlayerType.Image, Converter={dh:NullOrEmptyToVisibilityConverter}}""
                            Width=""24""
                            Height=""24""
                            VerticalAlignment=""Center""
                            HorizontalAlignment=""Center""
                            />
                        <TextBlock 
                            Text=""{Binding PlayerType.Name}"" 
                            Margin=""5,0,0,0""
                            VerticalAlignment=""Center"" />
                    </StackPanel>
                </DataTemplate>");

            var xmlReader = XmlReader.Create(stringReader);

            var dataTemplate = (DataTemplate)XamlReader.Load(xmlReader);
            dataTemplate.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
                DataMemberBinding = new Binding(member),
                Width = GetColumnHeaderWidth("Standard Reg") + 40,
                CellTemplate = dataTemplate,
                UniqueName = member
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
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetMPvsEP", nameof(Indicators.ThreeBetMPvsEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetCOvsEP", nameof(Indicators.ThreeBetCOvsEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetCOvsMP", nameof(Indicators.ThreeBetCOvsMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBTNvsEP", nameof(Indicators.ThreeBetBTNvsEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBTNvsMP", nameof(Indicators.ThreeBetBTNvsMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBTNvsCO", nameof(Indicators.ThreeBetBTNvsCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetSBvsEP", nameof(Indicators.ThreeBetSBvsEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetSBvsMP", nameof(Indicators.ThreeBetSBvsMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetSBvsCO", nameof(Indicators.ThreeBetSBvsCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetSBvsBTN", nameof(Indicators.ThreeBetSBvsBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBBvsEP", nameof(Indicators.ThreeBetBBvsEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBBvsMP", nameof(Indicators.ThreeBetBBvsMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBBvsCO", nameof(Indicators.ThreeBetBBvsCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBBvsBTN", nameof(Indicators.ThreeBetBBvsBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ThreeBetBBvsSB", nameof(Indicators.ThreeBetBBvsSB), ColumnType.Percentile),

            // 4-bet based
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet", nameof(Indicators.FourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_EP", nameof(Indicators.FourBetInEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_MP", nameof(Indicators.FourBetInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_CO", nameof(Indicators.FourBetInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_BTN", nameof(Indicators.FourBetInBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_SB", nameof(Indicators.FourBetInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4Bet_BB", nameof(Indicators.FourBetInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4BetRange", nameof(Indicators.FourBetRange), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_4BetVsBlind3Bet", nameof(Indicators.FourBetRange), ColumnType.Percentile),
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
            new Tuple<string, string, ColumnType>("Reports_Column_BetFlopWhenCheckedToSRP", nameof(Indicators.BetFlopWhenCheckedToSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BetTurnWhenCheckedToSRP", nameof(Indicators.BetTurnWhenCheckedToSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BetRiverWhenCheckedToSRP", nameof(Indicators.BetRiverWhenCheckedToSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BlindsFoldToSteal", nameof(Indicators.BlindsFoldSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BlindsReRaiseSteal", nameof(Indicators.BlindsReraiseSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNDefendCORaise", nameof(Indicators.BTNDefendCORaise), ColumnType.Percentile),            
            // C
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetIP", nameof(Indicators.Call4BetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetOOP", nameof(Indicators.Call4BetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetEP", nameof(Indicators.Call4BetEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetMP", nameof(Indicators.Call4BetMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetCO", nameof(Indicators.Call4BetCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetBTN", nameof(Indicators.Call4BetBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetSB", nameof(Indicators.Call4BetSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Call4BetBB", nameof(Indicators.Call4BetBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CallFlopCBetIP", nameof(Indicators.CallFlopCBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CallFlopCBetOOP", nameof(Indicators.CallFlopCBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CallFlopFoldTurn", nameof(Indicators.CallFlopFoldTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CallRiverRaise", nameof(Indicators.CallRiverRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledStealInSB", nameof(Indicators.CalledStealInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledStealInBB", nameof(Indicators.CalledStealInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledBTNStealInSB", nameof(Indicators.CalledBTNStealInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledBTNStealInBB", nameof(Indicators.CalledBTNStealInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledCOStealInSB", nameof(Indicators.CalledCOStealInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledCOStealInBB", nameof(Indicators.CalledCOStealInBB), ColumnType.Percentile),
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
            new Tuple<string, string, ColumnType>("Reports_Column_CBetThenFoldFlopSRP", nameof(Indicators.CBetThenFoldFlopSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CalledTurnCheckRaise", nameof(Indicators.CalledTurnCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndXCOnTurnOOP", nameof(Indicators.CheckFlopAsPFRAndXCOnTurnOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndXFOnTurnOOP", nameof(Indicators.CheckFlopAsPFRAndXFOnTurnOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndCallOnTurn", nameof(Indicators.CheckFlopAsPFRAndCallOnTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldOnTurn", nameof(Indicators.CheckFlopAsPFRAndFoldOnTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndRaiseOnTurn", nameof(Indicators.CheckFlopAsPFRAndRaiseOnTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToTurnBetIPSRP", nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetIPSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToTurnBetOOPSRP", nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetOOPSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToRiverBetIPSRP", nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetIPSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToRiverBetOOPSRP", nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetOOPSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToTurnBetIP3BetPot", nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot", nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToRiverBetIP3BetPot", nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot", nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRaise", nameof(Indicators.CheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_CheckRaiseFlopAsPFR", nameof(Indicators.CheckRaiseFlopAsPFR), ColumnType.Percentile),
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
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall3BetInBB", nameof(Indicators.ColdCall3BetInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall3BetInSB", nameof(Indicators.ColdCall3BetInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall3BetInMP", nameof(Indicators.ColdCall3BetInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall3BetInCO", nameof(Indicators.ColdCall3BetInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall3BetInBTN", nameof(Indicators.ColdCall3BetInBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall_4Bet", nameof(Indicators.ColdCallFourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall4BetInBB", nameof(Indicators.ColdCall4BetInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall4BetInSB", nameof(Indicators.ColdCall4BetInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall4BetInMP", nameof(Indicators.ColdCall4BetInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall4BetInCO", nameof(Indicators.ColdCall4BetInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ColdCall4BetInBTN", nameof(Indicators.ColdCall4BetInBTN), ColumnType.Percentile),
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
            new Tuple<string, string, ColumnType>("Reports_Column_DoubleBarrelSRP", nameof(Indicators.DoubleBarrelSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_DoubleBarrel3BetPot", nameof(Indicators.DoubleBarrel3BetPot), ColumnType.Percentile),
            // E
            new Tuple<string, string, ColumnType>("Reports_Column_EVDiff", nameof(Indicators.EVDiff), ColumnType.Financial),            
            // F
            new Tuple<string, string, ColumnType>("Reports_Column_FloatFlop", nameof(Indicators.FloatFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FloatFlopThenBetTurn", nameof(Indicators.FloatFlopThenBetTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopCheckCall", nameof(Indicators.FlopCheckCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopCheckRaise", nameof(Indicators.FlopCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopCheckBehind", nameof(Indicators.FlopCheckBehind), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopAgg", nameof(Indicators.FlopAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBet", nameof(Indicators.FlopBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeOneHalfOrLess", nameof(Indicators.FlopBetSizeOneHalfOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeOneQuarterOrLess", nameof(Indicators.FlopBetSizeOneQuarterOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeTwoThirdsOrLess", nameof(Indicators.FlopBetSizeTwoThirdsOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeThreeQuartersOrLess", nameof(Indicators.FlopBetSizeThreeQuartersOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeOneOrLess", nameof(Indicators.FlopBetSizeOneOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FlopBetSizeMoreThanOne", nameof(Indicators.FlopBetSizeMoreThanOne), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldBBvsSBSteal", nameof(Indicators.FoldBBvsSBSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldFlop", nameof(Indicators.FoldFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3Bet", nameof(Indicators.FoldToThreeBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetIP", nameof(Indicators.FoldToThreeBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetOOP", nameof(Indicators.FoldToThreeBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo4Bet", nameof(Indicators.FoldToFourBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo5Bet", nameof(Indicators.FoldToFiveBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldCBet", nameof(Indicators.FoldCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDoubleBarrel", nameof(Indicators.FoldToTurnCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDoubleBarrelSRP", nameof(Indicators.FoldToDoubleBarrelSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDoubleBarrelIn3BetPot", nameof(Indicators.FoldToTurnCBetIn3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDoubleBarrel4BetPot", nameof(Indicators.FoldToDoubleBarrel4BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCBetSRP", nameof(Indicators.FoldToCBetSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCBet3BetPot", nameof(Indicators.FoldFlopCBetFromThreeBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCBet4BetPot", nameof(Indicators.FoldFlopCBetFromFourBetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToDonkBet", nameof(Indicators.FoldToDonkBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopCheckRaise", nameof(Indicators.FoldToFlopCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopCBetIP", nameof(Indicators.FoldToFlopCBetIP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopCBetOOP", nameof(Indicators.FoldToFlopCBetOOP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToFlopRaise", nameof(Indicators.FoldToFlopRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToSqueez", nameof(Indicators.FoldToSqueez), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToSqueezeAsColdCaller", nameof(Indicators.FoldToSqueezeAsColdCaller), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToStealInSB", nameof(Indicators.FoldToStealInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToStealInBB", nameof(Indicators.FoldToStealInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToBTNStealInSB", nameof(Indicators.FoldToBTNStealInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToBTNStealInBB", nameof(Indicators.FoldToBTNStealInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCOStealInSB", nameof(Indicators.FoldToCOStealInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToCOStealInBB", nameof(Indicators.FoldToCOStealInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTurn", nameof(Indicators.FoldTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTurnCheckRaise", nameof(Indicators.FoldToTurnCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTurnRaise", nameof(Indicators.FoldToTurnRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTurnProbeIP", nameof(Indicators.FoldToProbeBetTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToRiverCBet", nameof(Indicators.FoldToRiverCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToRiverCheckRaise", nameof(Indicators.FoldToRiverCheckRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToRiverProbeIP", nameof(Indicators.FoldToProbeBetRiver), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToRiverRaise", nameof(Indicators.FoldToRiverRaise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTripleBarrelSRP", nameof(Indicators.FoldToTripleBarrelSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTripleBarrel3BetPot", nameof(Indicators.FoldToTripleBarrel3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldToTripleBarrel4BetPot", nameof(Indicators.FoldToTripleBarrel4BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInEPvs3BetMP", nameof(Indicators.FoldTo3BetInEPvs3BetMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInEPvs3BetCO", nameof(Indicators.FoldTo3BetInEPvs3BetCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInEPvs3BetBTN", nameof(Indicators.FoldTo3BetInEPvs3BetBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInEPvs3BetSB", nameof(Indicators.FoldTo3BetInEPvs3BetSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInEPvs3BetBB", nameof(Indicators.FoldTo3BetInEPvs3BetBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInMPvs3BetCO", nameof(Indicators.FoldTo3BetInMPvs3BetCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInMPvs3BetBTN", nameof(Indicators.FoldTo3BetInMPvs3BetBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInMPvs3BetSB", nameof(Indicators.FoldTo3BetInMPvs3BetSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInMPvs3BetBB", nameof(Indicators.FoldTo3BetInMPvs3BetBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInCOvs3BetBTN", nameof(Indicators.FoldTo3BetInCOvs3BetBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInCOvs3BetSB", nameof(Indicators.FoldTo3BetInCOvs3BetSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInCOvs3BetBB", nameof(Indicators.FoldTo3BetInCOvs3BetBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInBTNvs3BetSB", nameof(Indicators.FoldTo3BetInBTNvs3BetSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_FoldTo3BetInBTNvs3BetBB", nameof(Indicators.FoldTo3BetInBTNvs3BetBB), ColumnType.Percentile),
            // L
            new Tuple<string, string, ColumnType>("Reports_Column_Limp", nameof(Indicators.DidLimp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_EP", nameof(Indicators.LimpEp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_MP", nameof(Indicators.LimpMp), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_CO", nameof(Indicators.LimpCo), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_BTN", nameof(Indicators.LimpBtn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Limp_SB", nameof(Indicators.LimpSb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpCall", nameof(Indicators.DidLimpCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpFold", nameof(Indicators.DidLimpFold), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpEPFoldToPFR", nameof(Indicators.LimpEPFoldToPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpMPFoldToPFR", nameof(Indicators.LimpMPFoldToPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpCOFoldToPFR", nameof(Indicators.LimpCOFoldToPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpBTNFoldToPFR", nameof(Indicators.LimpBTNFoldToPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpSBFoldToPFR", nameof(Indicators.LimpSBFoldToPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpReRaise", nameof(Indicators.DidLimpReraise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_LimpReRaise", nameof(Indicators.LimpedPotFlopStealIP), ColumnType.Percentile),  
            // O
            new Tuple<string, string, ColumnType>("Reports_Column_OpenMinraise", nameof(Indicators.OpenMinraise), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPOpenMinraiseUOPFR", nameof(Indicators.EPOpenMinraiseUOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPOpenMinraiseUOPFR", nameof(Indicators.MPOpenMinraiseUOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COOpenMinraiseUOPFR", nameof(Indicators.COOpenMinraiseUOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNOpenMinraiseUOPFR", nameof(Indicators.BTNOpenMinraiseUOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBOpenMinraiseUOPFR", nameof(Indicators.SBOpenMinraiseUOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BBOpenMinraiseUOPFR", nameof(Indicators.BBOpenMinraiseUOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBOpenShove1to8bbUOPot", nameof(Indicators.SBOpenShove1to8bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBOpenShove9to14bbUOPot", nameof(Indicators.SBOpenShove9to14bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBOpenShove15to25bbUOPot", nameof(Indicators.SBOpenShove15to25bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBOpenShove26to50bbUOPot", nameof(Indicators.SBOpenShove26to50bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBOpenShove51plusbbUOPot", nameof(Indicators.SBOpenShove51plusbbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNOpenShove1to8bbUOPot", nameof(Indicators.BTNOpenShove1to8bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNOpenShove9to14bbUOPot", nameof(Indicators.BTNOpenShove9to14bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNOpenShove15to25bbUOPot", nameof(Indicators.BTNOpenShove15to25bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNOpenShove26to50bbUOPot", nameof(Indicators.BTNOpenShove26to50bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNOpenShove51plusbbUOPot", nameof(Indicators.BTNOpenShove51plusbbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COOpenShove1to8bbUOPot", nameof(Indicators.COOpenShove1to8bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COOpenShove9to14bbUOPot", nameof(Indicators.COOpenShove9to14bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COOpenShove15to25bbUOPot", nameof(Indicators.COOpenShove15to25bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COOpenShove26to50bbUOPot", nameof(Indicators.COOpenShove26to50bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COOpenShove51plusbbUOPot", nameof(Indicators.COOpenShove51plusbbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPOpenShove1to8bbUOPot", nameof(Indicators.MPOpenShove1to8bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPOpenShove9to14bbUOPot", nameof(Indicators.MPOpenShove9to14bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPOpenShove15to25bbUOPot", nameof(Indicators.MPOpenShove15to25bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPOpenShove26to50bbUOPot", nameof(Indicators.MPOpenShove26to50bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPOpenShove51plusbbUOPot", nameof(Indicators.MPOpenShove51plusbbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPOpenShove1to8bbUOPot", nameof(Indicators.EPOpenShove1to8bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPOpenShove9to14bbUOPot", nameof(Indicators.EPOpenShove9to14bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPOpenShove15to25bbUOPot", nameof(Indicators.EPOpenShove15to25bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPOpenShove26to50bbUOPot", nameof(Indicators.EPOpenShove26to50bbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPOpenShove51plusbbUOPot", nameof(Indicators.EPOpenShove51plusbbUOPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_OvercallBTNStealInBB", nameof(Indicators.OvercallBTNStealInBB), ColumnType.Percentile),  
            // P    
            new Tuple<string, string, ColumnType>("Reports_Column_PFR", nameof(Indicators.PFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_BB", nameof(Indicators.PFRInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_BTN", nameof(Indicators.PFRInBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_CO", nameof(Indicators.PFRInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_EP", nameof(Indicators.PFRInEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_MP", nameof(Indicators.PFRInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_PFR_SB", nameof(Indicators.PFRInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ProbeBetTurn", nameof(Indicators.ProbeBetTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_ProbeBetRiver", nameof(Indicators.ProbeBetRiver), ColumnType.Percentile),
            // R
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseCBet", nameof(Indicators.RaiseCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverBetSizeMoreThanOne", nameof(Indicators.RiverBetSizeMoreThanOne), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseFlop", nameof(Indicators.RaiseFlop), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseFlopCBetIn3BetPot", nameof(Indicators.RaiseFlopCBetIn3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseTurn", nameof(Indicators.RaiseTurn), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseRiver", nameof(Indicators.RaiseRiver), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseFreqFactor", nameof(Indicators.RaiseFrequencyFactor), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseLimpers", nameof(Indicators.RaiseLimpers), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseLimpersInMP", nameof(Indicators.RaiseLimpersInMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseLimpersInCO", nameof(Indicators.RaiseLimpersInCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseLimpersInBN", nameof(Indicators.RaiseLimpersInBN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseLimpersInSB", nameof(Indicators.RaiseLimpersInSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RaiseLimpersInBB", nameof(Indicators.RaiseLimpersInBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverAgg", nameof(Indicators.RiverAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverBet", nameof(Indicators.RiverBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverCallEffeciency", nameof(Indicators.RiverCallEffeciency), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverCheckCall", nameof(Indicators.RiverCheckCall), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_RiverCheckFold", nameof(Indicators.RiverCheckFold), ColumnType.Percentile),
            // S
            new Tuple<string, string, ColumnType>("Reports_Column_ShovedFlopAfter4Bet", nameof(Indicators.ShovedFlopAfter4Bet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Squeeze", nameof(Indicators.Squeeze), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeEP", nameof(Indicators.SqueezeEP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeMP", nameof(Indicators.SqueezeMP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeCO", nameof(Indicators.SqueezeCO), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBTN", nameof(Indicators.SqueezeBTN), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeSB", nameof(Indicators.SqueezeSB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBB", nameof(Indicators.SqueezeBB), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBBVsBTNPFR", nameof(Indicators.SqueezeBBVsBTNPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBBVsCOPFR", nameof(Indicators.SqueezeBBVsCOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBBVsMPPFR", nameof(Indicators.SqueezeBBVsMPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBBVsEPPFR", nameof(Indicators.SqueezeBBVsEPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeSBVsCOPFR", nameof(Indicators.SqueezeSBVsCOPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeSBVsMPPFR", nameof(Indicators.SqueezeSBVsMPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeSBVsEPPFR", nameof(Indicators.SqueezeSBVsEPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBTNVsMPPFR", nameof(Indicators.SqueezeBTNVsMPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeBTNVsEPPFR", nameof(Indicators.SqueezeBTNVsEPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeCOVsMPPFR", nameof(Indicators.SqueezeCOVsMPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeCOVsEPPFR", nameof(Indicators.SqueezeCOVsEPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeMPVsEPPFR", nameof(Indicators.SqueezeMPVsEPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SqueezeEPVsEPPFR", nameof(Indicators.SqueezeEPVsEPPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_Steal", nameof(Indicators.Steal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNReStealVsCOSteal", nameof(Indicators.BTNReStealVsCOSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNDefendVsCOSteal", nameof(Indicators.BTNDefendVsCOSteal), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SeenTurn", nameof(Indicators.TurnSeen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SeenRiver", nameof(Indicators.RiverSeen), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBShoveOverLimpers1to8bb", nameof(Indicators.SBShoveOverLimpers1to8bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBShoveOverLimpers9to14bb", nameof(Indicators.SBShoveOverLimpers9to14bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBShoveOverLimpers15to25bb", nameof(Indicators.SBShoveOverLimpers15to25bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBShoveOverLimpers26to50bb", nameof(Indicators.SBShoveOverLimpers26to50bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_SBShoveOverLimpers51plusbb", nameof(Indicators.SBShoveOverLimpers51plusbb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNShoveOverLimpers1to8bb", nameof(Indicators.BTNShoveOverLimpers1to8bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNShoveOverLimpers9to14bb", nameof(Indicators.BTNShoveOverLimpers9to14bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNShoveOverLimpers15to25bb", nameof(Indicators.BTNShoveOverLimpers15to25bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNShoveOverLimpers26to50bb", nameof(Indicators.BTNShoveOverLimpers26to50bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_BTNShoveOverLimpers51plusbb", nameof(Indicators.BTNShoveOverLimpers51plusbb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COShoveOverLimpers1to8bb", nameof(Indicators.COShoveOverLimpers1to8bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COShoveOverLimpers9to14bb", nameof(Indicators.COShoveOverLimpers9to14bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COShoveOverLimpers15to25bb", nameof(Indicators.COShoveOverLimpers15to25bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COShoveOverLimpers26to50bb", nameof(Indicators.COShoveOverLimpers26to50bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_COShoveOverLimpers51plusbb", nameof(Indicators.COShoveOverLimpers51plusbb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPShoveOverLimpers1to8bb", nameof(Indicators.MPShoveOverLimpers1to8bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPShoveOverLimpers9to14bb", nameof(Indicators.MPShoveOverLimpers9to14bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPShoveOverLimpers15to25bb", nameof(Indicators.MPShoveOverLimpers15to25bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPShoveOverLimpers26to50bb", nameof(Indicators.MPShoveOverLimpers26to50bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_MPShoveOverLimpers51plusbb", nameof(Indicators.MPShoveOverLimpers51plusbb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPShoveOverLimpers1to8bb", nameof(Indicators.EPShoveOverLimpers1to8bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPShoveOverLimpers9to14bb", nameof(Indicators.EPShoveOverLimpers9to14bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPShoveOverLimpers15to25bb", nameof(Indicators.EPShoveOverLimpers15to25bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPShoveOverLimpers26to50bb", nameof(Indicators.EPShoveOverLimpers26to50bb), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_EPShoveOverLimpers51plusbb", nameof(Indicators.EPShoveOverLimpers51plusbb), ColumnType.Percentile),  
            // T
            new Tuple<string, string, ColumnType>("Reports_Column_TotalOverCallSRP", nameof(Indicators.TotalOverCallSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TrueAgg", nameof(Indicators.TrueAggression), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TripleBarrel", nameof(Indicators.RiverCBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TripleBarrelSRP", nameof(Indicators.TripleBarrelSRP), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TripleBarrel3BetPot", nameof(Indicators.TripleBarrel3BetPot), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnAF", nameof(Indicators.TurnAF), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnAgg", nameof(Indicators.TurnAgg), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBet", nameof(Indicators.TurnBet), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneHalfOrLess", nameof(Indicators.TurnBetSizeOneHalfOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneQuarterOrLess", nameof(Indicators.TurnBetSizeOneQuarterOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneThirdOrLess", nameof(Indicators.TurnBetSizeOneThirdOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeTwoThirdsOrLess", nameof(Indicators.TurnBetSizeTwoThirdsOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeThreeQuartersOrLess", nameof(Indicators.TurnBetSizeThreeQuartersOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeOneOrLess", nameof(Indicators.TurnBetSizeOneOrLess), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnBetSizeMoreThanOne", nameof(Indicators.TurnBetSizeMoreThanOne), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_TurnCheckRaise", nameof(Indicators.TurnCheckRaise), ColumnType.Percentile),
            // U            
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
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAsPFR", nameof(Indicators.WTSDAsPFR), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAsPF3Bettor", nameof(Indicators.WTSDAsPF3Bettor), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WTSDAs4Bettor", nameof(Indicators.WTSDAs4Bettor), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WSWSF", nameof(Indicators.WSWSF), ColumnType.Percentile),
            new Tuple<string, string, ColumnType>("Reports_Column_WSSD", nameof(Indicators.WSSD), ColumnType.Percentile),
        };

        #endregion
    }
}
