namespace CentralEcoCity.Video
{
    partial class fm_FullScreen
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
            this.PanelFull = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // PanelFull
            // 
            this.PanelFull.BackgroundImage = global::ReplayAx.Properties.Resources.fullimg1;
            this.PanelFull.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PanelFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelFull.Location = new System.Drawing.Point(0, 0);
            this.PanelFull.Name = "PanelFull";
            this.PanelFull.Size = new System.Drawing.Size(646, 450);
            this.PanelFull.TabIndex = 0;
            this.PanelFull.DoubleClick += new System.EventHandler(this.PanelFull_DoubleClick);
            // 
            // fm_FullScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 450);
            this.Controls.Add(this.PanelFull);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fm_FullScreen";
            this.Text = "fm_FullScreen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelFull;
    }
}