//-----------------------------------------------------------------------
// <copyright file="PokerClientImporter.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Importers.Loggers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Base class of importers which get data directly from poker clients
    /// </summary>
    internal abstract class PokerClientImporter : BaseImporter, IBackgroundProcess
    {
        protected abstract ImporterIdentifier Identifier { get; }

        protected IPokerClientEncryptedLogger pokerClientLogger;

        public PokerClientImporter()
        {
            InitializeLogger();
        }

        #region Infrastructure

        protected virtual void InitializeLogger()
        {
            var pokerClientLoggerConfiguration = CreatePokerClientLoggerConfiguration();

            pokerClientLogger = ServiceLocator.Current.GetInstance<IPokerClientEncryptedLogger>(LogServices.Base.ToString());
            pokerClientLogger.Initialize(pokerClientLoggerConfiguration);
        }

        protected override void DoImport()
        {
            var pipeManager = ServiceLocator.Current.GetInstance<IPipeManager>();
            var dataManager = CreatePokerClientDataManager(pokerClientLogger);

            pokerClientLogger?.CleanLogs();
            pokerClientLogger?.StartLogging();

            IntPtr pipeServerHandle = IntPtr.Zero;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (IsDisabled())
                    {
                        LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "\"{0}\" importer has been disabled. Aborting process.", Identifier));
                        break;
                    }

                    pipeServerHandle = pipeManager.GetHandle(Identifier);

                    if (pipeServerHandle == IntPtr.Zero)
                    {
                        pipeServerHandle = CreatePipeServer();

                        // If pipe hasn't been created then exit from task
                        if (pipeServerHandle == IntPtr.Zero)
                        {
                            LogProvider.Log.Error(this, string.Format(CultureInfo.InvariantCulture, "Stop importing \"{0}\" data", Identifier));
                            RaiseProcessStopped();
                            return;
                        }

                        pipeManager.AddHandle(Identifier, pipeServerHandle);
                    }

                    var totalBuffer = new List<byte>();

                    bool readResult = false;
                    uint numberOfBytesRead = 0;

                    do
                    {
                        // Buffer for pipe data
                        var lbBuffer = new byte[BufferSize];

                        // Read data from the pipe.
                        readResult = WinApi.ReadFile(
                            pipeServerHandle,
                            lbBuffer,
                            BufferSize,
                            out numberOfBytesRead,
                            IntPtr.Zero);

                        if (numberOfBytesRead > 0)
                        {
                            totalBuffer.AddRange(lbBuffer.Take((int)numberOfBytesRead));
                        }

                    } while (!readResult && numberOfBytesRead != 0);

                    // If client is not connected, or no data in raw
                    if (!readResult || numberOfBytesRead == 0)
                    {
                        try
                        {
                            Task.Delay(PipeReadingTimeout).Wait(cancellationTokenSource.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }

                        continue;
                    }

                    // process pipe's data
                    dataManager.ProcessData(totalBuffer.ToArray());
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, string.Format("Data processing of \"{0}\" failed", Identifier), e);
                }
            }

            pipeManager.RemoveHandle(Identifier);

            LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Pipe for \"{0}\" was closed.", Identifier));

            dataManager.Dispose();

            pokerClientLogger?.StopLogging();

            RaiseProcessStopped();
        }

        /// <summary>
        /// Create pipe server to get data from catcher
        /// </summary>
        protected virtual IntPtr CreatePipeServer()
        {
            Check.Require(string.IsNullOrWhiteSpace(PipeName), "Pipe name must be not empty.");

            LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Creating pipe server to get \"{0}\" data.", Identifier));

            // Create and initialize security descriptor
            SecurityDescriptor sd;

            WinApi.InitializeSecurityDescriptor(out sd, 1);
            WinApi.SetSecurityDescriptorDacl(ref sd, true, IntPtr.Zero, false);

            // Create security attributes
            var sa = new SecurityAttributes();
            sa.lpSecurityDescriptor = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SecurityDescriptor)));
            sa.bInheritHandle = false;
            sa.nLength = Marshal.SizeOf(typeof(SecurityAttributes));

            // Marshal security descriptor structure to unmanaged block
            Marshal.StructureToPtr(sd, sa.lpSecurityDescriptor, false);

            // Allocate memory in unmanaged block for security attributes structure
            var pSa = Marshal.AllocHGlobal(sa.nLength);

            // Marshal security attributes structure to unmanaged block
            Marshal.StructureToPtr(sa, pSa, false);

            // Create the named pipe            
            var pipeServerHandle = WinApi.CreateNamedPipe(
                PipeName,
                PipeOpenMode.PIPE_ACCESS_DUPLEX,
                PipeMode.PIPE_TYPE_MESSAGE |
                PipeMode.PIPE_READMODE_MESSAGE |
                PipeMode.PIPE_WAIT,
                PipeNative.PIPE_UNLIMITED_INSTANCES,
                BufferSize,
                BufferSize,
                PipeNative.NMPWAIT_USE_DEFAULT_WAIT,
                pSa);

            if (pipeServerHandle.ToInt32() == PipeNative.INVALID_HANDLE_VALUE)
            {
                LogProvider.Log.Error(this, string.Format(CultureInfo.InvariantCulture, "Unable to create named pipe {0} w/ error 0x{1:X}", PipeName, WinApi.GetLastError()));
                return IntPtr.Zero;
            }

            LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Pipe server to get \"{0}\" data has been created.", Identifier));

            return pipeServerHandle;
        }

        /// <summary>
        /// Create configuration for logger
        /// </summary>
        /// <returns>Configuration for logger</returns>
        protected abstract PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration();

        /// <summary>
        /// Create data manager
        /// </summary>
        /// <returns>Data manager</returns>
        protected abstract IPokerClientDataManager CreatePokerClientDataManager(IPokerClientEncryptedLogger logger);

        /// <summary>
        /// Pipe name to get data from client
        /// </summary>        
        protected abstract string PipeName
        {
            get;
        }

        /// <summary>
        /// Buffer size
        /// </summary>
        protected abstract uint BufferSize
        {
            get;
        }

        // Timeout for reading pipe
        protected abstract int PipeReadingTimeout
        {
            get;
        }

        #endregion     
    }
}