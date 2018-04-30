//-----------------------------------------------------------------------
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
using Model.Data;
using System.Collections.Generic;

namespace Model
{
    public interface IPlayerStatisticRepository
    {
        void SetPlayerStatisticPath(string path);

        IEnumerable<Playerstatistic> GetAllPlayerStatistic(string playerName, short? pokersiteId);

        IEnumerable<Playerstatistic> GetPlayerStatisticFromFiles(IEnumerable<string> files);

        IEnumerable<Playerstatistic> GetPlayerStatisticFromFile(string file);

        IEnumerable<Playerstatistic> GetPlayerStatistic(int playerId);

        IEnumerable<Playerstatistic> GetPlayerStatistic(string playerName, short? pokersiteId);

        IDictionary<string, T> GetPlayersIndicators<T>(string[] playerNames, short? pokersiteId) where T : Indicators;

        void Store(IEnumerable<Playerstatistic> statistic);

        void Store(Playerstatistic statistic);

        void DeletePlayerStatisticFromFile(Playerstatistic statistic);

        void DeletePlayerStatistic(Dictionary<int, List<Handhistory>> playersHands);

        string[] GetPlayerFiles(int playerId);
    }
}