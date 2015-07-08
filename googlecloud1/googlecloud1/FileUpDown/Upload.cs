using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Daimto.Drive.api;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using System.Threading.Tasks;
using Daimto.Drive.api;
namespace googlecloud1.FileUpDown
{
    public delegate void StreamComplete(object sender, ProgressBar progress);
    public delegate void StreamProgress(object sender, long maxsize, long Downloaded, ProgressBar progress);
    public delegate void StreamControl(object sender, ProgressBar progress, Label label);
    class FileUpload : Download
    {
        DriveService service;
        string Parent;
        System.IO.FileStream realfile;
        public FileUpload(DriveService service, string file, string parent, System.IO.FileStream realfile)
        {
            this.service = service;
            this.filename = file;
            this.Parent = parent;
            this.realfile = realfile;
        }
    
        protected override async void StreamThread()
        {
            if (System.IO.File.Exists(this.filename))
            {
                File body = new File();
                long contentlong = 0;
                label.Text = "0%";
                label.Name = "label";
                label.AutoSize = true;
                progress.Value = 0;
                progress.Maximum = 100;
                progress.Controls.Add(label);
                progress.Size = new System.Drawing.Size(181, 23);
                body.Title = System.IO.Path.GetFileName(this.filename);
                body.Description = "File uploaded by 클라우드 통합관리";
                body.MimeType = DaimtoGoogleDriveHelper.GetMimeType(this.filename);
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = this.Parent } };
                FilesResource.InsertMediaUpload request = null;
                byte[] byteArray = new byte[this.StreamBlockSize];
                int ReadSize = 0;
                System.IO.MemoryStream mstream = new System.IO.MemoryStream();
                try
                {
                    while ((ReadSize = await realfile.ReadAsync(byteArray, 0, this.StreamBlockSize, CancellationToken.None)) > 0)
                    {
                        mstream.Capacity = ReadSize;
                        contentlong += ReadSize;
                        request = this.service.Files.Insert(body, mstream, DaimtoGoogleDriveHelper.GetMimeType(this.filename));
                        this.OnProgressChange(this, maxsize, contentlong, this.progress);
                    }
                    request.Upload();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return;
                }
            }
            else
            {
                Console.WriteLine("File does not exist: " + this.filename);
                return;
            }           
        }

        public override void Start()
        {
            if ((this.thread != null) & (this.thread.ThreadState == ThreadState.Stopped))
                this.Stop();
            thread = new Thread(new ThreadStart(this.StreamThread));
            this.Stopped = false;
            thread.Start();
        }

        public override void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
    }
}
