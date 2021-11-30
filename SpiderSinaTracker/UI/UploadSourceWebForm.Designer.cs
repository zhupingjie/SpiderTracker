namespace SpiderTracker.UI
{
    partial class UploadSourceWebForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UploadSourceWebForm));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.txtWebUrl = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGO = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 26);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(23, 28);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1026, 617);
            this.webBrowser1.TabIndex = 1;
            // 
            // txtWebUrl
            // 
            this.txtWebUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWebUrl.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtWebUrl.Location = new System.Drawing.Point(0, 0);
            this.txtWebUrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtWebUrl.Name = "txtWebUrl";
            this.txtWebUrl.Size = new System.Drawing.Size(974, 26);
            this.txtWebUrl.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtWebUrl);
            this.panel1.Controls.Add(this.btnGO);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1026, 26);
            this.panel1.TabIndex = 3;
            // 
            // btnGO
            // 
            this.btnGO.BackColor = System.Drawing.Color.White;
            this.btnGO.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnGO.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnGO.Location = new System.Drawing.Point(974, 0);
            this.btnGO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGO.Name = "btnGO";
            this.btnGO.Size = new System.Drawing.Size(52, 26);
            this.btnGO.TabIndex = 3;
            this.btnGO.Text = "GO";
            this.btnGO.UseVisualStyleBackColor = false;
            this.btnGO.Click += new System.EventHandler(this.btnGO_Click);
            // 
            // UploadSourceWebForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 643);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "UploadSourceWebForm";
            this.Text = "显示上传资源";
            this.Load += new System.EventHandler(this.UploadSourceWebForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TextBox txtWebUrl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnGO;
    }
}