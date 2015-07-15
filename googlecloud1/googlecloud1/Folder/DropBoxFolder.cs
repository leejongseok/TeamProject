using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemiro.OAuth;
using Daimto.Drive.api;
using googlecloud1.Files;

namespace googlecloud1.Folder
{
    class DropBoxFolder : AllFolder
    {
        Authentication.DropBoxLogin.TokenResult token = new Authentication.DropBoxLogin.TokenResult();
        List<DropBoxFile> file = new List<DropBoxFile>();
        public DropBoxFolder(Authentication.DropBoxLogin.TokenResult token)
        {
            this.token = token;
        }
        public override Task AddFiles(string id)
        {
            RequestResult result = OAuthUtility.ExecuteRequest("GET", "https://api.dropbox.com/1/metadata/auto/", new HttpParameterCollection { { "path", "/" }, { "access_token", token.AccessToken } });
            GetFiles_Result(result);
            return null;
        }

        public override List<Files.CloudFiles> GetFiles()
        {
            throw new NotImplementedException();
        }
        public void GetFiles_Result(RequestResult result)
        {
            if(result.StatusCode == 200)
            {
                foreach (UniValue item in result["contents"])
                {
                    DropBoxFile drop = new DropBoxFile(item);
                    file.Add(drop);
                }
            }
        }
    }
}
