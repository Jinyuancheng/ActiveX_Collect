namespace BethVideo
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlShape = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlShape
            // 
            this.pnlShape.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(148)))), ((int)(((byte)(187)))));
            this.pnlShape.Location = new System.Drawing.Point(200, 78);
            this.pnlShape.Name = "pnlShape";
            this.pnlShape.Size = new System.Drawing.Size(326, 122);
            this.pnlShape.TabIndex = 1;
            this.pnlShape.Visible = false;
            // 
            // ucVideoGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.pnlShape);
            this.Name = "ucVideoGroup";
            this.Size = new System.Drawing.Size(445, 225);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlShape;
    }
}
