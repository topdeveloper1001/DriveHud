//-----------------------------------------------------------------------
// <copyright file="UpdaterPaths.cs" company="Ace Poker Solutions">
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
using System.IO;
using System.Reflection;

namespace DriveHUD.Updater.Core
{
    public static class UpdaterPaths
    {
        private const string updaterBasePath = "http://pokerleakbuster.com/Updates/";
        private const string updaterDataPath = "dhupdater-test.xml";
        private const string appDataFolder = "DriveHUD";
        private const string tempUpdatesFolder = "_updates";
        private const string tempUnpackedFolder = "_unpacked";

        public static string GetUpdaterDataPath()
        {
            var uri = new Uri(new Uri(updaterBasePath), updaterDataPath);
            return uri.AbsoluteUri;
        }

        public static string GetUpdatePath(string path)
        {
            var uri = new Uri(new Uri(updaterBasePath), path);
            return uri.AbsoluteUri;
        }

        public static string GetAppDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDataFolder);
        }

        public static string GetTempUpdatesFolder()
        {
            return Path.Combine(GetAppDataFolderPath(), tempUpdatesFolder);
        }

        public static string GetTempUnpackedFolder()
        {
            return Path.Combine(GetTempUpdatesFolder(), tempUnpackedFolder);
        }

        public static string GetCurrentFolder()
        {
            return Assembly.GetEntryAssembly().Location;
        }
    }
}