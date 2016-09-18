using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    public interface ISettingsService
    {
        void SaveSettings(SettingsModel settings);

        SettingsModel GetSettings();
    }
}
