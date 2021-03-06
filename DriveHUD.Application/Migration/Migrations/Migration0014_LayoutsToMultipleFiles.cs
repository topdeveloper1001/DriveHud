﻿//-----------------------------------------------------------------------
// <copyright file="Migration0014_LayoutsToMultipleFiles.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.MigrationService.Migrators;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Model.Filters;
using Model.Hud;
using Model.Settings;
using Model.Stats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(14)]
    public class Migration0014_LayoutsToMultipleFiles : Migration
    {
        private readonly List<TableDescription> hashTable;

        private List<HudLayoutInfo> defaultLayouts;

        private readonly HudLayoutsMigrationService hudLayoutsService;

        private readonly ISettingsService settingsService;

        public Migration0014_LayoutsToMultipleFiles()
        {
            hudLayoutsService = new HudLayoutsMigrationService();
            settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();

            hashTable = GetHashTable();
        }

        private List<HudLayoutInfo> GetDefaultLayouts()
        {
            var result = new List<HudLayoutInfo>();

            foreach (var tableType in Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>())
            {
                result.AddRange(hudLayoutsService.GetAllLayouts(tableType));
            }

            return result;
        }

        private static HudSavedLayouts LoadOldLayouts(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            using (var fs = File.Open(fileName, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));

                var hudLayouts = xmlSerializer.Deserialize(fs) as HudSavedLayouts;

                return hudLayouts;
            }
        }

        private TableDescription GetTableDescription(int hash)
        {
            return hashTable.FirstOrDefault(h => h.Hash == hash);
        }

        private HudLayoutInfo GetHudLayoutInfo(List<HudSavedLayout> layouts, HudViewType hudViewType)
        {
            var masterLayout = layouts.FirstOrDefault();

            if (masterLayout == null)
            {
                return null;
            }

            var tableDescription = GetTableDescription(masterLayout.LayoutId);

            var sameDefault =
                defaultLayouts.FirstOrDefault(
                    l => l.TableType == tableDescription.TableType && l.HudViewType == hudViewType);

            if (settingsService.GetSettings().GeneralSettings.HudViewMode != (int)hudViewType
                && hudViewType != HudViewType.Plain)
                return null;

            var filteredLayouts = hudViewType == HudViewType.Plain
                                      ? layouts.Where(
                                              l =>
                                                  GetTableDescription(l.LayoutId).PokerSite != EnumPokerSites.Ignition
                                                  && GetTableDescription(l.LayoutId).PokerSite != EnumPokerSites.Bodog)
                                          .ToList()
                                      : layouts.Where(
                                              l =>
                                                  GetTableDescription(l.LayoutId).PokerSite == EnumPokerSites.Ignition
                                                  || GetTableDescription(l.LayoutId).PokerSite == EnumPokerSites.Bodog)
                                          .ToList();

            if (filteredLayouts.Count == 0)
                return null;

            var newLayout = new HudLayoutInfo();
            newLayout.TableType = tableDescription.TableType;
            newLayout.HudBumperStickerTypes = masterLayout.HudBumperStickerTypes.Select(s => s.Clone()).ToList();
            newLayout.HudPlayerTypes = masterLayout.HudPlayerTypes.Select(p => p.Clone()).ToList();

            newLayout.HudStats = masterLayout.HudStats.Select(s =>
            {
                var statInfoBreak = s as StatInfoBreak;

                if (statInfoBreak != null)
                {
                    return statInfoBreak.Clone();
                }

                return s.Clone();
            }).ToList();

            newLayout.IsDefault = false;
            newLayout.HudViewType = hudViewType;
            newLayout.UiPositionsInfo = sameDefault?.UiPositionsInfo.Select(p => p.Clone()).ToList();
            newLayout.Name = masterLayout.Name;

            foreach (var hudSavedLayout in filteredLayouts)
            {
                var newPositionInfo = new HudPositionsInfo();

                newPositionInfo.GameType = GetTableDescription(hudSavedLayout.LayoutId).GameType;
                newPositionInfo.PokerSite = GetTableDescription(hudSavedLayout.LayoutId).PokerSite;

                newPositionInfo.HudPositions =
                    hudSavedLayout.HudPositions.Where(
                            p => p.HudType == (hudViewType == HudViewType.Plain ? HudType.Plain : HudType.Default))
                        .Select(
                            p => new HudPositionInfo { Position = new Point(p.Position.X, p.Position.Y), Seat = p.Seat })
                        .ToList();

                if (newPositionInfo.HudPositions.Any())
                    newLayout.HudPositionsInfo.Add(newPositionInfo);
            }

            if (sameDefault != null)
            {
                foreach (var defaultPosInfo in sameDefault.HudPositionsInfo)
                {
                    if (
                        newLayout.HudPositionsInfo.Any(
                            p => p.GameType == defaultPosInfo.GameType && p.PokerSite == defaultPosInfo.PokerSite)) continue;
                    newLayout.HudPositionsInfo.Add(defaultPosInfo.Clone());
                }
            }

            if (HudObjectsComparer.AreEquals(newLayout, sameDefault))
                return null;

            if (HudObjectsComparer.AreEqualsExceptPositions(newLayout, sameDefault))
            {
                sameDefault.HudPositionsInfo = newLayout.HudPositionsInfo.Select(p => p.Clone()).ToList();
                return sameDefault;
            }

            return newLayout.HudPositionsInfo.Any() ? newLayout : null;
        }

        private string GetLayoutFileName(string layoutName)
        {
            return
                $"{Path.GetInvalidFileNameChars().Aggregate(layoutName, (current, c) => current.Replace(c.ToString(), string.Empty))}.xml";
        }

        private string Save(HudLayoutInfo newLayout)
        {
            var layoutsFile = Path.Combine(StringFormatter.GetAppDataFolderPath(), "Layouts", GetLayoutFileName(newLayout.Name));

            using (var fs = File.Open(layoutsFile, FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                xmlSerializer.Serialize(fs, newLayout);
            }

            return layoutsFile;
        }

        public override void Up()
        {
            try
            {
                LogProvider.Log.Info("Preparing migration #14");

                defaultLayouts = GetDefaultLayouts();

                var oldLayouts = LoadOldLayouts(Path.Combine(StringFormatter.GetAppDataFolderPath(), "Layouts.xml"));

                if (oldLayouts == null)
                {
                    return;
                }

                while (oldLayouts.Layouts.Count > 0)
                {
                    var currentLayout = oldLayouts.Layouts[0];

                    var tableDescription = hashTable.FirstOrDefault(h => h.Hash == currentLayout.LayoutId);

                    if (tableDescription == null)
                    {
                        oldLayouts.Layouts.RemoveAt(0);
                        continue;
                    }

                    var grouppedLayouts =
                        oldLayouts.Layouts.Where(
                            l =>
                                GetTableDescription(l.LayoutId).TableType == tableDescription?.TableType
                                && l.HudStats.Any()
                                && HudObjectsComparer.AreEquals(
                                    l.HudBumperStickerTypes,
                                    currentLayout.HudBumperStickerTypes)
                                && HudObjectsComparer.AreEquals(l.HudPlayerTypes, currentLayout.HudPlayerTypes)
                                && HudObjectsComparer.AreEquals(l.HudStats, currentLayout.HudStats)).ToList();

                    foreach (var hudViewType in Enum.GetValues(typeof(HudViewType)).OfType<HudViewType>())
                    {
                        var newLayout = GetHudLayoutInfo(grouppedLayouts, hudViewType);

                        if (newLayout == null)
                        {
                            continue;
                        }

                        if (!newLayout.IsDefault)
                        {
                            var i = 1;

                            string hudTypeName = string.Empty;

                            if (hudViewType != HudViewType.Plain)
                            {
                                hudTypeName = $" {hudViewType}";
                            }

                            string tableTypeName = string.Empty;

                            if (!newLayout.Name.Contains(CommonResourceManager.Instance.GetEnumResource(tableDescription.TableType)))
                            {
                                tableTypeName = $" {CommonResourceManager.Instance.GetEnumResource(tableDescription.TableType)}";
                            }

                            var layoutName = $"{newLayout.Name}{tableTypeName}{hudTypeName}";

                            while (hudLayoutsService.HudLayoutMappings.Mappings.Any(f => f.Name == layoutName))
                            {
                                layoutName = $"{newLayout.Name}{tableTypeName}{hudTypeName} {i}";
                                i++;
                            }

                            newLayout.Name = layoutName;
                        }
                        else
                        {
                            var def = defaultLayouts.FirstOrDefault(l => l.Name == newLayout.Name);
                            if (def != null)
                            {
                                defaultLayouts[defaultLayouts.IndexOf(def)] = newLayout;
                            }
                        }

                        var layoutFileName = Save(newLayout);

                        foreach (var selected in grouppedLayouts)
                        {
                            var table = hashTable.FirstOrDefault(h => h.Hash == selected.LayoutId);

                            if (table == null)
                                continue;

                            var mapping = new HudLayoutMapping
                            {
                                FileName = Path.GetFileName(layoutFileName),
                                GameType = table.GameType,
                                TableType = table.TableType,
                                PokerSite = table.PokerSite,
                                IsDefault = false,
                                IsSelected = selected.IsDefault,
                                Name = newLayout.Name,
                                HudViewType = newLayout.HudViewType
                            };

                            if (mapping.IsSelected
                                && hudLayoutsService.HudLayoutMappings.Mappings.Any(
                                    m =>
                                        m.IsSelected && m.PokerSite == table.PokerSite && m.TableType == table.TableType
                                        && m.GameType == table.GameType))
                                mapping.IsSelected = false;

                            hudLayoutsService.HudLayoutMappings.Mappings.Add(mapping);
                        }
                    }

                    oldLayouts.Layouts.RemoveAll(
                        l =>
                            GetTableDescription(l.LayoutId).TableType == tableDescription.TableType
                            && HudObjectsComparer.AreEquals(l.HudBumperStickerTypes, currentLayout.HudBumperStickerTypes)
                            && HudObjectsComparer.AreEquals(l.HudPlayerTypes, currentLayout.HudPlayerTypes)
                            && HudObjectsComparer.AreEquals(l.HudStats, currentLayout.HudStats));
                }

                hudLayoutsService.SaveLayoutMappings();

                LogProvider.Log.Info("Migration #14 executed.");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Migration #14 failed.", e);
            }
        }

        public override void Down()
        {
        }

        private List<TableDescription> GetHashTable()
        {
            return (from tableType in Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>()
                    from pokerSite in Enum.GetValues(typeof(EnumPokerSites)).OfType<EnumPokerSites>()
                    from gameType in Enum.GetValues(typeof(EnumGameType)).OfType<EnumGameType>()
                    select new TableDescription(GetHash(pokerSite, gameType, tableType), tableType, pokerSite, gameType))
                .ToList();
        }

        public int GetHash(EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            unchecked
            {
                var hashCode = (int)2166136261;

                hashCode = (hashCode * 16777619) ^ pokerSite.GetHashCode();
                hashCode = (hashCode * 16777619) ^ gameType.GetHashCode();
                hashCode = (hashCode * 16777619) ^ tableType.GetHashCode();

                return hashCode;
            }
        }

        private static class HudObjectsComparer
        {
            private static bool AreEquals(
                IEnumerable<StatInfoOptionValueRange> first,
                IEnumerable<StatInfoOptionValueRange> second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;

                var firstArray = first as StatInfoOptionValueRange[] ?? first.ToArray();
                var secondArray = second as StatInfoOptionValueRange[] ?? second.ToArray();

                if (firstArray.Length != secondArray.Length) return false;

                return
                    firstArray.All(
                        f =>
                            secondArray.FirstOrDefault(
                                s =>
                                    f.IsEditable == s.IsEditable && f.IsChecked == s.IsChecked && f.SortOrder == s.SortOrder
                                    && f.Value == s.Value && f.Value_IsValid == s.Value_IsValid
                                    && f.ValueRangeType == s.ValueRangeType && f.Color.Equals(s.Color)) != null);
            }

            private static bool AreEquals(StatInfoGroup first, StatInfoGroup second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;

                return first.Name == second.Name;
            }

            public static bool AreEquals(List<HudPlayerType> first, List<HudPlayerType> second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;
                if (first.Count != second.Count) return false;

                return
                    first.Select(
                        f =>
                            second.FirstOrDefault(
                                s =>
                                    f.Name == s.Name && f.ImageAlias == s.ImageAlias
                                    && f.EnablePlayerProfile == s.EnablePlayerProfile
                                    && f.DisplayPlayerIcon == s.DisplayPlayerIcon && f.MinSample == s.MinSample
                                    && AreEquals(f.Stats.Select(st => st), s.Stats))).All(ss => ss != null);
            }

            private static bool AreEquals(IEnumerable<HudPlayerTypeStat> first, IEnumerable<HudPlayerTypeStat> second)
            {
                if (first == null && second == null)
                    return true;

                if (first == null || second == null)
                    return false;

                var secondArray = second as HudPlayerTypeStat[] ?? second.ToArray();
                var firstArray = first as HudPlayerTypeStat[] ?? first.ToArray();
                if (firstArray.Length != secondArray.Length)
                    return false;
                return
                    firstArray.All(
                        hudPlayerTypeStat =>
                            secondArray.FirstOrDefault(
                                s =>
                                    s.High == hudPlayerTypeStat.High && s.Low == hudPlayerTypeStat.Low
                                    && s.Stat == hudPlayerTypeStat.Stat) != null);
            }

            public static bool AreEquals(List<HudBumperStickerType> first, List<HudBumperStickerType> second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;
                if (first.Count != second.Count) return false;

                for (var i = 0; i < first.Count; i++)
                {
                    if (!AreEquals(first[i], second[i])) return false;
                }
                return true;
            }

            private static bool AreEquals(HudBumperStickerType first, HudBumperStickerType second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;

                if (first.Description == "3-Bets too much, and folds to a 4-bet too often.") first.Description = "3-Bets too much, and folds to a 3-bet too often.";
                if (second.Description == "3-Bets too much, and folds to a 4-bet too often.") second.Description = "3-Bets too much, and folds to a 3-bet too often.";
                if (first.Description == "Open raises to wide of a range in early pre-flop positions.") first.Description = "Open raises too wide of a range in early pre-flop positions.";
                if (second.Description == "Open raises to wide of a range in early pre-flop positions.") second.Description = "Open raises too wide of a range in early pre-flop positions.";
                if (first.Name != second.Name || first.Label != second.Label || first.Description != second.Description
                    || first.ToolTip != second.ToolTip || first.EnableBumperSticker != second.EnableBumperSticker
                    || first.MinSample != second.MinSample || first.SelectedColor != second.SelectedColor) return false;

                return AreEquals(first.Stats.OfType<HudPlayerTypeStat>(), second.Stats.OfType<HudPlayerTypeStat>())
                       && AreEquals(first.FilterModelCollection, second.FilterModelCollection);
            }

            private static bool AreEquals(IFilterModelCollection first, IFilterModelCollection second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;
                if (first.Count != second.Count) return false;

                return first.All(f => second.FirstOrDefault(s => f.Type == s.Type) != null);
            }

            public static bool AreEquals(List<StatInfo> first, List<StatInfo> second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;
                if (first.Count != second.Count) return false;

                for (var i = 0; i < first.Count; i++)
                {
                    if (!AreEquals(first[i], second[i])) return false;
                }

                return true;
            }

            private static bool AreEquals(StatInfo first, StatInfo second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;

                if (first.Caption != second.Caption) return false;
                if (first.Stat != second.Stat) return false;
                if (!AreEquals(first.StatInfoGroup, second.StatInfoGroup)) return false;
                if (first.GroupName != second.GroupName) return false;                
                if (first.CurrentColor != second.CurrentColor) return false;
                if (first.SettingsAppearance_IsChecked != second.SettingsAppearance_IsChecked) return false;
                if (first.SettingsPlayerType_IsChecked != second.SettingsPlayerType_IsChecked) return false;
                if (first.SettingsAppearanceFontBold_IsChecked != second.SettingsAppearanceFontBold_IsChecked) return false;
                if (first.SettingsAppearanceFontSource != second.SettingsAppearanceFontSource) return false;
                if (first.SettingsAppearanceFontSize != second.SettingsAppearanceFontSize) return false;
                if (first.SettingsAppearanceFontBold != second.SettingsAppearanceFontBold) return false;
                if (first.SettingsAppearanceFontItalic_IsChecked != second.SettingsAppearanceFontItalic_IsChecked) return false;
                if (first.SettingsAppearanceFontItalic != second.SettingsAppearanceFontItalic) return false;
                if (first.SettingsAppearanceFontUnderline_IsChecked != second.SettingsAppearanceFontUnderline_IsChecked) return false;
                if (!AreEquals(first.SettingsAppearanceValueRangeCollection, second.SettingsAppearanceValueRangeCollection)) return false;
                return true;
            }

            private static bool AreEquals(List<HudPositionInfo> first, List<HudPositionInfo> second)
            {
                if (first == null && second == null) return true;
                if (first == null || second == null) return false;
                if (first.Count != second.Count) return false;

                foreach (var hudPositionInfo in first)
                {
                    var secondPosition = second.FirstOrDefault(x => x.Seat == hudPositionInfo.Seat);
                    if (secondPosition == null)
                        return false;
                    if (!secondPosition.Position.Equals(hudPositionInfo.Position))
                    {
                        var point1 = new Point(340, 133);
                        var point2 = new Point(347, 133);
                        if ((secondPosition.Position.Equals(point1) && hudPositionInfo.Position.Equals(point2)) || (secondPosition.Position.Equals(point2) && hudPositionInfo.Position.Equals(point1)))
                            continue;

                        point1 = new Point(340, 143);
                        point2 = new Point(347, 143);
                        if ((secondPosition.Position.Equals(point1) && hudPositionInfo.Position.Equals(point2)) || (secondPosition.Position.Equals(point2) && hudPositionInfo.Position.Equals(point1)))
                            continue;

                        point1 = new Point(666, 238);
                        point2 = new Point(667, 238);
                        if ((secondPosition.Position.Equals(point1) && hudPositionInfo.Position.Equals(point2)) || (secondPosition.Position.Equals(point2) && hudPositionInfo.Position.Equals(point1)))
                            continue;

                        point1 = new Point(666, 347);
                        point2 = new Point(667, 347);
                        if ((secondPosition.Position.Equals(point1) && hudPositionInfo.Position.Equals(point2)) || (secondPosition.Position.Equals(point2) && hudPositionInfo.Position.Equals(point1)))
                            continue;

                        return false;
                    }
                }

                return true;
            }

            public static bool AreEquals(HudLayoutInfo newLayout, HudLayoutInfo defaultLayout)
            {
                if (newLayout == null && defaultLayout == null) return true;
                if (newLayout == null || defaultLayout == null) return false;

                if (!AreEquals(newLayout.HudBumperStickerTypes, defaultLayout.HudBumperStickerTypes)) return false;
                if (!AreEquals(newLayout.HudPlayerTypes, defaultLayout.HudPlayerTypes)) return false;
                if (!AreEquals(newLayout.HudStats, defaultLayout.HudStats)) return false;
                foreach (var hudPositionsInfo in newLayout.HudPositionsInfo)
                {
                    var existiongPos =
                        defaultLayout.HudPositionsInfo.FirstOrDefault(
                            p => p.GameType == hudPositionsInfo.GameType && p.PokerSite == hudPositionsInfo.PokerSite);
                    if (existiongPos == null)
                        return false;

                    if (!AreEquals(existiongPos.HudPositions, hudPositionsInfo.HudPositions))
                        return false;
                }
                return true;
            }

            public static bool AreEqualsExceptPositions(HudLayoutInfo newLayout, HudLayoutInfo defaultLayout)
            {
                if (newLayout == null && defaultLayout == null) return true;
                if (newLayout == null || defaultLayout == null) return false;

                if (!AreEquals(newLayout.HudBumperStickerTypes, defaultLayout.HudBumperStickerTypes)) return false;
                if (!AreEquals(newLayout.HudPlayerTypes, defaultLayout.HudPlayerTypes)) return false;
                if (!AreEquals(newLayout.HudStats, defaultLayout.HudStats)) return false;
                return true;
            }
        }

        private class TableDescription
        {
            public int Hash { get; set; }
            public EnumTableType TableType { get; set; }
            public EnumPokerSites PokerSite { get; set; }
            public EnumGameType GameType { get; set; }

            public TableDescription(int hash, EnumTableType tableType, EnumPokerSites pokerSite, EnumGameType gameType)
            {
                Hash = hash;
                TableType = tableType;
                PokerSite = pokerSite;
                GameType = gameType;
            }
        }
    }
}