using System;
using System.Collections.Generic;
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
namespace googlecloud1
{
    public partial class Setting : Form
    {
        public static List<DriveService> googlelist = new List<DriveService>();
        public static List<ODConnection> onedrivelist = new List<ODConnection>();
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
                ODConnection connect = await OneDriveLogin.SignInToMicrosoftAccount(textBox1.Text, @"C:\Test");
                if(connect != null)
                {
                    onedrivelist.Add(connect);
                    listBox1.Items.Add("onedrive");
                }
            }
        }
    }
}
