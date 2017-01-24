using System;
using System.Collections.Generic;
using System.Linq;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.ViewModels;
using DriveHUD.Entities;
using FluentMigrator;
using Model.Filters;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(14)]
    public class Migration0014_LayoutsToMultipleFiles : Migration
    {
        private readonly List<TableDescription> hashTable;

        private List<HudLayoutInfo> defaultLayouts;

        private readonly IHudLayoutsService _hudLayoutsService;

        public Migration0014_LayoutsToMultipleFiles()
        {
            _hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            hashTable = GetHashTable();
        }

        private List<HudLayoutInfo> GetDefaultLayouts()
        {
            var result = new List<HudLayoutInfo>();
            foreach (var tableType in Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>())
            {
                result.AddRange(_hudLayoutsService.GetAllLayouts(tableType));
            }
            return result;
        }

        private static HudSavedLayouts LoadOldLayouts(string fileName)
        {
            if (!File.Exists(fileName)) return null;
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
            if (masterLayout == null) return null;

            var tableDescription = GetTableDescription(masterLayout.LayoutId);

            var sameDefault =
                defaultLayouts.FirstOrDefault(
                    l => l.TableType == tableDescription.TableType && l.HudViewType == hudViewType);



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

            if (filteredLayouts.Count == 0) return null;

            var newLayout = new HudLayoutInfo();
            newLayout.TableType = tableDescription.TableType;
            newLayout.HudBumperStickerTypes = masterLayout.HudBumperStickerTypes.Select(s => s.Clone()).ToList();
            newLayout.HudPlayerTypes = masterLayout.HudPlayerTypes.Select(p => p.Clone()).ToList();
            newLayout.HudStats = masterLayout.HudStats.Select(s => s.Clone()).ToList();
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
                if (newPositionInfo.HudPositions.Any()) newLayout.HudPositionsInfo.Add(newPositionInfo);
            }

            if (HudObjectsComparer.AreEquals(newLayout, sameDefault)) return null;

            return newLayout.HudPositionsInfo.Any() ? newLayout : null;
        }

        private static string GetLayoutFileName(string layoutName)
        {
            return
                $"{Path.GetInvalidFileNameChars().Aggregate(layoutName, (current, c) => current.Replace(c.ToString(), string.Empty))}.xml";
        }

        private static string Save(HudLayoutInfo newLayout)
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
            defaultLayouts = GetDefaultLayouts();

            var oldLayouts = LoadOldLayouts(Path.Combine(StringFormatter.GetAppDataFolderPath(), "Layouts.xml"));

            while (oldLayouts.Layouts.Count > 0)
            {
                var currentLayout = oldLayouts.Layouts[0];

                var tableDescription = hashTable.FirstOrDefault(h => h.Hash == currentLayout.LayoutId);

                var sameLayouts =
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
                    var newLayout = GetHudLayoutInfo(sameLayouts, hudViewType);
                    if (newLayout == null) continue;

                    var i = 1;
                    var layoutName = newLayout.Name;
                    while (_hudLayoutsService.HudLayoutMappings.Mappings.Any(f => f.Name == layoutName))
                    {
                        layoutName = $"{newLayout.Name} {i}";
                        i++;
                    }
                    newLayout.Name = layoutName;

                    var layoutFileName = Save(newLayout);

                    foreach (var selected in sameLayouts)
                    {
                        var table = hashTable.FirstOrDefault(h => h.Hash == selected.LayoutId);

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
                            && _hudLayoutsService.HudLayoutMappings.Mappings.Any(
                                m =>
                                    m.PokerSite == table.PokerSite && m.TableType == table.TableType
                                    && m.GameType == table.GameType)) mapping.IsSelected = false;

                        _hudLayoutsService.HudLayoutMappings.Mappings.Add(mapping);
                    }
                }
                oldLayouts.Layouts.RemoveAll(
                    l =>
                        GetTableDescription(l.LayoutId).TableType == tableDescription?.TableType
                        && HudObjectsComparer.AreEquals(l.HudBumperStickerTypes, currentLayout.HudBumperStickerTypes)
                        && HudObjectsComparer.AreEquals(l.HudPlayerTypes, currentLayout.HudPlayerTypes)
                        && HudObjectsComparer.AreEquals(l.HudStats, currentLayout.HudStats));
            }
            _hudLayoutsService.SaveLayoutMappings();
        }

        public override void Down()
        {
            throw new System.NotImplementedException();
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
    }


    public static class HudObjectsComparer
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
            return first.Name == second.Name;
        }

        public static bool AreEquals(List<HudPlayerType> first, List<HudPlayerType> second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;
            if (first.Count != second.Count) return false;

            for (var i = 0; i < first.Count; i++)
            {
                var f = first[i];
                var ss =
                    second.FirstOrDefault(
                        s =>
                            f.Name == s.Name && f.ImageAlias == s.ImageAlias
                            && f.EnablePlayerProfile == s.EnablePlayerProfile
                            && f.DisplayPlayerIcon == s.DisplayPlayerIcon && f.MinSample == s.MinSample
                            && AreEquals(f.Stats.Select(st => st), s.Stats));
                if (ss == null) return false;
            }

            return true;
        }

        private static bool AreEquals(IEnumerable<HudPlayerTypeStat> first, IEnumerable<HudPlayerTypeStat> second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;
            var secondArray = second as HudPlayerTypeStat[] ?? second.ToArray();
            var firstArray = first as HudPlayerTypeStat[] ?? first.ToArray();
            if (firstArray.Count() != secondArray.Count()) return false;
            foreach (var hudPlayerTypeStat in firstArray)
            {
                if (
                    secondArray.FirstOrDefault(
                        s =>
                            s.High == hudPlayerTypeStat.High && s.Low == hudPlayerTypeStat.Low
                            && s.Stat == hudPlayerTypeStat.Stat) == null) return false;
            }
            return true;
        }

        public static bool AreEquals(List<HudBumperStickerType> first, List<HudBumperStickerType> second)
        {
            if (first.Count != second.Count) return false;
            for (var i = 0; i < first.Count; i++)
            {
                if (!AreEquals(first[i], second[i])) return false;
            }
            return true;
        }

        private static bool AreEquals(HudBumperStickerType first, HudBumperStickerType second)
        {
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
            if (first.Caption != second.Caption) return false;
            if (first.Stat != second.Stat) return false;
            if (!AreEquals(first.StatInfoGroup, second.StatInfoGroup)) return false;
            if (first.GroupName != second.GroupName) return false;
            if (first.PropertyName != second.PropertyName) return false;
            if (first.CurrentColor != second.CurrentColor) return false;
            if (first.Settings_IsAvailable != second.Settings_IsAvailable) return false;
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
                if (secondPosition == null) return false;
                if (!secondPosition.Position.Equals(hudPositionInfo.Position)) return false;
            }

            return true;
        }

        public static bool AreEquals(HudLayoutInfo newLayout, HudLayoutInfo defaultLayout)
        {
            if (!AreEquals(newLayout.HudBumperStickerTypes, defaultLayout.HudBumperStickerTypes)) return false;
            if (!AreEquals(newLayout.HudPlayerTypes, defaultLayout.HudPlayerTypes)) return false;
            if (!AreEquals(newLayout.HudStats, defaultLayout.HudStats)) return false;
            foreach (var hudPositionsInfo in newLayout.HudPositionsInfo)
            {
                var existiongPos =
                    defaultLayout.HudPositionsInfo.FirstOrDefault(
                        p => p.GameType == hudPositionsInfo.GameType && p.PokerSite == hudPositionsInfo.PokerSite);
                if (existiongPos == null) return false;
                if (!AreEquals(existiongPos.HudPositions, hudPositionsInfo.HudPositions)) return false;
            }
            return true;
        }
    }

    public class TableDescription
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