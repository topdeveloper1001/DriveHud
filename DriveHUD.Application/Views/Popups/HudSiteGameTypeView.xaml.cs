using System;
using DriveHUD.Application.ViewModels.Popups;

namespace DriveHUD.Application.Views.Popups
{
    public partial class HudSiteGameTypeView
    {
        public HudSiteGameTypeView(HudSiteGameTypeViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CloseAction = Close;
            DataContext = viewModel;
        }

        private void HudSiteGameTypeViewOnClosing(object sender, EventArgs e)
        {
            DialogResult = (DataContext as HudSiteGameTypeViewModel)?.Result;
        }
    }
}