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
        List<SinaSource> SinaSources;
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
                this.pnlImagePanel.Controls.Add(imgCtrl);
            }
            this.pnlImagePanel.Dock = DockStyle.Fill;
        }

        public void ShowImages(FileInfo[] imageFiles, SpiderRunningConfig runningConfig, SinaStatus sinaStatus, List<SinaSource> sources)
        {
            if (!couldLoadImageTask) return;

            this.couldLoadImageTask = false;
            this.ResetImageCtrl();
            this.cacheImageFiles.Clear();
            this.cacheImageFiles.AddRange(imageFiles);
            this.RunningConfig = runningConfig;
            this.SinaStatus = sinaStatus;
            this.SinaSources = sources;
            this.resetEvent.Set();
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

                        Thread.Sleep(50);
                    }
                }
                couldLoadImageTask = true;
                this.resetEvent.Reset();
                this.resetEvent.WaitOne();
            }
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
                using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        var image = Image.FromStream(stream);
                        imageCtl.BackgroundImage = image;
                        imageCtl.BackgroundImageLayout = ImageLayout.Zoom;

                        var imageFile = PathUtil.GetImageByThumbImage(file);
                        imageCtl.Tag = new ImageCtrlData()
                        {
                            ThumbFile = file,
                            ImageFile = imageFile.FullName,
                            Name = imageFile.Name,
                            LocationX = imageCtl.Location.X,
                            LocationY = imageCtl.Location.Y
                        };

                        var source = this.CheckImageUploadStatus(imageFile.Name);
                        this.ShowImgCtrlTitle(imageCtl, source);
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
            #region 图片容器
            var imageCtl = new Panel();
            imageCtl.Name = $"imageCtl{imgCtrlIndex}";
            imageCtl.Width = imageWidth;
            imageCtl.Height = imageHeight;
            imageCtl.BackColor = Color.Transparent;
            imageCtl.BorderStyle = BorderStyle.FixedSingle;
            imageCtl.Click += ImageCtl_Click;
            imageCtl.DoubleClick += ImageCtl_DoubleClick;

            #region 横向双排展示
            //var x = (int)imgCtrlIndex / 2;
            //if (imgCtrlIndex % 2 == 0)
            //{
            //    imageCtl.Location = new Point(x * (imageWidth + 1), 0);
            //}
            //else
            //{
            //    imageCtl.Location = new Point(x *  (imageWidth + 1), 225);
            //}
            #endregion

            # region 纵向三列展示
            var row = (int)imgCtrlIndex / 3;
            var col = (int)imgCtrlIndex % 3;
            imageCtl.Location = new Point(col * (imageWidth+1), row * (imageHeight+1));
            #endregion

            #endregion

            #region 标题栏

            var panTitle = new Panel();
            panTitle.Name = "pnlTitle";
            panTitle.Height = 10;
            panTitle.Dock = DockStyle.Top;
            panTitle.BackColor = Color.Gold;
            panTitle.Visible = false;
            imageCtl.Controls.Add(panTitle);

            #endregion

            #region 工具栏
            var panTools = new Panel();
            panTools.Name = "pnlTools";
            panTools.Height = 20;
            panTools.Dock = DockStyle.Bottom;
            panTools.BackColor = Color.Transparent;
            panTools.Visible = false;
            imageCtl.Controls.Add(panTools);

            //忽略
            var btnDelImg = new Button();
            btnDelImg.Text = "❌";
            btnDelImg.Height = 20;
            btnDelImg.Width = 30;
            btnDelImg.Font = new Font("微软雅黑", 8);
            btnDelImg.Dock = DockStyle.Right;
            btnDelImg.Click += BtnDelImg_Click;
            panTools.Controls.Add(btnDelImg);

            //撤销上传
            var btnCnlImg = new Button();
            btnCnlImg.Name = "btnCancelUploadImg";
            btnCnlImg.Text = "▼";
            btnCnlImg.Height = 20;
            btnCnlImg.Width = 30;
            btnCnlImg.Font = new Font("微软雅黑", 8);
            btnCnlImg.Dock = DockStyle.Right;
            btnCnlImg.Click += BtnCnlImg_Click;
            panTools.Controls.Add(btnCnlImg);

            //上传
            var btnUpdImg = new Button();
            btnUpdImg.Name = "btnUploadImg";
            btnUpdImg.Text = "▲";
            btnUpdImg.Height = 20;
            btnUpdImg.Width = 30;
            btnUpdImg.Font = new Font("微软雅黑", 8);
            btnUpdImg.Dock = DockStyle.Right;
            btnUpdImg.Click += BtnUpdImg_Click;
            panTools.Controls.Add(btnUpdImg);

            //原图
            var btnOrgImg = new Button();
            btnOrgImg.Text = "◉";
            btnOrgImg.Height = 20;
            btnOrgImg.Width = 30;
            btnOrgImg.Font = new Font("微软雅黑", 8);
            btnOrgImg.Dock = DockStyle.Right;
            btnOrgImg.Click += BtnOrgImg_Click; ;
            panTools.Controls.Add(btnOrgImg);

            #endregion

            return imageCtl;
        }

        void ReloadUserSources()
        {
            var rep = new SinaRepository();
            this.SinaSources = rep.GetUserSources(SinaStatus.uid, SinaStatus.bid);
        }

        #region 缩略图工具栏事件

        private void BtnOrgImg_Click(object sender, EventArgs e)
        {
            var imgCtrlData = GetCurrentImageCtrlData();
            if (imgCtrlData == null) return;

            this.ShowOriginImage(imgCtrlData, true);
        }

        private void BtnUpdImg_Click(object sender, EventArgs e)
        {
            this.UploadRemoteImage();
        }

        private void BtnCnlImg_Click(object sender, EventArgs e)
        {
            this.DeleteRemoteImage();
        }

        private void BtnDelImg_Click(object sender, EventArgs e)
        {
            this.IgnoreOriginImage();
        }

        private void ImageCtl_DoubleClick(object sender, EventArgs e)
        {
            var imgCtrlName = (sender as Panel).Name;
            var imgCtrl = this.imageCtls.FirstOrDefault(c => c.Name == imgCtrlName);
            if (imgCtrl == null || imgCtrl.Tag == null) return;

            var imgCtrlData = imgCtrl.Tag as ImageCtrlData;
            if (imgCtrlData == null) return;

            this.ShowOriginImage(imgCtrlData, true);
        }

        private void ImageCtl_Click(object sender, EventArgs e)
        {
            var imgCtrlName = (sender as Panel).Name;
            SelectImageCtrl(imgCtrlName);
        }

        #endregion

        #region 原图工具栏事件

        private void pnlOriginPanel_DoubleClick(object sender, EventArgs e)
        {
            this.ShowOriginImage(null, false);
        }

        private void btnShowThumbImg_Click(object sender, EventArgs e)
        {
            var imgCtrlData = GetCurrentImageCtrlData();
            if (imgCtrlData == null) return;

            this.ShowOriginImage(imgCtrlData, false);
        }

        private void btnOrgNextImg_Click(object sender, EventArgs e)
        {
            this.ShowNextImage();
        }

        private void btnOrgPreImg_Click(object sender, EventArgs e)
        {
            this.ShowPreviousImage();
        }

        private void btnOrgUpdoadImg_Click(object sender, EventArgs e)
        {
            this.UploadRemoteImage();
        }

        private void btnDelOrgImg_Click(object sender, EventArgs e)
        {
            this.IgnoreOriginImage();
        }

        private void btnOrgDelImg_Click(object sender, EventArgs e)
        {
            this.DeleteRemoteImage();
        }

        #endregion

        void ResetImageCtrl()
        {
            ShowOriginImage(null, false);

            foreach (var imageCtr in this.imageCtls)
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

                this.ShowImgCtrlTools(imageCtl, false);
            });
        }

        void SelectImageCtrl(string findCtrlFullName)
        {
            var ctrls = this.Controls;
            var findCtrl = this.imageCtls.FirstOrDefault(c => c.Name == findCtrlFullName);
            foreach (var imgCtrl in imageCtls)
            {
                if (imgCtrl.Name == findCtrlFullName)
                {
                    imgCtrl.BorderStyle = BorderStyle.Fixed3D;
                    imgCtrl.BackColor = Color.Transparent;
                    imgCtrl.Select();

                    this.ShowImageInfo(imgCtrl.Tag as ImageCtrlData);
                    this.ShowRemoteInfo("");
                    this.ShowImgCtrlTools(imgCtrl, true);
                }
                else
                {
                    imgCtrl.BorderStyle = BorderStyle.FixedSingle;
                    imgCtrl.BackColor = Color.Transparent;

                    this.ShowImgCtrlTools(imgCtrl, false);
                }
            }
        }

        void ShowOriginImage(ImageCtrlData ctrlData, bool visible)
        {
            if (visible && ctrlData != null)
            {
                this.pnlImagePanel.Visible = false;
                this.pnlImagePanel.Dock = DockStyle.None;

                this.pnlOriginPanel.Visible = true;
                this.pnlOriginPanel.Dock = DockStyle.Fill;
                this.pnlOriginPanel.BringToFront();

                //检测上传&撤销按钮状态
                var upload = this.CheckImageUploadStatus(ctrlData.Name);
                var btnCancelUploadImg = pnlOriginPanel.Controls.Find("btnOrgDelImg", true).FirstOrDefault();
                if (btnCancelUploadImg != null)
                {
                    (btnCancelUploadImg as Button).Enabled = upload;
                }
                var btnUploadImg = pnlOriginPanel.Controls.Find("btnOrgUpdoadImg", true).FirstOrDefault();
                if (btnUploadImg != null)
                {
                    (btnUploadImg as Button).Enabled = !upload;
                }

                using (Stream stream = File.Open(ctrlData.ImageFile, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        var image = Image.FromStream(stream);
                        pnlOriginPanel.BackgroundImage = image;
                        pnlOriginPanel.BackgroundImageLayout = ImageLayout.Zoom;

                        //设置原图尺寸
                        ctrlData.ImageWidth = image.Width;
                        ctrlData.ImageHeight = image.Height;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else
            {
                this.pnlImagePanel.Visible = true;
                this.pnlImagePanel.Dock = DockStyle.Fill;
                this.pnlImagePanel.BringToFront();
                this.pnlOriginPanel.Visible = false;
                this.pnlOriginPanel.Dock = DockStyle.None;

                if (this.pnlOriginPanel.BackgroundImage != null)
                {
                    this.pnlOriginPanel.BackgroundImage.Dispose();
                    this.pnlOriginPanel.BackgroundImage = null;
                }
            }
        }

        void ShowNextImage()
        {
            var currentCtrl = GetCurrentImageCtrl();

            int index = 1;
            if (currentCtrl != null)
            {
                index = this.imageCtls.IndexOf(currentCtrl);
                if (index == this.cacheImageFiles.Count - 1) index = 0;
                else index += 1;
            }
            var imgCtrl = this.imageCtls.FirstOrDefault(c => c.Name == $"{imageCtrlName}{index}");
            if (imgCtrl == null) return;

            var imgCtrlData = imgCtrl.Tag as ImageCtrlData;
            if (imgCtrlData == null) return;

            SelectImageCtrl(imgCtrl.Name);
            ShowOriginImage(imgCtrlData, true);
        }

        void ShowPreviousImage()
        {
            var currentCtrl = GetCurrentImageCtrl();

            int index = 0;
            if (currentCtrl != null)
            {
                index = this.imageCtls.IndexOf(currentCtrl);
                if (index == 0) index = this.cacheImageFiles.Count - 1;
                else index -= 1;
            }
            var imgCtrl = this.imageCtls.FirstOrDefault(c => c.Name == $"{imageCtrlName}{index}");
            if (imgCtrl == null) return;

            var imgCtrlData = imgCtrl.Tag as ImageCtrlData;
            if (imgCtrlData == null) return;

            SelectImageCtrl(imgCtrl.Name);
            ShowOriginImage(imgCtrlData, true);
        }

        void UploadRemoteImage()
        {
            var imgCtrl = GetCurrentImageCtrl();
            if (imgCtrl == null || imgCtrl.Tag == null) return;

            var imgCtrlData = imgCtrl.Tag as ImageCtrlData;
            if (imgCtrlData == null) return;

            var imgFile = new FileInfo(imgCtrlData.ImageFile);
            if (imgFile.Exists)
            {
                var uploadFiles = new FileInfo[] { imgFile };
                var rep = new SinaRepository();
                if (rep.UploadSinaStatus(RunningConfig.Category, SinaStatus.bid, uploadFiles, true))
                {
                    PathUtil.CopyUploadImageFiles(uploadFiles, RunningConfig.DefaultUploadPath);

                    this.ReloadUserSources();

                    this.ShowImgCtrlTitle(imgCtrl, true);

                    this.ShowRemoteInfo($"上传成功");
                }
                else
                {
                    this.ShowRemoteInfo($"上传失败");
                }
            }
            else
            {
                this.ShowRemoteInfo($"图片不存在");
            }
        }

        void DeleteRemoteImage()
        {
            var imgCtrl = GetCurrentImageCtrl();

            var imgCtrlData = GetCurrentImageCtrlData();
            if (imgCtrlData == null) return;

            var imgFile = new FileInfo(imgCtrlData.ImageFile);
            var suc = HttpUtil.DeleteSinaSourceImage(RunningConfig, SinaStatus.bid, imgFile.Name);
            if (suc)
            {
                var rep = new SinaRepository();
                rep.UploadSinaStatus(RunningConfig.Category, SinaStatus.bid, new FileInfo[] { imgFile }, false);

                this.ReloadUserSources();

                this.ShowImgCtrlTitle(imgCtrl, false);

                this.ShowRemoteInfo($"撤销上传成功");
            }
            else
            {
                this.ShowRemoteInfo($"撤销上传失败");
            }
        }

        void IgnoreOriginImage()
        {
            var imgCtrlData = GetCurrentImageCtrlData();
            if (imgCtrlData == null) return;

            var thumb = new FileInfo(imgCtrlData.ThumbFile);
            if (thumb.Exists)
            {
                thumb.Delete();

                var imgFile = new FileInfo(imgCtrlData.ImageFile);
                if (imgFile.Exists) imgFile.Delete();
            }
            var rep = new SinaRepository();
            rep.IgnoreSinaSource(SinaStatus.uid, SinaStatus.bid, thumb.Name);

            this.DeleteRemoteImage();

            var imgCtrl = GetCurrentImageCtrl();
            ResetImageCtrl(imgCtrl);
        }

        void ShowImgCtrlTools(Panel imgCtrl, bool visible)
        {
            var pnlTools = imgCtrl.Controls.Find("pnlTools", false).FirstOrDefault();
            if (pnlTools != null)
            {
                (pnlTools as Panel).Visible = visible;

                if (visible)
                {
                    var upload = this.CheckImageUploadStatus(imgCtrl.Name);
                    var btnCancelUploadImg = pnlTools.Controls.Find("btnCancelUploadImg", false).FirstOrDefault();
                    if(btnCancelUploadImg != null)
                    {
                        (btnCancelUploadImg as Button).Enabled = upload;
                    }
                    var btnUploadImg = pnlTools.Controls.Find("btnUploadImg", false).FirstOrDefault();
                    if (btnUploadImg != null)
                    {
                        (btnUploadImg as Button).Enabled = !upload;
                    }
                }
            }
        }

        void ShowImgCtrlTitle(Panel imgCtrl, bool visible)
        {
            var pnlTitle = imgCtrl.Controls.Find("pnlTitle", false).FirstOrDefault();
            if (pnlTitle != null)
            {
                (pnlTitle as Panel).Visible = visible;
            }
        }

        bool CheckImageUploadStatus(string imgName)
        {
            return this.SinaSources.Any(c => c.name == imgName && c.upload > 0);
        }

        void ShowImageInfo(ImageCtrlData ctrlData)
        {
            this.lblImageMsg.Text = $"{ctrlData.Name}";
        }

        void ShowRemoteInfo(string msg)
        {
            this.lblReomteMsg.Text = msg;
        }

        Panel GetCurrentImageCtrl()
        {
            return this.imageCtls.FirstOrDefault(c => c.BorderStyle == BorderStyle.Fixed3D);
        }

        ImageCtrlData GetCurrentImageCtrlData()
        {
            var imgCtrl = GetCurrentImageCtrl();
            if (imgCtrl == null || imgCtrl.Tag == null) return null;

            var imgCtrlData = imgCtrl.Tag as ImageCtrlData;
            if (imgCtrlData == null) return null;

            return imgCtrlData;
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
    }

    public class ImageCtrlData
    { 
        public string Name { get; set; }
        public string ImageFile { get; set; }

        public string ThumbFile { get; set; }

        public int LocationX { get; set; }

        public int LocationY { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }
    }
}
