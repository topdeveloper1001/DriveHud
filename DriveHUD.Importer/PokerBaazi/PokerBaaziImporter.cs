//-----------------------------------------------------------------------
// <copyright file="PokerBaaziImporter.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.Importers.PokerBaazi.Model;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziImporter : GenericImporter, IPokerBaaziImporter
    {
        private const int NoDataDelay = 200;

        private readonly BlockingCollection<string> packetBuffer = new BlockingCollection<string>();

        protected readonly ISettingsService settings;

        public PokerBaaziImporter()
        {
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        #region IPokerBaaziImporter implementation

        public void AddPackage(string data)
        {
            packetBuffer.Add(data);
        }

        #endregion

        protected override string[] ProcessNames => new string[0];

        protected override EnumPokerSites Site => EnumPokerSites.PokerBaazi;

        protected override bool IsAdvancedLogEnabled
        {
            get
            {
                return settings.GetSettings()?.GeneralSettings.IsAdvancedLoggingEnabled ?? false;
            }
        }

        protected override void DoImport()
        {
            try
            {
                ProcessBuffer();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to process imported data. [{SiteString}]", e);
            }

            packetBuffer.Clear();

            RaiseProcessStopped();
        }

        protected override IntPtr FindWindow(ParsingResult parsingResult)
        {
            return IntPtr.Zero;
        }

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            return false;
        }

        /// <summary>
        /// Processes buffer data
        /// </summary>
        protected virtual void ProcessBuffer()
        {
            var handBuilder = ServiceLocator.Current.GetInstance<IPokerBaaziHandBuilder>();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (!packetBuffer.TryTake(out string capturedData))
                    {
                        Task.Delay(NoDataDelay).Wait();
                        continue;
                    }

                    if (!PokerBaaziPackage.TryParse(capturedData, out PokerBaaziPackage package))
                    {
                        continue;
                    }

                    if (!handBuilder.TryBuild(package, out HandHistory handHistory))
                    {
                        if (package.PackageType == PokerBaaziPackageType.InitResponse)
                        {
                            var initResponse = handBuilder.GetInitResponse(package.RoomId);

                            if (initResponse != null)
                            {
                                //SendPreImporedData()
                            }
                        }

                        continue;
                    }

                    var handHistoryText = SerializationHelper.SerializeObject(handHistory);

#if DEBUG
                    if (!Directory.Exists("Hands"))
                    {
                        Directory.CreateDirectory("Hands");
                    }

                    File.WriteAllText($"Hands\\pokerbaazi_hand_exported_{handHistory.HandId}.xml", handHistoryText);
#endif

                    var windowHandle = IntPtr.Zero;

                    var gameInfo = new GameInfo
                    {
                        WindowHandle = windowHandle.ToInt32(),
                        PokerSite = EnumPokerSites.PokerBaazi,
                        GameNumber = handHistory.HandId,
                        Session = windowHandle.ToString()
                    };

                    ProcessHand(handHistoryText, gameInfo);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not process captured data. [{SiteString}]", e);
                }
            }
        }
    }
}