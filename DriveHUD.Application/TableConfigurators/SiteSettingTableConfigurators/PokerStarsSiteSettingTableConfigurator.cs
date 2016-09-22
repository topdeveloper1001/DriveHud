using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;

namespace DriveHUD.Application.TableConfigurators
{
    internal class PokerStarsSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/{0}max.png";

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            switch (tableType)
            {
                case EnumTableType.Nine:
                case EnumTableType.Ten:
                    return String.Format(BackgroundTableImage, $"pokerstars-{(byte)tableType}");
                default:
                    return String.Format(BackgroundTableImage, (byte)tableType);
            }
        }
    }
}
