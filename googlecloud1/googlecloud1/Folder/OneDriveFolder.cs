using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneDrive;
using googlecloud1.Files;
namespace googlecloud1.Folder
{
    class OneDriveFolder : AllFolder
    {
        List<OneDriveFile> files;
        ODConnection connect;

        public ODConnection Connect
        {
            get { return connect; }
            set { connect = value; }
        }
        public OneDriveFolder(ODConnection connect)
        {
            this.connect = connect;
        }
        public override async Task AddFiles(string id)
        {
            ODItemReference item = new ODItemReference { Id = id };
            files = new List<OneDriveFile>();
            ODItem selectedItem;
            selectedItem = await connect.GetItemAsync(item, ItemRetrievalOptions.DefaultWithChildrenThumbnails);
            
            if (null != selectedItem)
            {
                var pagedItemCollection = await selectedItem.PagedChildrenCollectionAsync(connect, ChildrenRetrievalOptions.DefaultWithThumbnails);

                while (pagedItemCollection.MoreItemsAvailable())
                {
                    pagedItemCollection = await pagedItemCollection.GetNextPage(connect);
                }

                foreach (var items in pagedItemCollection.Collection)
                {
                    OneDriveFile oditem = new OneDriveFile(connect);
                    oditem.Oditem = items;
                    files.Add(oditem);
                }
            }
        }
        public override List<CloudFiles> GetFiles()
        {
            List<CloudFiles> cloude = new List<CloudFiles>();
            foreach (var item in files)
            {
                CloudFiles cl = item;
                cloude.Add(cl);
            }
            return cloude;
        }
    }
}
