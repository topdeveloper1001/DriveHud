//-----------------------------------------------------------------------
// <copyright file="TextureSettings.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using DriveHUD.Common.Linq;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.TextureObjects
{
    public abstract class TextureSettings : ReactiveObject
    {
        public TextureSettings()
        {
            Changed
                .Where(x => x.PropertyName != nameof(BoardTextureFilterType))
                .Subscribe(x => this.RaisePropertyChanged(nameof(BoardTextureFilterType)));

            openEndedStraightDrawsCollection = new ObservableCollection<int>(Enumerable.Range(0, 10));
            gutshotsCollection = new ObservableCollection<int>(Enumerable.Range(0, 10));
            possibleStraightsCompareCollection = new ObservableCollection<CompareEnum>(Enum.GetValues(typeof(CompareEnum)).OfType<CompareEnum>());
            possibleStraightsCollection = new ObservableCollection<int>(Enumerable.Range(0, 21));
            highestCardCollection = new ObservableCollection<string>(HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Reverse());

            cardTextureCollection = new ReactiveList<CardTexture>(HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Select(x => new CardTexture { Card = x, IsChecked = false }));
            cardTextureCollection.ChangeTrackingEnabled = true;
            cardTextureCollection.ItemChanged
                .Where(x => x.PropertyName == nameof(CardTexture.IsChecked))
                .Subscribe(x =>
                {
                    selectedCardTexture = string.Join(",", cardTextureCollection.Where(c => c.IsChecked).Select(c => c.Card).ToArray());
                    this.RaisePropertyChanged(nameof(SelectedCardTexture));
                });
        }

        private bool isFlushCardFilter;

        public bool IsFlushCardFilter
        {
            get
            {
                return isFlushCardFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isFlushCardFilter, value);
            }
        }

        private bool isOpenEndedStraightDrawsFilter;

        public bool IsOpenEndedStraightDrawsFilter
        {
            get
            {
                return isOpenEndedStraightDrawsFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isOpenEndedStraightDrawsFilter, value);
            }
        }

        private int openEndedStraightDraws;

        public int OpenEndedStraightDraws
        {
            get
            {
                return openEndedStraightDraws;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref openEndedStraightDraws, value);
            }
        }

        private bool isPairedFilter;

        public bool IsPairedFilter
        {
            get
            {
                return isPairedFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isPairedFilter, value);
            }
        }

        private bool isGutshotsFilter;

        public bool IsGutshotsFilter
        {
            get
            {
                return isGutshotsFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isGutshotsFilter, value);
            }
        }

        private bool isCardTextureFilter;

        public bool IsCardTextureFilter
        {
            get
            {
                return isCardTextureFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isCardTextureFilter, value);
            }
        }

        private bool isHighcardFilter;

        public bool IsHighcardFilter
        {
            get
            {
                return isHighcardFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isHighcardFilter, value);
            }
        }

        private bool isPossibleStraightsFilter;

        public bool IsPossibleStraightsFilter
        {
            get
            {
                return isPossibleStraightsFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isPossibleStraightsFilter, value);
            }
        }

        private CompareEnum possibleStraightsCompare;

        public CompareEnum PossibleStraightsCompare
        {
            get
            {
                return possibleStraightsCompare;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref possibleStraightsCompare, value);
            }
        }

        private int possibleStraights;

        public int PossibleStraights
        {
            get
            {
                return possibleStraights;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref possibleStraights, value);
            }
        }

        private int gutshots;

        public int Gutshots
        {
            get
            {
                return gutshots;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref gutshots, value);
            }
        }

        private string highestCard;

        public string HighestCard
        {
            get
            {
                return highestCard;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref highestCard, value);
            }
        }

        private string selectedCardTexture;

        public string SelectedCardTexture
        {
            get
            {
                return selectedCardTexture;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedCardTexture, value);

                var selectedCards = selectedCardTexture.Split(',');

                cardTextureCollection.ForEach(x => x.IsChecked = selectedCards.Contains(x.Card));
            }
        }

        private bool isPairedFilterTrue;

        public bool IsPairedFilterTrue
        {
            get
            {
                return isPairedFilterTrue;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isPairedFilterTrue, value);
            }
        }

        public abstract NoteStageType StageType { get; }

        private BoardTextureFilterType boardTextureFilterType;

        [XmlIgnore]
        public virtual BoardTextureFilterType BoardTextureFilterType
        {
            get
            {
                if (IsFlushCardFilter || IsOpenEndedStraightDrawsFilter || IsPairedFilter || IsGutshotsFilter || IsCardTextureFilter || IsHighcardFilter || IsPossibleStraightsFilter)
                {
                    return BoardTextureFilterType.FilterByTexture;
                }

                return BoardTextureFilterType.Any;
            }
            set
            {
                if (value == BoardTextureFilterType.FilterByTexture)
                {
                    return;
                }

                Reset();

                this.RaiseAndSetIfChanged(ref boardTextureFilterType, value);
            }
        }

        [XmlIgnore]
        public List<string> SelectedCardTextureList
        {
            get
            {
                return CardTextureCollection
                    .Where(x => x.IsChecked)
                    .Select(x => x.Card)
                    .ToList();
            }
        }

        private ReactiveList<CardTexture> cardTextureCollection;

        [XmlIgnore]
        public ReactiveList<CardTexture> CardTextureCollection
        {
            get
            {
                return cardTextureCollection;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref cardTextureCollection, value);
            }
        }

        private ObservableCollection<int> openEndedStraightDrawsCollection;

        [XmlIgnore]
        public ObservableCollection<int> OpenEndedStraightDrawsCollection
        {
            get
            {
                return openEndedStraightDrawsCollection;
            }
        }

        private ObservableCollection<int> gutshotsCollection;

        [XmlIgnore]
        public ObservableCollection<int> GutshotsCollection
        {
            get
            {
                return gutshotsCollection;
            }
        }

        private ObservableCollection<CompareEnum> possibleStraightsCompareCollection;

        [XmlIgnore]
        public ObservableCollection<CompareEnum> PossibleStraightsCompareCollection
        {
            get
            {
                return possibleStraightsCompareCollection;
            }
        }

        private ObservableCollection<int> possibleStraightsCollection;

        [XmlIgnore]
        public ObservableCollection<int> PossibleStraightsCollection
        {
            get
            {
                return possibleStraightsCollection;
            }
        }

        private ObservableCollection<string> highestCardCollection;

        [XmlIgnore]
        public ObservableCollection<string> HighestCardCollection
        {
            get
            {
                return highestCardCollection;
            }
        }

        public virtual void Reset()
        {
            IsFlushCardFilter = IsOpenEndedStraightDrawsFilter = IsPairedFilter = IsGutshotsFilter = IsCardTextureFilter = IsHighcardFilter = IsPossibleStraightsFilter = IsPairedFilterTrue = false;
        }

        public override bool Equals(object obj)
        {
            var textureSettings = obj as TextureSettings;

            if (textureSettings == null)
            {
                return false;
            }

            return IsFlushCardFilter == textureSettings.IsFlushCardFilter &&
                IsOpenEndedStraightDrawsFilter == textureSettings.IsOpenEndedStraightDrawsFilter &&
                (IsOpenEndedStraightDrawsFilter && OpenEndedStraightDraws == textureSettings.OpenEndedStraightDraws || !IsOpenEndedStraightDrawsFilter) &&
                IsGutshotsFilter == textureSettings.IsGutshotsFilter &&
                (IsGutshotsFilter && Gutshots == textureSettings.Gutshots || !IsGutshotsFilter) &&
                IsHighcardFilter == textureSettings.IsHighcardFilter &&
                (IsHighcardFilter && HighestCard == textureSettings.HighestCard || !IsHighcardFilter) &&
                IsPairedFilter == textureSettings.IsPairedFilter &&
                (IsPairedFilter && IsPairedFilterTrue == textureSettings.IsPairedFilterTrue || !IsPairedFilter) &&
                IsPossibleStraightsFilter == textureSettings.IsPossibleStraightsFilter &&
                (IsPossibleStraightsFilter && (PossibleStraights == textureSettings.PossibleStraights && PossibleStraightsCompare == textureSettings.PossibleStraightsCompare) || !IsPossibleStraightsFilter) &&
                IsCardTextureFilter == textureSettings.IsCardTextureFilter &&
                (IsCardTextureFilter && CompareHelpers.CompareStringLists(SelectedCardTextureList, textureSettings.SelectedCardTextureList) || !IsCardTextureFilter);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 19;
                hash += hash * 31 + IsFlushCardFilter.GetHashCode();
                hash += hash * 31 + IsPairedFilter.GetHashCode();
                hash += hash * 31 + IsOpenEndedStraightDrawsFilter.GetHashCode();
                hash += hash * 31 + IsGutshotsFilter.GetHashCode();
                hash += hash * 31 + IsCardTextureFilter.GetHashCode();
                hash += hash * 31 + IsHighcardFilter.GetHashCode();
                hash += hash * 31 + IsPossibleStraightsFilter.GetHashCode();
                hash += hash * 31 + OpenEndedStraightDraws.GetHashCode();
                hash += hash * 31 + PossibleStraightsCompare.GetHashCode();
                hash += hash * 31 + PossibleStraights.GetHashCode();
                hash += hash * 31 + Gutshots.GetHashCode();

                if (!string.IsNullOrEmpty(HighestCard))
                {
                    hash += hash * 31 + HighestCard.GetHashCode();
                }

                if (!string.IsNullOrEmpty(SelectedCardTexture))
                {
                    hash += hash * 31 + SelectedCardTexture.GetHashCode();
                }

                hash += hash * 31 + IsPairedFilterTrue.GetHashCode();
                hash += hash * 31 + StageType.GetHashCode();

                return hash;
            }
        }

        public class CardTexture : ReactiveObject
        {
            private string card;

            public string Card
            {
                get
                {
                    return card;
                }
                set
                {
                    this.RaiseAndSetIfChanged(ref card, value);
                }
            }

            private bool isChecked;

            public bool IsChecked
            {
                get
                {
                    return isChecked;
                }
                set
                {
                    this.RaiseAndSetIfChanged(ref isChecked, value);
                }
            }
        }
    }
}