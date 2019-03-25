//-----------------------------------------------------------------------
// <copyright file="HudNamedPipeBindingService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.HUD.Service
{
    public abstract class HudNamedPipeBindingService : IHudNamedPipeBindingService
    {
        #region Callback

        protected static IHudNamedPipeBindingCallbackService _callback;

        public static void RaiseReplayHand(long gameNumber, short pokerSiteId)
        {
            _callback?.ReplayHand(gameNumber, pokerSiteId);
        }

        public static void RaiseSaveHudLayout(HudLayoutContract hudLayout)
        {
            _callback?.SaveHudLayout(hudLayout);
        }

        public static void LoadLayout(string layoutName, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            _callback?.LoadLayout(layoutName, pokerSite, gameType, tableType);
        }

        public static void TagHands(IEnumerable<long> gameNumbers, short pokerSiteId, int tag)
        {
            _callback?.TagHands(gameNumbers, pokerSiteId, tag);
        }

        public static void TreatTableAs(IntPtr handle, EnumTableType tableType)
        {
            _callback?.TreatTableAs(handle, tableType);
        }

        #endregion

        #region Interface

        public abstract void ConnectCallbackChannel(string name);

        public abstract void UpdateHUD(byte[] data);

        public abstract void CloseTable(int windowHandle);

        #endregion
    }
}