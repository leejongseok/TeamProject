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
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Requests;
using Google.Apis.Download;
using Google.Apis.Http;
using System.Net;
namespace googlecloud1
{
    public partial class main : Form
    {
        private delegate void CSafeSetValue(object sender, long maxsize, long Downloaded);
        Download down;
        WebClient webclient;
        DriveService service;
        File file { get; set; }
        private CSafeSetValue cssv;
        private File SelectedItem { get; set; }
        public main()
        {
            InitializeComponent();
            webclient = new WebClient();
        }

        private async void main_Load(object sender, EventArgs e)
        {
            cssv = new CSafeSetValue(webclient_UploadProgressChanged);
            await Signin();
        }

        private void webclient_DownloadFileCompleted(object sender)
        {
            MessageBox.Show("다운 완료");
            downprogres.Visible = false;
            label1.Visible = false;
            down.Stop();
        }

        private void webclient_UploadProgressChanged(object sender, long maxsize, long Downloaded)
        {
            if(downprogres.InvokeRequired)
            {
                downprogres.Invoke(cssv, new object[] { sender, maxsize, Downloaded });
            }
            float per = ((float)Downloaded / maxsize) * 100;
            downprogres.Value = (int)per;
        }
        /// <summary>
        /// 드라이브 정보를 받아옴
        /// </summary>
        /// <returns></returns>
        public async Task Signin()
        {
            service = Authentication.AuthenticateOauth("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com", "eyOFpG-LFIfp8ad3usTL81LG", "bit12");
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

            //프로그래스바를 진행시킨다.
            ShowWork(true);
            //file.Parents.ToList();
           
            LoadChildren(null);  // 폴더 뷰를 비운다.
            try
            {
                FilesResource file = new FilesResource(service);
                var selectedItem = await file.Get(id).ExecuteAsync();
                ProcessFolder(selectedItem);
            }
            catch
            {
                
            }

            ShowWork(false);
        }
        //private void LoadChildren(FileList items, bool clearExistingItems = true)
        //{
        //    datacontent.SuspendLayout();

        //    if (clearExistingItems)
        //        datacontent.Controls.Clear();

        //    // Load the children
        //    if (null != items)
        //    {
        //        List<Control> newControls = new List<Control>();
        //        foreach (var obj in items.Items)
        //        {
        //            //newControls.Add(CreateControlForChildObject(obj));
        //        }
        //        datacontent.Controls.AddRange(newControls.ToArray());
        //    }

        //    datacontent.ResumeLayout();
        //}

        //private void LoadChildren(IList<File> items, bool clearExistingItems = true)
        //{
        //    if (null != items)
        //    {
        //        foreach (var item in items)
        //        {
        //           listBox1.Items.Add(item.Title);
        //        }
        //    }
        //}
        private async void ProcessFolder(File folder)
        {
            //폴더가 널이 아니면
            if (null != folder)
            {
                //폴더안에 있는 파일들을 가져오는 쿼리문 작성
                string query = "'" + folder.Id + "' in parents";

                //LoadProperties(folder);
                // 쿼리문을 보내 검색되어진 파일을 가져온다.
                IList<File> file = DaimtoGoogleDriveHelper.GetFiles(service, query);

                if(file.Count != 0)
                {
                    LoadChildren(file, false);
                }

                //while (pagedItemCollection.MoreItemsAvailable())
                //{
                    //pagedItemCollection = await pagedItemCollection.GetNextPage(Connection);
                    //LoadChildren(pagedItemCollection, false);
                //}
            }
        }

        private void LoadProperties(File item)
        {
            SelectedItem = item;
            dob.SelectedItem = item;
        }
        /// <summary>
        /// 가져온 파일들 각각의 컨트롤 생성
        /// </summary>
        /// <param name="items">파일 리스트</param>
        /// <param name="clearExistingItems"></param>
        private void LoadChildren(IList<File> items, bool clearExistingItems = true)
        {
            flowLayoutPanel_filecontent.SuspendLayout();

            if (clearExistingItems)
                flowLayoutPanel_filecontent.Controls.Clear();

            // 자식 파일을 불러온다
            if (null != items)
            {
                // 컨트롤 리스트를 생성한다.
                List<Control> newControls = new List<Control>();
                //가져온 파일 갯수만큼 컨트롤을 만들고 추가해준다.
                foreach (var obj in items)
                {
                    newControls.Add(CreateControlForChildObject(obj));
                }
                flowLayoutPanel_filecontent.Controls.AddRange(newControls.ToArray());
            }

            flowLayoutPanel_filecontent.ResumeLayout();
        }
        /// <summary>
        /// 각각의 파일들과 컨트롤을 연결시켜주고 이벤트를 활성화 시킨다
        /// </summary>
        /// <param name="item"> 파일</param>
        /// <returns>컨트롤을 반환</returns>
        private Control CreateControlForChildObject(File item)
        {
            FileTile tile = new FileTile { SourceItem = item, Connection = this.service };
            tile.Click += ChildObject_Click;
            tile.DoubleClick += ChildObject_DoubleClick;
            tile.Name = item.Id;
            return tile;
        }
        /// <summary>
        /// 파일을 더블클릭하였을떄 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChildObject_DoubleClick(object sender, EventArgs e)
        {
            // 어떤 타일을 눌렀는지 sender를 통해 넘어온다.
            var item = ((FileTile)sender).SourceItem;

            // 파일인지 폴더인지 검사하는 쿼리문 작성
            string query = "'" + item.Id + "' in parents";
            // 쿼리문으로 검사한다.
            IList<File> file = DaimtoGoogleDriveHelper.GetFiles(service, query);
            // 폴더이면 Count가 1이상이 된다.
            if (file.Count != 0)
            {
                //다시 폴더안에 파일들을 가져온다.
                NavigateToItemWithChildren(item);
            }
                //파일이면 count는 0이된다.
            else
            {
                try
                {
                    await DownloadAndSaveItem(item);
                }
                catch
                {
                    MessageBox.Show("파일 저장 오류");
                }
            }
        }

        private async Task DownloadAndSaveItem(File item)
        {
            if(!string.IsNullOrEmpty(item.DownloadUrl))
            {
                var dialog = new SaveFileDialog();
                dialog.FileName = item.OriginalFilename;
                dialog.Filter = "All Files (*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                downprogres.Visible = true;
                label1.Visible = true;
                down = new FileDownload(dialog.FileName, item.DownloadUrl, service, item.FileSize);
                down.StreamCompleteCallback += webclient_DownloadFileCompleted;
                down.StreamProgressCallback += webclient_UploadProgressChanged;
                down.Start();
            }
        }

        private async void NavigateToItemWithChildren(File item)
        {
            //FixBreadCrumbForCurrentFolder(folder);
            await LoadFolderFromId(item.Id);
        }

        private void ChildObject_Click(object sender, EventArgs e)
        {
            
        }

        private void downprogres_Click(object sender, EventArgs e)
        {

        }

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
