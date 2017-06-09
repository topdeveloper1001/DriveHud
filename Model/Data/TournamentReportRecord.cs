//-----------------------------------------------------------------------
// <copyright file="TournamentReportRecord.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using System;

namespace Model.Data
{
    public class TournamentReportRecord : ReportIndicators
    {
        public virtual string PlayerName { get; set; }

        public virtual string TournamentId { get; set; }

        public virtual int TournamentsPlayed { get; set; }

        public virtual int TournamentsInPrizes { get; set; }

        /// <summary>
        /// Holds the BuyIn value for tournament
        /// </summary>
        public virtual decimal BuyIn { get; set; }

        /// <summary>
        /// Holds the total amount spent on BuyIns
        /// </summary>
        public virtual decimal TotalBuyIn { get; set; }

        public virtual decimal Rake { get; set; }

        public virtual string TableType { get; set; }

        public virtual string TournamentSpeed { get; set; }

        public virtual string TournamentLength { get; set; }

        public virtual int FinishPosition { get; set; }

        public virtual int FinalTables { get; set; }

        public virtual decimal Rebuy { get; set; }

        public virtual decimal Won { get; set; }

        public virtual int TableSize { get; set; }

        public virtual DateTime Started { get; set; }

        public virtual decimal ROI
        {
            get
            {
                return GetPercentage(NetWon, TotalExpenses);
            }
        }

        public virtual decimal ITM
        {
            get
            {
                return GetPercentage(TournamentsInPrizes, TournamentsPlayed);
            }
        }

        public virtual decimal TotalExpenses
        {
            get
            {
                return Rake + TotalBuyIn + Rebuy;
            }
        }

        public override decimal NetWon
        {
            get
            {
                return Won - TotalExpenses;
            }
        }

        public void SetBuyIn(int buyinInCents)
        {
            BuyIn = buyinInCents / 100m;
        }

        public void SetTotalBuyIn(int buyinInCents)
        {
            TotalBuyIn = buyinInCents / 100m;
        }

        public void SetTableType(string tournamentTag)
        {
            this.TableType = string.Empty;

            TournamentsTags tag;

            if (Enum.TryParse(tournamentTag, out tag))
            {
                switch (tag)
                {
                    case TournamentsTags.MTT:
                        TableType = "MTT";
                        break;
                    case TournamentsTags.STT:
                        TableType = "S&G";
                        break;
                }
            }
        }

        internal void SetRebuy(int rebuyamountincents)
        {
            Rebuy = rebuyamountincents / 100m;
        }

        internal void SetWinning(int winningsincents)
        {
            Won = winningsincents / 100m;
        }

        internal void SetRake(int rakeincents)
        {
            Rake = rakeincents / 100m;
        }

        public void SetSpeed(int speedTypeId)
        {
            TournamentSpeed = string.Empty;

            TournamentSpeed speed;

            if (Enum.TryParse(speedTypeId.ToString(), out speed))
            {
                TournamentSpeed = speed.ToString();
            }
        }

        public void SetGameType(int pokergametypeId)
        {
            GameType = string.Empty;

            GameType gametype;

            if (Enum.TryParse(pokergametypeId.ToString(), out gametype))
            {
                GameType = gametype.ToString();
            }
        }

        public void SetTournamentLength(DateTime firstHandTimeStamp, DateTime lastHandTimeStamp)
        {
            var length = lastHandTimeStamp - firstHandTimeStamp;
            TournamentLength = string.Format("{0}m", Math.Round(length.TotalMinutes));
        }

        /* Do not inherit this method from the Indicators because TotalHands might be 0 for this container */
        protected override decimal GetPercentage(decimal? actual, decimal? possible)
        {
            if (!possible.HasValue || !actual.HasValue || possible == 0)
            {
                return 0;
            }

            return (actual.Value / possible.Value) * 100;
        }
    }
}