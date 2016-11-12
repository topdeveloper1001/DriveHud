using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels;
using DriveHUD.Entities;
using Model.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DriveHUD.Common.Linq;
using Model.Site;

namespace DriveHud.Tests
{
    [TestFixture]
    class LayoutCreator
    {
        private const string layoutsPath = @"d:\Git\DriveHUD\DriveHUD.Common.Resources\Layouts\";
        private const string generatedLayoutsPath = @"d:\Git\DriveHUD\DriveHUD.Common.Resources\Layouts\New";
        private const string baseLayout = @"d:\Git\DriveHUD\DriveHUD.Common.Resources\Layouts\DH-10max-Basic-PS.xml";

        [Test]
        public void GenerateLayouts()
        {
            var pokerSite = EnumPokerSites.Poker888;

            var layoutsDirectory = new DirectoryInfo(layoutsPath);
            var generatedDirectory = ConfigureDirectories(generatedLayoutsPath);

            var poker888Configuration = new Poker888Configuration();

            var gameTypes = Enum.GetValues(typeof(EnumGameType)).Cast<EnumGameType>();

            var hudLayout = ReadHudLayout(baseLayout);

            foreach (var gameType in gameTypes)
            {
                foreach (var tableType in poker888Configuration.TableTypes)
                {
                    try
                    {
                        UpdateLayout(pokerSite, gameType, tableType, hudLayout);

                        var layoutFileName = BuildLayoutFileName(gameType, tableType);
                        var layoutFileFullName = Path.Combine(generatedDirectory.FullName, layoutFileName);

                        WriteHudLayout(hudLayout, layoutFileFullName);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

        }

        private string BuildLayoutFileName(EnumGameType gameType, EnumTableType tableType)
        {
            var tableTypeOmahaSuffix = GetTableTypeOmahaSuffix(gameType, tableType);
            var gameTypeText = !string.IsNullOrEmpty(tableTypeOmahaSuffix.Item1) ? tableTypeOmahaSuffix.Item1.Replace(" ", "-") : tableTypeOmahaSuffix.Item1;
            var omahaPostfix = !string.IsNullOrEmpty(tableTypeOmahaSuffix.Item2) ? tableTypeOmahaSuffix.Item2.Replace(" ", "-") : tableTypeOmahaSuffix.Item2;

            var fileName = string.Format("DH-{0}max{1}-Basic{2}-888.xml", (int)tableType, gameTypeText, omahaPostfix);

            return fileName;
        }
        private string BuildLayoutName(EnumGameType gameType, EnumTableType tableType)
        {
            var tableTypeOmahaSuffix = GetTableTypeOmahaSuffix(gameType, tableType);

            var layoutName = string.Format("DriveHUD: {0}-max{1} Basic{2}", (int)tableType, tableTypeOmahaSuffix.Item1, tableTypeOmahaSuffix.Item2);

            return layoutName;
        }

        private Tuple<string, string> GetTableTypeOmahaSuffix(EnumGameType gameType, EnumTableType tableType)
        {
            var omahaPostfix = gameType.ToString().IndexOf("Omaha", StringComparison.InvariantCultureIgnoreCase) > 0 ? " OH" : string.Empty;
            var gameTypeText = gameType.ToString().Replace("Omaha", string.Empty).Replace("Holdem", string.Empty);
            gameTypeText = gameTypeText.Equals("Cash") ? string.Empty : $" {gameTypeText}";

            return new Tuple<string, string>(gameTypeText, omahaPostfix);
        }

        private void UpdateLayout(EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType, HudSavedLayout hudLayout)
        {
            var newHash = HudViewModel.GetHash(pokerSite, gameType, tableType);
            hudLayout.LayoutId = newHash;
            hudLayout.Name = BuildLayoutName(gameType, tableType);

            var tableConfigurator = new Poker888TableConfigurator();
            var hudElements = tableConfigurator.GenerateElements((int)tableType);

            hudLayout.HudPositions = hudElements.Select(y => new HudSavedPosition
            {
                Height = y.Height,
                Position = y.Position,
                Width = y.Width,
                Seat = y.Seat,
                HudType = y.HudType
            }).ToList();
        }

        private HudSavedLayout ReadHudLayout(string layoutFile)
        {
            using (var fs = File.Open(layoutFile, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));

                var hudLayouts = xmlSerializer.Deserialize(fs) as HudSavedLayouts;

                var hudLayout = hudLayouts.Layouts.FirstOrDefault();

                return hudLayout;
            }
        }

        private void WriteHudLayout(HudSavedLayout hudSavedLayout, string layoutFile)
        {
            var hudSavedLayouts = new HudSavedLayouts();
            hudSavedLayouts.Layouts.Add(hudSavedLayout);

            using (var fs = File.Open(layoutFile, FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));
                xmlSerializer.Serialize(fs, hudSavedLayouts);
            }
        }

        private EnumGameType ParseGameType(string name)
        {
            var isOmaha = name.IndexOf("-OH-", StringComparison.InvariantCultureIgnoreCase) > 0;

            if (name.IndexOf("-SNG-", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return isOmaha ? EnumGameType.SNGOmaha : EnumGameType.SNGHoldem;
            }

            if (name.IndexOf("-MTT-", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return isOmaha ? EnumGameType.MTTOmaha : EnumGameType.MTTHoldem;
            }

            return isOmaha ? EnumGameType.CashOmaha : EnumGameType.CashHoldem;
        }

        private EnumTableType ParseTableType(string name)
        {
            var nameSplitted = name.Split('-');

            var tableSize = int.Parse(nameSplitted[1].Replace("max", string.Empty));

            var tableType = (EnumTableType)tableSize;

            return tableType;
        }

        private DirectoryInfo ConfigureDirectories(string generatedLayoutsPath)
        {
            var generatedDirectory = new DirectoryInfo(generatedLayoutsPath);

            if (generatedDirectory.Exists && generatedDirectory.GetFiles().Any())
            {
                throw new InvalidOperationException("Directory exists and not empty");
            }
            else if (!generatedDirectory.Exists)
            {
                Directory.CreateDirectory(generatedLayoutsPath);
            }

            return generatedDirectory;
        }
    }
}
