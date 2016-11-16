//-----------------------------------------------------------------------
// <copyright file="ParserUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.GameDescription;
using System;

namespace HandHistories.Parser.Utils.FastParsing
{
    public static class ParserUtils
    {
        public static TournamentSpeed ParseTournamentSpeed(string input)
        {
            if (input.IndexOf("Super Turbo", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return TournamentSpeed.SuperTurbo;
            }

            if (input.IndexOf("Hyper Turbo", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return TournamentSpeed.HyperTurbo;
            }

            if (input.IndexOf("Turbo", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return TournamentSpeed.Turbo;
            }

            return TournamentSpeed.Regular;
        }
    }
}