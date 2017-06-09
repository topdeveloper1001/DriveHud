//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Windows.Forms;

namespace IgnitionPipeDataEmulator
{
    internal class Logger
    {
        private static TextBox tbLog;

        public static void Configure(TextBox textBoxLog)
        {
            tbLog = textBoxLog;
        }

        public static void Log(string message, params object[] args)
        {
            if (tbLog.InvokeRequired)
            {
                tbLog.Invoke(new Action(() =>
                {
                    tbLog.AppendText(string.Format(message, args));
                    tbLog.AppendText(Environment.NewLine);
                }));
            }
            else
            {
                tbLog.AppendText(string.Format(message, args));
                tbLog.AppendText(Environment.NewLine);
            }
        }
    }
}