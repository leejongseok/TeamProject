using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemiro.OAuth;
namespace googlecloud1.Files
{
    class DropBoxFile : CloudFiles
    {
        UniValue file;
        public DropBoxFile(UniValue value)
        {
            this.file = value;
        }
        public FileTile SetTile()
        {
            FileTile tile = new FileTile(file[0]["path"].ToString(), "");
            tile.Tag = this;
            tile.Name = "";
            return tile;
        }

        public string GetId()
        {
            throw new NotImplementedException();
        }

        public bool DoubleClick()
        {
            throw new NotImplementedException();
        }

        public Task<System.IO.Stream> GetSteam()
        {
            throw new NotImplementedException();
        }

        public long GetFileSize()
        {
            throw new NotImplementedException();
        }
    }
}
