﻿//-----------------------------------------------------------------------
// <copyright file="SecurityUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DriveHUD.Common.Security
{
    /// <summary>
    /// Security utilities
    /// </summary>
    public class SecurityUtils
    {
        /// <summary>
        /// Encrypt data with RSA public key
        /// </summary>
        /// <param name="data">Data to be encrypt</param>
        /// <returns>Encrypted data</returns>
        public static string EncryptStringRSA(string data, string publicKey)
        {
            var binaryData = Encoding.UTF8.GetBytes(data);

            using (var cipher = new RSACryptoServiceProvider(2048))
            {
                cipher.FromXmlString(publicKey);

                var encryptedData = cipher.Encrypt(binaryData, false);
                var encryptedString = Convert.ToBase64String(encryptedData);
                return encryptedString;
            }
        }

        public static bool ValidateFileHash(string file, string hash)
        {
            if (!File.Exists(file) || string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            var fileHash = string.Empty;

            try
            {
                using (var fs = new FileStream(file, FileMode.Open))
                {
                    var sha1CryptoProvider = new SHA1CryptoServiceProvider();

                    var hashBytes = sha1CryptoProvider.ComputeHash(fs);

                    var sb = new StringBuilder();

                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }

                    fileHash = sb.ToString();

                    System.Diagnostics.Debug.WriteLine(fileHash);
                }
            }
            catch
            {
                return false;
            }

            return fileHash.Equals(hash);
        }
    }
}