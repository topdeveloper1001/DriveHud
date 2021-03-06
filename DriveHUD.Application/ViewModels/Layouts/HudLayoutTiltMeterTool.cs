﻿//-----------------------------------------------------------------------
// <copyright file="HudLayoutTiltMeter.cs" company="Ace Poker Solutions">
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
    /// This class represent the tilt meter tool of the hud
    /// </summary>
    [Serializable, ProtoContract]
    public class HudLayoutTiltMeterTool : HudLayoutNonPopupTool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HudLayoutTiltMeterTool"/> class
        /// </summary>
        public HudLayoutTiltMeterTool() : base()
        {
            ToolType = HudDesignerToolType.TiltMeter;
        }

        /// <summary>
        /// Creates a copy of the current <see cref="HudLayoutTiltMeterTool"/> instance
        /// </summary>
        /// <returns>Copy of the current <see cref="HudLayoutTool"/> instance</returns>
        public override HudLayoutTool Clone()
        {
            var clone = new HudLayoutTiltMeterTool
            {
                Id = Id,
                Positions = Positions?.Select(x => x.Clone()).ToList(),
                UIPositions = UIPositions.Select(x => x.Clone()).ToList()
            };

            return clone;
        }

        /// <summary>
        /// Creates a view model of the current <see cref="HudLayoutTiltMeterTool"/> instance
        /// </summary>
        /// <returns>View model of the current <see cref="HudBaseToolViewModel"/> instance</returns>
        public override HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement)
        {
            var toolViewModel = new HudTiltMeterViewModel(this, hudElement);
            return toolViewModel;
        }
    }
}