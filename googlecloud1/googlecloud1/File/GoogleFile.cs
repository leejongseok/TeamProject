using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Daimto.Drive.api;
using System.Windows.Forms;
using googlecloud1;
namespace googlecloud1.Files
{
    class GoogleFile : CloudFiles
    {
        File item;
        DriveService service;

        public DriveService Service
        {
            get { return service; }
        }
        public File Item
        {
            get { return item; }
            set { item = value; }
        }
        public GoogleFile(DriveService service)
        {
            this.service = service;
        }
        public FileTile SetTile()
        {
            FileTile tile = new FileTile(item.Title, item.ThumbnailLink);
            tile.Tag = this;
            tile.Name = item.Id;
            return tile;
        }
        public string GetId()
        {
            return item.Id;
        }
        public bool DoubleClick()
        {
            if(item.FileExtension == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<System.IO.Stream> GetSteam()
        {
            try
            {
                return await service.HttpClient.GetStreamAsync(item.DownloadUrl);
            }
            catch
            {
                return null;
            }
        }
        public long GetFileSize()
        {
            return (long)item.FileSize;
        }
    }
}
