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
using System.Windows.Shapes;

using Twitterizer;

namespace DriveHUD.Application.Social
{
    /// <summary>
    /// Interaction logic for TwitterOAuth.xaml
    /// </summary>
    public partial class TwitterOAuth : Window
    {
        public TwitterOAuth()
        {
            InitializeComponent();

            gridAuth.Visibility = Visibility.Visible;
            gridTweet.Visibility = Visibility.Collapsed;

            this.Loaded += TwitterOAuth_Loaded;
        }

        void TwitterOAuth_Loaded(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            //browserControl.Refresh();

            Login();
        }

        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPincode.Text))
            {
                try
                {
                    TwitterStuff.pin = txtPincode.Text;
                    var response = OAuthUtility.GetAccessToken(TwitterStuff.consumerKey, TwitterStuff.consumerSecret, TwitterStuff.tokenResponse.Token, txtPincode.Text);
                    if (response != null)
                    {
                        TwitterStuff.tokenResponse2 = response;
                        TwitterStuff.screenName = TwitterStuff.tokenResponse2.ScreenName.ToString();
                        SetLocalTokens();

                        gridAuth.Visibility = Visibility.Collapsed;
                        gridTweet.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid pin");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Please enter valid pin");
                }

            }
        }

        private void Login()
        {
            TwitterStuff.tokenResponse = OAuthUtility.GetRequestToken(TwitterStuff.consumerKey, TwitterStuff.consumerSecret, TwitterStuff.callbackAddy);
            string target = "http://twitter.com/oauth/authenticate?oauth_token=" + TwitterStuff.tokenResponse.Token;
            try
            {
                browserControl.Navigate(new Uri(target));
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void SetLocalTokens()
        {
            TwitterStuff.tokens = new OAuthTokens();
            TwitterStuff.tokens.AccessToken = TwitterStuff.tokenResponse2.Token;
            TwitterStuff.tokens.AccessTokenSecret = TwitterStuff.tokenResponse2.TokenSecret;
            TwitterStuff.tokens.ConsumerKey = TwitterStuff.consumerKey;
            TwitterStuff.tokens.ConsumerSecret = TwitterStuff.consumerSecret;
        }

        private void btnTweet_Click(object sender, RoutedEventArgs e)
        {
            TwitterStuff ts = new TwitterStuff();
            if (!string.IsNullOrWhiteSpace(txtTweet.Text))
            {
                ts.tweetIt(txtTweet.Text);
                txtTweet.Clear();
                this.Close();
            }
        }

    }
}
