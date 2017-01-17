//-----------------------------------------------------------------------
// <copyright file="HudLayoutsService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DriveHUD.Common.Linq;
using DriveHUD.ViewModels;
using System.Collections.ObjectModel;
using Model.Enums;
using System.Reflection;
using DriveHUD.Common.Resources;
using DriveHUD.Common;
using Model;
using System.Windows.Media;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Entities;
using Telerik.Windows.Data;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class HudLayoutsService : IHudLayoutsService
    {
        private const string LayoutsFolderName = "Layouts";
        private const string LayoutFileExtension = ".xml";
        private const string PathToImages = @"data\PlayerTypes";

        public List<HudLayoutInfo> Layouts { get; private set; }

        public List<HudTableViewModel> HudTableViewModels { get; set; }

        public HudLayoutsService()
        {
            Initialize();
        }

        #region private classes
        private class MatchRatio
        {
            public HudPlayerType PlayerType { get; set; }

            public bool IsInRange { get; set; }

            public decimal Ratio { get; set; }

            public decimal ExtraRatio { get; set; }
        }

        private class PlayerMatchRatios
        {
            public HudElementViewModel HudElement { get; set; }

            public List<MatchRatio> MatchRatios { get; set; }

            public List<MatchRatio> ExtraMatchRatios { get; set; }
        }
        #endregion

        #region private methods

        private void Initialize()
        {
            Layouts = new List<HudLayoutInfo>();
            try
            {
                var layoutsDirectory = GetLayoutsDirectory();
                foreach (
                    var layoutFile in
                    layoutsDirectory.GetFiles()
                        .Where(
                            f =>
                                string.Equals(f.Extension, LayoutFileExtension,
                                    StringComparison.InvariantCultureIgnoreCase)))
                {
                    using (var fs = File.Open(layoutFile.FullName, FileMode.Open))
                    {
                        var layout = LoadLayoutFromStream(fs);
                        if (layout != null)
                            Layouts.Add(layout);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            if (Layouts.Any(l=>l.IsDefault))
                return;
            var predefindedLayout = GetPredefindedLayout();
            if (predefindedLayout == null)
                return;
            Layouts.Add(predefindedLayout);
            InternalSave(predefindedLayout);
        }

        private HudLayoutInfo GetPredefindedLayout()
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;
            try
            {
                using (
                    var stream =
                        resourcesAssembly.GetManifestResourceStream("DriveHUD.Common.Resources.Layouts.Default.xml"))
                {
                    return LoadLayoutFromStream(stream);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
            return null;
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
                            new BaseHudRangeStat {Stat = Stat.CBet, Low = 55, High = 100},
                            new BaseHudRangeStat {Stat = Stat.DoubleBarrel, Low = 0, High = 35},
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
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 19, High = 26},
                            new BaseHudRangeStat {Stat = Stat.PFR, Low = 15, High = 23},
                            new BaseHudRangeStat {Stat = Stat.S3Bet, Low = 8, High = 100},
                            new BaseHudRangeStat {Stat = Stat.WWSF, Low = 0, High = 42},
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
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 20, High = 30},
                            new BaseHudRangeStat {Stat = Stat.PFR, Low = 17, High = 28},
                            new BaseHudRangeStat {Stat = Stat.AGG, Low = 40, High = 49},
                            new BaseHudRangeStat {Stat = Stat.CBet, Low = 65, High = 80},
                            new BaseHudRangeStat {Stat = Stat.WWSF, Low = 44, High = 53},
                            new BaseHudRangeStat {Stat = Stat.DoubleBarrel, Low = 46, High = 100},
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "3 For Free",
                    SelectedColor = Colors.GreenYellow,
                    Description = "3-Bets too much, and folds to a 3-bet too often.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat>()
                        {
                            new BaseHudRangeStat {Stat = Stat.S3Bet, Low = 8.8m, High = 100},
                            new BaseHudRangeStat {Stat = Stat.FoldTo3Bet, Low = 66, High = 100},
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Way Too Early",
                    SelectedColor = Colors.Green,
                    Description = "Open raises to wide of a range in early pre-flop positions.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat>()
                        {
                            new BaseHudRangeStat {Stat = Stat.UO_PFR_EP, Low = 20, High = 100},
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
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 35, High = 100},
                            new BaseHudRangeStat {Stat = Stat.FoldToCBet, Low = 0, High = 40},
                            new BaseHudRangeStat {Stat = Stat.WTSD, Low = 29, High = 100},
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
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 40, High = 100},
                            new BaseHudRangeStat {Stat = Stat.FoldToCBet, Low = 0, High = 6},
                            new BaseHudRangeStat {Stat = Stat.AGG, Low = 0, High = 34},
                        }
                },
            };

            return bumperStickers;
        }

        private Tuple<bool, decimal, decimal> GetMatchRatio(HudElementViewModel hudElement, HudPlayerType hudPlayerType)
        {
            var matchRatios = (from stat in hudPlayerType.Stats
                               let low = stat.Low ?? -1
                               let high = stat.High ?? 100
                               let average = (high + low) / 2
                               let isStatDefined = stat.Low.HasValue || stat.High.HasValue
                               join hudElementStat in hudElement.StatInfoCollection on stat.Stat equals hudElementStat.Stat into gj
                               from grouped in gj.DefaultIfEmpty()
                               let inRange =
                               grouped != null ? (grouped.CurrentValue >= low && grouped.CurrentValue <= high) : !isStatDefined
                               let isGroupAndStatDefined = grouped != null && isStatDefined
                               let matchRatio = isGroupAndStatDefined ? Math.Abs(grouped.CurrentValue - average) : 0
                               let extraMatchRatio =
                               (isGroupAndStatDefined && (grouped.Stat == Stat.VPIP || grouped.Stat == Stat.PFR)) ? matchRatio : 0
                               select
                               new
                               {
                                   Ratio = matchRatio,
                                   InRange = inRange,
                                   IsStatDefined = isStatDefined,
                                   ExtraMatchRatio = extraMatchRatio
                               }).ToArray();

            return
                new Tuple<bool, decimal, decimal>(
                    matchRatios.All(x => x.InRange) && matchRatios.Any(x => x.IsStatDefined),
                    matchRatios.Sum(x => x.Ratio), matchRatios.Sum(x => x.ExtraMatchRatio));
        }

        private bool IsInRange(HudElementViewModel hudElement, IEnumerable<BaseHudRangeStat> rangeStats,
            Model.Data.HudIndicators source)
        {
            if (!rangeStats.Any(x => x.High.HasValue || x.Low.HasValue))
                return false;

            foreach (var rangeStat in rangeStats)
            {
                if (!rangeStat.High.HasValue && !rangeStat.Low.HasValue)
                    continue;

                var stat = hudElement.StatInfoCollection.FirstOrDefault(x => x.Stat == rangeStat.Stat);

                if (stat == null)
                    return false;

                var currentStat = new StatInfo {PropertyName = stat.PropertyName};
                currentStat.AssignStatInfoValues(source);

                var high = rangeStat.High ?? 100;
                var low = rangeStat.Low ?? -1;


                if (currentStat.CurrentValue < low || currentStat.CurrentValue > high)
                {
                    return false;
                }
            }

            return true;
        }

        private decimal GetStatAverageValue(HudPlayerTypeStat stat)
        {
            var low = stat.Low ?? 0;
            var high = stat.High ?? 100;

            return (high + low) / 2;
        }

        private DirectoryInfo GetLayoutsDirectory()
        {
            var layoutsDirectory =
                  new DirectoryInfo(Path.Combine(StringFormatter.GetAppDataFolderPath(), LayoutsFolderName));
            if (!layoutsDirectory.Exists)
                layoutsDirectory.Create();
            return layoutsDirectory;
        }

        private void InternalSave(HudLayoutInfo hudLayoutInfo)
        {
            var layoutsDirectory = GetLayoutsDirectory();
            InternalSave(hudLayoutInfo, layoutsDirectory.FullName);
        }

        private void InternalSave(HudLayoutInfo hudLayoutInfo, string layoutsDirectory)
        {
            var layoutsFile = hudLayoutInfo.IsDefault
                ? Path.Combine(layoutsDirectory, $"Default{LayoutFileExtension}")
                : Path.Combine(layoutsDirectory,
                    $"{Path.GetInvalidFileNameChars().Aggregate(hudLayoutInfo.Name, (current, c) => current.Replace(c.ToString(), string.Empty))}{LayoutFileExtension}");
            using (var fs = File.Open(layoutsFile, FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                xmlSerializer.Serialize(fs, hudLayoutInfo);
            }
        }

        private void InternalSaveAll()
        {
            try
            {
                var layoutsDirectory = GetLayoutsDirectory();
                foreach (var hudLayoutInfo in Layouts)
                {
                    InternalSave(hudLayoutInfo, layoutsDirectory.FullName);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        /// <summary>
        /// Import layouts from stream
        /// </summary>
        /// <param name="stream">Stream to be used for importing</param>
        /// <returns>Imported layout</returns>
        private HudLayoutInfo LoadLayoutFromStream(Stream stream)
        {
            Check.ArgumentNotNull(() => stream);

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                var layout = xmlSerializer.Deserialize(stream) as HudLayoutInfo;

                // set image after loading
                layout?.HudTableDefinedProperties.Where(x => x.HudPlayerTypes != null)
                    .SelectMany(x => x.HudPlayerTypes)
                    .ForEach(x =>
                    {
                        if (x != null)
                        {
                            x.Image = GetImageLink(x.ImageAlias);
                        }
                    });

                layout?.HudTableDefinedProperties.Where(x => x.HudPlayerTypes != null)
                    .SelectMany(x => x.HudBumperStickerTypes)
                    .ForEach(x =>
                    {
                        x?.InitializeFilterPredicate();
                    });

                return layout;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            return null;
        }

        private List<HudPlayerType> CreateDefaultPlayerTypes(EnumTableType tableType)
        {
            var hudPlayerTypes = new List<HudPlayerType>
            {
                new HudPlayerType(true)
                {
                    Name = "Nit",
                    ImageAlias = "Nit.png",
                    Image = GetImageLink("Nit.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 0, High = 17},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 16},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 4.3m}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 0, High = 11},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 11},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 3.7m}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                    Name = "Fish",
                    ImageAlias = "Fish.png",
                    Image = GetImageLink("Fish.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 36},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 13},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 4},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 0, High = 40}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 31},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 11},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 3.8m},
                                new HudPlayerTypeStat {Stat = Stat.AGG, Low = 0, High = 40}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                    Name = "Standard Reg",
                    ImageAlias = "Standard Reg.png",
                    Image = GetImageLink("Standard Reg.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 22, High = 27},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 18, High = 25},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 4.7m, High = 8.6m,},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 42}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 16, High = 22},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 15, High = 21},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 4.5m, High = 7.6m,},
                                new HudPlayerTypeStat {Stat = Stat.AGG, Low = 42}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                    Name = "Tight Reg",
                    ImageAlias = "book.png",
                    Image = GetImageLink("book.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 18, High = 22},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 14, High = 21},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 3.2m, High = 6m,},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 41}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 14, High = 18},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 14, High = 18},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 3.6m, High = 6.8m}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                    Name = "Bad LAG",
                    ImageAlias = "Bad LAG.png",
                    Image = GetImageLink("Bad LAG.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 26, High = 35},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 21, High = 33},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 6m},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 43}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 22, High = 29},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 20, High = 28},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 5.5m, High = 9.6m}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                    Name = "Tricky LAG",
                    ImageAlias = "Tricky LAG.png",
                    Image = GetImageLink("Tricky LAG.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 25, High = 34},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 21, High = 31},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 6.5m},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 45}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 21, High = 28},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 21, High = 28},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 6.0m, High = 10m},
                                new HudPlayerTypeStat {Stat = Stat.AGG, Low = 45}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                    Name = "Whale",
                    ImageAlias = "Whale.png",
                    Image = GetImageLink("Whale.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 44},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 12},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 4}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 42},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 11},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 4},
                                new HudPlayerTypeStat {Stat = Stat.AGG, Low = 0, High = 42}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                    Name = "Nutball",
                    ImageAlias = "Nutball.png",
                    Image = GetImageLink("Nutball.png"),
                    StatsToMerge = tableType == EnumTableType.Six
                        ?
                        // 6-max
                        new ObservableCollection<HudPlayerTypeStat>()
                        {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 40},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 22}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat>()
                            {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 38},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 22},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 5},
                                new HudPlayerTypeStat {Stat = Stat.AGG, Low = 44}
                            }
                            : new ObservableCollection<HudPlayerTypeStat>()
                }
            };

            return hudPlayerTypes;
        }

        #endregion

        public HudTableViewModel GetHudTableViewModel(EnumPokerSites? pokerSite, EnumGameType? gameType, EnumTableType tableType)
        {
            return
                HudTableViewModels.FirstOrDefault(
                    h => h.PokerSite == pokerSite && h.GameType == gameType && h.TableType == tableType);
        }

        public void SetLayoutActive(HudLayoutInfo hudToLoad, short pokerSiteId, short gameType, short tableType)
        {
            var tableDefinition = new HudTableDefinition
            {
                GameType = (EnumGameType) gameType,
                PokerSite = (EnumPokerSites) pokerSiteId,
                TableType = (EnumTableType) tableType
            };
            var layout = GetActiveFor(tableDefinition);
            if (!layout.IsDefault)
                layout.ActiveFor.RemoveByCondition(a => a.Equals(tableDefinition));
            hudToLoad.ActiveFor.Add(tableDefinition);
            InternalSave(hudToLoad);
        }

        private HudLayoutInfo GetActiveFor(HudTableDefinition tableDefinition)
        {
            var result = Layouts.FirstOrDefault(l => l.ActiveFor.Any(a=>a.Equals(tableDefinition))) ??
                         Layouts.FirstOrDefault(l => l.IsDefault);
            return result;
        }

        public HudLayoutInfo GetLayout(string name)
        {
            return Layouts.FirstOrDefault(x => x.Name.Equals(name));
        }

        /// <summary>
        /// Delete layout
        /// </summary>
        /// <param name="layout">Layout to delete</param>
        public bool Delete(HudLayoutInfo layout)
        {
            if (layout == null)
                return false;
            var layoutToDelete = GetLayout(layout.Name);
            if (layoutToDelete == null || layoutToDelete.IsDefault)
                return false;
            var layoutsDirectory = GetLayoutsDirectory();
            var fileName = string.Empty;
            foreach (
                var fileInfo in
                layoutsDirectory.GetFiles()
                    .Where(
                        f =>
                            string.Equals(f.Extension, LayoutFileExtension,
                                StringComparison.InvariantCultureIgnoreCase)))
            {
                using (var fs = File.Open(fileInfo.FullName, FileMode.Open))
                {
                    var l = LoadLayoutFromStream(fs);
                    if (l != null && l.Name == layout.Name)
                        fileName = fileInfo.FullName;
                }
                if (!string.IsNullOrEmpty(fileName))
                    break;
            }
            if (!string.IsNullOrEmpty(fileName))
                File.Delete(fileName);
            Layouts.Remove(layout);
            return true;
        }

        /// <summary>
        /// Save new layout
        /// </summary>
        /// <param name="hudData">Hud Data</param>
        public HudLayoutInfo SaveAs(HudSavedDataInfo hudData)
        {
            if (hudData == null)
                return null;

            var layout = GetLayout(hudData.Name);
            bool addLayout = layout == null;
            if (addLayout)
                layout = new HudLayoutInfo {Name = hudData.Name};
            else
                layout.Name = hudData.Name;
            layout.HudTableDefinedProperties =
                hudData.LayoutInfo.HudTableDefinedProperties.Select(p => p.Clone()).ToList();
            var targetProps =
                layout.HudTableDefinedProperties.FirstOrDefault(
                    p => p.HudTableDefinition.Equals(hudData.TableDefinition));
            if (targetProps != null)
            {
                targetProps.HudStats = hudData.Stats.Select(x =>
                {
                    var statInfoBreak = x as StatInfoBreak;
                    return statInfoBreak != null ? statInfoBreak.Clone() : x.Clone();
                }).ToList();
                targetProps.HudPositions =
                    hudData.HudTable.HudElements.Select(
                        x =>
                            new HudSavedPosition
                            {
                                Height = x.Height,
                                Position = x.Position,
                                Width = x.Width,
                                Seat = x.Seat,
                                HudType = x.HudType
                            }).ToList();
            }
            if (addLayout)
                Layouts.Add(layout);

            InternalSave(layout);

            return layout;
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <param name="layout">Layout to be exported</param>
        /// <param name="path">Path to file</param>
        public void Export(HudLayoutInfo layout, string path)
        {
            try
            {
                if (layout == null)
                    return;
                using (var fs = File.Open(path, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                    xmlSerializer.Serialize(fs, layout);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        /// <summary>
        /// Import layout
        /// </summary>
        /// <param name="path">Path to layout</param>
        public HudLayoutInfo Import(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            var newFileName = Path.GetFileName(path);
            var directory = GetLayoutsDirectory();
            var i = 0;
            while (File.Exists(Path.Combine(directory.FullName, newFileName)))
            {
                newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)} {i}{LayoutFileExtension}";
            }
            try
            {
                using (var fs = File.Open(Path.Combine(directory.FullName, newFileName), FileMode.Open))
                {
                    var importedHudLayout = LoadLayoutFromStream(fs);
                    Layouts.Add(importedHudLayout);
                    return importedHudLayout;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            return null;
        }

        /// <summary>
        /// Set icons for hud elements based on stats and layout player type settings
        /// </summary>
        public void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, string layoutName, HudTableDefinition tableDefinition)
        {
            var layout = GetLayout(layoutName);
            if (layout == null)
                return;

            // get total hands now to prevent enumeration in future
            var hudElementViewModels = hudElements as HudElementViewModel[] ?? hudElements.ToArray();
            var hudElementsTotalHands = (from hudElement in hudElementViewModels
                from stat in hudElement.StatInfoCollection
                where stat.Stat == Stat.TotalHands
                select new {HudElement = hudElement, TotalHands = stat.CurrentValue}).ToDictionary(x => x.HudElement,
                x => x.TotalHands);
            var targetProps =
                layout.HudTableDefinedProperties.FirstOrDefault(p => p.HudTableDefinition.Equals(tableDefinition));
            if (targetProps == null)
                return;
            // get match ratios by player
            var matchRatiosByPlayer = (from playerType in targetProps.HudPlayerTypes
                from hudElement in hudElementViewModels
                let matchRatio = GetMatchRatio(hudElement, playerType)
                where playerType.EnablePlayerProfile && playerType.MinSample <= hudElementsTotalHands[hudElement]
                group
                new MatchRatio
                {
                    IsInRange = matchRatio.Item1,
                    Ratio = matchRatio.Item2,
                    ExtraRatio = matchRatio.Item3,
                    PlayerType = playerType
                } by hudElement
                into grouped
                select
                new PlayerMatchRatios
                {
                    HudElement = grouped.Key,
                    MatchRatios = grouped.Where(x => x.IsInRange).OrderBy(x => x.Ratio).ToList(),
                    ExtraMatchRatios = grouped.OrderBy(x => x.ExtraRatio).ToList()
                }).ToList();

            var proccesedElements = new HashSet<int>();

            Action<PlayerMatchRatios, List<MatchRatio>> proccessPlayerRatios = (matchRatioPlayer, matchRatios) =>
            {
                if (proccesedElements.Contains(matchRatioPlayer.HudElement.Seat))
                {
                    return;
                }

                var match = matchRatios.FirstOrDefault();

                if (match == null)
                {
                    return;
                }

                var playerType = match.PlayerType;

                if (playerType.DisplayPlayerIcon)
                {
                    matchRatioPlayer.HudElement.PlayerIcon = GetImageLink(playerType.ImageAlias);
                }

                matchRatioPlayer.HudElement.PlayerIconToolTip =
                    $"{matchRatioPlayer.HudElement.PlayerName.Split('_').FirstOrDefault()}: {playerType.Name}";

                proccesedElements.Add(matchRatioPlayer.HudElement.Seat);
            };

            // set icons
            foreach (var playerRatios in matchRatiosByPlayer)
            {
                proccessPlayerRatios(playerRatios, playerRatios.MatchRatios);
            }

            // set icons for extra match
            foreach (var playerRatios in matchRatiosByPlayer)
            {
                proccessPlayerRatios(playerRatios, playerRatios.ExtraMatchRatios);
            }
        }

        /// <summary>
        /// Set stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        public IList<string> GetValidStickers(Playerstatistic statistic, string layoutName,
            HudTableDefinition tableDefinition)
        {
            var layout = GetLayout(layoutName);
            if (layout == null || statistic == null)
                return new List<string>();
            var layoutProps =
                layout.HudTableDefinedProperties.FirstOrDefault(p => p.HudTableDefinition.Equals(tableDefinition));
            if (layoutProps == null)
                return new List<string>();
            return
                layoutProps.HudBumperStickerTypes?.Where(
                        x => x.FilterPredicate != null && new[] {statistic}.AsQueryable().Where(x.FilterPredicate).Any())
                    .Select(x => x.Name)
                    .ToList();
        }

        /// <summary>
        /// Set stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        public void SetStickers(HudElementViewModel hudElement, IDictionary<string, Playerstatistic> stickersStatistics,
            string layoutName, HudTableDefinition tableDefinition)
        {
            hudElement.Stickers = new ObservableCollection<HudBumperStickerType>();
            var layout = GetLayout(layoutName);
            if (layout == null || stickersStatistics == null)
                return;
            var layoutProps = layout.HudTableDefinedProperties.Where(p => p.HudTableDefinition.Equals(tableDefinition));
            foreach (var hudTableDefinedProperties in layoutProps)
            {
                foreach (
                    var sticker in hudTableDefinedProperties.HudBumperStickerTypes.Where(x => x.EnableBumperSticker))
                {
                    if (!stickersStatistics.ContainsKey(sticker.Name))
                    {
                        continue;
                    }

                    var statistics = new Model.Data.HudIndicators(new[] {stickersStatistics[sticker.Name]});
                    if (statistics.TotalHands < sticker.MinSample || statistics.TotalHands == 0)
                    {
                        continue;
                    }

                    if (IsInRange(hudElement, sticker.Stats, statistics))
                    {
                        hudElement.Stickers.Add(sticker);
                    }
                }
            }
        }

        /// <summary>
        /// Save bumper stickers for layout specified
        /// </summary>
        public void SaveBumperStickers(HudLayoutInfo hudLayout, HudTableDefinition tableDefinition)
        {
            if (hudLayout == null)
                return;
            var layout = GetLayout(hudLayout.Name);
            if (layout == null)
                return;
            var targetProps = layout.HudTableDefinedProperties.FirstOrDefault(p => p.HudTableDefinition.Equals(tableDefinition));
            var sourceProps =
                hudLayout.HudTableDefinedProperties.FirstOrDefault(p => p.HudTableDefinition.Equals(tableDefinition));
            //TODO recheck!!!
            if (targetProps != null && sourceProps != null)
            {
                targetProps.HudBumperStickerTypes =
                    new List<HudBumperStickerType>(sourceProps.HudBumperStickerTypes.Select(x => x.Clone()));
                InternalSave(layout);
            }
        }

        

        /// <summary>
        /// Get path to image directory
        /// </summary>
        /// <returns>Path to image directory</returns>
        public string GetImageDirectory()
        {
            var executingApp = Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(executingApp), PathToImages);
        }

        /// <summary>
        /// Get link to image
        /// </summary>
        /// <param name="image">Image alias</param>
        /// <returns>Full path to image</returns>
        public virtual string GetImageLink(string image)
        {
            if (string.IsNullOrWhiteSpace(image))
            {
                return string.Empty;
            }

            var imageLink = Path.Combine(GetImageDirectory(), image);

            if (File.Exists(imageLink) && Path.GetExtension(imageLink).ToUpperInvariant().Equals(".PNG"))
            {
                return imageLink;
            }

            return string.Empty;
        }

        public HudLayoutInfo GetActiveLayout(HudTableDefinition tableDefinition)
        {
            var result = Layouts.FirstOrDefault(l => l.ActiveFor.Any(d => d.Equals(tableDefinition)));
            if (result != null)
                return result;
            result =
                Layouts.FirstOrDefault(
                    l =>
                        l.ActiveFor.Any(
                            d =>
                                (d.PokerSite == null || d.PokerSite == tableDefinition.PokerSite) &&
                                (d.GameType == null || d.GameType == tableDefinition.GameType)));
            if (result != null)
                return result;
            result =
                Layouts.FirstOrDefault(
                    l => l.HudTableDefinedProperties.Any(d => d.HudTableDefinition.Equals(tableDefinition)));
            if (result != null)
                return result;
            result =
                Layouts.FirstOrDefault(
                    l =>
                        l.HudTableDefinedProperties.Any(
                            d =>
                                (d.HudTableDefinition.PokerSite == null || d.HudTableDefinition.PokerSite == tableDefinition.PokerSite) &&
                                (d.HudTableDefinition.GameType == null || d.HudTableDefinition.GameType == tableDefinition.GameType)));
            if (result != null)
                return result;
            return Layouts.FirstOrDefault(l => l.IsDefault);
        }
    }
}