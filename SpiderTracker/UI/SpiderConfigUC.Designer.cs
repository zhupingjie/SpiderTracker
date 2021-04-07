
namespace SpiderTracker.UI
{
    partial class SpiderConfigUC
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
            this.label1 = new System.Windows.Forms.Label();
            this.GatherThreadCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.StartPageIndex = new System.Windows.Forms.NumericUpDown();
            this.MaxReadPageCount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.MinReadImageCount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.GatherCompleteShutdown = new System.Windows.Forms.CheckBox();
            this.GatherContinueLastPage = new System.Windows.Forms.CheckBox();
            this.ReadUserOfHeFocus = new System.Windows.Forms.CheckBox();
            this.IgnoreDownloadSource = new System.Windows.Forms.CheckBox();
            this.ReadAllOfUser = new System.Windows.Forms.CheckBox();
            this.ReadOwnerUserStatus = new System.Windows.Forms.CheckBox();
            this.ReadUserOfMyFocus = new System.Windows.Forms.CheckBox();
            this.IgnoreReadGetStatus = new System.Windows.Forms.CheckBox();
            this.IgnoreReadArchiveStatus = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.GatherThreadCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPageIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxReadPageCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinReadImageCount)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(9, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "并发用户数量";
            // 
            // GatherThreadCount
            // 
            this.GatherThreadCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GatherThreadCount.Location = new System.Drawing.Point(120, 30);
            this.GatherThreadCount.Name = "GatherThreadCount";
            this.GatherThreadCount.Size = new System.Drawing.Size(103, 23);
            this.GatherThreadCount.TabIndex = 1;
            this.GatherThreadCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.GatherThreadCount.ValueChanged += new System.EventHandler(this.GatherThreadCount_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(9, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "起始采集页码";
            // 
            // StartPageIndex
            // 
            this.StartPageIndex.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StartPageIndex.Location = new System.Drawing.Point(120, 59);
            this.StartPageIndex.Name = "StartPageIndex";
            this.StartPageIndex.Size = new System.Drawing.Size(103, 23);
            this.StartPageIndex.TabIndex = 3;
            this.StartPageIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.StartPageIndex.ValueChanged += new System.EventHandler(this.StartPageIndex_ValueChanged);
            // 
            // MaxReadPageCount
            // 
            this.MaxReadPageCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaxReadPageCount.Location = new System.Drawing.Point(120, 88);
            this.MaxReadPageCount.Name = "MaxReadPageCount";
            this.MaxReadPageCount.Size = new System.Drawing.Size(103, 23);
            this.MaxReadPageCount.TabIndex = 5;
            this.MaxReadPageCount.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.MaxReadPageCount.ValueChanged += new System.EventHandler(this.MaxReadPageCount_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(9, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "最大采集页数";
            // 
            // MinReadImageCount
            // 
            this.MinReadImageCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MinReadImageCount.Location = new System.Drawing.Point(120, 117);
            this.MinReadImageCount.Name = "MinReadImageCount";
            this.MinReadImageCount.Size = new System.Drawing.Size(103, 23);
            this.MinReadImageCount.TabIndex = 7;
            this.MinReadImageCount.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.MinReadImageCount.ValueChanged += new System.EventHandler(this.MinReadImageCount_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(9, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "最少图片数量";
            // 
            // GatherCompleteShutdown
            // 
            this.GatherCompleteShutdown.AutoSize = true;
            this.GatherCompleteShutdown.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GatherCompleteShutdown.Location = new System.Drawing.Point(6, 32);
            this.GatherCompleteShutdown.Name = "GatherCompleteShutdown";
            this.GatherCompleteShutdown.Size = new System.Drawing.Size(99, 21);
            this.GatherCompleteShutdown.TabIndex = 35;
            this.GatherCompleteShutdown.Text = "采集完成关机";
            this.GatherCompleteShutdown.UseVisualStyleBackColor = true;
            // 
            // GatherContinueLastPage
            // 
            this.GatherContinueLastPage.AutoSize = true;
            this.GatherContinueLastPage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GatherContinueLastPage.Location = new System.Drawing.Point(119, 111);
            this.GatherContinueLastPage.Name = "GatherContinueLastPage";
            this.GatherContinueLastPage.Size = new System.Drawing.Size(99, 21);
            this.GatherContinueLastPage.TabIndex = 36;
            this.GatherContinueLastPage.Text = "断点续传采集";
            this.GatherContinueLastPage.UseVisualStyleBackColor = true;
            // 
            // ReadUserOfHeFocus
            // 
            this.ReadUserOfHeFocus.AutoSize = true;
            this.ReadUserOfHeFocus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReadUserOfHeFocus.Location = new System.Drawing.Point(120, 57);
            this.ReadUserOfHeFocus.Name = "ReadUserOfHeFocus";
            this.ReadUserOfHeFocus.Size = new System.Drawing.Size(99, 21);
            this.ReadUserOfHeFocus.TabIndex = 37;
            this.ReadUserOfHeFocus.Text = "采集用户关注";
            this.ReadUserOfHeFocus.UseVisualStyleBackColor = true;
            this.ReadUserOfHeFocus.CheckedChanged += new System.EventHandler(this.ReadUserOfHeFocus_CheckedChanged);
            // 
            // IgnoreDownloadSource
            // 
            this.IgnoreDownloadSource.AutoSize = true;
            this.IgnoreDownloadSource.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IgnoreDownloadSource.Location = new System.Drawing.Point(12, 111);
            this.IgnoreDownloadSource.Name = "IgnoreDownloadSource";
            this.IgnoreDownloadSource.Size = new System.Drawing.Size(99, 21);
            this.IgnoreDownloadSource.TabIndex = 38;
            this.IgnoreDownloadSource.Text = "忽略下载资源";
            this.IgnoreDownloadSource.UseVisualStyleBackColor = true;
            // 
            // ReadAllOfUser
            // 
            this.ReadAllOfUser.AutoSize = true;
            this.ReadAllOfUser.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReadAllOfUser.Location = new System.Drawing.Point(120, 30);
            this.ReadAllOfUser.Name = "ReadAllOfUser";
            this.ReadAllOfUser.Size = new System.Drawing.Size(99, 21);
            this.ReadAllOfUser.TabIndex = 39;
            this.ReadAllOfUser.Text = "采集所有用户";
            this.ReadAllOfUser.UseVisualStyleBackColor = true;
            this.ReadAllOfUser.CheckedChanged += new System.EventHandler(this.ReadAllOfUser_CheckedChanged);
            // 
            // ReadOwnerUserStatus
            // 
            this.ReadOwnerUserStatus.AutoSize = true;
            this.ReadOwnerUserStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReadOwnerUserStatus.Location = new System.Drawing.Point(12, 30);
            this.ReadOwnerUserStatus.Name = "ReadOwnerUserStatus";
            this.ReadOwnerUserStatus.Size = new System.Drawing.Size(99, 21);
            this.ReadOwnerUserStatus.TabIndex = 40;
            this.ReadOwnerUserStatus.Text = "采集原创微博";
            this.ReadOwnerUserStatus.UseVisualStyleBackColor = true;
            this.ReadOwnerUserStatus.CheckedChanged += new System.EventHandler(this.ReadOwnerUserStatus_CheckedChanged);
            // 
            // ReadUserOfMyFocus
            // 
            this.ReadUserOfMyFocus.AutoSize = true;
            this.ReadUserOfMyFocus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReadUserOfMyFocus.Location = new System.Drawing.Point(12, 57);
            this.ReadUserOfMyFocus.Name = "ReadUserOfMyFocus";
            this.ReadUserOfMyFocus.Size = new System.Drawing.Size(99, 21);
            this.ReadUserOfMyFocus.TabIndex = 41;
            this.ReadUserOfMyFocus.Text = "采集我的关注";
            this.ReadUserOfMyFocus.UseVisualStyleBackColor = true;
            this.ReadUserOfMyFocus.CheckedChanged += new System.EventHandler(this.ReadUserOfMyFocus_CheckedChanged);
            // 
            // IgnoreReadGetStatus
            // 
            this.IgnoreReadGetStatus.AutoSize = true;
            this.IgnoreReadGetStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IgnoreReadGetStatus.Location = new System.Drawing.Point(120, 84);
            this.IgnoreReadGetStatus.Name = "IgnoreReadGetStatus";
            this.IgnoreReadGetStatus.Size = new System.Drawing.Size(99, 21);
            this.IgnoreReadGetStatus.TabIndex = 42;
            this.IgnoreReadGetStatus.Text = "忽略已采微博";
            this.IgnoreReadGetStatus.UseVisualStyleBackColor = true;
            this.IgnoreReadGetStatus.CheckedChanged += new System.EventHandler(this.IgnoreReadGetStatus_CheckedChanged);
            // 
            // IgnoreReadArchiveStatus
            // 
            this.IgnoreReadArchiveStatus.AutoSize = true;
            this.IgnoreReadArchiveStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IgnoreReadArchiveStatus.Location = new System.Drawing.Point(12, 84);
            this.IgnoreReadArchiveStatus.Name = "IgnoreReadArchiveStatus";
            this.IgnoreReadArchiveStatus.Size = new System.Drawing.Size(99, 21);
            this.IgnoreReadArchiveStatus.TabIndex = 43;
            this.IgnoreReadArchiveStatus.Text = "忽略存档微博";
            this.IgnoreReadArchiveStatus.UseVisualStyleBackColor = true;
            this.IgnoreReadArchiveStatus.CheckedChanged += new System.EventHandler(this.IgnoreReadArchiveStatus_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.GatherCompleteShutdown);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 285);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 59);
            this.panel1.TabIndex = 44;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel2.Controls.Add(this.label5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(248, 26);
            this.panel2.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "其它选项";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.IgnoreReadArchiveStatus);
            this.panel3.Controls.Add(this.GatherContinueLastPage);
            this.panel3.Controls.Add(this.IgnoreReadGetStatus);
            this.panel3.Controls.Add(this.ReadUserOfHeFocus);
            this.panel3.Controls.Add(this.ReadUserOfMyFocus);
            this.panel3.Controls.Add(this.IgnoreDownloadSource);
            this.panel3.Controls.Add(this.ReadOwnerUserStatus);
            this.panel3.Controls.Add(this.ReadAllOfUser);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 149);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(248, 136);
            this.panel3.TabIndex = 45;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel5.Controls.Add(this.label6);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(248, 26);
            this.panel5.TabIndex = 44;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "功能选项";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.GatherThreadCount);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.MinReadImageCount);
            this.panel4.Controls.Add(this.StartPageIndex);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.MaxReadPageCount);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(248, 149);
            this.panel4.TabIndex = 46;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel6.Controls.Add(this.label7);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(248, 26);
            this.panel6.TabIndex = 45;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "基本选项";
            // 
            // SpiderConfigUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Name = "SpiderConfigUC";
            this.Size = new System.Drawing.Size(248, 344);
            ((System.ComponentModel.ISupportInitialize)(this.GatherThreadCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPageIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxReadPageCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinReadImageCount)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown GatherThreadCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown StartPageIndex;
        private System.Windows.Forms.NumericUpDown MaxReadPageCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown MinReadImageCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox GatherCompleteShutdown;
        private System.Windows.Forms.CheckBox GatherContinueLastPage;
        private System.Windows.Forms.CheckBox ReadUserOfHeFocus;
        private System.Windows.Forms.CheckBox IgnoreDownloadSource;
        private System.Windows.Forms.CheckBox ReadAllOfUser;
        private System.Windows.Forms.CheckBox ReadOwnerUserStatus;
        private System.Windows.Forms.CheckBox ReadUserOfMyFocus;
        private System.Windows.Forms.CheckBox IgnoreReadGetStatus;
        private System.Windows.Forms.CheckBox IgnoreReadArchiveStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label7;
    }
}
