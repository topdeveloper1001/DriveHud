//-----------------------------------------------------------------------
// <copyright file="TournamentCacheService.cs" company="Ace Poker Solutions">
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using DriveHUD.Common.Log;

namespace DriveHUD.Importers.BetOnline
{
    public class TournamentsCacheService : ITournamentsCacheService
    {
        private const string cacheFolder = "data";
        private const string cacheFile = "bol.dat";
        
        private const string key = "jaQS0QDMob+vIK2cG+KRSjkwHbnAe1tmq7wvYJklE7Y=";
        private const string iv = "tA5Cs5xcDMOrH9TRNgEAWg==";

        private ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();
        private readonly string[] attributesToMerge = new string[] { "prizeFee", "fee", "seats", "buyIn" };

        private XDocument tournamentCache;

        public XElement GetTournamentInfo(string tournamentName)
        {
            if (string.IsNullOrEmpty(tournamentName))
            {
                return null;
            }

            if (tournamentCache == null)
            {
                LogProvider.Log.Warn("Tournament cache wasn't loaded");
                return null;
            }

            try
            {
                lockObject.EnterReadLock();

                var tournamentInfo = tournamentCache.Descendants("Table")
                                        .FirstOrDefault(x => x.Attribute("name") != null && x.Attribute("name").Value.Equals(tournamentName, StringComparison.InvariantCultureIgnoreCase));
                return tournamentInfo;
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        public void Initialize()
        {
            tournamentCache = new XDocument(new XElement("Tournaments"));

            var cacheFilePath = Path.Combine(cacheFolder, cacheFile);
          
            if (!File.Exists(cacheFilePath))
            {
                return;
            }

            var encryptedText = File.ReadAllText(cacheFilePath);

            try
            {
                var xml = Decrypt(encryptedText);

                tournamentCache = XDocument.Parse(xml);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not initialize tournament cache", e);
            }
        }

        public void Flush()
        {
            lockObject.EnterReadLock();

            try
            {
                if (!Directory.Exists(cacheFolder))
                {
                    Directory.CreateDirectory(cacheFolder);
                }

                var cacheFilePath = Path.Combine(cacheFolder, cacheFile);

                var xml = tournamentCache.ToString();

                var encryptedData = Encrypt(xml);

                File.WriteAllText(cacheFilePath, encryptedData);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not flush cache data", e);
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        public void Update(string tournamentXml)
        {
            if (string.IsNullOrWhiteSpace(tournamentXml))
            {
                LogProvider.Log.Warn("Tournament cache wasn't initialized");
                return;
            }

            try
            {
                var xDocument = XDocument.Parse(tournamentXml);
                Merge(xDocument);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not parse tournament data", e);
            }
        }

        #region Infrastructure

        private void Merge(XDocument xDocumentToMerge)
        {
            if (tournamentCache == null)
            {
                return;
            }

            lockObject.EnterWriteLock();

            try
            {
                var tablesToUpdate = (from tableToMerge in xDocumentToMerge.Descendants("Table").Where(x => x.Attribute("name") != null)
                                      join table in tournamentCache.Descendants("Table").Where(x => x.Attribute("name") != null) on tableToMerge.Attribute("name").Value equals table.Attribute("name").Value into gj
                                      from grouped in gj.DefaultIfEmpty()
                                      select new { OldTable = grouped, NewTable = tableToMerge }).ToArray();

                foreach (var tableToUpdate in tablesToUpdate)
                {
                    // add item
                    if (tableToUpdate.OldTable == null)
                    {
                        tournamentCache.Root.Add(tableToUpdate.NewTable);
                    }
                    else
                    {
                        foreach (var attributeToMerge in attributesToMerge)
                        {
                            if (tableToUpdate.OldTable.Attribute(attributeToMerge) != null && tableToUpdate.NewTable.Attribute(attributeToMerge) != null)
                            {
                                tableToUpdate.OldTable.Attribute(attributeToMerge).Value = tableToUpdate.NewTable.Attribute(attributeToMerge).Value;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not merge tournament data", e);
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

        private string Encrypt(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);

            using (var aesCryptoProvider = new AesManaged())
            {
                aesCryptoProvider.Key = Convert.FromBase64String(key);
                aesCryptoProvider.IV = Convert.FromBase64String(iv);

                var encryptor = aesCryptoProvider.CreateEncryptor();

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new BinaryWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }

                        var encryptedData = msEncrypt.ToArray();

                        var encryptedText = Convert.ToBase64String(encryptedData);

                        return encryptedText;
                    }
                }
            }
        }

        private string Decrypt(string encryptedText)
        {
            var encryptedData = Convert.FromBase64String(encryptedText);

            using (var aesCryptoProvider = new AesManaged())
            {
                aesCryptoProvider.Key = Convert.FromBase64String(key);
                aesCryptoProvider.IV = Convert.FromBase64String(iv);

                var decryptor = aesCryptoProvider.CreateDecryptor();

                using (var msDecrypt = new MemoryStream(encryptedData))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var swDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                        {
                            return swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        #endregion
    }
}