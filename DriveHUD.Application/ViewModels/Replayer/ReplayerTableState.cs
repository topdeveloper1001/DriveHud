using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using Model.Importer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Replayer
{
    internal class ReplayerTableState
    {
        internal void UpdatePlayerState(HandAction action)
        {
            ActivePlayer.IsActive = true;
            ActivePlayer.ActionString = Converter.ActionToString(action.HandActionType);
            ActivePlayer.OldBank = ActivePlayer.Bank;
            ActivePlayer.OldAmount = ActivePlayer.ActiveAmount;

            if (ActivePlayer.CurrentStreet != action.Street)
            {
                ActivePlayer.ActiveAmount = 0;
            }

            ActivePlayer.CurrentStreet = action.Street;

            if (IsWinAction(action))
            {
                ActivePlayer.IsWin = true;
                ActivePlayer.ActiveAmount = Math.Abs(action.Amount);
                ActivePlayer.Bank += Math.Abs(action.Amount);
            }
            else
            {
                ActivePlayer.ActiveAmount += Math.Abs(action.Amount);
                ActivePlayer.Bank -= Math.Abs(action.Amount);
            }

            if (action.HandActionType == HandActionType.FOLD)
            {
                ActivePlayer.IsFinished = true;
            }
            else if (!string.IsNullOrWhiteSpace(ActivePlayer.ActionString) && action.HandActionType != HandActionType.CHECK)
            {
                ActivePlayer.ActionString += string.Format(" {0:C2}", Math.Abs(action.Amount));
            }
        }

        private bool IsWinAction(HandAction action)
        {
            return action.HandActionType == HandActionType.WINS
                || action.HandActionType == HandActionType.WINS_SIDE_POT
                || action.HandActionType == HandActionType.WINS_THE_LOW
                || action.HandActionType == HandActionType.WINS_TOURNAMENT;
        }

        #region Properties
        private Street _currentStreet;
        private ReplayerPlayerViewModel _activePlayer;
        private bool _isStreetChangedAction;
        private decimal _actionAmount;
        private decimal _currentPotValue;
        private decimal _totalPotValue;

        internal Street CurrentStreet
        {
            get { return _currentStreet; }
            set { _currentStreet = value; }
        }

        internal ReplayerPlayerViewModel ActivePlayer
        {
            get { return _activePlayer; }
            set { _activePlayer = value; }
        }

        internal bool IsStreetChangedAction
        {
            get { return _isStreetChangedAction; }
            set { _isStreetChangedAction = value; }
        }

        internal decimal ActionAmount
        {
            get { return _actionAmount; }
            set { _actionAmount = value; }
        }

        internal decimal CurrentPotValue
        {
            get { return _currentPotValue; }
            set { _currentPotValue = value; }
        }

        internal decimal TotalPotValue
        {
            get { return _totalPotValue; }
            set { _totalPotValue = value; }
        }
        #endregion

    }
}
