//-----------------------------------------------------------------------
// <copyright file="HudLayoutHeatMapTool.cs" company="Ace Poker Solutions">
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
    /// This class represents the heat map tool of the hud
    /// </summary>
    [Serializable, ProtoContract]
    public class HudLayoutHeatMapTool : HudLayoutTool
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HudLayoutHeatMapTool"/> 
        /// </summary>
        public HudLayoutHeatMapTool() : base()
        {
            ToolType = HudDesignerToolType.HeatMap;
        }

        /// <summary>
        /// Gets or sets the main stat to which indicator is attached
        /// </summary>
        [ProtoMember(2)]
        public StatInfo BaseStat { get; set; }

        /// <summary>
        /// Gets or sets the position info of gauge indicator
        /// </summary>        
        public HudPositionInfo PositionInfo { get; set; }

        /// <summary>
        /// Clones the current <see cref="HudLayoutHeatMapTool" />
        /// </summary>
        /// <returns>Cloned <see cref="HudLayoutHeatMapTool" /></returns>
        public override HudLayoutTool Clone()
        {
            var cloned = new HudLayoutHeatMapTool
            {
                Id = Id,
                BaseStat = BaseStat?.Clone(),
                PositionInfo = PositionInfo?.Clone()
            };

            return cloned;
        }

        /// <summary>
        /// Creates the view model of the current <see cref="HudLayoutHeatMapTool" />
        /// </summary>
        /// <param name="hudElement"><see cref="HudElementViewModel"/> to create view model</param>
        /// <returns>The view model of the current <see cref="HudLayoutHeatMapTool" /></returns>
        public override HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement)
        {
            throw new NotImplementedException();
        }
    }
}