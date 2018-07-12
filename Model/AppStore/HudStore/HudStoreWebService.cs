//-----------------------------------------------------------------------
// <copyright file="HudStoreWebService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
using Model.AppStore.HudStore.Model;

namespace Model.AppStore.HudStore
{
    public class HudStoreWebService : IHudStoreWebService
    {
        private const string requestKey = "";

        public IEnumerable<GameType> GetGameTypes()
        {
            RandomDelay();

            return new List<GameType>
            {
                new GameType { Id = 1, Name = "Cash game"},
                new GameType { Id = 2, Name = "S&G"},
                new GameType { Id = 3, Name = "MTT"}
            };
        }

        public IEnumerable<GameVariant> GetGameVariants()
        {
            RandomDelay();

            return new List<GameVariant>
            {
                new GameVariant { Id = 1, Name = "No-limit Holdem"},
                new GameVariant { Id = 2, Name = "Limit Holdem"},
                new GameVariant { Id = 3, Name = "PLO"},
                new GameVariant { Id = 4, Name = "No-limit Omaha"},
                new GameVariant { Id = 5, Name = "Omaha Hi Limit"},
                new GameVariant { Id = 6, Name = "PLO8"},
                new GameVariant { Id = 7, Name = "Limit O8"},
                new GameVariant { Id = 8, Name = "No-Limit O8"},
            };
        }

        public IEnumerable<TableType> GetTableTypes()
        {
            RandomDelay();

            return Enumerable
                .Range(2, 8)
                .Select((x, i) => new TableType
                {
                    Id = (short)i,
                    Name = $"{x}-max",
                    MaxPlayers = (short)x
                })
                .ToList();
        }

        private void RandomDelay()
        {
            var random = new Random();
            var randomDelay = random.Next(200, 500);

            Task.Delay(randomDelay).Wait();
        }
    }
}