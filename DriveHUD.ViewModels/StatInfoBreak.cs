using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class StatInfoBreak : StatInfo
    {
        public StatInfoBreak() : base()
        {         
        }

        public new StatInfoBreak Clone()
        {
            return new StatInfoBreak();
        }
    }
}