using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    public class TournamentDescriptor
    {
        public string TournamentId { get; set; }

        public string TournamentInGameId { get; set; }

        public string TournamentName { get; set; }

        public string Summary { get; set; }

        public Buyin BuyIn { get; set; }

        public decimal Bounty { get; set; }

        public decimal Rebuy { get; set; }

        public decimal Addon { get; set; }

        public decimal Winning { get; set; }

        public int FinishPosition { get; set; }

        public int TotalPlayers { get; set; }

        public DateTime StartDate { get; set; }

        public TournamentSpeed Speed { get; set; }

        public bool IsSummary
        {
            get
            {
                return !string.IsNullOrEmpty(Summary);
            }
        }
    }
}