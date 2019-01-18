//-----------------------------------------------------------------------
// <copyright file="PokerBaaziCatcher.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Linq;

namespace DriveHUD.Importers.PokerBaazi
{
    /// <summary>
    /// Class for injecting catching DLL in PokerBaazi processes
    /// </summary>
    internal class PokerBaaziCatcher : PokerCatcher, IPokerBaaziCatcher
    {
        /// <summary>
        /// Dll to be injected
        /// </summary>
        private const string dllToInject = "CapPipedPB.dll";

        /// <summary>
        /// Name of process in which dll will be injected
        /// </summary>
        private const string processName = "PokerBaazi";

        /// <summary>
        /// Class of window in which dll will be injected
        /// </summary>
        private const string windowClassName = "Chrome_WidgetWin";

        /// <summary>
        /// Pattern to check title
        /// </summary>
        private static readonly string[] titleMatchPatterns = new[]
        {
            "PokerBaazi"          
        };

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
                return ImporterIdentifier.PokerBaazi;
            }
        }

        protected override ImporterIdentifier[] PipeIdentifiers
        {
            get
            {
                return new[] { ImporterIdentifier.PokerBaazi };
            }
        }

        protected override bool IsWindowMatch(string windowTitle, string windowClassName)
        {
            return windowClassName.IndexOf(WindowClassName, StringComparison.OrdinalIgnoreCase) >= 0 &&
                !string.IsNullOrEmpty(windowTitle) &&
                titleMatchPatterns.Any(titlePattern => windowTitle.IndexOf(titlePattern, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        protected override bool IsEnabled()
        {           
            return true;
        }       

        #endregion
    }
}