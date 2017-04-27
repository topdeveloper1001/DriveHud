//-----------------------------------------------------------------------
// <copyright file="HudLayoutGaugeIndicator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Resources;
using Model.Stats;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents the gauge indicator tool of the hud
    /// </summary>
    [Serializable, ProtoContract]
    public class HudLayoutGaugeIndicator : HudLayoutTool, IHudLayoutStats
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HudLayoutGaugeIndicator" />
        /// </summary>
        public HudLayoutGaugeIndicator() : base()
        {
            ToolType = HudDesignerToolType.GaugeIndicator;
            Stats = new ReactiveList<StatInfo>();
        }

        [ProtoMember(2)]
        /// <summary>
        /// Gets or sets the main stat to which indicator is attached
        /// </summary>
        public StatInfo BaseStat { get; set; }

        [ProtoMember(3)]
        /// <summary>
        /// Gets or sets the list of <see cref="StatInfo"/> stats of the gauge indicator
        /// </summary>
        public ReactiveList<StatInfo> Stats { get; set; }

        [ProtoMember(4)]
        /// <summary>
        /// Gets or sets whenever indicator is vertical 
        /// </summary>
        public bool IsVertical { get; set; }

        [ProtoMember(5)]
        /// <summary>
        /// Gets or sets the text on gauge indicator
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the position info of gauge indicator
        /// </summary>        
        public HudPositionInfo PositionInfo { get; set; }

        public override HudLayoutTool Clone()
        {
            var cloned = new HudLayoutGaugeIndicator
            {
                Id = Id,
                BaseStat = BaseStat,
                Stats = new ReactiveList<StatInfo>(Stats.Select(x =>
                {
                    var statInfoBreak = x as StatInfoBreak;

                    if (statInfoBreak != null)
                    {
                        return statInfoBreak.Clone();
                    }

                    return x.Clone();
                })),
                IsVertical = IsVertical,
                Text = Text,
                PositionInfo = PositionInfo?.Clone()
            };

            return cloned;
        }

        public override HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement)
        {
            var toolViewModel = new HudGaugeIndicatorViewModel(this, hudElement);
            toolViewModel.Width = HudDefaultSettings.GaugeIndicatorWidth;
            toolViewModel.Height = HudDefaultSettings.GaugeIndicatorHeight;
            toolViewModel.Opacity = 100;
            toolViewModel.Stats.ForEach(x => x.StatInfoMeter = new StatInfoMeterModel());
            return toolViewModel;
        }
    }
}