namespace SpiderTracker.UI
{
    partial class ServiceConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.spiderConfiguc1 = new SpiderTracker.UI.SpiderConfigUC();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.GatherUserDataServiceInterval = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.GatherUserDataSortAsc = new System.Windows.Forms.ComboBox();
            this.GatherUserDataSort = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GatherUserDataServiceInterval)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 464);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 56);
            this.panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 37);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(119, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 37);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // spiderConfiguc1
            // 
            this.spiderConfiguc1.Dock = System.Windows.Forms.DockStyle.Top;
            this.spiderConfiguc1.Location = new System.Drawing.Point(0, 0);
            this.spiderConfiguc1.Margin = new System.Windows.Forms.Padding(4);
            this.spiderConfiguc1.Name = "spiderConfiguc1";
            this.spiderConfiguc1.Size = new System.Drawing.Size(240, 368);
            this.spiderConfiguc1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.GatherUserDataSortAsc);
            this.panel2.Controls.Add(this.GatherUserDataSort);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.GatherUserDataServiceInterval);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 368);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 96);
            this.panel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "运行周期时间(分)";
            // 
            // GatherUserDataServiceInterval
            // 
            this.GatherUserDataServiceInterval.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherUserDataServiceInterval.Location = new System.Drawing.Point(119, 31);
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
            this.GatherUserDataServiceInterval.TabIndex = 3;
            this.GatherUserDataServiceInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel3.Controls.Add(this.label6);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(240, 24);
            this.panel3.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 4);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "服务选项";
            // 
            // GatherUserDataSortAsc
            // 
            this.GatherUserDataSortAsc.BackColor = System.Drawing.Color.White;
            this.GatherUserDataSortAsc.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GatherUserDataSortAsc.FormattingEnabled = true;
            this.GatherUserDataSortAsc.Items.AddRange(new object[] {
            "升序",
            "降序"});
            this.GatherUserDataSortAsc.Location = new System.Drawing.Point(173, 61);
            this.GatherUserDataSortAsc.Name = "GatherUserDataSortAsc";
            this.GatherUserDataSortAsc.Size = new System.Drawing.Size(45, 24);
            this.GatherUserDataSortAsc.TabIndex = 5;
            this.GatherUserDataSortAsc.Text = "降序";
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
            this.GatherUserDataSort.Location = new System.Drawing.Point(119, 61);
            this.GatherUserDataSort.Margin = new System.Windows.Forms.Padding(0);
            this.GatherUserDataSort.Name = "GatherUserDataSort";
            this.GatherUserDataSort.Size = new System.Drawing.Size(51, 24);
            this.GatherUserDataSort.TabIndex = 4;
            this.GatherUserDataSort.Text = "发布";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "采集用户优先级";
            // 
            // ServiceConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 520);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.spiderConfiguc1);
            this.Controls.Add(this.panel1);
            this.Name = "ServiceConfigForm";
            this.Text = "服务设置项";
            this.Load += new System.EventHandler(this.ServiceConfigForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GatherUserDataServiceInterval)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private SpiderConfigUC spiderConfiguc1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown GatherUserDataServiceInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox GatherUserDataSortAsc;
        private System.Windows.Forms.ComboBox GatherUserDataSort;
    }
}