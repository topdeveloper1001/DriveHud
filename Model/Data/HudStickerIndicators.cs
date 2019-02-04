//-----------------------------------------------------------------------
// <copyright file="HudStickerIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Collections.Generic;

namespace Model.Data
{
    public class HudStickerIndicators : HudLightIndicators
    {
        public HudStickerIndicators() : base()
        {
        }

        public HudStickerIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
        }

        public override void AddStatistic(Playerstatistic statistic)
        {
            Source += statistic;

            var unopened = statistic.IsUnopened ? 1 : 0;
            positionUnoppened?.Add(statistic.Position, unopened);

            if (gameNumberMax < statistic.GameNumber)
            {
                gameNumberMax = statistic.GameNumber;
            }

            statisticCount++;
            netWon += statistic.NetWon;
            bigBlind += statistic.BigBlind;
            netWonByBigBlind += GetDivisionResult(statistic.NetWon, statistic.BigBlind);
            evInBB += GetDivisionResult(statistic.NetWon + statistic.EVDiff, statistic.BigBlind);

            if (sessionStartTime > statistic.Time)
            {
                sessionStartTime = statistic.Time;
            }

            if (sessionEndTime < statistic.Time)
            {
                sessionEndTime = statistic.Time;
            }

            didDoubleBarrel += statistic.DidDoubleBarrel;
            couldDoubleBarrel += statistic.CouldDoubleBarrel;

            faced3Bet += statistic.FacedthreebetpreflopVirtual;
            foldedTo3Bet += statistic.FoldedtothreebetpreflopVirtual;
        }
    }
}