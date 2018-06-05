//-----------------------------------------------------------------------
// <copyright file="PMImporterHelper.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerMaster.Model;
using System;

namespace DriveHUD.Importers.PokerMaster
{
    internal class PMImporterHelper
    {        
        public static int PortFilter
        {
            get
            {
                return 9188;
            }
        }

        public static decimal ConvertSNGTypeToBuyIn(SNGRoomType sngRoomtype)
        {
            switch (sngRoomtype)
            {
                case SNGRoomType.QUICK_SNG:
                    return 200m;
                case SNGRoomType.NORMAL_SNG:
                    return 500m;
                case SNGRoomType.LONG_SNG:
                    return 1000m;
                case SNGRoomType.DEEP_SNG:
                    return 2000m;
                default:
                    throw new NotSupportedException($"SNGRoomType {sngRoomtype} isn't supported");
            }
        }
    }
}