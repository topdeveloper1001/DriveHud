using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Alias
{
    public class AliasPopupViewModelBase : PopupViewModelBase
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
