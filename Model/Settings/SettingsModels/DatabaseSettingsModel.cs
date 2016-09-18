using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    [Serializable]
    public class DatabaseSettings : SettingsBase
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public string LastDatabaseVersion { get; set; }
        public string TargetDatabaseVersion { get; set; }

        public DatabaseSettings()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            Server = "localhost";
            Port = "5432";
            User = "postgres";
            Password = "postgrespass";
            Database = "drivehud";
            LastDatabaseVersion = string.Empty;
        }
    }
}
