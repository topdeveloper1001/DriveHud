using DriveHUD.Common.Annotations;
using DriveHUD.Common.Reflection;
using DriveHUD.Entities;
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
            OnPropertyChanged(ReflectionHelper.GetPath<ComparableCardsStatistic>(o => o.HandNoteText));
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

    public class ComparableCard : IComparable<ComparableCard>, INotifyPropertyChanged
    {
        public string Cards { get; set; }

        public ComparableCard(string cards)
        {
            Cards = cards;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Cards;
        }

        public int CompareTo(ComparableCard other)
        {
            var currentCards = CardHelper.Split(Cards);
            var otherCards = CardHelper.Split(other.Cards);

            if (currentCards.Count != otherCards.Count)
            {
                return currentCards.Count - otherCards.Count;
            }

            int length = currentCards.Count;

            for (int i = 0; i < length; i++)
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