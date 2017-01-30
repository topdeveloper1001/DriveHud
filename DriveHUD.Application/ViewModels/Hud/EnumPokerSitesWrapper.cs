﻿using DriveHUD.Common.Resources;
using DriveHUD.Entities;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class EnumPokerSitesWrapper
    {
        public EnumPokerSites? PokerSite { get; }
        public string PokerSiteText { get; }

        public EnumPokerSitesWrapper(EnumPokerSites? pokerSite)
        {
            PokerSite = pokerSite;
            PokerSiteText = PokerSite.HasValue
                ? CommonResourceManager.Instance.GetEnumResource(PokerSite.Value)
                : string.Empty;
        }
    }
}