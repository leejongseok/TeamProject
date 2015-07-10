using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Daimto.Drive.api;
using System.Windows.Forms;
using googlecloud1;
namespace googlecloud1.Files
{
    class GoogleFile : CloudFiles
    {
        
        DriveService service;
        FilesResource.ListRequest list;
        public GoogleFile(DriveService service)
        {
            this.service = service;
            list = service.Files.List();
        }
        public async Task<List<Control>> GetFileTile(string id)
        {
            string query = "'" + id + "' in parents";
            IList<File> Files = new List<File>();
            try
            {
                list.MaxResults = 1000;
                if (query != null)
                {
                    list.Q = query;
                }
                FileList filesFeed = list.Execute();

                while (filesFeed.Items != null)
                {
                    // Adding each item  to the list.
                    foreach (var item in filesFeed.Items)
                    {
                        Files.Add(item);
                    }

                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (filesFeed.NextPageToken == null)
                    {
                        break;
                    }
                    list.PageToken = filesFeed.NextPageToken;
                    filesFeed = list.Execute();
                }
                return LoadChildren(Files);
            }
            catch
            {
                return null;
            }
        }
        private List<Control> LoadChildren(IList<File> items, bool clearExistingItems = true)
        {
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
                return newControls;
            }
            return null;
        }
        /// <summary>
        /// 각각의 파일들과 컨트롤을 연결시켜주고 이벤트를 활성화 시킨다
        /// </summary>
        /// <param name="item"> 파일</param>
        /// <returns>컨트롤을 반환</returns>
        private Control CreateControlForChildObject(File item)
        {
            FileTile tile = new FileTile(item.Title, item.ThumbnailLink);
            tile.Tag = item;
            tile.DoubleClick += ChildObject_DoubleClick;
            tile.Name = item.Id;
            return tile;
        }

        private async void ChildObject_DoubleClick(object sender, EventArgs e)
        {
            //    // 어떤 타일을 눌렀는지 sender를 통해 넘어온다.
             var item = (File)((FileTile)sender).Tag;

            //    // 파일인지 폴더인지 검사하는 쿼리문 작성
             string query = "'" + item.Id + "' in parents";
            //    // 쿼리문으로 검사한다.
                IList<File> file = DaimtoGoogleDriveHelper.GetFiles(service, query);
                // 폴더이면 Count가 1이상이 된다.
                if (file.Count != 0)
                {
                    //다시 폴더안에 파일들을 가져온다.
                    NavigateToItemWithChildren(item.Id, item.Title);
                }
                    //파일이면 count는 0이된다.
                else
                {
                    try
                    {
                        System.IO.Stream stream = await service.HttpClient.GetStreamAsync(item.DownloadUrl);
                        DownLoadToFileItem(stream, item.OriginalFilename, (long)item.FileSize);
                    }
                    catch
                    {
                        MessageBox.Show("파일 저장 오류");
                   }
                }
            }

        public void NavigateToItemWithChildren(string id, string name)
        {
            if (streamNavigateToItemWithChildren != null)
                streamNavigateToItemWithChildren(id, name);
        }


        public event navigate streamNavigateToItemWithChildren;


        public event streamdown streamdownload;

        public void DownLoadToFileItem(System.IO.Stream stream, string name, long maxsize)
        {
            if (streamdownload != null)
                streamdownload(stream, name, maxsize);
        }
    }
}
