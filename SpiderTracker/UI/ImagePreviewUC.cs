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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class ImagePreviewUC : UserControl
    {

        List<Task> tasks = new List<Task>();
        List<Panel> imageCtls = new List<Panel>();
        List<FileInfo> cacheImageFiles = new List<FileInfo>();
        string imageCtrlName = "imageCtl";
        SpiderRunningConfig RunningConfig;
        SinaStatus SinaStatus;
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
            this.InitImageCtrl();
        }

        void InitImageCtrl()
        {
            for (var i = 0; i < 9; i++)
            {
                var imgCtrl = MakeImagePanel(i);
                this.imageCtls.Add(imgCtrl);

                InvokeControl(pnlImagePanel, () =>
                {
                    this.pnlImagePanel.Controls.Add(imgCtrl);
                });
            }
        }

        void MakeThread()
        {
            this.showImageThread = new Thread(new ThreadStart(StartShowImageThread));
            this.showImageThread.IsBackground = true;
            this.showImageThread.Start();

            this.resetEvent = new ManualResetEvent(false);
        }
        bool couldLoadImageTask = true;
        void StartShowImageThread()
        {
            while (true)
            {
                if (cacheImageFiles.Count > 0)
                {
                    int index = 0;
                    foreach(var imageFile in cacheImageFiles)
                    {
                        ShowImage(index++, imageFile.FullName);

                        Thread.Sleep(100);
                    }
                    couldLoadImageTask = true;

                    this.resetEvent.Reset();
                }
                this.resetEvent.WaitOne();
            }
        }
        
        public void ShowImages(FileInfo[] imageFiles, SpiderRunningConfig runningConfig, SinaStatus sinaStatus)
        {
            if (!couldLoadImageTask) return;

            this.couldLoadImageTask = false;
            this.DispiseImage(imageFiles);
            this.cacheImageFiles.Clear();
            this.cacheImageFiles.AddRange(imageFiles);
            this.RunningConfig = runningConfig;
            this.SinaStatus = sinaStatus;
            this.resetEvent.Set();
        }

        void ShowImage(int imgCtrlIndex, string file)
        {
            var ctlName = $"imageCtl{imgCtrlIndex}";
            Panel imageCtl = null;
            if (!this.imageCtls.Any(c => c.Name == ctlName))
            {
                imageCtl = MakeImagePanel(imgCtrlIndex);
                this.imageCtls.Add(imageCtl);

                InvokeControl(pnlImagePanel, () =>
                {
                    this.pnlImagePanel.Controls.Add(imageCtl);
                });
            }
            else
            {
                imageCtl = imageCtls[imgCtrlIndex];
            }
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

        int imageWidth = 180;
        int imageHeight = 220;
        Panel MakeImagePanel(int imgCtrlIndex)
        {
            var imageCtl = new Panel();
            imageCtl.Name = $"imageCtl{imgCtrlIndex}";
            imageCtl.Width = imageWidth;
            imageCtl.Height = imageHeight;
            imageCtl.BackColor = Color.Transparent;
            imageCtl.BorderStyle = BorderStyle.FixedSingle;
            imageCtl.Click += ImageCtl_Click;
            imageCtl.DoubleClick += ImageCtl_DoubleClick;

            //横向双排展示
            //var x = (int)imgCtrlIndex / 2;
            //if (imgCtrlIndex % 2 == 0)
            //{
            //    imageCtl.Location = new Point(5 * x + x * 160, 0);
            //}
            //else
            //{
            //    imageCtl.Location = new Point(5 * x + x * 160, 225);
            //}
            //纵向三列展示
            var row = (int)imgCtrlIndex / 3;
            var col = (int)imgCtrlIndex % 3;
            imageCtl.Location = new Point(col * (imageWidth+1), row * (imageHeight+1));
            return imageCtl;
        }

        private void ImageCtl_DoubleClick(object sender, EventArgs e)
        {
            var index = GetImageCtrlIndex((sender as Panel).Name);
            if (index == -1) return;

            SelectOriginImage(index);
        }

        private void ImageCtl_Click(object sender, EventArgs e)
        {
            var index = GetImageCtrlIndex((sender as Panel).Name);
            if (index == -1) return;

            SelectImageBox(index);
        }

        int GetImageCtrlIndex(string name)
        {
            if (!name.StartsWith(imageCtrlName)) return -1;
            int index = -1;
            var str = name.Replace(imageCtrlName, "");
            int.TryParse(str, out index);
            return index;
        }

        void DispiseImage(FileInfo[] images)
        {
            foreach(var imageCtr in this.imageCtls)
            {
                ResetImageCtrl(imageCtr);
            }
        }

        void ResetImageCtrl(Panel imageCtl)
        {
            InvokeControl(imageCtl, () =>
            {
                if (imageCtl.BackgroundImage != null)
                {
                    imageCtl.BackgroundImage.Dispose();
                    imageCtl.BackgroundImage = null;
                    imageCtl.Tag = null;
                }
                imageCtl.BorderStyle = BorderStyle.FixedSingle;
                imageCtl.BackColor = Color.Transparent;
            });
        }

        void InvokeControl(Control control, Action action)
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

        void SelectImageBox(int index)
        {
            //if (index >= cacheImageFiles.Count) return;

            this.SetImagePanelBorder(imageCtrlName, index, null);
        }
        
        void SelectOriginImage(int index)
        {
            if (index >= cacheImageFiles.Count) return;

            ViewImgForm frm = new ViewImgForm();
            frm.ViewThumbImgPaths = cacheImageFiles;
            frm.ViewImgIndex = index;
            frm.ImageName = RunningConfig.Category;
            frm.UploadPath = RunningConfig.DefaultUploadPath;
            frm.ImageUser = SinaStatus.uid;
            frm.ImageStatus = SinaStatus.bid;
            frm.ShowDialog();
        }

        void SetImagePanelBorder(string findCtrlName, int index,  Control parentCtrl = null)
        {
            var ctrls = this.Controls;
            var findCtrlFullName = $"{findCtrlName}{index}";
            if (parentCtrl != null) ctrls = parentCtrl.Controls;
            foreach (Control ctrl in ctrls)
            {
                if (ctrl.Name.StartsWith(findCtrlName))
                {
                    var panel = ctrl as Panel;
                    if (ctrl.Name.Equals(findCtrlFullName))
                    {
                        panel.BorderStyle = BorderStyle.Fixed3D;
                        panel.BackColor = Color.Gold;
                    }
                    else
                    {
                        panel.BorderStyle = BorderStyle.FixedSingle;
                        panel.BackColor = Color.Transparent;

                        SetImagePanelBorder(findCtrlName, index, ctrl);
                    }
                }
                else
                {
                    SetImagePanelBorder(findCtrlName, index, ctrl);
                }
            }
        }
    }
}
