//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticReImporterTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Tests.IntegrationTests.Base;
using DriveHUD.Importers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    [TestFixture]
    class PlayerStatisticReImporterTests : BaseDatabaseTest
    {
        protected override void InitializeDatabase()
        {
        }

        /// <summary>
        /// Initialize environment for test
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            Initalize();
        }

        [Test]
        public void ReImportTest()
        {
            using (var perfScope = new PerformanceMonitor("ReImportTest"))
            {
                var playerStatisticReImporter = new PlayerStatisticReImporter();
                playerStatisticReImporter.ReImport();
            }
        }
    }
}