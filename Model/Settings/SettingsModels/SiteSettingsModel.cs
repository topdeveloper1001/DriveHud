using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    [Serializable]
    public class SiteSettingsModel : SettingsBase
    {
        public bool IsCustomProcessedDataLocationEnabled { get; set; }
        public string CustomProcessedDataLocation { get; set; }

        public SiteSettingsModel()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            IsCustomProcessedDataLocationEnabled = false;
            CustomProcessedDataLocation = StringFormatter.GetAppDataFolderPath();
        }
    }
}
