using System;
using System.Linq;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;

namespace HandHistories.Objects.Hand
{
    public class HandHistory : HandHistorySummary
    {
        public HandHistory(GameDescriptor gameDescription) : base()
        {
            HandActions = new List<HandAction>();
            Players = new PlayerList();

            CommunityCards = BoardCards.ForPreflop();
            GameDescription = gameDescription;
        }

        public HandHistory() : this(new GameDescriptor())
        {

        }

        public List<HandAction> HandActions { get; set; }

        public IEnumerable<HandAction> PreFlop
        {
            get { return HandActions.Where(x => x.Street == Street.Preflop); }
        }

        public IEnumerable<HandAction> Flop
        {
            get { return HandActions.Where(x => x.Street == Street.Flop); }
        }

        public IEnumerable<HandAction> Turn
        {
            get { return HandActions.Where(x => x.Street == Street.Turn); }
        }

        public IEnumerable<HandAction> River
        {
            get { return HandActions.Where(x => x.Street == Street.River); }
        }

        public IEnumerable<HandAction> Showdown
        {
            get { return HandActions.Where(x => x.Street == Street.Showdown); }
        }

        public IEnumerable<HandAction> Summary
        {
            get { return HandActions.Where(x => x.Street == Street.Summary); }
        }

        public IEnumerable<HandAction> WinningActions
        {
            get { return HandActions.Where(x => x.IsWinningsAction); }
        }

        public BoardCards CommunityCards { get; set; }

        public PlayerList Players { get; set; }

        public Player Hero { get; set; }

        public Exception Exception { get; set; }

        public bool HasError
        {
            get
            {
                return Exception != null;
            }
        }

        public int NumPlayersActive
        {
            get { return Players.Count(p => p.IsSittingOut == false); }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            HandHistory hand = obj as HandHistory;
            if (hand == null) return false;

            return ToString().Equals(hand.ToString());
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", HandId, GameDescription.ToString());
        }
    }
}
