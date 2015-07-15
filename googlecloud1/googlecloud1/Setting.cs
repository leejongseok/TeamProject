using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Daimto.Drive.api;
using Google.Apis.Drive.v2;
using OneDrive;
using googlecloud1.login;
using Nemiro.OAuth.LoginForms;
using Nemiro.OAuth.Clients;
using Nemiro.OAuth;
namespace googlecloud1
{
    public partial class Setting : Form
    {
        public static List<DriveService> googlelist = new List<DriveService>();
        public static List<ODConnection> onedrivelist = new List<ODConnection>();
        public static List<Authentication.DropBoxLogin.TokenResult> dropbox = new List<Authentication.DropBoxLogin.TokenResult>();
        public Setting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != null)
            {
                DriveService service = Authentication.AuthenticateOauth("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com", "eyOFpG-LFIfp8ad3usTL81LG", textBox1.Text);
                if(service != null)
                {
                    googlelist.Add(service);
                    listBox1.Items.Add(service.Name);
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null)
            {
                ODConnection connect = await Authentication.OneDriveLogin.SignInToMicrosoftAccount(textBox1.Text, @"C:\Test");
                if(connect != null)
                {
                    onedrivelist.Add(connect);
                    listBox1.Items.Add("onedrive");
                }
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null)
            {
                Authentication.DropBoxLogin.TokenResult token = await Authentication.DropBoxLogin.SignInToDropBox(textBox1.Text, @"C:\Test");
                var parameter = new NameValueCollection
                {
                    {"access_token", token.AccessToken}
                };
                
                var result = OAuthUtility.ExecuteRequest("GET", "https://api.dropbox.com/1/account/info", parameter, null);
                var map = new ApiDataMapping();
                map.Add("uid", "UserId", typeof(string));
                map.Add("display_name", "DisplayName");
                map.Add("email", "Email");
                UserInfo user = new UserInfo((Dictionary<string, object>)result.Result, map);
                listBox1.Items.Add("DropBox - " + user.DisplayName);
                dropbox.Add(token);
            }
        }
    }
}
