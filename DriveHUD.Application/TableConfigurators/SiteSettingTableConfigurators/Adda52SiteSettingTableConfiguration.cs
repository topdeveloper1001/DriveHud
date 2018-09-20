//-----------------------------------------------------------------------
// <copyright file="Adda52SiteSettingTableConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
    internal class Adda52SiteSettingTableConfiguration : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/empty-table.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 395, 135 }, { 42, 135 } } },
            {  4, new double[,] { { 294, 34 }, { 294, 230 }, { 134, 230 }, { 134, 34 } } },
            {  6, new double[,] { { 294, 34 }, { 395, 135 }, { 294, 230 }, { 134, 230 }, { 42, 135 }, { 134, 34 } } },
            {  8, new double[,] { { 294, 34 }, { 394, 81 }, { 394, 184 }, { 294, 230 }, { 134, 230 }, { 40, 184 }, { 40, 81}, { 134, 34 } } },
            {  9, new double[,] { { 307, 34 }, { 388, 73 }, { 390, 168 }, { 305, 230 }, { 217, 230 }, { 121, 230 }, { 55, 168 }, { 52, 73 }, { 121, 34 } } },
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            return string.Format(BackgroundTableImage, (byte)tableType);
        }
    }
}