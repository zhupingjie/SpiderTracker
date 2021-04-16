
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
            this.lblReomteMsg = new System.Windows.Forms.Label();
            this.lblImageMsg = new System.Windows.Forms.Label();
            this.pnlOriginPanel = new System.Windows.Forms.Panel();
            this.pnlOriginImgTitle = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDelOrgImg = new System.Windows.Forms.Button();
            this.btnOrgDelImg = new System.Windows.Forms.Button();
            this.btnOrgUpdoadImg = new System.Windows.Forms.Button();
            this.btnOrgPreImg = new System.Windows.Forms.Button();
            this.btnOrgNextImg = new System.Windows.Forms.Button();
            this.btnShowThumbImg = new System.Windows.Forms.Button();
            this.btnSetWinBkg = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnlOriginPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlImagePanel
            // 
            this.pnlImagePanel.AutoScroll = true;
            this.pnlImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.pnlImagePanel.Location = new System.Drawing.Point(0, 27);
            this.pnlImagePanel.Name = "pnlImagePanel";
            this.pnlImagePanel.Padding = new System.Windows.Forms.Padding(5);
            this.pnlImagePanel.Size = new System.Drawing.Size(421, 396);
            this.pnlImagePanel.TabIndex = 23;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblReomteMsg);
            this.panel1.Controls.Add(this.lblImageMsg);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 27);
            this.panel1.TabIndex = 26;
            // 
            // lblReomteMsg
            // 
            this.lblReomteMsg.AutoSize = true;
            this.lblReomteMsg.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblReomteMsg.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblReomteMsg.Location = new System.Drawing.Point(811, 0);
            this.lblReomteMsg.Name = "lblReomteMsg";
            this.lblReomteMsg.Size = new System.Drawing.Size(0, 25);
            this.lblReomteMsg.TabIndex = 1;
            // 
            // lblImageMsg
            // 
            this.lblImageMsg.AutoSize = true;
            this.lblImageMsg.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblImageMsg.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblImageMsg.Location = new System.Drawing.Point(0, 0);
            this.lblImageMsg.Name = "lblImageMsg";
            this.lblImageMsg.Size = new System.Drawing.Size(0, 25);
            this.lblImageMsg.TabIndex = 0;
            // 
            // pnlOriginPanel
            // 
            this.pnlOriginPanel.BackColor = System.Drawing.Color.Transparent;
            this.pnlOriginPanel.Controls.Add(this.pnlOriginImgTitle);
            this.pnlOriginPanel.Controls.Add(this.panel2);
            this.pnlOriginPanel.Location = new System.Drawing.Point(422, 26);
            this.pnlOriginPanel.Name = "pnlOriginPanel";
            this.pnlOriginPanel.Size = new System.Drawing.Size(384, 396);
            this.pnlOriginPanel.TabIndex = 0;
            this.pnlOriginPanel.Visible = false;
            this.pnlOriginPanel.DoubleClick += new System.EventHandler(this.pnlOriginPanel_DoubleClick);
            // 
            // pnlOriginImgTitle
            // 
            this.pnlOriginImgTitle.BackColor = System.Drawing.Color.Gold;
            this.pnlOriginImgTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOriginImgTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlOriginImgTitle.Name = "pnlOriginImgTitle";
            this.pnlOriginImgTitle.Size = new System.Drawing.Size(384, 10);
            this.pnlOriginImgTitle.TabIndex = 1;
            this.pnlOriginImgTitle.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSetWinBkg);
            this.panel2.Controls.Add(this.btnDelOrgImg);
            this.panel2.Controls.Add(this.btnOrgDelImg);
            this.panel2.Controls.Add(this.btnOrgUpdoadImg);
            this.panel2.Controls.Add(this.btnOrgPreImg);
            this.panel2.Controls.Add(this.btnOrgNextImg);
            this.panel2.Controls.Add(this.btnShowThumbImg);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 366);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(384, 30);
            this.panel2.TabIndex = 0;
            // 
            // btnDelOrgImg
            // 
            this.btnDelOrgImg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDelOrgImg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDelOrgImg.Location = new System.Drawing.Point(108, 0);
            this.btnDelOrgImg.Name = "btnDelOrgImg";
            this.btnDelOrgImg.Size = new System.Drawing.Size(46, 30);
            this.btnDelOrgImg.TabIndex = 14;
            this.btnDelOrgImg.Text = "❌";
            this.btnDelOrgImg.UseVisualStyleBackColor = true;
            this.btnDelOrgImg.Click += new System.EventHandler(this.btnDelOrgImg_Click);
            // 
            // btnOrgDelImg
            // 
            this.btnOrgDelImg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOrgDelImg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOrgDelImg.Location = new System.Drawing.Point(154, 0);
            this.btnOrgDelImg.Name = "btnOrgDelImg";
            this.btnOrgDelImg.Size = new System.Drawing.Size(46, 30);
            this.btnOrgDelImg.TabIndex = 13;
            this.btnOrgDelImg.Text = "▼";
            this.btnOrgDelImg.UseVisualStyleBackColor = true;
            this.btnOrgDelImg.Click += new System.EventHandler(this.btnOrgDelImg_Click);
            // 
            // btnOrgUpdoadImg
            // 
            this.btnOrgUpdoadImg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOrgUpdoadImg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOrgUpdoadImg.Location = new System.Drawing.Point(200, 0);
            this.btnOrgUpdoadImg.Name = "btnOrgUpdoadImg";
            this.btnOrgUpdoadImg.Size = new System.Drawing.Size(46, 30);
            this.btnOrgUpdoadImg.TabIndex = 11;
            this.btnOrgUpdoadImg.Text = "▲";
            this.btnOrgUpdoadImg.UseVisualStyleBackColor = true;
            this.btnOrgUpdoadImg.Click += new System.EventHandler(this.btnOrgUpdoadImg_Click);
            // 
            // btnOrgPreImg
            // 
            this.btnOrgPreImg.BackColor = System.Drawing.Color.Transparent;
            this.btnOrgPreImg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOrgPreImg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOrgPreImg.Location = new System.Drawing.Point(246, 0);
            this.btnOrgPreImg.Name = "btnOrgPreImg";
            this.btnOrgPreImg.Size = new System.Drawing.Size(46, 30);
            this.btnOrgPreImg.TabIndex = 10;
            this.btnOrgPreImg.Text = "◀";
            this.btnOrgPreImg.UseVisualStyleBackColor = false;
            this.btnOrgPreImg.Click += new System.EventHandler(this.btnOrgPreImg_Click);
            // 
            // btnOrgNextImg
            // 
            this.btnOrgNextImg.BackColor = System.Drawing.Color.Transparent;
            this.btnOrgNextImg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOrgNextImg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOrgNextImg.Location = new System.Drawing.Point(292, 0);
            this.btnOrgNextImg.Name = "btnOrgNextImg";
            this.btnOrgNextImg.Size = new System.Drawing.Size(46, 30);
            this.btnOrgNextImg.TabIndex = 9;
            this.btnOrgNextImg.Text = "▶";
            this.btnOrgNextImg.UseVisualStyleBackColor = false;
            this.btnOrgNextImg.Click += new System.EventHandler(this.btnOrgNextImg_Click);
            // 
            // btnShowThumbImg
            // 
            this.btnShowThumbImg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnShowThumbImg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnShowThumbImg.Location = new System.Drawing.Point(338, 0);
            this.btnShowThumbImg.Name = "btnShowThumbImg";
            this.btnShowThumbImg.Size = new System.Drawing.Size(46, 30);
            this.btnShowThumbImg.TabIndex = 8;
            this.btnShowThumbImg.Text = "▢";
            this.btnShowThumbImg.UseVisualStyleBackColor = true;
            this.btnShowThumbImg.Click += new System.EventHandler(this.btnShowThumbImg_Click);
            // 
            // btnSetWinBkg
            // 
            this.btnSetWinBkg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSetWinBkg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSetWinBkg.Location = new System.Drawing.Point(62, 0);
            this.btnSetWinBkg.Name = "btnSetWinBkg";
            this.btnSetWinBkg.Size = new System.Drawing.Size(46, 30);
            this.btnSetWinBkg.TabIndex = 15;
            this.btnSetWinBkg.Text = "▨";
            this.btnSetWinBkg.UseVisualStyleBackColor = true;
            this.btnSetWinBkg.Click += new System.EventHandler(this.btnSetWinBkg_Click);
            // 
            // ImagePreviewUC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlOriginPanel);
            this.Controls.Add(this.pnlImagePanel);
            this.Controls.Add(this.panel1);
            this.Name = "ImagePreviewUC";
            this.Size = new System.Drawing.Size(811, 429);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlOriginPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlImagePanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblImageMsg;
        private System.Windows.Forms.Panel pnlOriginPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnShowThumbImg;
        private System.Windows.Forms.Button btnOrgUpdoadImg;
        private System.Windows.Forms.Button btnOrgPreImg;
        private System.Windows.Forms.Button btnOrgNextImg;
        private System.Windows.Forms.Button btnDelOrgImg;
        private System.Windows.Forms.Button btnOrgDelImg;
        private System.Windows.Forms.Label lblReomteMsg;
        private System.Windows.Forms.Panel pnlOriginImgTitle;
        private System.Windows.Forms.Button btnSetWinBkg;
    }
}
