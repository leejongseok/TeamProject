using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v2;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
namespace googlecloud1.login
{
    public partial class loginform : Form
    {
        public const string OAuthDesktopEndPoint = GoogleAuthConsts.AuthorizationUrl;
        public const string OAuthGoogleAAuthorizeService = GoogleAuthConsts.AuthorizationUrl;
        public const string OAuthGoogleATokenService = GoogleAuthConsts.TokenUrl;

        public string StartUrl { get; private set; }
        public string EndUrl { get; private set; }
        public string code { get; set; }
        public GoogleAuthorizationCodeFlow flow { get; set; }
        public loginform(string startutl, string endurl, string userid)
        {
           
            InitializeComponent();

            this.StartUrl = startutl;
            this.EndUrl = endurl;
            this.FormClosing += FormGoogleLoginAuth_FormClosing;
        }

        private void FormGoogleLoginAuth_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        private void loginform_Load(object sender, EventArgs e)
        {
           // this.webBrowser1.CanGoBackChanged += webBrowser_CanGoBackChanged;
            //this.webBrowser1.CanGoForwardChanged += webBrowser_CanGoBackChanged;
            FixUpNavigationButtons();
            System.Diagnostics.Debug.WriteLine("Navigating to start URL: " + this.StartUrl);
            this.webBrowser1.Navigate(this.StartUrl);
        }

        void webBrowser_CanGoBackChanged(object sender, EventArgs e)
        {
            FixUpNavigationButtons();
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Navigated to: " + webBrowser1.Url.AbsoluteUri.ToString());

            this.Text = webBrowser1.Document.Title;

            if (this.webBrowser1.Document.Title.StartsWith(EndUrl))
            {
                this.code = AuthResult();
                CloseWindow();
            }
        }

        private string AuthResult()
        {            
            string text = webBrowser1.Url.AbsoluteUri;
            string html;
                html = webBrowser1.Document.Title;
                int num1 = html.IndexOf("=");
                html = html.Substring(num1 + 1, (html.Length - num1) - 1);
            return html;
        }

        private void CloseWindow()
        {
            const int interval = 100;
            var t = new System.Threading.Timer(new System.Threading.TimerCallback((state) =>
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.BeginInvoke(new MethodInvoker(() => this.Close()));
            }), null, interval, System.Threading.Timeout.Infinite);
        }

        private void FixUpNavigationButtons()
        {

        }
        public static async Task<string> GetAuthenticationToken(string clientId, IEnumerable<string> scopes, string userid, IWin32Window owner = null)
        {
            string startUrl, completeUrl;
            GenerateUrlsForOAuth(clientId, scopes, out startUrl, out completeUrl);

            loginform authForm = new loginform(startUrl, completeUrl, userid);
            DialogResult result = await authForm.ShowDialogAsync(owner);
            if (DialogResult.OK == result)
            {
                return OnAuthComplete(authForm.code);
            }
            return null;
        }

        private static string OnAuthComplete(string p)
        {
            return p;
        }
        private Task<System.Windows.Forms.DialogResult> ShowDialogAsync(IWin32Window owner = null)
        {
            TaskCompletionSource<DialogResult> tcs = new TaskCompletionSource<DialogResult>();
            this.FormClosed += (s, e) =>
            {
                tcs.SetResult(this.DialogResult);
            };
            if (owner == null)
                this.ShowDialog();
            else
                this.Show(owner);

            return tcs.Task;
        }

        private static void GenerateUrlsForOAuth(string clientId, IEnumerable<string> scopes, out string startUrl, out string completeUrl)
        {
            Dictionary<string, string> urlParam = new Dictionary<string, string>();
            urlParam.Add("client_id", clientId);
            urlParam.Add("scope", GenerateScopeString(scopes));
            urlParam.Add("redirect_uri", GoogleAuthConsts.InstalledAppRedirectUri);
            urlParam.Add("response_type", "code");

            startUrl = BuildUriWithParameters(OAuthGoogleAAuthorizeService, urlParam);
            completeUrl = "Success";
        }

        private static string BuildUriWithParameters(string baseUri, Dictionary<string, string> queryStringParameters)
        {
            var sb = new StringBuilder();
            sb.Append(baseUri);
            sb.Append("?");
            foreach (var param in queryStringParameters)
            {
                if (sb[sb.Length - 1] != '?')
                    sb.Append("&");
                sb.Append(param.Key);
                sb.Append("=");
                sb.Append(Uri.EscapeDataString(param.Value));
            }
            return sb.ToString();
        }

        private static string GenerateScopeString(IEnumerable<string> scopes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var scope in scopes)
            {
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.Append(scope);
            }
            return sb.ToString();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
