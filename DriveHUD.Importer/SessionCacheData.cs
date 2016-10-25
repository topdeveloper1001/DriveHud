﻿//-----------------------------------------------------------------------
// <copyright file="SessionCacheData.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model;
using DriveHUD.Entities;
using System.Collections.Generic;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Session cache object
    /// </summary>
    internal class SessionCacheData
    {
        private readonly Dictionary<PlayerCollectionItem, IList<Playerstatistic>> statisticByPlayer;
        private readonly Dictionary<PlayerCollectionItem, Playerstatistic> lastHandStatisticByPlayer;
        private readonly Dictionary<PlayerCollectionItem, Dictionary<string, Playerstatistic>> stickersStatisticByPlayer;
        private readonly List<HandHistoryRecord> records;

        public SessionCacheData()
        {
            statisticByPlayer = new Dictionary<PlayerCollectionItem, IList<Playerstatistic>>();
            lastHandStatisticByPlayer = new Dictionary<PlayerCollectionItem, Playerstatistic>();
            stickersStatisticByPlayer = new Dictionary<PlayerCollectionItem, Dictionary<string, Playerstatistic>>();
            records = new List<HandHistoryRecord>();
        }

        public Dictionary<PlayerCollectionItem, IList<Playerstatistic>> StatisticByPlayer
        {
            get
            {
                return statisticByPlayer;
            }
        }

        public  Dictionary<PlayerCollectionItem, Playerstatistic> LastHandStatisticByPlayer
        {
            get
            {
                return lastHandStatisticByPlayer;
            }
        }

        public Dictionary<PlayerCollectionItem, Dictionary<string, Playerstatistic>> StickersStatisticByPlayer
        {
            get
            {
                return stickersStatisticByPlayer;
            }
        }

        public List<HandHistoryRecord> Records
        {
            get
            {
                return records;
            }
        }
    }
}