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
                string url = string.Format("{0}?&scope=email%20profile%20" + DriveService.Scope.Drive + "&redirect_uri={1}&response_type={2}&client_id={3}&approval_prompt=force&access_type=online&state=security_token&include_granted_scopes=true",
                    "https://accounts.google.com/o/oauth2/auth",
                    GoogleAuthConsts.InstalledAppRedirectUri,
                    "code",
                    "892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com");
                this.webBrowser1.Navigate(url);
                //DriveService drive = Authentication.AuthenticateOauth("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com", "eyOFpG-LFIfp8ad3usTL81LG", "baba");
            }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            HtmlElement hel;
            string text = webBrowser1.Url.AbsoluteUri;

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
