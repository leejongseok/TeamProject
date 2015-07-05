using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Daimto.Drive.api;
namespace login11
{
    class login
    {
        private const string client_id = "892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com";
        private const string client_secret = "eyOFpG-LFIfp8ad3usTL81LG";

        public static void SignInToMicrosoftAccount(System.Windows.Forms.IWin32Window parentWindow)
        {
            Authentication.AuthenticateOauth(client_id, client_secret, "baba");
        }
    }
}
