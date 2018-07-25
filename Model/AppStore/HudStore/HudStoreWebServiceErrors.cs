//-----------------------------------------------------------------------
// <copyright file="HudStoreWebServiceErrors.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.AppStore.HudStore
{
    public static class HudStoreWebServiceErrors
    {
        public static ReadOnlyDictionary<string, string> Codes = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            ["DbGeneralFailure"] = "Common_HudWebService_DbGeneralFailure",
            ["BadRequest"] = "Common_HudWebService_BadRequest",
            ["InvalidData"] = "Common_HudWebService_InvalidData",
            ["NameMustBeNotEmpty"] = "Common_HudUploadToStoreView_NameMustBeNotEmpty",
            ["NameMustBeLongerThan10"] = "Common_HudUploadToStoreView_NameMustBeLongerThan10",
            ["NameIsAlreadyInUse"] = "Common_HudUploadToStoreView_NameIsAlreadyInUse",
            ["DescriptionMustBeNotEmpty"] = "Common_HudUploadToStoreView_DescriptionMustBeNotEmpty",
            ["CostFormatIsInvalid"] = "Common_HudUploadToStoreView_CostFormatIsInvalid",
            ["CostMustBeNotNegative"] = "Common_HudUploadToStoreView_CostMustBeNotNegative",
            ["GameVariantsMustBeSet"] = "Common_HudUploadToStoreView_GameVariantMustBeSelected",
            ["InvalidGameVariant"] = "Common_HudWebService_InvalidGameVariant",
            ["GameTypesMustBeSet"] = "Common_HudUploadToStoreView_GameTypeMustBeSelected",
            ["InvalidGameType"] = "Common_HudWebService_InvalidGameType",
            ["TableTypesMustBeSet"] = "Common_HudUploadToStoreView_TableTypeMustBeSelected",
            ["InvalidTableType"] = "Common_HudWebService_InvalidTableType",
            ["ImagesMustBeSet"] = "Common_HudWebService_ImagesMustBeSet",
            ["ImagesFilesMustBeSet"] = "Common_HudWebService_ImagesFilesMustBeSet",
            ["CaptionMustBeSet"] = "Common_HudWebService_CaptionMustBeSet",
            ["ImagesMustMatchImagesFiles"] = "Common_HudWebService_ImagesMustMatchImagesFiles",
            ["ImageFileMustBeImage"] = "Common_HudWebService_ImageFileMustBeImage",
            ["ImageFileIsTooLarge"] = "Common_HudWebService_ImageFileIsTooLarge",
            ["HudInsertFailed"] = "Common_HudWebService_HudInsertFailed",
            ["GameVariantsInsertFailed"] = "Common_HudWebService_GameVariantsInsertFailed",
            ["GameTypesInsertFailed"] = "Common_HudWebService_GameTypesInsertFailed",
            ["TableTypesInsertFailed"] = "Common_HudWebService_TableTypesInsertFailed",
            ["ImageInsertFailed"] = "Common_HudWebService_ImageInsertFailed",
            ["ImageFileMoveFailed"] = "Common_HudWebService_ImageFileMoveFailed",
            ["ValidationFailed"] = "Common_HudWebService_ValidationFailed",
        });
    }
}