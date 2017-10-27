//-----------------------------------------------------------------------
// <copyright file="PerformanceMonitor.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.Diagnostics;

namespace DriveHud.Common.Log
{
    public class PerformanceMonitor : IDisposable
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        private readonly string message;

        private readonly long initialMemory;

        private readonly bool isEnabled;

        private readonly string logger;

        public PerformanceMonitor(string message) : this(message, true, null)
        {
        }

        public PerformanceMonitor(string message, bool isEnabled, string logger)
        {
            this.isEnabled = isEnabled;
            this.logger = logger;

            if (!isEnabled)
            {
                return;
            }

            initialMemory = GC.GetTotalMemory(false);
            this.message = message;
            stopwatch.Start();
        }

        public void Dispose()
        {
            if (!isEnabled)
            {
                return;
            }

            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            var durationMessage = $"{message} (Duration): {stopwatch.ElapsedMilliseconds}ms";
            var memoryMessage = $"{message} (Memory): {initialMemory}/{GC.GetTotalMemory(false)}";

            Console.WriteLine(durationMessage);
            Console.WriteLine(memoryMessage);

            LogProvider.Log.Info(logger, durationMessage);
            LogProvider.Log.Info(logger, memoryMessage);
        }
    }
}