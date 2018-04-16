using DriveHUD.Application.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Wpf.Mvvm;
using System.ServiceModel;
using DriveHUD.Entities;
using System.Windows.Interop;
using System.Collections.ObjectModel;
using System.Threading;

namespace Simulator
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            InitializeEmulators();
            InitializeCommands();
        }

        #region Properties

        private CancellationTokenSource cancellationTokenSource;

        private ObservableCollection<IPokerEmulator> emulators;

        public ObservableCollection<IPokerEmulator> Emulators
        {
            get
            {
                return emulators;
            }
        }

        private IPokerEmulator selectedEmulator;

        public IPokerEmulator SelectedEmulator
        {
            get
            {
                return selectedEmulator;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedEmulator, value);
            }
        }

        private bool isRunning;

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isRunning, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand RunCommand { get; private set; }

        public ReactiveCommand StopCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void InitializeCommands()
        {
            RunCommand = ReactiveCommand.Create(Run, this.WhenAny(x1 => x1.SelectedEmulator.CanRun, x2 => x2.IsRunning, (x1, x2) => x1.Value && !x2.Value));
            StopCommand = ReactiveCommand.Create(Stop, this.WhenAny(x => x.IsRunning, x => x.Value));
        }

        private void InitializeEmulators()
        {
            emulators = new ObservableCollection<IPokerEmulator>
            {
                new PokerStarsEmulator()
            };
        }

        private async void Run()
        {
            cancellationTokenSource = new CancellationTokenSource();

            IsRunning = true;

            await SelectedEmulator.Run(cancellationTokenSource.Token);

            IsRunning = false;
        }

        private void Stop()
        {
            cancellationTokenSource?.Cancel();
        }

        #endregion

        //private async void Run()
        //{
        //    var tablesToGenerate = 2;
        //    var testData = Enumerable.Range(0, tablesToGenerate).Select(x => ACRHelper.GenerateTournamentTestData()).ToArray();
        //    var titleTemplate = "$10 Freeroll - On Demand, Table 54 - No Limit - 100/200 Ante 20 Hold'em ({0})";

        //    for (var i = 0; i < tablesToGenerate; i++)
        //    {
        //        var title = string.Format(titleTemplate, testData[i].TournamentId);
        //        Utils.CreateTableWindow(title, ACRHelper.ClientWidth, ACRHelper.ClientHeight);
        //    }

        //    var handHistoryFolder = new DirectoryInfo("./HandHistories/ACR");

        //    if (!handHistoryFolder.Exists)
        //    {
        //        handHistoryFolder.Create();
        //    }

        //    handHistoryFolder.GetFiles().ForEach(x => x.Delete());

        //    await Task.Run(() =>
        //    {
        //        try
        //        {
        //            var testDataFile = @"./TestData/ACR-TestHand.txt";

        //            var handHistoryTestData = File.ReadAllText(testDataFile);

        //            // split by hands
        //            var hands = Utils.SplitUpMultipleHands(handHistoryTestData).ToArray();

        //            var handsQueue = new Queue<string>(hands);

        //            for (var i = 0; i < tablesToGenerate; i++)
        //            {
        //                var hand = handsQueue.Dequeue();
        //                handsQueue.Enqueue(hand);

        //                var gameIdText = "Game ID: ";
        //                var startIndexOfGameId = hand.IndexOf(gameIdText, StringComparison.Ordinal) + gameIdText.Length;
        //                var endIndexOfGameId = hand.IndexOf(' ', startIndexOfGameId);
        //                var gameId = hand.Substring(startIndexOfGameId, endIndexOfGameId - startIndexOfGameId);

        //                hand = hand.Remove(startIndexOfGameId, gameId.Length).Insert(startIndexOfGameId, ACRHelper.GenerateGameId().ToString());

        //                var handHistoryPath = Path.Combine(handHistoryFolder.FullName, testData[i].HandHistoryFile);

        //                File.AppendAllText(handHistoryPath, hand + "\r\n\r\n");

        //                if (i == tablesToGenerate - 1)
        //                {
        //                    i = -1;
        //                }

        //                Task.Delay(10000).Wait();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine(e);
        //        }
        //    });
        //}

        private async void SendHand()
        {
            var endpointAddress = "net.pipe://localhost/DriveHUD/Importer/ImportHandHistory";
            var pipeFactory = new ChannelFactory<IDHImporterService>(new NetNamedPipeBinding(), new EndpointAddress(endpointAddress));
            var dhImporterService = pipeFactory.CreateChannel();

            var window = Utils.CreateTableWindow("TestWindow", 816, 631);

            var handHistoryDto = new HandHistoryDto
            {
                PokerSite = EnumPokerSites.BetOnline,
                WindowHandle = new WindowInteropHelper(window).Handle.ToInt32()
            };

            await Task.Run(() =>
            {
                var testDataFile = @"./TestData/BetOnlineGenericHHWithPlayersPlayedManyGames-6max.xml";

                var handHistoryText = File.ReadAllText(testDataFile);

                handHistoryDto.HandText = handHistoryText;

                dhImporterService.ImportHandHistory(handHistoryDto);

            });
        }
    }
}