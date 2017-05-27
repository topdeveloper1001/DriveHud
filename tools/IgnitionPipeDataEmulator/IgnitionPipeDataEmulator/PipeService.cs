//-----------------------------------------------------------------------
// <copyright file="PipeService.cs" company="Ace Poker Solutions">
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
using System.Globalization;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IgnitionPipeDataEmulator
{
    internal class PipeService : IDisposable
    {
        private const int Timeout = 10000;

        private NamedPipeClientStream pipeClient;

        private TimeSpan? previousTime;

        public PipeService()
        {
            Logger.Log($"Connecting to '{Globals.PipeName}' pipe server");
            pipeClient = new NamedPipeClientStream(".", Globals.PipeName, PipeDirection.Out);
            try
            {
                pipeClient.Connect(Timeout);
            }
            catch (Exception e)
            {
                Logger.Log($"Connection to '{Globals.PipeName}' pipe server failed. Please stop/start DH.");
                throw e;
            }
            Logger.Log($"Connection to '{Globals.PipeName}' pipe server established");
        }

        public void Send(IgnitionPipeData pipeData)
        {
            if (!pipeClient.IsConnected || pipeData == null || string.IsNullOrEmpty(pipeData.Message))
            {
                return;
            }

            var messageTime = ParseTime(pipeData.TimeString);

            if (previousTime.HasValue)
            {
                var delay = messageTime - previousTime.Value;
                Thread.Sleep((int)delay.TotalMilliseconds);
            }

            previousTime = messageTime;

            var bytesToSend = Encoding.ASCII.GetBytes(pipeData.Message);

            pipeClient.Write(bytesToSend, 0, bytesToSend.Count());
            pipeClient.Flush();
        }

        public void Dispose()
        {
            pipeClient?.Dispose();
            Logger.Log($"Connection to '{Globals.PipeName}' pipe server closed");
        }

        private TimeSpan ParseTime(string timeString)
        {
            var splittedString = timeString.Split(':').Select(x => int.Parse(x)).ToArray();
            return new TimeSpan(splittedString[0], splittedString[1], splittedString[2]);
        }
    }
}