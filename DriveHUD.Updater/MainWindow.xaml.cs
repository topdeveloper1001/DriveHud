//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowWpf : WindowBase
    {
        public MainWindowWpf()
            : this(new MainViewModel())
        {
        }

        public MainViewModel ViewModel { get; private set; }

        private MainWindowWpf(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;         
            InitializeComponent();
        }
      
        private static MainWindowWpf instance;

        public static MainWindowWpf Instance
        {
            get
            {
                return instance ?? (instance = new MainWindowWpf());
            }
        }
    }
}