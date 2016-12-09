using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators.SiteSettingTableConfigurators
{
    internal class WinningPokerNetworkSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/empty-table.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 220, 36 }, { 220, 228 } } },
            {  3, new double[,] { { 392, 120 }, { 220, 228 }, { 40, 120 } } },
            {  4, new double[,] { { 220, 36 }, { 392, 120 }, { 220, 228 }, { 40, 120 } } },
            {  6, new double[,] { { 219, 34 }, { 380, 65 }, { 380, 175 }, { 219, 230 }, { 57, 175 }, { 57, 65 } } },
            {  8, new double[,] { { 220, 36 }, { 388, 80 }, { 388, 164 }, { 298, 228 }, { 220, 228 }, { 142, 228 }, { 50, 164 }, { 50, 80 } } },
            {  9, new double[,] { { 276, 36 }, { 388, 80 }, { 388, 164 }, { 298, 228 }, { 220, 228 }, { 142, 228 }, { 50, 164 }, { 50, 80 }, { 165, 36 } } },
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            return BackgroundTableImage;
        }
    }
}
