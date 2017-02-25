//-----------------------------------------------------------------------
// <copyright file="BovadaCatcher.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Class for inject catching DLL in Bovada or Bodog processes
    /// </summary>
    internal class BovadaCatcher : PokerCatcher, IBovadaCatcher
    {
        /// <summary>
        /// Dll to be injected
        /// </summary>
        private const string dllToInject = "CapPipedI.dll";

        /// <summary>
        /// Name of process in which dll will be injected
        /// </summary>
        private const string processName = "Lobby";

        /// <summary>
        /// Class of window in which dll will be injected
        /// </summary>
        private const string windowClassName = "Qt5QWindowIcon";

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

        protected override string WindowClassName
        {
            get
            {
                return windowClassName;
            }
        }

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.Bovada;
            }
        }

        #endregion
    }
}