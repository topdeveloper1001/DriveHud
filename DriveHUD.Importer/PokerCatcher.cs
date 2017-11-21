//-----------------------------------------------------------------------
// <copyright file="PokerCatcher.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    internal abstract class PokerCatcher : IPokerCatcher
    {
        protected bool isRunning;

        /// <summary>
        /// Path to libraries
        /// </summary>
        private const string LibPath = "Lib";

        /// <summary>
        /// Process where dll will be injected
        /// </summary>
        protected Process pokerClientProcess;

        /// <summary>
        /// Handle to Dll in memory
        /// </summary>
        protected IntPtr handle;

        /// <summary>
        /// Injected process module
        /// </summary>
        protected ProcessModule injectedProcessModule;

        /// <summary>
        /// Timeout between process searching operations
        /// </summary>
        private const int ProcessSearchingTimeout = 5000;

        /// <summary>
        /// Timeout 
        /// </summary>
        private const int EjectDllTimeout = 2000;

        /// <summary>
        /// True if Dll is injected
        /// </summary>
        protected bool isInjected;

        private const int AllowedFailedAttempts = 4;

        protected CancellationTokenSource cancellationTokenSource;

        protected abstract ImporterIdentifier Identifier { get; }

        protected abstract ImporterIdentifier[] PipeIdentifiers { get; }

        public virtual bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        public Process PokerClientProcess
        {
            get
            {
                return pokerClientProcess;
            }
        }

        protected abstract string DllToInject { get; }

        protected abstract string ProcessName { get; }

        protected abstract string WindowClassName { get; }

        #region IBackgroundProcess implementation

        public event EventHandler ProcessStopped;

        public virtual void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            if (isRunning)
            {
                return;
            }

            if (!IsEnabled())
            {
                LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "\"{0}\" catcher is disabled.", Identifier));
                return;
            }

            LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Starting \"{0}\" catcher", Identifier));

            // start main job
            Task.Run(() =>
            {
                isRunning = true;
                DoCatch();
            }, cancellationTokenSource.Token);
        }

        public virtual void Stop()
        {
            if (isRunning)
            {
                LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Stopping  \"{0}\" catcher", Identifier));
            }

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            isRunning = false;
        }

        #endregion

        #region IDisposable implementation

        public virtual void Dispose()
        {
        }

        #endregion

        #region Infrastructure

        /// <summary>
        /// Get client process
        /// </summary>
        /// <returns>Client process if exist, otherwise - null</returns>
        protected virtual Process GetPokerClientProcess()
        {
            var processes = Process.GetProcesses();

            var pokerClientProcess = processes.FirstOrDefault(x =>
            {
                if (!x.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(WindowClassName))
                {
                    var sb = new StringBuilder(256);

                    if (WinApi.GetClassName(x.MainWindowHandle, sb, sb.Capacity) != 0)
                    {
                        var windowTitle = WinApi.GetWindowText(x.MainWindowHandle);
                        var windowClassName = sb.ToString();

                        if (windowClassName.Equals(WindowClassName, StringComparison.OrdinalIgnoreCase))
                        {
                            LogProvider.Log.Info($"Found the process with main window = [{windowTitle}, {windowClassName}] [{Identifier}]");
                            return true;
                        }
                    }

                    return false;
                }

                return true;
            });

            return pokerClientProcess;
        }

        /// <summary>
        /// Main job of background process
        /// </summary>
        protected virtual void DoCatch()
        {
            LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Searching processes for \"{0}\". [{1}]", ProcessName, Identifier));

            var failedAttempts = 0;

            while (true)
            {
                if (!IsEnabled())
                {
                    Stop();
                }

                if (pokerClientProcess == null || pokerClientProcess.HasExited)
                {
                    failedAttempts = 0;

                    if (pokerClientProcess != null && pokerClientProcess.HasExited)
                    {
                        var pipeManager = ServiceLocator.Current.GetInstance<IPipeManager>();

                        PipeIdentifiers.ForEach(x => pipeManager.RemoveHandle(x));

                        LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Process \"{0}\" has exited. [{1}]", ProcessName, Identifier));
                    }

                    isInjected = false;

                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        RaiseProcessStopped();
                        return;
                    }

                    pokerClientProcess = GetPokerClientProcess();

                    if (pokerClientProcess == null)
                    {
                        Task.Delay(ProcessSearchingTimeout).Wait();
                        continue;
                    }

                    LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Process \"{0}\" [{1}] has been found. [{2}]", ProcessName, pokerClientProcess.Id, Identifier));
                }

                if (!isInjected && failedAttempts < AllowedFailedAttempts)
                {
                    try
                    {
                        InjectDll();
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, string.Format(CultureInfo.InvariantCulture, "Injecting of processes \"{0}\" failed. [{1}]", ProcessName, Identifier), e);

                        failedAttempts++;

                        if (failedAttempts >= AllowedFailedAttempts)
                        {
                            LogProvider.Log.Info(this, $"The limit of injecting into \"{ProcessName}\" attempts has been reached. Put catcher to sleep. [{Identifier}]");
                        }
                    }
                }

                if (cancellationTokenSource.IsCancellationRequested)
                {
                    if (pokerClientProcess != null && !pokerClientProcess.HasExited)
                    {
                        if (injectedProcessModule != null)
                        {
                            try
                            {
                                EjectDll(injectedProcessModule.BaseAddress);
                            }
                            catch (Exception e)
                            {
                                LogProvider.Log.Error(this, string.Format(CultureInfo.InvariantCulture, "Ejecting of processes \"{0}\" failed. [{1}]", ProcessName, Identifier), e);
                            }
                        }

                        handle = IntPtr.Zero;

                        RaiseProcessStopped();

                        return;
                    }
                }

                Task.Delay(ProcessSearchingTimeout).Wait();
            }
        }

        /// <summary>
        /// Raise process stopped event
        /// </summary>
        protected void RaiseProcessStopped()
        {
            isRunning = false;

            ProcessStopped?.Invoke(this, EventArgs.Empty);

            LogProvider.Log.Info(this, $"\"{Identifier}\" catcher has been stopped");
        }

        #region Dll injection

        /// <summary>
        /// Inject DLL in pokerClientProcess
        /// </summary>
        protected virtual void InjectDll()
        {
            Check.Require(pokerClientProcess != null, "Cannot inject Dll to not existing process");
            Check.Require((pokerClientProcess.Id == Process.GetCurrentProcess().Id), "Cannot create an injector for the current process");

            LogProvider.Log.Log(this, string.Format(CultureInfo.InvariantCulture, "Injecting into process \"{0}\"", ProcessName), LogMessageType.Info);

            // Enter in debug mode to be able to manipulate with processes
            Process.EnterDebugMode();

            // Required access to client process
            var requiredAccess = ProcessAccessFlags.QueryInformation | ProcessAccessFlags.CreateThread | ProcessAccessFlags.VMOperation |
                                    ProcessAccessFlags.VMWrite | ProcessAccessFlags.VMRead;

            // Open client process and get its handle
            handle = WinApi.OpenProcess(requiredAccess, false, pokerClientProcess.Id);

            if (handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var dllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), LibPath, DllToInject);

            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException(string.Format("Dll {0} is missing", dllPath));
            }

            var injectedDllProcessAddress = GetInjectedDllProcessAddress();

            // Eject dll from client process
            if (injectedDllProcessAddress != IntPtr.Zero)
            {
                LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Found already injected dll in \"{0}\" processes. Dll will be ejected. [{1}]", ProcessName, Identifier));

                EjectDll(injectedDllProcessAddress);
                Task.Delay(EjectDllTimeout).Wait();
            }

            // pointer to allocated memory of lib path string
            IntPtr pLibRemote = IntPtr.Zero;

            // handle to thread from CreateRemoteThread
            IntPtr hThread = IntPtr.Zero;

            // unmanaged C-String pointer
            IntPtr pLibFullPathUnmanaged = Marshal.StringToHGlobalUni(dllPath);

            try
            {
                uint sizeUni = (uint)Encoding.Unicode.GetByteCount(dllPath);

                // Get Handle to Kernel32.dll and pointer to LoadLibraryW
                IntPtr hKernel32 = WinApi.GetModuleHandle("Kernel32");

                if (hKernel32 == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Kernel32 not found");
                }

                IntPtr hLoadLib = WinApi.GetProcAddress(hKernel32, "LoadLibraryW");

                if (hLoadLib == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "FreeLibrary not found");
                }

                // allocate memory to the local process for libFullPath
                pLibRemote = WinApi.VirtualAllocEx(handle, IntPtr.Zero, sizeUni, AllocationType.Commit, MemoryProtection.ReadWrite);

                if (pLibRemote == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Memory hasn't been allocated");
                }

                // write libFullPath to pLibPath
                int bytesWritten;

                if (!WinApi.WriteProcessMemory(handle, pLibRemote, pLibFullPathUnmanaged, sizeUni, out bytesWritten) || bytesWritten != (int)sizeUni)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Couldn't write data in process memory");
                }

                // load dll via call to LoadLibrary using CreateRemoteThread
                hThread = WinApi.CreateRemoteThread(handle, IntPtr.Zero, 0, hLoadLib, pLibRemote, 0, IntPtr.Zero);

                if (hThread == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Couldn't create remote thread");
                }

                if (WinApi.WaitForSingleObject(hThread, (uint)ThreadWaitValue.Infinite) != (uint)ThreadWaitValue.Object0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "WaitForSingleObject return wrong code");
                }

                // get address of loaded module - this doesn't work in x64, so just iterate module list to find injected module
                IntPtr hLibModule;

                if (!WinApi.GetExitCodeThread(hThread, out hLibModule))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Couldn't obtain exit code");
                }

                if (hLibModule == IntPtr.Zero)
                {
                    throw new Exception("Code executed properly, but unable to get an appropriate module handle, possible Win32Exception", new Win32Exception(Marshal.GetLastWin32Error()));
                }

                pokerClientProcess.Refresh();

                // iterate modules in target process to find our newly injected module                
                foreach (ProcessModule module in pokerClientProcess.Modules)
                {
                    if (module.ModuleName == DllToInject)
                    {
                        injectedProcessModule = module;
                        break;
                    }
                }

                if (injectedProcessModule == null)
                {
                    throw new Exception("Injected module could not be found within the target process!");
                }

                isInjected = true;

                LogProvider.Log.Log(this, string.Format(CultureInfo.InvariantCulture, "Successfully injected into process \"{0}\". [{1}]", ProcessName, Identifier), LogMessageType.Info);
            }
            finally
            {
                Marshal.FreeHGlobal(pLibFullPathUnmanaged);
                WinApi.CloseHandle(hThread);
                WinApi.VirtualFreeEx(pokerClientProcess.Handle, pLibRemote, 0, AllocationType.Release);
            }
        }

        protected virtual void EjectDll(IntPtr injectedProcessAddress)
        {
            LogProvider.Log.Log(this, string.Format(CultureInfo.InvariantCulture, "Ejecting from process \"{0}\". [{1}]", ProcessName, Identifier), LogMessageType.Info);

            IntPtr hThread = IntPtr.Zero;

            try
            {
                // get handle to kernel32 and FreeLibrary
                IntPtr hKernel32 = WinApi.GetModuleHandle("Kernel32");

                if (hKernel32 == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Kernel32 not found");
                }

                IntPtr hFreeLib = WinApi.GetProcAddress(hKernel32, "FreeLibrary");

                if (hFreeLib == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "FreeLibrary not found");
                }

                hThread = WinApi.CreateRemoteThread(handle, IntPtr.Zero, 0, hFreeLib, injectedProcessAddress, 0, IntPtr.Zero);

                if (hThread == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Couldn't create remote thread");
                }

                if (WinApi.WaitForSingleObject(hThread, (uint)ThreadWaitValue.Infinite) != (uint)ThreadWaitValue.Object0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "WaitForSingleObject return wrong code");
                }

                IntPtr pFreeLibRet;

                if (!WinApi.GetExitCodeThread(hThread, out pFreeLibRet))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Couldn't obtain exit code");
                }

                if (pFreeLibRet == IntPtr.Zero)
                {
                    throw new Exception("FreeLibrary failed in remote process");
                }

                LogProvider.Log.Log(this, string.Format(CultureInfo.InvariantCulture, "Successfully ejected from process \"{0}\". [{1}]", ProcessName, Identifier), LogMessageType.Info);
            }
            finally
            {
                WinApi.CloseHandle(hThread);
                injectedProcessModule = null;
            }

            isInjected = false;
        }

        protected virtual IntPtr GetInjectedDllProcessAddress()
        {
            Check.Require(pokerClientProcess != null, "Client process isn't determined");

            pokerClientProcess.Refresh();

            foreach (ProcessModule module in pokerClientProcess.Modules)
            {
                if (module.ModuleName.Equals(DllToInject))
                {
                    return module.BaseAddress;
                }
            }

            return IntPtr.Zero;
        }

        protected virtual bool IsEnabled()
        {
            return true;
        }

        #endregion

        #endregion
    }
}