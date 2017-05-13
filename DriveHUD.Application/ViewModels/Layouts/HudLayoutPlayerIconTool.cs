//-----------------------------------------------------------------------
// <copyright file="HudLayoutPlayerIconTool.cs" company="Ace Poker Solutions">
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
using ProtoBuf;
using System;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represent the player icon tool of the hud
    /// </summary>
    [Serializable, ProtoContract]
    public class HudLayoutPlayerIconTool : HudLayoutNonPopupTool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HudLayoutTiltMeterTool"/> class
        /// </summary>
        public HudLayoutPlayerIconTool() : base()
        {
            ToolType = HudDesignerToolType.PlayerProfileIcon;
        }

        /// <summary>
        /// Creates a copy of the current <see cref="HudLayoutPlayerIconTool"/> instance
        /// </summary>
        /// <returns>Copy of the current <see cref="HudLayoutTool"/> instance</returns>
        public override HudLayoutTool Clone()
        {
            var cloned = new HudLayoutPlayerIconTool
            {
                Id = Id,
                Positions = Positions?.Select(x => x.Clone()).ToList(),
                UIPositions = UIPositions.Select(x => x.Clone()).ToList()
            };

            return cloned;
        }

        /// <summary>
        /// Creates a view model of the current <see cref="HudLayoutTiltMeterTool"/> instance
        /// </summary>
        /// <returns>View model of the current <see cref="HudBaseToolViewModel"/> instance</returns>
        public override HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement)
        {
            var toolViewModel = new HudPlayerIconViewModel(this, hudElement);
            return toolViewModel;
        }
    }
}