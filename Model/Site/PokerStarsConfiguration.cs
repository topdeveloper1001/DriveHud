//-----------------------------------------------------------------------
// <copyright file="PokerStarsConfiguration.cs" company="Ace Poker Solutions">
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
using Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Site
{
    public class PokerStarsConfiguration : ISiteConfiguration
    {
        private static readonly string[] PossibleFolders = new string[] { "PokerStars", "PokerStars.EU", "PokerStars.USNJ" };
        private static readonly string[] HandHistoryFolders = new string[] { "HandHistory", "TournSummary" };

        public PokerStarsConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Eight,
                EnumTableType.Nine,
                EnumTableType.Ten
            };
        }

        public EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PokerStars;
            }
        }

        private readonly IEnumerable<EnumTableType> tableTypes;

        public IEnumerable<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        public string HeroName
        {
            get;
            set;
        }

        private readonly Dictionary<int, int> prefferedSeat;

        public Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public string[] GetHandHistoryFolders()
        {
            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var dirs = (from possibleFolder in PossibleFolders
                        from handHistoryFolder in HandHistoryFolders
                        let folder = Path.Combine(localApplicationData, possibleFolder)
                        let hhFolder = Path.Combine(folder, handHistoryFolder)
                        where Directory.Exists(folder)
                        select hhFolder).ToArray();

            return dirs;
        }
    }
}