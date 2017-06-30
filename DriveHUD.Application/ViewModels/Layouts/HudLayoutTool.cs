//-----------------------------------------------------------------------
// <copyright file="HudLayoutTool.cs" company="Ace Poker Solutions">
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
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents base tool in hud layout
    /// </summary>        
    [Serializable, ProtoContract]
    [XmlInclude(typeof(HudLayoutNonPopupTool)), ProtoInclude(29, typeof(HudLayoutNonPopupTool))]
    [XmlInclude(typeof(HudLayoutGaugeIndicator)), ProtoInclude(32, typeof(HudLayoutGaugeIndicator))]
    [XmlInclude(typeof(HudLayoutGraphTool)), ProtoInclude(36, typeof(HudLayoutGraphTool))]
    [XmlInclude(typeof(HudLayoutHeatMapTool)), ProtoInclude(38, typeof(HudLayoutHeatMapTool))]    
    public abstract class HudLayoutTool
    {
        public HudLayoutTool()
        {
            Id = Guid.NewGuid();
        }

        [ProtoMember(1), XmlAttribute]
        /// <summary>
        /// Gets the id of tool
        /// </summary>
        public Guid Id
        {
            get; set;
        }

        /// <summary>
        /// Gets the type <see cref="HudDesignerToolType"/> of the tool
        /// </summary>     
        [XmlIgnore]
        public HudDesignerToolType ToolType { get; protected set; }

        /// <summary>
        /// Creates a copy of the current <see cref="HudLayoutTool"/> instance
        /// </summary>
        /// <returns>Copy of the current <see cref="HudLayoutTool"/> instance</returns>
        public abstract HudLayoutTool Clone();

        /// <summary>
        /// Creates a view model of the current <see cref="HudBaseToolViewModel"/> instance
        /// </summary>
        /// <returns>View model of the current <see cref="HudBaseToolViewModel"/> instance</returns>
        public abstract HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement);
    }
}