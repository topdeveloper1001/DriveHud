using DriveHUD.Common.Extensions;
using DriveHUD.Common.Ifrastructure;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Entities;
using DriveHUD.HUD.Service;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Interfaces;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Threading;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudWindowViewModel : BaseViewModel
    {
        private ReaderWriterLockSlim _readerWriterLock;
        private IDataService _dataService;
        private long _gameNumber;
        private short _pokerSiteId;

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        internal HudWindowViewModel()
        {
            _readerWriterLock = new ReaderWriterLockSlim();
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();

            NotificationRequest = new InteractionRequest<INotification>();

            ExportHandCommand = new RelayCommand(ExportHand);
            TagHandCommand = new RelayCommand(TagHand);
            ReplayLastHandCommand = new RelayCommand(ReplayLastHand);
        }

        #region ICommand

        public ICommand ExportHandCommand { get; set; }
        public ICommand TagHandCommand { get; set; }
        public ICommand ReplayLastHandCommand { get; set; }

        #endregion

        #region Methods

        internal void SetLayout(HudLayout layout)
        {
            using (var write = _readerWriterLock.Write())
            {
                if (layout != null)
                {
                    _gameNumber = layout.GameNumber;
                    _pokerSiteId = layout.PokerSiteId;
                }
            }
        }

        internal void SaveHudPositions(HudLayoutContract hudTable)
        {
            if (hudTable == null)
            {
                return;
            }

            HudNamedPipeBindingService.RaiseSaveHudLayout(hudTable);
        }

        private void TagHand(object obj)
        {
            EnumHandTag tag = EnumHandTag.None;
            if (obj == null || !Enum.TryParse(obj.ToString(), out tag))
            {
                return;
            }

            using (var readToken = _readerWriterLock.Read())
            {
                var handNote = _dataService.GetHandNote(_gameNumber, _pokerSiteId);
                if (handNote == null)
                {
                    handNote = new Handnotes()
                    {
                        Gamenumber = _gameNumber,
                        PokersiteId = _pokerSiteId
                    };
                }
                handNote.CategoryId = (int)tag;
                _dataService.Store(handNote);
            }
        }

        private void ExportHand(object obj)
        {
            EnumExportType exportType = EnumExportType.Raw;

            if (obj == null || !Enum.TryParse(obj.ToString(), out exportType))
            {
                return;
            }

            using (var readToken = _readerWriterLock.Read())
            {
                ExportFunctions.ExportHand(_gameNumber, _pokerSiteId, exportType, true);
                RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
            }
        }

        private void ReplayLastHand(object obj)
        {
            using (var readToken = _readerWriterLock.Read())
            {
                HudNamedPipeBindingService.RaiseReplayHand(_gameNumber, _pokerSiteId);
            }
        }

        private void RaiseNotification(string content, string title)
        {
            this.NotificationRequest.Raise(
                    new PopupActionNotification
                    {
                        Content = content,
                        Title = title,
                    },
                    n => { });
        }

        #endregion
    }
}
