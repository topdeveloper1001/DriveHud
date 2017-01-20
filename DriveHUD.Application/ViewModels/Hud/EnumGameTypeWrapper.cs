//-----------------------------------------------------------------------
// <copyright file="EnumGameTypeWrapper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using DriveHUD.Entities;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Wrapper for GameType
    /// </summary>
    public class EnumGameTypeWrapper
    {
        public EnumGameTypeWrapper(EnumGameType? gameType)
        {
            GameType = gameType;
        }

        public EnumGameType? GameType { get; }

        public string GameTypeText
        {
            get
            {
                var text = GameType.HasValue ? CommonResourceManager.Instance.GetEnumResource(GameType) : string.Empty;

                return text;
            }
        }
    }
}