using DriveHUD.Common.Annotations;
using DriveHUD.Common.Reflection;
using DriveHUD.Entities;
using Prism.Mvvm;
using ProtoBuf;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model.Extensions
{
    public class ComparableCardsStatistic : INotifyPropertyChanged
    {
        public ComparableCardsStatistic(Playerstatistic stat)
        {
            Statistic = stat;
            PlayerCards = new ComparableCard(stat.Cards);
            BoardCards = new ComparableCard(stat.Board);
        }

        public void UpdateHandNoteText()
        {
            OnPropertyChanged(nameof(HandNoteText));
        }

        #region Properties

        public virtual string HandNoteText
        {
            get
            {
                return Statistic.HandNoteText;
            }
        }

        public Playerstatistic Statistic { get; set; }

        public ComparableCard PlayerCards { get; set; }

        public ComparableCard BoardCards { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [ProtoContract]
    public sealed class ComparableCard : BindableBase, IComparable<ComparableCard>
    {
        public ComparableCard(string cards)
        {
            Cards = cards;
        }

        [ProtoMember(1)]
        private string cards;

        public string Cards
        {
            get
            {
                return cards;
            }
            private set
            {
                SetProperty(ref cards, value);
            }
        }

        public override string ToString()
        {
            return Cards;
        }

        public int CompareTo(ComparableCard other)
        {
            if (other == null)
            {
                return 1;
            }

            var currentCards = CardHelper.Split(Cards);
            var otherCards = CardHelper.Split(other.Cards);

            if (currentCards.Count != otherCards.Count)
            {
                return currentCards.Count - otherCards.Count;
            }

            for (int i = 0; i < currentCards.Count; i++)
            {
                int result = CardHelper.GetCardRank(currentCards[i]) - CardHelper.GetCardRank(otherCards[i]);

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }
}