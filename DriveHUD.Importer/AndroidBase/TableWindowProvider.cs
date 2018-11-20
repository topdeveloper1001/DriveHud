//-----------------------------------------------------------------------
// <copyright file="TableWindowProvider.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Importers.AndroidBase.EmulatorProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DriveHUD.Importers.AndroidBase
{
    internal class TableWindowProvider : ITableWindowProvider
    {
        private readonly Dictionary<int, TableWindow> windows = new Dictionary<int, TableWindow>();

        private readonly IEmulatorProvider[] providers = new IEmulatorProvider[]
        {
            new NoxEmulatorProvider(),
            new MemuEmulatorProvider(),
            new MomoEmulatorProvider()
        };

        private string logger;

        public void SetLogger(string logger)
        {
            this.logger = logger;
            providers.ForEach(x => x.Logger = logger);
        }

        public IntPtr GetTableWindowHandle(Process process)
        {
            if (process == null)
            {
                return IntPtr.Zero;
            }

            try
            {
                if (windows.TryGetValue(process.Id, out TableWindow window))
                {
                    if (!window.Process.HasExited && WinApi.IsWindow(window.WindowHandle))
                    {
                        return window.WindowHandle;
                    }

                    windows.Remove(process.Id);
                }

                foreach (var provider in providers)
                {
                    if (provider.CanProvide(process))
                    {
                        var windowHandle = provider.GetProcessWindowHandle(process);

                        if (windowHandle != IntPtr.Zero)
                        {
                            windows.Add(process.Id, new TableWindow
                            {
                                Process = process,
                                WindowHandle = windowHandle
                            });
                        }

                        return windowHandle;
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(logger, $"Could not find window associated with process={process.Id}", e);
            }

            return IntPtr.Zero;
        }

        private class TableWindow
        {
            public Process Process { get; set; }

            public IntPtr WindowHandle { get; set; }
        }
    }
}