using DriveHUD.Common.Linq;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Filters;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;
using DriveHUD.Entities;
using System.Linq.Expressions;
using DriveHUD.Common.Utils;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Bumper sticker type 
    /// </summary>
    [Serializable]
    public class HudBumperStickerType : ReactiveObject
    {
        private const int MinSampleDefault = 15;
        private readonly Color DefaultColor = Colors.OrangeRed;
        private readonly string[] StringsToExcludeFromLabels = new string[] { "and", "&" };

        public HudBumperStickerType()
        {
        }

        public HudBumperStickerType(bool initialize) : this()
        {
            if (!initialize)
            {
                return;
            }

            MinSample = MinSampleDefault;
            EnableBumperSticker = true;
            SelectedColor = DefaultColor;

            stats = new ObservableCollection<BaseHudRangeStat>()
            {
                new BaseHudRangeStat { Stat = Stat.VPIP },
                new BaseHudRangeStat { Stat = Stat.PFR },
                new BaseHudRangeStat { Stat = Stat.S3Bet },
                new BaseHudRangeStat { Stat = Stat.AGG },
                new BaseHudRangeStat { Stat = Stat.CBet },
                new BaseHudRangeStat { Stat = Stat.WWSF },
                new BaseHudRangeStat { Stat = Stat.WTSD },
                new BaseHudRangeStat { Stat = Stat.FoldTo3Bet },
                new BaseHudRangeStat { Stat = Stat.DoubleBarrel },
                new BaseHudRangeStat { Stat = Stat.CheckRaise },
                new BaseHudRangeStat { Stat = Stat.UO_PFR_EP },
            };
        }

        #region Properties

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
                Label = GenerateLabel(Name);
                UpdateToolTip();
            }
        }

        private string label;

        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref label, value);
            }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
                UpdateToolTip();
            }
        }

        private string toolTip;

        public string ToolTip
        {
            get { return toolTip; }
            set
            {
                this.RaiseAndSetIfChanged(ref toolTip, value);
            }
        }

        private bool enableBumperSticker;

        public bool EnableBumperSticker
        {
            get
            {
                return enableBumperSticker;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref enableBumperSticker, value);
            }
        }

        private int minSample;

        public int MinSample
        {
            get
            {
                return minSample;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref minSample, value);
            }
        }

        private Color selectedColor;

        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedColor, value);
            }
        }

        private ObservableCollection<BaseHudRangeStat> stats;

        public ObservableCollection<BaseHudRangeStat> Stats
        {
            get
            {
                return stats;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stats, value);
            }
        }

        private BuiltFilterModel builtFilter;

        public BuiltFilterModel BuiltFilter
        {
            get { return builtFilter; }
            set
            {
                this.RaiseAndSetIfChanged(ref builtFilter, value);
            }
        }

        private IFilterModelCollection filterModelCollection;

        public IFilterModelCollection FilterModelCollection
        {
            get { return filterModelCollection; }
            set
            {
                this.RaiseAndSetIfChanged(ref filterModelCollection, value);
            }
        }

        [XmlIgnore]
        public Expression<Func<Playerstatistic, bool>> FilterPredicate { get; internal set; } = null;

        /// <summary>
        /// Property is used to initialize massive arrays of HudPlayerType with defaults stat list without creation empty stats
        /// Only stats which needs to be initialized with data must be set 
        /// </summary>
        public IEnumerable<IHudRangeStat> StatsToMerge
        {
            set
            {
                if (stats == null || value == null)
                {
                    return;
                }

                var statsToMerge = (from stat in stats
                                    join statToMerge in value on stat.Stat equals statToMerge.Stat into gj
                                    from grouped in gj.DefaultIfEmpty()
                                    where grouped != null
                                    select new { OldStat = stat, NewStat = grouped }).ToArray();

                statsToMerge.ForEach(x =>
                {
                    x.OldStat.Low = x.NewStat.Low;
                    x.OldStat.High = x.NewStat.High;
                });
            }
        }

        #endregion

        private string GenerateLabel(string name)
        {
            var labels = name.Split(new char[] { ' ', '-' })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Except(StringsToExcludeFromLabels, StringComparer.CurrentCultureIgnoreCase)
                .Select(x => x.Trim().ToUpper().FirstOrDefault());
            return string.Join(string.Empty, labels);
        }

        private void UpdateToolTip()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                ToolTip = Name;
            }
            else
            {
                ToolTip = $"{Name} | {Description}";
            }
        }

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Cloned object</returns>
        public HudBumperStickerType Clone()
        {
            var clone = (HudBumperStickerType)MemberwiseClone();
            clone.Stats = new ObservableCollection<BaseHudRangeStat>(clone.Stats.Select(x => (BaseHudRangeStat)x.Clone()));
            clone.FilterModelCollection = FilterModelCollection != null && FilterModelCollection.Any()
                ? new IFilterModelCollection(FilterModelCollection?.Select(x => (IFilterModel)x.Clone()))
                : new IFilterModelCollection();

            return clone;
        }

        internal void InitializeFilterPredicate()
        {
            FilterPredicate = PredicateBuilder.True<Playerstatistic>();

            if (FilterModelCollection == null)
            {
                return;
            }

            foreach (var filter in FilterModelCollection)
            {
                var filterPredicate = filter.GetFilterPredicate();
                if (filterPredicate != null)
                {
                    FilterPredicate = FilterPredicate.And(filterPredicate);
                }
            }
        }
    }
}
