//-----------------------------------------------------------------------
// <copyright file="HudHeatMapViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using Model.Data;
using Model.Stats;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Represents view model of heat map
    /// </summary>
    [ProtoContract]
    public class HudHeatMapViewModel : HudBaseToolViewModel, IHudBaseStatToolViewModel
    {
        [ProtoMember(1)]
        private HudLayoutHeatMapTool tool;

        /// <summary>
        /// Initializes an instance of <see cref="HudGraphViewModel"/>
        /// </summary>
        private HudHeatMapViewModel()
        {
            Opacity = 1;
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudHeatMapViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutGraphTool"/> to initialize an instance</param>
        private HudHeatMapViewModel(HudLayoutHeatMapTool tool) : this()
        {
            Check.ArgumentNotNull(() => tool);

            this.tool = tool;

            InitializeCommands();
        }


        /// <summary>
        ///  Initialize an instance of <see cref="HudHeatMapViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutHeatMapTool"/> to initialize an instance</param>
        /// <param name="parent">Parent <see cref="HudElementViewModel"/> to initialize an instance</param>
        public HudHeatMapViewModel(HudLayoutHeatMapTool tool, HudElementViewModel parent) : this(tool)
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
        /// Gets the base stat to which gauge indicator is attached
        /// </summary>
        public StatInfo BaseStat
        {
            get
            {
                return tool.BaseStat;
            }
        }

        [ProtoMember(2)]
        private Dictionary<string, StatDto> heatMap = new Dictionary<string, StatDto>();

        /// <summary>
        /// Gets the top stat of the graph
        /// </summary>
        public Dictionary<string, StatDto> HeatMap
        {
            get
            {
                return heatMap;
            }
            set
            {
                if (ReferenceEquals(heatMap, value))
                {
                    return;
                }

                heatMap = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="BindingMode"/> for <see cref="HudBaseToolViewModel.IsSelected"/> property
        /// </summary>
        public override BindingMode IsSelectedBindingMode
        {
            get
            {
                return BindingMode.TwoWay;
            }
        }

        /// <summary>
        /// Gets whenever tools is re-sizable
        /// </summary>
        public override bool IsResizable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whenever save button is visible on toolbox
        /// </summary>
        public override bool IsSaveVisible
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Implementation of HudBaseToolViewModel

        public override void InitializePositions()
        {
            if (Parent == null)
            {
                return;
            }

            if (tool != null && tool.PositionInfo != null)
            {
                Position = tool.PositionInfo.Position;
            }
        }

        public override void InitializePositions(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType)
        {
        }

        public override void SetPositions(List<HudPositionInfo> positions)
        {
        }

        /// <summary>
        /// Saves <see cref="HudPositionInfo"/> positions for current tool
        /// </summary>
        /// <param name="positions">The list of <see cref="HudPositionInfo"/></param>
        public override void SavePositions(List<HudPositionInfo> positions)
        {
            if (tool != null)
            {
                tool.PositionInfo = new HudPositionInfo
                {
                    Position = Position
                };
            }
        }

        #endregion

        /// <summary>
        /// Initializes commands
        /// </summary>
        private void InitializeCommands()
        {
            SaveCommand = ReactiveCommand.Create();
            (SaveCommand as ReactiveCommand<object>).Subscribe(x =>
            {
                IsVisible = false;
                IsSelected = false;
            });
        }
    }
}