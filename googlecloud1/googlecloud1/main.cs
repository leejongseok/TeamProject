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
using googlecloud1.login;
using googlecloud1.Files;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Drive.v2;
using googlecloud1.Folder;
using OneDrive;
using googlecloud1.FileUpDown;
namespace googlecloud1
{
    public partial class main : Form
    {
        delegate void SafeDownloadComplete(object sender, ProgressBar progress);
        delegate void SafeDownloadProgresbar(object sender, long maxsize, long Downloaded, ProgressBar progress);
        delegate void SafeDownloadControl(object sender, ProgressBar progress, Label label);
        Setting setting;
        List<AllFolder> folder;
        SafeDownloadComplete SDC;
        SafeDownloadProgresbar SDP;
        SafeDownloadControl SDCT;
        FileDownload down;
        public main()
        {
            InitializeComponent();
            SDC = new SafeDownloadComplete(down_StreamCompleteCallback);
            SDP = new SafeDownloadProgresbar(down_StreamProgressCallback);
            SDCT = new SafeDownloadControl(down_StreamControlCallBack);
        }

        private void main_Load(object sender, EventArgs e)
        {
            setting = new Setting();
            folder = new List<AllFolder>();
        }

        private void 로그인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           setting.ShowDialog();
        }
        private async void FileLoad()
        {
            if(Setting.googlelist != null)
            {
                ShowWork(true);
                foreach (var item in Setting.googlelist)
                {
                    await LoadGoogleFile("root", item);
                }
            }
            if(Setting.onedrivelist != null)
            {
                foreach (var item in Setting.onedrivelist)
                {
                    await LoadOneDriveFile("root", item);
                }
            }
            foreach (var item in folder)
            {
                LoadTile(item); 
            }  
            ShowWork(false);
        }

        private void ShowWork(bool p)
        {
            this.UseWaitCursor = p;
            this.progressBar1.Visible = p;
        }
        
        private void LoadTile(AllFolder folder)
        {
           if(folder != null)
           {
               flowLayoutPanel_filecontent.SuspendLayout();
               List<CloudFiles> cl = folder.GetFiles();
               List<Control> newControl = new List<Control>();
               foreach (var item in cl)
               {
                  newControl.Add(item.SetTile());
                }
                foreach (var item in newControl)
                {
                   item.DoubleClick += item_DoubleClick;
                   item.MouseClick += item_MouseClick;
                }
                flowLayoutPanel_filecontent.Controls.AddRange(newControl.ToArray());
                flowLayoutPanel_filecontent.ResumeLayout();
           }
        }

        void item_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        async void item_DoubleClick(object sender, EventArgs e)
        {
            var item = (CloudFiles)((FileTile)sender).Tag;
            if(item.DoubleClick())
            {
                SaveFileDialog filedialog = new SaveFileDialog();
                var result = filedialog.ShowDialog();
                if(result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                System.IO.Stream stream = await item.GetSteam();
                down = new FileDownload(filedialog.FileName, stream, (long)item.GetFileSize());
                down.StreamCompleteCallback += down_StreamCompleteCallback;
                down.StreamControlCallBack += down_StreamControlCallBack;
                down.StreamProgressCallback += down_StreamProgressCallback;
                down.Start();
            }
            else
            {
                NavigateToItem(item);
            }
        }

        void down_StreamProgressCallback(object sender, long maxsize, long Downloaded, ProgressBar progress)
        {
            if (progress.InvokeRequired)
                progress.Invoke(SDP, new object[] { sender, maxsize, Downloaded, progress });
            else
            {
                float per = ((float)Downloaded / maxsize) * 100;
                progress.Value = (int)per;
                progress.Controls[0].Text = string.Format("{0}%", (int)per);
            }
        }

        void down_StreamControlCallBack(object sender, ProgressBar progress, Label label)
        {
            if(downflowPanel.InvokeRequired)
                downflowPanel.Invoke(SDCT, new object[] { sender, progress, label });
            else
            {
                downflowPanel.Controls.Add(progress);
            }
        }

        void down_StreamCompleteCallback(object sender, ProgressBar progress)
        {
            if (progress.InvokeRequired)
                progress.Invoke(SDC, new object[] { sender, progress });
            else
            {
                MessageBox.Show("다운 완료");
                int index = downflowPanel.Controls.IndexOf(progress);
                downflowPanel.Controls.RemoveAt(index);
                down.Stop();
            }

        }

        private async void NavigateToItem(CloudFiles item)
        {
            flowLayoutPanel_filecontent.Controls.Clear();
            if((item as GoogleFile) != null)
            {
                GoogleFile file = (GoogleFile)item;
                AllFolder newfolder = new GoogleFolder(file.Service);
                await newfolder.AddFiles(file.GetId());
                LoadTile(newfolder);
            }
            else if((item as OneDriveFile) != null)
            {
                OneDriveFile file = (OneDriveFile)item;
                AllFolder newfolder = new OneDriveFolder(file.Connect);
                await newfolder.AddFiles(file.GetId());
                LoadTile(newfolder);
            }
            else
            {
                MessageBox.Show("파일 열기 실패");
            }
        }
        private async Task LoadOneDriveFile(string p, ODConnection item)
        {
            OneDriveFolder Ones = new OneDriveFolder(item);
            await Ones.AddFiles(p);
            Ones.ToString();
            folder.Add(Ones);
        }
        private async Task LoadGoogleFile(string id, DriveService service)
        {
            GoogleFolder google = new GoogleFolder(service);
            await google.AddFiles(id);
            folder.Add(google);
        }
        private void 연결ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutPanel_filecontent.Controls.Clear();
            folder.Clear();
            FileLoad();
        }
    }
}
