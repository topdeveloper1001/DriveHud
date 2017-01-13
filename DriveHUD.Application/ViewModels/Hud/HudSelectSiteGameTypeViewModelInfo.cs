using System;
using DriveHUD.Entities;
using Model.Enums;

namespace DriveHUD.Application.ViewModels
{
    public class HudSelectSiteGameTypeViewModelInfo
    {
        public EnumPokerSites? PokerSite { get; set; }
        public EnumGameType? GameType { get; set; }
        public Action Save { get; set; }

        public Action Cancel { get; set; }
    }
}