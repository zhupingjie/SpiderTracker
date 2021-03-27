using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.BackgroundImage = image;
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
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.BackgroundImage = image;
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
                }
                var image = Image.FromFile(ViewImgPaths[ViewImgIndex]);
                this.imageCtl.BackgroundImage = image;
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
                    this.Close();
                    break;
            }
        }

        private void ViewImgForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.imageCtl.BackgroundImage != null)
            {
                this.imageCtl.BackgroundImage.Dispose();
                this.imageCtl.BackgroundImage = null;
            }
        }
    }
}
