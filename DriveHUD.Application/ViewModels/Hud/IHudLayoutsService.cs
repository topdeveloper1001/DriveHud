//-----------------------------------------------------------------------
// <copyright file="HudStatSettingsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Entities;
using Model.Enums;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Operates with HUD layouts
    /// </summary>
    internal interface IHudLayoutsService
    {
        List<HudTableViewModel> HudTableViewModels { get; set; }

        HudLayoutMappings HudLayoutMappings { get; set; }

        HudLayoutInfo GetActiveLayout(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType);

        HudTableViewModel GetHudTableViewModel(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType);

        HudLayoutInfo GetLayout(string name);

        /// <summary>
        /// Save new layout
        /// </summary>
        HudLayoutInfo SaveAs(HudSavedDataInfo hudData);

        /// <summary>
        /// Delete layout
        /// </summary>
        /// <param name="layout">Layout to delete</param>
        bool Delete(string layoutName);

        /// <summary>
        /// Export layout
        /// </summary>
        void Export(HudLayoutInfo layout, string path);

        /// <summary>
        /// Import layout
        /// </summary>
        /// <param name="path">Path to layout</param>
        HudLayoutInfo Import(string path);

        /// <summary>
        /// Set icons for hud elements based on stats and layout player type settings
        /// </summary>
        void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, string layoutName);

        /// <summary>
        /// Checks if specified hand is valid for sticker calculations
        /// </summary>
        /// <returns>List of sticker names for which specified statistic is valid</returns>
        IList<string> GetValidStickers(Playerstatistic statistic, string layoutName);

        /// <summary>
        /// Set stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        void SetStickers(HudElementViewModel hudElement, IDictionary<string, Playerstatistic> stickersStatistics,
            string layoutName);

        /// <summary>
        /// Save bumper stickers for layout specified
        /// </summary>
        //void SaveBumperStickers(HudLayoutInfo hudLayout);

        /// <summary>
        /// Get path to image directory
        /// </summary>
        /// <returns>Path to image directory</returns>
        string GetImageDirectory();

        /// <summary>
        /// Get link to image
        /// </summary>
        /// <param name="image">Image alias</param>
        /// <returns>Full path to image</returns>
        string GetImageLink(string image);

        void SetLayoutActive(HudLayoutInfo hudToLoad, short pokerSiteId, short gameType, short tableType);

        IEnumerable<string> GetLayoutsNames(EnumTableType tableType);

        IEnumerable<string> GetAvailableLayouts(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType);

        List<HudLayoutInfo> GetAllLayouts(EnumTableType tableType);
    }
}