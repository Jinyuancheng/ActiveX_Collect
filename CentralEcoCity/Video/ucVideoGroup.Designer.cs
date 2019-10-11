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
            this.pnlShape = new System.Windows.Forms.Panel();
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
    }
}
