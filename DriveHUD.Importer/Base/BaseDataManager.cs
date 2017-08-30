//-----------------------------------------------------------------------
// <copyright file="BaseDataManager.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DriveHUD.Importers
{
    internal class BaseDataManager
    {
        protected string Decrypt(string encryptedXml)
        {
            try
            {
                var key = "82yz1tqyodnl7wlk";
                var iv = "8gw9gz6cknqgvqsw";

                var encryptedData = Convert.FromBase64String(encryptedXml);

                var decryptedXml = string.Empty;

                using (var aesCryptoProvider = new AesManaged())
                {
                    aesCryptoProvider.Key = Encoding.UTF8.GetBytes(key);
                    aesCryptoProvider.IV = Encoding.UTF8.GetBytes(iv);

                    var decryptor = aesCryptoProvider.CreateDecryptor();

                    using (var msDecrypt = new MemoryStream(encryptedData))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var swDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                            {
                                decryptedXml = swDecrypt.ReadToEnd().Replace("\0", string.Empty);
                            }
                        }
                    }
                }

                if (decryptedXml[decryptedXml.Length - 2] == '>')
                {
                    decryptedXml = decryptedXml.Remove(decryptedXml.Length - 1);
                }

                return decryptedXml;
            }
            catch (Exception e)
            {
#if DEBUG
                LogProvider.Log.Error($"Could not recognize data: {encryptedXml}");
#else
                LogProvider.Log.Error("Could not recognize data");
#endif            
                throw e;
            }
        }
    }
}