﻿//-----------------------------------------------------------------------
// <copyright file="LocalConnection.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;

namespace DriveHUD.Common.Utils.Network
{
    public class LocalConnection
    {
        public IPEndPoint LocalAddress
        {
            get; set;
        }

        public IPEndPoint Destination
        {
            get; set;
        }

        public uint ProcessId
        {
            get; set;
        }

        public ProtocolType Protocol
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"{Protocol} {LocalAddress} -> {Destination} PID: {ProcessId}";
        }
    }
}