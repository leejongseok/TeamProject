namespace OneDriveSamples.Picker
{
    partial class FormOneDrivePicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOneDrivePicker));
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripBackButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripForwardButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 25);
            this.webBrowser.Margin = new System.Windows.Forms.Padding(2);
            this.webBrowser.MinimumSize = new System.Drawing.Size(11, 10);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(633, 423);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBackButton,
            this.toolStripForwardButton,
            this.toolStripButton1,
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(633, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripBackButton
            // 
            this.toolStripBackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBackButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBackButton.Image")));
            this.toolStripBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBackButton.Name = "toolStripBackButton";
            this.toolStripBackButton.Size = new System.Drawing.Size(48, 22);
            this.toolStripBackButton.Text = "< Back";
            this.toolStripBackButton.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripForwardButton
            // 
            this.toolStripForwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripForwardButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripForwardButton.Image")));
            this.toolStripForwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripForwardButton.Name = "toolStripForwardButton";
            this.toolStripForwardButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripForwardButton.Text = ">";
            this.toolStripForwardButton.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(88, 22);
            this.toolStripLabel1.Text = "toolStripLabel1";
            // 
            // FormOneDrivePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 448);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormOneDrivePicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OneDrive Picker";
            this.Load += new System.EventHandler(this.FormMicrosoftAccountAuth_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripBackButton;
        private System.Windows.Forms.ToolStripButton toolStripForwardButton;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}