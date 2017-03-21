//-----------------------------------------------------------------------
// <copyright file="HudBaseToolViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudPlainStatBoxViewModel : HudBaseToolViewModel
    {
        public HudPlainStatBoxViewModel()
        {
        }

        public HudPlainStatBoxViewModel(HudElementViewModel parent)
        {
            this.parent = parent;
        }

        #region Properties

        public override HudDesignerToolType ToolType
        {
            get
            {
                return HudDesignerToolType.PlainStatBox;
            }
        }

        private ObservableCollection<StatInfo> stats;

        public ObservableCollection<StatInfo> Stats
        {
            get
            {
                return stats;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stats, value);
            }
        }

        private HudElementViewModel parent;

        public HudElementViewModel Parent
        {
            get
            {
                return parent;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref parent, value);
            }
        }

        #endregion

    }
}