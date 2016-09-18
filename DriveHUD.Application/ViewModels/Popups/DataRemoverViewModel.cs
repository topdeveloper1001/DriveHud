using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class DataRemoverViewModel : BaseViewModel
    {
        private static string scriptPath = ConfigurationManager.AppSettings["cleanDbScriptPath"];

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
            var dataService = ServiceLocator.Current.GetInstance<IDataService>();
            dataService.RemoveAppData();
            RemoveDBData();
        }

        private void RemoveDBData()
        {
            NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DriveHudDB"].ConnectionString);
            try
            {
                CleanDb(conn);
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code == "3D000")
                {
                    conn.Close();
                    LogProvider.Log.Debug("DB doesn't exist");
                }
                else
                {
                    LogProvider.Log.Error(this, String.Format("Error : {0}", ex.Message));
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, String.Format("Error : {0}", ex.Message));
            }
            finally
            {
                Cancel(null);
            }
        }

        private void CleanDb(NpgsqlConnection conn)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptPath);
            string script = File.ReadAllText(path);
            LogProvider.Log.Debug("Removing DB Data");
            try
            {
                NpgsqlCommand command =
                                new NpgsqlCommand(script, conn);
                conn.Open();
                command.ExecuteNonQuery();
                LogProvider.Log.Debug("DB Data Removed");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, String.Format("Error : {0}", ex.Message));
            }
        }
    }
}
