using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Notifications
{
    public class PreDefinedRangesNotifcation : Confirmation
    {
        #region  Fields
        private IEnumerable<string> _itemsList;
        #endregion

        #region Properties
        public IEnumerable<string> ItemsList
        {
            get { return _itemsList; }
            set { _itemsList = value; }
        }

        #endregion

        public PreDefinedRangesNotifcation() : base() { }
    }
}
