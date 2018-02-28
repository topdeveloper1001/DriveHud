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

namespace Simulator
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            InitializeCommands();
        }

        public ReactiveCommand RunCommand { get; private set; }

        private void InitializeCommands()
        {
            RunCommand = ReactiveCommand.Create(Run);            
        }

        private async void Run()
        {
            var tablesToGenerate = 2;
            var testData = Enumerable.Range(0, tablesToGenerate).Select(x => ACRHelper.GenerateTournamentTestData()).ToArray();
            var titleTemplate = "$10 Freeroll - On Demand, Table 54 - No Limit - 100/200 Ante 20 Hold'em ({0})";

            for (var i = 0; i < tablesToGenerate; i++)
            {
                var title = string.Format(titleTemplate, testData[i].TournamentId);
                CreateTableWindow(title, ACRHelper.ClientWidth, ACRHelper.ClientHeight);
            }

            var handHistoryFolder = new DirectoryInfo("./HandHistories/ACR");

            if (!handHistoryFolder.Exists)
            {
                handHistoryFolder.Create();
            }

            handHistoryFolder.GetFiles().ForEach(x => x.Delete());

            await Task.Run(() =>
            {
                try
                {
                    var testDataFile = @"./TestData/ACR-TestHand.txt";

                    var handHistoryTestData = File.ReadAllText(testDataFile);

                    // split by hands
                    var hands = SplitUpMultipleHands(handHistoryTestData).ToArray();

                    var handsQueue = new Queue<string>(hands);

                    for (var i = 0; i < tablesToGenerate; i++)
                    {
                        var hand = handsQueue.Dequeue();
                        handsQueue.Enqueue(hand);

                        var gameIdText = "Game ID: ";
                        var startIndexOfGameId = hand.IndexOf(gameIdText, StringComparison.Ordinal) + gameIdText.Length;
                        var endIndexOfGameId = hand.IndexOf(' ', startIndexOfGameId);
                        var gameId = hand.Substring(startIndexOfGameId, endIndexOfGameId - startIndexOfGameId);

                        hand = hand.Remove(startIndexOfGameId, gameId.Length).Insert(startIndexOfGameId, ACRHelper.GenerateGameId().ToString());

                        var handHistoryPath = Path.Combine(handHistoryFolder.FullName, testData[i].HandHistoryFile);

                        File.AppendAllText(handHistoryPath, hand + "\r\n\r\n");

                        if (i == tablesToGenerate - 1)
                        {
                            i = -1;
                        }

                        Task.Delay(10000).Wait();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });
        }

        private static Regex HandSplitRegex = new Regex("\r\n\r\n", RegexOptions.Compiled);

        public IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false)
                            .Select(s => s.Trim('\r', '\n'));
        }

        private Window CreateTableWindow(string title, int width, int height)
        {
            var pokerClient = new PokerTable();
            pokerClient.Title = title;
            pokerClient.Width = width;
            pokerClient.Height = height;
            pokerClient.Show();

            return pokerClient;
        }
    }
}
