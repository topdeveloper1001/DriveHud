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
using System.Linq;

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
            unityContainer.RegisterType<IPokerEvaluator, OmahaHiLoEvaluator>(GeneralGameTypeEnum.OmahaHiLo.ToString());

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestCaseSource("TestDataCase", Category = "EquitySolver.EvDiff")]
        public void CalculateEquityTest(string handHistoryFile, double tolerance, EquityData[] expectedResults)
        {
            var handHistory = ReadHandHistory(handHistoryFile);

            var equitySolver = new EquitySolver();

            var actualResult = equitySolver.CalculateEquity(handHistory)
                .Where(x => x.Value.Equity != 0 || x.Value.EVDiff != 0)
                .ToDictionary(x => x.Key, x => x.Value);

            Assert.That(actualResult.Count, Is.EqualTo(expectedResults.Length));

            Assert.Multiple(() =>
            {
                foreach (var expectedResult in expectedResults)
                {
                    Assert.IsTrue(actualResult.ContainsKey(expectedResult.PlayerName), $"{expectedResult.PlayerName} has not been found in results.");

                    Assert.That(actualResult[expectedResult.PlayerName].Equity * 100, Is.EqualTo(expectedResult.Equity).Within(0.1), $"Equity must be equal [{expectedResult.PlayerName}]");
                    Assert.That(actualResult[expectedResult.PlayerName].EVDiff, Is.EqualTo(expectedResult.EVDiff).Within(tolerance), $"EV Diff must be equal [{expectedResult.PlayerName}]");
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

        private static IEnumerable<TestCaseData> TestDataCase()
        {
            yield return new TestCaseData(
                "AllIn-on-BB-Hero-raised.xml",
                0.01,
                new[]
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
                    }
                }).SetName("EVDifference: AllIn-on-BB-Hero-raised");

            yield return new TestCaseData(
                "Hero-AllIn-Preflop-Rake.txt",
                0.01,
                new[]
                {
                    new EquityData
                    {
                        PlayerName = "Hero",
                        Equity = 43.1m,
                        EVDiff = 77.28m
                    },
                    new EquityData
                    {
                        PlayerName = "darasxxx",
                        Equity = 56.9m,
                        EVDiff = -77.28m
                    }
                }).SetName("EVDifference: Hero-AllIn-Preflop-Rake");

            yield return new TestCaseData(
                "Hero-ExpectedValue-1.xml",
                0.01,
                new[]
                {
                    new EquityData
                    {
                        PlayerName = "Hero",
                        Equity = 90.9m,
                        EVDiff = -7.54m
                    },
                    new EquityData
                    {
                        PlayerName = "P3_600431XL",
                        Equity = 9.1m,
                        EVDiff = 7.54m
                    }
                }).SetName("EVDifference: Hero-ExpectedValue-1");

            yield return new TestCaseData(
                "DURKADURDUR-ExpectedValue-2.txt",
                0.2,
                new[]
                {
                    new EquityData
                    {
                        PlayerName = "DURKADURDUR",
                        Equity = 76.2m,
                        EVDiff = -147.86m
                    },
                    new EquityData
                    {
                        PlayerName = "Nehekus",
                        Equity = 0m,
                        EVDiff = -45.17m
                    },
                    new EquityData
                    {
                        PlayerName = "skipper8922",
                        Equity = 23.8m,
                        EVDiff = 193.02m
                    }
                }).SetName("EVDifference: DURKADURDUR-ExpectedValue-2");

            yield return new TestCaseData(
                "Hero-ExpectedValue-2.txt",
                1.5,
                new[]
                {
                    new EquityData
                    {
                        PlayerName = "Hero",
                        Equity = 21.6m,
                        EVDiff = -1889.93m
                    },
                    new EquityData
                    {
                        PlayerName = "P6_391468UT",
                        Equity = 61.8m,
                        EVDiff = -34.39m
                    },
                    new EquityData
                    {
                        PlayerName = "P2_875424AY",
                        Equity = 16.6m,
                        EVDiff = 1924.32m
                    }
                }).SetName("EVDifference: Hero-ExpectedValue-2");

            yield return new TestCaseData(
              "Hero-ExpectedValue-3.txt",
              0.01,
              new[]
              {
                    new EquityData
                    {
                        PlayerName = "Hero",
                        Equity = 60m,
                        EVDiff = 81.72m
                    },
                    new EquityData
                    {
                        PlayerName = "scriber98",
                        Equity = 40m,
                        EVDiff = -81.72m
                    }
              }).SetName("EVDifference: Hero-ExpectedValue-3");

            yield return new TestCaseData(
                "Hero-ExpectedValue-4.txt",
                0.01,
                new[]
                {
                        new EquityData
                        {
                            PlayerName = "Hero",
                            Equity = 16.7m,
                            EVDiff = 12.47m
                        },
                        new EquityData
                        {
                            PlayerName = "scriber98",
                            Equity = 83.3m,
                            EVDiff = -12.47m
                        }
                }).SetName("EVDifference: Hero-ExpectedValue-4");

            yield return new TestCaseData(
                "Hero-ExpectedValue-5.txt",
                0.01,
                new[]
                {
                        new EquityData
                        {
                            PlayerName = "Hero",
                            Equity = 3.3m,
                            EVDiff = 25.47m
                        },
                        new EquityData
                        {
                            PlayerName = "Desateur",
                            Equity = 96.7m,
                            EVDiff = -25.47m
                        }
                }).SetName("EVDifference: Hero-ExpectedValue-5");

            yield return new TestCaseData(
                "Peon84-ExpectedValue-1.txt",
                1.5,
                new[]
                {
                        new EquityData
                        {
                            PlayerName = "Peon84",
                            Equity = 26.3m,
                            EVDiff = 13168m
                        },
                        new EquityData
                        {
                            PlayerName = "Kalkulatorek",
                            Equity = 73.7m,
                            EVDiff = -13168m
                        }
                }).SetName("EVDifference: Peon84-ExpectedValue-1");

            yield return new TestCaseData(
                "OmahaHiLo-ExpectedValue-1.txt",
                0.5,
                new[]
                {
                        new EquityData
                        {
                            PlayerName = "getlucky675",
                            Equity = 47.2m,
                            EVDiff = -213m
                        },
                        new EquityData
                        {
                            PlayerName = "akashra",
                            Equity = 43.1m,
                            EVDiff = -533m
                        },
                        new EquityData
                        {
                            PlayerName = "ThreeBetStack",
                            Equity = 9.7m,
                            EVDiff = 747m
                        }
                }).SetName("EVDifference: OmahaHiLo-ExpectedValue-1");

            yield return new TestCaseData(
                "OmahaHiLo-ExpectedValue-2.txt",
                0.01,
                new[]
                {
                        new EquityData
                        {
                            PlayerName = "norefedrina",
                            Equity = 73.8m,
                            EVDiff = 0.64m
                        },
                        new EquityData
                        {
                            PlayerName = "nuvolai1967",
                            Equity = 26.3m,
                            EVDiff = -0.64m
                        }
                }).SetName("EVDifference: OmahaHiLo-ExpectedValue-2");

            yield return new TestCaseData(
               "OmahaHiLo-ExpectedValue-3.txt",
               0.01,
               new[]
               {
                        new EquityData
                        {
                            PlayerName = "norefedrina",
                            Equity = 60m,
                            EVDiff = 0.23m
                        },
                        new EquityData
                        {
                            PlayerName = "nuvolai1967",
                            Equity = 40m,
                            EVDiff = -0.23m
                        }
               }).SetName("EVDifference: OmahaHiLo-ExpectedValue-3");

            yield return new TestCaseData(
              "OmahaHiLo-ExpectedValue-4.txt",
              0.01,
              new[]
              {
                        new EquityData
                        {
                            PlayerName = "norefedrina",
                            Equity = 26.5m,
                            EVDiff = -1.29m
                        },
                        new EquityData
                        {
                            PlayerName = "NORIALEX99",
                            Equity = 22.3m,
                            EVDiff = 2.09m
                        },
                        new EquityData
                        {
                            PlayerName = "bo12327",
                            Equity = 51.2m,
                            EVDiff = -0.8m
                        }
              }).SetName("EVDifference: OmahaHiLo-ExpectedValue-4");
        }

        public class TestData
        {
            public string HandHistoryFile { get; set; }

            public EquityData[] ExpectedResult { get; set; }
        }
    }
}