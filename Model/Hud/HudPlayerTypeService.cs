//-----------------------------------------------------------------------
// <copyright file="HudPlayerTypeService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.Reflection;
using DriveHUD.Entities;
using Model.Data;
using Model.Enums;
using Model.Stats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Model.Hud
{
    internal class HudPlayerTypeService : IHudPlayerTypeService
    {
        private const string PathToImages = @"data\PlayerTypes";

        /// <summary>
        /// Gets default player types for the specified <see cref="EnumTableType"/>
        /// </summary>
        /// <param name="tableType">Table type to get player types</param>
        /// <returns>The collection of <see cref="HudPlayerType"/></returns>
        public IEnumerable<HudPlayerType> CreateDefaultPlayerTypes(EnumTableType tableType)
        {
            tableType = tableType < EnumTableType.Eight ? EnumTableType.Six : EnumTableType.Nine;

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

        /// <summary>
        /// Gets the link to the specified image
        /// </summary>
        /// <param name="image">The image to get the link</param>
        /// <returns>Path to the image</returns>
        public virtual string GetImageLink(string image)
        {
            if (string.IsNullOrWhiteSpace(image))
            {
                return string.Empty;
            }

            var imageLink = Path.Combine(GetImageDirectory(), image);

            if (File.Exists(imageLink) && Path.GetExtension(imageLink).Equals(".png", StringComparison.OrdinalIgnoreCase))
            {
                return imageLink;
            }

            return string.Empty;
        }


        /// <summary>
        /// Gets path to the image directory
        /// </summary>
        /// <returns>Path to the image directory</returns>
        public string GetImageDirectory()
        {
            var executingApp = Assembly.GetEntryAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(executingApp), PathToImages);
        }

        /// <summary>
        /// Check whenever players <see cref="Indicators"/> does match the specified player type
        /// </summary>
        /// <param name="playerIndicators">Indicators to match</param>
        /// <param name="playerTypes">Player type to match</param>
        /// <param name="strictMatch">Determinse whenever player <see cref="Indicators"/> must strictly matches player type</param>
        /// <returns><see cref="HudPlayerType"/> if matches; otherwise - null</returns>
        public HudPlayerType Match(Indicators playerIndicators, IEnumerable<HudPlayerType> playerTypes, bool strictMatch)
        {
            try
            {
                // check if players total hands matches min sample
                var filteredPlayerTypes = playerTypes
                    .Where(x => x.MinSample <= playerIndicators.TotalHands)
                    .ToArray();

                if (filteredPlayerTypes.Length == 0)
                {
                    return null;
                }

                // transforms indicators into statinfo collection
                var playerTypesStats = playerTypes
                    .SelectMany(x => x.Stats)
                    .Select(x => x.Stat)
                    .Distinct()
                    .ToArray();

                var indicatorsStatInfos = (from playerTypeStat in playerTypesStats
                                           let statBase = StatsProvider.StatsBases[playerTypeStat]
                                           select new StatInfo
                                           {
                                               Stat = playerTypeStat,
                                               CurrentValue = (decimal)ReflectionHelper.GetMemberValue(playerIndicators, statBase.PropertyName)
                                           }).ToArray();

                var matchRatios = (from playerType in filteredPlayerTypes
                                   let matchRatio = GetMatchRatio(indicatorsStatInfos, playerType)
                                   select new
                                   {
                                       IsInRange = matchRatio.Item1,
                                       Ratio = matchRatio.Item2,
                                       ExtraRatio = matchRatio.Item3,
                                       PlayerType = playerType
                                   }).ToArray();

                var closestMatchRatio = matchRatios.Where(x => x.IsInRange).OrderBy(x => x.Ratio).FirstOrDefault();

                if (closestMatchRatio == null && !strictMatch)
                {
                    closestMatchRatio = matchRatios.OrderBy(x => x.ExtraRatio).FirstOrDefault();
                }

                return closestMatchRatio?.PlayerType;

            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not check if player indicators matches one of the specified player types.", e);
            }

            return null;
        }


        private Tuple<bool, decimal, decimal> GetMatchRatio(IEnumerable<StatInfo> stats, HudPlayerType hudPlayerType)
        {
            var matchRatios = (from stat in hudPlayerType.Stats
                               let low = stat.Low ?? -1
                               let high = stat.High ?? 100
                               let average = (high + low) / 2
                               let isStatDefined = stat.Low.HasValue || stat.High.HasValue
                               join hudElementStat in stats on stat.Stat equals hudElementStat.Stat into gj
                               from grouped in gj.DefaultIfEmpty()
                               let inRange = grouped != null ? (grouped.CurrentValue >= low && grouped.CurrentValue <= high) : !isStatDefined
                               let isGroupAndStatDefined = grouped != null && isStatDefined
                               let matchRatio = isGroupAndStatDefined ? Math.Abs(grouped.CurrentValue - average) : 0
                               let extraMatchRatio = (isGroupAndStatDefined && (grouped.Stat == Stat.VPIP || grouped.Stat == Stat.PFR)) ? matchRatio : 0
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
    }
}