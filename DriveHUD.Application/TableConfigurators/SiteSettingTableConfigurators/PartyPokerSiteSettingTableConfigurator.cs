using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators.SiteSettingTableConfigurators
{
    internal class PartyPokerSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/empty-table.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 401, 130 }, { 40, 120 } } },
            {  3, new double[,] { { 392, 120 }, { 220, 228 }, { 40, 120 } } },
            {  4, new double[,] { { 380, 65 }, { 380, 175 }, { 57, 175 }, { 57, 65 } } },
            {  6, new double[,] { { 276, 36 }, { 392, 120 }, { 276, 228 }, { 142, 228 }, { 40, 120 }, { 142, 36 } } },
            {  8, new double[,] { { 276, 36 }, { 388, 80 }, { 388, 164 }, { 276, 228 }, { 142, 228 }, { 50, 164 }, { 50, 80 }, { 142, 36 } } },
            {  9, new double[,] { { 264, 34 }, { 380, 65 }, { 390, 165 }, { 309, 230 }, { 219, 230 }, { 128, 230 }, { 55, 175 }, { 57, 65 }, { 173, 34 } } },
            {  10, new double[,] { { 270, 34 }, { 380, 65 }, { 401, 130 }, { 350, 211 }, { 270, 230 }, { 165, 230 }, { 85, 211 }, { 37, 130 }, { 57, 65 }, { 165, 34 } } },
         };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            return BackgroundTableImage;
        }
    }
}
