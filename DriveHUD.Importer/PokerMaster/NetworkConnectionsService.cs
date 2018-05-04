﻿//-----------------------------------------------------------------------
// <copyright file="NetworkConnectionsService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.Utils.Network;
using DriveHUD.Entities;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace DriveHUD.Importers.PokerMaster
{
    internal class NetworkConnectionsService : INetworkConnectionsService
    {
        private List<ConnectionProcessInfo> connectionProcesses = new List<ConnectionProcessInfo>();

        private List<LocalConnection> localConnections = new List<LocalConnection>();

        private EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PokerMaster;
            }
        }

        public Process GetProcess(CapturedPacket capturedPacket)
        {
            var connectionProcess = connectionProcesses.FirstOrDefault(x => x.Source.Equals(capturedPacket.Source) && x.Destination.Equals(capturedPacket.Destination));

            if (connectionProcess != null && !connectionProcess.Validate())
            {
                connectionProcesses.Remove(connectionProcess);
                connectionProcess = null;
            }

            if (connectionProcess == null)
            {
                bool filterConnection(LocalConnection connection)
                {
                    return connection.LocalAddress.Equals(capturedPacket.Source) && connection.Destination.Equals(capturedPacket.Destination) ||
                        connection.LocalAddress.Equals(capturedPacket.Destination) && connection.Destination.Equals(capturedPacket.Source);
                }

                var localConnection = localConnections.FirstOrDefault(x => filterConnection(x));

                if (localConnection == null || localConnection.ProcessId == 0)
                {
                    localConnections = NetworkUtils.GetAllTcpConnections().ToList();
                    localConnection = localConnections.FirstOrDefault(x => filterConnection(x));
                }

                if (localConnection == null)
                {
                    LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Could not find associated process for packet {capturedPacket}");
                    return null;
                }

                if (localConnection.ProcessId == 0)
                {
                    LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Associated process isn't defined for packet {capturedPacket}");
                    return null;
                }

                connectionProcess = new ConnectionProcessInfo
                {
                    Source = capturedPacket.Source,
                    Destination = capturedPacket.Destination,
                    Process = Process.GetProcessById((int)localConnection.ProcessId)
                };

                connectionProcesses.Add(connectionProcess);
            }

            return connectionProcess.Process;
        }

        private class ConnectionProcessInfo
        {
            public IPEndPoint Source { get; set; }

            public IPEndPoint Destination { get; set; }

            public Process Process { get; set; }

            public bool Validate()
            {
                try
                {
                    return Process != null && !Process.HasExited;
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Process [{Process.Id}] could not be validated.", e);
                }

                return false;
            }
        }
    }
}