using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DriveHUD.ViewModels;
using DriveHUD.Application.ViewModels;

namespace DriveHUD.Application.Social
{
    /// <summary>
    /// Interaction logic for FacebookOAuth.xaml
    /// </summary>
    public partial class FacebookOAuth : Window
    {
        private FacebookClient FBClient;

        private string _AppID = "908856699194655";
        private string _Scope = "email,publish_actions";

        public FacebookOAuth()
        {
            InitializeComponent();

            DeleteFacebookCookie();

            var destinationURL = String.Format("https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri=https://www.facebook.com/connect/login_success.html&scope={1}&type=user_agent&display=popup",
               _AppID, //client_id
               _Scope //scope
               );

            WBrowser.Navigate(new Uri(destinationURL).AbsoluteUri);
        }

        private void WBrowser_OnNavigated(object sender, NavigationEventArgs e)
        {
            var url = e.Uri.Fragment;
            if (url.Contains("access_token") && url.Contains("#"))
            {
                url = (new System.Text.RegularExpressions.Regex("#")).Replace(url, "?", 1);
                string _AccessToken = System.Web.HttpUtility.ParseQueryString(url).Get("access_token");

                if (!string.IsNullOrEmpty(_AccessToken))
                {
                    FBClient = new FacebookClient(_AccessToken);

                    this.Height = 195;
                    this.Width = 350;
                    WBrowser.Visibility = Visibility.Collapsed;
                    gridPostComment.Visibility = Visibility.Visible;
                }
            }

        }

        private void DeleteFacebookCookie()
        {
            //Set the current user cookie to have expired yesterday
            string cookie = String.Format("c_user=; expires={0:R}; path=/; domain=.facebook.com", DateTime.UtcNow.AddDays(-1).ToString("R"));
            System.Windows.Application.SetCookie(new Uri("https://www.facebook.com"), cookie);
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FBClient.Post("/me/feed", new { message = txtComment.Text });
                this.Close();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
