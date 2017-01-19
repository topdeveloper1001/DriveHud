//-----------------------------------------------------------------------
// <copyright file="CommonVertOneTableConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using System.Collections.Generic;
using DriveHUD.Entities;

namespace DriveHUD.Application.TableConfigurators
{
    internal class CommonVertOneTableConfiguration : CommonTableConfigurator
    {
        public override int HudElementWidth
        {
            get
            {
                return (int)HudDefaultSettings.BovadaRichHudElementWidth;
            }
        }

        public override int HudElementHeight
        {
            get
            {
                return (int)HudDefaultSettings.BovadaRichHudElementHeight;
            }
        }

        public override HudViewType HudViewType
        {
            get
            {
                return HudViewType.Vertical_1;
            }
        }

        protected override Dictionary<int, int[,]> GetPredefinedPositions()
        {
            var predefinedPositions = GetPredefinedLabelPositions();

            foreach (var predefinedPosition in predefinedPositions)
            {
                for (var i = 0; i < predefinedPosition.Key; i++)
                {
                    // x coordinate                    
                    predefinedPosition.Value[i, 0] -= IsRightOriented(predefinedPosition.Key, i) ? 4 : 36;
                    // y coordinate
                    predefinedPosition.Value[i, 1] -= 59;
                }
            }

            return predefinedPositions;
        }

        protected override bool IsRightOriented(int seats, int seat)
        {
            return (seats > 6 && seat < 5) || (seats < 7 && seats > 2 && seat < 3) || (seats < 3 && seat < 1);
        }
    }
}