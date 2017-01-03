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
    public class HudTransmitter : IHudTransmitter
    {
        private const string hudClientFileName = "DriveHUD.HUD.exe";
        private const double delayMS = 1000;

        private Process hudClient;
        private DuplexChannelFactory<IHudNamedPipeBindingService> _namedPipeBindingFactory;
        private IHudNamedPipeBindingService _namedPipeBindingProxy;
        private IHudNamedPipeBindingCallbackService _callbackService;

        private Task _initializeTask;
        private CancellationTokenSource _cancellationTokenSource;

        private ReaderWriterLockSlim locker;
        private SettingsModel settingsModel;

        private bool isInitialized;

        public void Initialize()
        {
            LogProvider.Log.Info(this, "Initializing HUD");

            settingsModel = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            isInitialized = false;
            locker = new ReaderWriterLockSlim();
            _cancellationTokenSource = new CancellationTokenSource();

            _initializeTask = new Task(InitializeInternal);
            _initializeTask.Start();
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

                Task.Delay(5000).Wait();

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
            // create named pipe binding
            _callbackService = new HudNamedPipeBindingCallbackService();

            InstanceContext context = new InstanceContext(_callbackService);
            _namedPipeBindingFactory = new DuplexChannelFactory<IHudNamedPipeBindingService>(context, "HudNamedPipeBindingServiceEndpoint");
            _namedPipeBindingProxy = CreateProxyChannel(_namedPipeBindingFactory);

            while (!hudClient.HasExited)
            {
                try
                {
                    Task.Delay(TimeSpan.FromMilliseconds(delayMS)).Wait();

                    if (_cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested)
                    {
                        LogProvider.Log.Info(this, "Initialize cancelled");
                        return;
                    }

                    ((IClientChannel)_namedPipeBindingProxy).Open();

                    if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
                    {
                        LogProvider.Log.Info(this, "Successfully connected to the HUD service.");
                    }

                    return;
                }
                catch (EndpointNotFoundException)
                {
                    LogProvider.Log.Info(this, "Service hasn't been created yet.");

                    ((IClientChannel)_namedPipeBindingProxy).Abort();

                    ((IClientChannel)_namedPipeBindingProxy).Faulted -= Pipe_Faulted;
                    ((IClientChannel)_namedPipeBindingProxy).Opened -= Pipe_Opened;

                    _namedPipeBindingProxy = CreateProxyChannel(_namedPipeBindingFactory);
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, ex);
                }
            }
        }

        private IHudNamedPipeBindingService CreateProxyChannel(DuplexChannelFactory<IHudNamedPipeBindingService> bindingFactory)
        {
            var namedPipeBindingProxy = bindingFactory.CreateChannel();
            ((IClientChannel)namedPipeBindingProxy).Faulted += new EventHandler(Pipe_Faulted);
            ((IClientChannel)namedPipeBindingProxy).Opened += new EventHandler(Pipe_Opened);

            return namedPipeBindingProxy;
        }

        public void Send(byte[] data)
        {
            if (!isInitialized)
            {
                LogProvider.Log.Error(this, "HUD hasn't been initialized.");
                return;
            }

            if (hudClient.HasExited)
            {
                LogProvider.Log.Error(this, "HUD has exited.");
                Reinitialize();
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

        private

        #region EventHandlers

        void Pipe_Opened(object sender, EventArgs e)
        {
            try
            {
                string name = string.Empty;
                App.Current.Dispatcher.Invoke(() => { name = App.Current.MainWindow.Title; });

                _namedPipeBindingProxy.ConnectCallbackChannel(name);
                LogProvider.Log.Info(this, "HUD callback channel has been createad.");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Failed to setup the callback service", ex);
            }
        }

        private void Pipe_Faulted(object sender, EventArgs e)
        {
            if (isInitialized)
            {
                LogProvider.Log.Info(this, "HUD Service Faulted.");
                Reinitialize();
            }
        }

        private void Reinitialize()
        {
            LogProvider.Log.Info(this, "Trying to re-initialize the HUD.");
            Close();
            Initialize();
        }

        private void Close()
        {
            if (_initializeTask != null && _initializeTask.Status != TaskStatus.RanToCompletion)
            {
                try
                {
                    _cancellationTokenSource.Cancel();
                    _initializeTask.Wait();
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, ex);
                }
            }

            isInitialized = false;

            _initializeTask = null;
            _cancellationTokenSource = null;

            if (_namedPipeBindingFactory != null)
            {
                _namedPipeBindingFactory.Faulted -= Pipe_Faulted;
                _namedPipeBindingFactory.Opened -= Pipe_Opened;
                _namedPipeBindingFactory.Abort();

                _namedPipeBindingFactory = null;
                _namedPipeBindingProxy = null;
                _callbackService = null;
            }

            if (hudClient != null && !hudClient.HasExited)
            {
                hudClient.Kill();
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
