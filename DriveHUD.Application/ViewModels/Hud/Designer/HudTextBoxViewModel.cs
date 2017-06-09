//-----------------------------------------------------------------------
// <copyright file="HudTextBoxViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Linq;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Model.Stats;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    public class HudTextBoxViewModel : HudBaseNonPopupToolViewModel<HudLayoutTextBoxTool>
    {
        [ProtoMember(5)]
        protected HudLayoutTextBoxTool tool;

        /// <summary>
        /// Initializes an instance of <see cref="HudTextBoxViewModel"/>
        /// </summary>
        protected HudTextBoxViewModel()
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudTextBoxViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutTextBoxTool"/> to initialize an instance</param>
        protected HudTextBoxViewModel(HudLayoutTextBoxTool tool)
        {
            Check.ArgumentNotNull(() => tool);

            this.tool = tool;
        }

        /// <summary>
        ///  Initialize an instance of <see cref="HudTextBoxViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutTextBoxTool"/> to initialize an instance</param>
        /// <param name="parent">Parent <see cref="HudElementViewModel"/> to initialize an instance</param>
        public HudTextBoxViewModel(HudLayoutTextBoxTool tool, HudElementViewModel parent) : this(tool)
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
                return HudDefaultSettings.TextBoxWidth;
            }
        }

        /// <summary>
        /// Gets the default height of the tool
        /// </summary>
        public override double DefaultHeight
        {
            get
            {
                return HudDefaultSettings.TextBoxHeight;
            }
        }

        /// <summary>
        /// Gets or sets the text of the tool
        /// </summary>
        public string Text
        {
            get
            {
                return tool?.Text;
            }
            set
            {
                if (tool == null)
                {
                    return;
                }

                tool.Text = value;
                this.RaisePropertyChanged(nameof(Text));
            }
        }

        #endregion
    }
}