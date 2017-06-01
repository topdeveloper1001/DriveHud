//-----------------------------------------------------------------------
// <copyright file="HudLayoutsExtensions.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using Model.Enums;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    public static class HudLayoutsExtensions
    {
        public static void SetStatValue(this HudElementViewModel hudElement, Stat stat, decimal value)
        {
            if (hudElement == null)
            {
                return;
            }

            var statInfo = hudElement.StatInfoCollection.FirstOrDefault(x => x.Stat == stat);

            if (statInfo == null)
            {
                return;
            }

            statInfo.Caption = string.Format(statInfo.Format, value);
            statInfo.CurrentValue = value;
        }

        public static void SetStatValue(this HudElementViewModel hudElement, Stat stat, string propertyName, decimal value)
        {
            if (hudElement == null)
            {
                return;
            }

            var statInfo = hudElement.StatInfoCollection.FirstOrDefault(x => x.Stat == stat);

            if (statInfo == null)
            {
                return;
            }

            statInfo.Caption = string.Format(statInfo.Format, value);
            statInfo.CurrentValue = value;        
        }
    }
}