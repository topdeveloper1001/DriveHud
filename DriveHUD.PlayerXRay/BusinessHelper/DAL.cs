#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.DataAccessHelper.DriveHUD;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.DataTypes.InsertManagerObjects;
using Newtonsoft.Json;
using DriveHUD.Common.Log;

#endregion

namespace AcePokerSolutions.BusinessHelper
{
    public static class DAL
    {
        #region Delegates

        public delegate void DalErrorDelegate(string message, bool fatal);

        #endregion

        public static event DalErrorDelegate DalError;

        private static string ListToQueryList(IEnumerable<long> values)
        {
            string result = "(";

            foreach (int val in values)
            {
                result += val + ",";
            }

            if (result.Contains(","))
                result = result.Remove(result.LastIndexOf(','), 1);
            result += ")";

            return result;
        }

        public static List<Stake> GetAllStakes()
        {
            List<Stake> result = new List<Stake>();


            foreach (Playerstatistic pstatistic in StaticStorage.Playerstatistics)
            {
                if (!result.Any(x => x.ID == pstatistic.PokergametypeId && x.StakeValue == pstatistic.BigBlind))
                {
                    Stake stake = new Stake
                    {
                        ID = pstatistic.PokergametypeId,
                        Name = @"$"
                                    + pstatistic.BigBlind
                                    + " "
                                    + Enums.GetEnumDescription((GameType)pstatistic.PokergametypeId)
                                    + " "
                                    + GetTableNameByAmountOfMaxPlayers(pstatistic.HandHistory.GameDescription.SeatType.MaxPlayers),
                        StakeValue = pstatistic.BigBlind
                    };
                    result.Add(stake);
                }
            }
            return result;
        }

        private static string GetTableNameByAmountOfMaxPlayers(short maxPlayers)
        {
            if (maxPlayers == 6)
                return "(6 max)";
            if (maxPlayers == 10)
                return "FR";
            if (maxPlayers == 4)
                return "short";
            //todo check out of range values
            return "heads up";
        }


        public static List<Playerstatistic> GetAllStatistic()
        {
            Playerstatistic[] result = { };

            string url = Resources.WcfUrl + Resources.StatisticsPath + StaticStorage.CurrentPlayerName;
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            try
            {
                WebResponse response = request.GetResponse();

                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    string json = stream.ReadToEnd();
                    result = (Playerstatistic[])JsonConvert.DeserializeObject(json, typeof(Playerstatistic[]));
                }

                foreach (Playerstatistic playerstatistic in result)  //extraction handhistories for parsing
                {
                    playerstatistic.HandHistory = GetHandHistory(playerstatistic);
                }

            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(DAL), "Player statisics failed to load", ex);
            }



            //GetHandHistory(result.FirstOrDefault());
            //GetPlayerPosition(result.FirstOrDefault().PlayerName, result.FirstOrDefault().GameNumber, result.FirstOrDefault().PokersiteId.ToString(), result.FirstOrDefault().Time);

            return result.ToList();
        }

        public static string GetPlayerPosition(string playerName, long gameNumber, int pokerSite, DateTime time)
        {
            string playerPosition = "";

            Playerstatistic[] result = { };

            string url = Resources.WcfUrl + Resources.StatisticsPath
                            + playerName + "/"
                            + pokerSite + "/"
                            + gameNumber;
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            try
            {
                WebResponse response = request.GetResponse();

                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    playerPosition = stream.ReadToEnd().Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(DAL), "Player position failed to load", ex);
            }

            return playerPosition;

        }

        public static HandHistory GetHandHistory(Playerstatistic playerstatistic)
        {
            HandHistory receivedHand = new HandHistory();

            string url = Resources.WcfUrl + Resources.HandPath +
                playerstatistic.GameNumber +
                "/" + playerstatistic.PokersiteId;
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            try
            {
                WebResponse response = request.GetResponse();

                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    //var t = stream.ReadToEnd();
                    receivedHand = (HandHistory)new XmlSerializer(typeof(HandHistory)).Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(DAL), "Hand history failed to load", ex);
            }

            return receivedHand;
        }

        private static string GetDriveHudCurrentPlayer(out string playerName)
        {
            Player result = new Player();

            string url = Resources.WcfUrl + Resources.ActivePlayerPath;
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            try
            {
                WebResponse response = request.GetResponse();
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    string json = stream.ReadToEnd();
                    result = (Player)JsonConvert.DeserializeObject(json, typeof(Player));
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(DAL), "Current player failed to load", ex);                
            }

            playerName = result.Name;
            return result.PlayerId.ToString();
        }




        public static string GetCurrentPayer(int playerID, out string playerName)
        {
            return GetDriveHudCurrentPlayer(out playerName);
        }
    }
}