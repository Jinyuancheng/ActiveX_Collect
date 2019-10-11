namespace CentralEcoCity.Video
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ucVGSHow = new CentralEcoCity.Video.ucVideoGroup();
            this.SuspendLayout();
            // 
            // ucVGSHow
            // 
            this.ucVGSHow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.ucVGSHow.Location = new System.Drawing.Point(0, 0);
            this.ucVGSHow.Name = "ucVGSHow";
            this.ucVGSHow.Size = new System.Drawing.Size(905, 545);
            this.ucVGSHow.TabIndex = 0;
            // 
            // ucVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucVGSHow);
            this.DoubleBuffered = true;
            this.Name = "ucVideo";
            this.Size = new System.Drawing.Size(908, 548);
            this.ResumeLayout(false);

        }

        #endregion

        private ucVideoGroup ucVGSHow;
    }
}
