﻿//-----------------------------------------------------------------------
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
using HandHistories.Objects.Players;
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

namespace DriveHUD.Importers.BetOnline
{
    internal class BetOnlineDataManager : BetOnlineBaseDataManager, IBetOnlineDataManager
    {
        private IPokerClientEncryptedLogger logger;
        private bool isLoggingEnabled;
        private IEventAggregator eventAggregator;

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

                Task.Run(() =>
                {
                    var xml = Decrypt(encryptedXml);

                    if (logger != null && isLoggingEnabled)
                    {
                        logger.Log(xml);
                    }

                    var betOnlineXmlConverter = ServiceLocator.Current.GetInstance<IBetOnlineXmlConverter>();
                    var convertedResult = betOnlineXmlConverter.Convert(xml);

                    if (convertedResult == null)
                    {
                        return;
                    }

#if DEBUG
                    var streamFolder = "stream";

                    if (!Directory.Exists(streamFolder))
                    {
                        Directory.CreateDirectory(streamFolder);
                    }

                    var logfile = convertedResult.TableName;

                    foreach (var invalidChar in Path.GetInvalidFileNameChars())
                    {
                        logfile = logfile.Replace(new string(invalidChar, 1), string.Empty);
                    }

                    File.AppendAllText(string.Format("{0}\\{1}-stream.xml", streamFolder, logfile), xml);
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

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}