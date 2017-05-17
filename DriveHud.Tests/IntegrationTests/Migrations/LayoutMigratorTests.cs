//-----------------------------------------------------------------------
// <copyright file="LayoutMigratorTests.cs" company="Ace Poker Solutions">
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System.IO;

namespace DriveHud.Tests.IntegrationTests.Migrations
{
    [TestFixture]
    class LayoutMigratorTests : BaseLayoutsMigrationTest
    {
        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml")]
        public void LayoutV2MigrationTests(string layoutFile)
        {
            var layoutFileFullName = Path.Combine(TestDataFolder, layoutFile);

            Assert.IsTrue(File.Exists(layoutFileFullName), $"File '{layoutFileFullName}' not found");


        }


    }
}