//-----------------------------------------------------------------------
// <copyright file="GGNSiteSettingTableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.SiteSettingTableConfigurators
{
    internal class GGNSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/{0}max.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  6, new double[,] { { 380, 65 }, { 380, 175 }, { 219, 230 }, { 57, 175 }, { 57, 65 }, { 219, 34 } } },
            {  9, new double[,] { { 264, 34 }, { 380, 65 }, { 390, 165 }, { 309, 230 }, { 219, 230 }, { 128, 230 }, { 55, 175 }, { 57, 65 }, { 173, 34 } } },
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            switch (tableType)
            {
                case EnumTableType.Nine:
                    return string.Format(BackgroundTableImage, $"pokerstars-{(byte)tableType}");
                default:
                    return string.Format(BackgroundTableImage, (byte)tableType);
            }
        }
    }
}