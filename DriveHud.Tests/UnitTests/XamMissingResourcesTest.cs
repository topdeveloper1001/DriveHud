//-----------------------------------------------------------------------
// <copyright file="XamMissingResourcesTest.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class XamMissingResourcesTest
    {
        private readonly static Regex[] TextAttributesRegex = new[] {
            new Regex("Text=\"[^\\{]+", RegexOptions.Compiled),
            new Regex(@"\sContent=""[^\{]+", RegexOptions.Compiled),
            new Regex("Header=\"[^\\{]+", RegexOptions.Compiled),
            new Regex("Title=\"[^\\{]+", RegexOptions.Compiled),
        };

        private readonly static string[] ExcludedPaths = new[]
        {
            @"..\..\..\Simulator",
            @"..\..\..\tools",
            "SplashWindow.xaml",
            "MainWindowView.xaml"
        };

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        public void XamlHaveNoHardcodedTextsTest()
        {
            var xamlFiles = Directory.GetFiles(@"..\..\..", "*.xaml", SearchOption.AllDirectories);

            var checkResults = new List<XamlCheckResult>();

            xamlFiles.ForEach(x =>
            {
                if (ExcludedPaths.Any(p => x.Contains(p)))
                {
                    return;
                }

                var results = CheckXamlForHardcodedTexts(x);

                if (results.Count > 0)
                {
                    Console.WriteLine($"File: {x}");
                    results.ForEach(p => Console.WriteLine($"Line {p.Line}: {p.Text}"));
                    checkResults.AddRange(results);
                }
            });

            Assert.That(checkResults.Count, Is.EqualTo(0), "Found incorrect files. Please check output.");
        }

        private List<XamlCheckResult> CheckXamlForHardcodedTexts(string file)
        {
            var lines = File.ReadAllLines(file);

            var checkResults = new List<XamlCheckResult>();

            for (var i = 0; i < lines.Length; i++)
            {
                foreach (var regex in TextAttributesRegex)
                {
                    var matches = regex.Matches(lines[i]);

                    foreach (Match match in matches)
                    {
                        if (!CheckMatchValue(match.Value))
                        {
                            continue;
                        }

                        var checkResult = new XamlCheckResult
                        {
                            Line = i + 1,
                            Text = match.Value
                        };

                        checkResults.Add(checkResult);
                    }
                }
            }

            return checkResults;
        }

        private static bool CheckMatchValue(string matchText)
        {
            var startQuoteIndex = matchText.IndexOf('"');

            if (startQuoteIndex < 0)
            {
                return false;
            }

            var endQuoteIndex = matchText.IndexOf('"', startQuoteIndex + 1);

            if (endQuoteIndex < 0 || (endQuoteIndex - startQuoteIndex) <= 1)
            {
                return false;
            }

            var text = matchText.Substring(startQuoteIndex + 1, endQuoteIndex - startQuoteIndex - 1);

            if (text.Length < 3 || (text.StartsWith("M ") && text.EndsWith(" Z")) 
                || text.Count(x => char.IsLetter(x)) < 3)
            {
                return false;
            }

            return true;
        }

        private class XamlCheckResult
        {
            public int Line { get; set; }

            public string Text { get; set; }
        }
    }
}