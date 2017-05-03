//-----------------------------------------------------------------------
// <copyright file="HudFourStatsBoxViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common;
using DriveHUD.Common.Resources;
using Model.Stats;
using ProtoBuf;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Represents view model of 4-stat box
    /// </summary>
    [ProtoContract]
    public class HudFourStatsBoxViewModel : HudPlainStatBoxViewModel
    {
        /// <summary>
        /// Initializes an instance of <see cref="HudFourStatsBoxViewModel"/>
        /// </summary>
        protected HudFourStatsBoxViewModel()
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudFourStatsBoxViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutFourStatsBoxTool"/> to initialize an instance</param>
        protected HudFourStatsBoxViewModel(HudLayoutFourStatsBoxTool tool)
        {
            Check.ArgumentNotNull(() => tool);

            this.tool = tool;

            if (Stats != null)
            {
                Stats.Changed.Subscribe(x => RefreshStats());
            }
        }

        /// <summary>
        ///  Initialize an instance of <see cref="HudFourStatsBoxViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutFourStatsBoxTool"/> to initialize an instance</param>
        /// <param name="parent">Parent <see cref="HudElementViewModel"/> to initialize an instance</param>
        public HudFourStatsBoxViewModel(HudLayoutFourStatsBoxTool tool, HudElementViewModel parent) : this(tool)
        {
            Check.ArgumentNotNull(() => parent);

            Parent = parent;

            RotateCommand = ReactiveCommand.Create();
            (RotateCommand as ReactiveCommand<object>).Subscribe(x =>
            {
                IsVertical = !IsVertical;

                if (IsVertical)
                {                 
                    Height = HudDefaultSettings.FourStatVerticalBoxHeight;
                    Width = HudDefaultSettings.FourStatVerticalBoxWidth;
                }
                else
                {
                    Height = HudDefaultSettings.FourStatBoxHeight;
                    Width = HudDefaultSettings.FourStatBoxWidth;
                }
            });
        }

        /// <summary>
        /// Gets the default width of the tool
        /// </summary>
        public override double DefaultWidth
        {
            get
            {
                return HudDefaultSettings.FourStatBoxWidth;
            }
        }

        /// <summary>
        /// Gets the default height of the tool
        /// </summary>
        public override double DefaultHeight
        {
            get
            {
                return HudDefaultSettings.FourStatBoxHeight;
            }
        }

        /// <summary>
        /// Gets the first <see cref="StatInfo"/> of stats collection
        /// </summary>
        public StatInfo Stat1
        {
            get
            {
                return Stats.Count > 0 ? Stats[0] : null;
            }
        }

        /// <summary>
        /// Gets the second <see cref="StatInfo"/> of stats collection
        /// </summary>
        public StatInfo Stat2
        {
            get
            {
                return Stats.Count > 1 ? Stats[1] : null;
            }
        }

        /// <summary>
        /// Gets the third <see cref="StatInfo"/> of stats collection
        /// </summary>
        public StatInfo Stat3
        {
            get
            {
                return Stats.Count > 2 ? Stats[2] : null;
            }
        }

        /// <summary>
        /// Gets the fourth <see cref="StatInfo"/> of stats collection
        /// </summary>
        public StatInfo Stat4
        {
            get
            {
                return Stats.Count > 3 ? Stats[3] : null;
            }
        }

        /// <summary>
        /// Gets or sets whenever four stats box is vertical
        /// </summary>
        public bool IsVertical
        {
            get
            {
                var tool = this.tool as HudLayoutFourStatsBoxTool;
                return tool != null && tool.IsVertical;
            }
            set
            {
                var tool = this.tool as HudLayoutFourStatsBoxTool;

                if (tool == null)
                {
                    return;
                }

                tool.IsVertical = value;

                this.RaisePropertyChanged(nameof(IsVertical));
            }
        }

        /// <summary>
        /// Gets whenever rotate button is visible
        /// </summary>
        public override bool IsRotateVisible
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Raise property change event on each of four stats
        /// </summary>
        private void RefreshStats()
        {
            this.RaisePropertyChanged(nameof(Stat1));
            this.RaisePropertyChanged(nameof(Stat2));
            this.RaisePropertyChanged(nameof(Stat3));
            this.RaisePropertyChanged(nameof(Stat4));
        }
    }
}