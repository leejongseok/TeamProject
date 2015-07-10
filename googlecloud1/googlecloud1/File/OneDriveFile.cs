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
        ODConnection connect;
        ODItem oditem;
        public event navigate streamNavigateToItemWithChildren;
        public OneDriveFile(ODConnection connect)
        {
            this.connect = connect;
        }
        public async Task<List<Control>> GetFileTile(string id)
        {
            ODItemReference item = new ODItemReference { Id = id };
         
            var selectedItem = await connect.GetItemAsync(item, ItemRetrievalOptions.DefaultWithChildrenThumbnails);
            return await ProcessFolder(selectedItem);
        }

        private async Task<List<Control>> ProcessFolder(ODItem selectedItem)
        {
            if (null != selectedItem)
            {
                this.oditem = selectedItem;
                List<Control> controls = new List<Control>();
                //LoadProperties(folder);
                ODItemCollection pagedItemCollection = await selectedItem.PagedChildrenCollectionAsync(connect, ChildrenRetrievalOptions.DefaultWithThumbnails);
                controls = await LoadChildren(pagedItemCollection, false);

                while (pagedItemCollection.MoreItemsAvailable())
                {
                    pagedItemCollection = await pagedItemCollection.GetNextPage(connect);
                    controls = await LoadChildren(pagedItemCollection, false);
                }
                return controls;
            }
            return null;
        }
        private async Task<List<Control>> LoadChildren(ODItemCollection pagedItemCollection, bool p)
        {
            if (null != pagedItemCollection)
            {
                List<Control> newControls = new List<Control>();
                foreach (var obj in pagedItemCollection.Collection)
                {
                    newControls.Add(await CreateControlForChildObject(obj));
                }
                return newControls;
            }
            return null;
        }

        private async Task<Control> CreateControlForChildObject(ODItem obj)
        {
            //var tumb = await obj.GetDefaultThumbnailUrlAsync(connect, "Medium");
            FileTile tile = new FileTile(obj.Name, null);
            tile.Tag = obj;
            tile.sourceitem1 = obj.Name;
            tile.DoubleClick += ChildObject_DoubleClick;
            tile.Name = obj.Id;
            return tile;
        }

        private async void ChildObject_DoubleClick(object sender, EventArgs e)
        {
            var item = (ODItem)((FileTile)sender).Tag;

            // Look up the object by ID
            if (item.CanHaveChildren())
            {
                NavigateToItemWithChildren(item.Id, item.Name);
            }
            else
            {
                System.IO.Stream stream = await item.GetContentStreamAsync(connect, StreamDownloadOptions.Default);
                DownLoadToFileItem(stream, item.Name, item.Size);
            }
        }


        public void NavigateToItemWithChildren(string id, string name)
        {
            if (streamNavigateToItemWithChildren != null)
                streamNavigateToItemWithChildren(id, name);
        }


        public event streamdown streamdownload;

        public void DownLoadToFileItem(System.IO.Stream stream, string name, long maxsize)
        {
            if (streamdownload != null)
                streamdownload(stream, name, maxsize);
        }
    }
}
