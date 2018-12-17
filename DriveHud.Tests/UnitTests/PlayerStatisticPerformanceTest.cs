//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticPerformanceTest.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using Model;
using Model.Data;
using Model.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class PlayerStatisticPerformanceTest
    {
        [Test]
        public void Test()
        {
            ResourceRegistrator.Initialization();

            using (var pf = new PerformanceMonitor("Test"))
            {
                var indicator = new HudIndicators(new List<Stat>());

                var repository = new PlayerStatisticRepository();
                repository.SetPlayerStatisticPath(StringFormatter.GetPlayerStatisticDataFolderPath());
                repository.GetPlayerStatistic(13078).ForEach(x => indicator.AddStatistic(x));
            }
        }
    }
}
