//-----------------------------------------------------------------------
// <copyright file="HandHistoryTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Utils;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class HandHistoryTests
    {
        private const string TestDataFolder = "TestData";

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("HandHistoryCloneSample.xml")]
        public void TestClone(string fileName)
        {
            var handHistory = ReadHandHistory(fileName);

            var handHistoryClone = handHistory.DeepClone();

            AssertionUtils.AssertHandHistory(handHistory, handHistoryClone);
        }

        private HandHistories.Objects.Hand.HandHistory ReadHandHistory(string fileName)
        {
            var file = GetTestDataFilePath(fileName);

            FileAssert.Exists(file);

            var handHistoryText = File.ReadAllText(file);

            var handHistory = SerializationHelper.DeserializeObject<HandHistories.Objects.Hand.HandHistory>(handHistoryText);

            return handHistory;
        }

        private static string GetTestDataFilePath(string fileName)
        {
            return Path.Combine("UnitTests", "TestData", fileName);
        }
    }
}