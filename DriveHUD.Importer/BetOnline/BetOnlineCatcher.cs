﻿//-----------------------------------------------------------------------
// <copyright file="BetOnlineCatcher.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.BetOnline
{
    /// <summary>
    /// Class for inject catching DLL in BetOnline processes
    /// </summary>
    public class BetOnlineCatcher : PokerCatcher, IBetOnlineCatcher
    {
        /// Dll to be injected
        /// </summary>
        private const string dllToInject = "CapPipedB.dll";

        /// <summary>
        /// Name of process where dll will be injected
        /// </summary>
        private const string processName = "GameClient";

        #region PokerCatcher members

        protected override string DllToInject
        {
            get
            {
                return dllToInject;
            }
        }

        protected override string ProcessName
        {
            get
            {
                return processName;
            }
        }

        #endregion 
    }
}