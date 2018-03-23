//-----------------------------------------------------------------------
// <copyright file="EquitySolverTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Builders.iPoker;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Enums;
using Model.Solvers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class EquitySolverTests
    {
        private IUnityContainer unityContainer;

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            unityContainer = new UnityContainer();

            unityContainer.RegisterType<IPokerEvaluator, HoldemEvaluator>(GeneralGameTypeEnum.Holdem.ToString());
            unityContainer.RegisterType<IPokerEvaluator, OmahaEvaluator>(GeneralGameTypeEnum.Omaha.ToString());
            unityContainer.RegisterType<IPokerEvaluator, OmahaEvaluator>(GeneralGameTypeEnum.OmahaHiLo.ToString());

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestCaseSource("TestDataCase")]
        public void CalculateEquityTest(TestData testData)
        {
            var handHistory = ReadHandHistory(testData.HandHistoryFile);

            var equitySolver = new EquitySolver();

            var actualResult = equitySolver.CalculateEquity(handHistory);

            Assert.That(actualResult.Count, Is.EqualTo(testData.ExpectedResult.Length));

            Assert.Multiple(() =>
            {
                foreach (var expectedResult in testData.ExpectedResult)
                {
                    Assert.IsTrue(actualResult.ContainsKey(expectedResult.PlayerName), $"{expectedResult.PlayerName} has not been found in results.");

                    Assert.That(actualResult[expectedResult.PlayerName].Equity * 100, Is.EqualTo(expectedResult.Equity).Within(0.01), "Equity must be equal");
                    Assert.That(actualResult[expectedResult.PlayerName].EVDiff, Is.EqualTo(expectedResult.EVDiff).Within(0.001), "EV Diff must be equal");
                }
            });
        }

        private HandHistories.Objects.Hand.HandHistory ReadHandHistory(string file)
        {
            file = Path.Combine("UnitTests\\TestData\\EquitySolverData", file);

            FileAssert.Exists(file);

            var handHistoryText = File.ReadAllText(file);

            Assert.IsNotEmpty(handHistoryText);

            var parserFactory = new HandHistoryParserFactoryImpl();

            var parser = parserFactory.GetFullHandHistoryParser(handHistoryText);

            var handHistory = parser.ParseFullHandHistory(handHistoryText);

            Assert.IsNotNull(handHistory);

            return handHistory;
        }

        private static List<TestData> TestDataCase = new List<TestData>
        {
            new TestData
            {
                HandHistoryFile = "AllIn-on-BB-Hero-raised.xml",
                ExpectedResult = new []
                {
                    new EquityData
                    {
                        PlayerName = "Hero",
                        Equity = 19.7m,
                        EVDiff = 8.865m
                    },
                    new EquityData
                    {
                        PlayerName = "P6_391468UT",
                        Equity = 80.3m,
                        EVDiff = -8.865m
                    },
                    new EquityData
                    {
                        PlayerName = "P1_928114KV",
                        Equity = 0,
                        EVDiff = 0
                    },
                    new EquityData
                    {
                        PlayerName = "P2_875424AY",
                        Equity = 0,
                        EVDiff = 0
                    }
                }
            }
        };

        public class TestData
        {
            public string HandHistoryFile { get; set; }

            public EquityData[] ExpectedResult { get; set; }
        }
    }
}