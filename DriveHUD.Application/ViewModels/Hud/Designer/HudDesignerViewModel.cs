//-----------------------------------------------------------------------
// <copyright file="HudDesignerViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Application.ViewModels.Hud.Designer;
using System.Collections.ObjectModel;
using DriveHUD.Common.Wpf.AttachedBehaviors;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// View model class for HUD designer
    /// </summary>
    public class HudDesignerViewModel : ViewModelBase
    {
        private HudViewModel hudViewModel;

        public HudDesignerViewModel()
        {
            tools = new ReactiveList<HudBaseToolViewModel>();
        }

        /// <summary>
        /// Initialize view model
        /// </summary>
        /// <param name="hudViewModel">HUD view model</param>
        /// <param name="initialToolType">Type of tool which will be added on initializing</param>
        public void Initialize(HudViewModel hudViewModel, HudDesignerToolType initialToolType)
        {
            this.hudViewModel = hudViewModel;

            InitializeCommands();
        }

        #region Properties

        private readonly ReactiveList<HudBaseToolViewModel> tools;

        public ReactiveList<HudBaseToolViewModel> Tools
        {
            get
            {
                return tools;
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> AddToolCommand { get; private set; }

        #endregion

        #region Infrastructure

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            AddToolCommand = ReactiveCommand.Create();
            AddToolCommand.Subscribe(x =>
            {
                var dragDropDataObject = x as DragDropDataObject;

                if (dragDropDataObject == null)
                {
                    return;
                }

                var toolType = dragDropDataObject.Data as HudDesignerToolType?;

                if (toolType.HasValue && CanAddTool(toolType.Value))
                {
                    AddTool(toolType.Value, dragDropDataObject.Position);
                }
            });
        }

        /// <summary>
        /// Add designer tool on table
        /// </summary>
        /// <param name="toolType">Type of tool</param>
        /// <param name="position">Position of tool</param>
        private void AddTool(HudDesignerToolType toolType, Point position)
        {
            var factory = ServiceLocator.Current.GetInstance<IHudToolFactory>();

            var tool = factory.CreateTool(toolType);
            tool.Position = position;

            Tools.Add(tool);
        }

        /// <summary>
        /// Check if tool can be added
        /// </summary>
        /// <param name="toolType">Type of tool</param>
        /// <returns>True if tool can be added, otherwise - false</returns>
        public bool CanAddTool(HudDesignerToolType toolType)
        {
            return true;
        }

        #endregion
    }
}