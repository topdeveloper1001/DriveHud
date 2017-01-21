﻿using DriveHUD.Entities;
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators
{
    internal class PokerStarsSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/{0}max.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 219, 34 }, { 219, 230 } } },
            {  3, new double[,] { { 219, 34 }, { 309, 230 }, { 128, 230 } } },
            {  4, new double[,] { { 219, 34 }, { 401, 130 }, { 219, 230 }, { 37, 130 } } },
            {  6, new double[,] { { 219, 34 }, { 380, 65 }, { 380, 175 }, { 219, 230 }, { 57, 175 }, { 57, 65 } } },
            {  8, new double[,] { { 219, 34 }, { 380, 65 }, { 401, 130 }, { 350, 211 }, { 219, 230 }, { 85, 211 }, { 37, 130 }, { 57, 65 } } },
            {  9, new double[,] { { 264, 34 }, { 380, 65 }, { 390, 165 }, { 309, 230 }, { 219, 230 }, { 128, 230 }, { 55, 175 }, { 57, 65 }, { 173, 34 } } },
            {  10, new double[,] { { 270, 34 }, { 380, 65 }, { 401, 130 }, { 350, 211 }, { 270, 230 }, { 165, 230 }, { 85, 211 }, { 37, 130 }, { 57, 65 }, { 165, 34 } } },
        };

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
