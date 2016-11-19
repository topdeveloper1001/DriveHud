//-----------------------------------------------------------------------
// <copyright file="ParsingResult.cs" company="Ace Poker Solutions">
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
using HandHistories.Objects.Hand;
using System.Collections.Generic;

namespace HandHistories.Parser.Parsers
{
    public class ParsingResult
    {
        /// <summary>
        /// Record to be inserted it database
        /// </summary>
        public Handhistory HandHistory { get; set; }

        /// <summary>
        /// List of players involved in game
        /// </summary>
        public IList<Players> Players { get; set; }

        /// <summary>
        /// Game specific information. Should be inserted into database
        /// </summary>
        public Gametypes GameType { get; set; }

        /// <summary>
        /// Parsing Source
        /// </summary>
        public HandHistory Source { get; set; }

        /// <summary>
        /// Tournament tag
        /// </summary>
        public TournamentsTags TournamentsTags { get; set; }

        /// <summary>
        /// Flag if hand has been imported
        /// </summary>
        public bool WasImported { get; set; }

        /// <summary>
        /// Flag if hand has already been imported
        /// </summary>
        public bool IsDuplicate { get; set; }

        /// <summary>
        /// Flag if result is just summary hand
        /// </summary>
        public bool IsSummary
        {
            get
            {
                return Source != null && Source.GameDescription != null && Source.GameDescription.IsTournament && Source.GameDescription.Tournament.IsSummary;
            }
        }
    }
}