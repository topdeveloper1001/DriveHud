//-----------------------------------------------------------------------
// <copyright file="HandBuilderException.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Importers
{
    public class HandBuilderException : Exception
    {
        public HandBuilderException(long gameNumber, string message) : base($"Hand #{gameNumber}: {message}")
        {
        }

        public HandBuilderException(string message)
            : base(message)
        {
        }

        public HandBuilderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}