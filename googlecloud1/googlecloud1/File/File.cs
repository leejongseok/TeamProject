using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Google.Apis.Drive.v2.Data;
using Newtonsoft.Json;
using System.IO;
namespace googlecloud1.Files
{
    public delegate void navigate(string id, string name);
    public delegate void streamdown(Stream stream, string name, long maxsize);
    interface CloudFiles
    {
        Task<List<Control>> GetFileTile(string id);
        void NavigateToItemWithChildren(string id, string name);
        void DownLoadToFileItem(Stream stream, string name, long maxsize);
        event navigate streamNavigateToItemWithChildren;
        event streamdown streamdownload;
    }
}
