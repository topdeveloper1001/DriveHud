using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Initializing info of poker client data manager
    /// </summary>
    public class PokerClientDataManagerInfo
    {
        public IPokerClientEncryptedLogger Logger
        {
            get;
            set;
        }

        public string Site
        {
            get;
            set;
        }
    }
}
