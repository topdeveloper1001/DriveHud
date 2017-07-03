using DriveHUD.Common.Linq;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using Model.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Filters
{
    [Serializable]
    public class FilterHandActionModel : FilterBaseEntity, IFilterModel, IXmlSerializable
    {
        public FilterHandActionModel()
        {
            this.Name = "Hand Action";
            this.Type = EnumFilterModelType.FilterHandActionModel;
        }

        public void Initialize()
        {
            FilterPreflopItemsInitialize();
            FilterFlopItemsInitialize();
            FilterTurnItemsInitialize();
            FilterRiverItemsInitialize();
        }

        #region Methods
        private IEnumerable<HandActionFilterItem> CreateHandActionsList(Street street, HandActionType actionType)
        {
            return new List<HandActionFilterItem>
            {
                new HandActionFilterItem() { Name = "No Additional Actions", BeginningActionType = actionType, ActionString = string.Empty, TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Fold", BeginningActionType = actionType, ActionString = "F", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Call", BeginningActionType = actionType, ActionString = "C", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Call Then Fold", BeginningActionType = actionType, ActionString = "CF", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Call Then Call", BeginningActionType = actionType, ActionString = "CC", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Call Then Call Then Call", BeginningActionType = actionType, ActionString = "CCC", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Call Then Raise Then Fold", BeginningActionType = actionType, ActionString = "CRF", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Call Then Raise Then Call", BeginningActionType = actionType, ActionString = "CRC", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Raise", BeginningActionType = actionType, ActionString = "R", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Raise then Raise", BeginningActionType = actionType, ActionString = "RR", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Raise then Fold", BeginningActionType = actionType, ActionString = "RF", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Raise then Call", BeginningActionType = actionType, ActionString = "RC", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Raise then Raise then Raise", BeginningActionType = actionType, ActionString = "RRR", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Raise then Raise then Fold", BeginningActionType = actionType, ActionString = "RRF", TargetStreet = street },
                new HandActionFilterItem() { Name = "Then Raise then Raise then Call", BeginningActionType = actionType, ActionString = "RRC", TargetStreet = street },
            };
        }

        private ObservableCollection<HandActionFilterButton> GetButtonsForStreet(Street street)
        {
            switch (street)
            {
                case Street.Preflop:
                    return new ObservableCollection<HandActionFilterButton>()
                        {
                            new HandActionFilterButton() { HandActionType = HandActionType.RAISE, TargetStreet = street },
                            new HandActionFilterButton() { HandActionType = HandActionType.CALL, TargetStreet = street },
                            new HandActionFilterButton() { HandActionType = HandActionType.CHECK, TargetStreet = street },
                        };
                default:
                    return new ObservableCollection<HandActionFilterButton>()
                        {
                            new HandActionFilterButton() { HandActionType = HandActionType.CHECK, TargetStreet = street },
                            new HandActionFilterButton() { HandActionType = HandActionType.BET, TargetStreet = street },
                            new HandActionFilterButton() { HandActionType = HandActionType.CALL, TargetStreet = street },
                            new HandActionFilterButton() { HandActionType = HandActionType.RAISE, TargetStreet = street },
                        };
            }

        }

        private void FilterPreflopItemsInitialize()
        {
            PreflopItems = new ObservableCollection<HandActionFilterItem>();
            PreflopItems.AddRange(CreateHandActionsList(Street.Preflop, HandActionType.RAISE));
            PreflopItems.AddRange(CreateHandActionsList(Street.Preflop, HandActionType.CALL));
            PreflopItems.AddRange(CreateHandActionsList(Street.Preflop, HandActionType.CHECK));

            PreflopButtons = GetButtonsForStreet(Street.Preflop);

        }

        private void FilterFlopItemsInitialize()
        {
            FlopItems = new ObservableCollection<HandActionFilterItem>();
            FlopItems.AddRange(CreateHandActionsList(Street.Flop, HandActionType.RAISE));
            FlopItems.AddRange(CreateHandActionsList(Street.Flop, HandActionType.CALL));
            FlopItems.AddRange(CreateHandActionsList(Street.Flop, HandActionType.CHECK));
            FlopItems.AddRange(CreateHandActionsList(Street.Flop, HandActionType.BET));

            FlopButtons = GetButtonsForStreet(Street.Flop);
        }

        private void FilterTurnItemsInitialize()
        {
            TurnItems = new ObservableCollection<HandActionFilterItem>();
            TurnItems.AddRange(CreateHandActionsList(Street.Turn, HandActionType.RAISE));
            TurnItems.AddRange(CreateHandActionsList(Street.Turn, HandActionType.CALL));
            TurnItems.AddRange(CreateHandActionsList(Street.Turn, HandActionType.CHECK));
            TurnItems.AddRange(CreateHandActionsList(Street.Turn, HandActionType.BET));

            TurnButtons = GetButtonsForStreet(Street.Turn);
        }

        private void FilterRiverItemsInitialize()
        {
            RiverItems = new ObservableCollection<HandActionFilterItem>();
            RiverItems.AddRange(CreateHandActionsList(Street.River, HandActionType.RAISE));
            RiverItems.AddRange(CreateHandActionsList(Street.River, HandActionType.CALL));
            RiverItems.AddRange(CreateHandActionsList(Street.River, HandActionType.CHECK));
            RiverItems.AddRange(CreateHandActionsList(Street.River, HandActionType.BET));

            RiverButtons = GetButtonsForStreet(Street.River);
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            List<Expression<Func<Playerstatistic, bool>>> predicateList = new List<Expression<Func<Playerstatistic, bool>>>()
            {
                GetPredicateForStreet(Street.Preflop),
                GetPredicateForStreet(Street.Flop),
                GetPredicateForStreet(Street.Turn),
                GetPredicateForStreet(Street.River),
            };

            predicateList = predicateList.Where(x => x != null).ToList();
            if (predicateList.Count == 0)
                return null;

            var resultPredicate = predicateList.First();
            for (int i = 1; i < predicateList.Count; i++)
            {
                resultPredicate = resultPredicate.And(predicateList[i]);
            }

            return resultPredicate;
        }

        public void ResetFilter()
        {
            ResetPreflopFilter();
            ResetFlopFilter();
            ResetTurnFilter();
            ResetRiverFilter();
        }

        public override object Clone()
        {
            FilterHandActionModel model = this.DeepCloneJson();

            return model;
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterHandActionModel)
            {
                var loadedFilter = filter as FilterHandActionModel;

                ResetPreflopFilterTo(loadedFilter.PreflopButtons, loadedFilter.PreflopItems);
                ResetFlopFilterTo(loadedFilter.FlopButtons, loadedFilter.FlopItems);
                ResetTurnFilterTo(loadedFilter.TurnButtons, loadedFilter.TurnItems);
                ResetRiverFilterTo(loadedFilter.RiverButtons, loadedFilter.RiverItems);
            }
        }
        #endregion

        #region Reset Filters
        public void ResetPreflopFilter()
        {
            PreflopButtons.ForEach(x => x.IsChecked = false);
            PreflopItems.ForEach(x => x.IsChecked = false);
        }

        public void ResetFlopFilter()
        {
            FlopButtons.ForEach(x => x.IsChecked = false);
            FlopItems.ForEach(x => x.IsChecked = false);
        }

        public void ResetTurnFilter()
        {
            TurnButtons.ForEach(x => x.IsChecked = false);
            TurnItems.ForEach(x => x.IsChecked = false);
        }

        public void ResetRiverFilter()
        {
            RiverButtons.ForEach(x => x.IsChecked = false);
            RiverItems.ForEach(x => x.IsChecked = false);
        }
        #endregion

        #region Restore Defaults
        private void ResetPreflopFilterTo(IEnumerable<HandActionFilterButton> buttonsList, IEnumerable<HandActionFilterItem> actionList)
        {
            foreach (var action in actionList)
            {
                var cur = PreflopItems.FirstOrDefault(x => x.ToString() == action.ToString());
                if (cur != null)
                {
                    cur.IsChecked = action.IsChecked;
                }
            }

            foreach (var button in buttonsList)
            {
                var cur = PreflopButtons.FirstOrDefault(x => x.HandActionType == button.HandActionType);
                if (cur != null)
                {
                    cur.IsChecked = button.IsChecked;
                }
            }
        }

        private void ResetFlopFilterTo(IEnumerable<HandActionFilterButton> buttonsList, IEnumerable<HandActionFilterItem> actionList)
        {
            foreach (var action in actionList)
            {
                var cur = FlopItems.FirstOrDefault(x => x.ToString() == action.ToString());
                if (cur != null)
                {
                    cur.IsChecked = action.IsChecked;
                }
            }

            foreach (var button in buttonsList)
            {
                var cur = FlopButtons.FirstOrDefault(x => x.HandActionType == button.HandActionType);
                if (cur != null)
                {
                    cur.IsChecked = button.IsChecked;
                }
            }
        }

        private void ResetTurnFilterTo(IEnumerable<HandActionFilterButton> buttonsList, IEnumerable<HandActionFilterItem> actionList)
        {
            foreach (var action in actionList)
            {
                var cur = TurnItems.FirstOrDefault(x => x.ToString() == action.ToString());
                if (cur != null)
                {
                    cur.IsChecked = action.IsChecked;
                }
            }

            foreach (var button in buttonsList)
            {
                var cur = TurnButtons.FirstOrDefault(x => x.HandActionType == button.HandActionType);
                if (cur != null)
                {
                    cur.IsChecked = button.IsChecked;
                }
            }
        }

        private void ResetRiverFilterTo(IEnumerable<HandActionFilterButton> buttonsList, IEnumerable<HandActionFilterItem> actionList)
        {
            foreach (var action in actionList)
            {
                var cur = RiverItems.FirstOrDefault(x => x.ToString() == action.ToString());
                if (cur != null)
                {
                    cur.IsChecked = action.IsChecked;
                }
            }

            foreach (var button in buttonsList)
            {
                var cur = RiverButtons.FirstOrDefault(x => x.HandActionType == button.HandActionType);
                if (cur != null)
                {
                    cur.IsChecked = button.IsChecked;
                }
            }
        }
        #endregion

        #region Predicates
        private Expression<Func<Playerstatistic, bool>> GetPredicateForStreet(Street street)
        {
            Expression<Func<Playerstatistic, bool>> predicate = null;
            var button = GetButonsCollectionForStreet(street).FirstOrDefault(x => x.IsChecked);
            if (button != null)
            {
                var collection = GetItemsCollectionForStreet(street).Where(x => x.BeginningActionType == button.HandActionType);
                if (collection != null && collection.Any(x => x.IsChecked))
                {
                    foreach (var item in collection.Where(x => x.IsChecked))
                    {
                        if (predicate == null)
                        {
                            predicate = PredicateBuilder.Create<Playerstatistic>(x => FilterHelpers.CheckActionLine(x.Line, item.ToString(), street));
                        }
                        else
                        {
                            predicate = predicate.Or(x => FilterHelpers.CheckActionLine(x.Line, item.ToString(), street));
                        }
                    }
                }
            }

            return predicate;
        }
        #endregion

        #region Properties
        private EnumFilterModelType _type;
        private HandActionFilterItem _selectedPreflopItem;
        private ObservableCollection<HandActionFilterItem> _preflopItems;
        private ObservableCollection<HandActionFilterItem> _flopItems;
        private ObservableCollection<HandActionFilterItem> _turnItems;
        private ObservableCollection<HandActionFilterItem> _riverItems;

        private ObservableCollection<HandActionFilterButton> _preflopButtons;
        private ObservableCollection<HandActionFilterButton> _flopButtons;
        private ObservableCollection<HandActionFilterButton> _turnButtons;
        private ObservableCollection<HandActionFilterButton> _riverButtons;

        public ObservableCollection<HandActionFilterItem> FlopItems
        {
            get { return _flopItems; }
            set { _flopItems = value; }
        }

        public ObservableCollection<HandActionFilterItem> PreflopItems
        {
            get { return _preflopItems; }
            set { _preflopItems = value; }
        }

        public ObservableCollection<HandActionFilterItem> TurnItems
        {
            get { return _turnItems; }
            set { _turnItems = value; }
        }

        public ObservableCollection<HandActionFilterItem> RiverItems
        {
            get { return _riverItems; }
            set { _riverItems = value; }
        }

        #region Preflop
        public IEnumerable<HandActionFilterItem> PreflopRaiseItems
        {
            get { return PreflopItems.Where(x => x.BeginningActionType == HandActionType.RAISE); }
        }

        public IEnumerable<HandActionFilterItem> PreflopCallItems
        {
            get { return PreflopItems.Where(x => x.BeginningActionType == HandActionType.CALL); }
        }

        public IEnumerable<HandActionFilterItem> PreflopCheckItems
        {
            get { return PreflopItems.Where(x => x.BeginningActionType == HandActionType.CHECK); }
        }
        #endregion

        #region Flop
        public IEnumerable<HandActionFilterItem> FlopRaiseItems
        {
            get { return FlopItems.Where(x => x.BeginningActionType == HandActionType.RAISE); }
        }

        public IEnumerable<HandActionFilterItem> FlopBetItems
        {
            get { return FlopItems.Where(x => x.BeginningActionType == HandActionType.BET); }
        }

        public IEnumerable<HandActionFilterItem> FlopCheckItems
        {
            get { return FlopItems.Where(x => x.BeginningActionType == HandActionType.CHECK); }
        }

        public IEnumerable<HandActionFilterItem> FlopCallItems
        {
            get { return FlopItems.Where(x => x.BeginningActionType == HandActionType.CALL); }
        }
        #endregion

        #region Turn
        public IEnumerable<HandActionFilterItem> TurnRaiseItems
        {
            get { return TurnItems.Where(x => x.BeginningActionType == HandActionType.RAISE); }
        }

        public IEnumerable<HandActionFilterItem> TurnBetItems
        {
            get { return TurnItems.Where(x => x.BeginningActionType == HandActionType.BET); }
        }

        public IEnumerable<HandActionFilterItem> TurnCallItems
        {
            get { return TurnItems.Where(x => x.BeginningActionType == HandActionType.CALL); }
        }

        public IEnumerable<HandActionFilterItem> TurnCheckItems
        {
            get { return TurnItems.Where(x => x.BeginningActionType == HandActionType.CHECK); }
        }
        #endregion

        #region River

        public IEnumerable<HandActionFilterItem> RiverRaiseItems
        {
            get { return RiverItems.Where(x => x.BeginningActionType == HandActionType.RAISE); }
        }

        public IEnumerable<HandActionFilterItem> RiverBetItems
        {
            get { return RiverItems.Where(x => x.BeginningActionType == HandActionType.BET); }
        }

        public IEnumerable<HandActionFilterItem> RiverCallItems
        {
            get { return RiverItems.Where(x => x.BeginningActionType == HandActionType.CALL); }
        }

        public IEnumerable<HandActionFilterItem> RiverCheckItems
        {
            get { return RiverItems.Where(x => x.BeginningActionType == HandActionType.CHECK); }
        }
        #endregion

        public HandActionFilterItem SelectetdPreflopItem
        {
            get { return _selectedPreflopItem; }
            set
            {
                if (_selectedPreflopItem == value) return;
                _selectedPreflopItem = value;
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<HandActionFilterButton> PreflopButtons
        {
            get
            {
                return _preflopButtons;
            }

            set
            {
                _preflopButtons = value;
            }
        }

        public ObservableCollection<HandActionFilterButton> FlopButtons
        {
            get
            {
                return _flopButtons;
            }

            set
            {
                _flopButtons = value;
            }
        }

        public ObservableCollection<HandActionFilterButton> TurnButtons
        {
            get
            {
                return _turnButtons;
            }

            set
            {
                _turnButtons = value;
            }
        }

        public ObservableCollection<HandActionFilterButton> RiverButtons
        {
            get
            {
                return _riverButtons;
            }

            set
            {
                _riverButtons = value;
            }
        }

        public IEnumerable<HandActionFilterItem> GetItemsCollectionForStreet(Street street)
        {
            switch (street)
            {
                case Street.Preflop:
                    return PreflopItems;
                case Street.Flop:
                    return FlopItems;
                case Street.Turn:
                    return TurnItems;
                case Street.River:
                    return RiverItems;
            }

            throw new ArgumentOutOfRangeException("street", street, "Street should be within Preflop-River range");
        }

        public IEnumerable<HandActionFilterButton> GetButonsCollectionForStreet(Street street)
        {
            switch (street)
            {
                case Street.Preflop:
                    return PreflopButtons;
                case Street.Flop:
                    return FlopButtons;
                case Street.Turn:
                    return TurnButtons;
                case Street.River:
                    return RiverButtons;
            }

            throw new ArgumentOutOfRangeException("street", street, "Street should be within Preflop-River range");
        }

        #endregion

        #region IXmlSerializable implementation

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Initialize();

            reader.MoveToContent();

            Action<HandActionFilterItem, HandActionFilterItem> updateHandActionFilterItem = (existingItem, item) =>
            {
                existingItem.IsChecked = item.IsChecked;
            };

            Func<HandActionFilterItem, HandActionFilterItem, bool> handActionFilterItemPredicate = (existingItem, item) => existingItem.Name == item.Name && existingItem.BeginningActionType == item.BeginningActionType;

            Action<HandActionFilterButton, HandActionFilterButton> updateHandActionFilterButton = (existingItem, item) =>
            {
                existingItem.IsChecked = item.IsChecked;
            };

            Func<HandActionFilterButton, HandActionFilterButton, bool> handActionFilterButtonPredicate = (existingItem, item) => existingItem.Name == item.Name && existingItem.HandActionType == item.HandActionType;

            while (reader.Read())
            {
                if (reader.IsStartElement(nameof(Id)))
                {
                    Id = Guid.Parse(reader.ReadElementContentAsString());
                }

                ReadHandValueCollectionsXml(reader, PreflopItems, nameof(PreflopItems), updateHandActionFilterItem, handActionFilterItemPredicate);
                ReadHandValueCollectionsXml(reader, FlopItems, nameof(FlopItems), updateHandActionFilterItem, handActionFilterItemPredicate);
                ReadHandValueCollectionsXml(reader, TurnItems, nameof(TurnItems), updateHandActionFilterItem, handActionFilterItemPredicate);
                ReadHandValueCollectionsXml(reader, RiverItems, nameof(RiverItems), updateHandActionFilterItem, handActionFilterItemPredicate);

                ReadHandValueCollectionsXml(reader, PreflopButtons, nameof(PreflopButtons), updateHandActionFilterButton, handActionFilterButtonPredicate);
                ReadHandValueCollectionsXml(reader, FlopButtons, nameof(FlopButtons), updateHandActionFilterButton, handActionFilterButtonPredicate);
                ReadHandValueCollectionsXml(reader, TurnButtons, nameof(TurnButtons), updateHandActionFilterButton, handActionFilterButtonPredicate);
                ReadHandValueCollectionsXml(reader, RiverButtons, nameof(RiverButtons), updateHandActionFilterButton, handActionFilterButtonPredicate);

                if (reader.Name == nameof(FilterHandActionModel) && reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.ReadEndElement();
                    break;
                }
            }
        }

        private static void ReadHandValueCollectionsXml<T>(XmlReader reader, ObservableCollection<T> collection, string nameOfCollection, Action<T, T> update, Func<T, T, bool> predicate)
            where T : FilterBaseEntity
        {
            if (!reader.IsStartElement(nameOfCollection) || reader.IsEmptyElement)
            {
                return;
            }

            reader.ReadStartElement(nameOfCollection);

            while (reader.IsStartElement(typeof(T).Name))
            {
                var itemType = typeof(T);

                var serializer = new XmlSerializer(itemType);

                var handValueItem = (T)serializer.Deserialize(reader);

                var existingHandValueItem = collection.FirstOrDefault(x => predicate(x, handValueItem));

                if (existingHandValueItem == null)
                {
                    collection.Add(handValueItem);
                }
                else
                {
                    existingHandValueItem.Id = handValueItem.Id;
                    existingHandValueItem.IsActive = handValueItem.IsActive;

                    update?.Invoke(existingHandValueItem, handValueItem);
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Id));
            writer.WriteValue(Id.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(Type));
            writer.WriteValue(Type.ToString());
            writer.WriteEndElement();

            Func<HandActionFilterItem, bool> handActionFilterItemPredicate = item => item.IsChecked;
            Func<HandActionFilterButton, bool> handActionFilterButtonPredicate = item => item.IsChecked;

            WriteHandValueCollectionsXml(writer, PreflopItems, nameof(PreflopItems), handActionFilterItemPredicate);
            WriteHandValueCollectionsXml(writer, FlopItems, nameof(FlopItems), handActionFilterItemPredicate);
            WriteHandValueCollectionsXml(writer, TurnItems, nameof(TurnItems), handActionFilterItemPredicate);
            WriteHandValueCollectionsXml(writer, RiverItems, nameof(RiverItems), handActionFilterItemPredicate);

            WriteHandValueCollectionsXml(writer, PreflopButtons, nameof(PreflopButtons), handActionFilterButtonPredicate);
            WriteHandValueCollectionsXml(writer, FlopButtons, nameof(FlopButtons), handActionFilterButtonPredicate);
            WriteHandValueCollectionsXml(writer, TurnButtons, nameof(TurnButtons), handActionFilterButtonPredicate);
            WriteHandValueCollectionsXml(writer, RiverButtons, nameof(RiverButtons), handActionFilterButtonPredicate);
        }

        private static void WriteHandValueCollectionsXml<T>(XmlWriter writer, ObservableCollection<T> collection, string nameOfCollection, Func<T, bool> predicate)
        {
            var filteredCollection = collection?
                .ToArray()
                .Where(predicate)
                .ToArray();

            if (filteredCollection != null && filteredCollection.Length > 0)
            {
                writer.WriteStartElement(nameOfCollection);

                foreach (var handValueItem in filteredCollection)
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    var xmlSerializer = new XmlSerializer(handValueItem.GetType());
                    xmlSerializer.Serialize(writer, handValueItem, ns);
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }

    [Serializable]
    public class HandActionFilterItem : FilterBaseEntity
    {
        public static Action<Street> OnChanged;

        private bool _isChecked;
        private Street _targetStreet;
        private string _actionString;
        private HandActionType _beginningActionType;

        public HandActionType BeginningActionType
        {
            get { return _beginningActionType; }
            set { _beginningActionType = value; }
        }

        public string ActionString
        {
            get { return _actionString; }
            set { _actionString = value; }
        }

        public Street TargetStreet
        {
            get { return _targetStreet; }
            set { _targetStreet = value; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnChanged != null)
                {
                    OnChanged.Invoke(TargetStreet);
                }
            }
        }

        public override string ToString()
        {
            switch (BeginningActionType)
            {
                case HandActionType.RAISE:
                    return "R" + ActionString;
                case HandActionType.BET:
                    return "B" + ActionString;
                case HandActionType.CALL:
                    return "C" + ActionString;
                case HandActionType.CHECK:
                    return "X" + ActionString;
            }
            return ActionString;
        }
    }

    [Serializable]
    public class HandActionFilterButton : FilterBaseEntity
    {
        private HandActionType _handActionType;
        private bool _isChecked;
        private Street _targetStreet;

        public Street TargetStreet
        {
            get { return _targetStreet; }
            set { _targetStreet = value; }
        }

        public HandActionType HandActionType
        {
            get
            {
                return _handActionType;
            }

            set
            {
                _handActionType = value;
            }
        }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked == value) return;
                _isChecked = value;

                OnPropertyChanged();

                if (HandActionFilterItem.OnChanged != null)
                {
                    HandActionFilterItem.OnChanged.Invoke(TargetStreet);
                }
            }
        }
    }

}
