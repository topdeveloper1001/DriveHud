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

using System;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using ReactiveUI;
using DriveHUD.Common;
using System.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Represents view model of plain stat box
    /// </summary>
    public class HudPlainStatBoxViewModel : HudBaseToolViewModel
    {
        private readonly HudLayoutPlainBoxTool tool;

        /// <summary>
        /// Initialize an instance of <see cref="HudPlainStatBoxViewModel"/>
        /// </summary>
        private HudPlainStatBoxViewModel()
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudPlainStatBoxViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutPlainBoxTool"/> to initialize an instance</param>
        private HudPlainStatBoxViewModel(HudLayoutPlainBoxTool tool)
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
        /// Gets the list of <see cref="StatInfo"/> of the current tool
        /// </summary>
        public ReactiveList<StatInfo> Stats
        {
            get
            {
                return tool.Stats;
            }
        }

        #endregion

        public override void SetPositions()
        {
            var uiPosition = tool.UIPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (uiPosition == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not set UI positions {Parent.Seat}"));
            }

            Width = uiPosition.Width != 0 ? uiPosition.Width : HudDefaultSettings.PlainStatBoxWidth;
            Height = uiPosition.Height != 0 ? uiPosition.Height : HudDefaultSettings.PlainStatBoxHeight;

            Opacity = Parent.Opacity;
            Position = uiPosition.Position;
        }

        /// <summary>
        /// Sets position and size for the current <see cref="HudPlainStatBoxViewModel"/> for the specified <see cref="EnumPokerSites"/> and <see cref="EnumGameType"/>
        /// </summary>
        /// <param name="pokerSite"><see cref="EnumPokerSites"/></param>
        /// <param name="gameType"><see cref="EnumGameType"/></param>
        /// <exception cref="DHBusinessException" />
        public override void SetPositions(EnumPokerSites pokerSite, EnumGameType gameType)
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

#warning remove hardcoded values
            Opacity = Parent.Opacity;
            Width = 135;
            Height = 75;
        }
    }
}