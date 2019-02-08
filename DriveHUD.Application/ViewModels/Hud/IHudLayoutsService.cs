//-----------------------------------------------------------------------
// <copyright file="IHudLayoutsService.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using DriveHUD.HUD.Service;
using Model.Data;
using Model.Hud;
using System.Collections.Generic;
using System.IO;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Defines the service for initializing, loading, deleting layouts of the hud
    /// </summary>
    internal interface IHudLayoutsService
    {
        /// <summary>
        /// Gets or sets <see cref="HudLayoutMappings"/> the mappings of the layouts
        /// </summary>
        HudLayoutMappings HudLayoutMappings { get; set; }

        /// <summary>
        /// Gets active <see cref="HudLayoutInfoV2"/> layout for specified <see cref="EnumPokerSites"/> poker site, <see cref="EnumTableType"/> table type and <see cref="EnumGameType"/> game type
        /// </summary>
        /// <param name="pokerSite">Poker site</param>
        /// <param name="tableType">Type of table</param>
        /// <param name="gameType">Type of game</param>
        /// <returns>Active layout</returns>
        HudLayoutInfoV2 GetActiveLayout(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType);

        /// <summary>
        /// Gets layout with the specified name
        /// </summary>
        /// <param name="name">Name of layout to get</param>
        /// <returns>Layout</returns>
        HudLayoutInfoV2 GetLayout(string name);

        /// <summary>
        /// Saves the specified layout on the default path
        /// </summary>
        /// <param name="hudLayout">The layout to save</param>
        void Save(HudLayoutInfoV2 hudLayout);

        /// <summary>
        /// Updates existing HUD using data from the specified <see cref="HudLayoutContract"/>
        /// </summary>
        /// <param name="hudLayoutContract">Contrat to update existing HUD</param>
        void Save(HudLayoutContract hudLayoutContract);

        /// <summary>
        /// Saves layout based on the specified <see cref="HudSavedDataInfo"/> data
        /// </summary>
        /// <param name="hudData">Data to save layout</param>
        HudLayoutInfoV2 SaveAs(HudSavedDataInfo hudData);

        /// <summary>
        /// Deletes the specified layout
        /// </summary>
        /// <param name="layoutName">Name of the layout to delete</param>
        /// <returns>True if the layout is deleted, otherwise - false</returns>
        bool Delete(string layoutName);

        /// <summary>
        /// Exports <see cref="HudLayoutInfoV2"/> the layout to the specified path
        /// </summary>
        /// <param name="layout">Layout to export</param>
        /// <param name="path">Path to file</param>
        void Export(HudLayoutInfoV2 layout, string path);

        /// <summary>
        /// Imports <see cref="HudLayoutInfoV2"/> layout on the specified path
        /// </summary>
        /// <param name="path">Path to layout</param>
        HudLayoutInfoV2 Import(string path);

        /// <summary>
        /// Imports <see cref="HudLayoutInfoV2"/> layout from the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        HudLayoutInfoV2 Import(Stream stream);

        /// <summary>
        /// Imports <see cref="HudLayoutInfoV2"/> layout from the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="layoutId">Id of layout</param>
        HudLayoutInfoV2 Import(Stream stream, int layoutId);

        /// <summary>
        /// Exports <see cref="IEnumerable{HudPlayerType}"/> to the specified path
        /// </summary>
        /// <param name="path">Path to file</param>
        void ExportPlayerType(HudPlayerType[] playerTypes, string path);

        /// <summary>
        /// Exports <see cref="IEnumerable{BumperStickerType}"/> to the specified path
        /// </summary>
        /// <param name="path">Path to file</param>
        void ExportBumperStickerType(HudBumperStickerType[] bumperStickerTyps, string path);

        /// <summary>
        /// Imports <see cref="HudPlayerType"/> on the specified path
        /// </summary>
        /// <param name="path">Path to player type</param>
        HudPlayerType[] ImportPlayerType(string path);

        /// <summary>
        /// Imports <see cref="HudBumperStickerType"/> on the specified path
        /// </summary>
        /// <param name="path">Path to bumper sticker type</param>
        HudBumperStickerType[] ImportBumperStickerType(string path);

        /// <summary>
        /// Sets icons for hud elements based on stats and layout player type settings
        /// </summary>
        void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, HudLayoutInfoV2 layout);

        /// <summary>
        /// Checks if specified hand is valid for sticker calculations
        /// </summary>
        /// <returns>List of sticker names for which specified statistic is valid</returns>
        IList<string> GetValidStickers(Playerstatistic statistic, HudLayoutInfoV2 layout);

        /// <summary>
        /// Sets stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        void SetStickers(HudElementViewModel hudElement, IDictionary<string, HudStickerIndicators> stickersStatistics, HudLayoutInfoV2 layout);

        /// <summary>
        /// Sets active layout for the specified <see cref="EnumPokerSites"/> poker site, <see cref="EnumGameType"/> game type and <see cref="EnumTableType"/> table type
        /// </summary>
        /// <param name="hudToLoad">Layout to be set as active</param>
        /// <param name="pokerSite">Poker site to set active layout</param>
        /// <param name="gameType">Game type to set active layout</param>
        /// <param name="tableType">Table type to set active layout</param>
        void SetActiveLayout(HudLayoutInfoV2 hudToLoad, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType);

        /// <summary>
        /// Gets layouts names for specified <see cref="EnumTableType"/>  table type
        /// </summary>
        /// <param name="tableType">Type of table</param>
        /// <returns>Collection of names</returns>
        IEnumerable<string> GetLayoutsNames(EnumTableType tableType);

        /// <summary>
        /// Gets the names of available layouts for specified <see cref="EnumPokerSites"/> poker site, <see cref="EnumTableType"/> table type and <see cref="EnumGameType"/> game type
        /// </summary>
        /// <param name="tableType">Type of table</param>        
        IEnumerable<string> GetAvailableLayouts(EnumTableType tableType);

        /// <summary>
        /// Gets the sorted list of <see cref="HudLayoutInfoV2"/> layouts for the specified <see cref="EnumTableType"/> table type
        /// </summary>
        /// <param name="tableType">Type of table</param>        
        List<HudLayoutInfoV2> GetAllLayouts(EnumTableType tableType);

        /// <summary>
        /// Saves the mapping of the layout to the file on the default path
        /// </summary>
        void SaveLayoutMappings();

        /// <summary>
        /// Gets the path to the directory with layouts
        /// </summary>
        /// <returns>Directory</returns>
        DirectoryInfo GetLayoutsDirectory();

        /// <summary>
        /// Duplicates the specified <see cref="HudLayoutInfoV2" />
        /// </summary>
        /// <param name="tableType">Table type of the duplicated layout</param>
        /// <param name="layoutName">Name of the duplicated layout</param>
        /// <param name="layoutToDuplicate">Layout to duplicate</param>
        /// <returns>The duplicated layout</returns>
        HudLayoutInfoV2 DuplicateLayout(EnumTableType tableType, string layoutName, HudLayoutInfoV2 layoutToDuplicate);
    }
}