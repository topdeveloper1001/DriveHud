//-----------------------------------------------------------------------
// <copyright file="DataServiceHelper.cs" company="Ace Poker Solutions">
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
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHud.Tests.UnitTests.Helpers
{
    /// <summary>
    /// Class helper to get specific data from db files
    /// </summary>
    public class DataServiceHelper
    {
        /// <summary>
        /// Get player statistic from specified file
        /// </summary>
        /// <param name="file">File with statistic</param>
        /// <param name="filter">Filter</param>
        /// <returns>Player statistic collection</returns>
        public static IList<Playerstatistic> GetPlayerStatisticFromFile(string file, Func<Playerstatistic, bool> filter = null)
        {
            List<Playerstatistic> result = new List<Playerstatistic>();

            var allLines = File.ReadAllLines(file);

            foreach (var line in allLines)
            {
                byte[] byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/'));

                using (MemoryStream afterStream = new MemoryStream(byteAfter64))
                {
                    var stat = Serializer.Deserialize<Playerstatistic>(afterStream);

                    stat.CalculatePositionalStats();

                    if (filter != null)
                    {
                        if (filter(stat))
                        {
                            result.Add(stat);
                        }
                    }
                    else
                    {
                        result.Add(stat);
                    }
                }
            }

            return result;
        }
    }
}