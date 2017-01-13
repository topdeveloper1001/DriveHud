using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(13)]
    public class Migration0013NonDbLayoutsToNewLayoutModel : Migration
    {
        private const string layoutsFileSettings = "Layouts.xml";

        public override void Up()
        {
            LogProvider.Log.Info("Running migration #13");
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
                        var layouts = new List<HudLayoutInfo>();
                        foreach (var hudSavedLayout in hudLayouts.Layouts.Where(l=>l.Name != "Default"))
                        {
                            EnumPokerSites? pokerSite = null;
                            EnumGameType? gameType = null;
                            EnumTableType? tableType = null;
                            foreach (var ps in Enum.GetValues(typeof(EnumPokerSites)).Cast<EnumPokerSites>())
                            {
                                foreach (var gt in Enum.GetValues(typeof(EnumGameType)).Cast<EnumGameType>())
                                {
                                    foreach (var tt in Enum.GetValues(typeof(EnumTableType)).Cast<EnumTableType>())
                                    {
                                        if (GetHash(ps, gt, tt) == hudSavedLayout.LayoutId)
                                        {
                                            tableType = tt;
                                            pokerSite = ps;
                                            gameType = gt;
                                            break;
                                        }
                                    }
                                    if (tableType.HasValue)
                                        break;
                                }
                                if (tableType.HasValue)
                                    break;
                            }
                            if (tableType.HasValue)
                            {
                                var name = hudSavedLayout.Name.Replace($" {CommonResourceManager.Instance.GetEnumResource(tableType.Value)}", "").Trim();
                                var existingLayoutInfo = layouts.FirstOrDefault(l => l.Name == name);
                                if (existingLayoutInfo == null)
                                {
                                    var hudLayoutInfo = new HudLayoutInfo { IsDefault = false, Name = name };
                                    var hudTableDefinedProps = new HudTableDefinedProperties();
                                    hudTableDefinedProps.HudTableDefinition = new HudTableDefinition
                                    {
                                        GameType = gameType,
                                        PokerSite = pokerSite,
                                        TableType = tableType.Value
                                    };
                                    hudTableDefinedProps.HudBumperStickerTypes =
                                        hudSavedLayout.HudBumperStickerTypes.Select(c => c.Clone()).ToList();
                                    hudTableDefinedProps.HudPlayerTypes =
                                        hudSavedLayout.HudPlayerTypes.Select(c => c.Clone()).ToList();
                                    hudTableDefinedProps.HudPositions = hudSavedLayout.HudPositions.Select(p => p.Clone()).ToList();
                                    hudTableDefinedProps.HudStats = hudSavedLayout.HudStats.Select(s => s.Clone()).ToList();
                                    hudLayoutInfo.HudTableDefinedProperties.Add(hudTableDefinedProps);
                                    layouts.Add(hudLayoutInfo);
                                }
                                else
                                {
                                    var hudTableDefinedProps = new HudTableDefinedProperties();
                                    hudTableDefinedProps.HudTableDefinition = new HudTableDefinition
                                    {
                                        GameType = gameType,
                                        PokerSite = pokerSite,
                                        TableType = tableType.Value
                                    };
                                    hudTableDefinedProps.HudBumperStickerTypes =
                                        hudSavedLayout.HudBumperStickerTypes.Select(c => c.Clone()).ToList();
                                    hudTableDefinedProps.HudPlayerTypes =
                                        hudSavedLayout.HudPlayerTypes.Select(c => c.Clone()).ToList();
                                    hudTableDefinedProps.HudPositions = hudSavedLayout.HudPositions.Select(p => p.Clone()).ToList();
                                    hudTableDefinedProps.HudStats = hudSavedLayout.HudStats.Select(s => s.Clone()).ToList();
                                    existingLayoutInfo.HudTableDefinedProperties.Add(hudTableDefinedProps);
                                }
                            }
                        }
                        if (layouts.Any())
                        {
                            var layoutsDirectory =
                                new DirectoryInfo(Path.Combine(StringFormatter.GetAppDataFolderPath(), "Layouts"));
                            if (!layoutsDirectory.Exists)
                                layoutsDirectory.Create();
                            foreach (var hudLayoutInfo in layouts)
                            {
                                var file = Path.Combine(layoutsDirectory.FullName,
                                    $"{Path.GetInvalidFileNameChars().Aggregate(hudLayoutInfo.Name, (current, c) => current.Replace(c.ToString(), string.Empty))}.xml");
                                using (var fs = File.Open(file, FileMode.Create))
                                {
                                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                                    xmlSerializer.Serialize(fs, hudLayoutInfo);
                                }
                            }
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

            LogProvider.Log.Info("Migration #13 executed.");
        }

        private int GetHash(EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            unchecked
            {
                int hashCode = (int)2166136261;

                hashCode = (hashCode * 16777619) ^ pokerSite.GetHashCode();
                hashCode = (hashCode * 16777619) ^ gameType.GetHashCode();
                hashCode = (hashCode * 16777619) ^ tableType.GetHashCode();

                return hashCode;
            }
        }

        public override void Down()
        {
        }

    }
}