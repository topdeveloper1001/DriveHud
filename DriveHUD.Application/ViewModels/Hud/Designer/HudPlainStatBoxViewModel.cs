//-----------------------------------------------------------------------
// <copyright file="HudPlainStatBoxViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    /// <summary>
    /// Represents view model of plain stat box
    /// </summary>
    public class HudPlainStatBoxViewModel : HudBaseToolViewModel, IHudStatsToolViewModel
    {
        [ProtoMember(5)]
        private HudLayoutPlainBoxTool tool;

        /// <summary>
        /// Initializes an instance of <see cref="HudPlainStatBoxViewModel"/>
        /// </summary>
        protected HudPlainStatBoxViewModel()
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudPlainStatBoxViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutPlainBoxTool"/> to initialize an instance</param>
        protected HudPlainStatBoxViewModel(HudLayoutPlainBoxTool tool)
        {
            Check.ArgumentNotNull(() => tool);

            this.tool = tool;
        }

        /// <summary>
        ///  Initialize an instance of <see cref="HudPlainStatBoxViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutPlainBoxTool"/> to initialize an instance</param>
        /// <param name="parent">Parent <see cref="HudElementViewModel"/> to initialize an instance</param>
        public HudPlainStatBoxViewModel(HudLayoutPlainBoxTool tool, HudElementViewModel parent) : this(tool)
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
        /// Gets the list of <see cref="StatInfo"/> of the current tool
        /// </summary>
        public ReactiveList<StatInfo> Stats
        {
            get
            {
                return tool.Stats;
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
        public virtual double DefaultWidth
        {
            get
            {
                return HudDefaultSettings.PlainStatBoxWidth;
            }
        }

        /// <summary>
        /// Gets the default height of the tool
        /// </summary>
        public virtual double DefaultHeight
        {
            get
            {
                return HudDefaultSettings.PlainStatBoxHeight;
            }
        }

        #endregion

        /// <summary>
        /// Initializes UI position and size for the current <see cref="HudBaseToolViewModel"/>
        /// </summary>
        /// <exception cref="DHBusinessException" />
        public override void InitializePositions()
        {
            var uiPosition = tool.UIPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (uiPosition == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not set UI positions {Parent.Seat}"));
            }

            Width = uiPosition.Width != 0 ? uiPosition.Width : DefaultWidth;
            Height = uiPosition.Height != 0 ? uiPosition.Height : DefaultHeight;

            Opacity = Parent.Opacity;
            Position = uiPosition.Position;
        }

        /// <summary>
        /// Initializes position and size for the current <see cref="HudPlainStatBoxViewModel"/> for the specified <see cref="EnumPokerSites"/> and <see cref="EnumGameType"/>
        /// </summary>
        /// <param name="pokerSite"><see cref="EnumPokerSites"/></param>
        /// <param name="gameType"><see cref="EnumGameType"/></param>
        /// <exception cref="DHBusinessException" />
        public override void InitializePositions(EnumPokerSites pokerSite, EnumGameType gameType)
        {
            var positionInfo = tool.Positions.FirstOrDefault(x => x.PokerSite == pokerSite && x.GameType == gameType);

            if (positionInfo == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not find data for position info {Parent.Seat}"));
            }

            var positions = positionInfo.HudPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (positions == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not find data for positions {Parent.Seat}"));
            }

            Position = positions.Position;
            Opacity = Parent.Opacity;

            var uiPositions = tool.UIPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            Width = uiPositions != null && uiPositions.Width != 0 ? uiPositions.Width : DefaultWidth;
            Height = uiPositions != null && uiPositions.Height != 0 ? uiPositions.Height : DefaultHeight;
        }

        /// <summary>
        /// Sets <see cref="HudLayoutPlainBoxTool"/> positions for current tool
        /// </summary>
        /// <param name="positions">The list of <see cref="HudPositionInfo"/></param>
        public override void SetPositions(List<HudPositionInfo> positions)
        {
            if (positions == null)
            {
                return;
            }

            tool.UIPositions = positions;
        }

        /// <summary>
        /// Saves <see cref="HudPositionInfo"/> positions for current tool
        /// </summary>
        /// <param name="positions">The list of <see cref="HudPositionInfo"/></param>
        public override void SavePositions(List<HudPositionInfo> positions)
        {
            SetPositions(positions);
        }
    }
}