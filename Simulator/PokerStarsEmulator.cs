using DriveHUD.Common.Linq;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Entities;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace Simulator
{
    public class PokerStarsEmulator : ViewModelBase, IPokerEmulator
    {
        private const int delayBetweenHands = 15000;

        public PokerStarsEmulator()
        {
            tablesToEmulateCollection = new ObservableCollection<int>(Enumerable.Range(1, 10));

            BrowseCommand = ReactiveCommand.Create(() =>
            {
                var fbDialog = new FolderBrowserDialog();

                if (fbDialog.ShowDialog() == DialogResult.OK)
                {
                    HandHistoryLocation = fbDialog.SelectedPath;
                }
            });
        }

        protected virtual string HandHistoryFilter { get => "*.txt"; }

        public virtual string Name { get => "PokerStars Emulator"; }

        public virtual EnumPokerSites Site => EnumPokerSites.PokerStars;

        private ObservableCollection<int> tablesToEmulateCollection;

        public ObservableCollection<int> TablesToEmulateCollection
        {
            get
            {
                return tablesToEmulateCollection;
            }
        }

        public bool CanRun
        {
            get
            {
                return tablesToEmulate > 0 && Directory.Exists(HandHistoryLocation);
            }
        }

        private int tablesToEmulate;

        public int TablesToEmulate
        {
            get
            {
                return tablesToEmulate;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref tablesToEmulate, value);
                this.RaisePropertyChanged(nameof(CanRun));
            }
        }

        private string handHistoryLocation;

        public string HandHistoryLocation
        {
            get
            {
                return handHistoryLocation;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref handHistoryLocation, value);
                this.RaisePropertyChanged(nameof(CanRun));
            }
        }

        #region Commands

        public ICommand BrowseCommand { get; private set; }

        #endregion

        public Task Run(CancellationToken cancellationToken)
        {
            var files = Directory.GetFiles(HandHistoryLocation, HandHistoryFilter)
                .ToList()
                .SplitListByChunk(TablesToEmulate)
                .ToList();

            var emulationTasks = new List<Task>();

            for (var i = 0; i < TablesToEmulate; i++)
            {
                if (i >= files.Count)
                {
                    break;
                }

                emulationTasks.Add(EmulateTable($"Table #{i + 1}", files[i], cancellationToken));
            }

            return Task.WhenAll(emulationTasks);
        }

        private Task EmulateTable(string tableTitle, IEnumerable<string> files, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Window window = null;
                int windowHandle = 0;

                bool windowClosed = false;

                App.Current.Dispatcher.Invoke(() =>
                {
                    window = Utils.CreateTableWindow(tableTitle, 808, 585);
                    window.Closed += (s, e) => windowClosed = true;
                    windowHandle = new WindowInteropHelper(window).Handle.ToInt32();
                });

                foreach (var file in files)
                {
                    if (cancellationToken.IsCancellationRequested || windowClosed)
                    {
                        break;
                    }

                    var fileContent = File.ReadAllText(file);

                    var handHistories = Utils.SplitUpMultipleHands(fileContent);

                    foreach (var handHistory in handHistories)
                    {
                        if (cancellationToken.IsCancellationRequested || windowClosed)
                        {
                            break;
                        }

                        var handHistoryDto = new HandHistoryDto
                        {
                            HandText = handHistory,
                            PokerSite = Site,
                            WindowHandle = windowHandle
                        };

                        var dhImporterService = Utils.GetImporterService();

                        try
                        {
                            dhImporterService.ImportHandHistory(handHistoryDto);
                        }
                        catch
                        {
                            dhImporterService = Utils.GetImporterService(true);
                        }

                        try
                        {
                            Task.Delay(delayBetweenHands).Wait(cancellationToken);
                        }
                        catch
                        {
                        }
                    }
                }

                while (!cancellationToken.IsCancellationRequested || windowClosed)
                {
                    try
                    {
                        Task.Delay(delayBetweenHands).Wait(cancellationToken);
                    }
                    catch
                    {
                    }
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!windowClosed)
                    {
                        window.Close();
                    }
                });
            });
        }
    }
}