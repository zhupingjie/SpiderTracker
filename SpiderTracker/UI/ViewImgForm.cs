using SpiderTracker.Imp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class ViewImgForm : Form
    {
        public ViewImgForm()
        {
            InitializeComponent();
        }

        public List<string> ViewImgPaths { get; set; }
        public int ViewImgIndex { get; set; }

        private void ViewImgForm_Load(object sender, EventArgs e)
        {
            if (ViewImgPaths != null && ViewImgPaths.Count > 0 && ViewImgIndex < ViewImgPaths.Count)
            {
                if (this.imageCtl.BackgroundImage != null)
                {
                    this.imageCtl.BackgroundImage.Dispose();
                    this.imageCtl.BackgroundImage = null;
                    this.imageCtl.Tag = null;
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.BackgroundImage = image;
                this.imageCtl.Tag = ViewImgPaths[ViewImgIndex];
            }
        }

        private void imageLeft_Click(object sender, EventArgs e)
        {
            ViewImgIndex--;
            if (ViewImgIndex < 0) ViewImgIndex = 0;
            if (ViewImgPaths != null && ViewImgPaths.Count > 0 && ViewImgIndex < ViewImgPaths.Count)
            {
                if (this.imageCtl.BackgroundImage != null)
                {
                    this.imageCtl.BackgroundImage.Dispose();
                    this.imageCtl.BackgroundImage = null;
                    this.imageCtl.Tag = null;
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.BackgroundImage = image;
                this.imageCtl.Tag = ViewImgPaths[ViewImgIndex];
            }
        }

        private void imageRight_Click(object sender, EventArgs e)
        {
            ViewImgIndex++;
            if (ViewImgIndex >= ViewImgPaths.Count) ViewImgIndex = ViewImgPaths.Count - 1;
            if (ViewImgPaths != null && ViewImgPaths.Count > 0 && ViewImgIndex < ViewImgPaths.Count)
            {
                if (this.imageCtl.BackgroundImage != null)
                {
                    this.imageCtl.BackgroundImage.Dispose();
                    this.imageCtl.BackgroundImage = null;
                    this.imageCtl.Tag = null;
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.BackgroundImage = image;
                this.imageCtl.Tag = ViewImgPaths[ViewImgIndex];
            }
        }

        void DeleteImage()
        {
            if (this.imageCtl.Tag != null)
            {
                var imageFile = this.imageCtl.Tag.ToString();

                this.imageCtl.BackgroundImage.Dispose();
                this.imageCtl.BackgroundImage = null;
                this.imageCtl.Tag = null;

                if (File.Exists(imageFile)) File.Delete(imageFile);

                this.ViewImgPaths.Remove(imageFile);

                if(this.ViewImgPaths.Count == 0)
                {
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }
            }
        }

        private void ViewImgForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    imageLeft_Click(sender, e);
                    break;
                case Keys.Right:
                    imageRight_Click(sender, e);
                    break;
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
                case Keys.Delete:
                    this.DeleteImage();
                    imageRight_Click(sender, e);
                    break;
            }
        }

        private void ViewImgForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.imageCtl.BackgroundImage != null)
            {
                this.imageCtl.BackgroundImage.Dispose();
                this.imageCtl.BackgroundImage = null;
                this.imageCtl.Tag = null;
            }
        }
    }
}
