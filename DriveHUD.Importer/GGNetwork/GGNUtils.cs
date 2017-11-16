//-----------------------------------------------------------------------
// <copyright file="GGNUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.GGNetwork
{
    internal class GGNUtils
    {
        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckData(byte[] data)
        {
            return data.Length > 44;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetStartIndex(byte[] data)
        {
            if (data[31] == 0x1F && data[32] == 0x8B) return 31;
            if (data[32] == 0x1F && data[33] == 0x8B) return 32;
            if (data[33] == 0x1F && data[34] == 0x8B) return 33;
            if (data[34] == 0x1F && data[35] == 0x8B) return 34;
            if (data[35] == 0x1F && data[36] == 0x8B) return 35;
            if (data[36] == 0x1F && data[37] == 0x8B) return 36;
            if (data[37] == 0x1F && data[38] == 0x8B) return 37;
            if (data[38] == 0x1F && data[39] == 0x8B) return 38;
            if (data[39] == 0x1F && data[40] == 0x8B) return 39;
            if (data[40] == 0x1F && data[41] == 0x8B) return 40;
            if (data[41] == 0x1F && data[42] == 0x8B) return 41;
            if (data[42] == 0x1F && data[43] == 0x8B) return 42;
            if (data[43] == 0x1F && data[44] == 0x8B) return 43;

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<byte[]> DecompressDataAsync(byte[] data)
        {
            try
            {
                using (var ms = new MemoryStream(data))
                {
                    using (var ds = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        using (var rs = new MemoryStream())
                        {
                            await ds.CopyToAsync(rs);
                            return rs.ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<string> ExtractDataAsync(byte[] data)
        {
            if (!CheckData(data)) return null;

            var index = GetStartIndex(data);

            var buff = data.Skip(index).ToArray();

            var arr = await DecompressDataAsync(buff);

            if (arr == null) return string.Empty;

            return index == 0 ? string.Empty : Encoding.UTF8.GetString(arr);
        }

        /// <summary>
        /// Gets the <see cref="GGNDataType"/> of input data
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static GGNDataType GetDataType(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return GGNDataType.Unknown;
            }

            if (input.StartsWith("{\"Histories\"") && !input.Contains("TourneyBrandName"))
            {
                return GGNDataType.CashGameHandHistories;
            }

            if (input.StartsWith("{\"HandHistory\"") && !input.Contains("TourneyBrandName"))
            {
                return GGNDataType.CashGameHandHistory;
            }

            if (input.StartsWith("{\"Histories\"") && input.Contains("TourneyBrandName"))
            {
                return GGNDataType.TourneyHandHistories;
            }

            if (input.StartsWith("{\"HandHistory\"") && input.Contains("TourneyBrandName"))
            {
                return GGNDataType.TourneyHandHistory;
            }

            return GGNDataType.Unknown;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HandHistoryInformation DeserializeHandHistory(string data)
        {
            return JsonConvert.DeserializeObject<HandHistoryInformation>(data);
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HandHistoriesInformation DeserializeHandHistories(string data)
        {
            return JsonConvert.DeserializeObject<HandHistoriesInformation>(data);
        }

        public static string ReplaceMoneyWithChipsInTitle(string tableName)
        {
            var lastHyphenIndex = tableName.LastIndexOf('-');

            if (lastHyphenIndex <= 0)
            {
                return tableName;
            }

            var tournamentName = tableName.Substring(0, lastHyphenIndex - 1).Trim();
            var tableNumber = tableName.Substring(lastHyphenIndex).Trim();

            var dollarIndex = tournamentName.LastIndexOf('$');

            if (dollarIndex <= 0)
            {
                return tableName;
            }

            var buyinText = tournamentName.Substring(dollarIndex + 1);

            decimal buyin = 0;

            if (!decimal.TryParse(buyinText, NumberStyles.Any, new CultureInfo("en-US"), out buyin))
            {
                return tableName;
            }

            buyin *= 1000;

            tournamentName = tournamentName.Substring(0, dollarIndex).Trim();

            return $"{tournamentName} {buyin.ToString("N0", new CultureInfo("en-US"))} {tableNumber}";
        }
    }
}