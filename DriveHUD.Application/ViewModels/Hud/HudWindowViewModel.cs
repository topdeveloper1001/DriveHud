using DriveHUD.Common.Extensions;
using DriveHUD.Common.Ifrastructure;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Entities;
using DriveHUD.HUD.Service;
using Model.Enums;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudWindowViewModel : BaseViewModel
    {
        private ReaderWriterLockSlim _readerWriterLock;
        private long _gameNumber;
        private short _pokerSiteId;
        public EnumGameType? GameType { get; set; }
        public EnumTableType TableType { get; set; }

        private string _layoutName;
        public string LayoutName
        {
            get { return _layoutName; }
            set { SetProperty(ref _layoutName, value); }
        }

        internal HudWindowViewModel()
        {
            _readerWriterLock = new ReaderWriterLockSlim();

            NotificationRequest = new InteractionRequest<INotification>();

            _layoutsCollection = new ObservableCollection<string>();

            ExportHandCommand = new RelayCommand(ExportHand);
            TagHandCommand = new RelayCommand(TagHand);
            ReplayLastHandCommand = new RelayCommand(ReplayLastHand);
            LoadLayoutCommand = new RelayCommand(LoadLayout);
        }

        #region Properties

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private string _selectedLayout;
        public string SelectedLayout
        {
            get { return _selectedLayout; }
            set { SetProperty(ref _selectedLayout, value); }
        }

        private ObservableCollection<string> _layoutsCollection;
        
        public ObservableCollection<string> LayoutsCollection
        {
            get { return _layoutsCollection; }
            set { SetProperty(ref _layoutsCollection, value); }
        }

        #endregion

        #region ICommand

        public ICommand ExportHandCommand { get; set; }
        public ICommand TagHandCommand { get; set; }
        public ICommand ReplayLastHandCommand { get; set; }
        public ICommand LoadLayoutCommand { get; set; }
        
        #endregion

        #region Methods

        internal void SetLayout(HudLayout layout)
        {
            using (var write = _readerWriterLock.Write())
            {
                if (layout != null)
                {
                    LayoutName = layout.LayoutName;
                    _gameNumber = layout.GameNumber;
                    _pokerSiteId = (short)layout.PokerSite;

                    if (layout.AvailableLayouts != null)
                    {
                        LayoutsCollection = new ObservableCollection<string>(layout.AvailableLayouts);
                        SelectedLayout = LayoutsCollection.FirstOrDefault(x => x == layout.LayoutName);
                    }
                }
            }
        }

        internal void SaveHudPositions(HudLayoutContract hudTable)
        {
            if (hudTable == null)
            {
                return;
            }

            HudNamedPipeBindingService.RaiseSaveHudLayout(hudTable, _pokerSiteId, (short)GameType.Value, (short)TableType);
        }

        private async void TagHand(object obj)
        {
            await Task.Run(() =>
            {
                using (var readToken = _readerWriterLock.Read())
                {
                    EnumHandTag tag = EnumHandTag.None;
                    if (obj == null || !Enum.TryParse(obj.ToString(), out tag))
                    {
                        return;
                    }

                    HudNamedPipeBindingService.TagHand(_gameNumber, _pokerSiteId, (int)tag);
                }
            });
        }

        private async void ExportHand(object obj)
        {
            EnumExportType exportType = EnumExportType.Raw;

            if (obj == null || !Enum.TryParse(obj.ToString(), out exportType))
            {
                return;
            }

            await Task.Run(() =>
            {
                using (var readToken = _readerWriterLock.Read())
                {
                    ExportFunctions.ExportHand(_gameNumber, _pokerSiteId, exportType, true);
                    RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
                }
            });
        }

        private async void ReplayLastHand(object obj)
        {
            await Task.Run(() =>
            {
                using (var readToken = _readerWriterLock.Read())
                {
                    HudNamedPipeBindingService.RaiseReplayHand(_gameNumber, _pokerSiteId);
                }
            });
        }

        private async void LoadLayout(object obj)
        {
            await Task.Run(() =>
            {
                using (var readToken = _readerWriterLock.Read())
                {
                    var layoutName = obj?.ToString();
                    if (string.IsNullOrWhiteSpace(layoutName))
                    {
                        return;
                    }

                    HudNamedPipeBindingService.LoadLayout(layoutName, _pokerSiteId, (short) GameType.Value,
                        (short) TableType);
                    RaiseNotification($"HUD with name \"{layoutName}\" will be loaded on the next hand.", "Load HUD");
                }
            });
        }

        private void RaiseNotification(string content, string title)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                this.NotificationRequest.Raise(
                        new PopupActionNotification
                        {
                            Content = content,
                            Title = title,
                        });
            });
        }

        #endregion
    }
}
