using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Settings
{
    public interface ISettingsViewModel
    {
        string Name { get; set; }

        void SetSettingsModel(ISettingsBase model);
    }
}
