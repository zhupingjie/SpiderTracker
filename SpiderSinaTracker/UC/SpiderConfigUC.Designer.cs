
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
            this.GatherUserNewPublishTime = new System.Windows.Forms.CheckBox();
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
            this.IgnoreReadDownStatus = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.GatherStatusUpdateLocalSource = new System.Windows.Forms.CheckBox();
            this.GatherStatusWithNoSource = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.GatherUserDataServiceInterval = new System.Windows.Forms.NumericUpDown();
            this.lblGatherUserDataServiceInterval = new System.Windows.Forms.Label();
            this.GatherUserDataSort = new System.Windows.Forms.ComboBox();
            this.GatherUserDataSortAsc = new System.Windows.Forms.ComboBox();
            this.lblGatherUserDataSort = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
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
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GatherUserDataServiceInterval)).BeginInit();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(10, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "并发用户数量";
            // 
            // GatherThreadCount
            // 
            this.GatherThreadCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherThreadCount.Location = new System.Drawing.Point(124, 31);
            this.GatherThreadCount.Margin = new System.Windows.Forms.Padding(4);
            this.GatherThreadCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.GatherThreadCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GatherThreadCount.Name = "GatherThreadCount";
            this.GatherThreadCount.Size = new System.Drawing.Size(99, 23);
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
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(10, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "起始采集页码";
            // 
            // StartPageIndex
            // 
            this.StartPageIndex.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartPageIndex.Location = new System.Drawing.Point(124, 58);
            this.StartPageIndex.Margin = new System.Windows.Forms.Padding(4);
            this.StartPageIndex.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.StartPageIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.StartPageIndex.Name = "StartPageIndex";
            this.StartPageIndex.Size = new System.Drawing.Size(99, 23);
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
            this.MaxReadPageCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MaxReadPageCount.Location = new System.Drawing.Point(124, 85);
            this.MaxReadPageCount.Margin = new System.Windows.Forms.Padding(4);
            this.MaxReadPageCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.MaxReadPageCount.Name = "MaxReadPageCount";
            this.MaxReadPageCount.Size = new System.Drawing.Size(99, 23);
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
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(10, 91);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "最大采集页数";
            // 
            // MinReadImageCount
            // 
            this.MinReadImageCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MinReadImageCount.Location = new System.Drawing.Point(124, 115);
            this.MinReadImageCount.Margin = new System.Windows.Forms.Padding(4);
            this.MinReadImageCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MinReadImageCount.Name = "MinReadImageCount";
            this.MinReadImageCount.Size = new System.Drawing.Size(99, 23);
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
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(10, 116);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "最少图片数量[*]";
            // 
            // GatherUserNewPublishTime
            // 
            this.GatherUserNewPublishTime.AutoSize = true;
            this.GatherUserNewPublishTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherUserNewPublishTime.Location = new System.Drawing.Point(121, 32);
            this.GatherUserNewPublishTime.Margin = new System.Windows.Forms.Padding(4);
            this.GatherUserNewPublishTime.Name = "GatherUserNewPublishTime";
            this.GatherUserNewPublishTime.Size = new System.Drawing.Size(99, 21);
            this.GatherUserNewPublishTime.TabIndex = 35;
            this.GatherUserNewPublishTime.Text = "采集更新时间";
            this.GatherUserNewPublishTime.UseVisualStyleBackColor = true;
            // 
            // GatherContinueLastPage
            // 
            this.GatherContinueLastPage.AutoSize = true;
            this.GatherContinueLastPage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherContinueLastPage.Location = new System.Drawing.Point(121, 58);
            this.GatherContinueLastPage.Margin = new System.Windows.Forms.Padding(4);
            this.GatherContinueLastPage.Name = "GatherContinueLastPage";
            this.GatherContinueLastPage.Size = new System.Drawing.Size(99, 21);
            this.GatherContinueLastPage.TabIndex = 36;
            this.GatherContinueLastPage.Text = "采集起始页码";
            this.GatherContinueLastPage.UseVisualStyleBackColor = true;
            // 
            // ReadUserOfHeFocus
            // 
            this.ReadUserOfHeFocus.AutoSize = true;
            this.ReadUserOfHeFocus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ReadUserOfHeFocus.Location = new System.Drawing.Point(14, 59);
            this.ReadUserOfHeFocus.Margin = new System.Windows.Forms.Padding(4);
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
            this.IgnoreDownloadSource.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreDownloadSource.Location = new System.Drawing.Point(121, 86);
            this.IgnoreDownloadSource.Margin = new System.Windows.Forms.Padding(4);
            this.IgnoreDownloadSource.Name = "IgnoreDownloadSource";
            this.IgnoreDownloadSource.Size = new System.Drawing.Size(99, 21);
            this.IgnoreDownloadSource.TabIndex = 38;
            this.IgnoreDownloadSource.Text = "忽略下载资源";
            this.IgnoreDownloadSource.UseVisualStyleBackColor = true;
            // 
            // ReadAllOfUser
            // 
            this.ReadAllOfUser.AutoSize = true;
            this.ReadAllOfUser.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ReadAllOfUser.Location = new System.Drawing.Point(14, 33);
            this.ReadAllOfUser.Margin = new System.Windows.Forms.Padding(4);
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
            this.ReadOwnerUserStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ReadOwnerUserStatus.Location = new System.Drawing.Point(14, 32);
            this.ReadOwnerUserStatus.Margin = new System.Windows.Forms.Padding(4);
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
            this.ReadUserOfMyFocus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ReadUserOfMyFocus.Location = new System.Drawing.Point(121, 33);
            this.ReadUserOfMyFocus.Margin = new System.Windows.Forms.Padding(4);
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
            this.IgnoreReadGetStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreReadGetStatus.Location = new System.Drawing.Point(121, 57);
            this.IgnoreReadGetStatus.Margin = new System.Windows.Forms.Padding(4);
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
            this.IgnoreReadArchiveStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreReadArchiveStatus.Location = new System.Drawing.Point(14, 57);
            this.IgnoreReadArchiveStatus.Margin = new System.Windows.Forms.Padding(4);
            this.IgnoreReadArchiveStatus.Name = "IgnoreReadArchiveStatus";
            this.IgnoreReadArchiveStatus.Size = new System.Drawing.Size(99, 21);
            this.IgnoreReadArchiveStatus.TabIndex = 43;
            this.IgnoreReadArchiveStatus.Text = "忽略存档微博";
            this.IgnoreReadArchiveStatus.UseVisualStyleBackColor = true;
            this.IgnoreReadArchiveStatus.CheckedChanged += new System.EventHandler(this.IgnoreReadArchiveStatus_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.IgnoreDownloadSource);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.IgnoreReadGetStatus);
            this.panel1.Controls.Add(this.GatherUserNewPublishTime);
            this.panel1.Controls.Add(this.ReadOwnerUserStatus);
            this.panel1.Controls.Add(this.IgnoreReadDownStatus);
            this.panel1.Controls.Add(this.IgnoreReadArchiveStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 260);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 215);
            this.panel1.TabIndex = 44;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel2.Controls.Add(this.label5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(248, 24);
            this.panel2.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 6);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "运行选项";
            // 
            // IgnoreReadDownStatus
            // 
            this.IgnoreReadDownStatus.AutoSize = true;
            this.IgnoreReadDownStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreReadDownStatus.Location = new System.Drawing.Point(14, 86);
            this.IgnoreReadDownStatus.Margin = new System.Windows.Forms.Padding(4);
            this.IgnoreReadDownStatus.Name = "IgnoreReadDownStatus";
            this.IgnoreReadDownStatus.Size = new System.Drawing.Size(99, 21);
            this.IgnoreReadDownStatus.TabIndex = 45;
            this.IgnoreReadDownStatus.Text = "忽略本地资源";
            this.IgnoreReadDownStatus.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.GatherStatusUpdateLocalSource);
            this.panel3.Controls.Add(this.ReadAllOfUser);
            this.panel3.Controls.Add(this.GatherStatusWithNoSource);
            this.panel3.Controls.Add(this.GatherContinueLastPage);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.ReadUserOfHeFocus);
            this.panel3.Controls.Add(this.ReadUserOfMyFocus);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 146);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(248, 114);
            this.panel3.TabIndex = 45;
            // 
            // GatherStatusUpdateLocalSource
            // 
            this.GatherStatusUpdateLocalSource.AutoSize = true;
            this.GatherStatusUpdateLocalSource.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherStatusUpdateLocalSource.Location = new System.Drawing.Point(121, 85);
            this.GatherStatusUpdateLocalSource.Margin = new System.Windows.Forms.Padding(4);
            this.GatherStatusUpdateLocalSource.Name = "GatherStatusUpdateLocalSource";
            this.GatherStatusUpdateLocalSource.Size = new System.Drawing.Size(99, 21);
            this.GatherStatusUpdateLocalSource.TabIndex = 47;
            this.GatherStatusUpdateLocalSource.Text = "同步本地资源";
            this.GatherStatusUpdateLocalSource.UseVisualStyleBackColor = true;
            // 
            // GatherStatusWithNoSource
            // 
            this.GatherStatusWithNoSource.AutoSize = true;
            this.GatherStatusWithNoSource.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherStatusWithNoSource.Location = new System.Drawing.Point(14, 85);
            this.GatherStatusWithNoSource.Margin = new System.Windows.Forms.Padding(4);
            this.GatherStatusWithNoSource.Name = "GatherStatusWithNoSource";
            this.GatherStatusWithNoSource.Size = new System.Drawing.Size(99, 21);
            this.GatherStatusWithNoSource.TabIndex = 46;
            this.GatherStatusWithNoSource.Text = "采集已读微博";
            this.GatherStatusWithNoSource.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel5.Controls.Add(this.label6);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(248, 25);
            this.panel5.TabIndex = 44;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 6);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "启动选项";
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
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(248, 146);
            this.panel4.TabIndex = 46;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel6.Controls.Add(this.label7);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(4);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(248, 25);
            this.panel6.TabIndex = 45;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 6);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "基本选项";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel8.Controls.Add(this.label8);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(4);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(248, 24);
            this.panel8.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 6);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 17);
            this.label8.TabIndex = 0;
            this.label8.Text = "其它选项";
            // 
            // GatherUserDataServiceInterval
            // 
            this.GatherUserDataServiceInterval.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherUserDataServiceInterval.Location = new System.Drawing.Point(121, 36);
            this.GatherUserDataServiceInterval.Margin = new System.Windows.Forms.Padding(4);
            this.GatherUserDataServiceInterval.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.GatherUserDataServiceInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GatherUserDataServiceInterval.Name = "GatherUserDataServiceInterval";
            this.GatherUserDataServiceInterval.Size = new System.Drawing.Size(99, 23);
            this.GatherUserDataServiceInterval.TabIndex = 8;
            this.GatherUserDataServiceInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.GatherUserDataServiceInterval.ValueChanged += new System.EventHandler(this.GatherUserDataServiceInterval_ValueChanged);
            // 
            // lblGatherUserDataServiceInterval
            // 
            this.lblGatherUserDataServiceInterval.AutoSize = true;
            this.lblGatherUserDataServiceInterval.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblGatherUserDataServiceInterval.Location = new System.Drawing.Point(14, 38);
            this.lblGatherUserDataServiceInterval.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGatherUserDataServiceInterval.Name = "lblGatherUserDataServiceInterval";
            this.lblGatherUserDataServiceInterval.Size = new System.Drawing.Size(100, 17);
            this.lblGatherUserDataServiceInterval.TabIndex = 7;
            this.lblGatherUserDataServiceInterval.Text = "运行周期时间(分)";
            // 
            // GatherUserDataSort
            // 
            this.GatherUserDataSort.BackColor = System.Drawing.Color.White;
            this.GatherUserDataSort.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherUserDataSort.FormattingEnabled = true;
            this.GatherUserDataSort.Items.AddRange(new object[] {
            "发布",
            "更新",
            "用户",
            "名称",
            "微博",
            "读取",
            "采集",
            "上传",
            "忽略",
            "原创",
            "转发",
            "关注",
            "页码",
            "点赞",
            "来源"});
            this.GatherUserDataSort.Location = new System.Drawing.Point(121, 66);
            this.GatherUserDataSort.Margin = new System.Windows.Forms.Padding(0);
            this.GatherUserDataSort.Name = "GatherUserDataSort";
            this.GatherUserDataSort.Size = new System.Drawing.Size(51, 24);
            this.GatherUserDataSort.TabIndex = 9;
            this.GatherUserDataSort.Text = "发布";
            this.GatherUserDataSort.SelectedIndexChanged += new System.EventHandler(this.GatherUserDataSort_SelectedIndexChanged);
            // 
            // GatherUserDataSortAsc
            // 
            this.GatherUserDataSortAsc.BackColor = System.Drawing.Color.White;
            this.GatherUserDataSortAsc.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherUserDataSortAsc.FormattingEnabled = true;
            this.GatherUserDataSortAsc.Items.AddRange(new object[] {
            "升序",
            "降序"});
            this.GatherUserDataSortAsc.Location = new System.Drawing.Point(175, 66);
            this.GatherUserDataSortAsc.Name = "GatherUserDataSortAsc";
            this.GatherUserDataSortAsc.Size = new System.Drawing.Size(45, 24);
            this.GatherUserDataSortAsc.TabIndex = 10;
            this.GatherUserDataSortAsc.Text = "降序";
            this.GatherUserDataSortAsc.SelectedIndexChanged += new System.EventHandler(this.GatherUserDataSortAsc_SelectedIndexChanged);
            // 
            // lblGatherUserDataSort
            // 
            this.lblGatherUserDataSort.AutoSize = true;
            this.lblGatherUserDataSort.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblGatherUserDataSort.Location = new System.Drawing.Point(14, 68);
            this.lblGatherUserDataSort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGatherUserDataSort.Name = "lblGatherUserDataSort";
            this.lblGatherUserDataSort.Size = new System.Drawing.Size(92, 17);
            this.lblGatherUserDataSort.TabIndex = 11;
            this.lblGatherUserDataSort.Text = "采集用户优先级";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.lblGatherUserDataSort);
            this.panel7.Controls.Add(this.GatherUserDataSortAsc);
            this.panel7.Controls.Add(this.GatherUserDataSort);
            this.panel7.Controls.Add(this.lblGatherUserDataServiceInterval);
            this.panel7.Controls.Add(this.GatherUserDataServiceInterval);
            this.panel7.Controls.Add(this.panel8);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 117);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(248, 98);
            this.panel7.TabIndex = 46;
            // 
            // SpiderConfigUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SpiderConfigUC";
            this.Size = new System.Drawing.Size(248, 475);
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
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GatherUserDataServiceInterval)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
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
        private System.Windows.Forms.CheckBox GatherUserNewPublishTime;
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
        private System.Windows.Forms.CheckBox IgnoreReadDownStatus;
        private System.Windows.Forms.CheckBox GatherStatusWithNoSource;
        private System.Windows.Forms.CheckBox GatherStatusUpdateLocalSource;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lblGatherUserDataSort;
        private System.Windows.Forms.ComboBox GatherUserDataSortAsc;
        private System.Windows.Forms.ComboBox GatherUserDataSort;
        private System.Windows.Forms.Label lblGatherUserDataServiceInterval;
        private System.Windows.Forms.NumericUpDown GatherUserDataServiceInterval;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label8;
    }
}
