using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AcePokerSolutions.ClientHelpers.DriveHUD
{
    public class DriveHudConfiguration
    {
        private string FilePath { get; set; }

        public bool CheckDhInstalled()
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

        private void LoadFromDefaultPath()
        {
            FilePath = GetDHConfigPath();
        }

        private string GetDHConfigPath()
        {
            string fn = "Program Files\\Ace Poker Solutions\\DriveHUD\\DriveHUD.Application.exe";
            char let = 'c';
            string fnn = let + ":\\" + fn;
            while (!File.Exists(fnn) && (let < 'z'))
            {
                let = (char)(let + 1);
                fnn = let + ":\\" + fn;
            }

            if (!File.Exists(fnn))
            {
                fn = "Program Files (x86)\\Ace Poker Solutions\\DriveHUD\\DriveHUD.Application.exe";
                let = 'c';
                fnn = let + ":\\" + fn;
                while (!File.Exists(fnn) && (let < 'z'))
                {
                    let = (char)(let + 1);
                    fnn = let + ":\\" + fn;
                }
            }

            if (!File.Exists(fnn))
                throw new Exception("Config not found");
            return fnn;
        }


        [Serializable]       
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        public class DriveHudConfigurationSetting
        {
            [XmlAttribute("name", Form = XmlSchemaForm.Unqualified)]
            public string Name { get; set; }

            [XmlText]
            public string Value { get; set; }
        }
    }
}
