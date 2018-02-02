//-----------------------------------------------------------------------
// <copyright file="ReportHandViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Extensions;
using Prism.Mvvm;
using ProtoBuf;
using System;

namespace Model.Data
{
    [ProtoContract]
    public class ReportHandViewModel : BindableBase
    {
        private ReportHandViewModel()
        {
        }

        public ReportHandViewModel(Playerstatistic statistic)
        {
            cards = new ComparableCard(statistic.Cards);
            boardCards = new ComparableCard(statistic.Board);
            handNote = statistic.HandNoteText;
            gameNumber = statistic.GameNumber;
            pokerSiteId = statistic.PokersiteId;
            time = statistic.Time;
            facingPreflop = statistic.FacingPreflop;
            line = statistic.Line;
            netWon = statistic.NetWon;
            position = statistic.Position;
            positionString = statistic.PositionString;
            equity = statistic.Equity;
            handTag = statistic.HandTag;
            playerName = statistic.PlayerName;
        }

        [ProtoMember(1)]
        private ComparableCard cards;

        public ComparableCard Cards
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

        [ProtoMember(2)]
        private ComparableCard boardCards;

        public ComparableCard BoardCards
        {
            get
            {
                return boardCards;
            }
            private set
            {
                SetProperty(ref boardCards, value);
            }
        }

        [ProtoMember(3)]
        private string handNote;

        public string HandNote
        {
            get
            {
                return handNote;
            }
            set
            {
                SetProperty(ref handNote, value);
            }
        }

        [ProtoMember(4)]
        private long gameNumber;

        public long GameNumber
        {
            get
            {
                return gameNumber;
            }
            private set
            {
                SetProperty(ref gameNumber, value);
            }
        }

        [ProtoMember(5)]
        private int pokerSiteId;

        public int PokerSiteId
        {
            get
            {
                return pokerSiteId;
            }
            private set
            {
                SetProperty(ref pokerSiteId, value);
            }
        }

        [ProtoMember(6)]
        private DateTime time;

        public DateTime Time
        {
            get
            {
                return time;
            }
            private set
            {
                SetProperty(ref time, value);
            }
        }

        [ProtoMember(7)]
        private EnumFacingPreflop facingPreflop;

        public EnumFacingPreflop FacingPreflop
        {
            get
            {
                return facingPreflop;
            }
            private set
            {
                SetProperty(ref facingPreflop, value);
            }
        }

        [ProtoMember(8)]
        private string line;

        public string Line
        {
            get
            {
                return line;
            }
            private set
            {
                SetProperty(ref line, value);
            }
        }

        [ProtoMember(9)]
        private decimal netWon;

        public decimal NetWon
        {
            get
            {
                return netWon;
            }
            private set
            {
                SetProperty(ref netWon, value);
            }
        }

        [ProtoMember(10)]
        private EnumPosition position;

        public EnumPosition Position
        {
            get
            {
                return position;
            }
            private set
            {
                SetProperty(ref position, value);
            }
        }

        [ProtoMember(11)]
        private string positionString;

        public string PositionString
        {
            get
            {
                return positionString;
            }
            private set
            {
                SetProperty(ref positionString, value);
            }
        }

        [ProtoMember(12)]
        private decimal equity;

        public decimal Equity
        {
            get
            {
                return equity;
            }
            private set
            {
                SetProperty(ref equity, value);
            }
        }

        [ProtoMember(13)]
        private decimal evDiff;

        public decimal EVDiff
        {
            get
            {
                return evDiff;
            }
            private set
            {
                SetProperty(ref evDiff, value);
            }
        }

        [ProtoMember(14)]
        private EnumHandTag handTag;

        public EnumHandTag HandTag
        {
            get
            {
                return handTag;
            }
            private set
            {
                SetProperty(ref handTag, value);
            }
        }

        [ProtoMember(15)]
        private string playerName;

        public string PlayerName
        {
            get
            {
                return playerName;
            }
            private set
            {
                SetProperty(ref playerName, value);
            }
        }
    }
}