//-----------------------------------------------------------------------
// <copyright file="HudStatsDesignerBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Stats;
using System.Windows;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudStatsDesignerBehavior : HudStatsBehavior
    {
        public static readonly DependencyProperty DragDropCommandProperty = DependencyProperty.Register("DragDropCommand", typeof(ICommand), typeof(HudStatsBehavior));

        public ICommand DragDropCommand
        {
            get
            {
                return (ICommand)GetValue(DragDropCommandProperty);
            }
            set
            {
                SetValue(DragDropCommandProperty, value);
            }
        }

        public static readonly DependencyProperty StatClickCommandProperty = DependencyProperty.Register("StatClickCommand", typeof(ICommand), typeof(HudStatsBehavior));

        public ICommand StatClickCommand
        {
            get
            {
                return (ICommand)GetValue(StatClickCommandProperty);
            }
            set
            {
                SetValue(StatClickCommandProperty, value);
            }
        }

        protected override FrameworkElement CreateStatBlock(StatInfo statInfo)
        {
            var block = base.CreateStatBlock(statInfo);

            DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop.SetIsDragTarget(block, true);
            DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop.SetGroupProperty(block, HudDragDropGroups.Popups);
            DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop.SetDragDropCommand(block, DragDropCommand);

            return block;
        }

        protected override void ConfigureToolTip(FrameworkElement element, StatInfo statInfo)
        {
            ConfigureSimpleToolTip(element, statInfo);
        }
    }
}