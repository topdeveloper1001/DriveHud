//-----------------------------------------------------------------------
// <copyright file="EmulatorService.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Importers.AndroidBase.EmulatorProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DriveHUD.Importers.AndroidBase
{
    internal class EmulatorService : IEmulatorService
    {
        private const int exitTimeout = 5000;

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
                        var windowHandle = provider.GetProcessWindowHandle(process, out Process emulatorProcess);

                        if (windowHandle != IntPtr.Zero)
                        {
                            windows.Add(process.Id, new TableWindow
                            {
                                Process = process,
                                EmulatorProcess = emulatorProcess,
                                WindowHandle = windowHandle,
                                Provider = provider
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

        public string[] ExecuteAdbCommand(Process process, params string[] args)
        {
            if (process == null)
            {
                return new string[0];
            }

            try
            {
                if (!windows.TryGetValue(process.Id, out TableWindow window) ||
                    window.Process.HasExited || window.EmulatorProcess.HasExited)
                {
                    LogProvider.Log.Warn(this, $"Failed to execute emu command because process isn't recognized.");
                    return new string[0];
                }

                var provider = window.Provider;

                var emulatorPath = WinApi.GetMainModuleFileName(window.EmulatorProcess);

                var adb = Path.Combine(Path.GetDirectoryName(emulatorPath), provider.GetAdbLocation());

                var devices = GetAdbDevices(adb);

                var result = new List<string>();

                foreach (var device in devices)
                {
                    try
                    {
                        var processInfo = new ProcessStartInfo
                        {
                            FileName = adb,
                            Arguments = $"-s {device} {string.Join(" ", args)}",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };

                        using (var adbProcess = new Process())
                        {
                            adbProcess.StartInfo = processInfo;
                            adbProcess.StartInfo.RedirectStandardOutput = true;
                            adbProcess.Start();

                            var reader = adbProcess.StandardOutput;
                            var output = reader.ReadToEnd();

                            if (!output.StartsWith("error:", StringComparison.OrdinalIgnoreCase) &&
                                !output.StartsWith("*", StringComparison.OrdinalIgnoreCase))
                            {
                                result.Add(output);
                            }

                            adbProcess.WaitForExit(exitTimeout);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error(this, $"Failed to execute command on device {device}", ex);
                    }
                }

                return result.ToArray();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(logger, $"Could not find window associated with process={process.Id}", e);
            }

            return new string[0];
        }

        private string[] GetAdbDevices(string adb)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = adb,
                    Arguments = "devices",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    var reader = process.StandardOutput;

                    var devices = new List<string>();
                    var outputLines = new List<string>();

                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();

                        if (!string.IsNullOrEmpty(line))
                        {
                            outputLines.Add(line);
                        }
                    }

                    if (outputLines.Count == 0)
                    {
                        return new string[0];
                    }

                    foreach (var line in outputLines.Skip(1))
                    {
                        if (line.ContainsIgnoreCase("offline"))
                        {
                            continue;
                        }

                        var endIndex = line.IndexOf("\t");

                        if (endIndex >= 0)
                        {
                            var device = line.Substring(0, endIndex);
                            devices.Add(device);
                        }
                    }

                    process.WaitForExit(exitTimeout);

                    return devices.ToArray();
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Failed to read the list of devices.", e);
            }

            return new string[0];
        }

        private class TableWindow
        {
            public Process Process { get; set; }

            public Process EmulatorProcess { get; set; }

            public IntPtr WindowHandle { get; set; }

            public IEmulatorProvider Provider { get; set; }
        }
    }
}