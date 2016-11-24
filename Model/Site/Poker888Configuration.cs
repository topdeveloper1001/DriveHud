//-----------------------------------------------------------------------
// <copyright file="Poker888Configuration.cs" company="Ace Poker Solutions">
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
    public class Poker888Configuration : ISiteConfiguration
    {
        public Poker888Configuration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,      
                EnumTableType.Three,
                EnumTableType.Four,          
                EnumTableType.Five,
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
                return EnumPokerSites.Poker888;
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
            var myDocumentData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var possibleFolders = new string[] { "888poker" };

            var dirs = (from possibleFolder in possibleFolders
                        let folder = Path.Combine(myDocumentData, possibleFolder, "HandHistory")
                        select folder).ToArray();

            return dirs;
        }

        public void ValidateSiteConfiguration()
        {
        }
    }
}