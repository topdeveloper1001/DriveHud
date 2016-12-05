using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Model.Importer;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class StringFormatter
    {
        private static ISettingsService SettingsService
        {
            get { return ServiceLocator.Current.GetInstance<ISettingsService>(); }
        }

        public static string GetAppDataFolderPath()
        {
            var appdata = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppDataFolder);
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appdata);
        }

        public static string GetProcessedDataFolderPath()
        {
            return Path.Combine(SettingsService.GetSettings().SiteSettings.ProcessedDataLocation);
        }

        public static string GetPlayersDataFolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.DefaultPlayersFolderName));
        }

        public static string GetActivePlayerFilePath()
        {
            return Path.Combine(GetPlayersDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.ActivePlayerFileName));
        }

        public static string ActionLineSeparator = "-";

        public static string GetDateString(DateTime utcDateTime)
        {
            return Converter.ToLocalizedDateTime(utcDateTime).ToString("MM/dd/yyyy");
        }

        public static string GetDateTimeString(DateTime utcDateTime)
        {
            return Converter.ToLocalizedDateTime(utcDateTime).ToString("MM/dd/yyyy hh.mm tt");
        }

        public static string GetReplayerHeaderString(string stakeLevel, DateTime handDate)
        {
            string header = CommonResourceManager.Instance.GetResourceString(ResourceStrings.ReplayerHeaderResourceString);
            return String.Format(header, stakeLevel, GetDateTimeString(handDate));
        }

        public static string GetConnectionString(string server, string port, string database, string user, string password)
        {
            return string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};Persist Security Info=true;", server, port, database, user, password);
        }
    }
}
