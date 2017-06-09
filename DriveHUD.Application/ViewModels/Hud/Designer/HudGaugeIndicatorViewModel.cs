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

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Wpf.AttachedBehaviors;
using DriveHUD.Entities;
using Model.Stats;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Represents view model of gauge indicator
    /// </summary>
    [ProtoContract]
    public class HudGaugeIndicatorViewModel : HudBaseToolViewModel, IHudStatsToolViewModel, IHudBaseStatToolViewModel
    {
        [ProtoMember(5)]
        private HudLayoutGaugeIndicator tool;

        /// <summary>
        /// Initializes an instance of <see cref="HudGaugeIndicatorViewModel"/>
        /// </summary>
        private HudGaugeIndicatorViewModel()
        {
            Opacity = 1;
        }

        /// <summary>
        /// Initialize an instance of <see cref="HudGaugeIndicatorViewModel"/>
        /// </summary>
        /// <param name="tool"><see cref="HudLayoutGaugeIndicator"/> to initialize an instance</param>
        private HudGaugeIndicatorViewModel(HudLayoutGaugeIndicator tool) : this()
        {
            Check.ArgumentNotNull(() => tool);

            this.tool = tool;

            if (Stats != null)
            {
                Stats.CollectionChanged += OnStatsCollectionChanged;
            }

            InitializeCommands();
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
        /// Gets or sets text on the left side of gauge indicator
        /// </summary>
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

        /// <summary>
        /// Gets or sets text on the left side of gauge indicator
        /// </summary>
        public string HeaderText
        {
            get
            {
                return tool.HeaderText;
            }
            set
            {
                tool.HeaderText = value;
                this.RaisePropertyChanged(nameof(HeaderText));
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

        /// <summary>
        /// Gets the list of <see cref="StatInfo"/> of gauge indicator
        /// </summary>
        public ReactiveList<StatInfo> Stats
        {
            get
            {
                return tool.Stats;
            }
        }

        /// <summary>
        /// Gets whenever heat map is attached to the <see cref="BaseStat"/> 
        /// </summary>
        public bool IsHeatMapVisible
        {
            get
            {
                return Parent != null && Parent.Tools != null &&
                    Parent.Tools.OfType<HudHeatMapViewModel>().Any(x => x.BaseStat != null && x.BaseStat.Stat == BaseStat.Stat);
            }
        }

        /// <summary>
        /// Gets <see cref="HudHeatMapViewModel"/> which is attached to the <see cref="BaseStat"/> 
        /// </summary>
        public HudHeatMapViewModel HeatMapViewModel
        {
            get
            {
                return IsHeatMapVisible ?
                    Parent.Tools.OfType<HudHeatMapViewModel>().FirstOrDefault(x => x.BaseStat != null && x.BaseStat.Stat == BaseStat.Stat) :
                    null;
            }
        }

        /// <summary>
        /// Gets the command for drag & drop actions
        /// </summary>
        public virtual ICommand DragDropCommand
        {
            get;
            protected set;
        }

        #endregion

        #region Implementation of HudBaseToolViewModel

        public override void InitializePositions()
        {
            if (Parent == null)
            {
                return;
            }

            Opacity = Parent.Opacity;

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

            DragDropCommand = new RelayCommand(x =>
            {
                var dragDropDataObject = x as DragDropDataObject;

                if (dragDropDataObject == null)
                {
                    return;
                }

                var dropStatInfoSource = dragDropDataObject.Source as StatInfo;

                if (dropStatInfoSource == null)
                {
                    return;
                }

                var dropStatInfoSourceIndex = Stats.IndexOf(dropStatInfoSource) + 1;

                if (dropStatInfoSourceIndex < 0 || dropStatInfoSourceIndex >= Stats.Count)
                {
                    return;
                }

                IsSelected = false;
                Stats.Insert(dropStatInfoSourceIndex, new StatInfoBreak());
                IsSelected = true;
            }, x =>
            {
                var dragDropDataObject = x as DragDropDataObject;

                if (dragDropDataObject == null)
                {
                    return false;
                }

                var toolType = dragDropDataObject.DropData as HudDesignerToolType?;

                return toolType == HudDesignerToolType.LineBreak;
            });
        }

        /// <summary>
        /// Handles stats collection changed event
        /// </summary>   
        private void OnStatsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                var random = new Random();

                var addedStats = e.NewItems.OfType<StatInfo>().ToArray();

                // adds random values
                addedStats.ForEach(x =>
                {
                    x.CurrentValue = random.Next(0, 100);
                    x.StatInfoMeter = new StatInfoMeterModel();
                });
            }
        }
    }
}