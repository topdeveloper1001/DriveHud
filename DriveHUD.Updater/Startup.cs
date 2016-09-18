//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DriveHUD.Updater.Core;

namespace DriveHUD.Updater
{
    class Startup
    {
        [STAThread]
        public static void Main()
        {           
            //use same as MainApp guid to prevent running updater and app together
            //if (SingleInstance<App>.InitializeAsFirstInstance(DriveHUDUpdaterPaths.MainApplicationGuid, false))
            //{
                if (CertificateHelper.Verify(DriveHUDUpdaterPaths.MainApplicationProccess))
                {
                    var application = new App();                    
                    application.Run();
                }
                else
                {
                    MessageBox.Show("Certificate is invalid. Updater couldn't be run.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            //    SingleInstance<App>.Cleanup();
            //}
        }
    }
}