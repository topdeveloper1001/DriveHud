using DriveHUD.Entities;
using Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Replayer
{
    public class ReplayerDataModel
    {
        private long _gameNumber;
        private bool _isActive;
        private string _gameType;
        private DateTime _time;
        private string _cards;
        private decimal _pot;
        private decimal _netWon;
        private short _pokersiteId;
        private Playerstatistic _statistic;

        public ReplayerDataModel(Playerstatistic statistic)
        {
            this.Statistic = statistic;

            this.GameNumber = statistic.GameNumber;
            this.GameType = statistic.GameType;
            this.Time = statistic.Time;
            this.Cards = statistic.Cards;
            this.Pot = statistic.Pot;
            this.NetWon = statistic.NetWon;
            this.PokersiteId = (short)statistic.PokersiteId;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ReplayerDataModel)
            {
                return (obj as ReplayerDataModel).GameNumber == GameNumber;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GameNumber.GetHashCode();
        }

        #region Properties
        public long GameNumber
        {
            get
            {
                return _gameNumber;
            }

            set
            {
                _gameNumber = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
            }
        }

        public string GameType
        {
            get
            {
                return _gameType;
            }

            set
            {
                _gameType = value;
            }
        }

        public DateTime Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
            }
        }

        public string Cards
        {
            get
            {
                return _cards;
            }

            set
            {
                _cards = value;
            }
        }

        public decimal Pot
        {
            get
            {
                return _pot;
            }

            set
            {
                _pot = value;
            }
        }

        public decimal NetWon
        {
            get
            {
                return _netWon;
            }

            set
            {
                _netWon = value;
            }
        }

        public short PokersiteId
        {
            get
            {
                return _pokersiteId;
            }

            set
            {
                _pokersiteId = value;
            }
        }

        public Playerstatistic Statistic
        {
            get
            {
                return _statistic;
            }

            set
            {
                _statistic = value;
            }
        }
        #endregion
    }
}
