using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Settings
{
    /// <summary>
    /// Proxy class for <see cref="PopupViewModelBase"/>
    /// </summary>
    public class SettingsPopupViewModelBase : PopupViewModelBase
    {
        internal new void InitializeCommands()
        {
            base.InitializeCommands();
        }

        internal new void OpenPopup(object viewModel)
        {
            base.OpenPopup(viewModel);
        }

        internal new void ClosePopup()
        {
            base.ClosePopup();
        }
    }
}
