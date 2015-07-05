﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v2;
using Google.Apis.Auth.OAuth2;
using Daimto.Drive.api;
namespace MicrosoftAccount.WindowsForms
{
    public partial class FormMicrosoftAccountAuth : Form
    {
        public const string OAuthDesktopEndPoint = GoogleAuthConsts.AuthorizationUrl;
        public const string OAuthMSAAuthorizeService = GoogleAuthConsts.AuthorizationUrl;
        public const string OAuthMSATokenService = GoogleAuthConsts.TokenUrl;

        #region Properties
        public string StartUrl { get; private set; }
        public string EndUrl { get; private set; }
        public AuthResult AuthResult { get; private set; }
        public OAuthFlow AuthFlow { get; private set; }

        #endregion

        #region Constructor
        public FormMicrosoftAccountAuth(string startUrl, string endUrl, OAuthFlow flow = OAuthFlow.AuthorizationCodeGrant)
        {
            InitializeComponent();

            this.StartUrl = startUrl;
            this.EndUrl = endUrl;
            this.AuthFlow = flow;
            this.FormClosing += FormMicrosoftAccountAuth_FormClosing;
        }
        #endregion

        #region Private Methods
        private void FormMicrosoftAccountAuth_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormMicrosoftAccountAuth_Load(object sender, EventArgs e)
        {
            this.webBrowser.Navigated += webBrowser_Navigated;
            System.Diagnostics.Debug.WriteLine("Navigating to start URL: " + this.StartUrl);
            this.webBrowser.Navigate(this.StartUrl);
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Navigated to: " + webBrowser.Url.AbsoluteUri.ToString());

            this.Text = webBrowser.DocumentTitle;
            if (this.webBrowser.Url.AbsoluteUri.StartsWith(EndUrl))
            {
                this.AuthResult = new AuthResult(this.webBrowser.Url, this.AuthFlow);
                CloseWindow();
            }
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
        #endregion


        public Task<DialogResult> ShowDialogAsync(IWin32Window owner = null)
        {
            TaskCompletionSource<DialogResult> tcs = new TaskCompletionSource<DialogResult>();
            this.FormClosed += (s, e) =>
            {
                tcs.SetResult(this.DialogResult);
            };
            if (owner == null)
                this.Show();
            else
                this.Show(owner);

            return tcs.Task;
        }

        #region Static Methods

        private static string GenerateScopeString(string[] scopes)
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

        public static void GenerateUrlsForOAuth(string clientId, string[] scopes, OAuthFlow flow, out string startUrl, out string completeUrl, string redirectUrl = OAuthDesktopEndPoint)
        {
            Dictionary<string, string> urlParam = new Dictionary<string, string>();
            urlParam.Add("client_id", clientId);
            urlParam.Add("scope", GenerateScopeString(scopes));
            urlParam.Add("redirect_uri", redirectUrl);
            urlParam.Add("display", "popup");

            switch (flow)
            {
                case OAuthFlow.ImplicitGrant:
                    urlParam.Add("response_type", "token");
                    break;
                case OAuthFlow.AuthorizationCodeGrant:
                    urlParam.Add("response_type", "code");
                    break;
                default:
                    throw new NotSupportedException("flow value not supported");
            }

            startUrl = BuildUriWithParameters(OAuthMSAAuthorizeService, urlParam);
            completeUrl = redirectUrl;
        }

        public static async Task<string> GetAuthenticationToken(string clientId, string[] scopes, OAuthFlow flow, IWin32Window owner = null)
        {
            string startUrl, completeUrl;
            GenerateUrlsForOAuth(clientId, scopes, flow, out startUrl, out completeUrl);

            FormMicrosoftAccountAuth authForm = new FormMicrosoftAccountAuth(startUrl, completeUrl, flow);
            DialogResult result = await authForm.ShowDialogAsync(owner);
            if (DialogResult.OK == result)
            {
                return OnAuthComplete(authForm.AuthResult);
            }
            return null;
        }

        private static string OnAuthComplete(AuthResult authResult)
        {
            switch (authResult.AuthFlow)
            {
                case OAuthFlow.ImplicitGrant:
                    return authResult.AccessToken;
                case OAuthFlow.AuthorizationCodeGrant:
                    return authResult.AuthorizeCode;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported AuthFlow value");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
        }
        #endregion

        public static async Task<string> GetUserId(string authToken)
        {
            if (string.IsNullOrEmpty(authToken)) throw new ArgumentNullException("authToken");

            string requestUrl = "https://apis.live.net/v5.0/me?access_token=" + System.Uri.EscapeUriString(authToken);
            HttpWebRequest request = HttpWebRequest.CreateHttp(requestUrl);

            HttpWebResponse response;
            try
            {

                WebResponse wr = await request.GetResponseAsync();
                response = wr as HttpWebResponse;
            }
            catch (WebException webex)
            {
                response = webex.Response as HttpWebResponse;
            }

            if (null == response) return null;

            UserObject user = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream);
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserObject>(await reader.ReadToEndAsync());
            }

            response.Dispose();

            if (null != user)
                return user.Id;
            else
                return null;
        }


        internal class UserObject
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

    }



    public enum OAuthFlow
    {
        ImplicitGrant,
        AuthorizationCodeGrant
    }


}