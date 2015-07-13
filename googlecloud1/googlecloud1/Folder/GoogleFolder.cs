using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Drive.v2;
using googlecloud1.Files;
namespace googlecloud1.Folder
{
    class GoogleFolder : AllFolder
    {
        List<GoogleFile> GoogleFiles;
        DriveService service;

        public DriveService Service
        {
            get { return service; }
            set { service = value; }
        }
        public GoogleFolder(DriveService service)
        {
            this.service = service;
        }
        public override async Task AddFiles(string id)
        {
            FilesResource.ListRequest list = new FilesResource.ListRequest(service);
            string query = "'" + id + "' in parents";
            GoogleFiles = new List<GoogleFile>();
            try
            {
                list.MaxResults = 1000;
                if (query != null)
                {
                    list.Q = query;
                }
                FileList filesFeed = list.Execute();

                while (filesFeed.Items != null)
                {
                    // Adding each item  to the list.
                    foreach (var item in filesFeed.Items)
                    {
                        GoogleFile gfile = new GoogleFile(service);
                        gfile.Item = item;
                        GoogleFiles.Add(gfile);
                    }

                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (filesFeed.NextPageToken == null)
                    {
                        break;
                    }
                    list.PageToken = filesFeed.NextPageToken;
                    filesFeed = list.Execute();
                }
            }
            catch
            {
                //return null;
            }
        }
        public override List<CloudFiles> GetFiles()
        {
            List<CloudFiles> cloude = new List<CloudFiles>();
            foreach (var item in GoogleFiles)
            {
                CloudFiles cl = item;
                cloude.Add(cl);
            }
            return cloude;
        }
    }
}
