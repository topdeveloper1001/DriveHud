using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;

namespace DriveHUD.ViewModels
{
    [ProtoContract]
    [Serializable]
    public class StatInfoBreak : StatInfo
    {
        public StatInfoBreak() : base()
        {
            Stat = Stat.PlayerInfoIcon;
        }

        public override StatInfo Clone()
        {
            return new StatInfoBreak();
        }
    }
}