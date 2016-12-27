using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Model.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Model.Settings
{
    public class SettingsService : ISettingsService
    {
        private const string _settingsFileName = "Settings.xml";

        private static readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();

        private readonly string _settingsFolder;
        private string _settingsFile;
        private SettingsModel _settingsModel;
        private bool _skipFileCreation;

        #region Constructor

        public SettingsService(string settingsFolder, bool skipFileCreation)
        {
            _settingsFolder = settingsFolder;
            _skipFileCreation = skipFileCreation;

            Initialize();
        }

        public SettingsService(string settingsFolder) : this(settingsFolder, false)
        {
        }

        private void Initialize()
        {
            try
            {
                if (!Directory.Exists(_settingsFolder) && !_skipFileCreation)
                {
                    Directory.CreateDirectory(_settingsFolder);
                }

                _settingsFile = Path.Combine(_settingsFolder, _settingsFileName);

                if (!TryLoad())
                {
                    SaveSettings(new SettingsModel());
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        #endregion

        #region Methods

        private bool TryLoad()
        {
            try
            {
                using (rwlock.Write())
                {
                    if (File.Exists(_settingsFile))
                    {
                        using (var fs = File.Open(_settingsFile, FileMode.Open))
                        {
                            var xmlSerializer = new XmlSerializer(typeof(SettingsModel));
                            _settingsModel = xmlSerializer.Deserialize(fs) as SettingsModel;
                            return _settingsModel != null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, ex);
            }

            return false;
        }

        /// <summary>
        /// Saves user settings
        /// </summary>
        /// <param name="settings"></param>
        public void SaveSettings(SettingsModel settings)
        {
            if (settings == null)
            {
                return;
            }

            using (rwlock.Write())
            {
                _settingsModel = (SettingsModel)settings.Clone();

                if (_skipFileCreation)
                {
                    return;
                }

                try
                {
                    if (!Directory.Exists(_settingsFolder))
                    {
                        Directory.CreateDirectory(_settingsFolder);
                    }

                    _settingsFile = Path.Combine(_settingsFolder, _settingsFileName);

                    using (var fs = File.Open(_settingsFile, FileMode.Create))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(SettingsModel));
                        xmlSerializer.Serialize(fs, _settingsModel);
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, e);
                }
            }
        }

        /// <summary>
        /// Gets user settings
        /// </summary>
        /// <returns></returns>
        public SettingsModel GetSettings()
        {
            using (rwlock.Read())
            {
                return (SettingsModel)_settingsModel.Clone();
            }
        }

        #endregion
    }
}
