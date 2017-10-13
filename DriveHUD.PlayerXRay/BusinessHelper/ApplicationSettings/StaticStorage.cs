#region Usings

using DriveHUD.Entities;
using DriveHUD.PlayerXRay.DataTypes;
using System.Collections.Generic;

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
    
    }
}