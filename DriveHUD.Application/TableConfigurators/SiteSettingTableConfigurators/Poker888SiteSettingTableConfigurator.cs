//-----------------------------------------------------------------------
// <copyright file="Poker888TableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Model.Enums;

namespace DriveHUD.Application.TableConfigurators
{
    internal class Poker888SiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/empty-table.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 220, 36 }, { 220, 228 } } },
            {  3, new double[,] { { 392, 120 }, { 220, 228 }, { 40, 120 } } },
            {  4, new double[,] { { 220, 36 }, { 392, 120 }, { 220, 228 }, { 40, 120 } } },
            {  5, new double[,] { { 298, 36 }, { 388, 164 }, { 220, 228 }, { 50, 164 }, { 142, 36 } } },
            {  6, new double[,] { { 276, 36 }, { 392, 120 }, { 276, 228 }, { 142, 228 }, { 40, 120 }, { 142, 36 } } },
            {  8, new double[,] { { 276, 36 }, { 388, 80 }, { 388, 164 }, { 276, 228 }, { 142, 228 }, { 50, 164 }, { 50, 80 }, { 142, 36 } } },
            {  9, new double[,] { { 276, 36 }, { 388, 80 }, { 388, 164 }, { 298, 228 }, { 220, 228 }, { 142, 228 }, { 50, 164 }, { 50, 80 }, { 165, 36 } } },
            {  10, new double[,] { { 298, 36 }, { 388, 80 }, { 388, 164 }, { 298, 228 }, { 220, 228 }, { 142, 228 }, { 50, 164 }, { 50, 80 }, { 142, 36 }, { 220, 36 } } },
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            return BackgroundTableImage;
        }
    }
}