namespace BethVideo
{
    partial class ucSingleVideo
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucSingleVideo));
            this.pnlCaption = new System.Windows.Forms.Panel();
            this.lblCamName = new System.Windows.Forms.Label();
            this.pnlShowVideo = new System.Windows.Forms.Panel();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pnlCaption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCaption
            // 
            this.pnlCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(133)))), ((int)(((byte)(154)))));
            this.pnlCaption.Controls.Add(this.lblCamName);
            this.pnlCaption.Controls.Add(this.pbClose);
            this.pnlCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCaption.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pnlCaption.ForeColor = System.Drawing.Color.White;
            this.pnlCaption.Location = new System.Drawing.Point(0, 0);
            this.pnlCaption.Margin = new System.Windows.Forms.Padding(1);
            this.pnlCaption.Name = "pnlCaption";
            this.pnlCaption.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.pnlCaption.Size = new System.Drawing.Size(349, 18);
            this.pnlCaption.TabIndex = 2;
            // 
            // lblCamName
            // 
            this.lblCamName.AutoSize = true;
            this.lblCamName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCamName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.lblCamName.Location = new System.Drawing.Point(5, 2);
            this.lblCamName.Margin = new System.Windows.Forms.Padding(0);
            this.lblCamName.Name = "lblCamName";
            this.lblCamName.Size = new System.Drawing.Size(41, 12);
            this.lblCamName.TabIndex = 14;
            this.lblCamName.Text = "摄像机";
            this.lblCamName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlShowVideo
            // 
            this.pnlShowVideo.BackgroundImage = global::BethVideo.Properties.Resources.双视3;
            this.pnlShowVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlShowVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShowVideo.Location = new System.Drawing.Point(0, 18);
            this.pnlShowVideo.Name = "pnlShowVideo";
            this.pnlShowVideo.Size = new System.Drawing.Size(349, 286);
            this.pnlShowVideo.TabIndex = 3;
            // 
            // pbClose
            // 
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.pbClose.Image = ((System.Drawing.Image)(resources.GetObject("pbClose.Image")));
            this.pbClose.InitialImage = global::BethVideo.Properties.Resources.VideoClose_0;
            this.pbClose.Location = new System.Drawing.Point(324, 2);
            this.pbClose.Margin = new System.Windows.Forms.Padding(0);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(20, 14);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbClose.TabIndex = 13;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            this.pbClose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbClose_MouseDown);
            this.pbClose.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbClose_MouseUp);
            // 
            // ucSingleVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlShowVideo);
            this.Controls.Add(this.pnlCaption);
            this.Name = "ucSingleVideo";
            this.Size = new System.Drawing.Size(349, 304);
            this.pnlCaption.ResumeLayout(false);
            this.pnlCaption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCaption;
        private System.Windows.Forms.Label lblCamName;
        public System.Windows.Forms.PictureBox pbClose;
        public System.Windows.Forms.Panel pnlShowVideo;
    }
}
