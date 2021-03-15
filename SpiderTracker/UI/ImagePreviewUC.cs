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
    public partial class ImagePreviewUC : UserControl
    {
        public ImagePreviewUC()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.imageCtls = new List<Panel> { this.imageCtl1, this.imageCtl2, this.imageCtl3, this.imageCtl4, this.imageCtl5, this.imageCtl6, this.imageCtl7, this.imageCtl8, this.imageCtl9 };
        }

        List<Panel> imageCtls = null;
        string[] cacheImageFiles = null;
        int imageIndex = 0;

        public void ShowImages(string[] imageFiles, int showIndex, int showImageCount)
        {
            this.cacheImageFiles = imageFiles;
            this.imageIndex = showIndex;

            for (var i = 0; i < 9; i++)
            {
                if (i < cacheImageFiles.Length && i < showImageCount)
                {
                    ShowImage(imageCtls[i], cacheImageFiles[i]);
                }
                else
                {
                    DispiseImage(imageCtls[i]);
                }
            }
        }
        void ShowImage(Panel imageCtl, string file)
        {
            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                var image = Image.FromStream(stream);
                imageCtl.BackgroundImage = image;
                imageCtl.BackgroundImageLayout = ImageLayout.Zoom;
            }
        }

        void DispiseImage(Panel imageCtl)
        {
            if(imageCtl.BackgroundImage != null)
            {
                imageCtl.BackgroundImage.Dispose();
                imageCtl.BackgroundImage = null;
            }
        }

        void ShowOriginImage(int index)
        {
            if (index >= cacheImageFiles.Length) return;

            ViewImgForm frm = new ViewImgForm();
            frm.ViewImgPaths = cacheImageFiles;
            frm.ViewImgIndex = index;
            frm.ShowDialog();
        }
        
        private void imageCtl1_Click(object sender, EventArgs e)
        {
            ShowOriginImage(0);
        }

        private void imageCtl2_Click(object sender, EventArgs e)
        {
            ShowOriginImage(1);
        }

        private void imageCtl3_Click(object sender, EventArgs e)
        {
            ShowOriginImage(2);
        }

        private void imageCtl4_Click(object sender, EventArgs e)
        {
            ShowOriginImage(3);
        }

        private void imageCtl5_Click(object sender, EventArgs e)
        {
            ShowOriginImage(4);
        }

        private void imageCtl6_Click(object sender, EventArgs e)
        {
            ShowOriginImage(5);
        }

        private void imageCtl7_Click(object sender, EventArgs e)
        {
            ShowOriginImage(6);
        }

        private void imageCtl8_Click(object sender, EventArgs e)
        {
            ShowOriginImage(7);
        }

        private void imageCtl9_Click(object sender, EventArgs e)
        {
            ShowOriginImage(8);
        }
    }
}
