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
using System.Diagnostics;
using DriveHUD.Common;
using Model;
using System.Windows.Media;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class HudLayoutsService : IHudLayoutsService
    {
        private const string layoutsFileSettings = "Layouts.xml";

        private const string DefaultHudName = "Default";

        private const string pathToImages = @"data\PlayerTypes";

        private static readonly string[] predefinedLayouts = new string[] { "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-OH.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-OH.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-9max-Basic-OH.xml",
            "DriveHUD.Common.Resources.Layouts.DH-FR-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-MTT-Basic.xml",
            "DriveHUD.Common.Resources.Layouts.DH-SNG-Basic.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-OH.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-OH.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-OH.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-OH.xml", "DriveHUD.Common.Resources.Layouts.DH-MTT-Basic-OH.xml",
            "DriveHUD.Common.Resources.Layouts.DH-SNG-Basic-OH.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-OH-BOL.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-OH-BOL.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-OH-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-BOL-T.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-OH-BOL-S.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-OH-BOL-T.xml"
        };

        protected HudSavedLayouts hudLayouts;

        protected HudSavedLayouts baseHudLayouts;

        private string settingsFolder;

        private string layoutsFile;

        public HudLayoutsService()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            settingsFolder = StringFormatter.GetAppDataFolderPath();

            try
            {
                if (!Directory.Exists(settingsFolder))
                {
                    Directory.CreateDirectory(settingsFolder);
                }

                layoutsFile = Path.Combine(settingsFolder, layoutsFileSettings);

                if (File.Exists(layoutsFile))
                {
                    using (var fs = File.Open(layoutsFile, FileMode.Open))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));

                        hudLayouts = xmlSerializer.Deserialize(fs) as HudSavedLayouts;

                        // set image after loading
                        hudLayouts.Layouts.Where(x => x.HudPlayerTypes != null).SelectMany(x => x.HudPlayerTypes).ForEach(x =>
                        {
                            if (x != null)
                            {
                                x.Image = GetImageLink(x.ImageAlias);
                            }
                        });

                        // set default bumper stickers if they are missing
                        hudLayouts.Layouts.Where(x => x.HudBumperStickerTypes.Count == 0).ForEach(x => x.HudBumperStickerTypes = CreateDefaultBumperStickers());
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            if (hudLayouts == null)
            {
                hudLayouts = new HudSavedLayouts();
            }

            baseHudLayouts = hudLayouts.Clone();
        }

        public HudSavedLayouts Layouts
        {
            get
            {
                return hudLayouts;
            }
        }

        public HudSavedLayout GetLayoutByName(string name, int layoutId)
        {
            var layout = Layouts.Layouts.FirstOrDefault(x => x.Name.Equals(name) && x.LayoutId == layoutId);
            return layout;
        }

        public HudSavedLayout GetActiveLayout(int layoutId)
        {
            var defaultLayout = hudLayouts.Layouts.FirstOrDefault(x => x.IsDefault && x.LayoutId == layoutId);
            return defaultLayout;
        }

        public IEnumerable<HudSavedLayout> GetNotEmptyStatsLayout()
        {
            /* Replayer doesn't have an ability to select hud stats so display whatever is selected in the hud for now */
            return hudLayouts.Layouts.Where(x => x.HudStats.Count > 0);
        }

        public void SetLayoutActive(HudSavedLayout layout)
        {
            if (layout == null)
            {
                return;
            }

            Layouts.Layouts.Where(x => x.LayoutId == layout.LayoutId).ForEach(x => x.IsDefault = false);
            layout.IsDefault = true;

            var baseHudLayout = baseHudLayouts.Layouts.FirstOrDefault(x => x.LayoutId == layout.LayoutId && x.Name == layout.Name);

            if (baseHudLayout == null)
            {
                return;
            }

            baseHudLayouts.Layouts.Where(x => x.LayoutId == layout.LayoutId).ForEach(x => x.IsDefault = false);
            baseHudLayout.IsDefault = true;

            InternalSave();
        }

        public HudSavedLayout Load(string name, int layoutId)
        {
            var layout = hudLayouts.Layouts.FirstOrDefault(x => x.Name.Equals(name) && x.LayoutId == layoutId);
            return layout;
        }

        /// <summary>
        /// Delete layout
        /// </summary>
        /// <param name="layout">Layout to delete</param>
        public bool Delete(HudSavedLayout layout, out HudSavedLayout activeLayout)
        {
            activeLayout = null;

            if (layout == null)
            {
                return false;
            }

            var currentLayouts = Layouts.Layouts.Where(x => x.LayoutId == layout.LayoutId).ToArray();

            if (currentLayouts.Length < 2)
            {
                return false;
            }

            Layouts.Layouts.Remove(layout);

            var baseLayout = baseHudLayouts.Layouts.FirstOrDefault(x => x.LayoutId == layout.LayoutId && x.Name == layout.Name);

            if (baseLayout != null)
            {
                baseHudLayouts.Layouts.Remove(baseLayout);
            }

            if (layout.IsDefault)
            {
                activeLayout = currentLayouts.FirstOrDefault();
                SetLayoutActive(activeLayout);
            }

            InternalSave();

            return true;
        }

        /// <summary>
        /// Save existing data
        /// </summary>
        /// <param name="hudData">Hud Data</param>
        public void Save(HudSavedDataInfo hudData)
        {
            if (hudData == null)
            {
                return;
            }

            var defaultLayout = Layouts.Layouts.FirstOrDefault(x => x.LayoutId == hudData.LayoutId && x.IsDefault);

            if (defaultLayout == null)
            {
                return;
            }

            defaultLayout.HudPositions = hudData.HudTable.HudElements.Select(x => new HudSavedPosition
            {
                Height = x.Height,
                Position = x.Position,
                Width = x.Width,
                Seat = x.Seat,
                HudType = x.HudType
            }).ToList();

            defaultLayout.HudStats = hudData.Stats.Select(x =>
            {
                var statInfoBreak = x as StatInfoBreak;

                if (statInfoBreak != null)
                {
                    return statInfoBreak.Clone();
                }

                return x.Clone();
            }).ToList();
        }

        /// <summary>
        /// Save specified layout
        /// </summary>
        /// <param name="hudLayout">Layout</param>
        public void Save(HudSavedLayout hudLayout)
        {
            if (hudLayout == null)
            {
                return;
            }

            var layout = Layouts.Layouts.FirstOrDefault(x => x.LayoutId == hudLayout.LayoutId && x.IsDefault);

            // save only same reference, otherwise need to merge data
            if (ReferenceEquals(hudLayout, layout))
            {
                InternalSave();
            }
        }

        /// <summary>
        /// Save only not existing items as defaults
        /// </summary>
        /// <param name="hudTables">Hud tables</param>
        public void SaveDefaults(Dictionary<int, HudTableViewModel> hudTables)
        {
            if (hudTables == null)
            {
                return;
            }

            var layoutsToModify = (from hudTable in hudTables
                                   join layout in Layouts.Layouts.Where(x => x.IsDefault) on hudTable.Key equals layout.LayoutId into gj
                                   from grouped in gj.DefaultIfEmpty()
                                   select new { LayoutId = hudTable.Key, HudTable = hudTable.Value, Layout = grouped }).ToArray();

            layoutsToModify.ForEach(x =>
            {
                if (x.Layout == null)
                {
                    var newLayout = new HudSavedLayout
                    {
                        IsDefault = true,
                        Name = DefaultHudName,
                        LayoutId = x.LayoutId,
                        HudPlayerTypes = CreateDefaultPlayerTypes(x.HudTable.TableLayout.TableType),
                        HudBumperStickerTypes = CreateDefaultBumperStickers(),
                        HudPositions = x.HudTable.HudElements.Select(y => new HudSavedPosition
                        {
                            Height = y.Height,
                            Position = y.Position,
                            Width = y.Width,
                            Seat = y.Seat,
                            HudType = y.HudType
                        }).ToList()
                    };

                    Layouts.Layouts.Add(newLayout);

                    MergeWithBaseLayout(newLayout);
                }
            });

            ImportPredefinedLayouts();

            InternalSave();
        }

        /// <summary>
        /// Save new layout
        /// </summary>
        /// <param name="hudData">Hud Data</param>
        public HudSavedLayout SaveAs(HudSavedDataInfo hudData)
        {
            if (hudData == null)
            {
                return null;
            }

            var layout = Layouts.Layouts.FirstOrDefault(x => x.LayoutId == hudData.LayoutId && x.Name.Equals(hudData.Name));

            if (layout == null)
            {
                layout = new HudSavedLayout
                {
                    LayoutId = hudData.LayoutId,
                    Name = hudData.Name,
                    HudPlayerTypes = CreateDefaultPlayerTypes(hudData.HudTable.TableLayout.TableType),
                    HudBumperStickerTypes = CreateDefaultBumperStickers(),
                };

                Layouts.Layouts.Add(layout);
            }

            layout.HudPositions = hudData.HudTable.HudElements.Select(x => new HudSavedPosition
            {
                Height = x.Height,
                Position = x.Position,
                Width = x.Width,
                Seat = x.Seat,
                HudType = x.HudType
            }).ToList();

            layout.HudStats = hudData.Stats.Select(x =>
            {
                var statInfoBreak = x as StatInfoBreak;

                if (statInfoBreak != null)
                {
                    return statInfoBreak.Clone();
                }

                return x.Clone();
            }).ToList();

            SetLayoutActive(layout);

            MergeWithBaseLayout(layout);

            InternalSave();

            return layout;
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <param name="layout">Layout to be exported</param>
        /// <param name="path">Path to file</param>
        public void Export(HudSavedDataInfo hudData, string path)
        {
            if (hudData == null)
            {
                return;
            }

            var layoutToBeExported = Layouts.Layouts.FirstOrDefault(x => x.LayoutId == hudData.LayoutId && x.IsDefault);

            if (layoutToBeExported == null)
            {
                return;
            }

            layoutToBeExported.HudPositions = hudData.HudTable.HudElements.Select(x => new HudSavedPosition
            {
                Height = x.Height,
                Position = x.Position,
                Width = x.Width,
                Seat = x.Seat,
                HudType = x.HudType
            }).ToList();

            layoutToBeExported.HudStats = hudData.Stats.Select(x =>
            {
                var statInfoBreak = x as StatInfoBreak;

                if (statInfoBreak != null)
                {
                    return statInfoBreak.Clone();
                }

                return x.Clone();
            }).ToList();

            var hudSavedLayouts = new HudSavedLayouts();
            hudSavedLayouts.Layouts.Add(layoutToBeExported);

            try
            {
                using (var fs = File.Open(path, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));
                    xmlSerializer.Serialize(fs, hudSavedLayouts);
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
        public HudSavedLayout Import(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                using (var fs = File.Open(path, FileMode.Open))
                {
                    var importedHudLayout = Import(fs, true);
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
        /// <param name="hudElements">Hud elements</param>
        /// <param name="layoutId">Layout</param>
        public void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, int layoutId)
        {
            var layout = Layouts.Layouts.FirstOrDefault(x => x.LayoutId == layoutId && x.IsDefault);

            if (layout == null)
            {
                return;
            }

            // get total hands now to prevent enumeration in future
            var hudElementsTotalHands = (from hudElement in hudElements
                                         from stat in hudElement.StatInfoCollection
                                         where stat.Stat == Stat.TotalHands
                                         select new { HudElement = hudElement, TotalHands = stat.CurrentValue }).ToDictionary(x => x.HudElement, x => x.TotalHands);

            // get match ratios by player
            var matchRatiosByPlayer = (from playerType in layout.HudPlayerTypes
                                       from hudElement in hudElements
                                       let matchRatio = GetMatchRatio(hudElement, playerType)
                                       where playerType.EnablePlayerProfile && playerType.MinSample <= hudElementsTotalHands[hudElement]
                                       group new MatchRatio
                                       {
                                           IsInRange = matchRatio.Item1,
                                           Ratio = matchRatio.Item2,
                                           ExtraRatio = matchRatio.Item3,
                                           PlayerType = playerType
                                       } by hudElement into grouped
                                       select new PlayerMatchRatios
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

                matchRatioPlayer.HudElement.PlayerIconToolTip = String.Format("{0}: {1}", matchRatioPlayer.HudElement.PlayerName.Split('_').FirstOrDefault(), playerType.Name);

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
        /// <param name="hudElements">Hud elements</param>
        /// <param name="layoutId">Layout</param>
        public void SetStickers(IEnumerable<HudElementViewModel> hudElements, int layoutId)
        {
            var layout = Layouts.Layouts.FirstOrDefault(x => x.LayoutId == layoutId && x.IsDefault);

            if (layout == null)
            {
                return;
            }

            var hudElementsTotalHands = (from hudElement in hudElements
                                         from stat in hudElement.StatInfoCollection
                                         where stat.Stat == Stat.TotalHands
                                         select new { HudElement = hudElement, TotalHands = stat.CurrentValue }).ToDictionary(x => x.HudElement, x => x.TotalHands);

            foreach (var item in hudElements)
            {
                item.Stickers = new ObservableCollection<HudBumperStickerType>(
                    layout.HudBumperStickerTypes
                    .Where(x => (hudElementsTotalHands[item] >= x.MinSample) 
                                && x.EnableBumperSticker 
                                && IsInRange(item, x.Stats)));
            }
        }

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

        /// <summary>
        /// Get path to image directory
        /// </summary>
        /// <returns>Path to image directory</returns>
        public string GetImageDirectory()
        {
            var executingApp = Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(executingApp), pathToImages);
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

        private Tuple<bool, decimal, decimal> GetMatchRatio(HudElementViewModel hudElement, HudPlayerType hudPlayerType)
        {
            var matchRatios = (from stat in hudPlayerType.Stats
                               let low = stat.Low.HasValue ? stat.Low.Value : -1
                               let high = stat.High.HasValue ? stat.High.Value : 100
                               let average = (high + low) / 2
                               let isStatDefined = stat.Low.HasValue || stat.High.HasValue
                               join hudElementStat in hudElement.StatInfoCollection on stat.Stat equals hudElementStat.Stat into gj
                               from grouped in gj.DefaultIfEmpty()
                               let inRange = grouped != null ? (grouped.CurrentValue >= low && grouped.CurrentValue <= high) : !isStatDefined
                               let isGroupAndStatDefined = grouped != null && isStatDefined
                               let matchRatio = isGroupAndStatDefined ? Math.Abs(grouped.CurrentValue - average) : 0
                               let extraMatchRatio = (isGroupAndStatDefined && (grouped.Stat == Stat.VPIP || grouped.Stat == Stat.PFR)) ?
                                                        matchRatio : 0
                               select new { Ratio = matchRatio, InRange = inRange, IsStatDefined = isStatDefined, ExtraMatchRatio = extraMatchRatio }).ToArray();

            return new Tuple<bool, decimal, decimal>(matchRatios.All(x => x.InRange) && matchRatios.Any(x => x.IsStatDefined), matchRatios.Sum(x => x.Ratio), matchRatios.Sum(x => x.ExtraMatchRatio));
        }

        private bool IsInRange(HudElementViewModel hudElement, IEnumerable<BaseHudRangeStat> rangeStats)
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

                var high = rangeStat.High.HasValue ? rangeStat.High.Value : 100;
                var low = rangeStat.Low.HasValue ? rangeStat.Low.Value : -1;

                if (stat.CurrentValue < low || stat.CurrentValue > high)
                {
                    return false;
                }
            }

            return true;
        }

        private decimal GetStatAverageValue(HudPlayerTypeStat stat)
        {
            var low = stat.Low.HasValue ? stat.Low.Value : 0;
            var high = stat.High.HasValue ? stat.High.Value : 100;

            return (high + low) / 2;
        }

        private void InternalSave()
        {
            try
            {
                if (!Directory.Exists(settingsFolder))
                {
                    Directory.CreateDirectory(settingsFolder);
                }

                layoutsFile = Path.Combine(settingsFolder, layoutsFileSettings);

                using (var fs = File.Open(layoutsFile, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));
                    xmlSerializer.Serialize(fs, baseHudLayouts);
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
        /// <param name="saveLayouts">Save imported layouts</param>
        /// <param name="overrideIsDefaults">If true then isDefaults will be false</param>
        /// <param name="skipIfExists">If true then layout will not be imported</param>
        /// <returns>Imported layout</returns>
        private HudSavedLayout Import(Stream stream, bool saveLayouts, bool overrideIsDefaults = true, bool skipIfExists = false)
        {
            Check.ArgumentNotNull(() => stream);

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));
                var importedHudLayouts = xmlSerializer.Deserialize(stream) as HudSavedLayouts;

                var importedHudLayout = importedHudLayouts.Layouts.FirstOrDefault();

                if (importedHudLayout == null || Layouts == null)
                {
                    return null;
                }

                var layouts = Layouts.Layouts.Where(x => x.LayoutId == importedHudLayout.LayoutId).ToArray();

                if (skipIfExists && layouts.Length > 1)
                {
                    return null;
                }

                importedHudLayout.HudPlayerTypes.ForEach(x =>
                {
                    x.Image = GetImageLink(x.ImageAlias);
                });

                if (importedHudLayout.HudBumperStickerTypes == null || importedHudLayout.HudBumperStickerTypes.Count == 0)
                {
                    importedHudLayout.HudBumperStickerTypes = CreateDefaultBumperStickers();
                }

                var counter = 1;

                while (layouts.Any(x => x.Name.Equals(importedHudLayout.Name)))
                {
                    importedHudLayout.Name = string.Format("{0} ({1})", importedHudLayout.Name, counter++);
                }

                if (overrideIsDefaults)
                {
                    importedHudLayout.IsDefault = false;
                }
                else if (importedHudLayout.IsDefault)
                {
                    layouts.ForEach(x => x.IsDefault = false);
                }

                Layouts.Layouts.Add(importedHudLayout);

                MergeWithBaseLayout(importedHudLayout);

                if (saveLayouts)
                {
                    InternalSave();
                }

                return importedHudLayout;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            return null;
        }

        /// <summary>
        /// Import predefined layouts (DriveHUD.Common.Resources.Layouts.*)
        /// </summary>
        private void ImportPredefinedLayouts()
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

            try
            {
                foreach (var predefinedLayout in predefinedLayouts)
                {
                    using (var stream = resourcesAssembly.GetManifestResourceStream(predefinedLayout))
                    {
                        Import(stream, false, false, true);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
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
                     StatsToMerge = tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 0, High = 17 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 0, High = 16 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 0, High = 4.3m }
                         } : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 0, High = 11 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 0, High = 11 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 0, High = 3.7m }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                     Name = "Fish",
                     ImageAlias = "Fish.png",
                     Image = GetImageLink("Fish.png"),
                     StatsToMerge =  tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 36 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 0, High = 13 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 0, High = 4 },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 0, High = 40 }
                         } : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 31 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 0, High = 11 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 0, High = 3.8m },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 0, High = 40 }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                     Name = "Standard Reg",
                     ImageAlias = "Standard Reg.png",
                     Image = GetImageLink("Standard Reg.png"),
                     StatsToMerge = tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 22, High = 27  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 18, High = 25 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 4.7m, High = 8.6m, },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 42 }
                         }  : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 16, High = 22  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 15, High = 21 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 4.5m, High = 7.6m, },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 42 }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                     Name = "Tight Reg",
                     ImageAlias = "book.png",
                     Image = GetImageLink("book.png"),
                     StatsToMerge = tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 18, High = 22  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 14, High = 21 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 3.2m, High = 6m, },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 41 }
                         }  : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 14, High = 18  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 14, High = 18 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 3.6m, High = 6.8m }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                     Name = "Bad LAG",
                     ImageAlias = "Bad LAG.png",
                     Image = GetImageLink("Bad LAG.png"),
                     StatsToMerge = tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 26, High = 35  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 21, High = 33 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 6m },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 43 }
                         } : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 22, High = 29  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 20, High = 28 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 5.5m, High = 9.6m }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                     Name = "Tricky LAG",
                     ImageAlias = "Tricky LAG.png",
                     Image = GetImageLink("Tricky LAG.png"),
                     StatsToMerge = tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 25, High = 34  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 21, High = 31 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 6.5m },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 45 }
                         }  : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 21, High = 28  },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 21, High = 28 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 6.0m, High = 10m },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 45 }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                     Name = "Whale",
                     ImageAlias = "Whale.png",
                     Image = GetImageLink("Whale.png"),
                     StatsToMerge = tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 44 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 0, High = 12 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 0, High = 4 }
                         } : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 42 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 0, High = 11 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 0, High = 4 },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 0, High = 42 }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                },
                new HudPlayerType(true)
                {
                     Name = "Nutball",
                     ImageAlias = "Nutball.png",
                     Image = GetImageLink("Nutball.png"),
                     StatsToMerge = tableType == EnumTableType.Six ?
                         // 6-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 40 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 22 }
                         }  : (tableType == EnumTableType.Nine) ?
                         // 9-max
                         new ObservableCollection<HudPlayerTypeStat>()
                         {
                             new HudPlayerTypeStat { Stat = Stat.VPIP, Low = 38 },
                             new HudPlayerTypeStat { Stat = Stat.PFR, Low = 22 },
                             new HudPlayerTypeStat { Stat = Stat.S3Bet, Low = 5 },
                             new HudPlayerTypeStat { Stat = Stat.AGG, Low = 44 }
                         } : new ObservableCollection<HudPlayerTypeStat>()
                }
            };

            return hudPlayerTypes;
        }

        private List<HudBumperStickerType> CreateDefaultBumperStickers()
        {
            var bumperStickers = new List<HudBumperStickerType>()
            {
                new HudBumperStickerType(true)
                {
                     Name = "One and Done",
                     SelectedColor = Colors.OrangeRed,
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

        /// <summary>
        /// Merge specified layout with cache
        /// </summary>
        /// <param name="layout">Layout to merge</param>
        private void MergeWithBaseLayout(HudSavedLayout layout)
        {
            if (baseHudLayouts == null)
            {
                return;
            }

            var layoutCopy = layout.Clone();

            var existingLayout = baseHudLayouts.Layouts.FirstOrDefault(x => x.LayoutId == layoutCopy.LayoutId && x.Name == layoutCopy.Name);

            if (existingLayout != null)
            {
                baseHudLayouts.Layouts.Remove(existingLayout);
            }

            if (layoutCopy.IsDefault)
            {
                baseHudLayouts.Layouts.Where(x => x.LayoutId == layout.LayoutId).ForEach(x => x.IsDefault = false);
            }

            baseHudLayouts.Layouts.Add(layoutCopy);
        }
    }
}