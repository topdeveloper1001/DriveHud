//-----------------------------------------------------------------------
// <copyright file="PKLoginResponse.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.PokerKing.Model
{
    internal class PKLoginResponse
    {
        public uint UserId { get; set; }

        public string UserToken { get; set; }

        public static bool TryParse(string xml, out PKLoginResponse loginResponse)
        {
            loginResponse = null;

            if (string.IsNullOrEmpty(xml))
            {
                return false;
            }

            try
            {
                var userToken = ReadXmlValue(xml, "user_token");

                if (string.IsNullOrEmpty(userToken))
                {
                    return false;
                }

                var userIdText = ReadXmlValue(xml, "user_id");

                if (string.IsNullOrEmpty(userIdText) ||
                    !uint.TryParse(userIdText, out uint userId))
                {
                    return false;
                }

                var decodedToken = DecodeToken(userToken);

                loginResponse = new PKLoginResponse
                {
                    UserId = userId,
                    UserToken = decodedToken
                };

                return true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(PKLoginResponse), "Failed to parse login response.", e);
                return false;
            }
        }

        private static string ReadXmlValue(string xml, string parameter)
        {
            var searchPattern = $"{parameter}\">";

            var startIndex = xml.IndexOf(searchPattern);

            if (startIndex < 0)
            {
                return null;
            }

            var endIndex = xml.IndexOf("<", startIndex);

            if (endIndex < 0)
            {
                return null;
            }

            var text = xml.Substring(startIndex + searchPattern.Length, endIndex - startIndex - searchPattern.Length);

            return text;
        }

        // @lnFi8<eIKYazt:$_;MX9T/d(gk[JW3{Upcw
        private static readonly byte[] decodeKey = new byte[] { 0x40, 0x6C, 0x6E, 0x46, 0x69, 0x38, 0x3C, 0x65, 0x49, 0x4B,
            0x59, 0x61, 0x7A, 0x74, 0x3A, 0x24, 0x5F, 0x3B, 0x4D, 0x58, 0x39, 0x54, 0x2F, 0x64, 0x28, 0x67, 0x6B, 0x5B, 0x4A, 0x57, 0x33, 0x7B, 0x55, 0x70, 0x63, 0x77 };

        private static string DecodeToken(string token)
        {
            var tokenBytes = Convert.FromBase64String(token);
            var decryptedData = PKCipherHelper.Decode(decodeKey.Take(32).ToArray(), tokenBytes);
            var decryptedToken = Encoding.ASCII.GetString(decryptedData);

            return decryptedToken;
        }
    }
}