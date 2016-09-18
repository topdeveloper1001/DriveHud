using DriveHUD.Bootstrapper.App.Infrastructure;
using DriveHUD.Bootstrapper.App.Utilities;
using DriveHUD.Bootstrapper.App.ViewModels;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Threading;

namespace DriveHUD.Bootstrapper.App.Models
{
    public class BootstrapperAppSingletonModel
    {
        #region Singleton

        private static BootstrapperAppSingletonModel _instance;
        public static BootstrapperAppSingletonModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BootstrapperAppSingletonModel();
                }
                return _instance;
            }
        }

        #endregion

        private string _manufacturer = "Ace Poker Solutions";
        private IntPtr _hwnd;
        private BundleInfo _bundleInfo;

        private BootstrapperAppSingletonModel()
        {
            this._hwnd = IntPtr.Zero;

            this.ErrorsList = new List<string>();
        }

        #region Properties
        private string _installDir;
        private bool? _isCreateDesktopShortcut;
        private bool? _isCreateProgramMenuShortcut;
        private bool? _isRemovePlayerData;

        public BootstrapperApp Bootstrapper { get; set; }

        public int FinalResult { get; set; }

        public MainWindowViewModel MainWindowViewModel { get; set; }

        public string InstallDir
        {
            get
            {
                if (String.IsNullOrEmpty(_installDir))
                {
                    _installDir = GetDefaultInstallPath();
                }
                return _installDir.Trim('"');
            }
            set
            {
                _installDir = value;
                Bootstrapper.Engine.StringVariables[Variables.ManufacturerDir] = _installDir;
            }
        }

        public string ExecutableName
        {
            get
            {
                if (Bootstrapper.Engine.StringVariables.Contains(Variables.Executable))
                {
                    return Bootstrapper.Engine.StringVariables[Variables.Executable];
                }
                return string.Empty;
            }
        }

        public string RelativeExecutablePath
        {
            get
            {
                if (Bootstrapper.Engine.StringVariables.Contains(Variables.RelativeExecutablePath))
                {
                    return Bootstrapper.Engine.StringVariables[Variables.RelativeExecutablePath];
                }
                return string.Empty;
            }
        }

        public bool IsCreateDesktopShortcut
        {
            get
            {
                if(_isCreateDesktopShortcut == null)
                {
                    IsCreateDesktopShortcut = GetDefaultDesktopShortcutState();
                }
                return _isCreateDesktopShortcut.HasValue ? _isCreateDesktopShortcut.Value : false;
            }
            set
            {
                _isCreateDesktopShortcut = value;
                Bootstrapper.Engine.StringVariables[Variables.InstallDesktopShortcut] = value ? _isCreateDesktopShortcut.ToString() : null;
            }
        }

        public bool IsCreateProgramMenuShortcut
        {
            get
            {
                if (_isCreateProgramMenuShortcut == null)
                {
                    IsCreateProgramMenuShortcut = GetDefaultProgramMenuShortcutState();
                }
                return _isCreateProgramMenuShortcut.HasValue ? _isCreateProgramMenuShortcut.Value : false;
            }
            set
            {
                _isCreateProgramMenuShortcut = value;
                Bootstrapper.Engine.StringVariables[Variables.InstallProgramMenuShortcut] = value ? _isCreateProgramMenuShortcut.ToString() : null;
            }
        }

        public bool IsRemovePlayerData
        {
            get
            {
                if (_isRemovePlayerData == null)
                {
                    IsRemovePlayerData = GetDefaultRemovePlayerDataState();
                }
                return _isRemovePlayerData.HasValue ? _isRemovePlayerData.Value : false;
            }
            set
            {
                _isRemovePlayerData = value;
                Bootstrapper.Engine.StringVariables[Variables.RemovePlayerData] = value ? _isRemovePlayerData.ToString() : null;
            }
        }

        public BundleInfo BundleInfo
        {
            get
            {
                if (_bundleInfo == null)
                {
                    _bundleInfo = BundleInfoLoader.Load();
                }
                return _bundleInfo;
            }
        }

        public LaunchAction LastPlannedAction { get; set; }

        public List<string> ErrorsList { get; set; }
        #endregion

        #region Methods
        private string GetDefaultInstallPath()
        {
            string result = string.Empty;
            if (Bootstrapper.Engine.StringVariables.Contains(Variables.Manufacturer))
            {
                result = Bootstrapper.Engine.StringVariables[Variables.Manufacturer];
            }
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), string.IsNullOrEmpty(result) ? _manufacturer : result);
        }

        private bool GetDefaultDesktopShortcutState()
        {
            bool result = true;
            if (Bootstrapper.Engine.StringVariables.Contains(Variables.InstallDesktopShortcut))
            {
                var desktopShortcut = Bootstrapper.Engine.StringVariables[Variables.InstallDesktopShortcut];
                if(bool.TryParse(desktopShortcut, out result))
                {
                    return result;
                }
            }

            return result;
        }

        private bool GetDefaultProgramMenuShortcutState()
        {
            bool result = true;
            if (Bootstrapper.Engine.StringVariables.Contains(Variables.InstallProgramMenuShortcut))
            {
                var installProgramMenuShortcut = Bootstrapper.Engine.StringVariables[Variables.InstallProgramMenuShortcut];
                if (bool.TryParse(installProgramMenuShortcut, out result))
                {
                    return result;
                }
            }

            return result;
        }

        private bool GetDefaultRemovePlayerDataState()
        {
            bool result = false;
            if (Bootstrapper.Engine.StringVariables.Contains(Variables.RemovePlayerData))
            {
                var removePlayerData = Bootstrapper.Engine.StringVariables[Variables.RemovePlayerData];
                if (bool.TryParse(removePlayerData, out result))
                {
                    return result;
                }
            }

            return result;
        }

        public void SetMainViewModel(MainWindowViewModel viewModel)
        {
            this.MainWindowViewModel = viewModel;
        }

        public void SetWindowHandle(System.Windows.Window view)
        {
            this._hwnd = new WindowInteropHelper(view).Handle;
        }

        public void PlanAction(LaunchAction action)
        {
            LastPlannedAction = action;
            this.Bootstrapper.Engine.Plan(action);
        }

        public void ApplyAction()
        {
            this.Bootstrapper.Engine.Apply(this._hwnd);
        }

        public void LogMessage(string message)
        {
            this.Bootstrapper.Engine.Log(LogLevel.Standard, message);
        }

        public void ProcessError(Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ErrorEventArgs e)
        {
            string s = string.Format("{0} - {1}", e.ErrorCode, e.ErrorMessage);
            LogMessage(s);
            ErrorsList.Add(s);
            Debug.WriteLine(s);
        }

        public void InvokeFinalView()
        {
            Action action = delegate
            {
                MainWindowViewModel.SelectedView = new Views.FinalView();
            };
            Bootstrapper.Dispatcher.Invoke(action);
        }
        #endregion
    }
}
