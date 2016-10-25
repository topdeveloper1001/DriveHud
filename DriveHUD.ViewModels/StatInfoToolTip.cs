using DriveHUD.Common.Annotations;
using DriveHUD.Common.Reflection;
using Model.Data;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.ViewModels
{
    [ProtoContract]
    public class StatInfoToolTip : INotifyPropertyChanged
    {
        private string _categoryName;

        [ProtoMember(1)]
        public string CategoryName
        {
            get
            {
                return _categoryName.ToUpper();
            }
            set
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }

        private StatInfo _categoryStat;

        [ProtoMember(2)]
        public StatInfo CategoryStat
        {
            get { return _categoryStat; }
            set
            {
                _categoryStat = value;
                OnPropertyChanged(nameof(CategoryStat));
            }
        }

        private ObservableCollection<StatInfo> _statsCollection;

        [ProtoMember(3)]
        public ObservableCollection<StatInfo> StatsCollection
        {
            get
            {
                return _statsCollection;
            }
            set
            {
                _statsCollection = value;
                OnPropertyChanged(nameof(StatsCollection));
            }
        }

        public StatInfoToolTipCardsList _cardsList;

        [ProtoMember(4)]
        public StatInfoToolTipCardsList CardsList
        {
            get { return _cardsList; }
            set
            {
                _cardsList = value;
                OnPropertyChanged(nameof(CardsList));
            }
        }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        // This is test feature so no UI is present. Implemented temporary methods to fill with data
        #region Static Methods

        public static ObservableCollection<StatInfoToolTip> GetToolTipCollection(Model.Enums.Stat stat)
        {
            switch (stat)
            {
                case Model.Enums.Stat.PFR:
                    return GetPFRToolTip();
                case Model.Enums.Stat.VPIP:
                    return GetVPIPToolTip();
                case Model.Enums.Stat.S3Bet:
                    return GetThreeBetToolTip();
                case Model.Enums.Stat.AGG:
                    return GetAggressionToolTip();
                default:
                    return null;
            }
        }

        private static ObservableCollection<StatInfoToolTip> GetVPIPToolTip()
        {
            var list = new ObservableCollection<StatInfoToolTip>();
            var vpip = new StatInfoToolTip();
            var coldCall = new StatInfoToolTip();

            vpip.CategoryName = "TOTAL";
            vpip.CategoryStat = new StatInfo() { Stat = Model.Enums.Stat.VPIP, PropertyName = nameof(HudIndicators.VPIP) };
            vpip.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_EP, PropertyName = nameof(HudIndicators.VPIP_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_MP, PropertyName = nameof(HudIndicators.VPIP_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_CO, PropertyName = nameof(HudIndicators.VPIP_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_BN, PropertyName = nameof(HudIndicators.VPIP_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_SB, PropertyName = nameof(HudIndicators.VPIP_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_BB, PropertyName = nameof(HudIndicators.VPIP_BB), StatInfoMeter = new StatInfoMeterModel() },
            };

            coldCall.CategoryName = "COLD CALL";
            coldCall.CategoryStat = new StatInfo { Stat = Model.Enums.Stat.ColdCall, PropertyName = nameof(HudIndicators.ColdCall) };
            coldCall.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_EP, PropertyName = nameof(HudIndicators.ColdCall_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_MP, PropertyName = nameof(HudIndicators.ColdCall_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_CO, PropertyName = nameof(HudIndicators.ColdCall_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_BN, PropertyName = nameof(HudIndicators.ColdCall_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_SB, PropertyName = nameof(HudIndicators.ColdCall_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_BB, PropertyName = nameof(HudIndicators.ColdCall_BB), StatInfoMeter = new StatInfoMeterModel() },
            };

            list.Add(vpip);
            list.Add(coldCall);

            return list;
        }

        private static ObservableCollection<StatInfoToolTip> GetPFRToolTip()
        {
            var list = new ObservableCollection<StatInfoToolTip>();
            var pfr = new StatInfoToolTip();

            pfr.CategoryName = "UNOPENED";
            pfr.CategoryStat = new StatInfo() { Stat = Model.Enums.Stat.PFR, PropertyName = nameof(HudIndicators.PFR) };
            pfr.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_EP, PropertyName = nameof(HudIndicators.UO_PFR_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_MP, PropertyName = nameof(HudIndicators.UO_PFR_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_CO, PropertyName = nameof(HudIndicators.UO_PFR_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_BN, PropertyName = nameof(HudIndicators.UO_PFR_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_SB, PropertyName = nameof(HudIndicators.UO_PFR_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_BB, PropertyName = nameof(HudIndicators.UO_PFR_BB), StatInfoMeter = new StatInfoMeterModel() },
            };

            list.Add(pfr);

            return list;
        }

        private static ObservableCollection<StatInfoToolTip> GetThreeBetToolTip()
        {
            var list = new ObservableCollection<StatInfoToolTip>();
            var threeBet = new StatInfoToolTip();

            threeBet.CategoryName = "TOTAL";
            threeBet.CategoryStat = new StatInfo() { Stat = Model.Enums.Stat.S3Bet, PropertyName = nameof(HudIndicators.ThreeBet) };
            threeBet.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.ThreeBet_EP, PropertyName = nameof(HudIndicators.ThreeBet_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ThreeBet_MP, PropertyName = nameof(HudIndicators.ThreeBet_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ThreeBet_CO, PropertyName = nameof(HudIndicators.ThreeBet_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ThreeBet_BN, PropertyName = nameof(HudIndicators.ThreeBet_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ThreeBet_SB, PropertyName = nameof(HudIndicators.ThreeBet_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.ThreeBet_BB, PropertyName = nameof(HudIndicators.ThreeBet_BB), StatInfoMeter = new StatInfoMeterModel() },
            };

            threeBet.CardsList = new StatInfoToolTipCardsList() { ListSize = 4, PropertyName = nameof(HudIndicators.ThreeBetCardsList) };

            list.Add(threeBet);

            return list;
        }

        private static ObservableCollection<StatInfoToolTip> GetAggressionToolTip()
        {
            var list = new ObservableCollection<StatInfoToolTip>();
            var aggPr = new StatInfoToolTip();

            aggPr.CategoryName = "TOTAL";
            aggPr.CategoryStat = new StatInfo() { Stat = Model.Enums.Stat.AGG, PropertyName = nameof(HudIndicators.AggPr) };

            var recentAggMeterColor = new StatInfoMeterModel();
            recentAggMeterColor.UpdateBackgroundBrushes("#FF28F0DD", "#FF28C3F0", "#FF289EF0", "#FF2868F0", "#FF283AF0", "#FF3812E4", "#FF3812E4", "#FF7B12E4", "#FFD112E4", "#FFE412A1");
            recentAggMeterColor.UpdateBorderBrushes("#FF59FDED", "#FF48D9F0", "#FF48ABF0", "#FF4885F0", "#FF4857F0", "#FF5B2DF5", "#FF5B2DF5", "#FF882DF5", "#FFDA2DF5", "#FFF52DD1");

            aggPr.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.FlopAGG, PropertyName = nameof(HudIndicators.FlopAgg), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.TurnAGG, PropertyName = nameof(HudIndicators.TurnAgg), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.RiverAGG, PropertyName = nameof(HudIndicators.RiverAgg), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Model.Enums.Stat.RecentAgg, PropertyName = nameof(HudIndicators.RecentAggPr), StatInfoMeter = recentAggMeterColor },
            };

            list.Add(aggPr);

            return list;
        }

        #endregion
    }

    [ProtoContract]
    public class StatInfoToolTipCardsList : INotifyPropertyChanged
    {
        [ProtoMember(1)]
        public int ListSize { get; set; }

        [ProtoMember(2)]
        public string PropertyName { get; set; }

        [ProtoMember(3)]
        public ObservableCollection<string> Cards { get; set; }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

}
