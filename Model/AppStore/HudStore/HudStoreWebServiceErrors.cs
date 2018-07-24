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
            ["DbGeneralFailure"] = "",
            ["BadRequest"] = "",
            ["InvalidData"] = "",
            ["NameMustBeNotEmpty"] = "Common_HudUploadToStoreView_NameMustBeNotEmpty",
            ["NameMustBeLongerThan10"] = "Common_HudUploadToStoreView_NameMustBeLongerThan10",
            ["NameIsAlreadyInUse"] = "Common_HudUploadToStoreView_NameIsAlreadyInUse",
            ["DescriptionMustBeNotEmpty"] = "Common_HudUploadToStoreView_DescriptionMustBeNotEmpty",
            ["CostFormatIsInvalid"] = "",
            ["CostMustBeNotNegative"] = "Common_HudUploadToStoreView_CostMustBeNotNegative",
            ["GameVariantsMustBeSet"] = "Common_HudUploadToStoreView_GameVariantMustBeSelected",
            ["InvalidGameVariant"] = "",
            ["GameTypesMustBeSet"] = "Common_HudUploadToStoreView_GameTypeMustBeSelected",
            ["InvalidGameType"] = "",
            ["TableTypesMustBeSet"] = "Common_HudUploadToStoreView_TableTypeMustBeSelected",
            ["InvalidTableType"] = "",
            ["ImagesMustBeSet"] = "",
            ["ImagesFilesMustBeSet"] = "",
            ["CaptionMustBeSet"] = "",
            ["ImagesMustMatchImagesFiles"] = "",
            ["ImageFileMustBeImage"] = "",
            ["ImageFileIsTooLarge"] = "",
            ["HudInsertFailed"] = "",
            ["GameVariantsInsertFailed"] = "",
            ["GameTypesInsertFailed"] = "",
            ["TableTypesInsertFailed"] = "",
            ["ImageInsertFailed"] = "",
            ["ImageFileMoveFailed"] = "",
            ["ValidationFailed"] = "",
        });
    }
}