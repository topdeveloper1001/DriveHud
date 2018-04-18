//-----------------------------------------------------------------------
// <copyright file="NoxEmulatorProviderTest.cs" company="Ace Poker Solutions">
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
    /// Tests for <see cref="MomoEmulatorProvider"/>. 
    /// Requires to run several instances of MOMO manually, then update PID, and expected handles in testcases. <para />
    /// *** Possible automation is to run MOMO from <see cref="Process.Start"/>, then to find expected PID and handles
    /// </summary>
    [TestFixture]
    class MomoEmulatorProviderTest
    {
        //[TestCase(4532, true)]
        //[TestCase(21080, true)]
        //[TestCase(14112, false)]
        public void CanProvideTest(int processId, bool expected)
        {
            var process = Process.GetProcessById(processId);

            Assert.IsNotNull(process);

            var noxEmulatorProvider = new MomoEmulatorProvider();
            var result = noxEmulatorProvider.CanProvide(process);

            Assert.That(result, Is.EqualTo(expected));
        }

        //[TestCase(4532, 0x001F10F4)]
        //[TestCase(21080, 0x00D40EB2)]
        public void GetProcessWindowHandleTest(int processId, int expected)
        {
            var process = Process.GetProcessById(processId);

            Assert.IsNotNull(process);

            var noxEmulatorProvider = new MomoEmulatorProvider();
            var actual = noxEmulatorProvider.GetProcessWindowHandle(process).ToInt32();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}