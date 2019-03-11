using DriveHUD.API.DataContracts;
using DriveHUD.API.Interfaces;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Web;

namespace DriveHUD.API
{
    public class APIService : IAPIService
    {
        private const string DATE_TIME_FORMAT = "yyyyMMdd";
        private readonly IDataService dataService;
        private readonly IPlayerStatisticRepository playerStatisticRepository;
        private readonly SingletonStorageModel storageModel;

        public APIService()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();
            playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
        }

        public HandHistory GetHandById(string id, string pokerSite)
        {
            EnumPokerSites site = EnumPokerSites.Unknown;
            long parsedId = 0;

            if (!long.TryParse(id, out parsedId))
            {
                throw new WebFaultException<string>("Cannot parse hand id", System.Net.HttpStatusCode.BadRequest);
            }

            if (Enum.TryParse(pokerSite, out site))
            {
                // Return hand for currently selected user
                if (site == EnumPokerSites.Unknown)
                {
                    site = storageModel.PlayerSelectedItem?.PokerSite ?? EnumPokerSites.Unknown;
                }

                return dataService.GetGame(parsedId, (short)site);
            }
            else
            {
                throw new WebFaultException<string>($"Cannot recognise site {pokerSite}", System.Net.HttpStatusCode.BadRequest);
            }
        }

        public HandHistory[] GetHands(string date)
        {
            var statisticCollection = storageModel.GetStatisticCollection();

            DateTime parsedDate = ParseDate(date, statisticCollection.Max(x => x.Time));

            var handsToSelect = statisticCollection.Where(x => x.Time.Date == parsedDate.Date);

            if (!handsToSelect.Any())
            {
                return new HandHistory[] { };
            }

            var pokerSiteId = handsToSelect.FirstOrDefault().PokersiteId;
            return dataService.GetGames(handsToSelect.Select(x => x.GameNumber), (short)pokerSiteId).ToArray();
        }

        public HandHistory[] GetPlayerHands(string playerName, string pokerSite, string date)
        {
            EnumPokerSites site = EnumPokerSites.Unknown;

            if (!Enum.TryParse(pokerSite, out site) || site == EnumPokerSites.Unknown)
            {
                throw new WebFaultException<string>($"Cannot recognise site {pokerSite}", System.Net.HttpStatusCode.BadRequest);
            }

            var statistic = playerStatisticRepository.GetPlayerStatistic(playerName, (short)site);

            DateTime parsedDate = ParseDate(date, statistic.Max(x => x.Time));

            var handsToSelect = statistic.Where(x => x.Time.Date == parsedDate.Date);

            if (!handsToSelect.Any())
            {
                return new HandHistory[] { };
            }

            var pokerSiteId = handsToSelect.FirstOrDefault().PokersiteId;

            return dataService.GetGames(handsToSelect.Select(x => x.GameNumber), (short)pokerSiteId).ToArray();
        }

        public HandInfoDataContract[] GetHandsList(string date)
        {
            var statisticCollection = storageModel.GetStatisticCollection();

            DateTime parsedDate = ParseDate(date, statisticCollection.Max(x => x.Time));

            if (TryParseDate(date, out parsedDate))
            {
                return statisticCollection.Where(x => x.Time.Date == parsedDate.Date).Select(x => HandInfoDataContract.Map(x)).ToArray();
            }
            else
            {
                return statisticCollection.Select(x => HandInfoDataContract.Map(x)).ToArray();
            }
        }

        public HandInfoDataContract[] GetPlayerHandsList(string playerName, string pokerSite, string date)
        {
            EnumPokerSites site = EnumPokerSites.Unknown;

            if (!Enum.TryParse(pokerSite, out site) || site == EnumPokerSites.Unknown)
            {
                throw new WebFaultException<string>($"Cannot recognise site {pokerSite}", System.Net.HttpStatusCode.BadRequest);
            }

            var statistic = playerStatisticRepository.GetPlayerStatistic(playerName, (short)site);

            DateTime parsedDate = DateTime.Now;

            if (TryParseDate(date, out parsedDate))
            {
                return statistic.Where(x => x.Time.Date == parsedDate.Date).Select(x => HandInfoDataContract.Map(x)).ToArray();
            }
            else
            {
                return statistic.Select(x => HandInfoDataContract.Map(x)).ToArray();
            }
        }

        private bool TryParseDate(string date, out DateTime parsedDate)
        {
            return DateTime.TryParseExact(date, DATE_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }

        private DateTime ParseDate(string date, DateTime defaultValue)
        {
            DateTime parsedDate = DateTime.Now;

            if (!DateTime.TryParseExact(date, DATE_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                parsedDate = defaultValue;
            }

            return parsedDate;
        }

    }
}