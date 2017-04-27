//-----------------------------------------------------------------------
// <copyright file="HudLayoutGraphTool.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using Model.Stats;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents the graph tool of the hud
    /// </summary>
    [Serializable, ProtoContract]
    public class HudLayoutGraphTool : HudLayoutTool, IHudLayoutStats
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HudLayoutGraphTool" />
        /// </summary>
        public HudLayoutGraphTool() : base()
        {
            ToolType = HudDesignerToolType.Graph;
            Stats = new ReactiveList<StatInfo>();
        }

        /// <summary>
        /// Gets or sets the main stat of graph tool
        /// </summary>
        [ProtoMember(2)]
        public StatInfo BaseStat { get; set; }

        [ProtoMember(3)]
        /// <summary>
        /// Gets or sets the list of <see cref="StatInfo"/> stats of the gauge indicator
        /// </summary>
        public ReactiveList<StatInfo> Stats { get; set; }

        /// <summary>
        /// Gets or sets whenever indicator is vertical 
        /// </summary>
        [ProtoMember(4)]
        public bool IsVertical { get; set; }

        /// <summary>
        /// Gets or sets the parent of the graph tool 
        /// </summary>
        [ProtoMember(5)]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Gets or sets the position info of gauge indicator
        /// </summary>        
        public HudPositionInfo PositionInfo { get; set; }

        public override HudLayoutTool Clone()
        {
            var cloned = new HudLayoutGraphTool
            {
                Id = Id,
                BaseStat = BaseStat,
                IsVertical = IsVertical,
                ParentId = ParentId,
                PositionInfo = PositionInfo?.Clone(),
                Stats = new ReactiveList<StatInfo>(Stats.Select(x =>
                {
                    var statInfoBreak = x as StatInfoBreak;

                    if (statInfoBreak != null)
                    {
                        return statInfoBreak.Clone();
                    }

                    return x.Clone();
                }))
            };

            return cloned;
        }

        public override HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement)
        {
            var toolViewModel = new HudGraphViewModel(this, hudElement)
            {
                Width = HudDefaultSettings.GraphWidth,
                Height = HudDefaultSettings.GraphHeight
            };

            return toolViewModel;
        }
    }
}