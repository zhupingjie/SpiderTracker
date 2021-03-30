namespace SpiderTracker.UI
{
    partial class ViewImgForm
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
            this.imageCtl = new System.Windows.Forms.Panel();
            this.imageRight = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.imageLeft = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.imageRight.SuspendLayout();
            this.imageLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageCtl
            // 
            this.imageCtl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imageCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageCtl.Location = new System.Drawing.Point(50, 0);
            this.imageCtl.Name = "imageCtl";
            this.imageCtl.Size = new System.Drawing.Size(804, 621);
            this.imageCtl.TabIndex = 1;
            // 
            // imageRight
            // 
            this.imageRight.BackColor = System.Drawing.Color.White;
            this.imageRight.Controls.Add(this.label1);
            this.imageRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.imageRight.Location = new System.Drawing.Point(854, 0);
            this.imageRight.Name = "imageRight";
            this.imageRight.Size = new System.Drawing.Size(50, 621);
            this.imageRight.TabIndex = 2;
            this.imageRight.Click += new System.EventHandler(this.imageRight_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 276);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = ">";
            // 
            // imageLeft
            // 
            this.imageLeft.BackColor = System.Drawing.Color.White;
            this.imageLeft.Controls.Add(this.label2);
            this.imageLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.imageLeft.Location = new System.Drawing.Point(0, 0);
            this.imageLeft.Name = "imageLeft";
            this.imageLeft.Size = new System.Drawing.Size(50, 621);
            this.imageLeft.TabIndex = 3;
            this.imageLeft.Click += new System.EventHandler(this.imageLeft_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "<";
            // 
            // ViewImgForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(904, 621);
            this.Controls.Add(this.imageCtl);
            this.Controls.Add(this.imageLeft);
            this.Controls.Add(this.imageRight);
            this.Name = "ViewImgForm";
            this.Text = "查看原图";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewImgForm_FormClosing);
            this.Load += new System.EventHandler(this.ViewImgForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewImgForm_KeyDown);
            this.imageRight.ResumeLayout(false);
            this.imageRight.PerformLayout();
            this.imageLeft.ResumeLayout(false);
            this.imageLeft.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel imageCtl;
        private System.Windows.Forms.Panel imageRight;
        private System.Windows.Forms.Panel imageLeft;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}