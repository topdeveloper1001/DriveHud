//-----------------------------------------------------------------------
// <copyright file="MemuEmulatorProviderTest.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerMaster.EmulatorProviders;
using NUnit.Framework;
using System.Diagnostics;

namespace PMCatcher.Tests
{
    /// <summary>
    /// Tests for <see cref="MemuEmulatorProvider"/>. 
    /// Requires to run several instances of MEmu manually, then update PID, and expected handles in testcases. <para />
    /// *** Possible automation is to run MEmu from <see cref="Process.Start"/>, then to find expected PID and handles
    /// </summary>
    [TestFixture]
    class MemuEmulatorProviderTest
    {
        //[TestCase(2696, true)]
        //[TestCase(10576, true)]
        //[TestCase(22496, false)]
        public void CanProvideTest(int processId, bool expected)
        {
            var process = Process.GetProcessById(processId);

            Assert.IsNotNull(process);

            var memuEmulatorProvider = new MemuEmulatorProvider();
            var result = memuEmulatorProvider.CanProvide(process);

            Assert.That(result, Is.EqualTo(expected));
        }

        //[TestCase(2696, 2951682)]
        //[TestCase(10576, 2365226)]
        public void GetProcessWindowHandleTest(int processId, int expected)
        {
            var process = Process.GetProcessById(processId);

            Assert.IsNotNull(process);

            var noxEmulatorProvider = new MemuEmulatorProvider();
            var actual = noxEmulatorProvider.GetProcessWindowHandle(process).ToInt32();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}