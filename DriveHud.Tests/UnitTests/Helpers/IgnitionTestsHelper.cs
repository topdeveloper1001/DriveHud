//-----------------------------------------------------------------------
// <copyright file="IgnitionTestsHelper.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DriveHud.Tests.UnitTests.Helpers
{
    internal class IgnitionTestsHelper
    {
        public static List<byte[]> PrepareInfoTestData(string fileName)
        {
            // read test data from file
            var infoDataText = File.ReadAllText(fileName);

            // split data by new lines
            var splittedInfoData = infoDataText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // convert string to bytes (utf-8)
            var testData = splittedInfoData.Select(s => Encoding.UTF8.GetBytes(s)).ToList();

            return testData;
        }
    }
}