//-----------------------------------------------------------------------
// <copyright file="HorizonSiteSettingTableConfigurator.cs" company="Ace Poker Solutions">
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
    internal class HorizonSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/rev-{0}max.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 362, 100 }, { 72, 100 } } },
            {  3, new double[,] { { 362, 100 }, { 219, 230 }, { 72, 100 } } },
            {  4, new double[,] { { 379, 56 }, { 336, 215 }, { 96, 215 }, { 55, 56 } } },
            {  5, new double[,] { { 380, 65 }, { 380, 175 }, { 219, 230 }, { 57, 175 }, { 57, 65 } } },
            {  6, new double[,] { { 312, 34 }, { 380, 175 }, { 294, 230 }, { 134, 230 }, { 57, 175 }, { 120, 34 } } },
            {  7, new double[,] { { 312, 34 }, { 401, 130 }, { 294, 230 }, { 217, 230 }, { 133, 230 }, { 37, 130 }, { 121 , 34 } } },
            {  8, new double[,] { { 314, 34 }, { 394, 81 }, { 366, 204 }, { 263, 230 }, { 165, 230 }, { 68, 204 }, { 40, 81}, { 115, 34 } } },
            {  9, new double[,] { { 307, 34 }, { 388, 73 }, { 390, 168 }, { 305, 230 }, { 217, 230 }, { 121, 230 }, { 55, 168 }, { 52, 73 }, { 121, 34 } } },
            {  10, new double[,] { { 299, 34 }, { 380, 65 }, { 401, 130 }, { 350, 211 }, { 280, 230 }, { 155, 230 }, { 85, 211 }, { 37, 130 }, { 50, 65 }, { 140, 34 } } },
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {            
            return string.Format(BackgroundTableImage, (byte)tableType);
        }
    }
}