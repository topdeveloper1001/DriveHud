﻿//-----------------------------------------------------------------------
// <copyright file="IPlayerStatisticRepository.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IPlayerStatisticRepository
    {
        void SetPlayerStatisticPath(string path);

        IEnumerable<Playerstatistic> GetPlayerStatisticFromFiles(IEnumerable<string> files);

        IEnumerable<Playerstatistic> GetPlayerStatisticFromFile(string file);

        IEnumerable<Playerstatistic> GetPlayerStatistic(int playerId);

        IEnumerable<Playerstatistic> GetPlayerStatistic(string playerName, short? pokersiteId);

        void Store(IEnumerable<Playerstatistic> statistic);

        void Store(Playerstatistic statistic);

        void DeletePlayerStatisticFromFile(Playerstatistic statistic);

        string[] GetPlayerFiles(int playerId);
    }
}