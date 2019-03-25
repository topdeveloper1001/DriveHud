//-----------------------------------------------------------------------
// <copyright file="ReportGadgetView.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Controls;
using DriveHUD.Application.ReportsLayout;
using DriveHUD.Application.ValueConverters;
using DriveHUD.Application.ViewModels;
using DriveHUD.Common.Ifrastructure;
using DriveHUD.Common.Log;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Converters;
using DriveHUD.Entities;
using DriveHUD.ViewModels.Replayer;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model;
using Model.Data;
using Model.Enums;
using Model.Events;
using Model.Hud;
using Model.Interfaces;
using Model.Reports;
using OfficeOpenXml;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;
using Telerik.Windows.Persistence;
using Telerik.Windows.Persistence.Services;

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for ReportGadget.xaml
    /// </summary>
    public partial class ReportGadgetView : UserControl
    {
        private ReportGadgetViewModel reportGadgetViewModel;
        private Model.Enums.EnumReports reportCache;
        private RadContextMenu handsGridContextMenu;
        private RadContextMenu tournamentsGridContextMenu;
        private RadContextMenu reportsGridContextMenu;
        private readonly IDataService dataService;

        public ReportGadgetView()
        {
            InitializeComponent();

            reportCancellationTokenSource = new CancellationTokenSource();

            dataService = ServiceLocator.Current.GetInstance<IDataService>();

            ServiceProvider.RegisterPersistenceProvider<ICustomPropertyProvider>(typeof(RadGridView), new GridViewCustomPropertyProvider());

            GridViewReportMenu.ItemsSource = GridViewReport.Columns;
            GridViewKnownHandsMenu.ItemsSource = GridViewKnownHands.Columns;

            DataContextChanged += (o, e) =>
            {
                try
                {
                    reportGadgetViewModel = (ReportGadgetViewModel)this.DataContext;
                    InitContextMenu();

                    reportGadgetViewModel.PropertyChanged += ReportGadgetViewModel_PropertyChanged;
                    LoadData();
                    ReportUpdate();
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(ex);
                }
            };

            Unloaded += (o, e) =>
            {
                try
                {
                    GridLayoutSave(GridViewKnownHands, "HandGridLayout.data");

                    ReportLayoutSave();
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(ex);
                }
            };

            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            eventAggregator.GetEvent<CancelReportLoadingEvent>().Subscribe(x =>
            {
                reportCancellationTokenSource?.Cancel();
                reportCancellationTokenSource = new CancellationTokenSource();
            });
        }

        private void InitContextMenu()
        {
            handsGridContextMenu = new RadContextMenu();
            tournamentsGridContextMenu = new RadContextMenu();
            reportsGridContextMenu = new RadContextMenu();

            /* Calculate equity item */
            RadMenuItem calculateEquityItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.CalculateEquityResourceString), false,
                EquityCalcMenuItem_Click);

            Binding equityEnabledBinding = new Binding(nameof(ReportGadgetViewModel.IsEquityCalculatorEnabled)) { Source = this.reportGadgetViewModel };
            calculateEquityItem.SetBinding(RadMenuItem.IsEnabledProperty, equityEnabledBinding);

            handsGridContextMenu.Opening += (s, e) =>
            {
                var item = handsGridContextMenu.GetClickedElement<GridViewRow>();

                if (item != null && item.DataContext != null &&
                    item.DataContext is ReportHandViewModel hand &&
                        (GameTypeUtils.CompareGameType((GameType)hand.PokerGameTypeId, GameType.NoLimitOmaha) ||
                            GameTypeUtils.CompareGameType((GameType)hand.PokerGameTypeId, GameType.NoLimitOmahaHiLo)))
                {
                    calculateEquityItem.Visibility = Visibility.Collapsed;
                    return;
                }

                calculateEquityItem.Visibility = Visibility.Visible;
            };

            /* Export items */
            RadMenuItem exportHandItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ExportHandResourceString), false, null);
            RadMenuItem twoPlustTwoItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.TwoPlustTwoResourceString), false, GeneralExportItem_Click, EnumExportType.TwoPlusTwo);
            RadMenuItem cardsChatItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.CardsChatResourceString), false, GeneralExportItem_Click, EnumExportType.CardsChat);
            RadMenuItem pokerStrategyItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.PokerStrategyString), false, GeneralExportItem_Click, EnumExportType.PokerStrategy);
            RadMenuItem icmizerHistoryItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ICMizerHandHistory), false,
                RawExportItem_Click, extraAction: item =>
            {
                var binding = new Binding(nameof(ReportGadgetViewModel.IsShowTournamentData))
                {
                    Source = reportGadgetViewModel,
                    Converter = new BoolToVisibilityConverter()
                };

                item.SetBinding(VisibilityProperty, binding);
            });

            RadMenuItem exportTo3rdPartyItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ExportGridHandsResourceString), false, null,
                command: nameof(ReportGadgetViewModel.ExportSelectedHandsTo3rdPartyCommand));

            RadMenuItem rawHistoryItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.RawHandHistoryString), false, RawExportItem_Click);
            RadMenuItem plainTextHandHistoryItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.PlainTextHandHistoryString), false, PlainExportItem_Click);


            exportHandItem.Items.Add(twoPlustTwoItem);
            exportHandItem.Items.Add(cardsChatItem);
            exportHandItem.Items.Add(pokerStrategyItem);
            exportHandItem.Items.Add(icmizerHistoryItem);
            exportHandItem.Items.Add(rawHistoryItem);
            exportHandItem.Items.Add(plainTextHandHistoryItem);
            exportHandItem.Items.Add(exportTo3rdPartyItem);

            /* Replay hand */
            RadMenuItem replayHand = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ReplayHandResourceString), false, ReplayHand);
            /* Tag Hand */
            RadMenuItem tagHandItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.TagHand), false, null);
            RadMenuItem forReviewItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagForReview), false, TagHand, EnumHandTag.ForReview);
            RadMenuItem bluffItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBluff), false, TagHand, EnumHandTag.Bluff);
            RadMenuItem heroCallitem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagHeroCall), false, TagHand, EnumHandTag.HeroCall);
            RadMenuItem bigFold = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBigFold), false, TagHand, EnumHandTag.BigFold);
            RadMenuItem removeTag = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagNone), false, TagHand, EnumHandTag.None);

            tagHandItem.Items.Add(forReviewItem);
            tagHandItem.Items.Add(bluffItem);
            tagHandItem.Items.Add(heroCallitem);
            tagHandItem.Items.Add(bigFold);
            tagHandItem.Items.Add(removeTag);

            var makeNoteItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.MakeNote), false, MakeNote);
            var editTournamentItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.EditTournamentResourceString), false, EditTournament);
            var deleteHandItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DeleteHandResourceString), false, null, command: nameof(ReportGadgetViewModel.DeleteHandCommand));
            var deleteTournamentItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DeleteTournamentResourceString), false, DeleteTournament);

            handsGridContextMenu.Items.Add(calculateEquityItem);
            handsGridContextMenu.Items.Add(exportHandItem);
            handsGridContextMenu.Items.Add(replayHand);
            handsGridContextMenu.Items.Add(tagHandItem);
            handsGridContextMenu.Items.Add(makeNoteItem);
            handsGridContextMenu.Items.Add(deleteHandItem);

            tournamentsGridContextMenu.Items.Add(editTournamentItem);
            tournamentsGridContextMenu.Items.Add(deleteTournamentItem);
            tournamentsGridContextMenu.Items.Add(CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.RefreshReportResourceString), false, RefreshReport));
            tournamentsGridContextMenu.Items.Add(CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ExportToExcelReportResourceString), false, ExportReport));
            tournamentsGridContextMenu.Items.Add(CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ExportHandsResourceString), false, null,
                command: nameof(ReportGadgetViewModel.ExportSelectedReportsTo3rdPartyCommand)));

            reportsGridContextMenu.Items.Add(CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.RefreshReportResourceString), false, RefreshReport));
            reportsGridContextMenu.Items.Add(CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ExportToExcelReportResourceString), false, ExportReport));
            reportsGridContextMenu.Items.Add(CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ExportHandsResourceString), false, null,
                command: nameof(ReportGadgetViewModel.ExportSelectedReportsTo3rdPartyCommand)));
        }

        private RadMenuItem CreateRadMenuItem(string header, bool isCheckable, RadRoutedEventHandler clickAction, object tag = null,
            Action<RadMenuItem> extraAction = null, string command = null)
        {
            var item = new RadMenuItem
            {
                Header = header,
                IsCheckable = isCheckable,
                Tag = tag
            };

            if (!string.IsNullOrEmpty(command))
            {
                var commandBinding = new Binding(command)
                {
                    Source = reportGadgetViewModel
                };

                item.SetBinding(RadMenuItem.CommandProperty, commandBinding);
            }

            if (clickAction != null)
            {
                item.Click += clickAction;
            }

            if (TryFindResource("HandGridMenuItemStyle") is Style style)
            {
                item.Style = style;
            }

            extraAction?.Invoke(item);

            return item;
        }

        private void EditTournament(object sender, RadRoutedEventArgs e)
        {
            var item = tournamentsGridContextMenu.GetClickedElement<GridViewRow>();

            if (item != null)
            {
                reportGadgetViewModel.EditTournamentCommand.Execute(item.DataContext);
            }
        }

        private void DeleteTournament(object sender, RadRoutedEventArgs e)
        {
            var item = tournamentsGridContextMenu.GetClickedElement<GridViewRow>();

            if (item != null)
            {
                reportGadgetViewModel.DeleteTournamentCommand.Execute(item.DataContext);
            }
        }

        private void RefreshReport(object sender, RadRoutedEventArgs e)
        {
            ReportLayoutSave();
            ReportUpdate(true);
        }

        private void ExportReport(object sender, RadRoutedEventArgs e)
        {
            try
            {
                string extension = "xls";

                var dialog = new SaveFileDialog()
                {
                    DefaultExt = extension,
                    Filter = String.Format("{1} files (.{0})|.{0}|All files (.)|.", extension, "Excel"),
                    FilterIndex = 1
                };

                if (dialog.ShowDialog() == true)
                {
                    using (var stream = dialog.OpenFile())
                    {
                        using (var excel = new ExcelPackage(stream))
                        {
                            var reportName = CommonResourceManager.Instance.GetEnumResource(reportGadgetViewModel.ReportSelectedItemStat);

                            var ws = excel.Workbook.Worksheets.Add(reportName);

                            using (var ms = new MemoryStream())
                            {
                                GridViewReport.ElementExporting += GridViewReport_ElementExporting;

                                GridViewReport.Export(ms, new GridViewCsvExportOptions
                                {
                                    Format = ExportFormat.Csv,
                                    Encoding = Encoding.UTF8,
                                    ShowColumnHeaders = true,
                                    ShowColumnFooters = false,
                                    ShowGroupFooters = false,
                                    ColumnDelimiter = ";"
                                });

                                GridViewReport.ElementExporting -= GridViewReport_ElementExporting;

                                var csv = Encoding.UTF8.GetString(ms.ToArray()).Replace("\"", string.Empty);

                                var textFormat = new ExcelTextFormat
                                {
                                    Delimiter = ';'
                                };

                                var columns = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                                if (columns.Length > 0)
                                {
                                    var columsCount = columns[0].Count(x => x == ';') + 1;
                                    textFormat.DataTypes = Enumerable.Range(0, columsCount).Select(x => eDataTypes.String).ToArray();
                                }

                                ws.Cells["A1"].LoadFromText(csv, textFormat);
                                ws.Cells.AutoFitColumns();

                                excel.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not export report to excel format.", ex);
                ErrorBox.Show("Error", ex, "Unexpected error occurred. Please contact support.");
            }
        }

        private void GridViewReport_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            if (e.Element == ExportElement.Cell)
            {
                if (e.Value is HudPlayerType hudPlayerType)
                {
                    e.Value = hudPlayerType.Name;
                }
                else if (e.Value is EnumMRatio mRatio)
                {
                    var converter = new MRatioToTextConverter();
                    e.Value = converter.Convert(e.Value, typeof(string), null, null);
                }
            }
        }

        private void MakeNote(object sender, RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();

            if (item != null)
            {
                reportGadgetViewModel.MakeNoteCommand.Execute(item.DataContext);
            }
        }

        private void TagHand(object sender, RadRoutedEventArgs e)
        {
            var item = sender as RadMenuItem;

            if (item == null)
            {
                return;
            }

            var reportHand = item.DataContext as ReportHandViewModel;

            if (reportHand == null)
            {
                return;
            }

            var handNoteEntity = new Handnotes
            {
                Note = reportHand.HandNote,
                Gamenumber = reportHand.GameNumber,
                PokersiteId = (short)reportHand.PokerSiteId,
                HandTag = (int)(item.Tag ?? 0)
            };

            dataService.Store(handNoteEntity);

            reportHand.HandTag = handNoteEntity.HandTag.HasValue ?
                (EnumHandTag)handNoteEntity.HandTag.Value :
                EnumHandTag.None;

            var statistic = reportGadgetViewModel.ReportSelectedItem?.Statistics?
                .FirstOrDefault(x => x.GameNumber == reportHand.GameNumber && x.PokersiteId == reportHand.PokerSiteId);

            if (statistic != null)
            {
                statistic.HandNote = handNoteEntity;
            }

            if (reportGadgetViewModel.FilterTaggedHands_IsChecked)
            {
                reportGadgetViewModel.RefreshReport();
            }
        }

        private void ExportItem(Func<HandHistory, string> convertToText)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();

            if (item == null)
            {
                return;
            }

            var reportHand = item.DataContext as ReportHandViewModel;

            if (reportHand == null)
            {
                return;
            }

            var handHistory = dataService.GetGame(reportHand.GameNumber, (short)reportHand.PokerSiteId);

            var handHistoryText = convertToText(handHistory);

            Clipboard.SetText(handHistoryText);

            reportGadgetViewModel.RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
        }

        private void PlainExportItem_Click(object sender, RadRoutedEventArgs e)
        {
            ExportItem(handHistory => ExportFunctions.ConvertHHToForumFormat(handHistory, EnumExportType.PlainText));
        }

        private void GeneralExportItem_Click(object sender, RadRoutedEventArgs e)
        {
            var menuItem = sender as RadMenuItem;
            var exportType = menuItem?.Tag as EnumExportType? ?? EnumExportType.TwoPlusTwo;

            ExportItem(handHistory => ExportFunctions.ConvertHHToForumFormat(handHistory, exportType));
        }

        private void RawExportItem_Click(object sender, RadRoutedEventArgs e)
        {
            ExportItem(handHistory => handHistory.FullHandHistoryText);
        }

        private void EquityCalcMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();

            if (item == null)
            {
                return;
            }

            var reportHand = item.DataContext as ReportHandViewModel;

            if (reportHand == null)
            {
                return;
            }

            reportGadgetViewModel?.CalculateEquityCommand.Execute(reportHand);
        }

        private void ReportGadgetViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(ReportGadgetViewModel.ReportSelectedItemStat))
                {
                    ReportLayoutSave();
                    ReportUpdate();
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
        }

        public void LoadData()
        {
            GridLayoutLoad(GridViewKnownHands, "HandGridLayout.data");
        }

        private void ReportLayoutLoad()
        {
            GridLayoutLoad(GridViewReport, string.Format("{0}ReportLayout.data", reportGadgetViewModel.ReportSelectedItemStat));
        }

        private void ReportLayoutSave()
        {
            GridLayoutSave(GridViewReport, string.Format("{0}ReportLayout.data", reportCache));
        }

        private async Task ReportSet(EnumReports reportType, bool forceRefresh)
        {
            try
            {
                reportGadgetViewModel.IsBusy = true;

                // disable radio button panel to restrict changing the report type during loading
                ReportRadioButtonPanel.IsEnabled = false;

                var layout = ReportManager.GetReportLayout(reportType);
                var creator = ReportManager.GetReportCreator(reportType);

                if (layout == null || creator == null)
                {
                    return;
                }

                var storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();

                var statistic = creator.IsTournament ?
                    storageModel.GetFilteredTournamentPlayerStatistic() :
                    storageModel.GetFilteredCashPlayerStatistic();

                // stop loading report
                reportCancellationTokenSource?.Cancel();
                reportCancellationTokenSource = new CancellationTokenSource();

                var reportCancellationToken = reportCancellationTokenSource.Token;

                var reportCollection = GetReportCollectionAsync(creator, statistic, forceRefresh, reportCancellationToken);

                if (reportCancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // clear columns in order to avoid  Binding exceptions
                GridViewReport.Columns.Clear();

                reportGadgetViewModel.ReportCollection.Clear();
                reportGadgetViewModel.SelectedReportItems.Clear();

                foreach (var item in await reportCollection)
                {
                    reportGadgetViewModel.ReportCollection.Add(item);
                }

                reportGadgetViewModel.IsBusy = false;
                GridViewReport.FrozenColumnCount = 0;

                layout.Create(GridViewReport);

                reportGadgetViewModel.ReportSelectedItem = reportGadgetViewModel.ReportCollection.FirstOrDefault();

                reportCache = reportType;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
            finally
            {
                ReportRadioButtonPanel.IsEnabled = true;
            }
        }

        private Task<ObservableCollection<ReportIndicators>> GetReportCollectionAsync(IReportCreator reportCreator,
            List<Playerstatistic> playerstatistics, bool forceRefresh, CancellationToken reportCancellationToken)
        {
            return Task.Run(() =>
            {
                return reportCreator.Create(playerstatistics, reportCancellationToken, forceRefresh);
            });
        }

        private CancellationTokenSource reportCancellationTokenSource;

        private async void ReportUpdate(bool forceRefresh = false)
        {
            ResizeReportGrid();
            await ReportSet(reportGadgetViewModel.ReportSelectedItemStat, forceRefresh);

            ReportLayoutLoad();
        }

        private void GridLayoutSave(RadGridView gridView, string fileName)
        {
            try
            {
                var manager = new PersistenceManager();

                var stream = manager.Save(gridView);

                using (var file = dataService.OpenStorageStream(fileName, FileMode.Create))
                {
                    stream.CopyTo(file);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not save grid layout.", e);
            }
        }

        private void GridLayoutLoad(RadGridView gridView, string fileName)
        {
            var manager = new PersistenceManager();

            try
            {
                using (var file = dataService.OpenStorageStream(fileName, FileMode.Open))
                {
                    if (file != null)
                    {
                        manager.Load(gridView, file);
                        gridView.UpdateLayout();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // If there is no file proceed normally;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not load grid layout.", e);
            }
        }

        private void ReplayHand(ReportHandViewModel hand)
        {
            if (hand == null)
            {
                return;
            }

            bool showHoleCards = reportGadgetViewModel.ReplayerShowHolecards_IsChecked;

            ServiceLocator.Current.GetInstance<IReplayerService>()
                .ReplayHand(hand.PlayerName, hand.GameNumber, (short)hand.PokerSiteId, showHoleCards);
        }

        private void ReplayHand(object sender, RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();

            if (item != null)
            {
                ReplayHand(item.DataContext as ReportHandViewModel);
            }
        }

        private void GridViewKnownHands_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = GridViewKnownHands.SelectedItem;

            if (row == null)
            {
                return;
            }

            ReplayHand(row as ReportHandViewModel);
        }

        private void GridViewKnownHands_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row is GridViewRow row)
            {
                RadContextMenu.SetContextMenu(row, handsGridContextMenu);
            }
        }

        private void GridViewReport_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row is GridViewRow row)
            {
                if (reportGadgetViewModel.ReportSelectedItemStat == EnumReports.Tournaments)
                {
                    RadContextMenu.SetContextMenu(row, tournamentsGridContextMenu);
                }
                else
                {
                    RadContextMenu.SetContextMenu(row, reportsGridContextMenu);
                }
            }
        }

        private void GridViewReport_Sorting(object sender, GridViewSortingEventArgs e)
        {
            var reports = new List<ReportIndicators>(e.DataControl.ItemsSource as IEnumerable<ReportIndicators>);

            if (reports == null)
            {
                e.Cancel = true;
                return;
            }

            var sorted = SortData(reports, e);

            if (sorted != null)
            {
                e.DataControl.SortDescriptors.Clear();

                var collection = (DataContext as ReportGadgetViewModel).ReportCollection;

                // Remove and reassign binding to not affect performance
                // (In case  if  SelectedItem index changed it will lose selection so HandsGrid will be unloaded)                
                var binding = BindingOperations.GetBinding(e.DataControl, GridViewDataControl.SelectedItemProperty);

                BindingOperations.ClearBinding(e.DataControl, GridViewDataControl.SelectedItemProperty);

                for (int i = 0; i < sorted.Count(); i++)
                {
                    collection.Move(collection.IndexOf(sorted.ElementAt(i)), i);
                }

                BindingOperations.SetBinding(e.DataControl, GridViewDataControl.SelectedItemProperty, binding);

                e.Cancel = true;
            }
        }

        private void GridViewKnownHands_Sorting(object sender, GridViewSortingEventArgs e)
        {
            var reportHands = new List<ReportHandViewModel>(e.DataControl.ItemsSource as IEnumerable<ReportHandViewModel>);

            if (reportHands == null)
            {
                e.Cancel = true;
                return;
            }

            var sorted = SortData(reportHands, e);

            if (sorted != null)
            {
                e.DataControl.SortDescriptors.Clear();

                var collection = (DataContext as ReportGadgetViewModel).FilteredReportSelectedItemStatisticsCollection;

                for (int i = 0; i < sorted.Count(); i++)
                {
                    collection.Move(collection.IndexOf(sorted.ElementAt(i)), i);
                }

                e.Cancel = true;
            }
        }

        private IEnumerable<T> SortData<T>(IEnumerable<T> collection, GridViewSortingEventArgs e)
        {
            var columnValues = collection.Select(x => ReflectionHelper.GetMemberValue(x, (e.Column as GridViewDataColumn).GetDataMemberName()));

            IEnumerable<T> result = null;

            if (columnValues.Any(x => x == null || string.IsNullOrEmpty(x.ToString())))
            {
                return result;
            }

            decimal decValue = 0;

            if (!columnValues.Any(x => !Decimal.TryParse(x.ToString(), out decValue)))
            {
                if (e.OldSortingState == SortingState.None)
                {
                    e.NewSortingState = SortingState.Ascending;
                    result = collection.OrderBy(x => Decimal.Parse(ReflectionHelper.GetMemberValue(x, (e.Column as GridViewDataColumn).GetDataMemberName()).ToString())).ToList();
                }
                else if (e.OldSortingState == SortingState.Ascending)
                {
                    e.NewSortingState = SortingState.Descending;
                    result = collection.OrderByDescending(x => Decimal.Parse(ReflectionHelper.GetMemberValue(x, (e.Column as GridViewDataColumn).GetDataMemberName()).ToString())).ToList();
                }
                else
                {
                    e.NewSortingState = SortingState.None;
                    result = collection.OrderByDescending(x => Math.Abs(Decimal.Parse(ReflectionHelper.GetMemberValue(x, (e.Column as GridViewDataColumn).GetDataMemberName()).ToString()))).ToList();
                }
            }

            return result;
        }

        #region GridSplitter

        private bool isMaxSizeChanged = false;

        private void ResizeReportGrid()
        {
            isMaxSizeChanged = false;
            if (ReportGridRowDefinition.Height != GridLength.Auto)
            {
                GridViewReport.MaxHeight = GridViewReport.ActualHeight;


                if (HandsGridRowDefinition.ActualHeight < 250)
                {
                    GridViewReport.MaxHeight = this.ActualHeight - 250;
                }

                if (GridViewReport.MaxHeight < 100)
                {
                    GridViewReport.MaxHeight = 120;
                }

                ReportGridRowDefinition.Height = GridLength.Auto;

            }
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (!isMaxSizeChanged)
            {
                isMaxSizeChanged = true;
                GridViewReport.MaxHeight = double.PositiveInfinity;
            }
        }

        #endregion
    }
}