#region Usings

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace AcePokerSolutions.ClientHelpers.PokerTracker
{
    public class PokerTrackerConfiguration
    {
        //public PokerTrackerConfiguration()
        //{
        //}

        //public PokerTrackerConfiguration(string filePath)
        //{
        //    FilePath = filePath;
        //    Load();
        //}

        private string FilePath { get; set; }

        public bool CheckPtInstalled()
        {
            try
            {
                LoadFromDefaultPath();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void LoadFromDefaultPath()
        {
            FilePath = GetHMConfigPath();
            Load();
        }

        private static String GetHMConfigPath()
        {
            string fn = "Program Files\\PokerTracker 3\\Data\\Config\\PokerTracker.cfg";
            char let = 'c';
            string fnn = let + ":\\" + fn;
            while (!File.Exists(fnn) && (let < 'z'))
            {
                let = (char) (let + 1);
                fnn = let + ":\\" + fn;
            }

            if (!File.Exists(fnn))
            {
                fn = "Program Files (x86)\\PokerTracker 3\\Data\\Config\\PokerTracker.cfg";
                let = 'c';
                fnn = let + ":\\" + fn;
                while (!File.Exists(fnn) && (let < 'z'))
                {
                    let = (char) (let + 1);
                    fnn = let + ":\\" + fn;
                }
            }

            if (!File.Exists(fnn))
                throw new Exception("Config not found");
            return fnn;
        }

        private void Load()
        {
            IniFile ini = new IniFile(FilePath);

            HostName = ini.IniReadValue("Database", "DB1.Postgres.Server");
            CurrentDatabase = ini.IniReadValue("Database", "DB1.Postgres.Database");
            UserId = ini.IniReadValue("Database", "DB1.Postgres.User");
            Pass = ini.IniReadValue("Database", "DB1.Postgres.Password");
            Port = ini.IniReadValue("Database", "DB1.Postgres.Port");
        }

        #region Defined Properties

        private string HostName { get; set; }

        public string CurrentDatabase { get; private set; }

        private string UserId { get; set; }

        private string Pass { get; set; }

        private string Port { get; set; }

        public string ConnectionString => $"SERVER={HostName};PORT={Port};UID={UserId};PWD={Pass}";

        #endregion
    }

    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        private readonly string m_path;

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <param name="iniPath"></param>
        public IniFile(string iniPath)
        {
            m_path = iniPath;
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                          int size, string filePath);

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string IniReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, m_path);
            return temp.ToString();
        }
    }
}