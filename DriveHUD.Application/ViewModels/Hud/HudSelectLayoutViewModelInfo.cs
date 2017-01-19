//-----------------------------------------------------------------------
// <copyright file="HudSelectLayoutViewModelInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using System;

namespace DriveHUD.Application.ViewModels
{
    public class HudSelectLayoutViewModelInfo
    {
        public EnumTableType TableType { get; set; }
        public string LayoutName { get; set; }
        public Action Save { get; set; }
        public Action Cancel { get; set; }
        public bool IsSaveAsMode { get; set; }
        public bool IsDeleteMode { get; set; }
    }
}