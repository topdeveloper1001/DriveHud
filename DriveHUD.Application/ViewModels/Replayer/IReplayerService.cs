using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Replayer
{
    interface IReplayerService
    {
        void ReplayHand(string playerName, long gamenumber, short pokerSiteId, bool showHoleCards);
        void ReplayHand(Playerstatistic currentStat, IList<Playerstatistic> statistics, bool showHoleCards);
    }
}
