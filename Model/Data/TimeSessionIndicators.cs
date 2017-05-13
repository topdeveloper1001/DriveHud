//-----------------------------------------------------------------------
// <copyright file="TimeSessionIndicators.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data
{
    public class TimeSessionIndicators : LightIndicators
    {
        private readonly List<DateTime> statisticsTimeCollection = new List<DateTime>();

        public TimeSessionIndicators() : base()
        {
        }

        public TimeSessionIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
        }

        public override List<DateTime> StatisticsTimeCollection
        {
            get
            {
                return statisticsTimeCollection;
            }
        }

        public override void AddStatistic(Playerstatistic statistic)
        {
            base.AddStatistic(statistic);

            statisticsTimeCollection.Add(statistic.Time);
        }
    }
}