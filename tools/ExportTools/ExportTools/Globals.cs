using System;
using System.IO;

namespace ExportTools
{
    internal static class Globals
    {
        public const string AppDataFolder = "DriveHUD";

        public const string DbFileName = "drivehud.db";

        public static string GetAppDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolder);
        }

        public static string GetSQLiteDbFilePath()
        {
            var appData = GetAppDataFolderPath();
            return Path.Combine(appData, DbFileName);
        }

        public static string GetSQLiteConnectionString()
        {
            var dbFile = GetSQLiteDbFilePath();
            return $"Data Source={dbFile};Version=3;foreign keys=true;journal_mode=WAL;";
        }
    }
}