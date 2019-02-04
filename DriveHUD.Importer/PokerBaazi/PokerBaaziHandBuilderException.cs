//-----------------------------------------------------------------------
// <copyright file="PokerBaaziHandBuilderException.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziHandBuilderException : HandBuilderException
    {
        public PokerBaaziHandBuilderException(PokerBaaziHandBuilderError error, long gameNumber, string message) : this(gameNumber, message)
        {
            Error = error;
        }

        public PokerBaaziHandBuilderException(long gameNumber, string message) : base($"Hand #{gameNumber}: {message}")
        {
        }

        public PokerBaaziHandBuilderException(string message)
            : base(message)
        {
        }

        public PokerBaaziHandBuilderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public PokerBaaziHandBuilderError Error { get; }
    }
}