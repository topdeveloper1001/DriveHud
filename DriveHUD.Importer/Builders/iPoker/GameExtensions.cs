//-----------------------------------------------------------------------
// <copyright file="GameExtensions.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Bovada;
using Model.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal static class GameExtensions
    {
        public static IList<int> GetWinners(this IEnumerable<Round> rounds, GameType gameType)
        {
            if (rounds == null)
            {
                throw new ArgumentNullException(nameof(rounds));
            }

            var evaluator = GetEvaluator(gameType);

            if (evaluator == null)
            {
                return new List<int>();
            }

            var roundCards = rounds.Where(x => x.Cards != null).SelectMany(x => x.Cards).ToArray();
            var boardCards = roundCards.Where(x => x.Type != CardsType.Pocket).Select(x => x.Value).ToArray();
            var boardCardsString = string.Join(" ", boardCards);

            evaluator.SetCardsOnTable(boardCardsString);

            var playersCards = roundCards.Where(x => x.Type == CardsType.Pocket && !x.Value.Contains(PokerConfiguration.UnknownCard)).ToArray();

            if (playersCards.Length < 1)
            {
                return new List<int>();
            }

            foreach (var playerCards in playersCards)
            {
                evaluator.SetPlayerCards(playerCards.Seat, playerCards.Value);
            }

            var winners = evaluator.GetWinners();

            return winners.All?.ToList() ?? new List<int>();
        }

        private static IPokerEvaluator GetEvaluator(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.Omaha:
                case GameType.OmahaHiLo:
                    return new OmahaEvaluator();
                case GameType.Holdem:
                    return new HoldemEvaluator();
                default:
                    return null;
            }
        }
    }
}