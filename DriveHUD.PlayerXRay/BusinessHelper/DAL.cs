#region Usings

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

#endregion

namespace DriveHUD.PlayerXRay.BusinessHelper
{
    public static class DAL
    {        
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