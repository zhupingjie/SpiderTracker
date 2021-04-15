
namespace SpiderTracker.UI
{
    partial class ImagePreviewUC
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
            this.pnlImagePanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlImagePanel
            // 
            this.pnlImagePanel.AutoScroll = true;
            this.pnlImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.pnlImagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImagePanel.Location = new System.Drawing.Point(0, 27);
            this.pnlImagePanel.Name = "pnlImagePanel";
            this.pnlImagePanel.Padding = new System.Windows.Forms.Padding(5);
            this.pnlImagePanel.Size = new System.Drawing.Size(811, 402);
            this.pnlImagePanel.TabIndex = 23;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 27);
            this.panel1.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(396, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "*[双击]=查看原图  [Del]=忽略图片  [回车]=上传图片  [BACK]=撤销上传";
            // 
            // ImagePreviewUC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlImagePanel);
            this.Controls.Add(this.panel1);
            this.Name = "ImagePreviewUC";
            this.Size = new System.Drawing.Size(811, 429);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlImagePanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}
