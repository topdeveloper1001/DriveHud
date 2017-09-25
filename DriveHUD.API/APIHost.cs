using DriveHUD.Common.Log;
using System;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace DriveHUD.API
{
    public class APIHost : IAPIHost
    {
        private WebServiceHost _apiServiceHost;
        private bool isRunning;

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        public void StartAPIService()
        {
            InitializeService();
            isRunning = true;
        }

        public void CloseAPIService()
        {
            try
            {
                _apiServiceHost.Faulted -= APIServiceHost_Faulted;
                _apiServiceHost.Closing -= APIServiceHost_Closing;
                _apiServiceHost.Close();
            }
            catch
            {
                _apiServiceHost.Abort();
            }

            isRunning = false;
        }

        private void InitializeService()
        {
            try
            {
                _apiServiceHost = new WebServiceHost(typeof(APIService));
                _apiServiceHost.Closing += new EventHandler(APIServiceHost_Closing);
                _apiServiceHost.Faulted += new EventHandler(APIServiceHost_Faulted);
                _apiServiceHost.Open();

                LogProvider.Log.Info("API Service Started.");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, ex);
            }
        }

        private void APIServiceHost_Faulted(object sender, EventArgs e)
        {
            LogProvider.Log.Error(this, "API Service Faulted");
        }

        private void APIServiceHost_Closing(object sender, EventArgs e)
        {
            LogProvider.Log.Error(this, "API Service Closed");
        }
    }
}
