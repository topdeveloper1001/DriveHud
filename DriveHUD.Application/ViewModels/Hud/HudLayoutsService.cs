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

using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.TableConfigurators.PositionProviders;
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
using Model.Stats;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
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
        private const string PathToImages = @"data\PlayerTypes";
        private readonly string[] PredefinedLayoutPostfixes = new[] { string.Empty, "Vertical_1", "Vertical_2", "Horizontal" };

        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        private IEventAggregator eventAggregator;
      
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

                var layoutToDelete = GetLayout(layoutName);

                if (layoutToDelete == null || layoutToDelete.IsDefault)
                {
                    return false;
                }

                var layoutsDirectory = GetLayoutsDirectory();

                var fileName = HudLayoutMappings.Mappings.FirstOrDefault(m => m.Name == layoutToDelete.Name)?.FileName;

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

            var layout = GetLayout(hudData.Name);

            var isNewLayout = layout == null;

            // need to check if we don't change table type of layout
            if (!isNewLayout && hudData.Name.Equals(layout.Name, StringComparison.OrdinalIgnoreCase) &&
                layout.IsDefault && layout.TableType != hudData.LayoutInfo.TableType)
            {
                eventAggregator.GetEvent<MainNotificationEvent>().Publish(new MainNotificationEventArgs("DriveHUD", "The default layout can't be overwritten"));
                return null;
            }

            layout = hudData.LayoutInfo.Clone();
            layout.Name = hudData.Name;

            var fileName = InternalSave(layout);

            if (isNewLayout && !layout.IsDefault)
            {
                var layoutMappings = HudLayoutMappings.Mappings.RemoveByCondition(m => m.Name == layout.Name);

                var pokerSites = Enum.GetValues(typeof(EnumPokerSites)).OfType<EnumPokerSites>().Where(p => p != EnumPokerSites.Unknown);

                foreach (var pokerSite in pokerSites)
                {
                    foreach (EnumGameType gameType in Enum.GetValues(typeof(EnumGameType)))
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
                LogProvider.Log.Error(this, $"Layout from '{path}' has not been imported.", e);
            }

            return null;
        }

        /// <summary>
        /// Sets icons for hud elements based on stats and layout player type settings
        /// </summary>
        public void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, string layoutName)
        {
            var layout = GetLayout(layoutName);

            if (layout == null)
            {
                return;
            }

            // get total hands now to prevent enumeration in future
            var hudElementViewModels = hudElements as HudElementViewModel[] ?? hudElements.ToArray();

            var hudElementsTotalHands = (from hudElement in hudElementViewModels
                                         from stat in hudElement.StatInfoCollection
                                         where stat.Stat == Stat.TotalHands
                                         select new { HudElement = hudElement, TotalHands = stat.CurrentValue }).ToDictionary(x => x.HudElement, x => x.TotalHands);

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
        /// Sets stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        public void SetStickers(HudElementViewModel hudElement, IDictionary<string, Playerstatistic> stickersStatistics, string layoutName)
        {
            hudElement.Stickers = new ObservableCollection<HudBumperStickerType>();

            var layout = GetLayout(layoutName);

            if (layout == null || stickersStatistics == null)
            {
                return;
            }

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
        /// Gets path to the image directory
        /// </summary>
        /// <returns>Path to the image directory</returns>
        public string GetImageDirectory()
        {
            var executingApp = Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(executingApp), PathToImages);
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

            if (File.Exists(imageLink) && Path.GetExtension(imageLink).ToUpperInvariant().Equals(".PNG"))
            {
                return imageLink;
            }

            return string.Empty;
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

        #endregion

        #region Infrastructure

        /// <summary>
        /// Initializes <see cref="HudLayoutsService"/>
        /// </summary>
        protected virtual void Initialize()
        {
            try
            {
                eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

                var layoutsDirectory = GetLayoutsDirectory();

                var mappingsFilePath = Path.Combine(layoutsDirectory.FullName, $"{MappingsFileName}{LayoutFileExtension}");

                HudLayoutMappings = LoadLayoutMappings(mappingsFilePath);

                foreach (EnumTableType tableType in Enum.GetValues(typeof(EnumTableType)))
                {
                    foreach (var predefinedPostfix in PredefinedLayoutPostfixes)
                    {
                        var defaultLayoutInfo = GetPredefinedLayout(tableType, predefinedPostfix);

                        if (File.Exists(Path.Combine(layoutsDirectory.FullName, GetLayoutFileName(defaultLayoutInfo.Name))))
                        {
                            continue;
                        }

                        var fileName = InternalSave(defaultLayoutInfo);

                        var existingMapping = HudLayoutMappings.Mappings.FirstOrDefault(x => x.TableType == tableType &&
                                                    x.Name == defaultLayoutInfo.Name);

                        if (existingMapping == null)
                        {
                            HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                            {
                                TableType = tableType,
                                Name = defaultLayoutInfo.Name,
                                IsDefault = string.IsNullOrEmpty(predefinedPostfix),
                                FileName = Path.GetFileName(fileName)
                            });
                        }
                        else
                        {
                            existingMapping.FileName = Path.GetFileName(fileName);
                        }
                    }
                }

                SaveLayoutMappings(mappingsFilePath, HudLayoutMappings);
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

        private bool IsInRange(HudElementViewModel hudElement, IEnumerable<BaseHudRangeStat> rangeStats, HudIndicators source)
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
            Check.ArgumentNotNull(() => stream);

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                var layout = xmlSerializer.Deserialize(stream) as HudLayoutInfoV2;

                // set image after loading
                layout?.HudPlayerTypes.ForEach(x =>
                {
                    if (x != null)
                    {
                        x.Image = GetImageLink(x.ImageAlias);
                    }
                });

                // initialize filter predicates for bumper stickers
                layout?.HudBumperStickerTypes.ForEach(x =>
                {
                    x?.InitializeFilterPredicate();
                });

                var layoutStats = layout.LayoutTools.OfType<IHudLayoutStats>().SelectMany(x => x.Stats).ToArray();
                StatInfoHelper.UpdateStats(layoutStats);

                return layout;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Layout could not be loaded.", e);
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