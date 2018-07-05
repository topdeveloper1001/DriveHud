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
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Data;
using Model.Enums;
using Model.Events;
using Model.Hud;
using Model.Stats;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Service for initializing, loading, deleting layouts of the hud
    /// </summary>
    internal class HudLayoutsService : IHudLayoutsService
    {
        protected string LayoutFileExtension;
        protected string MappingsFileName;
        private readonly string[] PredefinedLayoutPostfixes = new[] { string.Empty, "Cash", "Vertical_1", "Horizontal", "Winamax" };

        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        private readonly IEventAggregator eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        private readonly IHudPlayerTypeService playerTypeService = ServiceLocator.Current.GetInstance<IHudPlayerTypeService>();

        /// <summary>
        /// Initializes an instance of <see cref="HudLayoutsService"/>
        /// </summary>        
        public HudLayoutsService()
        {
            LayoutFileExtension = StringFormatter.GetLayoutsExtension();
            MappingsFileName = StringFormatter.GetLayoutsMappings();

            Initialize();
        }

        #region Properties

        /// <summary>
        /// Gets or sets <see cref="HudLayoutMappings"/> the mappings of the layouts
        /// </summary>
        public HudLayoutMappings HudLayoutMappings { get; set; }

        #endregion

        #region Implementation of IHudLayoutsService

        /// <summary>
        /// Saves the mapping of the layout to the file on the default path
        /// </summary>
        public void SaveLayoutMappings()
        {
            var layoutsDirectory = GetLayoutsDirectory();
            var mappingsFilePath = Path.Combine(layoutsDirectory.FullName, $"{MappingsFileName}{LayoutFileExtension}");

#if DEBUG
            if (HudLayoutMappings.Mappings.Count < 2)
            {
                LogProvider.Log.Error(this, "Layouts mappings were cleared!.");
            }
#endif 

            SaveLayoutMappings(mappingsFilePath, HudLayoutMappings);
        }

        /// <summary>
        /// Sets active layout for the specified <see cref="EnumPokerSites"/> poker site, <see cref="EnumGameType"/> game type and <see cref="EnumTableType"/> table type
        /// </summary>
        /// <param name="hudToLoad">Layout to be set as active</param>
        /// <param name="pokerSite">Poker site to set active layout</param>
        /// <param name="gameType">Game type to set active layout</param>
        /// <param name="tableType">Table type to set active layout</param>
        public void SetActiveLayout(HudLayoutInfoV2 hudToLoad, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            try
            {
                var mappings = HudLayoutMappings.Mappings.
                                Where(m => m.PokerSite == pokerSite
                                        && m.GameType == gameType
                                        && m.TableType == tableType).ToArray();

                var mapping = mappings.FirstOrDefault(x => x.Name == hudToLoad.Name);

                if (mapping == null)
                {
                    mapping = new HudLayoutMapping
                    {
                        PokerSite = pokerSite,
                        TableType = tableType,
                        GameType = gameType,
                        IsDefault = hudToLoad.IsDefault,
                        Name = hudToLoad.Name,
                        FileName = GetLayoutFileName(hudToLoad.Name)
                    };

                    HudLayoutMappings.Mappings.Add(mapping);
                }

                mappings.Where(x => x.IsSelected).ForEach(x => x.IsSelected = false);

                mapping.IsSelected = true;

                SaveLayoutMappings();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Layout has not been set active", e);
            }
        }

        /// <summary>
        /// Deletes the specified layout
        /// </summary>
        /// <param name="layoutName">Name of the layout to delete</param>
        /// <returns>True if the layout is deleted, otherwise - false</returns>
        public bool Delete(string layoutName)
        {
            try
            {
                if (string.IsNullOrEmpty(layoutName))
                {
                    return false;
                }

                var mapping = HudLayoutMappings.Mappings.FirstOrDefault(m => m.Name == layoutName);

                if (mapping == null || mapping.IsDefault)
                {
                    LogProvider.Log.Error(this, $"Layout '{layoutName}' has not been found");
                    return false;
                }

                var layoutToDelete = GetLayout(layoutName);

                if (layoutToDelete == null)
                {
                    LogProvider.Log.Error(this, $"Layout '{layoutName}' has not been found");
                    return false;
                }

                var layoutsDirectory = GetLayoutsDirectory();

                var fileName = mapping.FileName;

                if (string.IsNullOrEmpty(fileName))
                {
                    LogProvider.Log.Error(this, $"Layout '{layoutName}' has not been found");
                    return false;
                }

                HudLayoutMappings.Mappings.
                    RemoveByCondition(m => string.Equals(m.FileName, Path.GetFileName(fileName), StringComparison.InvariantCultureIgnoreCase));

                SaveLayoutMappings();

                File.Delete(Path.Combine(layoutsDirectory.FullName, fileName));
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Layout '{layoutName}' has not been deleted properly.", e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves the specified layout on the default path
        /// </summary>
        /// <param name="hudLayout">The layout to save</param>
        public void Save(HudLayoutInfoV2 hudLayout)
        {
            if (hudLayout == null)
            {
                return;
            }

            InternalSave(hudLayout);
        }

        /// <summary>
        /// Saves layout based on the specified <see cref="HudSavedDataInfo"/> data
        /// </summary>
        /// <param name="hudData">Data to save layout</param>
        public HudLayoutInfoV2 SaveAs(HudSavedDataInfo hudData)
        {
            if (hudData == null || string.IsNullOrWhiteSpace(hudData.Name))
            {
                return null;
            }

            var originalLayout = GetLayout(hudData.Name);

            var isNewLayout = originalLayout == null;

            // need to check if we don't change table type of layout
            if (!isNewLayout && hudData.Name.Equals(originalLayout.Name, StringComparison.OrdinalIgnoreCase) &&
                originalLayout.IsDefault && originalLayout.TableType != hudData.LayoutInfo.TableType)
            {
                eventAggregator.GetEvent<MainNotificationEvent>().Publish(new MainNotificationEventArgs("DriveHUD", "The default layout can't be overwritten"));
                return null;
            }

            var layout = hudData.LayoutInfo.Clone();
            layout.Name = hudData.Name;

            if (isNewLayout)
            {
                layout.IsDefault = false;
            }
            else
            {
                var nonPopupToolsUpdateMap = (from nonPopupTool in originalLayout.LayoutTools.OfType<HudLayoutNonPopupTool>()
                                              join tool in layout.LayoutTools.OfType<HudLayoutNonPopupTool>() on new { nonPopupTool.Id, nonPopupTool.ToolType } equals new { tool.Id, tool.ToolType } into grouped
                                              from groupedTool in grouped
                                              select new
                                              {
                                                  Tool = groupedTool,
                                                  Positions = nonPopupTool.Positions
                                              }).ToArray();

                nonPopupToolsUpdateMap.ForEach(x => x.Tool.Positions = x.Positions);
            }

            var fileName = InternalSave(layout);

            if (isNewLayout)
            {
                var layoutMappings = HudLayoutMappings.Mappings.RemoveByCondition(m => m.Name == layout.Name);

                var pokerSites = Enum.GetValues(typeof(EnumPokerSites)).OfType<EnumPokerSites>().Where(p => p != EnumPokerSites.Unknown);

                HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                {
                    FileName = Path.GetFileName(fileName),
                    Name = layout.Name,
                    TableType = layout.TableType,
                    IsSelected = false,
                    IsDefault = false
                });

                SaveLayoutMappings();
            }

            return layout;
        }

        /// <summary>
        /// Exports <see cref="HudLayoutInfoV2"/> the layout to the specified path
        /// </summary>
        /// <param name="layout">Layout to export</param>
        /// <param name="path">Path to file</param>
        public void Export(HudLayoutInfoV2 layout, string path)
        {
            if (layout == null)
            {
                return;
            }

            locker.EnterReadLock();

            try
            {
                using (var fs = File.Open(path, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                    xmlSerializer.Serialize(fs, layout);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Layout {layout.Name} has not been exported", e);
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Imports <see cref="HudLayoutInfoV2"/> layout on the specified path
        /// </summary>
        /// <param name="path">Path to layout</param>
        public HudLayoutInfoV2 Import(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                HudLayoutInfoV2 importedHudLayout;

                locker.EnterReadLock();

                try
                {
                    using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        importedHudLayout = LoadLayoutFromStream(fs);
                    }
                }
                finally
                {
                    locker.ExitReadLock();
                }

                var layoutName = importedHudLayout.Name;
                importedHudLayout.IsDefault = false;

                var i = 1;

                while (HudLayoutMappings.Mappings.Any(l => l.Name == importedHudLayout.Name))
                {
                    importedHudLayout.Name = $"{layoutName} ({i})";
                    i++;
                }

                var fileName = InternalSave(importedHudLayout);

                HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                {
                    FileName = Path.GetFileName(fileName),
                    Name = importedHudLayout.Name,
                    TableType = importedHudLayout.TableType,
                    IsSelected = false,
                    IsDefault = false
                });

                SaveLayoutMappings();

                return importedHudLayout;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Layout from '{path}' has not been imported.", e);
            }

            return null;
        }

        /// <summary>
        /// Exports <see cref="IEnumerable{HudPlayerType}"/> to the specified path
        /// </summary>
        /// <param name="path">Path to file</param>
        public void ExportPlayerType(HudPlayerType[] playerTypes, string path)
        {
            if (playerTypes == null || playerTypes.Length == 0)
            {
                return;
            }

            try
            {
                using (var fs = File.Open(path, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudPlayerType[]));
                    xmlSerializer.Serialize(fs, playerTypes);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Player types has not been exported", e);
            }
        }

        /// <summary>
        /// Imports <see cref="HudPlayerType"/> on the specified path
        /// </summary>
        /// <param name="path">Path to player type</param>
        public HudPlayerType[] ImportPlayerType(string path)
        {
            if (!File.Exists(path))
            {
                LogProvider.Log.Error($"Player type could not be imported. File '{path}' not found.");
                return null;
            }

            try
            {
                using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudPlayerType[]));
                    var playerTypes = xmlSerializer.Deserialize(fs) as HudPlayerType[];

                    playerTypes.ForEach(playerType =>
                    {
                        playerType.Image = playerTypeService.GetImageLink(playerType.ImageAlias);
                    });

                    return playerTypes;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Player type from '{path}' has not been imported.", e);
            }

            return null;
        }

        /// <summary>
        /// Sets icons for hud elements based on stats and layout player type settings
        /// </summary>
        public void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, HudLayoutInfoV2 layout)
        {
            if (layout == null)
            {
                return;
            }

            // get total hands now to prevent enumeration in future
            var hudElementViewModels = hudElements as HudElementViewModel[] ?? hudElements.ToArray();

            var hudElementsTotalHands = hudElementViewModels
                .Select(hudElement =>
                {
                    var totalHandsStatInfo = hudElement.StatInfoCollection.FirstOrDefault(x => x.Stat == Stat.TotalHands);

                    return new
                    {
                        HudElement = hudElement,
                        TotalHands = totalHandsStatInfo != null ? totalHandsStatInfo.CurrentValue : default(decimal)
                    };
                })
                .ToDictionary(x => x.HudElement, x => x.TotalHands);

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
                                           ExtraMatchRatios = grouped.Where(x => x.ExtraRatio != 0).OrderBy(x => x.ExtraRatio).ToList()
                                       }).ToList();

            var proccesedElements = new HashSet<int>();

            void proccessPlayerRatios(PlayerMatchRatios matchRatioPlayer, List<MatchRatio> matchRatios)
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
                    matchRatioPlayer.HudElement.PlayerIcon = playerTypeService.GetImageLink(playerType.ImageAlias);
                }

                matchRatioPlayer.HudElement.PlayerIconToolTip =
                    $"{matchRatioPlayer.HudElement.PlayerName.Split('_').FirstOrDefault()}: {playerType.Name}";

                proccesedElements.Add(matchRatioPlayer.HudElement.Seat);
            }

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
        public IList<string> GetValidStickers(Playerstatistic statistic, HudLayoutInfoV2 layout)
        {
            if (layout == null || statistic == null)
            {
                return new List<string>();
            }

            return layout.HudBumperStickerTypes?
                .Where(x => x.FilterPredicate != null && x.FilterPredicate.Compile()(statistic))
                .Select(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// Sets stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        public void SetStickers(HudElementViewModel hudElement, IDictionary<string, HudLightIndicators> stickersStatistics, HudLayoutInfoV2 layout)
        {
            hudElement.Stickers = new ObservableCollection<HudBumperStickerType>();

            if (layout == null || stickersStatistics == null)
            {
                return;
            }

            foreach (var sticker in layout.HudBumperStickerTypes.Where(x => x.EnableBumperSticker))
            {
                if (!stickersStatistics.TryGetValue(sticker.Name, out HudLightIndicators statistics) ||
                    statistics.TotalHands < sticker.MinSample || statistics.TotalHands == 0 ||
                    !IsInRange(hudElement, sticker.Stats, statistics))
                {
                    continue;
                }

                hudElement.Stickers.Add(sticker);
            }
        }

        /// <summary>
        /// Gets active <see cref="HudLayoutInfoV2"/> layout for specified <see cref="EnumPokerSites"/> poker site, <see cref="EnumTableType"/> table type and <see cref="EnumGameType"/> game type
        /// </summary>
        /// <param name="pokerSite">Poker site</param>
        /// <param name="tableType">Type of table</param>
        /// <param name="gameType">Type of game</param>
        /// <returns>Active layout</returns>
        public HudLayoutInfoV2 GetActiveLayout(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType)
        {
            var mapping =
               HudLayoutMappings.Mappings.FirstOrDefault(
                   m => m.PokerSite == pokerSite && m.TableType == tableType && m.GameType == gameType && m.IsSelected);

            if (mapping == null)
            {
                mapping =
                    HudLayoutMappings.Mappings.FirstOrDefault(
                        m => (m.PokerSite == pokerSite || !m.PokerSite.HasValue) && m.TableType == tableType && (m.GameType == gameType || !m.GameType.HasValue) && m.IsSelected) ??
                    HudLayoutMappings.Mappings.FirstOrDefault(
                        m => (m.PokerSite == pokerSite || !m.PokerSite.HasValue) && m.TableType == tableType && (m.GameType == gameType || !m.GameType.HasValue) && m.IsDefault);
            }

            if (mapping == null)
            {
                return LoadDefault(tableType);
            }

            return LoadLayout(mapping);
        }

        /// <summary>
        /// Gets layout with the specified name
        /// </summary>
        /// <param name="name">Name of layout to get</param>
        /// <returns>Layout</returns>
        public HudLayoutInfoV2 GetLayout(string name)
        {
            var mapping = HudLayoutMappings.Mappings.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return LoadLayout(mapping);
        }

        /// <summary>
        /// Gets layouts names for specified <see cref="EnumTableType"/>  table type
        /// </summary>
        /// <param name="tableType">Type of table</param>
        /// <returns>Collection of names</returns>
        public IEnumerable<string> GetLayoutsNames(EnumTableType tableType)
        {
            return HudLayoutMappings.Mappings.Where(x => x.TableType == tableType).OrderByDescending(m => m.IsDefault).Select(m => m.Name).Distinct();
        }

        /// <summary>
        /// Gets the names of available layouts for specified <see cref="EnumPokerSites"/> poker site, <see cref="EnumTableType"/> table type and <see cref="EnumGameType"/> game type
        /// </summary>
        /// <param name="pokerSite">Poker site</param>
        /// <param name="tableType">Type of table</param>
        /// <param name="gameType">Type of game</param>        
        public IEnumerable<string> GetAvailableLayouts(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType)
        {
            var defaultNames = HudLayoutMappings.Mappings.Where(m => m.TableType == tableType)
                        .Select(m => m.Name).Distinct().ToList();

            return defaultNames;
        }

        /// <summary>
        /// Gets the sorted list of <see cref="HudLayoutInfoV2"/> layouts for the specified <see cref="EnumTableType"/> table type
        /// </summary>
        /// <param name="tableType">Type of table</param>        
        public List<HudLayoutInfoV2> GetAllLayouts(EnumTableType tableType)
        {
            var result = new List<HudLayoutInfoV2>();

            var layoutMappings = HudLayoutMappings.Mappings.Where(x => x.TableType == tableType).Select(x => x.FileName).Distinct().ToArray();

            var layoutsDirectory = GetLayoutsDirectory();

            foreach (var layoutMapping in layoutMappings)
            {
                locker.EnterReadLock();

                try
                {
                    var fileName = Path.Combine(layoutsDirectory.FullName, layoutMapping);

                    using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var layout = LoadLayoutFromStream(fs);

                        if (layout != null)
                        {
                            if (layout.IsDefault)
                            {
                                result.Insert(0, layout);
                            }
                            else
                            {
                                result.Add(layout);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not load layout {layoutMapping}", e);
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }

            return result.OrderBy(l => l.TableType).ToList();
        }

        /// <summary>
        /// Gets the path to the directory with layouts
        /// </summary>
        /// <returns>Directory</returns>
        public DirectoryInfo GetLayoutsDirectory()
        {
            var layoutsDirectory = new DirectoryInfo(StringFormatter.GetLayoutsV2FolderPath());

            if (!layoutsDirectory.Exists)
            {
                try
                {
                    layoutsDirectory.Create();
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Directory '{layoutsDirectory.FullName}' for layouts has not been created.", e);
                }
            }

            return layoutsDirectory;
        }


        /// <summary>
        /// Duplicates the specified <see cref="HudLayoutInfoV2" />
        /// </summary>
        /// <param name="tableType">Table type of the duplicated layout</param>
        /// <param name="layoutName">Name of the duplicated layout</param>
        /// <param name="layoutToDuplicate">Layout to duplicate</param>
        /// <returns>The duplicated layout</returns>
        public HudLayoutInfoV2 DuplicateLayout(EnumTableType tableType, string layoutName, HudLayoutInfoV2 layoutToDuplicate)
        {
            if (layoutToDuplicate == null)
            {
                return null;
            }

            layoutName = layoutName ?? layoutToDuplicate.Name;

            var currentTableTypeText = CommonResourceManager.Instance.GetEnumResource(layoutToDuplicate.TableType);
            var duplicateTableTypeText = CommonResourceManager.Instance.GetEnumResource(tableType);

            // rename n-max to k-max in table name
            layoutName = layoutName.Replace(currentTableTypeText, duplicateTableTypeText);

            var copyIndex = 1;

            // try to get if copyIndex exists
            var startIndex = layoutName.LastIndexOf("(") + 1;
            var lastIndex = layoutName.LastIndexOf(")");

            if (startIndex > 0 && lastIndex > startIndex)
            {
                var copyIndexText = layoutName.Substring(startIndex, lastIndex - startIndex);

                if (int.TryParse(copyIndexText, out copyIndex))
                {
                    layoutName = layoutName.Remove(startIndex - 1).Trim();
                }
                else
                {
                    copyIndex = 1;
                }
            }

            var newLayoutName = layoutName;

            var layoutsDirectory = GetLayoutsDirectory().FullName;
            var layoutFile = Path.Combine(layoutsDirectory, GetLayoutFileName(newLayoutName));

            // name is busy
            while (File.Exists(layoutFile))
            {
                newLayoutName = $"{layoutName} ({copyIndex++})";
                layoutFile = Path.Combine(layoutsDirectory, GetLayoutFileName(newLayoutName));
            }

            var factory = ServiceLocator.Current.GetInstance<IHudToolFactory>();

            var duplicateLayout = layoutToDuplicate.Clone();
            duplicateLayout.TableType = tableType;
            duplicateLayout.Name = newLayoutName;
            duplicateLayout.IsDefault = false;

            foreach (var layoutTool in duplicateLayout.LayoutTools.OfType<HudLayoutNonPopupTool>())
            {
                layoutTool.Positions = new List<HudPositionsInfo>();

                var position = layoutTool.UIPositions.FirstOrDefault(x => x.Seat == 1);

                if (position == null)
                {
                    LogProvider.Log.Error($"{layoutToDuplicate.Name} could not be duplicated. Position for seat #1 has not been found");
                    return null;
                }

                var uiPositions = factory.GetHudUIPositions(tableType, layoutToDuplicate.TableType, position.Position);
                layoutTool.UIPositions = uiPositions;
            }

            var duplicateLayoutFile = InternalSave(duplicateLayout);

            if (!File.Exists(duplicateLayoutFile))
            {
                return null;
            }

            if (HudLayoutMappings.Mappings.Any(x => x.Name == newLayoutName))
            {
                return duplicateLayout;
            }

            var mapping = new HudLayoutMapping
            {
                FileName = Path.GetFileName(duplicateLayoutFile),
                IsDefault = false,
                IsSelected = false,
                Name = newLayoutName,
                TableType = tableType
            };

            HudLayoutMappings.Mappings.Add(mapping);

            SaveLayoutMappings();

            return duplicateLayout;
        }

        #endregion

        #region Infrastructure

        /// <summary>
        /// Initializes <see cref="HudLayoutsService"/>
        /// </summary>
        protected virtual void Initialize()
        {
            try
            {
                var layoutsDirectory = GetLayoutsDirectory();

                var mappingsFilePath = Path.Combine(layoutsDirectory.FullName, $"{MappingsFileName}{LayoutFileExtension}");

                HudLayoutMappings = LoadLayoutMappings(mappingsFilePath);

                var predefinedMappings = GetPredefinedMappings();

                foreach (EnumTableType tableType in Enum.GetValues(typeof(EnumTableType)))
                {
                    foreach (var predefinedPostfix in PredefinedLayoutPostfixes)
                    {
                        var defaultLayoutInfo = GetPredefinedLayout(tableType, predefinedPostfix);

                        if (defaultLayoutInfo == null || File.Exists(Path.Combine(layoutsDirectory.FullName, GetLayoutFileName(defaultLayoutInfo.Name))))
                        {
                            continue;
                        }

                        var fileName = InternalSave(defaultLayoutInfo);

                        var existingMapping = HudLayoutMappings.Mappings.FirstOrDefault(x => x.TableType == tableType &&
                                                    x.Name == defaultLayoutInfo.Name);

                        if (existingMapping == null)
                        {
                            var predefinedMapping = predefinedMappings.Mappings.FirstOrDefault(x => x.TableType == tableType &&
                                                    x.Name == defaultLayoutInfo.Name);

                            if (predefinedMapping != null)
                            {
                                HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                                {
                                    TableType = tableType,
                                    Name = predefinedMapping.Name,
                                    IsDefault = predefinedMapping.IsDefault,
                                    FileName = Path.GetFileName(fileName),
                                    PokerSite = predefinedMapping.PokerSite,
                                    GameType = predefinedMapping.GameType,
                                    IsSelected = predefinedMapping.IsSelected
                                });
                            }
                            else
                            {
                                HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                                {
                                    TableType = tableType,
                                    Name = defaultLayoutInfo.Name,
                                    IsDefault = defaultLayoutInfo.IsDefault,
                                    FileName = Path.GetFileName(fileName)
                                });
                            }
                        }
                        else
                        {
                            existingMapping.FileName = Path.GetFileName(fileName);
                        }
                    }
                }

                SaveLayoutMappings(mappingsFilePath, HudLayoutMappings);
                RemoveNotExistingLayouts(layoutsDirectory.FullName);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Layouts have not been initialized.", e);
            }
        }

        /// <summary>
        /// Loads <see cref="HudLayoutMappings"/> mappings of the layout defined in specified file
        /// </summary>
        /// <param name="fileName">File with layout</param>
        /// <returns>Mappings of layout</returns>
        protected HudLayoutMappings LoadLayoutMappings(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return new HudLayoutMappings();
            }

            locker.EnterReadLock();

            try
            {
                var layoutsFolder = GetLayoutsDirectory();

                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutMappings));
                    var hudLayoutMappings = xmlSerializer.Deserialize(stream) as HudLayoutMappings;
                    return hudLayoutMappings;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
            finally
            {
                locker.ExitReadLock();
            }

            return new HudLayoutMappings();
        }

        /// <summary>
        /// Removes not-existing layouts from mappings
        /// </summary>
        protected void RemoveNotExistingLayouts(string layoutsDirectory)
        {
            locker.EnterReadLock();

            try
            {
                foreach (var mapping in HudLayoutMappings.Mappings.ToArray())
                {
                    var layoutFile = Path.Combine(layoutsDirectory, mapping.FileName);

                    if (!File.Exists(layoutFile))
                    {
                        HudLayoutMappings.Mappings.Remove(mapping);
                        LogProvider.Log.Warn($"Layout '{mapping.Name}' (default={mapping.IsDefault}) has not been found at '{layoutFile}' and will be skipped.");
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Saves <see cref="HudLayoutMappings"/> mappings of layout to the specified file
        /// </summary>
        /// <param name="fileName">The file to save to</param>
        /// <param name="mappings">The mappings of layout to save to the file</param>
        protected void SaveLayoutMappings(string fileName, HudLayoutMappings mappings)
        {
            locker.EnterWriteLock();

            try
            {
                using (var fs = File.Open(fileName, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutMappings));
                    xmlSerializer.Serialize(fs, mappings);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Mappings has not been saved.", e);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Loads default <see cref="HudLayoutInfoV2"/> for specified <see cref="EnumTableType"/> table type
        /// </summary>
        /// <param name="tableType">Type of table</param>
        /// <returns>Default layout</returns>
        private HudLayoutInfoV2 LoadDefault(EnumTableType tableType)
        {
            var fileName = $"DH: {CommonResourceManager.Instance.GetEnumResource(tableType)}{LayoutFileExtension}".RemoveInvalidFileNameChars();
            return LoadLayout(Path.Combine(GetLayoutsDirectory().FullName, fileName));
        }

        /// <summary>
        /// Loads a <see cref="HudLayoutInfoV2"/> of the specified mapping
        /// </summary>
        /// <param name="mapping">Mapping to load layout</param>
        /// <returns>Loaded layout</returns>
        private HudLayoutInfoV2 LoadLayout(HudLayoutMapping mapping)
        {
            if (mapping == null || string.IsNullOrEmpty(mapping.FileName))
            {
                return null;
            }

            var pathToLayout = Path.Combine(GetLayoutsDirectory().FullName, mapping.FileName);

            return LoadLayout(pathToLayout);
        }

        /// <summary>
        /// Loads a <see cref="HudLayoutInfoV2"/> on the specified path
        /// </summary>
        /// <param name="fileName">Path to layout</param>
        /// <returns>Loaded layout</returns>
        private HudLayoutInfoV2 LoadLayout(string fileName)
        {
            locker.EnterReadLock();

            try
            {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var layout = LoadLayoutFromStream(stream);
                    return layout;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Layout has not been loaded on the '{fileName}' path.", e);
            }
            finally
            {
                locker.ExitReadLock();
            }

            return null;
        }

        /// <summary>
        /// Gets predefined <see cref="HudLayoutInfoV2"/> layout based on the table type
        /// </summary>
        /// <param name="tableType">Table type</param>
        /// <returns>Predefined <see cref="HudLayoutInfoV2"/> layout</returns>
        private HudLayoutInfoV2 GetPredefinedLayout(EnumTableType tableType, string postfix)
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

            try
            {
                if (!string.IsNullOrEmpty(postfix))
                {
                    postfix = $" {postfix}";
                }

                var resourceName = $"DriveHUD.Common.Resources.Layouts.DH {CommonResourceManager.Instance.GetEnumResource(tableType)}{postfix}.xml";

                using (var stream = resourcesAssembly.GetManifestResourceStream(resourceName))
                {
                    return LoadLayoutFromStream(stream);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not load predefined layout", e);
            }

            return null;
        }

        /// <summary>
        /// Gets the predefined mappings
        /// </summary>
        /// <returns></returns>
        private HudLayoutMappings GetPredefinedMappings()
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

            try
            {
                var resourceName = $"DriveHUD.Common.Resources.Layouts.Mappings.xml";

                using (var stream = resourcesAssembly.GetManifestResourceStream(resourceName))
                {
                    try
                    {
                        var xmlSerializer = new XmlSerializer(typeof(HudLayoutMappings));
                        var hudLayoutMappings = xmlSerializer.Deserialize(stream) as HudLayoutMappings;
                        return hudLayoutMappings;
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, "Layout could not be loaded.", e);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not load predefined layout", e);
            }

            return null;
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

        private bool IsInRange(HudElementViewModel hudElement, IEnumerable<BaseHudRangeStat> rangeStats, HudLightIndicators source)
        {
            if (!rangeStats.Any(x => x.High.HasValue || x.Low.HasValue))
            {
                return false;
            }

            foreach (var rangeStat in rangeStats)
            {
                if (!rangeStat.High.HasValue && !rangeStat.Low.HasValue)
                {
                    continue;
                }

                var stat = hudElement.StatInfoCollection.FirstOrDefault(x => x.Stat == rangeStat.Stat);

                if (stat == null)
                {
                    return false;
                }

                var propertyName = StatsProvider.GetStatProperyName(stat.Stat);

                var currentStat = new StatInfo();
                currentStat.AssignStatInfoValues(source, propertyName);

                var high = rangeStat.High ?? 100;
                var low = rangeStat.Low ?? -1;

                if (currentStat.CurrentValue < low || currentStat.CurrentValue > high)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Saves <see cref="HudLayoutInfoV2"/> hud layout to the predefined file based on the name of the layout
        /// </summary>
        /// <param name="hudLayoutInfo">Layout to save</param>
        /// <returns>Path to the saved layout</returns>
        private string InternalSave(HudLayoutInfoV2 hudLayoutInfo)
        {
            Check.ArgumentNotNull(() => hudLayoutInfo);

            var layoutsDirectory = GetLayoutsDirectory().FullName;

            var layoutsFile = Path.Combine(layoutsDirectory, GetLayoutFileName(hudLayoutInfo.Name));

            locker.EnterWriteLock();

            try
            {
                using (var fs = File.Open(layoutsFile, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                    xmlSerializer.Serialize(fs, hudLayoutInfo);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not save layout {hudLayoutInfo.Name} to the '{layoutsFile}'", e);
            }
            finally
            {
                locker.ExitWriteLock();
            }

            return layoutsFile;
        }

        /// <summary>
        /// Loads <see cref="HudLayoutInfoV2"/> layouts from specified stream
        /// </summary>
        /// <param name="stream">Stream to be used for loading</param>
        /// <returns>Loaded <see cref="HudLayoutInfoV2"/> layout</returns>
        private HudLayoutInfoV2 LoadLayoutFromStream(Stream stream)
        {
            if (stream == null)
            {
                return null;
            }

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                var layout = xmlSerializer.Deserialize(stream) as HudLayoutInfoV2;

                // set image after loading
                layout?.HudPlayerTypes.ForEach(x =>
                {
                    if (x != null)
                    {
                        x.Image = playerTypeService.GetImageLink(x.ImageAlias);
                    }
                });

                // initialize filter predicates for bumper stickers
                layout?.HudBumperStickerTypes.ForEach(x =>
                {
                    x?.InitializeFilterPredicate();
                });

                var layoutStats = layout.LayoutTools.OfType<IHudLayoutStats>().SelectMany(x => x.Stats).ToArray();
                StatsProvider.UpdateStats(layoutStats);

                return layout;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Layout could not be loaded.", e);
            }

            return null;
        }

        protected string GetLayoutFileName(string layoutName)
        {
            return $"{Path.GetInvalidFileNameChars().Aggregate(layoutName, (current, c) => current.Replace(c.ToString(), string.Empty))}{LayoutFileExtension}";
        }

        #endregion

        #region Private classes

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
    }
}