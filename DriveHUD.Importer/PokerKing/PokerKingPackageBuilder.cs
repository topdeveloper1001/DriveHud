//-----------------------------------------------------------------------
// <copyright file="PokerKingPackageBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerKing.Model;
using Model;
using System;
using System.Linq;

namespace DriveHUD.Importers.PokerKing
{
    internal class PokerKingPackageBuilder : IPackageBuilder<PokerKingPackage>
    {
        public bool TryParse(byte[] bytes, int startingPosition, out PokerKingPackage package)
        {
            try
            {
                var body = bytes.Skip(startingPosition).ToArray();

                var packageTypeBytes = body.Take(2).ToArray();

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(packageTypeBytes);
                }

                var packageType = BitConverter.ToInt16(packageTypeBytes, 0);

                package = new PokerKingPackage
                {
                    PackageType = (PackageType)packageType,
                    Body = body.Skip(8).ToArray(),
                    Timestamp = DateTime.Now
                };

                return true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PKCatcher, $"Failed to parse package", e);
            }

            package = null;

            return false;
        }
    }
}