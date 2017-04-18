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
using Model.Enums;
using ProtoBuf;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents the gauge indicator tool of the hud
    /// </summary>
    [Serializable, ProtoContract]    
    public class HudLayoutGaugeIndicator : HudLayoutTool
    {
        public HudLayoutGaugeIndicator() : base()
        {
            ToolType = HudDesignerToolType.GaugeIndicator;
        }

        [ProtoMember(2)]
        /// <summary>
        /// Gets or sets the main stat to which indicator is attached
        /// </summary>
        public Stat BaseStat { get; set; }

        [ProtoMember(3)]
        /// <summary>
        /// Gets or sets the list of <see cref="Stat"/> stats of the gauge indicator
        /// </summary>
        public ReactiveList<Stat> Stats { get; set; }

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

        public override HudLayoutTool Clone()
        {
            var cloned = new HudLayoutGaugeIndicator
            {
                Id = Id,
                BaseStat = BaseStat,
                Stats = new ReactiveList<Stat>(Stats),
                IsVertical = IsVertical,
                Text = Text
            };

            return cloned;
        }

        public override HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement)
        {
            throw new NotImplementedException();
        }
    }
}
