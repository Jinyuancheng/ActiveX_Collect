namespace CentralEcoCity.Video
{
    partial class ucVideoGroup
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlShape = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmSplitScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmOneScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmFourScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmNineScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSixTeenScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDisVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDisAllVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlShape
            // 
            this.pnlShape.BackColor = System.Drawing.Color.PaleGreen;
            this.pnlShape.Location = new System.Drawing.Point(196, 107);
            this.pnlShape.Name = "pnlShape";
            this.pnlShape.Size = new System.Drawing.Size(47, 43);
            this.pnlShape.TabIndex = 2;
            this.pnlShape.Visible = false;
            this.pnlShape.Resize += new System.EventHandler(this.ucVideoGroup_Resize);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSplitScreen,
            this.tsmDisVideo,
            this.tsmDisAllVideo});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 92);
            // 
            // tsmSplitScreen
            // 
            this.tsmSplitScreen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmOneScreen,
            this.tsmFourScreen,
            this.tsmNineScreen,
            this.tsmSixTeenScreen});
            this.tsmSplitScreen.Name = "tsmSplitScreen";
            this.tsmSplitScreen.Size = new System.Drawing.Size(180, 22);
            this.tsmSplitScreen.Text = "分屏切换";
            this.tsmSplitScreen.Click += new System.EventHandler(this.tsmSplitScreen_Click);
            // 
            // tsmOneScreen
            // 
            this.tsmOneScreen.Name = "tsmOneScreen";
            this.tsmOneScreen.Size = new System.Drawing.Size(180, 22);
            this.tsmOneScreen.Text = "单画面";
            // 
            // tsmFourScreen
            // 
            this.tsmFourScreen.Name = "tsmFourScreen";
            this.tsmFourScreen.Size = new System.Drawing.Size(180, 22);
            this.tsmFourScreen.Text = "四画面";
            // 
            // tsmNineScreen
            // 
            this.tsmNineScreen.Name = "tsmNineScreen";
            this.tsmNineScreen.Size = new System.Drawing.Size(180, 22);
            this.tsmNineScreen.Text = "九画面";
            // 
            // tsmSixTeenScreen
            // 
            this.tsmSixTeenScreen.Name = "tsmSixTeenScreen";
            this.tsmSixTeenScreen.Size = new System.Drawing.Size(180, 22);
            this.tsmSixTeenScreen.Text = "十六画面";
            // 
            // tsmDisVideo
            // 
            this.tsmDisVideo.Name = "tsmDisVideo";
            this.tsmDisVideo.Size = new System.Drawing.Size(180, 22);
            this.tsmDisVideo.Text = "断开视频";
            // 
            // tsmDisAllVideo
            // 
            this.tsmDisAllVideo.Name = "tsmDisAllVideo";
            this.tsmDisAllVideo.Size = new System.Drawing.Size(180, 22);
            this.tsmDisAllVideo.Text = "断开全部视频";
            // 
            // ucVideoGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.Controls.Add(this.pnlShape);
            this.DoubleBuffered = true;
            this.Name = "ucVideoGroup";
            this.Size = new System.Drawing.Size(472, 309);
            this.Resize += new System.EventHandler(this.ucVideoGroup_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlShape;
        private System.Windows.Forms.ContextMenuStrip cmsVideo;
        private System.Windows.Forms.ToolStripMenuItem 分屏切换ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tmsiSingleScreen;
        private System.Windows.Forms.ToolStripMenuItem tmsiFourScreen;
        private System.Windows.Forms.ToolStripMenuItem tmsiNineScreen;
        private System.Windows.Forms.ToolStripMenuItem tmsiSixtheenScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiDisConect;
        private System.Windows.Forms.ToolStripMenuItem tsmiDisconectAll;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSplitScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmOneScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmFourScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmNineScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmSixTeenScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmDisVideo;
        private System.Windows.Forms.ToolStripMenuItem tsmDisAllVideo;
    }
}
