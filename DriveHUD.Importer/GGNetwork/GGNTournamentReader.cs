//-----------------------------------------------------------------------
// <copyright file="GGNTournamentReader.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using DriveHUD.Importers.GGNetwork.Model;
using DriveHUD.Importers.GGNetwork.Network;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using WebSocketSharp;

namespace DriveHUD.Importers.GGNetwork
{
    internal class GGNTournamentReader : IGGNTournamentReader
    {
        private const string Url = "wss://session1.rhymetree.net";

        private WebSocket webSocket;

        private int relayId = 1;
        private int packetIdSeq = 1;

        private const int MagicKey = -1162814976;
        private const int SerialMode = 1;
        private const int JunkData = 0;
        private const int SendHeaderSize = 36;
        private const int RecvHeaderSize = 36;
        private const int Timeout = 15000;
        private const int DelayInterval = 100;
        private const int MaxConnectionAttempts = 5;

        private const string EmptyRequest = "{}";

        private string userId;

        private bool isInfoRequested;

        private string tournamentsInfoMessage;

        private bool isAdvancedLogging;

        public GGNTournamentReader()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings;
            isAdvancedLogging = settings.IsAdvancedLoggingEnabled;
        }

        private bool IsDataReceived
        {
            get
            {
                return !string.IsNullOrEmpty(tournamentsInfoMessage);
            }
        }

        public async Task<IEnumerable<TournamentInformation>> ReadAllTournamentsAsync()
        {
            try
            {
                webSocket = new WebSocket(Url);

                webSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
                webSocket.EmitOnPing = true;
                webSocket.OnMessage += WebSocketOnMessage;
                webSocket.OnError += WebSocket_OnError;

                var connectionAttempts = 0;

                while (webSocket.ReadyState != WebSocketState.Open &&
                    connectionAttempts < MaxConnectionAttempts)
                {
                    webSocket.Connect();
                    connectionAttempts++;
                }

                if (isAdvancedLogging)
                {
                    LogProvider.Log.Info(this, $"Connected to tournaments info server on {connectionAttempts} attempt. [{EnumPokerSites.GGN}].");
                }

                var initialRequest = NetworkRequests.CreateInitialRequest();

                Send(initialRequest, ProtocolId.InitialRequest);

                await Task.Run(() =>
                {
                    var interval = 0;
                    var maxIntervals = Timeout / DelayInterval;

                    while (!IsDataReceived && interval < maxIntervals)
                    {
                        Task.Delay(DelayInterval).Wait();
                        interval++;
                    }
                });

                if (!IsDataReceived)
                {
                    LogProvider.Log.Warn(this, $"Tournaments info has not been received. [{EnumPokerSites.GGN}]");
                    return new TournamentInformation[0];
                }

                var tournamentInfoResponse = JsonConvert.DeserializeObject<TournamentInfoResponse>(tournamentsInfoMessage);

                if (isAdvancedLogging)
                {
                    LogProvider.Log.Info(this, $"Tournaments info has been read [{tournamentInfoResponse.Tournaments.Length} tournaments] [{EnumPokerSites.GGN}].");
                }

                return tournamentInfoResponse.Tournaments;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Tournaments info has not been read. [{EnumPokerSites.GGN}]", e);
                return new TournamentInformation[0];
            }
            finally
            {
                try
                {
                    webSocket.OnMessage -= WebSocketOnMessage;
                    webSocket.Close(CloseStatusCode.Away);
                }
                catch (Exception e)
                {
                    if (isAdvancedLogging)
                    {
                        LogProvider.Log.Error(this, $"Websocket was closed with error [{EnumPokerSites.GGN}].", e);
                    }
                }
            }
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
        }

        private void WebSocketOnMessage(object sender, MessageEventArgs e)
        {
            relayId++;

            if (e.RawData == null)
            {
                return;
            }

            try
            {
                var message = PacketUtils.ExtractData(e.RawData, RecvHeaderSize);

                var dataType = GetDataType(message);

                switch (dataType)
                {
                    case ResponseDataType.AccountInfo:
                        {
                            userId = GetUserId(message);

                            var jsonData = NetworkRequests.CreateUserIdMessage(userId);

                            Send(jsonData, ProtocolId.UserIdRequset);

                            break;
                        }
                    case ResponseDataType.TourneyGuid:
                        {
                            if (!isInfoRequested)
                            {
                                var jsonData = NetworkRequests.CreateTourneyMessage(userId);

                                Send(jsonData, ProtocolId.TourneyRequest);

                                isInfoRequested = true;
                            }

                            break;
                        }
                    case ResponseDataType.ServiceId:
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                switch (i)
                                {
                                    case 1:
                                        {
                                            Send(EmptyRequest, ProtocolId.Msg1);
                                            break;
                                        }
                                    case 2:
                                        {
                                            Send(EmptyRequest, ProtocolId.Msg2);
                                            break;
                                        }
                                    case 3:
                                        {
                                            Send(EmptyRequest, ProtocolId.Msg3);
                                            break;
                                        }
                                    case 4:
                                        {
                                            Send(EmptyRequest, ProtocolId.Msg4);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case ResponseDataType.TournamentsInfo:
                        {
                            tournamentsInfoMessage = message;
                            break;
                        }

                    case ResponseDataType.Unknown:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                if (isAdvancedLogging)
                {
                    LogProvider.Log.Error(this, $"Socket response has not been processed. [{EnumPokerSites.GGN}]", ex);
                }
            }
        }

        private void Send(string message, ProtocolId protocolId)
        {
            if (!webSocket.IsAlive)
            {
                return;
            }

            var packet = PacketUtils.BuildPacket(message, protocolId, MagicKey, SerialMode, ref packetIdSeq, relayId, JunkData, SendHeaderSize);

            webSocket.Send(packet);
        }

        private static ResponseDataType GetDataType(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return ResponseDataType.Unknown;
            }

            if (message.StartsWith("{\"AccountInfo\":", StringComparison.OrdinalIgnoreCase))
            {
                return ResponseDataType.AccountInfo;
            }

            if (message.StartsWith("{\"ServiceId\":", StringComparison.OrdinalIgnoreCase))
            {
                return ResponseDataType.ServiceId;
            }

            if (message.StartsWith("{\"TourneyGuid\":", StringComparison.OrdinalIgnoreCase))
            {
                return ResponseDataType.TourneyGuid;
            }

            if (message.StartsWith("{\"List\":[{\"HasPassword\":", StringComparison.OrdinalIgnoreCase))
            {
                return ResponseDataType.TournamentsInfo;
            }

            return ResponseDataType.Unknown;
        }

        private static string GetUserId(string data)
        {
            var accountData = JsonConvert.DeserializeObject<AccountData>(data);
            return accountData.AccountInfo.Account.UserId;
        }
    }
}