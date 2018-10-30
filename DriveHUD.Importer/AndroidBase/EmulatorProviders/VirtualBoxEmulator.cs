//-----------------------------------------------------------------------
// <copyright file="VirtualBoxEmulator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
using System;
using System.Diagnostics;
using System.Linq;

namespace DriveHUD.Importers.AndroidBase.EmulatorProviders
{
    internal abstract class VirtualBoxEmulator : IEmulatorProvider
    {
        public string Logger { get; set; }

        protected abstract string EmulatorName { get; }

        protected abstract string ProcessName { get; }

        protected abstract string VbProcessName { get; }

        protected abstract string InstanceArgumentPrefix { get; }

        protected abstract string VbInstanceArgumentPrefix { get; }

        protected virtual int? VbEmptyInstanceNumber { get { return 0; } }

        protected virtual int? EmptyInstanceNumber { get { return null; } }

        public bool CanProvide(Process process)
        {
            try
            {
                var result = process != null && process.ProcessName.StartsWith(VbProcessName, StringComparison.OrdinalIgnoreCase);
                LogProvider.Log.Info(Logger, $"Check if process '{process?.ProcessName}' matches '{VbProcessName}': {result} [{EmulatorName}]");

                return result;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(Logger, $"Could not check if process is associated with {EmulatorName} emulator {process.Id}", e);
                return false;
            }
        }

        public IntPtr GetProcessWindowHandle(Process process)
        {
            try
            {
                if (process == null || process.HasExited)
                {
                    return IntPtr.Zero;
                }

                var cmd = process.GetCommandLine();

                var instanceNumber = GetInstanceNumber(cmd, VbInstanceArgumentPrefix, null);

                LogProvider.Log.Info(Logger, $"Check command line of '{process?.ProcessName}': '{cmd}'. Instance: {instanceNumber} [{EmulatorName}]");

                var emulatorProcess = Process.GetProcesses().
                    FirstOrDefault(x =>
                    {
                        if (!x.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase) || x.Id == process.Id)
                        {
                            return false;
                        }

                        var cmdLine = x.GetCommandLine();

                        var currentNoxInstanceNumber = GetInstanceNumber(cmdLine, InstanceArgumentPrefix, EmptyInstanceNumber);

                        return instanceNumber.HasValue ?
                            currentNoxInstanceNumber == instanceNumber :
                            (!currentNoxInstanceNumber.HasValue ||
                                (currentNoxInstanceNumber.HasValue && VbEmptyInstanceNumber == currentNoxInstanceNumber));
                    });

                if (emulatorProcess != null)
                {
                    LogProvider.Log.Info(Logger, $"Found emulator process '{emulatorProcess.ProcessName}'. MainWindow={emulatorProcess.MainWindowHandle} [{EmulatorName}]");

                    if (emulatorProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        emulatorProcess.Refresh();
                    }

                    return emulatorProcess.MainWindowHandle;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(Logger, $"Could not get window handle of {EmulatorName} emulator for process {process.Id}", e);
            }

            return IntPtr.Zero;
        }

        private int? GetInstanceNumber(string cmd, string argumentPrefix, int? emptyInstanceNumber)
        {
            int? instanceNumber = emptyInstanceNumber;

            var instanceArgumentPrefixLength = argumentPrefix.Length;
            var instanceIndex = cmd.IndexOf(argumentPrefix, StringComparison.OrdinalIgnoreCase);

            if (instanceIndex > 0)
            {
                var spaceAfterNoxInstanceIndex = cmd.IndexOf(' ', instanceIndex);

                var instanceIndexText = spaceAfterNoxInstanceIndex > 0 ?
                    cmd.Substring(instanceIndex + instanceArgumentPrefixLength, spaceAfterNoxInstanceIndex - instanceIndex - instanceArgumentPrefixLength) :
                    cmd.Substring(instanceIndex + instanceArgumentPrefixLength);

                instanceIndexText = ExtractInstanceNumber(instanceIndexText);

                if (int.TryParse(instanceIndexText, out int parsedIntanceNumber))
                {
                    instanceNumber = parsedIntanceNumber;
                }
            }

            return instanceNumber;
        }

        protected virtual string ExtractInstanceNumber(string instanceIndexText)
        {
            return instanceIndexText;
        }
    }
}