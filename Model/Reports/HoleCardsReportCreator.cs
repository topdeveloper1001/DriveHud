using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Model.Data;
using DriveHUD.Entities;
using Model.Extensions;

namespace Model.Reports
{
    /// <summary>
    /// This report groups games by cards that was dealt to player
    /// </summary>
    public class HoleCardsReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            foreach (var group in statistics.Where(x=> !x.IsTourney).GroupBy(x => x.Cards.ToCards()))
            {
                HoleCardsReportRecord stat = new HoleCardsReportRecord();
                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                    if(stat.Cards == null)
                    {
                        stat.Cards = new ComparableReportCards();
                    }
                    stat.Cards.CardsString = group.Key;
                }

                report.Add(stat);
            }

            return report;
        }
    }

    public class TournamentHoleCardsReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if(statistics == null)
            {
                return report;
            }

            foreach (var group in statistics.Where(x=> x.IsTourney).GroupBy(x => x.Cards.ToCards()))
            {
                HoleCardsReportRecord stat = new HoleCardsReportRecord();
                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                    if(stat.Cards == null)
                    {
                        stat.Cards = new ComparableReportCards();
                    }
                    stat.Cards.CardsString = group.Key;
                }

                report.Add(stat);
            }

            return report;
        }
    }

    public static class HoleCardsReportStringExtensions
    {
        public static string ToCards(this string holeCards)
        {
            if (string.IsNullOrWhiteSpace(holeCards))
                return string.Empty;

            var cards = CardHelper.Split(holeCards);
            if (cards.Count != 2)
                return string.Empty;

            var value1 = cards[0].TrimEnd('c', 'd', 'h', 's');
            var value2 = cards[1].TrimEnd('c', 'd', 'h', 's');

            var result = value1 + value2;
            if (value1 != value2)
            {
                result += cards[0].Last() == cards[1].Last() ? "s" : "o";
            }

            return result;
        }
    }

}