using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Settings
{
    internal class SettingsRakeBackViewModelInfo<T>
    {
        internal T Model { get; set; }

        internal Action<T> Add { get; set; }
        internal Action Close { get; set; }
    }
}
