namespace InkDesktop
{
    partial class InkHub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InkHub));
            this.picHardware = new System.Windows.Forms.PictureBox();
            this.cboHardware = new System.Windows.Forms.ComboBox();
            this.picRefresh = new System.Windows.Forms.PictureBox();
            this.picSettings = new System.Windows.Forms.PictureBox();
            this.cmsWebServer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPort = new System.Windows.Forms.ToolStripMenuItem();
            this.btnWebServer = new System.Windows.Forms.PictureBox();
            this.btnSign = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picHardware)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSettings)).BeginInit();
            this.cmsWebServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnWebServer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSign)).BeginInit();
            this.SuspendLayout();
            // 
            // picHardware
            // 
            this.picHardware.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picHardware.Image = global::InkDesktop.Properties.Resources.signpad;
            this.picHardware.Location = new System.Drawing.Point(12, 12);
            this.picHardware.Name = "picHardware";
            this.picHardware.Size = new System.Drawing.Size(90, 90);
            this.picHardware.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picHardware.TabIndex = 1;
            this.picHardware.TabStop = false;
            // 
            // cboHardware
            // 
            this.cboHardware.BackColor = System.Drawing.SystemColors.Control;
            this.cboHardware.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHardware.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboHardware.FormattingEnabled = true;
            this.cboHardware.Location = new System.Drawing.Point(124, 22);
            this.cboHardware.Name = "cboHardware";
            this.cboHardware.Size = new System.Drawing.Size(206, 32);
            this.cboHardware.TabIndex = 5;
            // 
            // picRefresh
            // 
            this.picRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picRefresh.Image = global::InkDesktop.Properties.Resources.refresh;
            this.picRefresh.Location = new System.Drawing.Point(260, 60);
            this.picRefresh.Name = "picRefresh";
            this.picRefresh.Size = new System.Drawing.Size(32, 32);
            this.picRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRefresh.TabIndex = 6;
            this.picRefresh.TabStop = false;
            this.picRefresh.Click += new System.EventHandler(this.picRefresh_Click);
            // 
            // picSettings
            // 
            this.picSettings.BackColor = System.Drawing.Color.White;
            this.picSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSettings.Image = global::InkDesktop.Properties.Resources.info;
            this.picSettings.Location = new System.Drawing.Point(298, 60);
            this.picSettings.Name = "picSettings";
            this.picSettings.Size = new System.Drawing.Size(32, 32);
            this.picSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSettings.TabIndex = 9;
            this.picSettings.TabStop = false;
            this.picSettings.Click += new System.EventHandler(this.picSettings_Click);
            // 
            // cmsWebServer
            // 
            this.cmsWebServer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiToggle,
            this.tsmiPort});
            this.cmsWebServer.Name = "cmsWebServer";
            this.cmsWebServer.Size = new System.Drawing.Size(134, 48);
            // 
            // tsmiToggle
            // 
            this.tsmiToggle.Name = "tsmiToggle";
            this.tsmiToggle.Size = new System.Drawing.Size(133, 22);
            this.tsmiToggle.Text = "Start Server";
            this.tsmiToggle.Click += new System.EventHandler(this.tsmiToggle_Click);
            // 
            // tsmiPort
            // 
            this.tsmiPort.Name = "tsmiPort";
            this.tsmiPort.Size = new System.Drawing.Size(133, 22);
            this.tsmiPort.Text = "Port";
            this.tsmiPort.Click += new System.EventHandler(this.tsmiPort_Click);
            // 
            // btnWebServer
            // 
            this.btnWebServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnWebServer.ContextMenuStrip = this.cmsWebServer;
            this.btnWebServer.Image = global::InkDesktop.Properties.Resources.webserver_stop;
            this.btnWebServer.InitialImage = null;
            this.btnWebServer.Location = new System.Drawing.Point(3, 129);
            this.btnWebServer.Name = "btnWebServer";
            this.btnWebServer.Size = new System.Drawing.Size(64, 64);
            this.btnWebServer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnWebServer.TabIndex = 10;
            this.btnWebServer.TabStop = false;
            this.btnWebServer.Click += new System.EventHandler(this.btnWebServer_Click);
            // 
            // btnSign
            // 
            this.btnSign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnSign.ContextMenuStrip = this.cmsWebServer;
            this.btnSign.Image = global::InkDesktop.Properties.Resources.sign1;
            this.btnSign.InitialImage = null;
            this.btnSign.Location = new System.Drawing.Point(68, 129);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(64, 64);
            this.btnSign.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnSign.TabIndex = 11;
            this.btnSign.TabStop = false;
            this.btnSign.Click += new System.EventHandler(this.btnSign_Click);
            // 
            // InkHub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 194);
            this.ContextMenuStrip = this.cmsWebServer;
            this.Controls.Add(this.btnSign);
            this.Controls.Add(this.btnWebServer);
            this.Controls.Add(this.picSettings);
            this.Controls.Add(this.picRefresh);
            this.Controls.Add(this.cboHardware);
            this.Controls.Add(this.picHardware);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InkHub";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ink Desktop";
            this.Activated += new System.EventHandler(this.InkHub_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InkHub_FormClosing);
            this.Load += new System.EventHandler(this.InkHub_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picHardware)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSettings)).EndInit();
            this.cmsWebServer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnWebServer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSign)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picHardware;
        private System.Windows.Forms.ComboBox cboHardware;
        private System.Windows.Forms.PictureBox picRefresh;
        private System.Windows.Forms.PictureBox picSettings;
        private System.Windows.Forms.ContextMenuStrip cmsWebServer;
        private System.Windows.Forms.ToolStripMenuItem tsmiToggle;
        private System.Windows.Forms.ToolStripMenuItem tsmiPort;
        private System.Windows.Forms.PictureBox btnWebServer;
        private System.Windows.Forms.PictureBox btnSign;
    }
}

