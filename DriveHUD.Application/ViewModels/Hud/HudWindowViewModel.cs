//-----------------------------------------------------------------------
// <copyright file="HudWindowViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Ifrastructure;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
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
        private EnumPokerSites _pokerSite;

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

        private EnumGameType? gameType;
        public EnumGameType? GameType
        {
            get
            {
                return gameType;
            }
            set
            {
                SetProperty(ref gameType, value);
            }
        }

        private EnumTableType tableType;
        public EnumTableType TableType
        {
            get
            {
                return tableType;
            }
            set
            {
                SetProperty(ref tableType, value);
            }
        }

        private string _layoutName;
        public string LayoutName
        {
            get { return _layoutName; }
            set { SetProperty(ref _layoutName, value); }
        }


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
                    _pokerSite = layout.PokerSite;

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

            LogProvider.Log.Info($"Saving hud positions '{hudTable.LayoutName}' '{hudTable.TableType}' '{hudTable.GameType}' [{hudTable.PokerSite}]");

            HudNamedPipeBindingService.RaiseSaveHudLayout(hudTable);
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

                    LogProvider.Log.Info($"Tagging hand {_gameNumber} [{_pokerSite}]");

                    HudNamedPipeBindingService.TagHand(_gameNumber, (short)_pokerSite, (int)tag);
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
                    LogProvider.Log.Info($"Exporting hand {_gameNumber}, {exportType} [{_pokerSite}]");

                    ExportFunctions.ExportHand(_gameNumber, (short)_pokerSite, exportType, true);
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
                    LogProvider.Log.Info($"Replaying hand {_gameNumber} [{_pokerSite}]");

                    HudNamedPipeBindingService.RaiseReplayHand(_gameNumber, (short)_pokerSite);
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

                    LayoutName = layoutName;

                    HudNamedPipeBindingService.LoadLayout(layoutName, _pokerSite, GameType.Value, TableType);
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
                            Title = title
                        });
            });
        }

        #endregion
    }
}
