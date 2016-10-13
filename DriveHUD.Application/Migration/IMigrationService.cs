using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService
{
    internal interface IMigrationService
    {
        void MigrateToLatest(string connectionString);
    }
}
