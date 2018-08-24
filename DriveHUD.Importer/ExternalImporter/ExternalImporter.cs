//-----------------------------------------------------------------------
// <copyright file="GGNImporter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.FastParsing;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DriveHUD.Importers.ExternalImporter
{
    internal class ExternalImporter : GenericImporter, IExternalImporter
    {
        private static readonly object locker = new object();

        private readonly static string[] processNames = new[] { "GGNet" };        

        private ServiceHost serviceHost;

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.Unknown;
            }
        }

        protected override string[] ProcessNames
        {
            get
            {
                return processNames;
            }
        }

        public override string SiteString
        {
            get
            {
                return "GenericExternalImporter";
            }
        }

        protected override void DoImport()
        {
            lock (locker)
            {
                try
                {
                    var baseAddress = new Uri(StringFormatter.GetImporterPipeAddress());
                    var baseAddresses = new Uri[] { baseAddress };

                    var service = new DHImporterService(this);

                    if (serviceHost != null)
                    {
                        serviceHost.Faulted -= OnServiceHostFaulted;
                    }

                    serviceHost = new ServiceHost(service, baseAddresses);
                    serviceHost.AddServiceEndpoint(typeof(IDHImporterService), new NetNamedPipeBinding(), nameof(IDHImporterService.ImportHandHistory));
                    serviceHost.Faulted += OnServiceHostFaulted;

                    serviceHost.Open();
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"{SiteString} auto-import failed.", e);

                    Clean();
                    RaiseProcessStopped();
                }
            }
        }

        private void OnServiceHostFaulted(object sender, EventArgs e)
        {
            LogProvider.Log.Error(this, $"Service host faulted [{SiteString}]");
        }

        public override void Stop()
        {
            base.Stop();

            Task.Run(() =>
            {
                lock (locker)
                {
                    Clean();
                    RaiseProcessStopped();
                }
            });
        }

        /// <summary>
        /// Updates <see cref="GameInfo"/> when hand has been parsed already
        /// </summary>
        /// <param name="parsingResults">Parsing results</param>
        /// <param name="gameInfo">Game information</param>
        protected virtual void UpdateGameInfo(IEnumerable<ParsingResult> parsingResults, GameInfo gameInfo)
        {
            var parsingResult = parsingResults?.FirstOrDefault();

            if (parsingResult == null || parsingResult.Source == null || gameInfo == null)
            {
                return;
            }

            var window = gameInfo.WindowHandle != 0 ? new IntPtr(gameInfo.WindowHandle) : FindWindow(parsingResult);

            if (window == IntPtr.Zero)
            {
                return;
            }

            var title = WinApi.GetWindowText(window);

            gameInfo.GameNumber = parsingResult.Source.HandId;
            gameInfo.Session = window.ToInt32().ToString();
            gameInfo.WindowHandle = window.ToInt32();
            gameInfo.TournamentSpeed = ParserUtils.ParseTournamentSpeed(title);
        }

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
              parsingResult.Source == null || parsingResult.Source.GameDescription == null || string.IsNullOrEmpty(parsingResult.Source.TableName))
            {
                return false;
            }

            // temporary solution (need to rework with next external importer)
            if (parsingResult.Source.GameDescription.Site != EnumPokerSites.GGN)
            {
                return false;
            }

            if (parsingResult.Source.GameDescription.IsTournament)
            {
                return ExternalImporterUtils.IsTournamentTableMatch(title, parsingResult.Source.TableName);
            }

            return ExternalImporterUtils.IsCashTableMatch(title, parsingResult.Source.TableName);
        }

        /// <summary>
        /// Cleans up resources
        /// </summary>
        protected void Clean()
        {
            if (serviceHost == null)
            {
                return;
            }

            try
            {
                serviceHost.Faulted -= OnServiceHostFaulted;
                serviceHost.Close();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could close host service [{SiteString}]", e);
            }
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            var autoCenterSeats = GetAutoCenterSeats(handHistory.GameDescription.Site);

            if (heroSeat != 0 && autoCenterSeats.ContainsKey(maxPlayers))
            {
                var prefferedSeat = autoCenterSeats[maxPlayers];

                var shift = (prefferedSeat - heroSeat) % maxPlayers;

                foreach (var player in playerList)
                {
                    player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
                }
            }

            return playerList;
        }

        protected virtual Dictionary<int, int> GetAutoCenterSeats(EnumPokerSites pokerSite)
        {
            switch (pokerSite)
            {
                case EnumPokerSites.GGN:
                    return new Dictionary<int, int>
                    {
                        { 4, 3 },
                        { 6, 4 },
                        { 9, 6 }
                    };
                case EnumPokerSites.PokerMaster:
                    return new Dictionary<int, int>
                    {
                        { 2, 2 },
                        { 3, 2 },
                        { 4, 3 },
                        { 5, 3 },
                        { 6, 4 },
                        { 7, 4 },
                        { 8, 5 },
                        { 9, 5 }
                    };
                default:
                    return new Dictionary<int, int>();
            }
        }

        protected override bool IsDisabled()
        {
            return false;
        }

        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
        private class DHImporterService : IDHImporterService
        {
            private ExternalImporter importer;

            public DHImporterService(ExternalImporter importer)
            {
                this.importer = importer ?? throw new ArgumentNullException(nameof(importer));
            }

            public void ImportHandHistory(HandHistoryDto handHistory)
            {
                if (handHistory == null || string.IsNullOrEmpty(handHistory.HandText))
                {
                    return;
                }

                try
                {
                    var gameInfo = new GameInfo
                    {
                        PokerSite = handHistory.PokerSite,
                        UpdateAction = importer.UpdateGameInfo,
                        WindowHandle = handHistory.WindowHandle
                    };

                    importer.ProcessHand(handHistory.HandText, gameInfo);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Hand has not been processed [{importer.SiteString}]", e);
                }
            }
        }
    }
}