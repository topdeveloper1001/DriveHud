using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;

namespace DriveHUD.Application.TableConfigurators
{
    internal class CommonSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/{0}max.png";


        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            return String.Format(BackgroundTableImage, (byte)tableType);
        }
    }
}
