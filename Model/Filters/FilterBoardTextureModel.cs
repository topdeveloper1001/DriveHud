using DriveHUD.Common.Linq;
using DriveHUD.Common.Utils;
using HandHistories.Objects.Cards;
using Model.BoardTextureAnalyzers;
using DriveHUD.Entities;
using Model.Enums;
using Model.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Model.Filters
{
    [Serializable]
    public class FilterBoardTextureModel : FilterBaseEntity, IFilterModel
    {
        #region Constructor
        public FilterBoardTextureModel()
        {
            this.Name = "Board Texture";
            this.Type = EnumFilterModelType.FilterBoardTextureModel;

            FilterSectionCardItemsCollectionsInitialize();
            FilterSectionFlopBoardTextureCollectionInitialize();
            FilterSectionTurnBoardTextureCollectionInitialize();
            FilterSectionRiverBoardTextureCollectionInitialize();
        }
        #endregion

        #region Methods
        private void FilterSectionCardItemsCollectionsInitialize()
        {
            FlopCardItemsCollection = new ObservableCollection<BoardCardItem>();
            TurnCardItemsCollection = new ObservableCollection<BoardCardItem>();
            RiverCardItemsCollection = new ObservableCollection<BoardCardItem>();

            for (int i = 0; i < 5; i++)
            {
                FlopCardItemsCollection.Add(new BoardCardItem()
                {
                    Rank = RangeCardRank.None,
                    Suit = RangeCardSuit.None,
                    TargetStreet = Street.Flop,
                    IsEnabled = FlopCardItemsCollection.Count < 3
                });
                TurnCardItemsCollection.Add(new BoardCardItem()
                {
                    Rank = RangeCardRank.Two,
                    Suit = RangeCardSuit.None,
                    TargetStreet = Street.Turn,
                    IsEnabled = TurnCardItemsCollection.Count < 4
                });
                RiverCardItemsCollection.Add(new BoardCardItem()
                {
                    Rank = RangeCardRank.None,
                    Suit = RangeCardSuit.None,
                    TargetStreet = Street.River,
                    IsEnabled = RiverCardItemsCollection.Count < 5
                });
            }
        }

        private void FilterSectionFlopBoardTextureCollectionInitialize()
        {
            FlopBoardTextureCollection = new ObservableCollection<BoardTextureItem>()
            {
                new HighCardBoardTextureItem() { Name = "Highcard of {0} So the highest card on flop is whatever is selected", TargetStreet = Street.Flop, BoardTexture = BoardTextures.HighCard },
                new BoardTextureItem() { Name = "Rainbow [3 different suits]", TargetStreet = Street.Flop, BoardTexture = BoardTextures.Rainbow },
                new BoardTextureItem() { Name = "Two-Tone [2 of the same suit]", TargetStreet = Street.Flop, BoardTexture = BoardTextures.TwoTone },
                new BoardTextureItem() { Name = "Monotone [All 3 of the same suit]", TargetStreet = Street.Flop, BoardTexture = BoardTextures.Monotone },
                new BoardTextureItem() { Name = "Uncoordinated [No connected cards]", TargetStreet = Street.Flop, BoardTexture = BoardTextures.Uncoordinated },
                new BoardTextureItem() { Name = "1 Gapper [Semi-coordinated with 1 gap between 2 cards. EX: 6x8x]", TargetStreet = Street.Flop, BoardTexture = BoardTextures.OneGapper },
                new BoardTextureItem() { Name = "2 Gapper [Less Coordinated with 2 gaps between 2 cards. EX: 5x8x]", TargetStreet = Street.Flop, BoardTexture = BoardTextures.TwoGapper },
                new StraightBoardTextureItem() { Name = "Open Ended {0} # of open ended straights {1}", TargetStreet = Street.Flop, BoardTexture = BoardTextures.OpenEndedStraight },
                new StraightBoardTextureItem() { Name = "Made Straight {0} # of made straights {1}", TargetStreet = Street.Flop, BoardTexture = BoardTextures.MadeStraight },
                new StraightBoardTextureItem() { Name = "Open Ended that Beat Nuts {0} # of open ended straights that beat the Nuts {1}", TargetStreet = Street.Flop, BoardTexture = BoardTextures.OpenEndedBeatNuts },
                new StraightBoardTextureItem() { Name = "GutShot that Beats Nuts {0} # of gutshot straights that beat the Nuts {1}", TargetStreet = Street.Flop, BoardTexture = BoardTextures.GutShotBeatNuts },
            };
        }

        private void FilterSectionTurnBoardTextureCollectionInitialize()
        {
            TurnBoardTextureCollection = new ObservableCollection<BoardTextureItem>()
            {
                new HighCardBoardTextureItem() { Name = "Highcard of {0} So the highest card on turn is whatever is selected", TargetStreet = Street.Turn, BoardTexture = BoardTextures.HighCard },
                new BoardTextureItem() { Name = "Rainbow [3 different suits]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.Rainbow },
                new BoardTextureItem() { Name = "Two Flush Draws [2 sets of 2 suits]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.TwoFlushDraws },
                new BoardTextureItem() { Name = "Two-Tone [2 of the same suit]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.TwoTone },
                new BoardTextureItem() { Name = "Three-Tone [3 of the same suit]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.ThreeTone },
                new BoardTextureItem() { Name = "Monotone [All 4 of the same suit]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.Monotone },
                new BoardTextureItem() { Name = "Uncoordinated [No connected cards]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.Uncoordinated },
                new BoardTextureItem() { Name = "1 Gapper [Semi-coordinated with 1 gap between 2 cards. EX: 6x8x]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.OneGapper },
                new BoardTextureItem() { Name = "2 Gapper [Less Coordinated with 2 gaps between 2 cards. EX: 5x8x]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.TwoGapper },
                new StraightBoardTextureItem() { Name = "Open Ended {0} # of open ended straights {1}", TargetStreet = Street.Turn, BoardTexture = BoardTextures.OpenEndedStraight },
                new BoardTextureItem() { Name = "Three connected cards [Turn creates 3 connected card. EX: 7x8x9x]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.ThreeConnected },
                new BoardTextureItem() { Name = "Four connected cards [Turn creates 4 connected cards. EX: 7x8x9xTx]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.FourConnected },
                new StraightBoardTextureItem() { Name = "Made Straight {0} # of made straights {1}", TargetStreet = Street.Turn, BoardTexture = BoardTextures.MadeStraight },
                new StraightBoardTextureItem() { Name = "Open Ended that Beat Nuts {0} # of open ended straights that beat the Nuts {1}", TargetStreet = Street.Turn, BoardTexture = BoardTextures.OpenEndedBeatNuts },
                new StraightBoardTextureItem() { Name = "GutShot that Beats Nuts {0} # of gutshot straights that beat the Nuts {1}", TargetStreet = Street.Turn, BoardTexture = BoardTextures.GutShotBeatNuts },
                new BoardTextureItem() { Name = "No Pair [Turn contains no paired cards]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.NoPair },
                new BoardTextureItem() { Name = "Single pair [Turn pairs a flop card]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.SinglePair },
                new BoardTextureItem() { Name = "Two Pair [Turn brings a second pair to the flop]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.TwoPair },
                new BoardTextureItem() { Name = "Three of a Kind [Turn creates 3 of a kind]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.ThreeOfAKind },
                new BoardTextureItem() { Name = "Four of a Kind [Turn creates quads]", TargetStreet = Street.Turn, BoardTexture = BoardTextures.FourOfAKind },
            };
        }

        private void FilterSectionRiverBoardTextureCollectionInitialize()
        {
            RiverBoardTextureCollection = new ObservableCollection<BoardTextureItem>()
            {
                new HighCardBoardTextureItem() { Name = "Highcard of {0} So the highest card on river is whatever is selected", TargetStreet = Street.River, BoardTexture = BoardTextures.HighCard },
                new BoardTextureItem() { Name = "No Flush Possible", TargetStreet = Street.River, BoardTexture = BoardTextures.NoFlushPossible },
                new BoardTextureItem() { Name = "Flush Possible [3 of the same suit]", TargetStreet = Street.River, BoardTexture = BoardTextures.FlushPossible },
                new BoardTextureItem() { Name = "Four Flush [4 of the same suit]", TargetStreet = Street.River, BoardTexture = BoardTextures.FourFlush },
                new BoardTextureItem() { Name = "Flush on Board [All 5 of the same suit]", TargetStreet = Street.River, BoardTexture = BoardTextures.FlushOnBoard },
                new BoardTextureItem() { Name = "Uncoordinated [No connected cards]", TargetStreet = Street.River, BoardTexture = BoardTextures.Uncoordinated },
                new BoardTextureItem() { Name = "1 Gapper [Semi-coordinated with 1 gap between 2 cards. EX: 6x8x]", TargetStreet = Street.River, BoardTexture = BoardTextures.OneGapper },
                new BoardTextureItem() { Name = "2 Gapper [Less Coordinated with 2 gaps between 2 cards. EX: 5x8x]", TargetStreet = Street.River, BoardTexture = BoardTextures.TwoGapper },
                new BoardTextureItem() { Name = "Three connected cards [River creates 3 connected card. EX: 7x8x9x]", TargetStreet = Street.River, BoardTexture = BoardTextures.ThreeConnected },
                new BoardTextureItem() { Name = "Four connected cards [River creates 4 connected cards. EX: 7x8x9xTx]", TargetStreet = Street.River, BoardTexture = BoardTextures.FourConnected },
                new StraightBoardTextureItem() { Name = "Made Straight {0} # of made straights {1}", TargetStreet = Street.River, BoardTexture = BoardTextures.MadeStraight },
                new BoardTextureItem() { Name = "No Pair [River contains no paired cards]", TargetStreet = Street.River, BoardTexture = BoardTextures.NoPair },
                new BoardTextureItem() { Name = "Single pair [River pairs a flop or turn card]", TargetStreet = Street.River, BoardTexture = BoardTextures.SinglePair },
                new BoardTextureItem() { Name = "Two Pair [River brings a second pair to the flop or turn]", TargetStreet = Street.River, BoardTexture = BoardTextures.TwoPair },
                new BoardTextureItem() { Name = "Three of a Kind [River creates 3 of a kind]", TargetStreet = Street.River, BoardTexture = BoardTextures.ThreeOfAKind },
                new BoardTextureItem() { Name = "Four of a Kind [River creates quads]", TargetStreet = Street.River, BoardTexture = BoardTextures.FourOfAKind },
                new BoardTextureItem() { Name = "Full House [River brings full house]", TargetStreet = Street.River, BoardTexture = BoardTextures.FullHouse },
            };
        }

        public IEnumerable<BoardCardItem> GetCardItemsCollectionForStreet(Street street)
        {
            switch (street)
            {
                case Street.Flop:
                    return FlopCardItemsCollection;
                case Street.Turn:
                    return TurnCardItemsCollection;
                case Street.River:
                    return RiverCardItemsCollection;
            }
            return null;
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            var cardItemsPredicate = GetBoardCardItemsPredicate();
            var boardTexturePredicate = GetBoardTexturePredicate();

            if (cardItemsPredicate == null && boardTexturePredicate == null)
            {
                return null;
            }

            return (cardItemsPredicate == null ? PredicateBuilder.False<Playerstatistic>() : cardItemsPredicate)
                .Or(boardTexturePredicate == null ? PredicateBuilder.False<Playerstatistic>() : boardTexturePredicate);
        }

        public void ResetFilter()
        {
            ResetFlopCardItemsCollection();
            ResetTurnCardItemsCollection();
            ResetRiverCardItemsCollection();

            ResetFlopBoardTextureCollection();
            ResetTurnBoardTextureCollection();
            ResetRiverBoardTextureCollection();
        }

        public override object Clone()
        {
            FilterBoardTextureModel model = this.DeepCloneJson();

            return model;
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterBoardTextureModel)
            {
                var filterToLoad = filter as FilterBoardTextureModel;

                ResetFlopCardsTo(filterToLoad.FlopCardItemsCollection);
                ResetTurnCardsTo(filterToLoad.TurnCardItemsCollection);
                ResetRiverCardsTo(filterToLoad.RiverCardItemsCollection);

                ResetFlopBoardTextureTo(filterToLoad.FlopBoardTextureCollection);
                ResetTurnBoardTextureTo(filterToLoad.TurnBoardTextureCollection);
                ResetRiverBoardTextureTo(filterToLoad.RiverBoardTextureCollection);
            }
        }
        #endregion

        #region Reset Filters
        public void ResetFlopCardItemsCollection()
        {
            FlopCardItemsCollection.ForEach(x => x.Reset());
        }

        public void ResetTurnCardItemsCollection()
        {
            TurnCardItemsCollection.ForEach(x => x.Reset());
        }

        public void ResetRiverCardItemsCollection()
        {
            RiverCardItemsCollection.ForEach(x => x.Reset());
        }

        public void ResetFlopBoardTextureCollection()
        {
            FlopBoardTextureCollection.ForEach(x => x.IsChecked = false);
        }

        public void ResetTurnBoardTextureCollection()
        {
            TurnBoardTextureCollection.ForEach(x => x.IsChecked = false);
        }

        public void ResetRiverBoardTextureCollection()
        {
            RiverBoardTextureCollection.ForEach(x => x.IsChecked = false);
        }
        #endregion

        #region Restore Defaults
        private void ResetCardsCollectionTo(IEnumerable<BoardCardItem> cardList, IEnumerable<BoardCardItem> collection)
        {
            for (int i = 0; i < cardList.Count(); i++)
            {
                collection.ElementAt(i).Rank = cardList.ElementAt(i).Rank;
                collection.ElementAt(i).Suit = cardList.ElementAt(i).Suit;
            }
        }

        private void ResetFlopCardsTo(IEnumerable<BoardCardItem> cardList)
        {
            ResetCardsCollectionTo(cardList, FlopCardItemsCollection);
        }

        private void ResetTurnCardsTo(IEnumerable<BoardCardItem> cardList)
        {
            ResetCardsCollectionTo(cardList, TurnCardItemsCollection);
        }

        private void ResetRiverCardsTo(IEnumerable<BoardCardItem> cardList)
        {
            ResetCardsCollectionTo(cardList, RiverCardItemsCollection);
        }

        private void ResetBoardTextureCollectionTo(IEnumerable<BoardTextureItem> textureList, IEnumerable<BoardTextureItem> collection)
        {
            foreach (var texture in textureList)
            {
                var cur = collection.FirstOrDefault(x => x.Name == texture.Name);
                if (cur != null)
                {
                    FillBoardTextItemInfo(cur, texture);
                }
            }
        }

        private void FillBoardTextItemInfo(BoardTextureItem to, BoardTextureItem from)
        {
            to.IsChecked = from.IsChecked;
            if ((to is HighCardBoardTextureItem) && (from is HighCardBoardTextureItem))
            {
                (to as HighCardBoardTextureItem).SelectedRank = (from as HighCardBoardTextureItem).SelectedRank;
            }
            else if ((to is StraightBoardTextureItem) && (from is StraightBoardTextureItem))
            {
                (to as StraightBoardTextureItem).SelectedNumber = (from as StraightBoardTextureItem).SelectedNumber;
                (to as StraightBoardTextureItem).SelectedSign = (from as StraightBoardTextureItem).SelectedSign;
            }
        }


        private void ResetFlopBoardTextureTo(IEnumerable<BoardTextureItem> textureList)
        {
            ResetBoardTextureCollectionTo(textureList, FlopBoardTextureCollection);
        }

        private void ResetTurnBoardTextureTo(IEnumerable<BoardTextureItem> textureList)
        {
            ResetBoardTextureCollectionTo(textureList, TurnBoardTextureCollection);
        }

        private void ResetRiverBoardTextureTo(IEnumerable<BoardTextureItem> textureList)
        {
            ResetBoardTextureCollectionTo(textureList, RiverBoardTextureCollection);
        }

        #endregion

        #region Predicates
        private Expression<Func<Playerstatistic, bool>> GetBoardCardItemsPredicate()
        {
            Expression<Func<Playerstatistic, bool>> resultPredicate = null;
            var analyzer = new ExactCardsTextureAnalyzer();

            if (FlopCardItemsCollection.Any(x => x.Suit != RangeCardSuit.None))
            {
                var collection = FlopCardItemsCollection.Where(x => x.Suit != RangeCardSuit.None);
                if (resultPredicate == null)
                {
                    resultPredicate = PredicateBuilder.Create<Playerstatistic>(x => analyzer.Analyze(x.Board, collection));
                }
                else
                {
                    resultPredicate = resultPredicate.Or(x => analyzer.Analyze(x.Board, collection));
                }
            }

            if (TurnCardItemsCollection.Any(x => x.Suit != RangeCardSuit.None))
            {
                var collection = TurnCardItemsCollection.Where(x => x.Suit != RangeCardSuit.None);
                if (resultPredicate == null)
                {
                    resultPredicate = PredicateBuilder.Create<Playerstatistic>(x => analyzer.Analyze(x.Board, collection));
                }
                else
                {
                    resultPredicate = resultPredicate.Or(x => analyzer.Analyze(x.Board, collection));
                }
            }

            if (RiverCardItemsCollection.Any(x => x.Suit != RangeCardSuit.None))
            {
                var collection = RiverCardItemsCollection.Where(x => x.Suit != RangeCardSuit.None);
                if (resultPredicate == null)
                {
                    resultPredicate = PredicateBuilder.Create<Playerstatistic>(x => analyzer.Analyze(x.Board, collection));
                }
                else
                {
                    resultPredicate = resultPredicate.Or(x => analyzer.Analyze(x.Board, collection));
                }
            }

            return resultPredicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetBoardTexturePredicate()
        {
            Expression<Func<Playerstatistic, bool>> resultPredicate = null;
            var analyzers = BoardTextureAnalyzer.GetDefaultAnalyzers();

            if (FlopBoardTextureCollection.Any(x => x.IsChecked))
            {
                if (resultPredicate == null) resultPredicate = PredicateBuilder.False<Playerstatistic>();

                var collection = FlopBoardTextureCollection.Where(x => x.IsChecked);
                resultPredicate = resultPredicate.Or(p => collection.All(item => analyzers.First(a => a.GetRank() == item.BoardTexture).Analyze(BoardCards.FromCards(p.Board), item)));
            }

            if (TurnBoardTextureCollection.Any(x => x.IsChecked))
            {
                if (resultPredicate == null) resultPredicate = PredicateBuilder.False<Playerstatistic>();

                var collection = TurnBoardTextureCollection.Where(x => x.IsChecked);
                resultPredicate = resultPredicate.Or(p => collection.All(item => analyzers.First(a => a.GetRank() == item.BoardTexture).Analyze(BoardCards.FromCards(p.Board), item)));
            }

            if (RiverBoardTextureCollection.Any(x => x.IsChecked))
            {
                if (resultPredicate == null) resultPredicate = PredicateBuilder.False<Playerstatistic>();

                var collection = RiverBoardTextureCollection.Where(x => x.IsChecked);
                resultPredicate = resultPredicate.Or(p => collection.All(item => analyzers.First(a => a.GetRank() == item.BoardTexture).Analyze(BoardCards.FromCards(p.Board), item)));
            }

            return resultPredicate;
        }
        #endregion

        #region Properties
        private EnumFilterModelType _type;
        private ObservableCollection<BoardCardItem> _flopCardItemsCollection;
        private ObservableCollection<BoardCardItem> _turnCardItemsCollection;
        private ObservableCollection<BoardCardItem> _riverCardITemsCollection;

        private ObservableCollection<BoardTextureItem> _flopBoardTextureCollection;
        private ObservableCollection<BoardTextureItem> _turnBoardTextureCollection;
        private ObservableCollection<BoardTextureItem> _riverBoardTextureCollection;

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

        public ObservableCollection<BoardCardItem> FlopCardItemsCollection
        {
            get { return _flopCardItemsCollection; }
            set { _flopCardItemsCollection = value; }
        }

        public ObservableCollection<BoardCardItem> TurnCardItemsCollection
        {
            get { return _turnCardItemsCollection; }
            set { _turnCardItemsCollection = value; }
        }

        public ObservableCollection<BoardCardItem> RiverCardItemsCollection
        {
            get { return _riverCardITemsCollection; }
            set { _riverCardITemsCollection = value; }
        }

        public ObservableCollection<BoardTextureItem> FlopBoardTextureCollection
        {
            get { return _flopBoardTextureCollection; }
            set { _flopBoardTextureCollection = value; }
        }

        public ObservableCollection<BoardTextureItem> TurnBoardTextureCollection
        {
            get { return _turnBoardTextureCollection; }
            set { _turnBoardTextureCollection = value; }
        }

        public ObservableCollection<BoardTextureItem> RiverBoardTextureCollection
        {
            get { return _riverBoardTextureCollection; }
            set { _riverBoardTextureCollection = value; }
        }

        #endregion
    }

    [Serializable]
    public class BoardCardItem : FilterBaseEntity
    {
        public void Reset()
        {
            this.Suit = RangeCardSuit.None;
            this.Rank = RangeCardRank.None;
        }

        public static Action<Street> OnChanged;

        private RangeCardRank _rank;
        private RangeCardSuit _suit;
        private Street _targetStreet;
        private bool _isEnabled;

        public RangeCardRank Rank
        {
            get
            {
                return _rank;
            }

            set
            {
                if (value == _rank) return;
                _rank = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke(TargetStreet);
            }
        }

        public RangeCardSuit Suit
        {
            get
            {
                return _suit;
            }

            set
            {
                if (value == _suit) return;
                _suit = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke(TargetStreet);
            }
        }

        public Street TargetStreet
        {
            get
            {
                return _targetStreet;
            }

            set
            {
                _targetStreet = value;
            }
        }

        public int CardsCount
        {
            get
            {
                switch (TargetStreet)
                {
                    case Street.Flop:
                        return 3;
                    case Street.Turn:
                        return 4;
                    case Street.River:
                        return 5;
                }
                return -1;
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public override string ToString()
        {
            return Rank.ToRankString() + Suit.ToSuitString();
        }
    }

    [Serializable]
    public class BoardTextureItem : FilterBaseEntity
    {
        public static Action<Street> OnChanged;

        private bool _isChecked;
        private Street _targetStreet;
        private BoardTextures _boardTexture;

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

                if (OnChanged != null) OnChanged.Invoke(TargetStreet);
            }
        }

        public Street TargetStreet
        {
            get
            {
                return _targetStreet;
            }

            set
            {
                _targetStreet = value;
            }
        }

        public BoardTextures BoardTexture
        {
            get
            {
                return _boardTexture;
            }
            set
            {
                _boardTexture = value;
            }
        }

        public int IntStreetValue
        {
            get
            {
                return CardHelper.GetCardsAmountForStreet(TargetStreet);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [Serializable]
    public class HighCardBoardTextureItem : BoardTextureItem
    {
        private string _selectedRank;

        public HighCardBoardTextureItem()
        {
            SelectedRank = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.First();
        }

        public string SelectedRank
        {
            get { return _selectedRank; }
            set
            {
                if (value == _selectedRank) return;
                _selectedRank = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke(TargetStreet);
            }
        }

        public override string ToString()
        {
            return String.Format(this.Name, SelectedRank);
        }
    }

    [Serializable]
    public class StraightBoardTextureItem : BoardTextureItem
    {
        private Dictionary<EnumEquality, string> _equalityList;
        private KeyValuePair<EnumEquality, string> _selectedSign;
        private IEnumerable<int> _numericList;
        private int _selectedNumber;

        public StraightBoardTextureItem()
        {
            var list = new Dictionary<EnumEquality, string>()
            {
                { EnumEquality.GreaterThan, ">" },
                { EnumEquality.LessThan, "<" },
                { EnumEquality.EqualTo, "=" },
            };

            var numList = new List<int>();
            for (int i = 1; i <= 16; i++)
            {
                numList.Add(i);
            }

            EqualityList = new Dictionary<EnumEquality, string>(list);
            NumericList = numList.ToList();

            SelectedSign = EqualityList.First();
            SelectedNumber = NumericList.First();
        }

        public Dictionary<EnumEquality, string> EqualityList
        {
            get { return _equalityList; }
            set { _equalityList = value; }
        }

        public IEnumerable<int> NumericList
        {
            get { return _numericList; }
            set { _numericList = value; }
        }

        public KeyValuePair<EnumEquality, string> SelectedSign
        {
            get { return _selectedSign; }
            set
            {
                if (value.Equals(_selectedSign)) return;
                _selectedSign = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke(TargetStreet);
            }
        }

        public int SelectedNumber
        {
            get { return _selectedNumber; }
            set
            {
                if (value == _selectedNumber) return;
                _selectedNumber = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke(TargetStreet);
            }
        }

        public override string ToString()
        {
            return String.Format(this.Name, SelectedSign.Value, SelectedNumber);
        }
    }
}

