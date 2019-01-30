//-----------------------------------------------------------------------
// <copyright file="DHBasedHandExportPreparingService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;

namespace Model.Export
{
    internal class DHBasedHandExportPreparingService : CommonHandExportPreparingService
    {
        public override string PrepareHand(string hand, EnumPokerSites site)
        {
            var parserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            var parser = parserFactory.GetFullHandHistoryParser(site);

            var handHistory = parser.ParseFullHandHistory(hand);

            var convert = ServiceLocator.Current.GetInstance<IHandHistoryToIPokerConverter>();

            hand = convert.Convert(handHistory);

            return hand;
        }
    }
}