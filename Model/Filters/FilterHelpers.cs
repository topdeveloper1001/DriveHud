using DriveHUD.Common.Resources;
using HandHistories.Objects.Cards;
using Model.Extensions;
using Model.OmahaHoleCardsAnalyzers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Filters
{
    public static class FilterHelpers
    {
        /// <summary>
        /// Checks if action line occurs on a  specific street
        /// </summary>
        /// <param name="lineToCheck">action line to search in</param>
        /// <param name="streetActions">action line to search for</param>
        /// <param name="street">street</param>
        /// <returns>true if streeActions is equal to the action line on a specific street</returns>
        public static bool CheckActionLine(string lineToCheck, string streetActions, Street street)
        {
            if(string.IsNullOrEmpty(lineToCheck) || string.IsNullOrEmpty(streetActions))
            {
                return false;
            }

            var lines = lineToCheck.Split(StringFormatter.ActionLineSeparator.ToCharArray());
            if(lines != null && lines.Any() && !string.IsNullOrEmpty(streetActions))
            {
                int streetIndex = -1;
                switch (street)
                {
                    case Street.Preflop:
                        streetIndex = 0;
                        break;
                    case Street.Flop:
                        streetIndex = 1;
                        break;
                    case Street.Turn:
                        streetIndex = 2;
                        break;
                    case Street.River:
                        streetIndex = 3;
                        break;
                }

                if(streetIndex != -1 && lines.Count() > streetIndex)
                {
                    return String.Compare(lines[streetIndex], streetActions, ignoreCase: true) == 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if collection contains specified cards
        /// </summary>
        /// <param name="cards">cards to look for</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool CheckHoleCards(string cards, IEnumerable<HoleCardsItem> collection)
        {
            var splittedCards = CardHelper.Split(cards);

            return collection.Any(c => c.Contains(splittedCards));
        }

        /// <summary>
        /// Checks if specific omaha hole cards pass the filter
        /// </summary>
        /// <param name="cards">omaha hole cards</param>
        /// <param name="collection">collection of items that contains filter description</param>
        /// <returns></returns>
        public static bool CheckOmahaHoleCards(string cards, IEnumerable<OmahaHandGridItem> collection)
        {
            var cardsList = CardGroup.Parse(cards);
            var analyzers = OmahaHoleCardsAnalyzer.GetDefaultOmahaHoleCardsAnalyzers();

            return collection.All(item => analyzers.First(a => a.GetRank() == item.HoleCards).Analyze(cardsList, item));
        }
    }
}
