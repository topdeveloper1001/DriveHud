using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    public class BaseDatabaseSettingsProvider
    {
        public string Server { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "5432";
        public string UserName { get; set; } = "postgres";
        public string Password { get; set; } = "postgrespass";

        public override string ToString()
        {
            return string.Format("Server={0};Port={1};User Id={2};Password={3};", Server, Port, UserName, Password);
        }
    }
}
