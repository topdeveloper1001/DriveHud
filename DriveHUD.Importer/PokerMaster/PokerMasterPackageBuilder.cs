﻿//-----------------------------------------------------------------------
// <copyright file="PokerMasterPackageBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerMaster.Model;

namespace DriveHUD.Importers.PokerMaster
{
    internal class PokerMasterPackageBuilder : IPackageBuilder<PokerMasterPackage>
    {
        public bool TryParse(byte[] bytes, int startingPosition, out PokerMasterPackage package)
        {
            return SerializationHelper.TryDeserialize(bytes, startingPosition, out package);
        }
    }
}