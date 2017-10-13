//-----------------------------------------------------------------------
// <copyright file="NotesAppSettingsHelper.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using AcePokerSolutions.Helpers;
using DriveHUD.Common.Log;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings
{
    [Obsolete]
    public static class NotesAppSettingsHelper
    {
        #region Private Members

        private static string m_appDataPath;
        private static string m_configurationFilePath;
        #endregion

        #region Public Properties

        public static NotesAppSettings CurrentNotesAppSettings { get; private set; }

        public static ContentControl MainWindow { get; set; }

        private static string AppDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_appDataPath))
                    m_appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                    "\\AcePokerSolutions\\";
                return m_appDataPath;
            }
        }

        #endregion

        #region Private Properties

        private static string ConfigurationFilePath
        {
            get
            {
                m_configurationFilePath = Path.Combine(AppDataPath, "NotesAppSettings.xml");

                if (!Directory.Exists(AppDataPath))
                    Directory.CreateDirectory(AppDataPath);
                return m_configurationFilePath;
            }
        }

        #endregion

        #region Public Methods

        public static void SaveAppSettings()
        {
            try
            {
                var xml = Serializer.ToXml(CurrentNotesAppSettings, typeof(NotesAppSettings));
                File.WriteAllText(ConfigurationFilePath, xml);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(NotesAppSettingsHelper), $"Couldn't save setting into '{ConfigurationFilePath}'", e);
            }
        }

        public static void LoadAppSettings()
        {
            try
            {
                string xml = File.ReadAllText(ConfigurationFilePath);
                CurrentNotesAppSettings = (NotesAppSettings)Serializer.FromXml(xml, typeof(NotesAppSettings));
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(NotesAppSettingsHelper), "Could not find existing configuration file", ex);
            }

            if (CurrentNotesAppSettings == null)
            {
                CurrentNotesAppSettings = new NotesAppSettings();
                SaveAppSettings();
            }

            if (CurrentNotesAppSettings.StagesList == null ||
                CurrentNotesAppSettings.StagesList.Count == 0)
            {
                InitializeDefaultNotes();
            }
        }

        #endregion

        #region Private Methods

        private static void InitializeDefaultNotes()
        {
            string xml = File.ReadAllText(Path.Combine(
                AssemblyHelpers.GetRunningPath(Assembly.GetExecutingAssembly()),
                "config\\defaultNotes.xml"));

            NotesAppSettings settings = (NotesAppSettings)Serializer.FromXml(xml, typeof(NotesAppSettings));
            CurrentNotesAppSettings.StagesList = settings.StagesList;
            CurrentNotesAppSettings.Profiles = settings.Profiles;

            SaveAppSettings();
        }

        #endregion
    }
}