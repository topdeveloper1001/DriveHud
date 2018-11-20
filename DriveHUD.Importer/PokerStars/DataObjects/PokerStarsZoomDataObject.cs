//-----------------------------------------------------------------------
// <copyright file="PokerStarsZoomDataObject.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.Bovada;

namespace DriveHUD.Importers.PokerStars
{
    /// <summary>
    /// Class represents PS Zoom stream data object
    /// </summary>
    public class PokerStarsZoomDataObject
    {
        public int Handle { get; set; }

        public string Title { get; set; }

        internal GameType GameType { get; set; }

        public GameFormat GameFormat { get; set; }

        public EnumTableType TableType { get; set; }

        public byte MaxPlayers { get; set; }

        public PlayerDataObject[] Players { get; set; }

        public string TableName { get; set; }

        public string HeroName { get; set; }

        public long HandNumber { get; set; }

        public bool IsValid
        {
            get
            {
                return Handle != 0 && !string.IsNullOrEmpty(Title) && MaxPlayers != 0 &&
                    !string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(HeroName) && Players != null;
            }
        }
    }
}