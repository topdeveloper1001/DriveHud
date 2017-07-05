//-----------------------------------------------------------------------
// <copyright file="BetOnlineDataManager.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DriveHUD.Importers.BetOnline
{
    internal class BetOnlineDataManager : BetOnlineBaseDataManager, IBetOnlineDataManager
    {
        private IPokerClientEncryptedLogger logger;
        private bool isLoggingEnabled;
        private IEventAggregator eventAggregator;
        private const int delay = 2000;

        private readonly List<RelocationData> relocationData = new List<RelocationData>();
        private static object locker = new object();

        private List<string> buffer = new List<string>();

        public BetOnlineDataManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.BetOnline;
            }
        }

        public void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            Check.ArgumentNotNull(() => dataManagerInfo);

            logger = dataManagerInfo.Logger;

            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var settings = settingsService.GetSettings();

            isLoggingEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;
        }

        /// <summary>
        /// Process stream data 
        /// </summary>
        /// <param name="data">Stream data</param>
        /// <returns>Result of processing</returns>
        public void ProcessData(byte[] data)
        {
            try
            {
                var encryptedXml = Encoding.UTF8.GetString(data).Replace("\0", string.Empty);

                var partialXml = Decrypt(encryptedXml);

                if (logger != null && isLoggingEnabled)
                {
                    logger.Log(partialXml);
                }

                if (TryParseRelocationData(partialXml))
                {
                    return;
                }

                string xml;

                if (!AddToBuffer(partialXml, out xml))
                {
                    return;
                }

                Task.Run(() =>
                {
                    // wait for possible relocation data
                    Task.Delay(delay).Wait();

                    var betOnlineXmlConverter = ServiceLocator.Current.GetInstance<IBetOnlineXmlConverter>();
                    betOnlineXmlConverter.Initialize(xml);

                    lock (locker)
                    {
                        var relocation = relocationData.FirstOrDefault(x => x.Hand == betOnlineXmlConverter.HandNumber);

                        if (relocation != null && relocation.RelocationElement != null)
                        {
                            betOnlineXmlConverter.AddRelocationData(relocation.RelocationElement);
                            relocationData.Remove(relocation);
                        }
                    }

                    var convertedResult = betOnlineXmlConverter.Convert();

                    if (convertedResult == null)
                    {
                        return;
                    }

#if DEBUG
                    //var streamFolder = "stream";

                    //if (!Directory.Exists(streamFolder))
                    //{
                    //    Directory.CreateDirectory(streamFolder);
                    //}

                    //var logfile = convertedResult.TableName;

                    //foreach (var invalidChar in Path.GetInvalidFileNameChars())
                    //{
                    //    logfile = logfile.Replace(new string(invalidChar, 1), string.Empty);
                    //}

                    //File.AppendAllText(string.Format("{0}\\{1}-stream.xml", streamFolder, logfile), xml);
#endif                    

                    ImportResult(convertedResult);
                });
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Stream data has wrong format. [{Identifier}]", ex);
            }
        }

        /// <summary>
        /// Import results in DB
        /// </summary>
        /// <param name="convertedResult"></param>
        private void ImportResult(ConvertedResult convertedResult)
        {
            // send data to file importer
            var dbImporter = ServiceLocator.Current.GetInstance<IFileImporter>();
            var progress = new DHProgress();

            IEnumerable<ParsingResult> parsingResult = null;

            try
            {
                parsingResult = dbImporter.Import(convertedResult.ConvertedXml, progress, convertedResult.GameInfo);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, string.Format("Hand {0} has not been imported. [{1}]", convertedResult.HandNumber, Identifier), e);
            }

            if (parsingResult == null)
            {
                return;
            }

            foreach (var result in parsingResult)
            {
                if (result.HandHistory == null)
                {
                    continue;
                }

                if (result.IsDuplicate)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. Duplicate. [{1}]", result.HandHistory.Gamenumber, Identifier));
                    continue;
                }

                if (!result.WasImported)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. [{1}]", result.HandHistory.Gamenumber, Identifier));
                    continue;
                }

                LogProvider.Log.Info(this, string.Format("Hand {0} has been imported. [{1}]", result.HandHistory.Gamenumber, Identifier));

                var dataImportedArgs = new DataImportedEventArgs(result.Source.Players, convertedResult.GameInfo);

                eventAggregator.GetEvent<DataImportedEvent>().Publish(dataImportedArgs);
            }
        }

        private bool TryParseRelocationData(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml) || !xml.StartsWith("<RelocationData", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var xDocument = XDocument.Parse(xml);
                var relocationElement = xDocument.Root;

                var hand = relocationElement.Attribute("hand").Value;

                var relocation = new RelocationData
                {
                    Hand = hand,
                    RelocationElement = relocationElement
                };

                lock (locker)
                {
                    relocationData.Add(relocation);
                }

                return true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Relocation data has not been parsed.", e);
            }

            return false;
        }

        private bool AddToBuffer(string xml, out string combinedXml)
        {
            combinedXml = string.Empty;

            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            if (xml.StartsWith("<Error", StringComparison.OrdinalIgnoreCase))
            {
                if (isLoggingEnabled)
                {

                    string errorText = null;

                    var errorTextStartIndex = xml.IndexOf("text=\"", StringComparison.OrdinalIgnoreCase) + 6;

                    if (errorTextStartIndex > 0)
                    {
                        var errorTextEndIndex = xml.IndexOf("\"", errorTextStartIndex, StringComparison.OrdinalIgnoreCase);

                        if (errorTextEndIndex > 0)
                        {
                            errorText = xml.Substring(errorTextStartIndex, errorTextEndIndex - errorTextStartIndex);
                        }
                    }

                    errorText = errorText ?? xml;

                    LogProvider.Log.Warn($"Error occurred in stream: {errorText}");
                }

                return false;
            }

            if (xml.StartsWith("<TableDetails", StringComparison.OrdinalIgnoreCase) && buffer.Count > 0)
            {
                buffer.Clear();
                buffer.Add(xml);

                if (isLoggingEnabled)
                {
                    var isTournament = xml.IndexOf("TOURNAMENT_TABLE", StringComparison.OrdinalIgnoreCase) > 0;

                    var tagName = isTournament ? "tournamentName" : "name";

                    var indexStart = xml.IndexOf(tagName, StringComparison.OrdinalIgnoreCase) + tagName.Length;

                    var table = string.Empty;

                    if (indexStart > 0)
                    {
                        var indexEnd = xml.IndexOf("\"", indexStart, StringComparison.OrdinalIgnoreCase);

                        if (indexEnd > 0)
                        {
                            table = xml.Substring(indexStart, indexEnd - indexStart);
                        }
                    }

                    LogProvider.Log.Warn($"Unexpected table details [{table}]");
                }

                return false;
            }

            buffer.Add(xml);

            if (buffer.Count == 3)
            {
                var sb = new StringBuilder();
                sb.AppendLine("<HandHistory>");
                buffer.ForEach(b =>
                {
                    if (!b.StartsWith("<TableDetails", StringComparison.OrdinalIgnoreCase) && !b.StartsWith("<GameState"))
                    {
                        sb.AppendLine("<Changes>");
                        sb.AppendLine(b);
                        sb.AppendLine("</Changes>");
                    }
                    else
                    {
                        sb.AppendLine(b);
                    }
                });
                sb.AppendLine("</HandHistory>");

                combinedXml = sb.ToString();

                buffer.Clear();

                return true;
            }

            return false;
        }

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion

        private class RelocationData
        {
            public string Hand { get; set; }

            public XElement RelocationElement { get; set; }
        }
    }
}