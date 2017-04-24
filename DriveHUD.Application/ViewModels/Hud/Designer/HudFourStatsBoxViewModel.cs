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
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud.Designer
{
    /// <summary>
    /// Represents view model of 4-stat box
    /// </summary>
    [ProtoContract]
    public class HudFourStatsBoxViewModel : HudPlainStatBoxViewModel
    {
        [ProtoMember(5)]
        private HudLayoutFourStatsBoxTool tool;

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
    }
}