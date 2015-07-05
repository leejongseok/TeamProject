using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MicrosoftAccount.WindowsForms;
using Google.Apis.Auth.OAuth2;
namespace googlecloud
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMicrosoftAccountAuth(GoogleAuthConsts.AuthorizationUrl, GoogleAuthConsts.InstalledAppRedirectUri));
        }
    }
}
