//-----------------------------------------------------------------------
// <copyright file="HudBumperStickerService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using System.Collections.Generic;
using System.Windows.Media;

namespace Model.Hud
{
    public class HudBumperStickerService : IHudBumperStickerService
    {
        /// <summary>
        /// Gets the default bumper stickers
        /// </summary>        
        /// <returns>The collection of <see cref="HudBumperStickerType"/></returns>
        public IEnumerable<HudBumperStickerType> CreateDefaultBumperStickerTypes()
        {
            var bumperSticketTypes = new List<HudBumperStickerType>
            {
                new HudBumperStickerType(true)
                {
                    Name = "One and Done",
                    Description = "C-bets at a high% on the flop, but then rarely double barrels.",                    
                    SelectedColor = Color.FromRgb(255, 69, 0),
                    StatsToMerge = new []
                    {
                        new BaseHudRangeStat { Stat = Stat.CBet, Low = 55, High = 100 },
                        new BaseHudRangeStat { Stat = Stat.DoubleBarrel, Low = 0, High = 35 },
                    }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Pre-Flop Reg",
                    Description = "Plays an aggressive pre-flop game, but doesn’t play well post flop.",                    
                    SelectedColor = Color.FromRgb(255, 165, 0),
                    StatsToMerge = new []
                    {
                        new BaseHudRangeStat { Stat = Stat.VPIP, Low = 19, High = 26 },
                        new BaseHudRangeStat { Stat = Stat.PFR, Low = 15, High = 23 },
                        new BaseHudRangeStat { Stat = Stat.S3Bet, Low = 8, High = 10 },
                        new BaseHudRangeStat { Stat = Stat.WWSF, Low = 0, High = 42 }                        
                    }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Barrelling",
                    Description = "Double and triple barrels a high percentage of the time.",
                    SelectedColor = Color.FromRgb(255, 255, 0),
                    StatsToMerge = new []
                    {
                        new BaseHudRangeStat { Stat = Stat.VPIP, Low = 20, High = 30 },
                        new BaseHudRangeStat { Stat = Stat.PFR, Low = 17, High = 28 },
                        new BaseHudRangeStat { Stat = Stat.AGG, Low = 40, High = 49 },
                        new BaseHudRangeStat { Stat = Stat.CBet, Low = 65, High = 80 },
                        new BaseHudRangeStat { Stat = Stat.WWSF, Low = 44, High = 53 },
                        new BaseHudRangeStat { Stat = Stat.DoubleBarrel, Low = 46, High = 100 }
                    }
                },
                new HudBumperStickerType(true)
                {
                    Name = "3 For Free",
                    Description = "3-Bets too much, and folds to a 3-bet too often.",
                    SelectedColor = Color.FromRgb(173, 255, 47),
                    StatsToMerge = new []
                    {
                        new BaseHudRangeStat { Stat = Stat.S3Bet, Low = 8.8m, High = 100 },
                        new BaseHudRangeStat { Stat = Stat.FoldTo3Bet, Low = 66, High = 100 }                   
                    }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Way Too Early",
                    Description = "Open raises to wide of a range in early pre-flop positions.",
                    SelectedColor = Color.FromRgb(0, 128, 0),
                    StatsToMerge = new []
                    {
                        new BaseHudRangeStat { Stat = Stat.UO_PFR_EP, Low = 20, High = 100 }                        
                    }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Sticky Fish",
                    Description = "Fishy player who can’t fold post flop if they get any piece of the board.",
                    SelectedColor = Color.FromRgb(0, 0, 255),
                    StatsToMerge = new []
                    {
                        new BaseHudRangeStat { Stat = Stat.VPIP, Low = 35, High = 100 },
                        new BaseHudRangeStat { Stat = Stat.WTSD, Low = 29, High = 100 }
                    }
                },
                new HudBumperStickerType(true)
                {
                    Name = "Yummy Fish",
                    Description = "Plays too many hands pre-flop and isn’t aggressive post flop.",
                    SelectedColor = Color.FromRgb(0, 0, 139),
                    StatsToMerge = new []
                    {
                        new BaseHudRangeStat { Stat = Stat.VPIP, Low = 40, High = 100 },
                        new BaseHudRangeStat { Stat = Stat.AGG, Low = 0, High = 34 }
                    }
                }
            };

            return bumperSticketTypes;
        }
    }
}