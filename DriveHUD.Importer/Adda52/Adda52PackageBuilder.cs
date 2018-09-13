//-----------------------------------------------------------------------
// <copyright file="Adda52PackageBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.AndroidBase;
using System;
using System.Linq;

namespace DriveHUD.Importers.Adda52
{
    internal class Adda52PackageBuilder : IPackageBuilder<Adda52Package>
    {
        public bool TryParse(byte[] bytes, int startingPosition, out Adda52Package package)
        {
            var offset = bytes[1] == 0x7E ? 4 : 2;

            package = null;

            if (bytes.Length <= offset)
            {
                return false;
            }

            var jsonBytes = bytes.Skip(offset).SkipWhile(x => x != 0x7B).ToArray();
            var lastBracketsIndex = Array.FindLastIndex(jsonBytes, x => x == 0x7D);

            if (lastBracketsIndex > 0)
            {
                package = new Adda52Package
                {
                    Bytes = jsonBytes.Take(lastBracketsIndex + 1).ToArray()
                };
            }

            return package != null && package.Bytes.Length != 0;
        }
    }
}