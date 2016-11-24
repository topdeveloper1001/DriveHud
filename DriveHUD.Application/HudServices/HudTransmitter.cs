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
using Microsoft.Practices.ServiceLocation;
using Model.Settings;

namespace DriveHUD.Application.HudServices
{
    internal class HudTransmitter : IHudTransmitter
    {
        private string hudClientFileName = "DriveHUD.HUD.exe";
        private Process hudClient;
        private AnonymousPipeServerStream pipeServer;
        private ReaderWriterLockSlim locker;
        private SettingsModel settingsModel;

        private bool isInitialized;

        public void Initialize()
        {
            LogProvider.Log.Info(this, "Initializing HUD");

            settingsModel = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

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

                if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
                {
                    SendSync();
                }
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
                LogProvider.Log.Error(this, "HUD hasn't been initialized");
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
                else
                {
                    LogProvider.Log.Warn(this, $"HUD data cannot be sent: PipeStatus={pipeServer.IsConnected}] HudStatus={!hudClient.HasExited}");
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

        private void SendSync()
        {
            LogProvider.Log.Info(this, "Synchronizing HUD");

            var syncCommand = Encoding.UTF8.GetBytes("SYNC");
            SendInternal(syncCommand);

            LogProvider.Log.Info(this, "HUD sync command has been sent");
        }

        private Process BuildClientProcess(string clientHandle)
        {
            if (!File.Exists(hudClientFileName))
            {
                throw new FileNotFoundException(string.Format("{0} not found.", hudClientFileName));
            }
            
            if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
            {
                LogProvider.Log.Info(this, $"Start HUD process with handle={clientHandle}");
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
