using DriveHUD.Common.Linq;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Enums;
using Model.OmahaHoleCardsAnalyzers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Model.Filters
{
    [Serializable]
    public class FilterOmahaHandGridModel : FilterBaseEntity, IFilterModel
    {
        #region Constructor
        public FilterOmahaHandGridModel()
        {
            this.Name = "Omaha Hand Grid";
            this.Type = EnumFilterModelType.FilterOmahaHandGrid;
        }

        public void Initialize()
        {
            FilterSectionHandGridCollectionInitialize();
        }
        #endregion

        #region Methods

        private void FilterSectionHandGridCollectionInitialize()
        {
            HandGridCollection = new ObservableCollection<OmahaHandGridItem>()
            {
                new OmahaHandGridItem() { Name = "Hole Card Pair - No Pair", HoleCards = OmahaHoleCards.NoPair },
                new OnePairHandGridItem() { Name = "Hole Card Pair - One Pair {0}", HoleCards = OmahaHoleCards.OnePair },
                new OmahaHandGridItem() { Name = "Hole Card Pair - Two Pairs", HoleCards = OmahaHoleCards.TwoPairs },
                new OmahaHandGridItem() { Name = "Hole Card Pair - Trips", HoleCards = OmahaHoleCards.Trips },
                new OmahaHandGridItem() { Name = "Hole Card Pair - Quads", HoleCards = OmahaHoleCards.Quads },
                new OmahaHandGridItem() { Name = "Suitedness - Rainbow", HoleCards = OmahaHoleCards.Rainbow },
                new OmahaHandGridItem() { Name = "Suitedness - Ace Suited", HoleCards = OmahaHoleCards.AceSuited },
                new OmahaHandGridItem() { Name = "Suitedness - No Ace Suited", HoleCards = OmahaHoleCards.NoAceSuited },
                new OmahaHandGridItem() { Name = "Suitedness - Double Suited", HoleCards = OmahaHoleCards.DoubleSuited },
                new WrapsAndRundownsHandGridItem() { Name = "Wraps and Rundowns - Two Card Wrap {0}", HoleCards = OmahaHoleCards.TwoCardWrap },
                new WrapsAndRundownsHandGridItem() { Name = "Wraps and Rundowns - Three Card Wrap {0}", HoleCards = OmahaHoleCards.ThreeCardWrap },
                new WrapsAndRundownsHandGridItem() { Name = "Wraps and Rundowns - Four Card Wrap {0}", HoleCards = OmahaHoleCards.FourCardWrap },
                new HoleCardStructureHandGridItem() { Name = "Hole Card Structure - {0} No. of Aces", HoleCards = OmahaHoleCards.CardStructureAces },
                new HoleCardStructureHandGridItem() { Name = "Hole Card Structure - {0} No. of Broadway's [K-T]", HoleCards = OmahaHoleCards.CardStructureBroadways },
                new HoleCardStructureHandGridItem() { Name = "Hole Card Structure - {0} No. of Mid Hands [9-6]", HoleCards = OmahaHoleCards.CardStructureMidHands },
                new HoleCardStructureHandGridItem() { Name = "Hole Card Structure - {0} No. of Low Cards [5-2]", HoleCards = OmahaHoleCards.CardStructureLowCards },
            };
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            Expression<Func<Playerstatistic, bool>> resultPredicate = null;
            var analyzers = OmahaHoleCardsAnalyzer.GetDefaultOmahaHoleCardsAnalyzers();

            if (HandGridCollection.Any(x => x.IsChecked))
            {
                var collection = HandGridCollection.Where(x => x.IsChecked);
                resultPredicate = PredicateBuilder.Create<Playerstatistic>(p => FilterHelpers.CheckOmahaHoleCards(p.Cards, collection));
            }

            return resultPredicate;
        }

        public void ResetFilter()
        {
            ResetHandGridCollection();
        }

        public override object Clone()
        {
            FilterOmahaHandGridModel model = this.DeepCloneJson();

            return model;
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterOmahaHandGridModel)
            {
                var filterToLoad = filter as FilterOmahaHandGridModel;

                ResetHandGridCollectionTo(filterToLoad.HandGridCollection);
            }
        }

        #endregion

        #region Reset Filters

        public void ResetHandGridCollection()
        {
            HandGridCollection.ForEach(x => x.IsChecked = false);
        }
        #endregion

        #region Restore Defaults

        private void ResetHandGridCollectionTo(IEnumerable<OmahaHandGridItem> collection)
        {
            foreach (var item in collection)
            {
                var cur = HandGridCollection.FirstOrDefault(x => x.Name == item.Name);
                if (cur != null)
                {
                    FillOmanaHandGridItemInfo(cur, item);
                }
            }
        }

        private void FillOmanaHandGridItemInfo(OmahaHandGridItem to, OmahaHandGridItem from)
        {
            to.IsChecked = from.IsChecked;
            if ((to is OnePairHandGridItem) && (from is OnePairHandGridItem))
            {
                (to as OnePairHandGridItem).SelectedRank = (from as OnePairHandGridItem).SelectedRank;
            }
            else if ((to is WrapsAndRundownsHandGridItem) && (from is WrapsAndRundownsHandGridItem))
            {
                (to as WrapsAndRundownsHandGridItem).SelectedGap = (from as WrapsAndRundownsHandGridItem).SelectedGap;
            }
            else if ((to is HoleCardStructureHandGridItem) && (from is HoleCardStructureHandGridItem))
            {
                (to as HoleCardStructureHandGridItem).SelectedNumber = (from as HoleCardStructureHandGridItem).SelectedNumber;
            }
        }

        #endregion

        #region Properties
        private EnumFilterModelType _type;

        private ObservableCollection<OmahaHandGridItem> _handGridCollection;

        public EnumFilterModelType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<OmahaHandGridItem> HandGridCollection
        {
            get { return _handGridCollection; }
            set { _handGridCollection = value; }
        }

        #endregion
    }

    [XmlInclude(typeof(HoleCardStructureHandGridItem))]
    [XmlInclude(typeof(WrapsAndRundownsHandGridItem))]
    [XmlInclude(typeof(OnePairHandGridItem))]
    [Serializable]
    public class OmahaHandGridItem : FilterBaseEntity
    {
        public static Action OnChanged;

        private bool _isChecked;
        private OmahaHoleCards _holeCards;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke();
            }
        }

        public OmahaHoleCards HoleCards
        {
            get
            {
                return _holeCards;
            }
            set
            {
                _holeCards = value;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [Serializable]
    public class OnePairHandGridItem : OmahaHandGridItem
    {
        private Tuple<string, string> _selectedRank;
        private List<Tuple<string, string>> _ranksList;
        
        [XmlIgnore]
        public Tuple<string, string> SelectedRank
        {
            get { return _selectedRank; }
            set
            {
                if (value?.Item1 == _selectedRank?.Item1) return;
                _selectedRank = RanksList.FirstOrDefault(x => x.Item1 == value.Item1);
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke();
            }
        }

        [XmlIgnore]
        public List<Tuple<string, string>> RanksList
        {
            get { return _ranksList; }
            set
            {
                _ranksList = value;
            }
        }

        [JsonIgnore]
        public object[] SelectedRankStub
        {
            get { return new object[] { SelectedRank.Item1, SelectedRank.Item2 }; }
            set
            {
                var item = value as object[];
                if (item != null && item.Length == 2)
                {
                    SelectedRank = RanksList.FirstOrDefault(x => x.Item1 == (item.FirstOrDefault() as string));
                }
            }
        }

        public OnePairHandGridItem()
        {
            RanksList = new List<Tuple<string, string>>();
            RanksList.Add(new Tuple<string, string>("Any", string.Empty));
            foreach (var item in HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Reverse())
            {
                RanksList.Add(new Tuple<string, string>(item, item));
            }

            SelectedRank = RanksList.First();
        }

        public override string ToString()
        {
            return String.Format(this.Name, SelectedRank.Item1);
        }
    }

    [Serializable]
    public class WrapsAndRundownsHandGridItem : OmahaHandGridItem
    {
        private Tuple<string, int> _selectedGap;
        private List<Tuple<string, int>> _gapsList;

        [XmlIgnore]
        public Tuple<string, int> SelectedGap
        {
            get { return _selectedGap; }
            set
            {
                if (value?.Item1 == _selectedGap?.Item1) return;
                _selectedGap = GapsList.FirstOrDefault(x => x.Item1 == value.Item1);
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke();
            }
        }

        [XmlIgnore]
        public List<Tuple<string, int>> GapsList
        {
            get { return _gapsList; }
            set { _gapsList = value; }
        }

        [JsonIgnore]
        public object[] SelectedGapStub
        {
            get { return new object[] { SelectedGap.Item1, SelectedGap.Item2 }; }
            set
            {
                var item = value as object[];
                if (item != null && item.Length == 2)
                {
                    SelectedGap = GapsList.FirstOrDefault(x => x.Item1 == (item.FirstOrDefault() as string));
                }
            }
        }

        public WrapsAndRundownsHandGridItem()
        {
            GapsList = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("Zero Gaps", 0),
                new Tuple<string, int>("One Gap", 1),
                new Tuple<string, int>("Two Gaps", 2),
                new Tuple<string, int>("Three Gaps", 3),
                new Tuple<string, int>("Four Gaps", 4),
            };

            SelectedGap = GapsList.FirstOrDefault();
        }

        public override string ToString()
        {
            return String.Format(this.Name, SelectedGap.Item1);
        }
    }

    [Serializable]
    public class HoleCardStructureHandGridItem : OmahaHandGridItem
    {
        private int _selectedNumber;

        public int SelectedNumber
        {
            get { return _selectedNumber; }
            set
            {
                if (value == _selectedNumber) return;
                _selectedNumber = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke();
            }
        }

        public HoleCardStructureHandGridItem()
        {
            SelectedNumber = 1;
        }

        public override string ToString()
        {
            return String.Format(this.Name, SelectedNumber);
        }
    }
}
