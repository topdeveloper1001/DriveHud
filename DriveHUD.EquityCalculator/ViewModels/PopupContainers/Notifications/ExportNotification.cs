using DriveHUD.EquityCalculator.Models;
using HandHistories.Objects.Hand;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class ExportNotification : Confirmation
    {
        #region Fields
        private HandHistory _currentHandHistory;
        private IEnumerable<PlayerModel> _playersList;
        private IEnumerable<CardModel> _boardCards;
        #endregion

        #region Properties
        internal HandHistory CurrentHandHistory
        {
            get
            {
                return _currentHandHistory;
            }

            set
            {
                _currentHandHistory = value;
            }
        }

        internal IEnumerable<PlayerModel> PlayersList
        {
            get
            {
                return _playersList;
            }

            set
            {
                _playersList = value;
            }
        }

        internal IEnumerable<CardModel> BoardCards
        {
            get
            {
                return _boardCards;
            }

            set
            {
                _boardCards = value;
            }
        }
        #endregion

        public ExportNotification() : base() {  }
    }
}
