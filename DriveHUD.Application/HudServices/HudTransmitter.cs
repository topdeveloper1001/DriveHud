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
using DriveHUD.HUD.Service;
using System.ServiceModel;

namespace DriveHUD.Application.HudServices
{
    internal class HudTransmitter : IHudTransmitter
    {
        private const string hudClientFileName = "DriveHUD.HUD.exe";
        private const double delayMS = 1000;

        private Process hudClient;
        private DuplexChannelFactory<IHudNamedPipeBindingService> _namedPipeBindingFactory;
        private IHudNamedPipeBindingService _namedPipeBindingProxy;

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

                hudClient = BuildClientProcess();
                hudClient.Start();
                StartPipe();

                isInitialized = true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "HUD cannot be initialized", e);
            }
        }

        private void StartPipe()
        {
            // start named pipe
            InstanceContext context = new InstanceContext(this);
            _namedPipeBindingFactory = new DuplexChannelFactory<IHudNamedPipeBindingService>(context, "HudNamedPipeBindingServiceEndpoint");
            _namedPipeBindingProxy = _namedPipeBindingFactory.CreateChannel();
            ((IClientChannel)_namedPipeBindingProxy).Faulted += new EventHandler(Pipe_Faulted);
            ((IClientChannel)_namedPipeBindingProxy).Opened += new EventHandler(Pipe_Opened);

            while (!hudClient.HasExited)
            {
                try
                {
                    Task.Delay(TimeSpan.FromMilliseconds(delayMS)).Wait();

                    ((IClientChannel)_namedPipeBindingProxy).Open();

                    if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
                    {
                        LogProvider.Log.Info("Successfully connected to the HUD service.");
                    }

                    return;
                }
                catch (EndpointNotFoundException)
                {
                    // service hasn't been started yet
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(ex);
                }
            }
        }

        public void Send(byte[] data)
        {
            if (!isInitialized)
            {
                LogProvider.Log.Error(this, "HUD hasn't been initialized");
                return;
            }

            locker.EnterWriteLock();
            try
            {
                _namedPipeBindingProxy.UpdateHUD(data);
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

        private Process BuildClientProcess()
        {
            if (!File.Exists(hudClientFileName))
            {
                throw new FileNotFoundException(string.Format("{0} not found.", hudClientFileName));
            }

            if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
            {
                LogProvider.Log.Info(this, $"Starting HUD process");
            }

            var hudClient = new Process();
            hudClient.StartInfo.FileName = hudClientFileName;
            hudClient.StartInfo.UseShellExecute = false;

            return hudClient;
        }

        #region EventHandlers

        void Pipe_Opened(object sender, EventArgs e)
        {
            try
            {
                _namedPipeBindingProxy.ConnectCallbackChannel();
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error("Failed to setup the callback service", ex);
            }
        }

        private void Pipe_Faulted(object sender, EventArgs e)
        {
            LogProvider.Log.Info("Pipe Faulted.");
        }

        #endregion

        #region IHudNamedPipeBindingCallbackService

        public void Message(string test)
        {
            LogProvider.Log.Info("Message received: " + test);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            isInitialized = false;

            if (_namedPipeBindingFactory != null)
            {
                if (_namedPipeBindingFactory.State == CommunicationState.Opened)
                {
                    _namedPipeBindingFactory.Close();
                }

                _namedPipeBindingFactory.Faulted -= Pipe_Faulted;
                _namedPipeBindingFactory.Opened -= Pipe_Opened;

                _namedPipeBindingFactory = null;
                _namedPipeBindingProxy = null;
            }

            if (hudClient != null && !hudClient.HasExited)
            {
                hudClient.Kill();
            }
        }

        #endregion
    }
}
