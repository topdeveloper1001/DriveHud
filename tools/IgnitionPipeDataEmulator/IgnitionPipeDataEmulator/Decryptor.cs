//-----------------------------------------------------------------------
// <copyright file="Globals.cs" company="Ace Poker Solutions">
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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IgnitionPipeDataEmulator
{
    internal class Decryptor : IDisposable
    {
        private RSACryptoServiceProvider cipher;
        private AesManaged aesCryptoProvider;
        private ICryptoTransform decryptor;

        public Decryptor()
        {
            cipher = new RSACryptoServiceProvider(2048);
            cipher.FromXmlString(Globals.RSAPrivateKey);

            aesCryptoProvider = new AesManaged();
        }

        public IgnitionPipeData Decrypt(string encryptedDataStr)
        {
            if (string.IsNullOrEmpty(encryptedDataStr))
            {
                return null;
            }

            // convert string to bytes
            var encryptedBytes = Convert.FromBase64String(encryptedDataStr.Replace('-', '+').Replace('_', '/'));

            var flag = (DataFlag)encryptedBytes[0];

            // get AES keys
            if (flag == DataFlag.Key)
            {
                var combinedAesKeyAndIVEncrypted = encryptedBytes.Skip(1).ToArray();

                InitializeAesDecryptor(combinedAesKeyAndIVEncrypted);

                return null;
            }

            var dataBytes = encryptedBytes.Skip(1).ToArray();

            var dateBytes = dataBytes.Take(8).ToArray();

            var date = Encoding.UTF8.GetString(dateBytes);

            string dataMessage;

            using (var msDecrypt = new MemoryStream(dataBytes.Skip(8).ToArray()))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var swDecrypt = new StreamReader(csDecrypt))
                    {
                        dataMessage = swDecrypt.ReadToEnd();
                    }
                }
            }

            var ignitionPipeData = new IgnitionPipeData
            {
                TimeString = date,
                Message = dataMessage
            };

            return ignitionPipeData;
        }

        public void Dispose()
        {
            aesCryptoProvider?.Dispose();
            cipher?.Dispose();
        }

        private void InitializeAesDecryptor(byte[] combinedAesKeyAndIVEncrypted)
        {
            var combinedAesKeyAndIV = cipher.Decrypt(combinedAesKeyAndIVEncrypted, false);

            var aesKey = combinedAesKeyAndIV.Take(32).ToArray();
            var aesIV = combinedAesKeyAndIV.Skip(32).ToArray();

            aesCryptoProvider.Key = aesKey;
            aesCryptoProvider.IV = aesIV;

            decryptor = aesCryptoProvider.CreateDecryptor();

            Logger.Log("AES Key found, decrypted and installed");
        }

        enum DataFlag : byte
        {
            Data = 0,
            Key = 1
        }
    }
}