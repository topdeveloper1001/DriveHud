//-----------------------------------------------------------------------
// <copyright file="DataRemoverViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using System;
using System.IO;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class DataRemoverViewModel : BaseViewModel
    {
        public ICommand UninstallCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public DataRemoverViewModel()
        {
            UninstallCommand = new RelayCommand(Uninstall);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Cancel(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Uninstall(object obj)
        {
            LogProvider.Log.Info(this, "Uninstalling DriveHUD.");

            var dataService = ServiceLocator.Current.GetInstance<IDataService>();
            dataService.RemoveAppData();

            RemoveSqliteDatabase();

            LogProvider.Log.Info(this, "DriveHUD has been uninstalled.");
        }

        private void RemoveSqliteDatabase()
        {
            LogProvider.Log.Debug(this, "Removing DB Data");

            try
            {
                var dbFile = StringFormatter.GetSQLiteDbFilePath();

                if (File.Exists(dbFile))
                {
                    File.Delete(dbFile);
                    LogProvider.Log.Debug("Database has been deleted.");
                }
                else
                {
                    LogProvider.Log.Debug("Database file has not been found.");
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Database has not been removed.", e);
            }
        }
    }
}