using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    public class HM2DatabaseSettingsProvider : BaseDatabaseSettingsProvider
    {
        public static HM2DatabaseSettingsProvider GetHM2DatabaseSettings()
        {
            var model = new HM2DatabaseSettingsProvider();

            var server = GetHM2RegistryValues("DatabaseServer");
            var port = GetHM2RegistryValues("DatabasePort");
            var user = GetHM2RegistryValues("DatabaseUser");
            var password = GetHM2RegistryValues("DatabasePassword");

            if(!string.IsNullOrWhiteSpace(server))
            {
                model.Server = server;
            }

            if(!string.IsNullOrWhiteSpace(port))
            {
                model.Port = port;
            }

            if(!string.IsNullOrWhiteSpace(user))
            {
                model.UserName = user;
            }

            if(!string.IsNullOrWhiteSpace(password))
            {
                model.Password = password;
            }

            return model;
        }

        private static string GetHM2RegistryValues(string valueName)
        {
            return (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\PASG\\HoldemManager", valueName, null);
        }
    }
}
