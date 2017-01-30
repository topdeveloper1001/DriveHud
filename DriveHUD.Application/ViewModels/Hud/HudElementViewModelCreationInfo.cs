﻿//-----------------------------------------------------------------------
// <copyright file="HudElementViewModelCreationInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudElementViewModelCreationInfo
    {        
        public HudLayoutInfo HudLayoutInfo { get; set; }

        public IEnumerable<HudElementViewModel> PreparedHudElements { get; set; }

        public int SeatNumber { get; set; }

        public EnumPokerSites PokerSite { get; set; }

        public EnumGameType GameType { get; set; }
    }
}