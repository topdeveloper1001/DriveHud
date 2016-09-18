using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Notifications
{
    public class CustomLikelihoodNotification : Confirmation
    {
        #region Fields
        private int _likelihood;
        #endregion

        #region Properties
        public int Likelihood
        {
            get { return _likelihood; }
            set { _likelihood = value; }
        }
        #endregion

        public CustomLikelihoodNotification() : base() { }
    }
}
