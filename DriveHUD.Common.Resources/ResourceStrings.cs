//-----------------------------------------------------------------------
// <copyright file="ResourceStrings.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Collections.Generic;

namespace DriveHUD.Common.Resources
{
    public static class ResourceStrings
    {
        public static string DataExportedMessageResourceString = "Common_DataExported";
        public static string ReplayerHeaderResourceString = "Main_ReplayerHeader";

        #region SystemSettings

        public static string ActivePlayerFileName = "SystemSettings_ActivePlayerFileName";
        public static string AppDataFolder = "SystemSettings_AppDataFolder";
        public static string AppStoreDataFolder = "SystemSettings_AppStoreDataFolder";
        public static string AppStoreLocalProductRepo = "SystemSettings_AppStoreLocalProductRepo";
        public static string AppStoreLocalTrainingRepo = "SystemSettings_AppStoreLocalTrainingProductRepo";
        public static string AppStoreLocalRakebackRepo = "SystemSettings_AppStoreLocalRakebackRepo";
        public static string AppStoreLocalTempProductRepo = "SystemSettings_AppStoreLocalTempProductRepo";
        public static string AppStoreLocalTempTrainingProductRepo = "SystemSettings_AppStoreLocalTempTrainingProductRepo";
        public static string AppStoreLocalTempRakebackRepo = "SystemSettings_AppStoreLocalTempRakebackRepo";
        public static string AppStoreRemoteProductRepo = "SystemSettings_AppStoreRemoteProductRepo";
        public static string AppStoreRemoteTrainingProductRepo = "SystemSettings_AppStoreRemoteTrainingProductRepo";
        public static string AppStoreRemoteRakebackRepo = "SystemSettings_AppStoreRemoteRakebackRepo";
        public static string AppStoreRemoteProductHash = "SystemSettings_AppStoreRemoteProductHash";
        public static string AppStoreRemoteTrainingProductHash = "SystemSettings_AppStoreRemoteTrainingProductHash";
        public static string AppStoreRemoteRakebackHash = "SystemSettings_AppStoreRemoteRakebackHash";
        public static string DefaultPlayersFolderName = "SystemSettings_DefaultPlayersFolderName";
        public static string DefaultPlayerStatisticFolderName = "SystemSettings_DefaultPlayerStatisticFolderName";
        public static string DefaultPlayerStatisticExtension = "SystemSettings_DefaultPlayerStatisticExtension";
        public static string PlayerStatisticTempFolderName = "SystemSettings_PlayerStatisticTempFolderName";
        public static string PlayerStatisticBackupFolderName = "SystemSettings_PlayerStatisticBackupFolderName";
        public static string PlayerStatisticOldFolderName = "SystemSettings_PlayerStatisticOldFolderName";
        public static string DbFileName = "SystemSettings_DbFileName";
        public static string HeroName = "SystemSettings_HeroName";
        public static string LayoutsFolder = "SystemSettings_LayoutsFolder";
        public static string LayoutsV2Folder = "SystemSettings_LayoutsV2Folder";
        public static string LayoutsExtension = "SystemSettings_LayoutsExtension";
        public static string LayoutsMappings = "SystemSettings_LayoutsMappings";
        public static string LogsFolder = "SystemSettings_LogsFolder";
        public static string ModulesFolder = "SystemSettings_ModulesFolder";
        public static string ImporterPipeAddress = "SystemSettings_ImporterPipeAddress";

        #endregion

        #region Context Menu

        public static string HandTagForReview = "Enum_HandTag_ForReview";
        public static string HandTagBluff = "Enum_HandTag_Bluff";
        public static string HandTagHeroCall = "Enum_HandTag_HeroCall";
        public static string HandTagBigFold = "Enum_HandTag_BigFold";
        public static string HandTagNone = "Enum_HandTag_None";
        public static string TagHand = "Main_ContextMenu_TagHand";

        public static string TwoPlustTwoResourceString = "Enum_HandExport_TwoPlusTwo";
        public static string CardsChatResourceString = "Enum_HandExport_CardsChat";
        public static string PokerStrategyString = "Enum_HandExport_PokerStrategy";
        public static string RawHandHistoryString = "Enum_HandExport_RawHandHistory";
        public static string ICMizerHandHistory = "Enum_HandExport_ICMizer";
        public static string ExportHandResourceString = "Main_ContextMenu_ExportHand";
        public static string PlainTextHandHistoryString = "Enum_HandExport_PlainTextHandHistory";

        public static string MakeNote = "Main_ContextMenu_MakeNote";
        public static string EditNote = "Main_ContextMenu_EditNote";
        public static string ReplayHandResourceString = "Main_ContextMenu_ReplayHand";
        public static string CalculateEquityResourceString = "Main_ContextMenu_CalculateEquity";
        public static string AllResourceString = "Main_ContextMenu_All";

        public static string EditTournamentResourceString = "Main_ContextMenu_EditTournament";
        public static string DeleteTournamentResourceString = "Main_ContextMenu_DeleteTournament";
        public static string RefreshReportResourceString = "Main_ContextMenu_RefreshReport";
        public static string ExportToExcelReportResourceString = "Main_ContextMenu_ExportToExcel";
        public static string DeleteHandResourceString = "Main_ContextMenu_DeleteHand";


        #endregion

        #region Help links

        public static Dictionary<EnumPokerSites, string> PokerSiteHelpLinks = new Dictionary<EnumPokerSites, string>
        {
            [EnumPokerSites.AmericasCardroom] = "Settings_AmericasCardroomHelpLink",
            [EnumPokerSites.BetOnline] = "Settings_BetOnlineHelpLink",
            [EnumPokerSites.BlackChipPoker] = "Settings_BlackChipPokerHelpLink",
            [EnumPokerSites.Bodog] = "Settings_BodogHelpLink",
            [EnumPokerSites.Ignition] = "Settings_BodogHelpLink",
            [EnumPokerSites.Bovada] = "Settings_BodogHelpLink",
            [EnumPokerSites.PartyPoker] = "Settings_PartyPokerHelpLink",
            [EnumPokerSites.Poker888] = "Settings_Poker888HelpLink",
            [EnumPokerSites.PokerStars] = "Settings_PokerStarsHelpLink",
            [EnumPokerSites.SportsBetting] = "Settings_SportsBettingHelpLink",
            [EnumPokerSites.TigerGaming] = "Settings_TigerGamingHelpLink",
            [EnumPokerSites.TruePoker] = "Settings_TruePokerHelpLink",
            [EnumPokerSites.WinningPokerNetwork] = "Settings_WinningPokerNetworkHelpLink",
            [EnumPokerSites.YaPoker] = "Settings_YaPokerHelpLink",
            [EnumPokerSites.IPoker] = "Settings_IPokerHelpLink",
            [EnumPokerSites.Horizon] = "Settings_HorizonHelpLink",
            [EnumPokerSites.Winamax] = "Settings_WinamaxHelpLink",
            [EnumPokerSites.Adda52] = "Settings_Adda52HelpLink",
        };

        #endregion
    }
}