using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using DriveHUD.Common.Log;

namespace DriveHUD.PlayerXRay.BusinessHelper
{
    public class WcfHelper
    {
        public static bool CheckConnection()
        {
            try
            {
                string url = Resources.WcfUrl + Resources.IsAlivePath;
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET"; 

                WebResponse response = request.GetResponse();
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    string json = stream.ReadToEnd();
                    bool result = (bool)JsonConvert.DeserializeObject(json, typeof(bool));
                }

                return true;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(NoteManager), "Connection check failed", ex);                
                return false;
            }
        }
    }
}
