//-----------------------------------------------------------------------
// <copyright file="HudLayoutsMigrationService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;


namespace DriveHUD.Application.MigrationService.Migrators
{
    /// <summary>
    /// Service for layout v1.0
    /// </summary>
    internal class HudLayoutsMigrationService : HudLayoutsService
    {
        /// <summary>
        /// Initializes an instance of <see cref="HudLayoutsMigrationService"/>
        /// </summary>        
        public HudLayoutsMigrationService() : base()
        {
        }

        /// <summary>
        /// Gets the sorted list of <see cref="HudLayoutInfo"/> layouts for the specified <see cref="EnumTableType"/> table type
        /// </summary>
        /// <param name="tableType">Type of table</param>        
        public new List<HudLayoutInfo> GetAllLayouts(EnumTableType tableType)
        {
            var result = new List<HudLayoutInfo>();

            var layoutMappings = HudLayoutMappings.Mappings.Where(x => x.TableType == tableType).Select(x => x.FileName).Distinct().ToArray();

            var layoutsDirectory = GetLayoutsDirectory();

            foreach (var layoutMapping in layoutMappings)
            {
                try
                {
                    var fileName = Path.Combine(layoutsDirectory.FullName, layoutMapping);

                    using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var layout = LoadLayoutFromStream(fs);

                        if (layout != null)
                        {
                            if (layout.HudViewType == HudViewType.Plain)
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
            }

            return result.OrderBy(l => l.TableType).ToList();
        }

        /// <summary>
        /// Initializes <see cref="HudLayoutsMigrationService"/>
        /// </summary>
        protected override void Initialize()
        {
            var layoutsDirectory = GetLayoutsDirectory();

            var mappingsFilePath = Path.Combine(layoutsDirectory.FullName, $"{MappingsFileName}{LayoutFileExtension}");

            HudLayoutMappings = LoadLayoutMappings(mappingsFilePath);

            foreach (var tableType in Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>())
            {
                var defaultLayoutInfo = GetPredefindedLayout(tableType);
                defaultLayoutInfo.TableType = tableType;
                defaultLayoutInfo.IsDefault = true;

                foreach (var hudViewType in Enum.GetValues(typeof(HudViewType)).OfType<HudViewType>())
                {
                    defaultLayoutInfo.Name =
                        $"DH: {CommonResourceManager.Instance.GetEnumResource(tableType)} {(hudViewType == HudViewType.Plain ? string.Empty : $"{hudViewType} - Ignition/Bodog")}"
                            .Trim();

                    if (File.Exists(Path.Combine(layoutsDirectory.FullName, GetLayoutFileName(defaultLayoutInfo.Name))))
                    {
                        continue;
                    }

                    defaultLayoutInfo.HudPositionsInfo.Clear();
                    defaultLayoutInfo.HudViewType = hudViewType;

                    var pokerSites = hudViewType == HudViewType.Plain
                        ? Enum.GetValues(typeof(EnumPokerSites))
                            .OfType<EnumPokerSites>()
                            .Where(p => p != EnumPokerSites.Unknown && p != EnumPokerSites.IPoker)
                        : new[] { EnumPokerSites.Ignition, EnumPokerSites.Bodog };

                    if (hudViewType != HudViewType.Plain)
                    {
                        defaultLayoutInfo.UiPositionsInfo.ForEach(x => x.Width = HudDefaultSettings.BovadaRichHudElementWidth);
                    }

                    var fileName = InternalSave(defaultLayoutInfo);

                    var existingMapping = HudLayoutMappings.Mappings.FirstOrDefault(x => x.TableType == tableType &&
                                                x.Name == defaultLayoutInfo.Name &&
                                                x.IsDefault &&
                                                x.HudViewType == defaultLayoutInfo.HudViewType);

                    if (existingMapping == null)
                    {
                        HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                        {
                            TableType = tableType,
                            Name = defaultLayoutInfo.Name,
                            IsDefault = true,
                            HudViewType = defaultLayoutInfo.HudViewType,
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

        /// <summary>
        /// Gets the predefined layout for the specified <see cref="EnumTableType"/> 
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private HudLayoutInfo GetPredefindedLayout(EnumTableType tableType)
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

            try
            {
                var resourceName = $"DriveHUD.Common.Resources.Layouts.Default-{CommonResourceManager.Instance.GetEnumResource(tableType)}.xml";

                using (var stream = resourcesAssembly.GetManifestResourceStream(resourceName))
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

        /// <summary>
        /// Loads <see cref="HudLayoutInfo"/> layouts from specified stream
        /// </summary>
        /// <param name="stream">Stream to be used for loading</param>
        /// <returns>Loaded <see cref="HudLayoutInfo/> layout</returns>
        private HudLayoutInfo LoadLayoutFromStream(Stream stream)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                var layout = xmlSerializer.Deserialize(stream) as HudLayoutInfo;

                return layout;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            return null;
        }

        /// <summary>
        /// Saves <see cref="HudLayoutInfo"/> hud layout to the predefined file based on the name of the layout
        /// </summary>
        /// <param name="hudLayoutInfo">Layout to save</param>
        /// <returns>Path to the saved layout</returns>
        private string InternalSave(HudLayoutInfo hudLayoutInfo)
        {
            var layoutsDirectory = GetLayoutsDirectory().FullName;

            var layoutsFile = Path.Combine(layoutsDirectory, GetLayoutFileName(hudLayoutInfo.Name));

            try
            {
                using (var fs = File.Open(layoutsFile, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                    xmlSerializer.Serialize(fs, hudLayoutInfo);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            return layoutsFile;
        }
    }
}