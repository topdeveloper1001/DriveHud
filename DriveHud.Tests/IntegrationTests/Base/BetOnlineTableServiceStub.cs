//-----------------------------------------------------------------------
// <copyright file="BetOnlineTableServiceStub.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.BetOnline;
using System;

namespace DriveHud.Tests.IntegrationTests.Base
{
    public class BetOnlineTableServiceStub : IBetOnlineTableService
    {
        private const int SessionCode = 7777777;

        public bool IsRunning
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler ProcessStopped;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string GetRandomPlayerName(string sessionCode, int seat)
        {
            return Utils.GenerateRandomPlayerName(seat);
        }

        public int GetSessionCode(string tableName, out EnumPokerSites site)
        {
            site = EnumPokerSites.BetOnline;
            return SessionCode;
        }

        public int GetWindowHandle(ulong handId, out EnumPokerSites site)
        {
            site = EnumPokerSites.BetOnline;
            return SessionCode;
        }

        public void Reset()
        {
        }

        public void ResetCache()
        {
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}