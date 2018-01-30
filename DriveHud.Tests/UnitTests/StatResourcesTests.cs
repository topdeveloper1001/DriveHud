//-----------------------------------------------------------------------
// <copyright file="StatResourcesTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using Model.Enums;
using NUnit.Framework;
using System;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class StatResourcesTests
    {
        #region SetUp

        /// <summary>
        /// Initialize environment for test
        /// </summary>
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            ResourceRegistrator.Initialization();
        }

        #endregion

        [Test]
        public void StatsArePresentedInEnumResources()
        {
            Assert.Multiple(() =>
            {
                var stats = Enum.GetValues(typeof(Stat)).OfType<Stat>().Except(new[] { Stat.LineBreak }).ToArray();

                foreach (var stat in stats)
                {
                    var statText = CommonResourceManager.Instance.GetEnumResource(stat);

                    Assert.IsNotNull(statText, $"Stat '{stat}' wasn't found in Enum.resx");
                    Assert.IsNotEmpty(statText, $"Stat '{stat}' wasfound in Enum.resx, but it's empty");
                }
            });
        }
    }
}