//-----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Importers.GGNetwork.Model
{
    public class Configuration
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int PlayType { get; set; }

        public int GameType { get; set; }

        public int GameLimitType { get; set; }

        public int GamePlayersCount { get; set; }

        public int MinPlayers { get; set; }

        public int MaxPlayers { get; set; }

        public int StartingChips { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public int TimeBetting { get; set; }

        public int TimeSandGlass { get; set; }

        public int TimeAddBetting { get; set; }
    }
}