//-----------------------------------------------------------------------
// <copyright file="KOPlayerEmulatorProviderTest.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.AndroidBase.EmulatorProviders;
using NUnit.Framework;
using System.Diagnostics;

namespace DriveHud.Tests.TcpImportersTests.PMTests
{
    [TestFixture]
    class KOPlayerEmulatorProviderTest
    {
        //[TestCase(8112, true)]
        //[TestCase(14232, true)]
        public void CanProvideTest(int processId, bool expected)
        {
            var process = Process.GetProcessById(processId);

            Assert.IsNotNull(process);

            var koPlayerEmulatorProvider = new KOPlayerEmulatorProvider();
            var result = koPlayerEmulatorProvider.CanProvide(process);

            Assert.That(result, Is.EqualTo(expected));
        }

        //[TestCase(8112, 0x00151836)]
        //[TestCase(14232, 0x001C0738)]
        public void GetProcessWindowHandleTest(int processId, int expected)
        {
            var process = Process.GetProcessById(processId);

            Assert.IsNotNull(process);

            var koPlayerEmulatorProvider = new KOPlayerEmulatorProvider();
            var actual = koPlayerEmulatorProvider.GetProcessWindowHandle(process).ToInt32();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
