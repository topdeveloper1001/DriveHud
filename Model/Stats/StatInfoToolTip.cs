//-----------------------------------------------------------------------
// <copyright file="StatInfoToolTip.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Annotations;
using Model.Data;
using Model.Enums;
using ProtoBuf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Model.Stats
{
    [Obsolete]
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

        public static ObservableCollection<StatInfoToolTip> GetToolTipCollection(Stat stat)
        {
            switch (stat)
            {
                case Stat.PFR:
                    return GetPFRToolTip();
                case Stat.VPIP:
                    return GetVPIPToolTip();
                case Stat.S3Bet:
                    return GetThreeBetToolTip();
                case Stat.AGG:
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
            vpip.CategoryStat = new StatInfo() { Stat = Stat.VPIP, PropertyName = nameof(Indicators.VPIP) };
            vpip.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Stat.VPIP_EP, PropertyName = nameof(Indicators.VPIP_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.VPIP_MP, PropertyName = nameof(Indicators.VPIP_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.VPIP_CO, PropertyName = nameof(Indicators.VPIP_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.VPIP_BN, PropertyName = nameof(Indicators.VPIP_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.VPIP_SB, PropertyName = nameof(Indicators.VPIP_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.VPIP_BB, PropertyName = nameof(Indicators.VPIP_BB), StatInfoMeter = new StatInfoMeterModel() },
            };

            coldCall.CategoryName = "COLD CALL";
            coldCall.CategoryStat = new StatInfo { Stat = Stat.ColdCall, PropertyName = nameof(Indicators.ColdCall) };
            coldCall.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat =  Stat.ColdCall_EP, PropertyName = nameof(Indicators.ColdCall_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ColdCall_MP, PropertyName = nameof(Indicators.ColdCall_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ColdCall_CO, PropertyName = nameof(Indicators.ColdCall_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ColdCall_BN, PropertyName = nameof(Indicators.ColdCall_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ColdCall_SB, PropertyName = nameof(Indicators.ColdCall_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ColdCall_BB, PropertyName = nameof(Indicators.ColdCall_BB), StatInfoMeter = new StatInfoMeterModel() },
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
            pfr.CategoryStat = new StatInfo() { Stat = Stat.PFR, PropertyName = nameof(Indicators.PFR) };
            pfr.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Stat.UO_PFR_EP, PropertyName = nameof(Indicators.UO_PFR_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.UO_PFR_MP, PropertyName = nameof(Indicators.UO_PFR_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.UO_PFR_CO, PropertyName = nameof(Indicators.UO_PFR_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.UO_PFR_BN, PropertyName = nameof(Indicators.UO_PFR_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.UO_PFR_SB, PropertyName = nameof(Indicators.UO_PFR_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.UO_PFR_BB, PropertyName = nameof(Indicators.UO_PFR_BB), StatInfoMeter = new StatInfoMeterModel() },
            };

            list.Add(pfr);

            return list;
        }

        private static ObservableCollection<StatInfoToolTip> GetThreeBetToolTip()
        {
            var list = new ObservableCollection<StatInfoToolTip>();
            var threeBet = new StatInfoToolTip();

            threeBet.CategoryName = "TOTAL";
            threeBet.CategoryStat = new StatInfo() { Stat = Stat.S3Bet, PropertyName = nameof(Indicators.ThreeBet) };
            threeBet.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Stat.ThreeBet_EP, PropertyName = nameof(Indicators.ThreeBet_EP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ThreeBet_MP, PropertyName = nameof(Indicators.ThreeBet_MP), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ThreeBet_CO, PropertyName = nameof(Indicators.ThreeBet_CO), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ThreeBet_BN, PropertyName = nameof(Indicators.ThreeBet_BN), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ThreeBet_SB, PropertyName = nameof(Indicators.ThreeBet_SB), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.ThreeBet_BB, PropertyName = nameof(Indicators.ThreeBet_BB), StatInfoMeter = new StatInfoMeterModel() },
            };

            threeBet.CardsList = new StatInfoToolTipCardsList() { ListSize = 4, PropertyName = nameof(HudLightIndicators.ThreeBetCardsList) };

            list.Add(threeBet);

            return list;
        }

        private static ObservableCollection<StatInfoToolTip> GetAggressionToolTip()
        {
            var list = new ObservableCollection<StatInfoToolTip>();
            var aggPr = new StatInfoToolTip();

            aggPr.CategoryName = "TOTAL";
            aggPr.CategoryStat = new StatInfo() { Stat = Stat.AGG, PropertyName = nameof(Indicators.AggPr) };

            var recentAggMeterColor = new StatInfoMeterModel();
            recentAggMeterColor.UpdateBackgroundBrushes("#FF28F0DD", "#FF28C3F0", "#FF289EF0", "#FF2868F0", "#FF283AF0", "#FF3812E4", "#FF3812E4", "#FF7B12E4", "#FFD112E4", "#FFE412A1");
            recentAggMeterColor.UpdateBorderBrushes("#FF59FDED", "#FF48D9F0", "#FF48ABF0", "#FF4885F0", "#FF4857F0", "#FF5B2DF5", "#FF5B2DF5", "#FF882DF5", "#FFDA2DF5", "#FFF52DD1");

            aggPr.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Stat.FlopAGG, PropertyName = nameof(Indicators.FlopAgg), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.TurnAGG, PropertyName = nameof(Indicators.TurnAgg), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.RiverAGG, PropertyName = nameof(Indicators.RiverAgg), StatInfoMeter = new StatInfoMeterModel() },
                new StatInfo() { Stat = Stat.RecentAgg, PropertyName = nameof(HudLightIndicators.RecentAggPr), StatInfoMeter = recentAggMeterColor },
            };

            list.Add(aggPr);

            return list;
        }

        #endregion

        public StatInfoToolTip Clone()
        {
            return new StatInfoToolTip
            {
                CategoryName = CategoryName,
                CategoryStat = CategoryStat,
                StatsCollection =
                               new ObservableCollection<StatInfo>(
                                   StatsCollection.Select(x => x.Clone()).ToList()),
                CardsList = CardsList.Clone()
            };
        }
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

        public StatInfoToolTipCardsList Clone()
        {
            return new StatInfoToolTipCardsList
            {
                ListSize = ListSize,
                PropertyName = PropertyName,
                Cards =
                               new ObservableCollection<string>(Cards.Select(x => x.Clone().ToString()).ToList())
            };
        }
    }

}
