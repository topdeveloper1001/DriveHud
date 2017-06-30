//-----------------------------------------------------------------------
// <copyright file="HudBumperStickersViewModel.cs" company="Ace Poker Solutions">
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
using ProtoBuf;
using System;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    public class HudBumperStickersViewModel : HudBaseNonPopupToolViewModel<HudLayoutBumperStickersTool>
    {
        [ProtoMember(5)]
        protected HudLayoutBumperStickersTool tool;

        /// <summary>
        /// Initializes an instance of <see cref="HudBumperStickersViewModel"/>
        /// </summary>
        protected HudBumperStickersViewModel()
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudBumperStickersViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutBumperStickersTool"/> to initialize an instance</param>
        protected HudBumperStickersViewModel(HudLayoutBumperStickersTool tool)
        {
            Check.ArgumentNotNull(() => tool);

            this.tool = tool;
        }

        /// <summary>
        ///  Initialize an instance of <see cref="HudBumperStickersViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutBumperStickersTool"/> to initialize an instance</param>
        /// <param name="parent">Parent <see cref="HudElementViewModel"/> to initialize an instance</param>
        public HudBumperStickersViewModel(HudLayoutBumperStickersTool tool, HudElementViewModel parent) : this(tool)
        {
            Check.ArgumentNotNull(() => parent);

            Parent = parent;
        }

        #region Properties

        /// <summary>
        /// Gets the <see cref="HudDesignerToolType"/> type of the current tool
        /// </summary>
        public override HudDesignerToolType ToolType
        {
            get
            {
                return tool.ToolType;
            }
        }

        /// <summary>
        /// Gets the id for the current tool
        /// </summary>
        public override Guid Id
        {
            get
            {
                return tool.Id;
            }
        }

        /// <summary>
        /// Gets the layout tool
        /// </summary>
        public override HudLayoutTool Tool
        {
            get
            {
                return tool;
            }
        }

        /// <summary>
        /// Gets the default width of the tool
        /// </summary>
        public override double DefaultWidth
        {
            get
            {
                return HudDefaultSettings.BumperStickersWidth;
            }
        }

        /// <summary>
        /// Gets the default height of the tool
        /// </summary>
        public override double DefaultHeight
        {
            get
            {
                return HudDefaultSettings.BumperStickersHeight;
            }
        }

        #endregion  
    }
}