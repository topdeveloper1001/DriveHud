using DriveHUD.Common.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DriveHUD.Application.HudServices
{
    internal class HudTransmitter : IHudTransmitter
    {
        private string hudClientFileName = "DriveHUD.HUD.exe";
        private Process hudClient;
        private AnonymousPipeServerStream pipeServer;
        private ReaderWriterLockSlim locker;

        private bool isInitialized;

        public void Initialize()
        {
            LogProvider.Log.Info("Initializing HUD");

            isInitialized = false;
            locker = new ReaderWriterLockSlim();

            Task.Run(() => InitializeInternal());
        }

        private void InitializeInternal()
        {
            try
            {
                var existingClientProcess = Process.GetProcessesByName(hudClientFileName).FirstOrDefault();

                if (existingClientProcess != null)
                {
                    existingClientProcess.Kill();
                }

                pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
                hudClient = BuildClientProcess(pipeServer.GetClientHandleAsString());
                hudClient.Start();
                pipeServer.DisposeLocalCopyOfClientHandle();

                isInitialized = true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "HUD cannot be initialized", e);
            }
        }

        public void Send(byte[] data)
        {
            if (!isInitialized)
            {
                LogProvider.Log.Error("HUD hasn't been initialized");
                return;
            }

            SendInternal(data);
        }

        private void SendInternal(byte[] data)
        {
            try
            {
                locker.EnterWriteLock();

                if (pipeServer.IsConnected && !hudClient.HasExited)
                {
                    using (var writer = new BinaryWriter(pipeServer, Encoding.UTF8, true))
                    {
                        pipeServer.WaitForPipeDrain();
                        pipeServer.Write(data, 0, data.Length);
                        writer.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "HUD data cannot be sent", e);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private Process BuildClientProcess(string clientHandle)
        {
            if (!File.Exists(hudClientFileName))
            {
                throw new FileNotFoundException(string.Format("{0} not found.", hudClientFileName));
            }

            var hudClient = new Process();
            hudClient.StartInfo.FileName = hudClientFileName;
            hudClient.StartInfo.Arguments = clientHandle;
            hudClient.StartInfo.UseShellExecute = false;

            return hudClient;
        }

        public void Dispose()
        {
            isInitialized = false;

            if (pipeServer != null)
            {
                pipeServer.Dispose();
            }

            if (hudClient != null && !hudClient.HasExited)
            {
                hudClient.Kill();
            }
        }
    }
}
