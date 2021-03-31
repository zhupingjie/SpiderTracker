using SpiderTracker.Imp;
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

        public List<string> ViewThumbImgPaths { get; set; }
        List<string> ViewImgPaths { get; set; } = new List<string>();
        public int ViewImgIndex { get; set; }

        public string ImageName { get; set; }
        public string ImageUser { get; set; }
        public string ImageStatus { get; set; }

        private void ViewImgForm_Load(object sender, EventArgs e)
        {
            var files = PathUtil.GetStoreUserImageFiles(ImageName, ImageUser, ImageStatus);
            ViewImgPaths.AddRange(files);

            if (ViewImgPaths != null && ViewImgPaths.Count > 0 && ViewImgIndex < ViewImgPaths.Count)
            {
                if (this.imageCtl.Image != null)
                {
                    this.imageCtl.Image.Dispose();
                    this.imageCtl.Image = null;
                    this.imageCtl.Tag = null;
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.Image = image;
                this.imageCtl.Tag = $"{ViewImgPaths[ViewImgIndex]},{ViewThumbImgPaths[ViewImgIndex]}";

                this.imageCtl.Image = image;
                this.imageCtl.Tag = $"{ViewImgPaths[ViewImgIndex]},{ViewThumbImgPaths[ViewImgIndex]}";
            }
        }

        private void imageLeft_Click(object sender, EventArgs e)
        {
            ViewImgIndex--;
            if (ViewImgIndex < 0) ViewImgIndex = 0;
            if (ViewImgPaths != null && ViewImgPaths.Count > 0 && ViewImgIndex < ViewImgPaths.Count)
            {
                if (this.imageCtl.Image != null)
                {
                    this.imageCtl.Image.Dispose();
                    this.imageCtl.Image = null;
                    this.imageCtl.Tag = null;
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.Image = image;
                this.imageCtl.Tag = $"{ViewImgPaths[ViewImgIndex]},{ViewThumbImgPaths[ViewImgIndex]}";
            }
        }

        private void imageRight_Click(object sender, EventArgs e)
        {
            ViewImgIndex++;
            if (ViewImgIndex >= ViewImgPaths.Count) ViewImgIndex = ViewImgPaths.Count - 1;
            if (ViewImgPaths != null && ViewImgPaths.Count > 0 && ViewImgIndex < ViewImgPaths.Count)
            {
                if (this.imageCtl.Image != null)
                {
                    this.imageCtl.Image.Dispose();
                    this.imageCtl.Image = null;
                    this.imageCtl.Tag = null;
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.Image = image;
                this.imageCtl.Tag = $"{ViewImgPaths[ViewImgIndex]},{ViewThumbImgPaths[ViewImgIndex]}";
            }
        }

        void DeleteImage()
        {
            if (this.imageCtl.Tag != null)
            {
                var imageFiles = this.imageCtl.Tag.ToString().Split(new string[] { "," }, StringSplitOptions.None).ToArray();
                if (imageFiles.Length != 2) return;

                this.imageCtl.Image.Dispose();
                this.imageCtl.Image = null;
                this.imageCtl.Tag = null;


                var imageFile = imageFiles[0];
                if (File.Exists(imageFile)) File.Delete(imageFile);
                this.ViewImgPaths.Remove(imageFile);

                var thumbFile = imageFiles[1];
                if (File.Exists(thumbFile)) File.Delete(thumbFile);
                this.ViewThumbImgPaths.Remove(thumbFile);

                if (this.ViewImgPaths.Count == 0)
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
                    ResetImage();
                    imageLeft_Click(sender, e);
                    break;
                case Keys.Right:
                    ResetImage();
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

        private void ResetImage()
        {
            this.imageCtl.SizeMode = PictureBoxSizeMode.Zoom; 
            this.imageCtl.Dock = DockStyle.Fill;
            this.imageCtl.Width = this.panel1.Width;
            this.imageCtl.Height = this.panel1.Height;
        }

        private void ViewImgForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.imageCtl.Image != null)
            {
                this.imageCtl.Image.Dispose();
                this.imageCtl.Image = null;
                this.imageCtl.Tag = null;
            }
        }

        private void imageCtl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                this.imageCtl.SizeMode = PictureBoxSizeMode.AutoSize;
                this.imageCtl.Dock = DockStyle.None;
            }
        }
    }
}
