namespace InkDesktop
{
    partial class SlideshowManager
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
            this.components = new System.ComponentModel.Container();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.listView = new System.Windows.Forms.ListView();
            this.cboPenDevice = new System.Windows.Forms.ComboBox();
            this.btnAddImage = new System.Windows.Forms.Button();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(64, 64);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listView
            // 
            this.listView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.LargeImageList = this.imageList;
            this.listView.Location = new System.Drawing.Point(0, 42);
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(578, 111);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            // 
            // cboPenDevice
            // 
            this.cboPenDevice.FormattingEnabled = true;
            this.cboPenDevice.Location = new System.Drawing.Point(12, 12);
            this.cboPenDevice.Name = "cboPenDevice";
            this.cboPenDevice.Size = new System.Drawing.Size(142, 21);
            this.cboPenDevice.TabIndex = 2;
            this.cboPenDevice.SelectedIndexChanged += new System.EventHandler(this.cboPenDevice_SelectedIndexChanged);
            // 
            // btnAddImage
            // 
            this.btnAddImage.Location = new System.Drawing.Point(493, 10);
            this.btnAddImage.Name = "btnAddImage";
            this.btnAddImage.Size = new System.Drawing.Size(75, 23);
            this.btnAddImage.TabIndex = 3;
            this.btnAddImage.Text = "Add Image";
            this.btnAddImage.UseVisualStyleBackColor = true;
            this.btnAddImage.Click += new System.EventHandler(this.btnAddImage_Click);
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(284, 165);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(75, 23);
            this.btnShow.TabIndex = 4;
            this.btnShow.Text = "Save";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Image = global::InkDesktop.Properties.Resources.refresh;
            this.btnRefresh.Location = new System.Drawing.Point(160, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(25, 25);
            this.btnRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.TabStop = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(219, 167);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(41, 20);
            this.txtInterval.TabIndex = 11;
            this.txtInterval.TextChanged += new System.EventHandler(this.txtInterval_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(145, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Interval (sec)";
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(379, 165);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(75, 23);
            this.btnStartStop.TabIndex = 13;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 201);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(580, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(52, 17);
            this.lblStatus.Text = "Running";
            // 
            // SlideshowManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 223);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInterval);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.btnAddImage);
            this.Controls.Add(this.cboPenDevice);
            this.Controls.Add(this.listView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SlideshowManager";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Slideshow Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SlideshowManager_FormClosing);
            this.Load += new System.EventHandler(this.SlideshowManager_Load);
            this.Resize += new System.EventHandler(this.SlideshowManager_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ComboBox cboPenDevice;
        private System.Windows.Forms.Button btnAddImage;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.PictureBox btnRefresh;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}