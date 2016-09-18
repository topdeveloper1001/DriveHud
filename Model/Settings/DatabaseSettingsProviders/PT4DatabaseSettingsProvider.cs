using DriveHUD.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    public class PT4DatabaseSettingsProvider : BaseDatabaseSettingsProvider
    {
        private static string pt4RelativeConfigPath = @"PokerTracker 4\Config\PokerTracker.cfg";

        public static PT4DatabaseSettingsProvider GetPT4DatabaseSettings()
        {
            var model = new PT4DatabaseSettingsProvider();

            FillModelFromIniFile(model);

            return model;
        }

        private static void FillModelFromIniFile(PT4DatabaseSettingsProvider model)
        {
            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), pt4RelativeConfigPath);
            if (File.Exists(file))
            {
                var keyValuePairs = IniFileHelpers.ReadKeyValuePairs("Database", file);

                var server = GetKeyValue("Default.Postgres.Server", "Postgres.Server", keyValuePairs);
                var port = GetKeyValue("Default.Postgres.Port", "Postgres.Port", keyValuePairs);
                var user = GetKeyValue("Default.Postgres.User", "Postgres.User", keyValuePairs);
                var password = GetKeyValue("Default.Postgres.Password", "Postgres.Password", keyValuePairs);

                if (!string.IsNullOrWhiteSpace(server))
                {
                    model.Server = server;
                }

                if (!string.IsNullOrWhiteSpace(port))
                {
                    model.Port = port;
                }

                if (!string.IsNullOrWhiteSpace(user))
                {
                    model.UserName = user;
                }

                if (!string.IsNullOrWhiteSpace(password))
                {
                    model.Password = password;
                }

            }
        }

        /// <summary>
        /// Searchs provided array for specific key
        /// </summary>
        /// <param name="fullKeyName">Key</param>
        /// <param name="alternativeKey">Key to use in case if full key wasn't found</param>
        /// <param name="keyValuePairs">array of key value pairs</param>
        /// <returns>Value of key if match was found, or value of key that contains alternative key if full key wasn't found</returns>
        private static string GetKeyValue(string fullKeyName, string alternativeKey, string[] keyValuePairs)
        {
            if (keyValuePairs == null)
            {
                return null;
            }

            string alternativeValue = string.Empty;
            foreach (var keyValue in keyValuePairs)
            {
                if (string.IsNullOrWhiteSpace(keyValue))
                {
                    continue;
                }

                var key = keyValue.Split('=');
                if (key.FirstOrDefault() == fullKeyName)
                {
                    return key.LastOrDefault();
                }
                else if (key.Contains(alternativeKey))
                {
                    alternativeValue = key.LastOrDefault();
                }
            }

            return alternativeValue;
        }

    }
}
