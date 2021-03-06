﻿//-----------------------------------------------------------------------
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

using DriveHUD.Application.TableConfigurators.PositionProviders;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Ifrastructure;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Entities;
using DriveHUD.HUD.Service;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudWindowViewModel : BaseViewModel
    {
        private ReaderWriterLockSlim readerWriterLock;
        private HudLayout layout;
        private Dictionary<HudToolKey, Point> panelOffsets;
        private readonly Dispatcher dispatcher;
        private FixedSizeList<long> lastHands;

        public HudWindowViewModel(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;

            readerWriterLock = new ReaderWriterLockSlim();

            NotificationRequest = new InteractionRequest<INotification>();

            layoutsCollection = new ObservableCollection<string>();

            ExportHandCommand = new RelayCommand(ExportHand);
            TagHandCommand = new RelayCommand(TagHand);
            TagLastHandsCommand = new RelayCommand(TagLastHands);
            ReplayLastHandCommand = new RelayCommand(ReplayLastHand);
            LoadLayoutCommand = new RelayCommand(LoadLayout);
            ApplyPositionsCommand = new RelayCommand(ApplyPositions);
            TreatAsCommand = new RelayCommand(DoTreatAs);
            RotateHUDToRightCommand = new RelayCommand(() => RotateHud(true));
            RotateHUDToLeftCommand = new RelayCommand(() => RotateHud(false));

            panelOffsets = new Dictionary<HudToolKey, Point>();

            var tableTypes = Enum.GetValues(typeof(EnumTableType)).Cast<byte>().ToArray();

            treatAs = new ObservableCollection<byte>(tableTypes);
            lastHands = new FixedSizeList<long>(3);
        }

        #region Properties

        private IntPtr windowHandle;

        public IntPtr WindowHandle
        {
            get
            {
                return windowHandle;
            }
            set
            {
                SetProperty(ref windowHandle, value);
            }
        }

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        public EnumGameType? GameType
        {
            get
            {
                return layout?.GameType;
            }
        }

        private EnumTableType? tableType;

        public EnumTableType? TableType
        {
            get
            {
                return tableType.HasValue ? tableType.Value : layout?.TableType;
            }
            private set
            {
                tableType = value;
            }
        }

        public bool PreLoadMode
        {
            get
            {
                return layout?.PreloadMode ?? false;
            }
        }

        public string PreLoadText
        {
            get
            {
                return layout?.PreloadText;
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

        private ObservableCollection<byte> treatAs;

        public ObservableCollection<byte> TreatAs
        {
            get
            {
                return treatAs;
            }
        }

        public ObservableCollection<int> Seats
        {
            get
            {
                return layout != null ? new ObservableCollection<int>(Enumerable.Range(1, (int)layout.TableType)) : null;
            }
        }

        internal Dictionary<HudToolKey, Point> PanelOffsets
        {
            get
            {
                return panelOffsets;
            }
        }

        public Action RefreshHud
        {
            get;
            set;
        }

        #endregion

        #region ICommand

        public ICommand ExportHandCommand { get; private set; }

        public ICommand TagHandCommand { get; private set; }

        public ICommand TagLastHandsCommand { get; private set; }

        public ICommand ReplayLastHandCommand { get; private set; }

        public ICommand LoadLayoutCommand { get; private set; }

        public ICommand ApplyPositionsCommand { get; private set; }

        public ICommand TreatAsCommand { get; private set; }

        public ICommand RotateHUDToRightCommand { get; private set; }

        public ICommand RotateHUDToLeftCommand { get; private set; }

        #endregion

        #region Methods

        internal Point GetPanelOffset(HudBaseToolViewModel toolViewModel)
        {
            var toolKey = HudToolKey.BuildKey(toolViewModel);

            if (toolViewModel != null && panelOffsets.ContainsKey(toolKey))
            {
                return panelOffsets[toolKey];
            }

            return new Point(0, 0);
        }

        internal void UpdatePanelOffset(HudBaseToolViewModel toolViewModel)
        {
            var toolKey = HudToolKey.BuildKey(toolViewModel);

            if (toolViewModel != null &&
                (toolViewModel.OffsetX.HasValue || toolViewModel.OffsetY.HasValue))
            {
                panelOffsets[toolKey] = new Point(toolViewModel.OffsetX ?? 0, toolViewModel.OffsetY ?? 0);
            }
        }

        internal void SetLayout(HudLayout layout)
        {
            using (var write = readerWriterLock.Write())
            {
                if (layout != null)
                {
                    if (TableType.HasValue && TableType != layout.TableType)
                    {
                        PanelOffsets.Clear();
                    }

                    this.layout = layout;
                    LayoutName = layout.LayoutName;

                    if (!lastHands.Contains(layout.GameNumber))
                    {
                        lastHands.Add(layout.GameNumber);
                    }

                    if (layout.AvailableLayouts != null)
                    {
                        LayoutsCollection = new ObservableCollection<string>(layout.AvailableLayouts);
                        SelectedLayout = LayoutsCollection.FirstOrDefault(x => x == layout.LayoutName);
                    }

                    RaisePropertyChanged(nameof(PreLoadMode));
                    RaisePropertyChanged(nameof(PreLoadText));
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
            if (layout == null || obj == null)
            {
                return;
            }

            await Task.Run(() => TagHands(new[] { layout.GameNumber }, obj));
        }

        private async void TagLastHands(object obj)
        {
            if (layout == null || obj == null)
            {
                return;
            }

            await Task.Run(() => TagHands(lastHands, obj));
        }

        private void TagHands(IEnumerable<long> handNumbers, object handTagObject)
        {
            using (var readToken = readerWriterLock.Read())
            {
                EnumHandTag tag = EnumHandTag.None;

                if (!Enum.TryParse(handTagObject.ToString(), out tag))
                {
                    return;
                }

                LogProvider.Log.Info($"Tagging hand(s) {string.Join(", ", handNumbers)} for {handTagObject} [{layout.PokerSite}]");

                HudNamedPipeBindingService.TagHands(handNumbers, (short)layout.PokerSite, (int)tag);
            }
        }

        private async void ExportHand(object obj)
        {
            if (layout == null)
            {
                return;
            }

            EnumExportType exportType = EnumExportType.Raw;

            if (obj == null || !Enum.TryParse(obj.ToString(), out exportType))
            {
                return;
            }

            await Task.Run(() =>
            {
                using (var readToken = readerWriterLock.Read())
                {
                    LogProvider.Log.Info($"Exporting hand {layout.GameNumber}, {exportType} [{layout.PokerSite}]");

                    ExportFunctions.ExportHand(layout.GameNumber, (short)layout.PokerSite, exportType, true);
                    RaiseNotification(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString), "Hand Export");
                }
            });
        }

        private async void ReplayLastHand(object obj)
        {
            if (layout == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                using (var readToken = readerWriterLock.Read())
                {
                    LogProvider.Log.Info($"Replaying hand {layout.GameNumber} [{layout.PokerSite}]");

                    HudNamedPipeBindingService.RaiseReplayHand(layout.GameNumber, (short)layout.PokerSite);
                }
            });
        }

        private async void LoadLayout(object obj)
        {
            if (layout == null)
            {
                return;
            }

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

                    HudNamedPipeBindingService.LoadLayout(layoutName, layout.PokerSite, GameType.Value, TableType.Value);
                    RaiseNotification($"HUD with name \"{layoutName}\" will be loaded on the next hand.", "Load HUD");
                }
            });
        }

        private void ApplyPositions(object obj)
        {
            var sourceSeat = obj as int?;

            if (!sourceSeat.HasValue || layout == null)
            {
                return;
            }

            var positionProvider = ServiceLocator.Current.GetInstance<IPositionProvider>(layout.PokerSite.ToString());

            if (!positionProvider.Positions.ContainsKey((int)TableType))
            {
                return;
            }

            var seatsPositions = positionProvider.Positions[(int)TableType];

            var baseHUDPlayer = layout.ListHUDPlayer.FirstOrDefault(x => x.SeatNumber == sourceSeat.Value);

            if (baseHUDPlayer == null)
            {
                return;
            }

            var baseSeatPosition = new Point(seatsPositions[sourceSeat.Value - 1, 0], seatsPositions[sourceSeat.Value - 1, 1]);

            var nonPopupToolViewModels = baseHUDPlayer.HudElement.Tools.OfType<IHudNonPopupToolViewModel>();

            var toolOffsetsDictionary = (from nonPopupToolViewModel in nonPopupToolViewModels
                                         let toolOffsetX = nonPopupToolViewModel.OffsetX ?? nonPopupToolViewModel.Position.X
                                         let toolOffsetY = nonPopupToolViewModel.OffsetY ?? nonPopupToolViewModel.Position.Y
                                         let shiftX = toolOffsetX - baseSeatPosition.X
                                         let shiftY = toolOffsetY - baseSeatPosition.Y
                                         select new { ToolId = nonPopupToolViewModel.Id, ShiftX = shiftX, ShiftY = shiftY }).ToDictionary(x => x.ToolId);

            for (var seat = 1; seat <= (int)TableType; seat++)
            {
                if (seat == sourceSeat.Value)
                {
                    continue;
                }

                foreach (var tool in toolOffsetsDictionary.Values)
                {
                    var toolKey = new HudToolKey
                    {
                        Id = tool.ToolId,
                        Seat = seat
                    };

                    var seatPosition = new Point(seatsPositions[seat - 1, 0], seatsPositions[seat - 1, 1]);

                    var offsetX = seatPosition.X + tool.ShiftX;
                    var offsetY = seatPosition.Y + tool.ShiftY;

                    PanelOffsets[toolKey] = new Point(offsetX, offsetY);
                }
            }

            var elementsToUpdate = layout.ListHUDPlayer.Where(x => x.SeatNumber != sourceSeat.Value).Select(x => x.HudElement).ToArray();

            elementsToUpdate.ForEach(element =>
            {
                element.Tools.OfType<IHudNonPopupToolViewModel>().ForEach(tool =>
                {
                    if (toolOffsetsDictionary.ContainsKey(tool.Id))
                    {
                        var seatPosition = new Point(seatsPositions[element.Seat - 1, 0], seatsPositions[element.Seat - 1, 1]);

                        tool.OffsetX = seatPosition.X + toolOffsetsDictionary[tool.Id].ShiftX;
                        tool.OffsetY = seatPosition.Y + toolOffsetsDictionary[tool.Id].ShiftY;
                    }
                });
            });

            RefreshHud?.Invoke();
        }

        private void RotateHud(bool clockwise)
        {
            if (!TableType.HasValue)
            {
                return;
            }

            try
            {
                var tableSize = (int)TableType;

                var toolsBySeats = layout.ListHUDPlayer
                   .SelectMany(x => x.HudElement.Tools)
                   .OfType<IHudNonPopupToolViewModel>()
                   .Concat(layout.EmptySeatsViewModels
                       .SelectMany(x => x.Tools)
                       .OfType<IHudNonPopupToolViewModel>())
                   .GroupBy(x => x.Parent.Seat)
                   .ToDictionary(x => x.Key, x => x.ToArray());

                var toolsById = toolsBySeats.Values
                   .SelectMany(x => x)
                   .GroupBy(x => x.Id, x => new
                   {
                       OffsetX = x.OffsetX ?? x.Position.X,
                       OffsetY = x.OffsetY ?? x.Position.Y,
                       x.Parent.Seat
                   })
                   .ToDictionary(x => x.Key, x => x.GroupBy(p => p.Seat).ToDictionary(p => p.Key, p => p.FirstOrDefault()));             

                // need to rotate all positions even if there is no player on position
                for (var seat = 1; seat <= tableSize; seat++)
                {
                    var newSeat = clockwise ? seat + 1 : seat - 1;

                    if (newSeat > tableSize)
                    {
                        newSeat = 1;
                    }
                    else if (newSeat < 1)
                    {
                        newSeat = tableSize;
                    }

                    var nonPopupTools = toolsBySeats[seat];

                    foreach (var nonPopupTool in nonPopupTools)
                    {
                        if (!toolsById.ContainsKey(nonPopupTool.Id) ||
                            !toolsById[nonPopupTool.Id].ContainsKey(newSeat))
                        {
                            continue;
                        }

                        var newOffsets = toolsById[nonPopupTool.Id][newSeat];

                        if (newOffsets == null)
                        {
                            continue;
                        }

                        nonPopupTool.OffsetX = newOffsets.OffsetX;
                        nonPopupTool.OffsetY = newOffsets.OffsetY;

                        var toolKey = new HudToolKey
                        {
                            Id = nonPopupTool.Id,
                            Seat = seat
                        };

                        if (!PanelOffsets.ContainsKey(toolKey))
                        {
                            PanelOffsets.Add(toolKey, new Point(newOffsets.OffsetX, newOffsets.OffsetY));
                        }
                        else
                        {
                            PanelOffsets[toolKey] = new Point(newOffsets.OffsetX, newOffsets.OffsetY);
                        }
                    }
                }             
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to rotate hud for table. [{WindowHandle}]", e);
            }

            RefreshHud?.Invoke();
        }

        private void RaiseNotification(string content, string title)
        {
            dispatcher.Invoke(() =>
            {
                NotificationRequest.Raise(
                        new PopupActionNotification
                        {
                            Content = content,
                            Title = title
                        });
            });
        }

        private async void DoTreatAs(object obj)
        {
            if (obj == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                using (var readToken = readerWriterLock.Read())
                {
                    var tableType = (EnumTableType)obj;
                    TableType = tableType;

                    PanelOffsets.Clear();

                    HudNamedPipeBindingService.TreatTableAs(WindowHandle, tableType);
                }
            });
        }

        #endregion

        internal class HudToolKey
        {
            public Guid Id { get; set; }

            public int Seat { get; set; }

            public static HudToolKey BuildKey(HudBaseToolViewModel toolViewModel)
            {
                if (toolViewModel == null)
                {
                    return null;
                }

                var seat = toolViewModel.Parent != null ? toolViewModel.Parent.Seat : 1;

                var toolKey = new HudToolKey
                {
                    Id = toolViewModel.Id,
                    Seat = seat
                };

                return toolKey;
            }

            public override int GetHashCode()
            {
                var hash = 23;
                hash = hash * 31 + Id.GetHashCode();
                hash = hash * 31 + Seat;

                return hash;
            }

            public override bool Equals(object obj)
            {
                var toolKey = obj as HudToolKey;

                if (toolKey == null)
                {
                    return false;
                }

                return toolKey.Id == Id && toolKey.Seat == Seat;
            }
        }
    }
}
