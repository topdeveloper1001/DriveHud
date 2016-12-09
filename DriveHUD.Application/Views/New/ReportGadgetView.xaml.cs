using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;


using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Persistence;
using Telerik.Windows.Persistence.Services;
using DriveHUD.Entities;
using Model.Interfaces;

using DriveHUD.Application.Controls;
using DriveHUD.Application.ReportsLayout;
using DriveHUD.Application.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls.GridView;
using System.Windows;
using DriveHUD.Common.Resources;
using Telerik.Windows;
using DriveHUD.Common.Ifrastructure;
using Model.Data;
using DriveHUD.Common.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Model.Extensions;
using Model;
using DriveHUD.Application.Views.Replayer;
using DriveHUD.Common.Utils;
using Model.Replayer;
using Model.Enums;
using System.Diagnostics;
using System.Threading.Tasks;
using Model.Reports;
using DriveHUD.Common.Log;

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for ReportGadget.xaml
    /// </summary>
    public partial class ReportGadgetView : UserControl
    {
        private const int REPLAYER_LAST_HANDS_AMOUNT = 10;

        ReportGadgetViewModel reportGadgetViewModel;
        private FixedSizeList<ReplayerDataModel> replayerDataModelList = new FixedSizeList<ReplayerDataModel>(REPLAYER_LAST_HANDS_AMOUNT);
        private Model.Enums.EnumReports reportCache;
        private RadContextMenu handsGridContextMenu;
        private RadContextMenu tournamentsGridContextMenu;
        
        public ReportGadgetView()
        {
            InitializeComponent();

            ServiceProvider.RegisterPersistenceProvider<ICustomPropertyProvider>(typeof(RadGridView), new GridViewCustomPropertyProvider());
            GridViewReportMenu.ItemsSource = GridViewReport.Columns;
            GridViewKnownHandsMenu.ItemsSource = GridViewKnownHands.Columns;

            this.DataContextChanged += (_o, _e) =>
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

            this.Unloaded += (_o, _e) =>
            {
                try
                {
                    GridLayoutSave(GridViewKnownHands, "HandGridLayout.data");

                    ReportLayoutSave();
                }
                catch(Exception ex)
                {
                    LogProvider.Log.Error(ex);
                }
            };
        }

        private void InitContextMenu()
        {
            handsGridContextMenu = new RadContextMenu();
            tournamentsGridContextMenu = new RadContextMenu();
            /* Calculate equity item */
            RadMenuItem calculateEquityItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.CalculateEquityResourceString), false, EquityCalcMenuItem_Click);
            /* Export items */
            RadMenuItem exportHandItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.ExportHandResourceString), false, null);
            RadMenuItem twoPlustTwoItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.TwoPlustTwoResourceString), false, GeneralExportItem_Click);
            RadMenuItem cardsChatItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.CardsChatResourceString), false, GeneralExportItem_Click);
            RadMenuItem pokerStrategyItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.PokerStrategyString), false, GeneralExportItem_Click);
            RadMenuItem rawHistoryItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.RawHandHistoryString), false, RawExportItem_Click);

            exportHandItem.Items.Add(twoPlustTwoItem);
            exportHandItem.Items.Add(cardsChatItem);
            exportHandItem.Items.Add(pokerStrategyItem);
            exportHandItem.Items.Add(rawHistoryItem);
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
            /* Make Note */
            RadMenuItem makeNoteItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.MakeNote), false, MakeNote);
            /* Edit tournament */
            RadMenuItem editTournamentItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.EditTournamentResourceString), false, EditTournament);
            /* Delete Hand */
            RadMenuItem deleteHandItem = CreateRadMenuItem(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DeleteHandResourceString), false, DeleteHand);

            handsGridContextMenu.Items.Add(calculateEquityItem);
            handsGridContextMenu.Items.Add(exportHandItem);
            handsGridContextMenu.Items.Add(replayHand);
            handsGridContextMenu.Items.Add(tagHandItem);
            handsGridContextMenu.Items.Add(makeNoteItem);
            handsGridContextMenu.Items.Add(deleteHandItem);

            tournamentsGridContextMenu.Items.Add(editTournamentItem);
        }

        private RadMenuItem CreateRadMenuItem(string header, bool isCheckable, RadRoutedEventHandler clickAction, object tag = null)
        {
            RadMenuItem item = new RadMenuItem();
            item.Header = header;
            item.IsCheckable = isCheckable;
            item.Tag = tag;
            if (clickAction != null)
            {
                item.Click += clickAction;
            }

            Style style = this.TryFindResource("HandGridMenuItemStyle") as Style;
            if (style != null)
            {
                item.Style = style;
            }

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

        private void DeleteHand(object sender, RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();
            if (item != null)
            {
                reportGadgetViewModel.DeleteHandCommand.Execute(item.DataContext);
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
            if (item == null || !(item.DataContext is ComparableCardsStatistic))
            {
                return;
            }

            var stat = (item.DataContext as ComparableCardsStatistic).Statistic;
            if (stat != null)
            {
                var handNoteEntity = stat.HandNote;
                if (handNoteEntity == null)
                {
                    handNoteEntity = new Handnotes
                    {
                        Gamenumber = stat.GameNumber,
                        PokersiteId = (short)stat.PokersiteId,
                    };
                    stat.HandNote = handNoteEntity;
                }
                handNoteEntity.CategoryId = (int)(item.Tag ?? 0);
                ServiceLocator.Current.GetInstance<IDataService>().Store(handNoteEntity);

                if (reportGadgetViewModel.FilterTaggedHands_IsChecked)
                {
                    reportGadgetViewModel.RefreshReport();
                }
            }
        }

        private void GeneralExportItem_Click(object sender, RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();
            if (item != null)
            {
                var playerStatistics = item.DataContext as ComparableCardsStatistic;
                if (playerStatistics != null)
                {
                    var handHistory = ServiceLocator.Current.GetInstance<IDataService>().GetGame(playerStatistics.Statistic.GameNumber, (short)playerStatistics.Statistic.PokersiteId);
                    String hh = ExportFunctions.ConvertHHToForumFormat(handHistory);
                    Clipboard.SetText(hh);
                    reportGadgetViewModel.RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
                }
            }
        }

        private void RawExportItem_Click(object sender, RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();
            if (item != null)
            {
                var playerStatistics = item.DataContext as ComparableCardsStatistic;
                if (playerStatistics != null)
                {
                    var handHistory = ServiceLocator.Current.GetInstance<IDataService>().GetGame(playerStatistics.Statistic.GameNumber, (short)playerStatistics.Statistic.PokersiteId);
                    Clipboard.SetText(handHistory.FullHandHistoryText);
                    reportGadgetViewModel.RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
                }
            }
        }

        private void EquityCalcMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();
            if (item != null)
            {
                if (this.DataContext is ReportGadgetViewModel)
                {
                    (this.DataContext as ReportGadgetViewModel).CalculateEquityCommand.Execute((item.DataContext as ComparableCardsStatistic).Statistic);
                }
            }
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
            catch(Exception ex)
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

        private async void ReportSet(EnumReports reportType)
        {
            try
            {
                BusyIndicator.Visibility = Visibility.Visible;
                GridViewReport.Visibility = Visibility.Collapsed;
                // disable radio button panel to restrict changing the report type during loading
                ReportRadioButtonPanel.IsEnabled = false;

                var layout = ReportManager.GetReportLayout(reportType);
                var creator = ReportManager.GetReportCreator(reportType);
                if (layout == null || creator == null) return;

                var reportCollection =
                    reportType == EnumReports.OpponentAnalysis ?
                    GetReportCollectionAsync(creator, await reportGadgetViewModel.GetTop()) :
                    GetReportCollectionAsync(creator, ServiceLocator.Current.GetInstance<SingletonStorageModel>().FilteredPlayerStatistic);
                
                // clear columns in order to avoid  Binding exceptions
                GridViewReport.Columns.Clear();
                reportGadgetViewModel.ReportCollection.Clear();

                foreach (var item in await reportCollection)
                {
                    reportGadgetViewModel.ReportCollection.Add(item);
                }

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
                BusyIndicator.Visibility = Visibility.Collapsed;
                GridViewReport.Visibility = Visibility.Visible;
            }
        }

        private Task<ObservableCollection<Indicators>> GetReportCollectionAsync(IReportCreator reportCreator, IList<Playerstatistic> playerstatistics)
        {
            return Task.Run(() =>
            {
                return reportCreator.Create(playerstatistics);
            });
        }

        private void ReportUpdate()
        {
            ResizeReportGrid();
            ReportSet(reportGadgetViewModel.ReportSelectedItemStat);

            ReportLayoutLoad();
        }

        private void GridLayoutSave(RadGridView gridView, string fileName)
        {
            PersistenceManager manager = new PersistenceManager();

            var stream = manager.Save(gridView);
            using (Stream file = ServiceLocator.Current.GetInstance<IDataService>().OpenStorageStream(fileName, FileMode.Create))
            {
                stream.CopyTo(file);
            }
        }

        private void GridLayoutLoad(RadGridView gridView, string fileName)
        {
            PersistenceManager manager = new PersistenceManager();
            try
            {
                using (Stream file = ServiceLocator.Current.GetInstance<IDataService>().OpenStorageStream(fileName, FileMode.Open))
                {
                    if (file != null)
                    {
                        manager.Load(gridView, file);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // If there is no file proceed normally;
            }
        }

        private void ReplayHand(ComparableCardsStatistic hand)
        {
            if (hand == null)
                return;

            var statistic = (hand as ComparableCardsStatistic).Statistic;
            var dataModelStatistic = new ReplayerDataModel(statistic);
            replayerDataModelList.ForEach(x => x.IsActive = false);

            var dataModel = replayerDataModelList.FirstOrDefault(x => x.Equals(dataModelStatistic));
            if (dataModel == null)
            {
                dataModelStatistic.IsActive = true;
                replayerDataModelList.Add(dataModelStatistic);
            }
            else
            {
                dataModel.IsActive = true;
                replayerDataModelList.Move(replayerDataModelList.IndexOf(dataModel), replayerDataModelList.Count - 1);
            }

            ReplayerView replayer = new ReplayerView(replayerDataModelList, ReplayerHelpers.CreateSessionHandsList(reportGadgetViewModel.StorageModel.StatisticCollection, statistic), (this.DataContext as ReportGadgetViewModel).ReplayerShowHolecards_IsChecked);
            replayer.Show();
        }

        private void ReplayHand(object sender, RadRoutedEventArgs e)
        {
            var item = handsGridContextMenu.GetClickedElement<GridViewRow>();
            if (item != null)
            {
                ReplayHand(item.DataContext as ComparableCardsStatistic);
            }
        }

        private void GridViewKnownHands_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = GridViewKnownHands.SelectedItem;
            if (row == null)
                return;

            ReplayHand(row as ComparableCardsStatistic);
        }

        private void GridViewKnownHands_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            var row = e.Row as GridViewRow;
            if (row != null)
            {
                RadContextMenu.SetContextMenu(row, handsGridContextMenu);
            }
        }

        private void GridViewReport_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            var row = e.Row as GridViewRow;
            if (row != null)
            {
                if (reportGadgetViewModel.ReportSelectedItemStat == EnumReports.Tournaments)
                {
                    RadContextMenu.SetContextMenu(row, tournamentsGridContextMenu);
                }
                else
                {
                    RadContextMenu.SetContextMenu(row, null);
                }
            }
        }

        private void GridViewReport_Sorting(object sender, GridViewSortingEventArgs e)
        {
            List<Indicators> statistics = new List<Indicators>(e.DataControl.ItemsSource as IEnumerable<Indicators>);
            if (statistics == null)
            {
                e.Cancel = true;
                return;
            }

            var sorted = SortData<Indicators>(statistics, e);
            if (sorted != null)
            {
                e.DataControl.SortDescriptors.Clear();
                var collection = (DataContext as ReportGadgetViewModel).ReportCollection;

                /* Remove and reassign binding to not affect performance
                *  (In case  if  SelectedItem index changed it will lose selection so HandsGrid will be unloaded)
                */
                var binding = System.Windows.Data.BindingOperations.GetBinding(e.DataControl, GridViewDataControl.SelectedItemProperty);
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
            List<ComparableCardsStatistic> statistics = new List<ComparableCardsStatistic>(e.DataControl.ItemsSource as IEnumerable<ComparableCardsStatistic>);
            if (statistics == null)
            {
                e.Cancel = true;
                return;
            }

            var sorted = SortData<ComparableCardsStatistic>(statistics, e);
            if (sorted != null)
            {
                e.DataControl.SortDescriptors.Clear();
                var collection = (DataContext as ReportGadgetViewModel).ReportSelectedItemStatisticsCollection_Filtered;

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
