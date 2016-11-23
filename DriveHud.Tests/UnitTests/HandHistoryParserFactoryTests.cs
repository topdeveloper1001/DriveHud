﻿using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Utils.Extensions;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class HandHistoryParserFactoryTests
    {
        private const string testFolder = "UnitTests\\TestData";

        [Test]
        [TestCase("iPoker.xml", EnumPokerSites.IPoker)]
        [TestCase("PokerStarsHands.txt", EnumPokerSites.PokerStars)]
        [TestCase("888PokerHands.txt", EnumPokerSites.Poker888)]
        public void GetFullHandHistoryParserReturnsExpectedParser(string fileName, EnumPokerSites expectedSite)
        {
            var file = Path.Combine(testFolder, fileName);

            if (!File.Exists(file))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", file));
            }

            var handText = File.ReadAllText(file);

            var handHistoryParserFactory = new HandHistoryParserFactoryImpl();

            var parser = handHistoryParserFactory.GetFullHandHistoryParser(handText);

            Assert.That(parser.SiteName, Is.EqualTo(expectedSite));
        }

        [Test]        
        [TestCase("PokerStarsEncoding.txt", EnumPokerSites.PokerStars)]        
        public void GetFullHandHistoryWithEncodingParserReturnsExpectedParser(string fileName, EnumPokerSites expectedSite)
        {
            var file = Path.Combine(testFolder, fileName);

            if (!File.Exists(file))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", file));
            }

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(0, SeekOrigin.Begin);

                var data = new byte[fs.Length - fs.Position];
                
                if (data.Length == 0)
                {
                    return;
                }

                fs.Read(data, 0, data.Length);

                var handText = Encoding.UTF8.GetString(data);

                EnumPokerSites siteName;

                var parseResult = EnumPokerSitesExtension.TryParse(handText, out siteName);

                Assert.IsTrue(parseResult);                               
                Assert.That(siteName, Is.EqualTo(expectedSite));
            }
        }
    }
}