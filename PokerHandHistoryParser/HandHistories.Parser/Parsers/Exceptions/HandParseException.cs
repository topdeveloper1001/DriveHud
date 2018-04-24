//-----------------------------------------------------------------------
// <copyright file="HandParseException.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace HandHistories.Parser.Parsers.Exceptions
{
    public abstract class HandParseException : Exception
    {
        private readonly string _handText;

        protected HandParseException(string handText)
            : base()
        {
            _handText = handText;
        }

        protected HandParseException(string[] handLines, string message) : base(message)
        {
            _handText = string.Join(Environment.NewLine, handLines);
        }

        protected HandParseException(string handText, string message) : base(message)
        {
            _handText = handText;
        }

        public string HandText
        {
            get { return _handText; }
        }
    }

    public class InvalidHandException : HandParseException
    {
        public InvalidHandException(string handText) : base(handText)
        {
        }

        public InvalidHandException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class HandActionException : HandParseException
    {
        public HandActionException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class ExtraHandParsingAction : HandParseException
    {
        public ExtraHandParsingAction(string handText)
            : base(handText)
        {
        }
    }

    public class PlayersException : HandParseException
    {
        public PlayersException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class UnrecognizedGameTypeException : HandParseException
    {
        public UnrecognizedGameTypeException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class ParseHandDateException : HandParseException
    {
        public ParseHandDateException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class HandIdException : HandParseException
    {
        public HandIdException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class TournamentIdException : HandParseException
    {
        public TournamentIdException(string handText, string message)
            : base(handText, message)
        {
        }
        public TournamentIdException(string[] handLines, string message)
          : base(handLines, message)
        {
        }
    }

    public class TableNameException : HandParseException
    {
        public TableNameException(string[] handLines, string message)
            : base(handLines, message)
        {
        }

        public TableNameException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class DealerPositionException : HandParseException
    {
        public DealerPositionException(string[] handLines, string message)
            : base(handLines, message)
        {
        }

        public DealerPositionException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class PokerFormatException : HandParseException
    {
        public PokerFormatException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class SeatTypeException : HandParseException
    {
        public SeatTypeException(string[] handLines, string message)
            : base(handLines, message)
        {
        }

        public SeatTypeException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class TableTypeException : HandParseException
    {
        public TableTypeException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class LimitException : HandParseException
    {
        public LimitException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class BuyinException : HandParseException
    {
        public BuyinException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class CurrencyException : HandParseException
    {
        public CurrencyException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class CardException : HandParseException
    {
        public CardException(string handText, string message)
            : base(handText, message)
        {
        }
    }
}