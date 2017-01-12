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
            this.signpadControl1 = new InkPlatform.UserControls.SignpadControl();
            this.listView1 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // signpadControl1
            // 
            this.signpadControl1.BackColor = System.Drawing.Color.White;
            this.signpadControl1.DefaultInkWidth = 0.7F;
            this.signpadControl1.DefaultPenColor = System.Drawing.Color.DarkBlue;
            this.signpadControl1.InkingOnButton = false;
            this.signpadControl1.Location = new System.Drawing.Point(12, 12);
            this.signpadControl1.Logging = true;
            this.signpadControl1.Name = "signpadControl1";
            this.signpadControl1.ResizeCondition = InkPlatform.UserControls.SignpadControl.RESIZE_CONDITION.ACTUAL_SIZE;
            this.signpadControl1.Size = new System.Drawing.Size(396, 100);
            this.signpadControl1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 118);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(396, 97);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            // 
            // SlideshowManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 599);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.signpadControl1);
            this.Name = "SlideshowManager";
            this.Text = "Slideshow Manager";
            this.ResumeLayout(false);

        }

        #endregion

        private InkPlatform.UserControls.SignpadControl signpadControl1;
        private System.Windows.Forms.ListView listView1;
    }
}