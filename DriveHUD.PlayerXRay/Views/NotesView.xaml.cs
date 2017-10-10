//-----------------------------------------------------------------------
// <copyright file="NotesView.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.PlayerXRay.Views
{
    /// <summary>
    /// Interaction logic for NotesView.xaml
    /// </summary>
    public partial class NotesView : UserControl
    {
        public NotesView()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as NotesViewModel).AddToSelectedFiltersCommand.Execute(null);
        }
    }
}