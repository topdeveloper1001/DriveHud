//-----------------------------------------------------------------------
// <copyright file="DropDownMenu.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for DropDownMenu.xaml
    /// </summary>
    public partial class DropDownMenu : UserControl
    {
        public DropDownMenu()
        {
            InitializeComponent();
            Command = new RelayCommand(SelectItem);
        }

        /// <summary>
        /// Selected item dependency property 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(IDropDownMenuItem), typeof(DropDownMenu), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Selected item
        /// </summary>
        public IDropDownMenuItem SelectedItem
        {
            get
            {
                return (IDropDownMenuItem)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Select command
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DropDownMenu), new PropertyMetadata());

        [Bindable(false)]
        /// <summary>
        /// Select command
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        private void SelectItem(object item)
        {
            SelectedItem = item as IDropDownMenuItem;
        }

        /// <summary>
        /// Menu item dependency property 
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<IDropDownMenuItem>), typeof(DropDownMenu), new PropertyMetadata());

        /// <summary>
        /// Menu items
        /// </summary>
        public ObservableCollection<IDropDownMenuItem> Items
        {
            get
            {
                return (ObservableCollection<IDropDownMenuItem>)GetValue(ItemsProperty);
            }
            set
            {
                SetValue(ItemsProperty, value);
            }
        }
    }
}
