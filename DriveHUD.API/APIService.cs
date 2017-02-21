using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DriveHUD.API
{
    public class APIService : IAPIService
    {
        public string GetHandById(string id)
        {
            return $"Hello {id}";
        }

        public HandHistory[] GetPlayerHands(string playerName, string pokersiteId)
        {
            var games = new List<HandHistory>();

            var stats = ServiceLocator.Current.GetInstance<SingletonStorageModel>().FilteredPlayerStatistic.Take(3);
            foreach (var s in stats)
            {
               games.Add(ServiceLocator.Current.GetInstance<IDataService>().GetGame(s.GameNumber, (short)s.PokersiteId));
            }

            return games.ToArray();
        }
    }
}
