﻿//-----------------------------------------------------------------------
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

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Entities;
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

        private double offsetX;

        public double OffsetX
        {
            get
            {
                return offsetX;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref offsetX, value);
            }
        }

        private double offsetY;

        public double OffsetY
        {
            get
            {
                return offsetY;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref offsetY, value);
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

        private HudElementViewModel parent;

        /// <summary>
        /// Gets or sets the parent <see cref="HudElementViewModel"/> of the current tool
        /// </summary>
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

        /// <summary>
        /// Sets UI position and size for the current <see cref="HudBaseToolViewModel"/>
        /// </summary>
        /// <exception cref="DHBusinessException" />
        public abstract void SetPositions();

        /// <summary>
        /// Sets position and size for the current <see cref="HudBaseToolViewModel"/> for the specified <see cref="EnumPokerSites"/> and <see cref="EnumGameType"/>
        /// </summary>
        /// <exception cref="DHBusinessException" />
        public abstract void SetPositions(EnumPokerSites pokerSite, EnumGameType gameType);
    }
}