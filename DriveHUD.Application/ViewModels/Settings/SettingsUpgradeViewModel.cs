using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Settings;
using DriveHUD.Common.Resources;
using System.Windows.Input;
using DriveHUD.Common.Infrastructure.Base;
using System.Diagnostics;
using DriveHUD.Common.Utils;
using DriveHUD.Application.Licensing;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Common.Log;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsUpgradeViewModel : SettingsViewModel<ISettingsBase>
    {
        internal SettingsUpgradeViewModel(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            AddActivationCommand = new RelayCommand(AddActivation);
            RenewLicenseCommand = new RelayCommand(RenewLicense);
        }

        #region Properties
        public string CurrentVersion
        {
            get
            {
                return App.Version.ToString();
            }
        }

        public string AvailableVersion
        {
            get
            {
                return App.Version.ToString();
            }
        }

        #endregion

        #region ICommand

        public ICommand AddActivationCommand { get; set; }
        public ICommand RenewLicenseCommand { get; set; }

        #endregion

        #region ICommand implementation

        private void AddActivation(object obj)
        {
            Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("SystemSettings_AddActivationLink"));
        }

        private void RenewLicense(object obj)
        {
            try
            {
                var license = ServiceLocator.Current.GetInstance<ILicenseService>();
                var licenseCode = license.LicenseInfos.Where(x => !string.IsNullOrEmpty(x.Serial) && !x.IsTrial).OrderBy(x => x.ExpiryDate).FirstOrDefault()?.Serial ?? string.Empty;
                Process.Start(BrowserHelper.GetDefaultBrowserPath(), String.Format(CommonResourceManager.Instance.GetResourceString("SystemSettings_RenewLicenseLink"), licenseCode));
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
        }

        #endregion
    }
}
