namespace BethVideo
{
    partial class ucVideo
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
            this.components = new System.ComponentModel.Container();
            this.tmrDealMsg = new System.Windows.Forms.Timer(this.components);
            this.ucVideoMain = new BethVideo.ucVideoGroup();
            this.SuspendLayout();
            // 
            // tmrDealMsg
            // 
            this.tmrDealMsg.Interval = 2;
            this.tmrDealMsg.Tick += new System.EventHandler(this.tmrDealMsg_Tick);
            // 
            // ucVideoMain
            // 
            this.ucVideoMain.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ucVideoMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucVideoMain.Location = new System.Drawing.Point(0, 0);
            this.ucVideoMain.Name = "ucVideoMain";
            this.ucVideoMain.Size = new System.Drawing.Size(384, 121);
            this.ucVideoMain.TabIndex = 0;
            // 
            // ucVideo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.ucVideoMain);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "ucVideo";
            this.Size = new System.Drawing.Size(384, 121);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmrDealMsg;
        private ucVideoGroup ucVideoMain;
    }
}
