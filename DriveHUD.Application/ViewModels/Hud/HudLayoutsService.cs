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

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Model;
using Model.Data;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.TableConfigurators.PositionProviders;


namespace DriveHUD.Application.ViewModels.Hud
{
    internal class HudLayoutsService : IHudLayoutsService
    {
        private const string LayoutsFolderName = "Layouts";
        private const string LayoutFileExtension = ".xml";
        private const string MappingsFileName = "Mappings";
        private const string PathToImages = @"data\PlayerTypes";
        private readonly EnumPokerSites[] _extendedHudPokerSites = { EnumPokerSites.Bodog, EnumPokerSites.Ignition };

        private static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        public HudLayoutMappings HudLayoutMappings { get; set; }

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
            try
            {
                var layoutsDirectory = GetLayoutsDirectory();

                var mappingsFilePath = Path.Combine(layoutsDirectory.FullName, $"{MappingsFileName}{LayoutFileExtension}");

                if (File.Exists(mappingsFilePath))
                {
                    HudLayoutMappings = LoadLayoutMappings(mappingsFilePath);
                }
                else
                {
                    HudLayoutMappings = new HudLayoutMappings();

                    foreach (var tableType in Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>())
                    {
                        var defaultLayoutInfo = GetPredefindedLayout(tableType);
                        defaultLayoutInfo.Name = $"Default {CommonResourceManager.Instance.GetEnumResource(tableType)}";
                        defaultLayoutInfo.TableType = tableType;
                        defaultLayoutInfo.IsDefault = true;
                        defaultLayoutInfo.HudPositionsInfo.Clear();
                        var pokerSites =
                            Enum.GetValues(typeof(EnumPokerSites))
                                .OfType<EnumPokerSites>()
                                .Where(p => p != EnumPokerSites.Unknown && p != EnumPokerSites.IPoker);
                        foreach (var pokerSite in pokerSites)
                        {
                            foreach (var gameType in Enum.GetValues(typeof(EnumGameType)).OfType<EnumGameType>())
                            {
                                var hudPositions = GeneratePositions(pokerSite, HudViewType.Plain, tableType);
                                if (hudPositions != null)
                                    defaultLayoutInfo.HudPositionsInfo.Add(new HudPositionsInfo
                                    {
                                        PokerSite = pokerSite,
                                        GameType = gameType,
                                        HudPositions = hudPositions
                                    });
                            }
                        }
                        InternalSave(defaultLayoutInfo);

                        HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                        {
                            TableType = tableType,
                            Name = defaultLayoutInfo.Name,
                            IsDefault = true,
                            FileName = $"{defaultLayoutInfo.Name}.xml"
                        });

                    }

                    SaveLayoutMappings(mappingsFilePath, HudLayoutMappings);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        private HudLayoutMappings LoadLayoutMappings(string fileName)
        {
            HudLayoutMappings hudLayoutMappings = null;
            _rwLock.EnterReadLock();
            try
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutMappings));
                    hudLayoutMappings = xmlSerializer.Deserialize(stream) as HudLayoutMappings;
                }
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
            return hudLayoutMappings;
        }

        private void SaveLayoutMappings()
        {
            var layoutsDirectory = GetLayoutsDirectory();
            var mappingsFilePath = Path.Combine(layoutsDirectory.FullName, $"{MappingsFileName}{LayoutFileExtension}");
            SaveLayoutMappings(mappingsFilePath, HudLayoutMappings);
        }

        private void SaveLayoutMappings(string fileName, HudLayoutMappings mappings)
        {
            _rwLock.EnterWriteLock();
            try
            {
                using (var fs = File.Open(fileName, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutMappings));
                    xmlSerializer.Serialize(fs, mappings);
                }
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private HudLayoutInfo LoadDefault(EnumTableType tableType)
        {
            return
                LoadLayout(Path.Combine(GetLayoutsDirectory().FullName,
                    $"Default {CommonResourceManager.Instance.GetEnumResource(tableType)}{LayoutFileExtension}"));
        }

        private HudLayoutInfo LoadLayout(HudLayoutMapping mapping)
        {
            return LoadLayout(Path.Combine(GetLayoutsDirectory().FullName, mapping.FileName));
        }

        private HudLayoutInfo LoadLayout(string fileName)
        {
            HudLayoutInfo result;
            _rwLock.EnterReadLock();
            try
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    result = LoadLayoutFromStream(stream);
                }
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
            return result;
        }

        private HudLayoutInfo GetPredefindedLayout(EnumTableType tableType)
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;
            try
            {
                using (
                    var stream =
                        resourcesAssembly.GetManifestResourceStream($"DriveHUD.Common.Resources.Layouts.Default-{CommonResourceManager.Instance.GetEnumResource(tableType)}.xml"))
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
            var bumperStickers = new List<HudBumperStickerType> {
                new HudBumperStickerType(true)
                {
                    Name = "One and Done",
                    SelectedColor = Colors.OrangeRed,
                    Description = "C-bets at a high% on the flop, but then rarely double barrels.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat> {
                            new BaseHudRangeStat {Stat = Stat.CBet, Low = 55, High = 100},
                            new BaseHudRangeStat {Stat = Stat.DoubleBarrel, Low = 0, High = 35}
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Pre-Flop Reg",
                    SelectedColor = Colors.Orange,
                    Description = "Plays an aggressive pre-flop game, but doesn’t play well post flop.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat> {
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 19, High = 26},
                            new BaseHudRangeStat {Stat = Stat.PFR, Low = 15, High = 23},
                            new BaseHudRangeStat {Stat = Stat.S3Bet, Low = 8, High = 100},
                            new BaseHudRangeStat {Stat = Stat.WWSF, Low = 0, High = 42}
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Barrelling",
                    SelectedColor = Colors.Yellow,
                    Description = "Double and triple barrels a high percentage of the time.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat> {
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 20, High = 30},
                            new BaseHudRangeStat {Stat = Stat.PFR, Low = 17, High = 28},
                            new BaseHudRangeStat {Stat = Stat.AGG, Low = 40, High = 49},
                            new BaseHudRangeStat {Stat = Stat.CBet, Low = 65, High = 80},
                            new BaseHudRangeStat {Stat = Stat.WWSF, Low = 44, High = 53},
                            new BaseHudRangeStat {Stat = Stat.DoubleBarrel, Low = 46, High = 100}
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "3 For Free",
                    SelectedColor = Colors.GreenYellow,
                    Description = "3-Bets too much, and folds to a 3-bet too often.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat> {
                            new BaseHudRangeStat {Stat = Stat.S3Bet, Low = 8.8m, High = 100},
                            new BaseHudRangeStat {Stat = Stat.FoldTo3Bet, Low = 66, High = 100}
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Way Too Early",
                    SelectedColor = Colors.Green,
                    Description = "Open raises to wide of a range in early pre-flop positions.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat> {
                            new BaseHudRangeStat {Stat = Stat.UO_PFR_EP, Low = 20, High = 100}
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Sticky Fish",
                    SelectedColor = Colors.Blue,
                    Description = "Fishy player who can’t fold post flop if they get any piece of the board.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat> {
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 35, High = 100},
                            new BaseHudRangeStat {Stat = Stat.FoldToCBet, Low = 0, High = 40},
                            new BaseHudRangeStat {Stat = Stat.WTSD, Low = 29, High = 100}
                        }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Yummy Fish",
                    SelectedColor = Colors.DarkBlue,
                    Description = "Plays too many hands pre-flop and isn’t aggressive post flop.",
                    StatsToMerge =
                        new ObservableCollection<BaseHudRangeStat> {
                            new BaseHudRangeStat {Stat = Stat.VPIP, Low = 40, High = 100},
                            new BaseHudRangeStat {Stat = Stat.FoldToCBet, Low = 0, High = 6},
                            new BaseHudRangeStat {Stat = Stat.AGG, Low = 0, High = 34}
                        }
                }
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
            HudIndicators source)
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

                var currentStat = new StatInfo { PropertyName = stat.PropertyName };
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


        private string InternalSave(HudLayoutInfo hudLayoutInfo)
        {
            var layoutsDirectory = GetLayoutsDirectory().FullName;
            var layoutsFile = hudLayoutInfo.IsDefault
                ? Path.Combine(layoutsDirectory,
                    $"Default {CommonResourceManager.Instance.GetEnumResource(hudLayoutInfo.TableType)}{LayoutFileExtension}")
                : Path.Combine(layoutsDirectory,
                    $"{Path.GetInvalidFileNameChars().Aggregate(hudLayoutInfo.Name, (current, c) => current.Replace(c.ToString(), string.Empty))}{LayoutFileExtension}");
            _rwLock.EnterWriteLock();
            try
            {
                using (var fs = File.Open(layoutsFile, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                    xmlSerializer.Serialize(fs, hudLayoutInfo);
                }
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
            return layoutsFile;
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
                layout?.HudPlayerTypes.ForEach(x =>
                {
                    if (x != null)
                    {
                        x.Image = GetImageLink(x.ImageAlias);
                    }
                });

                layout?.HudBumperStickerTypes.ForEach(x =>
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 0, High = 17},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 16},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 4.3m}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 36},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 13},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 4},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 0, High = 40}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 22, High = 27},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 18, High = 25},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 4.7m, High = 8.6m},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 42}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
                                new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 16, High = 22},
                                new HudPlayerTypeStat {Stat = Stat.PFR, Low = 15, High = 21},
                                new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 4.5m, High = 7.6m},
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 18, High = 22},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 14, High = 21},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 3.2m, High = 6m},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 41}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 26, High = 35},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 21, High = 33},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 6m},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 43}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 25, High = 34},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 21, High = 31},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 6.5m},
                            new HudPlayerTypeStat {Stat = Stat.AGG, Low = 45}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 44},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 0, High = 12},
                            new HudPlayerTypeStat {Stat = Stat.S3Bet, Low = 0, High = 4}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
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
                        new ObservableCollection<HudPlayerTypeStat> {
                            new HudPlayerTypeStat {Stat = Stat.VPIP, Low = 40},
                            new HudPlayerTypeStat {Stat = Stat.PFR, Low = 22}
                        }
                        : (tableType == EnumTableType.Nine)
                            ?
                            // 9-max
                            new ObservableCollection<HudPlayerTypeStat> {
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

        public void SetLayoutActive(HudLayoutInfo hudToLoad, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            var mappings = HudLayoutMappings.Mappings.
                            Where(m => (m.PokerSite == pokerSite || !m.PokerSite.HasValue)
                                    && (m.GameType == gameType || !m.GameType.HasValue)
                                    && m.TableType == tableType).
                            ToArray();

            if (mappings.Length == 0)
            {
                LogProvider.Log.Warn(this, $"Layout has not been set. Could not find layout map for {pokerSite} {gameType} {tableType}");
                return;
            }

            var mapping = mappings.FirstOrDefault(x => x.Name == hudToLoad.Name);

            if (mapping == null)
            {
                LogProvider.Log.Warn(this, $"Layout has not been set. Could not find layouts {hudToLoad.Name} for {pokerSite} {gameType} {tableType}");
                return;
            }

            mappings.Where(x => x.IsSelected).ForEach(x => x.IsSelected = false);

            mapping.IsSelected = true;

            SaveLayoutMappings();
        }

        public bool Delete(string layoutName)
        {
            if (string.IsNullOrEmpty(layoutName))
                return false;
            var layoutToDelete = GetLayout(layoutName);
            if (layoutToDelete == null || layoutToDelete.IsDefault)
                return false;
            var layoutsDirectory = GetLayoutsDirectory();
            var fileName = HudLayoutMappings.Mappings.FirstOrDefault(m => m.Name == layoutToDelete.Name)?.FileName;
            if (!string.IsNullOrEmpty(fileName))
                File.Delete(Path.Combine(layoutsDirectory.FullName, fileName));
            HudLayoutMappings.Mappings.RemoveByCondition(
                m => string.Equals(m.FileName, Path.GetFileName(fileName), StringComparison.InvariantCultureIgnoreCase));
            SaveLayoutMappings();
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
            var addLayout = layout == null;

            if (addLayout)
            {
                layout = new HudLayoutInfo();
            }

            layout.Name = hudData.Name;
            layout.TableType = hudData.LayoutInfo.TableType;
            if (addLayout || layout.HudViewType != hudData.LayoutInfo.HudViewType)
            {
                layout.HudPositionsInfo.Clear();
                var pokerSites = hudData.LayoutInfo.HudViewType == HudViewType.Plain
                    ? Enum.GetValues(typeof(EnumPokerSites))
                        .OfType<EnumPokerSites>()
                        .Where(p => p != EnumPokerSites.Unknown && p != EnumPokerSites.IPoker)
                    : new[] {EnumPokerSites.Ignition, EnumPokerSites.Bodog};
                foreach (var pokerSite in pokerSites)
                {
                    foreach (var gameType in Enum.GetValues(typeof(EnumGameType)).OfType<EnumGameType>())
                    {
                        var hudPositions = GeneratePositions(pokerSite, hudData.LayoutInfo.HudViewType, layout.TableType);
                        if (hudPositions != null)
                            layout.HudPositionsInfo.Add(new HudPositionsInfo
                            {
                                PokerSite = pokerSite,
                                GameType = gameType,
                                HudPositions = hudPositions
                            });
                    }
                }
            }
            else
            {
                layout.HudPositionsInfo = hudData.LayoutInfo.HudPositionsInfo.Select(p => p.Clone()).ToList();
            }
            layout.HudViewType = hudData.LayoutInfo.HudViewType;

            layout.HudStats = hudData.Stats.Select(x =>
            {
                var statInfoBreak = x as StatInfoBreak;
                return statInfoBreak != null ? statInfoBreak.Clone() : x.Clone();
            }).ToList();
            
            layout.HudBumperStickerTypes = hudData.LayoutInfo.HudBumperStickerTypes.Select(x => x.Clone()).ToList();
            layout.HudPlayerTypes = hudData.LayoutInfo.HudPlayerTypes.Select(x => x.Clone()).ToList();
            layout.UiPositionsInfo = hudData.HudTable.HudElements.Select(x => new UiPositionInfo
            {
                Seat = x.Seat,
                Height = x.Height,
                Width = x.Width,
                Position = x.Position
            }).ToList();

            var fileName = InternalSave(layout);

            if (!layout.IsDefault)
            {
                HudLayoutMappings.Mappings.RemoveByCondition(m => m.Name == layout.Name);

                var pokerSites = layout.HudViewType == HudViewType.Plain
                    ? Enum.GetValues(typeof(EnumPokerSites))
                        .OfType<EnumPokerSites>()
                        .Where(p => p != EnumPokerSites.Unknown)
                    : _extendedHudPokerSites;

                foreach (var pokerSite in pokerSites)
                {
                    foreach (var gameType in Enum.GetValues(typeof(EnumGameType)).OfType<EnumGameType>())
                    {
                        HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                        {
                            FileName = Path.GetFileName(fileName),
                            Name = layout.Name,
                            GameType = gameType,
                            PokerSite = pokerSite,
                            TableType = layout.TableType,
                            IsSelected = false,
                            IsDefault = false
                        });
                    }
                }
            }

            SaveLayoutMappings();

            return layout;
        }

        private List<HudPositionInfo> GeneratePositions(EnumPokerSites pokerSite, HudViewType hudViewType, EnumTableType tableType)
        {
            var positionsProvider =
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IPositionProvider>(
                    PositionConfiguratorHelper.GetServiceName(pokerSite, hudViewType));
            if (!positionsProvider.Positions.ContainsKey((int) tableType))
                return null;
            var result = new List<HudPositionInfo>();
            for (var i = 0; i < (int) tableType; i++)
            {
                result.Add(new HudPositionInfo
                {
                    Position =
                        new Point
                        {
                            X =
                                positionsProvider.Positions[(int) tableType][i, 0] +
                                positionsProvider.GetOffsetX((int) tableType, i + 1),
                            Y =
                                positionsProvider.Positions[(int) tableType][i, 1] +
                                positionsProvider.GetOffsetY((int) tableType, i + 1)
                        },
                    Seat = i + 1
                });
            }
            return result;
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <param name="layout">Layout to be exported</param>
        /// <param name="path">Path to file</param>
        public void Export(HudLayoutInfo layout, string path)
        {
            if (layout == null)
                return;
            _rwLock.EnterReadLock();
            try
            {
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
            finally
            {
                _rwLock.ExitReadLock();
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
            try
            {
                HudLayoutInfo importedHudLayout;
                _rwLock.EnterReadLock();
                try
                {
                    using (var fs = File.Open(path, FileMode.Open))
                    {
                        importedHudLayout = LoadLayoutFromStream(fs);
                    }
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
                var i = 0;
                var layoutName = importedHudLayout.Name;
                importedHudLayout.IsDefault = false;
                while (HudLayoutMappings.Mappings.Any(l => l.Name == importedHudLayout.Name))
                {
                    importedHudLayout.Name = $"{layoutName} {i}";
                    i++;
                }
                var fileName = InternalSave(importedHudLayout);
                foreach (var pokerSite in Enum.GetValues(typeof(EnumPokerSites)).OfType<EnumPokerSites>())
                {
                    foreach (var gameType in Enum.GetValues(typeof(EnumGameType)).OfType<EnumGameType>())
                    {
                        HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                        {
                            FileName = Path.GetFileName(fileName),
                            Name = importedHudLayout.Name,
                            GameType = gameType,
                            PokerSite = pokerSite,
                            TableType = importedHudLayout.TableType,
                            IsSelected = false,
                            IsDefault = false
                        });
                    }
                }
                SaveLayoutMappings();
                return importedHudLayout;
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
        public void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, string layoutName)
        {
            var layout = GetLayout(layoutName);
            if (layout == null)
                return;

            // get total hands now to prevent enumeration in future
            var hudElementViewModels = hudElements as HudElementViewModel[] ?? hudElements.ToArray();
            var hudElementsTotalHands = (from hudElement in hudElementViewModels
                                         from stat in hudElement.StatInfoCollection
                                         where stat.Stat == Stat.TotalHands
                                         select new { HudElement = hudElement, TotalHands = stat.CurrentValue }).ToDictionary(x => x.HudElement,
                x => x.TotalHands);
            // get match ratios by player
            var matchRatiosByPlayer = (from playerType in layout.HudPlayerTypes
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
        public IList<string> GetValidStickers(Playerstatistic statistic, string layoutName)
        {
            var layout = GetLayout(layoutName);
            if (layout == null || statistic == null)
                return new List<string>();
            return
                layout.HudBumperStickerTypes?.Where(
                        x => x.FilterPredicate != null && new[] { statistic }.AsQueryable().Where(x.FilterPredicate).Any())
                    .Select(x => x.Name)
                    .ToList();
        }

        /// <summary>
        /// Set stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        public void SetStickers(HudElementViewModel hudElement, IDictionary<string, Playerstatistic> stickersStatistics,
            string layoutName)
        {
            hudElement.Stickers = new ObservableCollection<HudBumperStickerType>();
            var layout = GetLayout(layoutName);
            if (layout == null || stickersStatistics == null)
                return;
            foreach (var sticker in layout.HudBumperStickerTypes.Where(x => x.EnableBumperSticker))
            {
                if (!stickersStatistics.ContainsKey(sticker.Name))
                {
                    continue;
                }

                var statistics = new HudIndicators(new[] { stickersStatistics[sticker.Name] });
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

        /// <summary>
        /// Save bumper stickers for layout specified
        /// </summary>
        //public void SaveBumperStickers(HudLayoutInfo hudLayout, HudTableDefinition tableDefinition)
        //{
        //    if (hudLayout == null)
        //        return;
        //    var layout = GetLayout(hudLayout.Name);
        //    if (layout == null)
        //        return;
        //    var targetProps =
        //        layout.HudTableDefinedProperties.FirstOrDefault(p => p.HudTableDefinition.Equals(tableDefinition));
        //    var sourceProps =
        //        hudLayout.HudTableDefinedProperties.FirstOrDefault(p => p.HudTableDefinition.Equals(tableDefinition));
        //    //TODO recheck!!!
        //    if (targetProps != null && sourceProps != null)
        //    {
        //        targetProps.HudBumperStickerTypes =
        //            new List<HudBumperStickerType>(sourceProps.HudBumperStickerTypes.Select(x => x.Clone()));
        //        InternalSave(layout);
        //    }
        //}



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

        public HudLayoutInfo GetActiveLayout(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType)
        {
            var mapping =
                HudLayoutMappings.Mappings.FirstOrDefault(
                    m => m.PokerSite == pokerSite && m.TableType == tableType && m.GameType == gameType && m.IsSelected) ??
                HudLayoutMappings.Mappings.FirstOrDefault(
                    m => m.PokerSite == pokerSite && m.TableType == tableType && m.GameType == gameType && m.IsDefault);
            if (mapping == null)
                return LoadDefault(tableType);
            return LoadLayout(mapping);
        }

        public HudTableViewModel GetHudTableViewModel(EnumPokerSites pokerSite, EnumTableType tableType,
            EnumGameType gameType)
        {
            return
                HudTableViewModels.FirstOrDefault(
                    h => h.PokerSite == pokerSite && h.GameType == gameType && h.TableType == tableType);
        }

        public HudLayoutInfo GetLayout(string name)
        {
            var defaultNames =
                Enum.GetValues(typeof(EnumTableType))
                    .OfType<EnumTableType>()
                    .Select(e => $"Default {CommonResourceManager.Instance.GetEnumResource(e)}")
                    .ToList();
            if (defaultNames.Contains(name))
                return LoadLayout(Path.Combine(GetLayoutsDirectory().FullName, $"{name}{LayoutFileExtension}"));
            var mapping = HudLayoutMappings.Mappings.FirstOrDefault(m => m.Name == name);
            return mapping == null ? null : LoadLayout(mapping);
        }

        public IEnumerable<string> GetLayoutsNames(EnumTableType tableType)
        {
            return HudLayoutMappings.Mappings.Where(x => x.TableType == tableType).OrderByDescending(m => m.IsDefault).Select(m => m.Name).Distinct();
        }

        public IEnumerable<string> GetAvailableLayouts(EnumPokerSites pokerSite, EnumTableType tableType,
            EnumGameType gameType)
        {
            return
                Enumerable.Repeat($"Default {CommonResourceManager.Instance.GetEnumResource(tableType)}", 1)
                    .Union(
                        HudLayoutMappings.Mappings.Where(
                                m => m.PokerSite == pokerSite && m.TableType == tableType && m.GameType == gameType)
                            .Select(m => m.Name))
                    .Distinct();
        }

        public List<HudLayoutInfo> GetAllLayouts(EnumTableType tableType)
        {
            var result = new List<HudLayoutInfo>();

            var layoutMappings = HudLayoutMappings.Mappings.Where(x => x.TableType == tableType).Select(x => x.FileName).Distinct().ToArray();

            var layoutsDirectory = GetLayoutsDirectory();

            foreach (var layoutMapping in layoutMappings)
            {
                _rwLock.EnterReadLock();

                try
                {
                    var fileName = Path.Combine(layoutsDirectory.FullName, layoutMapping);

                    using (var fs = File.Open(fileName, FileMode.Open))
                    {
                        var layout = LoadLayoutFromStream(fs);

                        if (layout != null)
                        {
                            result.Add(layout);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not load layout {layoutMapping}", e);
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }

            return result.OrderBy(l => l.TableType).ToList();
        }
    }
}