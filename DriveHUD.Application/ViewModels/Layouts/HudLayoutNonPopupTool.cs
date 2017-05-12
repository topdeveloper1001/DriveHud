//-----------------------------------------------------------------------
// <copyright file="HudLayoutNonPopupTool.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// Base class for tools which have positions on both HUD and Designer
    /// </summary>
    [Serializable, ProtoContract]
    [XmlInclude(typeof(HudLayoutPlainBoxTool)), ProtoInclude(30, typeof(HudLayoutPlainBoxTool))]
    [XmlInclude(typeof(HudLayoutTextBoxTool)), ProtoInclude(31, typeof(HudLayoutTextBoxTool))]
    [XmlInclude(typeof(HudLayoutTiltMeterTool)), ProtoInclude(34, typeof(HudLayoutTiltMeterTool))]
    [XmlInclude(typeof(HudLayoutPlayerIconTool)), ProtoInclude(35, typeof(HudLayoutPlayerIconTool))]
    [XmlInclude(typeof(HudLayoutBumperStickersTool)), ProtoInclude(37, typeof(HudLayoutBumperStickersTool))]
    public abstract class HudLayoutNonPopupTool : HudLayoutTool
    {
        /// <summary>
        /// Gets or sets the list of <see cref="HudPositionsInfo"/> positions of the plain box tool on hud
        /// </summary>
        public List<HudPositionsInfo> Positions { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudPositionInfo"/> UI positions of the plain box tool
        /// </summary>
        public List<HudPositionInfo> UIPositions { get; set; }
    }
}