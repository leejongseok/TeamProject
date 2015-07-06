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
        /// <summary>
        /// 폼이 로드되면서 발생되는 이벤트 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginform_Load(object sender, EventArgs e)
        {
           // this.webBrowser1.CanGoBackChanged += webBrowser_CanGoBackChanged;
            //this.webBrowser1.CanGoForwardChanged += webBrowser_CanGoBackChanged;
            FixUpNavigationButtons();
            System.Diagnostics.Debug.WriteLine("Navigating to start URL: " + this.StartUrl);
            // 처음 로그인 화면 url로 이동한다. 
            this.webBrowser1.Navigate(this.StartUrl);
        }

        void webBrowser_CanGoBackChanged(object sender, EventArgs e)
        {
            FixUpNavigationButtons();
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Navigated to: " + webBrowser1.Url.AbsoluteUri.ToString());
            // 브라우저의 html의 타이틀 값을 가져온다(이 타이틀에 code값이 적혀 있음)
            this.Text = webBrowser1.Document.Title;

            if (this.webBrowser1.Document.Title.StartsWith(EndUrl))
            {
                // 타이틀에 적혀 있는 값을 필요한 부분만 잘라서 code에 저장
                this.code = AuthResult();
                CloseWindow();
            }
        }
        /// <summary>
        /// 얻어온 html의 title에 적혀있는 code부분만 잘라내는 함수
        /// </summary>
        /// <returns></returns>
        private string AuthResult()
        {            
            string text = webBrowser1.Url.AbsoluteUri;
            string html;
                html = webBrowser1.Document.Title;
                int num1 = html.IndexOf("=");
                html = html.Substring(num1 + 1, (html.Length - num1) - 1);
            return html;
        }
        // 윈도우 창이 닫히면서 발생되는 이벤트 처리
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
        /// <summary>
        /// 얻어온 코드값을 반환해주는 함수 (뷰를 띄워주는 진입함수가 된다)
        /// </summary>
        /// <param name="clientId"> 클라이언트 id</param>
        /// <param name="scopes"> 접근 권한 주소가 담긴 list</param>
        /// <param name="userid"> 유저를 구분하기 위한 유저의 이름 (닉네임)</param>
        /// <param name="owner"> 부모 뷰의 핸들 (설정하지 않으면 기본값 null)</param>
        /// <returns></returns>
        public static async Task<string> GetAuthenticationToken(string clientId, IEnumerable<string> scopes, string userid, IWin32Window owner = null)
        {
            string startUrl, completeUrl;
            // 클라이언트 id와 접근 권한 리스트를 가지고 시작 url의 매개변수를 구성한다.
            GenerateUrlsForOAuth(clientId, scopes, out startUrl, out completeUrl);
            // 만들어온 시작 url을 이용하여 로그인 폼을 새로 만든다.
            loginform authForm = new loginform(startUrl, completeUrl, userid);
            // 로그인 폼 화면을 띄워준다.
            DialogResult result = await authForm.ShowDialogAsync(owner);
            // 화면이 정상적으로 종료 되었을떄 실행
            if (DialogResult.OK == result)
            {
                // 얻어온 코드값을 반환해준다.
                return OnAuthComplete(authForm.code);
            }
            return null;
        }

        private static string OnAuthComplete(string p)
        {
            return p;
        }
        // 현재 다이알로그 화면을 보여준다
        private Task<System.Windows.Forms.DialogResult> ShowDialogAsync(IWin32Window owner = null)
        {
            TaskCompletionSource<DialogResult> tcs = new TaskCompletionSource<DialogResult>();
            // 화면이 닫힌후 발생되는 이벤트 연결
            this.FormClosed += (s, e) =>
            {
                tcs.SetResult(this.DialogResult);
            };
            // 매개변수로 넘어옩 소유한 뷰의 핸들이 null일 경우 자기 자신을 띄워준다.
            if (owner == null)
                this.ShowDialog();
                // 아닐경우 매개변수로 넘어온 핸들의 뷰를 띄워준다.
            else
                this.Show(owner);

            return tcs.Task;
        }
        /// <summary>
        /// 서버 url로 넘겨줄 매개변수를 구성한다.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scopes"></param>
        /// <param name="startUrl"></param>
        /// <param name="completeUrl"></param>
        private static void GenerateUrlsForOAuth(string clientId, IEnumerable<string> scopes, out string startUrl, out string completeUrl)
        {
            // 파라미터 사전
            Dictionary<string, string> urlParam = new Dictionary<string, string>();
            urlParam.Add("client_id", clientId);
            urlParam.Add("scope", GenerateScopeString(scopes));
            //서버에서 되돌아오는 반환 uri 설정 인스톨 응용 프로그램은 urn:ietf:wg:oauth:2.0:oob
            urlParam.Add("redirect_uri", GoogleAuthConsts.InstalledAppRedirectUri);
            urlParam.Add("response_type", "code");

            startUrl = BuildUriWithParameters(OAuthGoogleAAuthorizeService, urlParam);
            completeUrl = "Success";
        }
        /// <summary>
        /// url 파라미터를 형식에 맞춰 url로 만들어준다
        /// </summary>
        /// <param name="baseUri"> 기본이 되는 url (https://accounts.google.com/o/oauth2/auth) </param>
        /// <param name="queryStringParameters"> baseuri뒤에 붙일 쿼리문 파라미터들</param>
        /// <returns></returns>
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
        /// <summary>
        /// 전달되어진 접근 권한 주소들이 담긴 list를 하나의 string으로 만들어준다.
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns> string을 반환</returns>
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
