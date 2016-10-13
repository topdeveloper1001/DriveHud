using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public interface IHudRangeStat
    {
        decimal? Low { get; set; }
        decimal? High { get; set; }
        Stat Stat { get; set; }

        IHudRangeStat Clone();
    }
}
