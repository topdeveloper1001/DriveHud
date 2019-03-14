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
            package = null;

            if (bytes.Length < 2)
            {
                return false;
            }

            var offset = bytes[1] == 0x7E ? 4 : 2;

            if (bytes.Length <= offset)
            {
                return false;
            }
         
            package = new Adda52Package
            {
                Bytes = bytes.Skip(offset).ToArray()
            };

            return package != null && package.Bytes.Length != 0;
        }
    }
}