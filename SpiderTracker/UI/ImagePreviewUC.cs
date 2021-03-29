using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class ImagePreviewUC : UserControl
    {

        List<Task> tasks = new List<Task>();
        List<Panel> imageCtls = null;
        List<string> cacheImageFiles = null;
        int imageIndex = 0;
        int imageCount = 0;
        Thread showImageThread = null;
        ManualResetEvent resetEvent = null;

        public ImagePreviewUC()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.MakeThread();
            this.cacheImageFiles = new List<string>();
            this.imageCtls = new List<Panel> { this.imageCtl1, this.imageCtl2, this.imageCtl3, this.imageCtl4, this.imageCtl5, this.imageCtl6, this.imageCtl7, this.imageCtl8, this.imageCtl9 };

        }

        void MakeThread()
        {
            this.showImageThread = new Thread(new ThreadStart(StartShowImageThread));
            this.showImageThread.IsBackground = true;
            this.showImageThread.Start();

            this.resetEvent = new ManualResetEvent(false);
        }

        void StartShowImageThread()
        {
            while (true)
            {
                if (cacheImageFiles.Count > 0)
                {
                    var showCount = cacheImageFiles.Count > this.imageCount ? this.imageCount : cacheImageFiles.Count;
                    for (var j = 0; j < showCount; j++)
                    {
                        ShowImage(imageCtls[j], cacheImageFiles[j]);
                    }
                    this.resetEvent.Reset();
                }
                this.resetEvent.WaitOne();
            }
        }

        public void ShowImages(string[] imageFiles, int showIndex, int showImageCount)
        {
            this.DispiseImage(imageFiles);
            this.cacheImageFiles.Clear();
            this.cacheImageFiles.AddRange(imageFiles);
            this.imageIndex = showIndex;
            this.imageCount = showImageCount;
            this.resetEvent.Set();
        }

        void ShowImage(Panel imageCtl, string file)
        {
            InvokeControl(imageCtl, () =>
            {
                if (imageCtl.Tag != null && imageCtl.Tag.ToString() == file) return;

                using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        var image = Image.FromStream(stream);
                        imageCtl.BackgroundImage = image;
                        imageCtl.BackgroundImageLayout = ImageLayout.Zoom;
                        imageCtl.Tag = file;
                    }
                    catch(Exception)
                    {
                        
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            });
        }

        void DispiseImage(string[] images)
        {
            for (var j = 0; j < this.imageCount; j++)
            {
                if (images.Length > j && imageCtls[j].Tag != null && images[j] == imageCtls[j].Tag.ToString()) continue;

                DispiseImage(imageCtls[j]);
            }
        }

        void DispiseImage(Panel imageCtl)
        {
            InvokeControl(imageCtl, () =>
            {
                if (imageCtl.BackgroundImage != null)
                {
                    imageCtl.BackgroundImage.Dispose();
                    imageCtl.BackgroundImage = null;
                    imageCtl.Tag = null;
                }
            });
        }
        private void InvokeControl(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }

        void ShowOriginImage(int index)
        {
            if (index >= cacheImageFiles.Count) return;

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
