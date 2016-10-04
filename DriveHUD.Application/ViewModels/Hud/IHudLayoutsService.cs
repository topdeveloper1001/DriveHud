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

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Operates with HUD layouts
    /// </summary>
    internal interface IHudLayoutsService
    {
        /// <summary>
        /// Cached layouts
        /// </summary>
        HudSavedLayouts Layouts { get; }

        /// <summary>
        /// Get layout by name
        /// </summary>
        /// <param name="name">Name of layout</param>
        /// <param name="layoutId">Layout id</param>
        /// <returns>Layout</returns>
        HudSavedLayout GetLayoutByName(string name, int layoutId);

        /// <summary>
        /// Set active layout
        /// </summary>
        /// <param name="layout">Layout</param>
        void SetLayoutActive(HudSavedLayout layout);

        /// <summary>
        /// Get active layout
        /// </summary>
        /// <param name="layoutId">Layout key</param>
        /// <returns>Active layout</returns>
        HudSavedLayout GetActiveLayout(int layoutId);

        /// <summary>
        /// Save current layout
        /// </summary>
        void Save(HudSavedDataInfo hudData);

        /// <summary>
        /// Save current layout
        /// </summary>
        void Save(HudSavedLayout hudData);

        /// <summary>
        /// Save only not existing items as defaults
        /// </summary>
        /// <param name="hudTables">Hud tables</param>
        void SaveDefaults(Dictionary<int, HudTableViewModel> hudTables);

        /// <summary>
        /// Save new layout
        /// </summary>
        HudSavedLayout SaveAs(HudSavedDataInfo hudData);

        /// <summary>
        /// Load layout
        /// </summary>
        HudSavedLayout Load(string name, int layoutId);

        /// <summary>
        /// Delete layout
        /// </summary>
        /// <param name="layout">Layout to delete</param>
        bool Delete(HudSavedLayout layout, out HudSavedLayout activeLayout);

        /// <summary>
        /// Export layout
        /// </summary>
        /// <param name="hudData">Data to be exported in HUD layout format</param>
        /// <param name="path">Path to file</param>
        void Export(HudSavedDataInfo hudData, string path);

        /// <summary>
        /// Import layout
        /// </summary>
        /// <param name="path">Path to layout</param>
        HudSavedLayout Import(string path);

        /// <summary>
        /// Set icons for hud elements based on stats and layout player type settings
        /// </summary>
        /// <param name="hudElements">Hud elements</param>
        /// <param name="layoutId">Layout</param>
        void SetPlayerTypeIcon(IEnumerable<HudElementViewModel> hudElements, int layoutId);

        /// <summary>
        /// Set stickers for hud elements based on stats and bumper sticker settings
        /// </summary>
        /// <param name="hudElements">Hud elements</param>
        /// <param name="layoutId">Layout</param>
        void SetStickers(IEnumerable<HudElementViewModel> hudElements, int layoutId);

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

        /// <summary>
        /// Get collection of layouts that have any stats selected
        /// </summary>
        /// <returns></returns>
        IEnumerable<HudSavedLayout> GetNotEmptyStatsLayout();
    }
}