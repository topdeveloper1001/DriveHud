using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels.Replayer;
using DriveHUD.Common.Ifrastructure;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using Model;
using Model.Enums;
using Model.Replayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

namespace DriveHUD.Application.Views.Replayer
{
    /// <summary>
    /// Interaction logic for ReplayerView.xaml
    /// </summary>
    public partial class ReplayerView : RadWindow
    {
        private FixedSizeList<ReplayerDataModel> _lastHandsCollection;

        public ReplayerView(FixedSizeList<ReplayerDataModel> dataModelList, IEnumerable<ReplayerDataModel> sessionHandsList, bool showHoleCards)
        {
            InitializeComponent();

            if (dataModelList == null || dataModelList.Count() == 0 || !dataModelList.Any(x => x.IsActive))
            {
                throw new ArgumentException("Data model list should contain at least one active value", "dataModelList");
            }
            this._lastHandsCollection = dataModelList;

            var dataModel = dataModelList.First(x => x.IsActive);

            this.Owner = App.Current.MainWindow;
            this.Header = StringFormatter.GetReplayerHeaderString(dataModel.GameType, dataModel.Time);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            ViewModel.IsShowHoleCards = showHoleCards;
            ViewModel.ActivePlayerName = dataModel.Statistic.PlayerName;
            ViewModel.CurrentHand = dataModel;
            ViewModel.LastHandsCollection = new ObservableCollection<ReplayerDataModel>(dataModelList);
            ViewModel.SessionHandsCollection = new ObservableCollection<ReplayerDataModel>(sessionHandsList);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (e.PropertyName == ReflectionHelper.GetPath<ReplayerViewModel>(o => o.CurrentHand))
            {
                ViewModel.StopCommand.Execute(null);

                Configurator.ConfigureTable(diagram, ViewModel);

                if (ViewModel.CurrentHand != null)
                {
                    UpdateCollections();
                }
            }
        }

        private void UpdateCollections()
        {
            if (this._lastHandsCollection != null)
            {
                if (!this._lastHandsCollection.Any(x => x.Equals(ViewModel.CurrentHand)))
                {
                    this._lastHandsCollection.Add(ViewModel.CurrentHand);
                    ViewModel.LastHandsCollection.Add(ViewModel.CurrentHand);
                }
            }

            if (ViewModel.SessionHandsCollection != null)
            {
                if (!ViewModel.SessionHandsCollection.Any(x => x.Equals(ViewModel.CurrentHand)))
                {
                    SessionListBox.SelectedIndex = -1;
                }
            }
        }

        private void OnDiagramViewportChanged(object sender, PropertyEventArgs<Rect> e)
        {
            diagram.BringIntoView(new Rect(1, 1, e.NewValue.Width, e.NewValue.Height), false);
        }

        private ReplayerViewModel ViewModel
        {
            get { return DataContext as ReplayerViewModel; }
        }

        private IReplayerTableConfigurator Configurator
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IReplayerTableConfigurator>();
            }
        }

        private void RadWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            ViewModel.StopCommand.Execute(null);
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void GeneralExportItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (ViewModel?.CurrentGame != null)
            {
                var menuItem = sender as RadMenuItem;
                var exportType = menuItem?.Tag as EnumExportType? ?? EnumExportType.TwoPlusTwo;

                Clipboard.SetText(ExportFunctions.ConvertHHToForumFormat(ViewModel.CurrentGame, exportType));
                ViewModel.RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
            }
        }

        private void RawExportItem_Click(object sender, RadRoutedEventArgs e)
        {
            if (ViewModel?.CurrentGame != null)
            {
                Clipboard.SetText(ViewModel.CurrentGame.FullHandHistoryText);
                ViewModel.RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
            }
        }

        private void PlainTextExportItem_Click(object sender, RadRoutedEventArgs e)
        {
            if (ViewModel?.CurrentGame != null)
            {
                Clipboard.SetText(ExportFunctions.ConvertHHToForumFormat(ViewModel.CurrentGame, Model.Enums.EnumExportType.PlainText));
                ViewModel.RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
            }
        }
    }
}