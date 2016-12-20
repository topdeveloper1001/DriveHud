using System;
using System.Windows.Input;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Npgsql;
using Model.Interfaces;
using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using DriveHUD.Common.Log;
using Model;
using System.Linq;
using DriveHUD.Common.Resources;
using Prism.Interactivity.InteractionRequest;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using System.Windows.Threading;
using DriveHUD.Application.MigrationService;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// The Configure Postgresql Server view model.
    /// </summary>
    public class ConfigurePostgresqlServerViewModel : BaseViewModel
    {
        private const int COMMAND_TIMEOUT = 60;
        private IDataService dataService;
        private readonly string TargetDatabaseVersion;

        private SettingsModel settings;

        private string _server;

        private string _port;

        private string _user;

        private string _password;

        private string _database;

        private bool _isDatabaseVersionValid;

        private static string scriptPath = ConfigurationManager.AppSettings["scriptPath"];

        private static NpgsqlConnectionStringBuilder builder;

        private SynchronizationContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurePostgresqlServerViewModel"/> class.
        /// </summary>
        internal ConfigurePostgresqlServerViewModel()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            Progress = new ProgressViewModel();
            context = SynchronizationContext.Current;
            TargetDatabaseVersion = CommonResourceManager.Instance.GetResourceString("SystemSettings_TargetDatabaseVersion");
            PopupRequest = new InteractionRequest<PopupBaseNotification>();

            Server = settings.DatabaseSettings.Server;
            Port = settings.DatabaseSettings.Port;
            User = settings.DatabaseSettings.User;
            Password = settings.DatabaseSettings.Password;
            _database = settings.DatabaseSettings.Database;
            ConnectCommand = new RelayCommand(Connect);
            CancelCommand = new RelayCommand(Cancel);
        }

        public InteractionRequest<PopupBaseNotification> PopupRequest { get; set; }

        public Dispatcher Dispatcher { get; set; }

        public IProgressViewModel Progress { get; private set; }

        public static bool IsConnected { get; set; }

        public string Server
        {
            get { return _server; }
            set
            {
                if (Equals(value, _server)) return;

                _server = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                if (Equals(value, _port)) return;

                _port = value;
                OnPropertyChanged();
            }
        }

        public string User
        {
            get { return _user; }
            set
            {
                if (Equals(value, _user)) return;

                _user = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (Equals(value, _password)) return;

                _password = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the connect command.
        /// </summary>
        public ICommand ConnectCommand { get; set; }

        /// <summary>
        /// Gets or sets the cancle command.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        public Action CloseAction { get; set; }

        public Action AfterConnectAction { get; set; }

        /// <summary>
        /// The save.
        /// </summary>
        internal async void Connect()
        {
            Progress.IsActive = true;
            _isDatabaseVersionValid = true;

            LogProvider.Log.Debug("Initializing database connection");
            try
            {
                LogProvider.Log.Debug("First attempt to connect...");
                await Task.Run(() => { DoConnect(Server, Port, _database, User, Password); });

                if (!IsConnected && _isDatabaseVersionValid)
                {
                    LogProvider.Log.Debug("Second attempt to connect...");
                    var hm2Settings = HM2DatabaseSettingsProvider.GetHM2DatabaseSettings();
                    await Task.Run(() => { DoConnect(hm2Settings.Server, hm2Settings.Port, _database, hm2Settings.UserName, hm2Settings.Password); });
                }

                if (!IsConnected && _isDatabaseVersionValid)
                {
                    //load pt4 settings
                    LogProvider.Log.Debug("Third attempt to connect...");
                    var pt4Settings = PT4DatabaseSettingsProvider.GetPT4DatabaseSettings();
                    await Task.Run(() => { DoConnect(pt4Settings.Server, pt4Settings.Port, _database, pt4Settings.UserName, pt4Settings.Password); });
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                LogProvider.Log.Error(ex);
            }
            finally
            {
                context.Post((x) =>
                {
                    if (AfterConnectAction != null)
                    {
                        AfterConnectAction.Invoke();
                    }
                }, null);
            }

            if (IsConnected)
            {
                LogProvider.Log.Debug("Database Connected");
            }
            else
            {
                LogProvider.Log.Debug("Databse Connection Failed");
            }

            Progress.IsActive = false;
        }

        private void DoConnect(string server, string port, string database, string user, string password)
        {
            NpgsqlConnection conn = null;

            string connectionString = StringFormatter.GetConnectionString(server, port, database, user, password);

            try
            {
                //  LogProvider.Log.Debug(string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", Server, Port, database, User, Password));

                conn = new NpgsqlConnection(connectionString);                                

                LogProvider.Log.Debug("Connecting to a PostgreSQL database");
                builder = new NpgsqlConnectionStringBuilder(conn.ConnectionString);               

                conn.Open();
                var version = DoWork(conn);
                conn.Close();

                if (_isDatabaseVersionValid)
                {
                    UpdateSettingsIfNeeded(server, port, database, user, password, version);
                }
            }
            catch (PostgresException ex)
            {
                conn.Close();
                if (ex.SqlState.Equals("3D000"))
                {
                    LogProvider.Log.Debug("Database doesn't exist");
                    CreateDatabase(server, port, database, user, password);
                }
                else
                {
                    LogProvider.Log.Error(ex.Message);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        message = ex.InnerException.InnerException.Message;
                    }
                }

                context.Post((x) =>
                {
                    MessageBox.Show(string.Format("Exception message = {0};", message));
                }, null);
                IsConnected = false;

                LogProvider.Log.Error(ex);
            }
        }

        private void CreateDatabase(string server, string port, string database, string user, string password)
        {
            string connectionString = StringFormatter.GetConnectionString(server, port, database, user, password);

            NpgsqlConnection conn;
            LogProvider.Log.Debug("Creating database");
            conn =
                new NpgsqlConnection(
                    string.Format(
                        "Server={0};Port={1};User Id={2};Password={3};CommandTimeout={4};",
                        builder.Host,
                        builder.Port,
                        builder.Username,
                        builder.Password, COMMAND_TIMEOUT));
            try
            {
                CreateTable(conn);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, ex);
                return;
            }
            finally
            {
                conn.Close();
            }

            conn = new NpgsqlConnection(connectionString);
            conn.Open();
            var version = DoWork(conn);
            conn.Close();

            if (_isDatabaseVersionValid)
            {
                UpdateSettingsIfNeeded(server, port, database, user, password, version);
            }

            dataService.Purge();
            LogProvider.Log.Debug("Database Created");
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        private void Cancel()
        {
            if (CloseAction != null)
            {
                CloseAction.Invoke();
            }
        }

        private void CreateTable(NpgsqlConnection conn)
        {
            LogProvider.Log.Debug("Creating table");
            NpgsqlCommand command =
                        new NpgsqlCommand(String.Format("CREATE DATABASE {0} WITH OWNER = \"{1}\" " +
                                                        "ENCODING = 'UTF8' " +
                                                        "CONNECTION LIMIT = -1;", builder.Database, builder.Username), conn);
            conn.Open();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns database version text
        /// </summary>
        /// <param name="conn"></param>
        /// <returns>Database version</returns>
        private string DoWork(NpgsqlConnection conn)
        {
            NpgsqlCommand command = new NpgsqlCommand("select version();", conn);
            var result = command.ExecuteReader();

            // Get databases version
            string fullVersionText = string.Empty;
            while (result.Read())
            {
                if (result.FieldCount > 0)
                {
                    LogProvider.Log.Info($"Database Version: {result[0]}");

                    fullVersionText = result[0].ToString();
                }
            }

            if (IsDatabaseVersionValid(fullVersionText))
            {
                LogProvider.Log.Debug("Database Exists");

                RunMigrator(conn.ConnectionString);

                _isDatabaseVersionValid = true;
                IsConnected = true;
            }
            else
            {
                _isDatabaseVersionValid = false;
            }

            return fullVersionText;
        }

        private bool IsDatabaseVersionValid(string fullVersionText)
        {
            var settingsVersion = settings.DatabaseSettings.LastDatabaseVersion;
            if (!string.IsNullOrWhiteSpace(settingsVersion))
            {
                if (settingsVersion == fullVersionText)
                {
                    return true;
                }

                if (!ProceedDatabaseDoesNotMatch(fullVersionText))
                {
                    return false;
                }
            }

            if (!ProcessDatabaseNotRecommended(fullVersionText))
            {
                return false;
            }

            return true;
        }

        private bool ProcessDatabaseNotRecommended(string versionText)
        {
            string versionNumber = string.Empty;
            var versionarray = versionText.ToString().Split(' ');
            if (versionarray.Count() > 1)
            {
                versionNumber = versionarray[1].Trim(new char[] { ' ', ',' });
            }
            else
            {
                versionNumber = versionarray.FirstOrDefault();
            }

            if (versionNumber != TargetDatabaseVersion)
            {
                string contentString = $"Current database version is {versionNumber}."
                    + Environment.NewLine
                    + $"We strongly recommend you to use PostgreSQL {TargetDatabaseVersion} that goes with official installation package in order to guarantee correct work of the DriveHUD software."
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Do you still want to proceed with this database?";

                return GetNotificationResult(contentString);
            }

            return true;
        }

        private bool ProceedDatabaseDoesNotMatch(string versionText)
        {
            string contentString = $"Database version does not match with version you were using before: '{versionText}'"
                 + Environment.NewLine + "If you decide to proceed with current database you will lose some of your data."
                 + Environment.NewLine + Environment.NewLine + "Do you still want to proceed with this database?";

            return GetNotificationResult(contentString);
        }

        private bool GetNotificationResult(string contentString)
        {
            bool confirmed = false;

            var notification = new PopupBaseNotification()
            {
                Title = "Database does not match",
                CancelButtonCaption = "Cancel",
                ConfirmButtonCaption = "Proceed",
                Content = contentString,
                IsDisplayH1Text = false
            };

            Action action = () =>
            {
                PopupRequest.Raise(notification,
                      confirmation =>
                      {
                          if (confirmation.Confirmed)
                          {
                              confirmed = true;
                          }
                          else
                          {
                              confirmed = false;
                          }
                      });
            };

            if (this.Dispatcher != null)
            {
                this.Dispatcher.Invoke(action);
            }
            else
            {
                App.Current.Dispatcher.Invoke(action);
            }

            return confirmed;
        }

        private void UpdateSettingsIfNeeded(string server, string port, string database, string user, string password, string databaseVersion)
        {
            if (settings.DatabaseSettings.Server != server
                || settings.DatabaseSettings.Port != port
                || settings.DatabaseSettings.User != user
                || settings.DatabaseSettings.Password != password
                || settings.DatabaseSettings.Database != database
                || (!string.IsNullOrWhiteSpace(databaseVersion) && (settings.DatabaseSettings.LastDatabaseVersion != databaseVersion)))
            {
                settings.DatabaseSettings.Server = server;
                settings.DatabaseSettings.Port = port;
                settings.DatabaseSettings.User = user;
                settings.DatabaseSettings.Password = password;
                settings.DatabaseSettings.Database = database;
                settings.DatabaseSettings.LastDatabaseVersion = databaseVersion;
                ServiceLocator.Current.GetInstance<ISettingsService>().SaveSettings(settings);
            }
        }

        private void RunMigrator(string connectionString)
        {
            LogProvider.Log.Debug("Run Migration Service");

            ServiceLocator.Current.GetInstance<IMigrationService>().MigrateToLatest(connectionString);

            LogProvider.Log.Debug("DH is up to date");
        }
    }
}