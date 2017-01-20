using System;
using DriveHUD.Entities;
using Model.Enums;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class HudLayoutMapping
    {
        public EnumTableType TableType { get; set; }
        public EnumGameType? GameType { get; set; }
        public EnumPokerSites? PokerSite { get; set; }
        public HudViewType HudViewType { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDefault { get; set; }
    }
}