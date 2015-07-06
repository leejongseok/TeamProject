using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v2;
using Daimto.Drive.api;
using Google.Apis.Drive.v2.Data;
namespace googlecloud1
{
    public partial class main : Form
    {
        DriveService service;
        File file { get; set; }
        private File SelectedItem { get; set; }
        public main()
        {
            InitializeComponent();
        }

        private async void main_Load(object sender, EventArgs e)
        {
            await Signin();
        }
        public async Task Signin()
        {
            service = Authentication.AuthenticateOauth("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com", "eyOFpG-LFIfp8ad3usTL81LG", "bitbit");
            if(service != null)
            {
                await LoadFolderFromId("root");
            }
        }
        private void ShowWork(bool working)
        {
            this.UseWaitCursor = working;
            this.progressBar1.Visible = working;
        }
        private async Task LoadFolderFromId(string id)
        {
            if (null == service) return;

            IList<File> files = DaimtoGoogleDriveHelper.GetFiles(service, null);
            // Update the UI for loading something new
            ShowWork(true);
            //LoadChildren(files);  // Clear the current folder view
            try
            {
                if (null != files)
                {
                    foreach (var item in files)
                    {

                        listBox1.Items.Add(item.Title);
                    }
                }
                FilesResource file = service.Files;
                var selectedItem = await file.Get(id).ExecuteAsync();
                //ProcessFolder(selectedItem);
            }
            catch
            {
                
            }

            ShowWork(false);
        }
        private void LoadChildren(IList<File> items, bool clearExistingItems = true)
        {
            if(null != items)
            {
                foreach (var item in items)
                {
                    
                    listBox1.Items.Add(item.Title);
                }
            }
        }
        //private async void ProcessFolder(File folder)
        //{
        //    if (null != folder)
        //    {
        //        this.file = folder;

        //        LoadProperties(folder);
        //        FileList pagedItemCollection = await folder.PagedChildrenCollectionAsync(Connection, ChildrenRetrievalOptions.DefaultWithThumbnails);
        //        LoadChildren(pagedItemCollection, false);

        //        while (pagedItemCollection.MoreItemsAvailable())
        //        {
        //            pagedItemCollection = await pagedItemCollection.GetNextPage(Connection);
        //            LoadChildren(pagedItemCollection, false);
        //        }
        //    }
        //}

        //private void LoadProperties(File item)
        //{
        //    SelectedItem = item;
        //    oneDriveObjectBrowser1.SelectedItem = item;
        //}
        //private void LoadChildren(FileList items, bool clearExistingItems = true)
        //{
        //    flowLayoutPanel_filecontent.SuspendLayout();

        //    if (clearExistingItems)
        //        flowLayoutPanel_filecontent.Controls.Clear();

        //    // Load the children
        //    if (null != items)
        //    {
        //        List<Control> newControls = new List<Control>();
        //        foreach (var obj in items.Items)
        //        {
        //            //newControls.Add(CreateControlForChildObject(obj));
        //        }
        //        flowLayoutPanel_filecontent.Controls.AddRange(newControls.ToArray());
        //    }

        //    flowLayoutPanel_filecontent.ResumeLayout();
        //}
        //private Control CreateControlForChildObject(File item)
        //{
        //    FileTile tile = new FileTile { SourceItem = item, Connection = this.Connection };
        //    tile.Click += ChildObject_Click;
        //    tile.DoubleClick += ChildObject_DoubleClick;
        //    tile.Name = item.Id;
        //    return tile;
        //}
    }
}
