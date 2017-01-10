//-----------------------------------------------------------------------
// <copyright file="HudTableViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Infrastructure.Base;
using ProtoBuf;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DriveHUD.Entities;
using Model.Enums;

namespace DriveHUD.Application.ViewModels
{
    [ProtoContract]
    public class HudTableViewModel : BaseViewModel
    {
        [ProtoMember(1)]
        public EnumPokerSites PokerSite { get; set; }

        [ProtoMember(2)]
        public EnumTableType TableType { get; set; }

        [ProtoMember(3)]
        public bool IsRelativePosition { get; set; }

        [ProtoMember(4)]
        public Point RelativePosition { get; set; }

        [ProtoMember(5)]
        public Point StartPosition { get; set; }
        
        public ObservableCollection<HudElementViewModel> HudElements { get; set; }
        
        internal ObservableCollection<ITableSeatArea> TableSeatAreaCollection { get; set; }
        
        public double ShiftX
        {
            get { return (IsRelativePosition ? -RelativePosition.X : 0) + StartPosition.X; }
        }

        public double ShiftY
        {
            get { return (IsRelativePosition ? -RelativePosition.Y : 0) + StartPosition.Y; }
        }

        public HudTableViewModel Clone()
        {
            var model = new HudTableViewModel();

            model.PokerSite = PokerSite;
            model.TableType = TableType;
            model.IsRelativePosition = IsRelativePosition;
            model.RelativePosition = RelativePosition;
            model.StartPosition = StartPosition;
            model.HudElements = new ObservableCollection<HudElementViewModel>(HudElements.Select(x => x.Clone()));
            model.TableSeatAreaCollection = new ObservableCollection<ITableSeatArea>();

            return model;
        }
    }
}