//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.IO;
using System.Linq;

namespace DriveHUD.Common.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveEOL(this string text)
        {
            return text.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public static string RemoveWhitespace(this string text)
        {
            return new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static string RemoveInvalidFileNameChars(this string text)
        {
            return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}