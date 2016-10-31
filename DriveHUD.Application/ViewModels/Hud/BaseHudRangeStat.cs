using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels
{
    public class BaseHudRangeStat : ReactiveObject, IHudRangeStat
    {
        #region Properties

        private decimal? low;

        public decimal? Low
        {
            get
            {
                return low;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref low, value);
                SwapLowHigh();
            }
        }

        private decimal? high;

        public decimal? High
        {
            get
            {
                return high;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref high, value);
                SwapLowHigh();
            }
        }

        private Stat stat;

        public Stat Stat
        {
            get
            {
                return stat;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stat, value);
            }
        }

        #endregion

        public IHudRangeStat Clone()
        {
            return (IHudRangeStat)MemberwiseClone();
        }

        private void SwapLowHigh()
        {
            if (!Low.HasValue || !High.HasValue || (Low <= High))
            {
                return;
            }

            var temp = Low;
            Low = High;
            High = temp;
        }
    }
}
