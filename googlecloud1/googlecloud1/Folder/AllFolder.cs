using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using googlecloud1.Files;
namespace googlecloud1.Folder
{
    abstract class AllFolder
    {
        public abstract Task AddFiles(string id);
        public abstract List<CloudFiles> GetFiles();
    }
}
