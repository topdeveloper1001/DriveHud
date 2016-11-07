using Model.Enums;
using ProtoBuf;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels
{
    [ProtoContract]
    public class BaseHudRangeStat : ReactiveObject, IHudRangeStat
    {
        #region Properties

        private decimal? low;

        [ProtoMember(1)]
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

        [ProtoMember(2)]
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

        [ProtoMember(3)]
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
