using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneDrive;
using System.Windows.Forms;
namespace googlecloud1.Files
{
    class OneDriveFile : CloudFiles
    {
        ODItem oditem;
        ODConnection connect;

        public ODConnection Connect
        {
            get { return connect; }
        }
        public ODItem Oditem
        {
            get { return oditem; }
            set { oditem = value; }
        }
        public OneDriveFile(ODConnection connect)
        {
            this.connect = connect;
        }
        public FileTile SetTile()
        {
            FileTile tile = new FileTile(oditem.Name, "");
            tile.Tag = this;
            tile.Name = oditem.Id;
            return tile;
        }
        public string GetId()
        {
            return oditem.Id;
        }
        public bool DoubleClick()
        {
            if(oditem.CanHaveChildren())
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
                return await oditem.GetContentStreamAsync(connect, StreamDownloadOptions.Default);
            }
            catch
            {
                return null;
            }
        }
        public long GetFileSize()
        {
            return oditem.Size;
        }
    }
}
