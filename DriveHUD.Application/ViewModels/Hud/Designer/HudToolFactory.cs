//-----------------------------------------------------------------------
// <copyright file="HudToolFactory.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Factory creates tool view model based on tool type
    /// </summary>
    internal class HudToolFactory : IHudToolFactory
    {
        public HudBaseToolViewModel CreateTool(HudDesignerToolType toopType)
        {
            var tool = new HudPlainStatBoxViewModel
            {
                Width = 135,
                Height = 75,
                Opacity = 100
            };

            return tool;
        }
    }
}