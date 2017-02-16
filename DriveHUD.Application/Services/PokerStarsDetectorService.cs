using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Model.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Application.Services
{
    /// <summary>
    /// Service taht detects if there are any instance of PokerStars opened
    /// </summary>
    internal class PokerStarsDetectorSingletonService
    {
        private const string ProcessName = "PokerStars";
        private const int TIMEOUT_MS = 5000;

        #region Singleton

        private static volatile PokerStarsDetectorSingletonService instance;
        private static object syncRoot = new object();

        public static PokerStarsDetectorSingletonService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PokerStarsDetectorSingletonService();
                    }
                }

                return instance;
            }
        }

        #endregion

        private PokerStarsDetectorSingletonService()
        {
            _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        #region Properties

        private CancellationTokenSource _cts;
        private Task _detectionTask;

        private IEventAggregator _eventAggregator;

        private bool _isRunning = false;
        private bool _isPokerStarsDetected = false;

        internal bool IsRunning
        {
            get { return _isRunning; }
            private set { _isRunning = value; }
        }

        internal bool IsPokerStarsDetected
        {
            get { return _isPokerStarsDetected; }
            private set { _isPokerStarsDetected = value; }
        }

        #endregion

        #region Methods

        internal void Start()
        {
            if (_isRunning)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            _detectionTask = Task.Run(() => Detect(_cts));
        }

        internal void Stop()
        {
            if (!_isRunning)
                return;

            _cts.Cancel();
        }

        private async Task Detect(CancellationTokenSource cts)
        {
            IsRunning = true;
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    var processes = Process.GetProcesses();
                    SendDetected(processes.Any(x => x.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase)));

                    await Task.Delay(TIMEOUT_MS);
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, ex);
                }
            }
            IsRunning = false;
        }

        private void SendDetected(bool isDetected)
        {
            if (IsPokerStarsDetected != isDetected)
            {
                _isPokerStarsDetected = isDetected;
                _eventAggregator.GetEvent<PokerStarsDetectedEvent>().Publish(new PokerStarsDetectedEventArgs(_isPokerStarsDetected));
            }
        }

        #endregion
    }
}
