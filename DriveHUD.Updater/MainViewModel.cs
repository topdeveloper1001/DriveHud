//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DriveHUD.Updater.Core;
using DriveHUD.Updater.Properties;

namespace DriveHUD.Updater
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
        }

        #region Properties

        private int currentProgress;
        public int CurrentProgress
        {
            get
            {
                return currentProgress;
            }
            private set
            {
                if (currentProgress == value)
                    return;

                currentProgress = value;
                RaisePropertyChanged(() => CurrentProgress);
            }
        }

        private string statusMessage;
        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                if (statusMessage == value)
                    return;

                statusMessage = value;
                RaisePropertyChanged(() => StatusMessage);
            }
        }

        public string Caption
        {
            get
            {
                return String.Format("{0} v.{1}", Resources.ApplicationCaption, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
        }

        private string currentFile;
        public string CurrentFile
        {
            get
            {
                return currentFile;
            }
            set
            {

                if (currentFile == value)
                    return;

                currentFile = value;
                RaisePropertyChanged(() => CurrentFile);
            }
        }

        #endregion

        public async virtual void InitializeAsync()
        {
            var appLoader = new HttpApplicationInfoLoader();

            try
            {
                if (IsAppRunning())
                {
                    await Task.Delay(5000);

                    if (IsAppRunning())
                    {
                        StatusMessage = Resources.Error_ManyApps;
                        return;
                    }
                }

                using (var appUpdater = new AppUpdater(appLoader))
                {
                    appUpdater.OperationChanged += OnOperationChanged;
                    appUpdater.ProgressChanged += OnProgressChanged;
                    appUpdater.UnzippingFileChanged += OnUnzippingFileChanged;
                    appUpdater.CopyingFileChanged += OnCopyingFileChanged;

                    await appUpdater.InitializeAsync();

                    if (!appUpdater.CheckIsUpdateAvailable(DriveHUDUpdaterPaths.MainApplicationGuid, DriveHUDUpdaterPaths.MainApplicationProccess))
                    {
                        StatusMessage = Resources.Error_NoUpdate;
                        return;
                    }

                    var unpackedDirectory = await appUpdater.UpdateApplicationAsync(DriveHUDUpdaterPaths.MainApplicationGuid, DriveHUDUpdaterPaths.MainApplicationProccess, true);                    

                    StatusMessage = Resources.Message_Operation_Completed;

                    var installerPath = Path.Combine(unpackedDirectory.FullName, DriveHUDUpdaterPaths.MainApplicationInstaller);

                    RunApplication(installerPath);
                }
            }
            catch (UpdaterException ex)
            {
                HandleUpdaterError(ex);
            }
            catch (FileNotFoundException)
            {
                StatusMessage = Resources.Error_AssemblyNotFound;
            }
            catch
            {
                StatusMessage = Resources.Error_Unexpected;
            }
        }

        private bool IsAppRunning()
        {
            Process[] appProcesses = Process.GetProcessesByName(DriveHUDUpdaterPaths.MainApplicationProccess);

            FileInfo appFile = new FileInfo(DriveHUDUpdaterPaths.MainApplicationProccess);

            foreach (var proc in appProcesses)
            {
                if (appFile.FullName == proc.MainModule.FileName)
                    return true;
            }

            return false;
        }

        private void RunApplication(string applicationPath)
        {
            SingleInstance<App>.Cleanup();

            try
            {
                if (File.Exists(applicationPath))
                {
                    var info = new ProcessStartInfo(applicationPath);

                    var appProcess = new Process()
                    {
                        StartInfo = info
                    };

                    appProcess.Start();
                }
            }
            finally
            {
                Application.Current.Shutdown();
            }
        }

        private void HandleUpdaterError(UpdaterException ex)
        {
            switch (ex.ErrorCode)
            {
                case UpdaterError.AppInfoLoadingFailed:
                    StatusMessage = Resources.Error_AppInfoLoadingFailed;
                    break;

                case UpdaterError.ComputingHashingFailed:
                    StatusMessage = Resources.Error_ComputingHashingFailed;
                    break;

                case UpdaterError.DeserializingFailed:
                    StatusMessage = Resources.Error_DeserializingFailed;
                    break;

                case UpdaterError.DownloadingFailed:
                    StatusMessage = Resources.Error_DownloadingFailed;
                    break;

                case UpdaterError.InvalidAppInfoPath:
                    StatusMessage = Resources.Error_InvalidAppInfoPath;
                    break;

                case UpdaterError.InvalidHash:
                    StatusMessage = Resources.Error_InvalidHash;
                    break;

                case UpdaterError.InvalidXmlFormat:
                    StatusMessage = Resources.Error_InvalidXmlFormat;
                    break;

                case UpdaterError.UnzippingFailed:
                    StatusMessage = Resources.Error_UnzippingFailed;
                    break;

                case UpdaterError.VerifyingFailed:
                    StatusMessage = Resources.Error_VerifyingFailed;
                    break;

                case UpdaterError.CopyingFailed:
                    StatusMessage = Resources.Error_VerifyingFailed;
                    break;

                default:
                    StatusMessage = Resources.Error_Unexpected;
                    break;
            }
        }

        private void OnUnzippingFileChanged(object sender, UpdateProcessingFileChangedEventArgs e)
        {
            StatusMessage = String.Format("{0} {1}", Resources.Message_Operation_Unzipping, e.FileName);
        }

        private void OnCopyingFileChanged(object sender, UpdateProcessingFileChangedEventArgs e)
        {
            StatusMessage = String.Format("{0} {1}", Resources.Message_Operation_Copying, e.FileName);
        }

        private void OnProgressChanged(object sender, UpdaterProgressChangedEventArgs e)
        {
            CurrentProgress = e.Progress;
        }

        private void OnOperationChanged(object sender, UpdaterOperationChangedEventArgs e)
        {
            CurrentProgress = 0;

            switch (e.OperationStatus)
            {
                case OperationStatus.Getting:
                    StatusMessage = Resources.Message_Operation_Getting;
                    break;
                case OperationStatus.Computing:
                    StatusMessage = Resources.Message_Operation_Computing;
                    break;
                case OperationStatus.Downloading:
                    StatusMessage = Resources.Message_Operation_Downloading;
                    break;
                case OperationStatus.Unzipping:
                    StatusMessage = Resources.Message_Operation_Unzipping;
                    break;
                case OperationStatus.Copying:
                    StatusMessage = Resources.Message_Operation_Copying;
                    break;
                default:
                    StatusMessage = String.Empty;
                    break;
            }
        }
    }
}