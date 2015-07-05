using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using MicrosoftAccount.WindowsForms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Daimto.Drive.api;
using Google.Apis.Requests;
using Google.Apis.Auth.OAuth2.Flows;
using System.Net.Http;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Requests;
namespace login11
{
    public partial class Form1 : Form
    {
        AuthResult au;
     
        public Form1()
        {
            InitializeComponent();
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
            {              
          // 4/7iao82zEKPM5u1AMSX4UTF042MvyVdONrNp4DFhtq9Q
              // 4/ZxjOgQ_VNzRtiEgPkfSjbx2gbxbyKmPu5o2D6JvQVx0
            //IAuthorizationCodeFlow app;
            //    AuthorizationCodeInstalledApp app = new AuthorizationCodeInstalledApp(IAuthorizationCodeFlow)
            //    Task<UserCredential> user = app.AuthorizeAsync("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com", CancellationToken.None);
                ServiceAccountCredential.Initializer it = new ServiceAccountCredential.Initializer("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com");

                string url = string.Format("{0}?&scope=email%20profile%20" + DriveService.Scope.Drive + "&redirect_uri={1}&response_type={2}&client_id={3}&approval_prompt=force&access_type=online&include_granted_scopes=true",
                    "https://accounts.google.com/o/oauth2/auth",
                    GoogleAuthConsts.InstalledAppRedirectUri,
                    "code",
                    "892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com");
                this.webBrowser1.Navigate(url);
            }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            HtmlElement hel;
            string text = webBrowser1.Url.AbsoluteUri;
            string html;
            try
            {
                html = webBrowser1.Document.Title;
                int num1 = html.IndexOf(" ");
                int num2 = html.IndexOf("&");
                html = html.Substring(num1 + 1, (num2 - num1) - 1);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GoogleAuthConsts.TokenUrl);
                DriveService.Initializer ini = new DriveService.Initializer();
                AuthorizationCodeTokenRequest token = new AuthorizationCodeTokenRequest();
                
                request.Headers.Host = "accounts.google.com";
                request.Headers.Add("code", html);
                request.Headers.Add("client_id", "892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com");
                request.Headers.Add("client_secret", "eyOFpG-LFIfp8ad3usTL81LG");
                //BearerToken.QueryParameterAccessMethod token = new BearerToken.QueryParameterAccessMethod();
                //string tok = token.GetAccessToken(request);
                //string url = string.Format("{0}?{1}&client_id={2}&client_secret={3}&redirect_uri={4}&grant_type=authorization_code",
                //    "https://accounts.google.com/o/oauth2/v3/token",
                //    html,
                //    "892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com",
                //    "eyOFpG-LFIfp8ad3usTL81LG",
                //    "urn:ietf:wg:oauth:2.0:oob");
                //this.webBrowser1.Navigate(url);
            }
            catch
            {
            }
            
            if (string.IsNullOrEmpty(text) == true)
            {
                return;
            }

            if (text.IndexOf("http://schemas.google.com/AddActivity") == -1)
            {
                return;
            }

            string[] fbAccess = text.Split('?');
            if (fbAccess.Length < 2)
            {
                return;
            }

            string access_part = fbAccess[1];
            string[] token_part = access_part.Split('&');
            if (token_part.Length < 2)
            {
                return;
            }

            string[] access_token_tuple = token_part[0].Split('=');
            if (access_token_tuple.Length < 2)
            {
                return;
            }

            string access_token = access_token_tuple[1];
            if (string.IsNullOrEmpty(access_token) == false)
            {
                this.webBrowser1.Visible = false;
            }
        }
    }
}
