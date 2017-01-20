using DriveHUD.Entities;
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators
{
    internal class BovadaSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/{0}max.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 219, 34 }, { 219, 230 } } },
            {  6, new double[,] { { 219, 34 }, { 380, 65 }, { 380, 175 }, { 219, 230 }, { 57, 175 }, { 57, 65 } } },
            {  9, new double[,] { { 264, 34 }, { 380, 65 }, { 390, 165 }, { 309, 230 }, { 219, 230 }, { 128, 230 }, { 55, 175 }, { 57, 65 }, { 173, 34 } } }
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            switch (tableType)
            {
                case EnumTableType.Six:
                case EnumTableType.Nine:
                    return String.Format(BackgroundTableImage, $"bovada-{(byte)tableType}");
                default:
                    return String.Format(BackgroundTableImage, (byte)tableType);
            }
        }
    }
}
