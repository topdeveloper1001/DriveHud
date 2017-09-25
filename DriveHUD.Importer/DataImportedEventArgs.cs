﻿//-----------------------------------------------------------------------
// <copyright file="DataImportedEventArgs.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Players;
using Model.Enums;
using System;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Imported event data
    /// </summary>
    public class DataImportedEventArgs : EventArgs
    {
        public DataImportedEventArgs(PlayerList players, GameInfo gameInfo, Player hero)
        {
            Players = players;
            GameInfo = gameInfo;
            Hero = hero;
        }

        public PlayerList Players { get; private set; }

        public Player Hero { get; private set; }

        public GameInfo GameInfo { get; private set; }

        public bool DoNotUpdateHud { get; set; }
    }
}