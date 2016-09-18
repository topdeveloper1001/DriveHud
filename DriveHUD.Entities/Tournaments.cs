//-----------------------------------------------------------------------
// <copyright file="Tournaments.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    public partial class Tournaments
    {
        public virtual int TourneydataId { get; set; }

        public virtual Players Player { get; set; }

        [Required]
        public virtual short PokergametypeId { get; set; }
  
        [Required]
        public virtual int Finishposition { get; set; }

        public virtual int? Bountyincents { get; set; }

        [Required]
        public virtual int Buyinincents { get; set; }

        [Required]
        public virtual short CurrencyId { get; set; }

        [Required]
        public virtual short ImporttypeId { get; set; }

        [Required]
        public virtual short Tablesize { get; set; }

        [Required]
        public virtual string Tourneynumber { get; set; }

        [Required]
        public virtual short Tourneytables { get; set; }

        public virtual DateTime Firsthandtimestamp { get; set; }

        [Required]
        public virtual int Winningsincents { get; set; }

        public virtual DateTime Lasthandtimestamp { get; set; }

        [Required]
        public virtual short SpeedtypeId { get; set; }

        [Required]
        public virtual DateTime Filelastmodifiedtime { get; set; }

        [Required]
        public virtual int Rebuyamountincents { get; set; }

        [Required]
        public virtual int Rakeincents { get; set; }

        [Required]
        public virtual bool Tourneyendedforplayer { get; set; }

        [Required]
        public virtual int Tourneysize { get; set; }

        [Required]
        public virtual string Tourneytagscsv { get; set; }

        [Required]
        public virtual short SiteId { get; set; }

        [Required]
        public virtual string Filename { get; set; }

        [Required]
        public virtual short Startingstacksizeinchips { get; set; }

        public virtual string PlayerName
        {
            get
            {
                return Player != null ? Player.Playername : string.Empty;
            }
        }
    }
}