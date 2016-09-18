using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Twitterizer;

namespace DriveHUD.Application.Social
{
    class TwitterStuff
    {
        public static string consumerKey = "x1wrLxfepnb4oOvjgPoVJxIys";
        public static string consumerSecret = "K0EF3oRMd8qiAad4sWvD3Gi5GSy75U7ANQJS8m2usP3wlNT1ec";
        public static string pin = "";
        public static string callbackAddy = "oob";
        public static string screenName;

        public static OAuthTokenResponse tokenResponse;
        public static OAuthTokenResponse tokenResponse2;

        public static OAuthTokens tokens;

        public bool tweetIt(string status)
        {
            try
            {
                var userOptions = new StatusUpdateOptions();
                userOptions.APIBaseAddress = "https://api.twitter.com/1.1/"; // <-- needed for API 1.1
                userOptions.UseSSL = true; // <-- needed for API 1.1

                var result = TwitterStatus.Update(tokens, status, userOptions);
                MessageBox.Show("Status updated successfuly");
                return true;
            }
            catch (TwitterizerException te)
            {
                MessageBox.Show(te.Message);
                return false;
            }
        }
        
    }
}
