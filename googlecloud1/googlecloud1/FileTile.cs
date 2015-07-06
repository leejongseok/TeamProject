using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Drive.v2;
namespace googlecloud1
{
    public partial class FileTile : UserControl
    {
         private File sourceItem;
        private DriveService connection;
        private bool _selected;

        public FileTile()
        {
            InitializeComponent();
        }

        public DriveService Connection
        {
            get { return connection; }
            set
            {
                if (value == connection)
                    return;
                connection = value;
                SourceItemChanged();
            }
        }
        public File SourceItem
        {
            get { return sourceItem; }
            set
            {
                if (value == sourceItem)
                    return;

                sourceItem = value;
                SourceItemChanged();
            }
        }

        private void SourceItemChanged()
        {
            if (null == SourceItem || null == Connection) return;

            this.filename.Text = SourceItem.Title;

            LoadThumbnail();
        }

        private void LoadThumbnail()
        {
            var thumbnail = sourceItem.IconLink;
            if (null != thumbnail)
            {
                string thumbnailUri = thumbnail;
                pictureBox1.ImageLocation = thumbnailUri;
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            OnClick(EventArgs.Empty);
        }

        private void Control_DoubleClick(object sender, EventArgs e)
        {
            OnDoubleClick(EventArgs.Empty);
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    this.filename.Font = _selected ? new Font(this.filename.Font, FontStyle.Bold) : new Font(this.filename.Font, FontStyle.Regular);
                }
            }
        }
    }
}
