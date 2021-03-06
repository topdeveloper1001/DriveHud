﻿//-----------------------------------------------------------------------
// <copyright file="CommonSiteSettingTableConfigurator.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators
{
    internal class CommonSiteSettingTableConfigurator : BaseSiteSettingTableConfigurator
    {
        private const string BackgroundTableImage = "/DriveHUD.Common.Resources;component/images/settings/{0}max.png";

        protected override Dictionary<int, double[,]> PredefinedPlayerPositions { get; } = new Dictionary<int, double[,]>()
        {
            {  2, new double[,] { { 219, 34 }, { 219, 230 } } },
            {  3, new double[,] { { 219, 34 }, { 309, 230 }, { 128, 230 } } },
            {  4, new double[,] { { 219, 34 }, { 401, 130 }, { 219, 230 }, { 37, 130 } } },
            {  6, new double[,] { { 219, 34 }, { 380, 65 }, { 380, 175 }, { 219, 230 }, { 57, 175 }, { 57, 65 } } },
            {  8, new double[,] { { 219, 34 }, { 380, 65 }, { 401, 130 }, { 350, 211 }, { 219, 230 }, { 85, 211 }, { 37, 130 }, { 57, 65 } } },
            {  10, new double[,] { { 219, 34 }, { 380, 65 }, { 397, 107 }, { 397, 157 }, { 350, 211 }, { 219, 230 }, { 85, 211 }, { 37, 154 }, { 37, 107 },  { 57, 65 } } },
        };

        protected override string GetBackgroundImage(EnumTableType tableType)
        {
            return String.Format(BackgroundTableImage, (byte)tableType);
        }
    }
}
