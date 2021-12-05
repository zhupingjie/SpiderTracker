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
            this.pnlToolSave = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.spiderConfiguc1 = new SpiderTracker.UI.SpiderConfigUC();
            this.pnlToolSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlToolSave
            // 
            this.pnlToolSave.Controls.Add(this.btnClose);
            this.pnlToolSave.Controls.Add(this.btnSave);
            this.pnlToolSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlToolSave.Location = new System.Drawing.Point(0, 464);
            this.pnlToolSave.Name = "pnlToolSave";
            this.pnlToolSave.Size = new System.Drawing.Size(232, 56);
            this.pnlToolSave.TabIndex = 1;
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
            this.spiderConfiguc1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spiderConfiguc1.Location = new System.Drawing.Point(0, 0);
            this.spiderConfiguc1.Margin = new System.Windows.Forms.Padding(4);
            this.spiderConfiguc1.Name = "spiderConfiguc1";
            this.spiderConfiguc1.Size = new System.Drawing.Size(232, 464);
            this.spiderConfiguc1.TabIndex = 2;
            // 
            // ServiceConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 520);
            this.Controls.Add(this.spiderConfiguc1);
            this.Controls.Add(this.pnlToolSave);
            this.Name = "ServiceConfigForm";
            this.Text = "服务设置项";
            this.Load += new System.EventHandler(this.ServiceConfigForm_Load);
            this.pnlToolSave.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlToolSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private SpiderConfigUC spiderConfiguc1;
    }
}