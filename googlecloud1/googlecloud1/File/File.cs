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
    interface CloudFiles
    {
        FileTile SetTile();
        string GetId();
        bool DoubleClick();
        Task<System.IO.Stream> GetSteam();
        long GetFileSize();
    }
}
