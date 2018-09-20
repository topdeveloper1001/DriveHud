//-----------------------------------------------------------------------
// <copyright file="Utils.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DeployLX.Licensing.v5;
using DriveHUD.Common.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace DriveHUD.Common.Utils
{
    /// <summary>
    /// Utilities
    /// </summary>
    public static class Utils
    {
        private static readonly Random random = new Random();

        private static string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Generate random player name (Thread-safe)
        /// </summary>
        /// <param name="seat">Player seat</param>
        /// <returns>Player name</returns>
        public static string GenerateRandomPlayerName(int seat)
        {
            var random = RandomProvider.GetThreadRandom();

            var randomNumber = random.Next(100000, 999999);

            Func<string, char> getRandomChar = text =>
            {
                var randomIndex = random.Next(0, text.Length - 1);
                return text[randomIndex];
            };

            var playerName = string.Format(CultureInfo.InvariantCulture, "P{0}_{1}{2}{3}", seat, randomNumber, getRandomChar(chars), getRandomChar(chars));

            return playerName;
        }

        /// <summary>
        /// Generate random number (thread-safe)
        /// </summary>
        /// <returns>Random  number</returns>
        public static int GenerateRandomNumber()
        {
            var random = RandomProvider.GetThreadRandom();

            var randomNumber = random.Next(1, 9999999);

            return randomNumber;
        }

        /// <summary>
        /// Convert money to cents (x100)
        /// </summary>
        /// <param name="value">Money to convert</param>
        /// <returns>Converted rounded value</returns>
        public static int ConvertToCents(decimal value)
        {
            return (int)(value * 100);
        }

        /// <summary>
        /// Convert BigBlind to NL (x100)
        /// </summary>
        /// <param name="value">BigBlind to convert</param>
        /// <returns>Converted rounded value</returns>
        public static int ConvertBigBlindToNL(decimal value)
        {
            return (int)(value * 100);
        }

        /// <summary>
        /// Convert BigBlind in cents to NL
        /// </summary>
        /// <param name="value">BigBlind to convert</param>
        /// <returns>Converted rounded value</returns>
        public static int ConvertBigBlindInCentsToNL(decimal value)
        {
            return (int)(value);
        }

        /// <summary>
        /// Validate email
        /// </summary>
        /// <param name="strIn">Input string</param>
        /// <returns>True if email is valid, otherwise - false</returns>
        public static bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get error codes from licensing exception
        /// </summary>
        /// <param name="e">Exception</param>
        /// <returns>List of error codes</returns>
        public static List<string> GetErrorCodes(NoLicenseException e)
        {
            var errorCodes = new List<string>();

            foreach (ValidationRecord validationRecord in e.ValidationRecords)
            {
                foreach (ValidationRecord subRecord in validationRecord.SubRecords)
                {
                    errorCodes.Add(subRecord.ErrorCode);
                }

                errorCodes.Add(validationRecord.ErrorCode);
            }

            return errorCodes;
        }

        /// <summary>
        /// Gets primary screen resolution
        /// </summary>
        /// <returns>Screen resolution</returns>
        public static SizeF GetScreenResolution()
        {
            return new SizeF
            {
                Width = (float)SystemParameters.PrimaryScreenWidth,
                Height = (float)SystemParameters.PrimaryScreenHeight
            };
        }

        /// <summary>
        /// Get screen dpi
        /// </summary>
        /// <returns>Screen dpi</returns>
        public static SizeF GetCurrentDpi()
        {
            using (Form form = new Form())
            using (Graphics g = form.CreateGraphics())
            {
                var result = new SizeF()
                {
                    Width = g.DpiX,
                    Height = g.DpiY
                };
                return result;
            }
        }

        /// <summary>
        /// Calculates md5 hash of specified file
        /// </summary>
        /// <param name="fileName">Path to file</param>
        /// <returns>Hash of specified file</returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return string.Empty;
            }

            try
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(Utils), $"Could not calculate hash of {fileName}", e);
                throw;
            }
        }

        /// <summary>
        /// Calculates md5 hash of specified file asynchronously
        /// </summary>
        /// <param name="fileName">Path to file</param>
        /// <returns>Hash of specified file</returns>
        public static Task<string> GetMD5HashFromFileAsync(string fileName)
        {
            return Task.Run(() =>
            {
                return GetMD5HashFromFile(fileName);
            });
        }

        /// <summary>
        /// Determines whenever date is in date range
        /// </summary>
        /// <returns></returns>
        public static bool IsDateInDateRange(DateTime date, DateTime? startDate, DateTime? endDate, TimeSpan rangeExtension)
        {
            var start = startDate.HasValue ? startDate.Value : DateTime.MinValue;
            var end = endDate.HasValue ? endDate.Value : DateTime.MaxValue;

            var rangeExtensionInMinutes = Math.Abs(rangeExtension.TotalMinutes);

            if (rangeExtensionInMinutes > 0)
            {
                start = DateTime.MinValue.AddMinutes(rangeExtensionInMinutes) < start ?
                    start.AddMinutes(-rangeExtensionInMinutes) :
                    DateTime.MinValue;

                end = DateTime.MaxValue.AddMinutes(-rangeExtensionInMinutes) > end ?
                    end.AddMinutes(rangeExtensionInMinutes) :
                    DateTime.MaxValue;
            }

            return date >= start && date <= end;
        }

        /// <summary>
        /// Removes trailing zeros from the specified byte array
        /// </summary>
        /// <param name="data">Byte array to remove trailing zeros</param>
        /// <returns>Byte array without trailing zeros</returns>
        public static byte[] RemoveTrailingZeros(byte[] data)
        {
            Check.Require(data != null, "data argument must be not null");

            var i = data.Length - 1;

            while (data[i] == 0)
            {
                --i;
            }

            var cleanData = new byte[i + 1];

            Array.Copy(data, cleanData, i + 1);

            return cleanData;
        }

        /// <summary>
        /// Get processes for the specified processes names
        /// </summary>
        /// <returns>Array of found processes</returns>
        public static Process[] GetProcessesByNames(string[] processNames)
        {
            var processes = Process.GetProcesses();

            var pokerClientProcesses = processes.Where(x => processNames.Any(p => x.ProcessName.Equals(p, StringComparison.OrdinalIgnoreCase))).ToArray();

            return pokerClientProcesses;
        }
    }
}