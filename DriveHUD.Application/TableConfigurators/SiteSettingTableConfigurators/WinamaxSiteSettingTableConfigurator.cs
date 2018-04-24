//-----------------------------------------------------------------------
// <copyright file="WinamaxSiteSettingTableConfigurator.cs" company="Ace Poker Solutions">
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
    internal class WinamaxSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/empty-table.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            [4] = new double[,]
            {
                { 220, 36 }, { 392, 120 }, { 220, 228 }, { 40, 120 }
            }
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            return BackgroundTableImage;
        }
    }
}