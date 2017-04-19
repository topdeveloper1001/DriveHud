//-----------------------------------------------------------------------
// <copyright file="HudGaugeIndicatorViewModel.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using ProtoBuf;
using System.Collections.Specialized;
using DriveHUD.Common.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Represents view model of gauge indicator
    /// </summary>
    [ProtoContract]
    public class HudGaugeIndicatorViewModel : HudBaseToolViewModel, IHudStatsToolViewModel
    {
        [ProtoMember(5)]
        private HudLayoutGaugeIndicator tool;

        /// <summary>
        /// Initializes an instance of <see cref="HudGaugeIndicatorViewModel"/>
        /// </summary>
        private HudGaugeIndicatorViewModel()
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudGaugeIndicatorViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutGaugeIndicator"/> to initialize an instance</param>
        private HudGaugeIndicatorViewModel(HudLayoutGaugeIndicator tool)
        {
            Check.ArgumentNotNull(() => tool);

            this.tool = tool;

            if (Stats != null)
            {
                Stats.CollectionChanged += OnStatsCollectionChanged;
            }
        }

        /// <summary>
        ///  Initialize an instance of <see cref="HudGaugeIndicatorViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutGaugeIndicator"/> to initialize an instance</param>
        /// <param name="parent">Parent <see cref="HudElementViewModel"/> to initialize an instance</param>
        public HudGaugeIndicatorViewModel(HudLayoutGaugeIndicator tool, HudElementViewModel parent) : this(tool)
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

        public string Text
        {
            get
            {
                return tool.Text;
            }
            set
            {
                tool.Text = value;
                this.RaisePropertyChanged(nameof(Text));
            }
        }

        public StatInfo BaseStat
        {
            get
            {
                return tool.BaseStat;
            }
        }

        public ReactiveList<StatInfo> Stats
        {
            get
            {
                return tool.Stats;
            }
        }

        #endregion

        #region Implementation of HudBaseToolViewModel

        public override void InitializePositions()
        {
        }

        public override void InitializePositions(EnumPokerSites pokerSite, EnumGameType gameType)
        {
        }

        public override void SetPositions(List<HudPositionInfo> positions)
        {
        }

        #endregion

        private void OnStatsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                var random = new Random();

                var addedStats = e.NewItems.OfType<StatInfo>().ToArray();
                addedStats.ForEach(x =>
                {
                    x.CurrentValue = random.Next(0, 100);
                    //x.Caption = string.Format(x.Format, x.CurrentValue);
                    x.StatInfoMeter = new StatInfoMeterModel();
                });
            }
        }
    }
}