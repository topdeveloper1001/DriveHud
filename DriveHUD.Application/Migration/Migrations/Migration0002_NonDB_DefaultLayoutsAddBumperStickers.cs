using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using FluentMigrator;
using Model;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(2)]
    public class Migration0002_NonDB_DefaultLayoutsAddBumperStickers : Migration
    {
        private const string layoutsFileSettings = "Layouts.xml";

        public override void Up()
        {
            LogProvider.Log.Info("Running migration #2");
            var settingsFolder = StringFormatter.GetAppDataFolderPath();

            if (!Directory.Exists(settingsFolder))
            {
                LogProvider.Log.Info($"Folder {settingsFolder} not found.");

                return;
            }

            var layoutsFile = Path.Combine(settingsFolder, layoutsFileSettings);

            if (File.Exists(layoutsFile))
            {
                HudSavedLayouts hudLayouts;
                try
                {
                    using (var fs = File.Open(layoutsFile, FileMode.Open))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));

                        hudLayouts = xmlSerializer.Deserialize(fs) as HudSavedLayouts;
                    }

                    if (hudLayouts != null && hudLayouts.Layouts != null && hudLayouts.Layouts.Any())
                    {
                        hudLayouts.Layouts.Where(x => x.HudBumperStickerTypes.Count == 0).ForEach(x => x.HudBumperStickerTypes = CreateDefaultBumperStickers());

                        using (var fs = File.Open(layoutsFile, FileMode.Create))
                        {
                            var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));

                            xmlSerializer.Serialize(fs, hudLayouts);
                        }
                    }
                    else
                    {
                        LogProvider.Log.Info("Wasn't able to find any active layouts.");
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(ex);
                }
            }
            else
            {
                LogProvider.Log.Info($"File {layoutsFile} not found.");
            }

            LogProvider.Log.Info("Migration #2 executed.");
        }

        public override void Down()
        {
        }

        private List<HudBumperStickerType> CreateDefaultBumperStickers()
        {
            var bumperStickers = new List<HudBumperStickerType>()
            {
                new HudBumperStickerType(true)
                {
                     Name = "One and Done",
                     SelectedColor = Colors.OrangeRed,
                     Description = "C-bets at a high% on the flop, but then rarely double barrels.",
                     StatsToMerge =
                         new ObservableCollection<BaseHudRangeStat>()
                         {
                             new BaseHudRangeStat { Stat = Stat.CBet, Low = 55, High = 100 },
                             new BaseHudRangeStat { Stat = Stat.DoubleBarrel, Low = 0, High = 35 },
                         }
                },
                new HudBumperStickerType(true)
                {
                     Name = "Pre-Flop Reg",
                     SelectedColor = Colors.Orange,
                     Description = "Plays an aggressive pre-flop game, but doesn’t play well post flop.",
                     StatsToMerge =
                         new ObservableCollection<BaseHudRangeStat>()
                         {
                             new BaseHudRangeStat { Stat = Stat.VPIP, Low = 19, High = 26 },
                             new BaseHudRangeStat { Stat = Stat.PFR, Low = 15, High = 23 },
                             new BaseHudRangeStat { Stat = Stat.S3Bet, Low = 8, High = 100 },
                             new BaseHudRangeStat { Stat = Stat.WWSF, Low = 0, High = 42 },
                         }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Barrelling",
                    SelectedColor = Colors.Yellow,
                    Description = "Double and triple barrels a high percentage of the time.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat>()
                        {
                            new BaseHudRangeStat { Stat = Stat.VPIP, Low = 20, High = 30 },
                            new BaseHudRangeStat { Stat = Stat.PFR, Low = 17, High = 28 },
                            new BaseHudRangeStat { Stat = Stat.AGG, Low = 40, High = 49 },
                            new BaseHudRangeStat { Stat = Stat.CBet, Low = 65, High = 80 },
                            new BaseHudRangeStat { Stat = Stat.WWSF, Low = 44, High = 53 },
                            new BaseHudRangeStat { Stat = Stat.DoubleBarrel, Low = 46, High = 100 },
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "3 For Free",
                    SelectedColor = Colors.GreenYellow,
                    Description = "3-Bets too much, and folds to a 4-bet too often.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat>()
                        {
                            new BaseHudRangeStat { Stat = Stat.S3Bet, Low = 8.8m, High = 100 },
                            new BaseHudRangeStat { Stat = Stat.FoldTo3Bet, Low = 66, High = 100 },
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Way Too Early",
                    SelectedColor = Colors.Green,
                    Description = "Open raises too wide of a range in early pre-flop positions.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat>()
                        {
                            new BaseHudRangeStat { Stat = Stat.UO_PFR_EP, Low = 20, High = 100 },
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Sticky Fish",
                    SelectedColor = Colors.Blue,
                    Description = "Fishy player who can’t fold post flop if they get any piece of the board.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat>()
                        {
                            new BaseHudRangeStat { Stat = Stat.VPIP, Low = 35, High = 100 },
                            new BaseHudRangeStat { Stat = Stat.FoldToCBet, Low = 0, High = 40 },
                            new BaseHudRangeStat { Stat = Stat.WTSD, Low = 29, High = 100 },
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Yummy Fish",
                    SelectedColor = Colors.DarkBlue,
                    Description = "Plays too many hands pre-flop and isn’t aggressive post flop.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat>()
                        {
                            new BaseHudRangeStat { Stat = Stat.VPIP, Low = 40, High = 100 },
                            new BaseHudRangeStat { Stat = Stat.FoldToCBet, Low = 0, High = 6 },
                            new BaseHudRangeStat { Stat = Stat.AGG, Low = 0, High = 34 },
                        }
                },

            };

            return bumperStickers;
        }
    }
}
