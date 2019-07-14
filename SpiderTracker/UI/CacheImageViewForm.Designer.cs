namespace SpiderTracker.UI
{
    partial class CacheImageViewForm
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
            this.components = new System.ComponentModel.Container();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lstImageStatus = new System.Windows.Forms.ListView();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lstImageGroup = new System.Windows.Forms.ListView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.pnlTop = new System.Windows.Forms.Panel();
            this.benOpen = new System.Windows.Forms.Button();
            this.btnFollower = new System.Windows.Forms.Button();
            this.btnBrowseUser = new System.Windows.Forms.Button();
            this.btnBrowseStatus = new System.Windows.Forms.Button();
            this.btnFocusUser = new System.Windows.Forms.Button();
            this.btnFocusStatus = new System.Windows.Forms.Button();
            this.btnIgnoreUser = new System.Windows.Forms.Button();
            this.btnIgnoreStatus = new System.Windows.Forms.Button();
            this.cbxName = new System.Windows.Forms.ComboBox();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlSearchUser = new System.Windows.Forms.Panel();
            this.lstUser = new System.Windows.Forms.ListBox();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblUserCount = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlBottom.SuspendLayout();
            this.panel11.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlSearchUser.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.lstImageStatus);
            this.pnlBottom.Controls.Add(this.panel11);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(172, 338);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(800, 202);
            this.pnlBottom.TabIndex = 1;
            // 
            // lstImageStatus
            // 
            this.lstImageStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstImageStatus.Location = new System.Drawing.Point(0, 26);
            this.lstImageStatus.Name = "lstImageStatus";
            this.lstImageStatus.Size = new System.Drawing.Size(800, 176);
            this.lstImageStatus.TabIndex = 0;
            this.lstImageStatus.UseCompatibleStateImageBehavior = false;
            this.lstImageStatus.SelectedIndexChanged += new System.EventHandler(this.lstImageStatus_SelectedIndexChanged);
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel11.Controls.Add(this.label15);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(800, 26);
            this.panel11.TabIndex = 13;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label15.Location = new System.Drawing.Point(4, 4);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 17);
            this.label15.TabIndex = 0;
            this.label15.Text = "用户图集详细";
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(172, 66);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 272);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lstImageGroup);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(774, 264);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "图集预览";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lstImageGroup
            // 
            this.lstImageGroup.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstImageGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstImageGroup.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstImageGroup.GridLines = true;
            this.lstImageGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstImageGroup.Location = new System.Drawing.Point(3, 3);
            this.lstImageGroup.Margin = new System.Windows.Forms.Padding(0);
            this.lstImageGroup.MultiSelect = false;
            this.lstImageGroup.Name = "lstImageGroup";
            this.lstImageGroup.ShowGroups = false;
            this.lstImageGroup.Size = new System.Drawing.Size(768, 258);
            this.lstImageGroup.TabIndex = 0;
            this.lstImageGroup.UseCompatibleStateImageBehavior = false;
            this.lstImageGroup.SelectedIndexChanged += new System.EventHandler(this.lstImageGroup_SelectedIndexChanged);
            this.lstImageGroup.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstImageGroup_KeyPress);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(774, 264);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "详图预览";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(768, 258);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(200, 200);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(100, 100);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.benOpen);
            this.pnlTop.Controls.Add(this.btnFollower);
            this.pnlTop.Controls.Add(this.btnBrowseUser);
            this.pnlTop.Controls.Add(this.btnBrowseStatus);
            this.pnlTop.Controls.Add(this.btnFocusUser);
            this.pnlTop.Controls.Add(this.btnFocusStatus);
            this.pnlTop.Controls.Add(this.btnIgnoreUser);
            this.pnlTop.Controls.Add(this.btnIgnoreStatus);
            this.pnlTop.Controls.Add(this.cbxName);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(972, 40);
            this.pnlTop.TabIndex = 3;
            // 
            // benOpen
            // 
            this.benOpen.BackColor = System.Drawing.Color.Magenta;
            this.benOpen.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.benOpen.ForeColor = System.Drawing.Color.White;
            this.benOpen.Location = new System.Drawing.Point(466, 2);
            this.benOpen.Name = "benOpen";
            this.benOpen.Size = new System.Drawing.Size(75, 35);
            this.benOpen.TabIndex = 19;
            this.benOpen.Text = "本地目录";
            this.benOpen.UseVisualStyleBackColor = false;
            this.benOpen.Click += new System.EventHandler(this.benOpen_Click);
            // 
            // btnFollower
            // 
            this.btnFollower.BackColor = System.Drawing.Color.Magenta;
            this.btnFollower.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFollower.ForeColor = System.Drawing.Color.White;
            this.btnFollower.Location = new System.Drawing.Point(543, 2);
            this.btnFollower.Name = "btnFollower";
            this.btnFollower.Size = new System.Drawing.Size(75, 35);
            this.btnFollower.TabIndex = 18;
            this.btnFollower.Text = "他的关注";
            this.btnFollower.UseVisualStyleBackColor = false;
            this.btnFollower.Click += new System.EventHandler(this.btnFollower_Click);
            // 
            // btnBrowseUser
            // 
            this.btnBrowseUser.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.btnBrowseUser.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowseUser.ForeColor = System.Drawing.Color.White;
            this.btnBrowseUser.Location = new System.Drawing.Point(314, 2);
            this.btnBrowseUser.Name = "btnBrowseUser";
            this.btnBrowseUser.Size = new System.Drawing.Size(75, 35);
            this.btnBrowseUser.TabIndex = 17;
            this.btnBrowseUser.Text = "浏览用户";
            this.btnBrowseUser.UseVisualStyleBackColor = false;
            this.btnBrowseUser.Click += new System.EventHandler(this.btnBrowseUser_Click);
            // 
            // btnBrowseStatus
            // 
            this.btnBrowseStatus.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.btnBrowseStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowseStatus.ForeColor = System.Drawing.Color.White;
            this.btnBrowseStatus.Location = new System.Drawing.Point(389, 2);
            this.btnBrowseStatus.Name = "btnBrowseStatus";
            this.btnBrowseStatus.Size = new System.Drawing.Size(75, 35);
            this.btnBrowseStatus.TabIndex = 16;
            this.btnBrowseStatus.Text = "浏览图集";
            this.btnBrowseStatus.UseVisualStyleBackColor = false;
            this.btnBrowseStatus.Click += new System.EventHandler(this.btnBrowseStatus_Click);
            // 
            // btnFocusUser
            // 
            this.btnFocusUser.BackColor = System.Drawing.Color.DarkOrange;
            this.btnFocusUser.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFocusUser.ForeColor = System.Drawing.Color.White;
            this.btnFocusUser.Location = new System.Drawing.Point(158, 3);
            this.btnFocusUser.Name = "btnFocusUser";
            this.btnFocusUser.Size = new System.Drawing.Size(75, 35);
            this.btnFocusUser.TabIndex = 15;
            this.btnFocusUser.Text = "关注用户";
            this.btnFocusUser.UseVisualStyleBackColor = false;
            this.btnFocusUser.Click += new System.EventHandler(this.btnFocusUser_Click);
            // 
            // btnFocusStatus
            // 
            this.btnFocusStatus.BackColor = System.Drawing.Color.DarkOrange;
            this.btnFocusStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFocusStatus.ForeColor = System.Drawing.Color.White;
            this.btnFocusStatus.Location = new System.Drawing.Point(233, 3);
            this.btnFocusStatus.Name = "btnFocusStatus";
            this.btnFocusStatus.Size = new System.Drawing.Size(75, 35);
            this.btnFocusStatus.TabIndex = 14;
            this.btnFocusStatus.Text = "关注图集";
            this.btnFocusStatus.UseVisualStyleBackColor = false;
            this.btnFocusStatus.Click += new System.EventHandler(this.btnFocusStatus_Click);
            // 
            // btnIgnoreUser
            // 
            this.btnIgnoreUser.BackColor = System.Drawing.Color.Firebrick;
            this.btnIgnoreUser.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIgnoreUser.ForeColor = System.Drawing.Color.White;
            this.btnIgnoreUser.Location = new System.Drawing.Point(2, 3);
            this.btnIgnoreUser.Name = "btnIgnoreUser";
            this.btnIgnoreUser.Size = new System.Drawing.Size(75, 35);
            this.btnIgnoreUser.TabIndex = 13;
            this.btnIgnoreUser.Text = "拉黑用户";
            this.btnIgnoreUser.UseVisualStyleBackColor = false;
            this.btnIgnoreUser.Click += new System.EventHandler(this.btnIgnoreUser_Click);
            // 
            // btnIgnoreStatus
            // 
            this.btnIgnoreStatus.BackColor = System.Drawing.Color.Firebrick;
            this.btnIgnoreStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIgnoreStatus.ForeColor = System.Drawing.Color.White;
            this.btnIgnoreStatus.Location = new System.Drawing.Point(77, 3);
            this.btnIgnoreStatus.Name = "btnIgnoreStatus";
            this.btnIgnoreStatus.Size = new System.Drawing.Size(75, 35);
            this.btnIgnoreStatus.TabIndex = 12;
            this.btnIgnoreStatus.Text = "拉黑图集";
            this.btnIgnoreStatus.UseVisualStyleBackColor = false;
            this.btnIgnoreStatus.Click += new System.EventHandler(this.btnIgnoreStatus_Click);
            // 
            // cbxName
            // 
            this.cbxName.BackColor = System.Drawing.SystemColors.Control;
            this.cbxName.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbxName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxName.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxName.FormattingEnabled = true;
            this.cbxName.ItemHeight = 31;
            this.cbxName.Items.AddRange(new object[] {
            ""});
            this.cbxName.Location = new System.Drawing.Point(818, 0);
            this.cbxName.Name = "cbxName";
            this.cbxName.Size = new System.Drawing.Size(154, 39);
            this.cbxName.TabIndex = 4;
            this.cbxName.Leave += new System.EventHandler(this.cbxName_Leave);
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.pnlSearchUser);
            this.pnlLeft.Controls.Add(this.panel6);
            this.pnlLeft.Controls.Add(this.panel13);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 40);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(172, 500);
            this.pnlLeft.TabIndex = 5;
            // 
            // pnlSearchUser
            // 
            this.pnlSearchUser.Controls.Add(this.lstUser);
            this.pnlSearchUser.Controls.Add(this.txtKeyword);
            this.pnlSearchUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearchUser.Location = new System.Drawing.Point(0, 26);
            this.pnlSearchUser.Name = "pnlSearchUser";
            this.pnlSearchUser.Size = new System.Drawing.Size(172, 433);
            this.pnlSearchUser.TabIndex = 18;
            // 
            // lstUser
            // 
            this.lstUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUser.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstUser.FormattingEnabled = true;
            this.lstUser.ItemHeight = 19;
            this.lstUser.Location = new System.Drawing.Point(0, 25);
            this.lstUser.Name = "lstUser";
            this.lstUser.Size = new System.Drawing.Size(172, 408);
            this.lstUser.TabIndex = 0;
            this.lstUser.SelectedIndexChanged += new System.EventHandler(this.lstUser_SelectedIndexChanged);
            this.lstUser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstUser_KeyPress);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtKeyword.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtKeyword.Location = new System.Drawing.Point(0, 0);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(172, 25);
            this.txtKeyword.TabIndex = 0;
            this.txtKeyword.TextChanged += new System.EventHandler(this.txtKeyword_TextChanged);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.LightCyan;
            this.panel6.Controls.Add(this.lblUserCount);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 459);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(172, 41);
            this.panel6.TabIndex = 17;
            // 
            // lblUserCount
            // 
            this.lblUserCount.AutoSize = true;
            this.lblUserCount.Location = new System.Drawing.Point(3, 16);
            this.lblUserCount.Name = "lblUserCount";
            this.lblUserCount.Size = new System.Drawing.Size(95, 12);
            this.lblUserCount.TabIndex = 0;
            this.lblUserCount.Text = "【共 0 个用户】";
            // 
            // panel13
            // 
            this.panel13.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.panel13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel13.Controls.Add(this.label1);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel13.Location = new System.Drawing.Point(0, 0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(172, 26);
            this.panel13.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "已缓存用户集";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(172, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 26);
            this.panel1.TabIndex = 17;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.progressBar1.Location = new System.Drawing.Point(619, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(179, 24);
            this.progressBar1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(1, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "用户图集预览";
            // 
            // CacheImageViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 540);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlTop);
            this.Name = "CacheImageViewForm";
            this.Text = "本地缓存图集预览";
            this.Load += new System.EventHandler(this.CacheImageViewForm_Load);
            this.pnlBottom.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlSearchUser.ResumeLayout(false);
            this.pnlSearchUser.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView lstImageGroup;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListView lstImageStatus;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.ListBox lstUser;
        private System.Windows.Forms.ComboBox cbxName;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lblUserCount;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnIgnoreStatus;
        private System.Windows.Forms.Button btnIgnoreUser;
        private System.Windows.Forms.Button btnFocusUser;
        private System.Windows.Forms.Button btnFocusStatus;
        private System.Windows.Forms.Button btnBrowseUser;
        private System.Windows.Forms.Button btnBrowseStatus;
        private System.Windows.Forms.Panel pnlSearchUser;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnFollower;
        private System.Windows.Forms.Button benOpen;
    }
}