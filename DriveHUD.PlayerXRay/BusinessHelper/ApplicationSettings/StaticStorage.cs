#region Usings

using System.Collections.Generic;
using System.Linq;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.DataAccessHelper.DriveHUD;
using DriveHUD.PlayerXRay.DataTypes;

#endregion

namespace DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings
{
    public static class StaticStorage
    {
        static StaticStorage()
        {
            Stakes = new List<Stake>();
            Playerstatistics = new List<Playerstatistic>();
        }

        public static List<Stake> Stakes { get; private set; }
        public static List<Playerstatistic> Playerstatistics { get; private set; }
        public static string CurrentPlayer { get; set; }
        public static string CurrentPlayerName { get; set; }    

        public static void LoadStaticObjects()
        {
            Playerstatistics = DAL.GetAllStatistic(); 
            Stakes = DAL.GetAllStakes();            
        }
    }
}