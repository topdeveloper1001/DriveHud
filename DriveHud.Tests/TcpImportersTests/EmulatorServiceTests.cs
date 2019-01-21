//-----------------------------------------------------------------------
// <copyright file="EmulatorServiceTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerKing.Model;
using NUnit.Framework;
using System.Diagnostics;

namespace DriveHud.Tests.TcpImportersTests
{
    /// <summary>
    /// Tests for emulator service, requires to have one or more emulators running
    /// </summary>
    [TestFixture]
    class EmulatorServiceTests
    {
        //[TestCase(20792)]
        public void TestAdbCommand(int processId)
        {
            var process = Process.GetProcessById(processId);

            Assert.IsNotNull(process, $"Process must be running.");

            var emulatorService = new EmulatorService();
            // load emulator data to service
            emulatorService.GetTableWindowHandle(process);

            var outputs = emulatorService.ExecuteAdbCommand(process,
                 "shell",
                 "\"cat /data/data/com.ylc.qp.Pokermate/shared_prefs/Cocos2dxPrefsFile.xml\"");

            Assert.Multiple(() =>
            {
                foreach (var output in outputs)
                {
                    var result = PKLoginResponse.TryParse(output, out PKLoginResponse loginResponse);
                    Assert.IsTrue(result);
                }
            });
        }
    }
}