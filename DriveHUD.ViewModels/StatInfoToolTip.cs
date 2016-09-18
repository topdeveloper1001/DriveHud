using DriveHUD.Common.Annotations;
using DriveHUD.Common.Reflection;
using Model.Data;
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
    public class StatInfoToolTip : INotifyPropertyChanged
    {
        private string _categoryName;

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

        private ObservableCollection<StatInfo> _statsCollection;

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
            vpip.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_EP, PropertyName = nameof(HudIndicators.VPIP_EP) },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_MP, PropertyName = nameof(HudIndicators.VPIP_MP) },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_CO, PropertyName = nameof(HudIndicators.VPIP_CO) },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_BN, PropertyName = nameof(HudIndicators.VPIP_BN) },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_SB, PropertyName = nameof(HudIndicators.VPIP_SB) },
                new StatInfo() { Stat = Model.Enums.Stat.VPIP_BB, PropertyName = nameof(HudIndicators.VPIP_BB) },
            };

            coldCall.CategoryName = "COLD CALL";
            coldCall.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_EP, PropertyName = nameof(HudIndicators.ColdCall_EP) },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_MP, PropertyName = nameof(HudIndicators.ColdCall_MP) },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_CO, PropertyName = nameof(HudIndicators.ColdCall_CO) },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_BN, PropertyName = nameof(HudIndicators.ColdCall_BN) },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_SB, PropertyName = nameof(HudIndicators.ColdCall_SB) },
                new StatInfo() { Stat = Model.Enums.Stat.ColdCall_BB, PropertyName = nameof(HudIndicators.ColdCall_BB) },
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
            pfr.StatsCollection = new ObservableCollection<StatInfo>()
            {
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_EP, PropertyName = nameof(HudIndicators.UO_PFR_EP) },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_MP, PropertyName = nameof(HudIndicators.UO_PFR_MP) },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_CO, PropertyName = nameof(HudIndicators.UO_PFR_CO) },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_BN, PropertyName = nameof(HudIndicators.UO_PFR_BN) },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_SB, PropertyName = nameof(HudIndicators.UO_PFR_SB) },
                new StatInfo() { Stat = Model.Enums.Stat.UO_PFR_BB, PropertyName = nameof(HudIndicators.UO_PFR_BB) },
            };

            list.Add(pfr);

            return list;
        }

        #endregion
    }
}
