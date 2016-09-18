using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class CalculateBluffNotification : Confirmation
    {
        #region Fields
        private double _equityValue = 0;
        private int _numberOfPlayers = 0;
        #endregion

        #region Properties
        internal double EquityValue
        {
            get
            {
                return _equityValue;
            }

            set
            {
                _equityValue = value;
            }
        }

        internal int NumberOfPlayers
        {
            get
            {
                return _numberOfPlayers;
            }
            set
            {
                _numberOfPlayers = value;
            }
        }
        #endregion

        public CalculateBluffNotification() : base() { }

    }
}
