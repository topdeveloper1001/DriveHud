//-----------------------------------------------------------------------
// <copyright file="PlayerXRayNoteService.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.PlayerXRay.BusinessHelper;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DriveHUD.PlayerXRay.Services
{
    internal class PlayerXRayNoteService : IPlayerXRayNoteService
    {
        private string configurationFile;

        private static object locker = new object();

        public PlayerXRayNoteService()
        {
            configurationFile = Path.Combine(StringFormatter.GetAppDataFolderPath(), "NotesAppSettings.xml");
            LoadAppSettings();
        }

        public IEnumerable<Playernotes> BuildNotes(Playerstatistic stats, HandHistory handHistory)
        {
            if (CurrentNotesAppSettings == null || !CurrentNotesAppSettings.AutoNotesEnabled)
            {
                return null;
            }

            var noteProcessingService = ServiceLocator.Current.GetInstance<INoteProcessingService>();

            try
            {
                var playerNotes = noteProcessingService.ProcessHand(CurrentNotesAppSettings.AllNotes, stats, handHistory);
                return playerNotes;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, $"Could not build notes", e);
                return null;
            }
        }

        #region Properties

        public NotesAppSettings CurrentNotesAppSettings { get; private set; }

        #endregion

        public void SaveAppSettings()
        {
            lock (locker)
            {
                try
                {
                    var xml = Serializer.ToXml(CurrentNotesAppSettings, typeof(NotesAppSettings));
                    File.WriteAllText(configurationFile, xml);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, $"Couldn't save settings in '{configurationFile}'", e);
                }
            }
        }

        public void LoadAppSettings()
        {
            lock (locker)
            {
                try
                {
                    if (File.Exists(configurationFile))
                    {
                        var xml = File.ReadAllText(configurationFile);
                        CurrentNotesAppSettings = (NotesAppSettings)Serializer.FromXml(xml, typeof(NotesAppSettings));
                    }
                    else
                    {
                        LogProvider.Log.Info(CustomModulesNames.PlayerXRay, "Could not find existing configuration file. Load defaults.");
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Could not load existing configuration file. Load defaults.", ex);
                }

                if (CurrentNotesAppSettings == null || CurrentNotesAppSettings.StagesList == null ||
                    CurrentNotesAppSettings.StagesList.Count == 0)
                {
                    InitializeDefaultNotes();
                }
            }
        }

        private void InitializeDefaultNotes()
        {
            try
            {
                var resourcesAssembly = typeof(PlayerXRayNoteService).Assembly;

                var resourceName = "DriveHUD.PlayerXRay.Resources.DefaultNotes.xml";

                using (var stream = resourcesAssembly.GetManifestResourceStream(resourceName))
                {
                    var xmlSerializer = new XmlSerializer(typeof(NotesAppSettings));
                    CurrentNotesAppSettings = xmlSerializer.Deserialize(stream) as NotesAppSettings;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Could not initialize default notes. Empty notes will be created.", e);
                CurrentNotesAppSettings = new NotesAppSettings();
            }

            SaveAppSettings();
        }
    }
}