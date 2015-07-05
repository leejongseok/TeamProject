using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        public main()
        {
            InitializeComponent();
        }

        private void main_Load(object sender, EventArgs e)
        {
            Signin();
        }
        public void Signin()
        {
            service = Authentication.AuthenticateOauth("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com", "eyOFpG-LFIfp8ad3usTL81LG", "low");
            if(service != null)
            {
                try
                {
                    string Q = "title = '메신저' and mimeType = 'application/vnd.google-apps.folder'";
                    IList<File> _Files = DaimtoGoogleDriveHelper.GetFiles(service, Q);

                    foreach (File item in _Files)
                    {
                        this.listBox1.Items.Add(item.Title + " " + item.MimeType);
                    }
                    // If there isn't a directory with this name lets create one.
                    if (_Files.Count == 0)
                    {
                        _Files.Add(DaimtoGoogleDriveHelper.createDirectory(service, "test", "test", "root"));
                    }

                    // We should have a directory now because we either had it to begin with or we just created one.
                    if (_Files.Count != 0)
                    {

                        // This is the ID of the directory 
                        string directoryId = _Files[0].Id;

                        //Upload a file
                        File newFile = DaimtoGoogleDriveHelper.uploadFile(service, @"c:\GoogleDevelop\dummyUploadFile.txt", directoryId);
                        // Update The file
                        File UpdatedFile = DaimtoGoogleDriveHelper.updateFile(service, @"c:\GoogleDevelop\dummyUploadFile.txt", directoryId, newFile.Id);
                        // Download the file
                        DaimtoGoogleDriveHelper.downloadFile(service, newFile, @"C:\GoogleDevelop\downloaded.txt");
                        // delete The file
                       // FilesResource.DeleteRequest request = service.Files.Delete(newFile.Id);
                       // request.Execute();
                    }

                    // Getting a list of ALL a users Files (This could take a while.)
                    _Files = DaimtoGoogleDriveHelper.GetFiles(service, null);

                    foreach (File item in _Files)
                    {
                        this.listBox1.Items.Add(item.Title + " " + item.MimeType);
                    }
                }
                catch (Exception ex)
                {

                    int i = 1;
                }
            }
        }
    }
}
