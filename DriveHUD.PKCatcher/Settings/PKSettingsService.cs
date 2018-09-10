﻿//-----------------------------------------------------------------------
// <copyright file="PKSettingsService.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using Model;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;

namespace DriveHUD.PKCatcher.Settings
{
    internal class PKSettingsService : IPKSettingsService
    {
        private readonly static string settingsFileName = Path.Combine(StringFormatter.GetAppDataFolderPath(), "PKSettings.xml");

        private static readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();

        private PKSettingsModel settingsModel;

        public PKSettingsService()
        {
            Initialize();
        }

        public PKSettingsModel GetSettings()
        {
            using (rwlock.Read())
            {
                return settingsModel.Clone();
            }
        }

        public void SaveSettings(PKSettingsModel settings)
        {
            if (settings == null)
            {
                return;
            }

            using (rwlock.Write())
            {
                settingsModel = settings.Clone();

                try
                {
                    var serializer = new DataContractSerializer(typeof(PKSettingsModel));

                    using (var fs = File.Open(settingsFileName, FileMode.Create))
                    {
                        using (var writer = new XmlTextWriter(fs, Encoding.UTF8))
                        {
                            writer.Formatting = Formatting.Indented;
                            serializer.WriteObject(writer, settingsModel);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, e);
                }
            }
        }

        private void Initialize()
        {
            try
            {
                if (!TryLoad())
                {
                    SaveSettings(new PKSettingsModel());
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Could not initialize settings", e);
            }
        }

        private bool TryLoad()
        {
            try
            {
                using (rwlock.Write())
                {
                    if (File.Exists(settingsFileName))
                    {
                        using (var fs = File.Open(settingsFileName, FileMode.Open))
                        {
                            var serializer = new DataContractSerializer(typeof(PKSettingsModel));
                            settingsModel = serializer.ReadObject(fs) as PKSettingsModel;
                            return settingsModel != null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Could not load settings.", ex);
            }

            return false;
        }
    }
}