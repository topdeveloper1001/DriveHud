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
        private ReaderWriterLockSlim readerWriterLock;
        private long gameNumber;
        private EnumPokerSites pokerSite;
        private HudLayout layout;

        internal HudWindowViewModel()
        {
            readerWriterLock = new ReaderWriterLockSlim();

            NotificationRequest = new InteractionRequest<INotification>();

            layoutsCollection = new ObservableCollection<string>();

            ExportHandCommand = new RelayCommand(ExportHand);
            TagHandCommand = new RelayCommand(TagHand);
            ReplayLastHandCommand = new RelayCommand(ReplayLastHand);
            LoadLayoutCommand = new RelayCommand(LoadLayout);
        }

        #region Properties

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        public EnumGameType? GameType
        {
            get
            {
                return layout?.GameType;
            }
        }

        public EnumTableType? TableType
        {
            get
            {
                return layout?.TableType;
            }
        }

        private string layoutName;

        public string LayoutName
        {
            get
            {
                return layoutName;
            }
            set
            {
                SetProperty(ref layoutName, value);
            }
        }

        private string selectedLayout;

        public string SelectedLayout
        {
            get
            {
                return selectedLayout;
            }
            set
            {
                SetProperty(ref selectedLayout, value);
            }
        }

        private ObservableCollection<string> layoutsCollection;

        public ObservableCollection<string> LayoutsCollection
        {
            get { return layoutsCollection; }
            set { SetProperty(ref layoutsCollection, value); }
        }

        #endregion

        #region ICommand

        public ICommand ExportHandCommand { get; private set; }

        public ICommand TagHandCommand { get; private set; }

        public ICommand ReplayLastHandCommand { get; private set; }

        public ICommand LoadLayoutCommand { get; private set; }        

        #endregion

        #region Methods

        internal void SetLayout(HudLayout layout)
        {
            using (var write = readerWriterLock.Write())
            {
                if (layout != null)
                {
                    this.layout = layout;
                    LayoutName = layout.LayoutName;

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
                using (var readToken = readerWriterLock.Read())
                {
                    EnumHandTag tag = EnumHandTag.None;

                    if (obj == null || !Enum.TryParse(obj.ToString(), out tag))
                    {
                        return;
                    }

                    LogProvider.Log.Info($"Tagging hand {gameNumber} [{pokerSite}]");

                    HudNamedPipeBindingService.TagHand(gameNumber, (short)pokerSite, (int)tag);
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
                using (var readToken = readerWriterLock.Read())
                {
                    LogProvider.Log.Info($"Exporting hand {gameNumber}, {exportType} [{pokerSite}]");

                    ExportFunctions.ExportHand(gameNumber, (short)pokerSite, exportType, true);
                    RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
                }
            });
        }

        private async void ReplayLastHand(object obj)
        {
            await Task.Run(() =>
            {
                using (var readToken = readerWriterLock.Read())
                {
                    LogProvider.Log.Info($"Replaying hand {gameNumber} [{pokerSite}]");

                    HudNamedPipeBindingService.RaiseReplayHand(gameNumber, (short)pokerSite);
                }
            });
        }

        private async void LoadLayout(object obj)
        {
            await Task.Run(() =>
            {
                using (var readToken = readerWriterLock.Read())
                {
                    var layoutName = obj?.ToString();

                    if (string.IsNullOrWhiteSpace(layoutName))
                    {
                        return;
                    }

                    LayoutName = layoutName;

                    HudNamedPipeBindingService.LoadLayout(layoutName, pokerSite, GameType.Value, TableType.Value);
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
