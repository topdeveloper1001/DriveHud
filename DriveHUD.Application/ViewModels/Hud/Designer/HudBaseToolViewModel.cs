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

using DriveHUD.Common.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    public abstract class HudBaseToolViewModel : ViewModelBase
    {
        #region Properties

        private double width;

        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref width, value);
            }
        }

        private double height;

        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref height, value);
            }
        }

        private Point position;

        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref position, value);
            }
        }

        private double opacity;

        public double Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref opacity, value);
            }
        }

        public abstract HudDesignerToolType ToolType
        {
            get;
        }

        #endregion
    }
}