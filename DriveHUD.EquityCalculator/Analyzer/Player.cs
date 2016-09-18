using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class Player
    {
        internal Player() { }
        internal Player(String name, int position, int startingStack)
        {
            this.PlayerName = name;
            this.Position = position;
            this.StartingStack = startingStack;
        }
        internal static Hashtable PlayersModelPic = new Hashtable();
        internal float OpenRaiseRange;
        internal String PlayerName;
        internal int HandWeight;
        internal int SeatNumber;
        internal int Position; // In the order of who acts first preflop (can be mapped to position_names array)
        internal int StartingStack;
        internal String Cards;
        internal bool Showdown;
        internal int Wins;
        internal double WinPrct;
        internal Stats Stats = new Stats();
        internal bool ShouldUseTempStats;
        internal bool TempUseStats;
        internal bool CanUseStats
        {
            get { return ShouldUseTempStats ? TempUseStats : Settings.CurrentSettings.UseOpponentSpecificStats && (!Settings.CurrentSettings.UseOpponentsStatsOnlyWhenOpponentHasXHands || (Settings.CurrentSettings.UseOpponentsStatsOnlyWhenOpponentHasXHands && Stats.NBHands >= Settings.CurrentSettings.OpponentXHands)); }
        }

        internal static String GetPlayerModel(bool is6Max, Player player)
        {
            String model = "", tmpModel = "N/A";
            Stats stat = player.Stats;
            if (stat.NBHands < 10) return "Unknown";

            if (!is6Max)
            {
                #region FR
                if (stat.VPIP <= 13)
                {
                    tmpModel = "NIT";
                    if (stat.Agg <= 2.1)
                        model = tmpModel;
                }
                if (stat.VPIP > 13 && stat.VPIP <= 16)
                {
                    tmpModel = "TIGHT TAG";
                    if (stat.Agg <= 2.5)
                        model = tmpModel;
                }
                if (stat.VPIP > 16 && stat.VPIP <= 22)
                {
                    tmpModel = "TAG";
                    if (stat.Agg >= 2.6 && stat.Agg <= 3.5)
                        model = tmpModel;
                }
                if (stat.VPIP > 22 && stat.VPIP <= 30)
                {
                    if (stat.Agg >= 3)
                        tmpModel = "LAG";
                    else if (stat.Agg <= 2.4) tmpModel = "BAD TAG";
                    else tmpModel = "TAG";
                    if (stat.Agg >= 3.2 && stat.Agg <= 4.6)
                        model = tmpModel;
                }
                if (stat.VPIP > 30 && stat.VPIP <= 45)
                {
                    tmpModel = "FISH";
                    if (stat.Agg <= 2.6)
                        model = tmpModel;
                }
                if (stat.VPIP >= 35)
                {
                    if (stat.Agg >= 3.5)
                    {
                        tmpModel = "GAMBLER";
                        model = tmpModel;
                    }
                }
                if (stat.VPIP >= 45.1)
                {
                    model = "WHALE";
                }
                #endregion FR
            }
            else if (is6Max)
            {
                #region 6Max
                if (stat.VPIP <= 15)
                {
                    tmpModel = "NIT";
                    if (stat.Agg <= 2.3)
                        model = tmpModel;
                }
                if (stat.VPIP > 15 && stat.VPIP <= 19)
                {
                    tmpModel = "TIGHT TAG";
                    if (stat.Agg <= 2.6)
                        model = tmpModel;
                }
                if (stat.VPIP > 19 && stat.VPIP <= 23)
                {
                    tmpModel = "TAG";
                    if (stat.Agg >= 2.5 && stat.Agg <= 3.6)
                        model = tmpModel;
                }
                if (stat.VPIP > 23 && stat.VPIP <= 31)
                {
                    if (stat.Agg <= 2.4) tmpModel = "BAD TAG";
                    else tmpModel = "TAG";

                    if (stat.Agg >= 3)
                        tmpModel = "LAG";
                    if (stat.Agg >= 3.2 && stat.Agg <= 4.6)
                        model = tmpModel;
                }
                if (stat.VPIP > 31 && stat.VPIP <= 45)
                {
                    tmpModel = "FISH";
                    if (stat.Agg <= 2.9)
                        model = tmpModel;
                }
                if (stat.VPIP >= 35)
                {
                    if (stat.Agg >= 3)
                    {
                        tmpModel = "GAMBLER";
                        model = tmpModel;
                    }
                }
                if (stat.VPIP >= 45.1)
                {
                    tmpModel = "WHALE";
                    if (stat.Agg <= 2.9)
                        model = tmpModel;
                }
                #endregion 6Max
            }
            return model.Equals("") ? tmpModel : model;
        }

        static Hashtable PlayerIDNames = new Hashtable();
        internal static String GetPlayerIDByName(String playerName, String site)
        {
            return "";
        }


    }
}
