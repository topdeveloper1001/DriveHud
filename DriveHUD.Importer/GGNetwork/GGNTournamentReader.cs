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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace DriveHUD.Importers.GGNetwork
{
    internal class GGNTournamentReader : IDisposable
    {
        private const string Url = "session1.rhymetree.net";

        private readonly WebSocket webSocket = new WebSocket($"wss://{Url}");

        public GGNTournamentReader()
        {
            webSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            webSocket.EmitOnPing = true;
            webSocket.OnMessage += WebSocketOnMessage;
        }

        public void Dispose()
        {
            webSocket.OnMessage -= WebSocketOnMessage;
            webSocket.Close();
        }

        private void WebSocketOnMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}