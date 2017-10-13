//-----------------------------------------------------------------------
// <copyright file="NoteProcessingService.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using HandHistories.Objects.Hand;
using System.Threading;
using Model;
using DriveHUD.PlayerXRay.BusinessHelper;
using DriveHUD.PlayerXRay.DataTypes;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;

namespace DriveHUD.PlayerXRay.Services
{
    internal class NoteProcessingService : INoteProcessingService
    {
        public event EventHandler<NoteProcessingServiceProgressChangedEventArgs> ProgressChanged;

        public Playernotes ProcessHand(IEnumerable<NoteObject> notes, Playerstatistic stats, HandHistory handHistory)
        {
            var notesMessages = new List<string>();

            foreach (var note in notes)
            {
                var playerstatistic = new PlayerstatisticExtended
                {
                    Playerstatistic = stats,
                    HandHistory = handHistory
                };

                var noteMessage = NoteManager.GetPlayerNote(note, new List<PlayerstatisticExtended> { playerstatistic });

                if (!string.IsNullOrEmpty(noteMessage))
                {
                    notesMessages.Add(noteMessage);
                }
            }

            if (notesMessages.Count == 0)
            {
                return null;
            }

            var playerNotes = new Playernotes
            {
                PlayerId = stats.PlayerId,
                PokersiteId = (short)stats.PokersiteId,
                Note = string.Join(Environment.NewLine, notesMessages)
            };

            return playerNotes;
        }

        public void ProcessNotes(IEnumerable<NoteObject> notes)
        {
            //var playerStatisticGroupedBySite = SingletonStorageModel.Instance.StatisticCollection
            //    .GroupBy(x => x.PokersiteId).Select(x => new { PokerSiteId = x.Key, PlayerStatistic = x.ToArray() }).ToArray();

            //var dataService = ServiceLocator.Current.GetInstance<IDataService>();

            //var playerstatistics = new List<PlayerstatisticExtended>();

            //foreach (var playerStatisticGrouped in playerStatisticGroupedBySite)
            //{
            //    var handHistories = dataService
            //        .GetGames(playerStatisticGrouped.PlayerStatistic.Select(x => x.GameNumber), (short)playerStatisticGrouped.PokerSiteId)
            //        .ToDictionary(x => x.HandId);

            //    foreach (var stat in playerStatisticGrouped.PlayerStatistic)
            //    {
            //        if (!handHistories.ContainsKey(stat.GameNumber))
            //        {
            //            continue;
            //        }

            //        var playerstatistic = new PlayerstatisticExtended
            //        {
            //            Playerstatistic = stat,
            //            HandHistory = handHistories[stat.GameNumber]
            //        };

            //        playerstatistics.Add(playerstatistic);
            //    }
            //}
    


        }
    }
}