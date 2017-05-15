//-----------------------------------------------------------------------
// <copyright file="UtilsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        [TestCase("", false)]
        [TestCase("1", false)]
        [TestCase("22@22.232", true)]
        [TestCase("peon84@yandex.ru", true)]
        [TestCase("al.v.dan@gmail.com", true)]

        public void TestIsValidEmail(string email, bool expected)
        {
            var actual = Utils.IsValidEmail(email);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(IsInDateRangeTestClass), "GetData")]
        public void TestIsInDateRange(IsInDateRangeTestClass testData)
        {
            var actual = Utils.IsDateInDateRange(testData.Date, testData.StartDate, testData.EndDate, testData.RangeExtension);
            Assert.That(actual, Is.EqualTo(testData.ExpectedResult), $"\r\nDate: {testData.Date} \r\nStartDate: {testData.StartDate} \r\nEndDate: {testData.EndDate} \r\nRangeExt: {testData.RangeExtension}");
        }

        public class IsInDateRangeTestClass
        {
            public DateTime Date { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            public TimeSpan RangeExtension { get; set; }

            public bool ExpectedResult { get; set; }

            public static IEnumerable<IsInDateRangeTestClass> GetData()
            {
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 14, 0, 0),
                    StartDate = new DateTime(2017, 5, 15, 13, 30, 0),
                    EndDate = new DateTime(2017, 5, 15, 14, 30, 0),
                    RangeExtension = TimeSpan.FromMinutes(0),
                    ExpectedResult = true
                };
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 13, 1, 0),
                    StartDate = new DateTime(2017, 5, 15, 13, 30, 0),
                    EndDate = new DateTime(2017, 5, 15, 14, 30, 0),
                    RangeExtension = TimeSpan.FromMinutes(30),
                    ExpectedResult = true
                };
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 14, 59, 0),
                    StartDate = new DateTime(2017, 5, 15, 13, 30, 0),
                    EndDate = new DateTime(2017, 5, 15, 14, 30, 0),
                    RangeExtension = TimeSpan.FromMinutes(30),
                    ExpectedResult = true
                };
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 12, 59, 0),
                    StartDate = new DateTime(2017, 5, 15, 13, 30, 0),
                    EndDate = new DateTime(2017, 5, 15, 14, 30, 0),
                    RangeExtension = TimeSpan.FromMinutes(30),
                    ExpectedResult = false
                };
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 15, 01, 0),
                    StartDate = new DateTime(2017, 5, 15, 13, 30, 0),
                    EndDate = new DateTime(2017, 5, 15, 14, 30, 0),
                    RangeExtension = TimeSpan.FromMinutes(30),
                    ExpectedResult = false
                };
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 14, 0, 0),
                    StartDate = null,
                    EndDate = new DateTime(2017, 5, 15, 14, 45, 0),
                    RangeExtension = TimeSpan.FromMinutes(30),
                    ExpectedResult = true
                };
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 14, 0, 0),
                    StartDate = new DateTime(2017, 5, 15, 13, 25, 0),
                    EndDate = null,
                    RangeExtension = TimeSpan.FromMinutes(30),
                    ExpectedResult = true
                };
                yield return new IsInDateRangeTestClass
                {
                    Date = new DateTime(2017, 5, 15, 14, 0, 0),
                    StartDate = DateTime.MinValue,
                    EndDate = DateTime.MaxValue,
                    RangeExtension = TimeSpan.FromMinutes(30),
                    ExpectedResult = true
                };
            }
        }
    }
}