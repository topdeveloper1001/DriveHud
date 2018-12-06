//-----------------------------------------------------------------------
// <copyright file="ConverterTests.cs" company="Ace Poker Solutions">
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
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Parsers.FastParser.Common;
using HandHistories.Parser.Utils.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class ConverterTests
    {
        private const string commonHandsFolder = "UnitTests\\TestData\\CommonHands";

        private readonly Dictionary<string, HandHistories.Objects.Hand.HandHistory> parsedHandHistories = new Dictionary<string, HandHistories.Objects.Hand.HandHistory>();

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("Common-Hand-Cash-8-max-1.xml", "2032602", EnumPosition.SB)]
        [TestCase("Common-Hand-Cash-8-max-1.xml", "2081694", EnumPosition.BB)]
        [TestCase("Common-Hand-Cash-8-max-1.xml", "144083", EnumPosition.STRDL)]
        [TestCase("Common-Hand-Cash-8-max-1.xml", "133969", EnumPosition.MP1)]
        [TestCase("Common-Hand-Cash-8-max-1.xml", "2037106", EnumPosition.MP2)]
        [TestCase("Common-Hand-Cash-8-max-1.xml", "2081907", EnumPosition.CO)]
        [TestCase("Common-Hand-Cash-8-max-1.xml", "2011933", EnumPosition.BTN)]
        public void ToPositionTest(string fileName, string playerName, EnumPosition expectedPosition)
        {
            var hand = ParseHand(fileName);

            var actualPosition = Model.Importer.Converter.ToPosition(hand, playerName);

            Assert.That(actualPosition, Is.EqualTo(expectedPosition), "Positions must be the same.");
        }

        private HandHistories.Objects.Hand.HandHistory ParseHand(string fileName)
        {
            fileName = Path.Combine(commonHandsFolder, fileName);

            if (parsedHandHistories.TryGetValue(fileName, out HandHistories.Objects.Hand.HandHistory hand))
            {
                return hand;
            }

            var handText = File.ReadAllText(fileName);

            var factory = new HandHistoryParserFactoryImpl();
            var parser = factory.GetFullHandHistoryParser(handText);

            hand = parser.ParseFullHandHistory(handText);

            parsedHandHistories[fileName] = hand;

            return hand;
        }
    }
}