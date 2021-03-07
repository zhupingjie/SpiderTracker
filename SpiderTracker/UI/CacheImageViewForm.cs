using SpiderTracker.Imp;
using SpiderTracker.Imp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class CacheImageViewForm : Form
    {
        public string SpiderName
        {
            get
            {
                return this.cbxName.Text.Trim();
            }
        }
        public CacheImageViewForm()
        {
            InitializeComponent();
            SendMessage(this.lstImageGroup.Handle, LVM_SETICONSPACING, 0, 0x10000 * 300 + 280);
            SendMessage(this.lstImageStatus.Handle, LVM_SETICONSPACING, 0, 0x10000 * 132 + 132);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private int LVM_SETICONSPACING = 0x1035;

        private void CacheImageViewForm_Load(object sender, EventArgs e)
        {
            LoadCacheName();

            LoadCacheUser(SpiderName);
        }

        private void lstUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItem == null) return;
            var user = this.lstUser.SelectedItem.ToString();
            Clipboard.SetDataObject(user);

            var groupName = SpiderName;

            Task.Factory.StartNew(() => {
                LoadCacheUserStatus(groupName, user);
            });
        }

        void LoadCacheUserStatus(string groupName, string user)
        {
            int index = 0;
            var userPath = PathUtil.GetStoreImageUserPath(groupName, user);
            if (!Directory.Exists(userPath)) return;

            InvokeControl(this.lstImageGroup, new Action(() =>
            {
                if (this.lstImageGroup.LargeImageList != null) this.lstImageGroup.LargeImageList.Dispose();
                this.lstImageGroup.LargeImageList = new ImageList();
                this.lstImageGroup.LargeImageList.ImageSize = new Size(256, 256);
                this.lstImageGroup.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
                this.lstImageGroup.BeginUpdate();
                this.lstImageGroup.Items.Clear();
                this.lstImageGroup.EndUpdate();
            }));

            var statusPaths = Directory.GetDirectories(userPath);
            foreach (var statusPath in statusPaths)
            {
                if (!Directory.Exists(statusPath)) continue;
                var files = Directory.GetFiles(statusPath);
                if (files.Length == 0) continue;

                var status = PathUtil.GetStatusByPath(statusPath);
                
                InvokeControl(this.lstImageGroup, new Action(() =>
                {
                    var item = new ListViewItem();
                    item.Text = status;
                    item.ImageIndex = index++;
                    item.Tag = user;
                    item.Name = status;

                    this.lstImageGroup.Items.Add(item);

                    var image = Image.FromFile(files[0]);
                    Image bmp = new Bitmap(image);
                    image.Dispose();

                    this.lstImageGroup.LargeImageList.Images.Add(bmp);
                }));

                InvokeControl(this.progressBar1, new Action(() =>
                {
                    progressBar1.Maximum = statusPaths.Length;
                    progressBar1.Value = index;
                }));
            }
        }

        private void lstImageGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            var select = this.lstImageGroup.FocusedItem;
            if (select == null) return;

            var user = select.Tag.ToString();
            var status = select.Text;
            var groupName = SpiderName;

            Task.Factory.StartNew(() =>
            {
                LoadCacheStatusImage(groupName, user, status);
            });
        }

        private void LoadCacheStatusImage(string groupName, string user, string status)
        {
            int index = 0;
            var statusPath = PathUtil.GetStoreImageUserStatusPath(groupName, user, status);
            if(!Directory.Exists(statusPath)) return;

            InvokeControl(this.lstImageStatus, new Action(() =>
            {
                if (this.lstImageStatus.LargeImageList != null) this.lstImageStatus.LargeImageList.Dispose();
                this.lstImageStatus.LargeImageList = new ImageList();
                this.lstImageStatus.LargeImageList.ImageSize = new Size(128, 128);
                this.lstImageStatus.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
                this.lstImageStatus.BeginUpdate();
                this.lstImageStatus.Items.Clear();
                this.lstImageStatus.EndUpdate();
            }));

            var files = Directory.GetFiles(statusPath);
            foreach (var file in files)
            {
                InvokeControl(this.lstImageStatus, new Action(() =>
                {
                    var item = new ListViewItem();
                    item.ImageIndex = index++;
                    item.Tag = file;

                    this.lstImageStatus.Items.Add(item);

                    var image = Image.FromFile(file);
                    Image bmp = new Bitmap(image);
                    image.Dispose();

                    this.lstImageStatus.LargeImageList.Images.Add(bmp);
                }));

            }
        }

        private void lstImageStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstImageStatus.FocusedItem == null) return;
            var file = this.lstImageStatus.FocusedItem.Tag.ToString();

            var image = Image.FromFile(file);
            Image bmp = new Bitmap(image);
            image.Dispose();

            this.tabControl1.SelectedIndex = 1;
            if (this.pictureBox1.Image != null) this.pictureBox1.Image.Dispose();
            this.pictureBox1.Image = bmp;
        }

        private void cbxName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SpiderName)) return;

            LoadCacheUser(SpiderName);
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            LoadCacheUser(SpiderName);
        }

        protected  void LoadCacheName()
        {
            var names = PathUtil.GetStoreNames();

            this.cbxName.BeginUpdate();
            this.cbxName.Items.Clear();
            this.cbxName.Items.AddRange(names);
            this.cbxName.Text = "default";
            this.cbxName.EndUpdate();
        }

        protected void LoadCacheUser(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            var cachePath = PathUtil.GetStoreImagePath(name);
            var userPaths = Directory.GetDirectories(cachePath);

            var users = new List<string>();
            foreach (var userPath in userPaths)
            {
                var user = PathUtil.GetUserByPath(userPath);
                users.Add(user);
            }

            this.lstUser.BeginUpdate();
            this.lstUser.Items.Clear();
            if (!string.IsNullOrEmpty(this.txtKeyword.Text.Trim()))
            {
                users = users.Where(c => c.Contains(this.txtKeyword.Text.Trim())).ToList();
            }
            this.lstUser.Items.AddRange(users.ToArray());
            this.lstUser.EndUpdate();

            this.lblUserCount.Text = $"【共 {users.Count} 个用户】";
        }

        private void btnBrowseUser_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItem == null) return;

            var user = this.lstUser.SelectedItem.ToString();
            var url = SinaUrlUtil.GetSinaUserUrl(user);
            System.Diagnostics.Process.Start(url);
        }

        private void btnBrowseStatus_Click(object sender, EventArgs e)
        {
            if (this.lstImageGroup.FocusedItem == null) return;

            var status = this.lstImageGroup.FocusedItem.Text;
            var url = SinaUrlUtil.GetSinaUserStatusUrl(status);
            System.Diagnostics.Process.Start(url);
        }

        private void btnFocusUser_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItem == null) return;

            if (MessageBox.Show("确认关注当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var user = this.lstUser.SelectedItem.ToString();
            var rep = new SinaRepository();
            rep.FocusSinaUser(user);
        }

        private void btnIgnoreUser_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItem == null) return;

            if (MessageBox.Show("确认拉黑当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var user = this.lstUser.SelectedItem.ToString();

            IgnoreUser(user);
        }

        private void benOpen_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItem == null) return;

            var user = this.lstUser.SelectedItem.ToString();
            var path = PathUtil.GetStoreImageUserPath(SpiderName, user);

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + path;
            System.Diagnostics.Process.Start(psi);
        }

        private void btnFollower_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItem == null) return;

            var user = this.lstUser.SelectedItem.ToString();
            var url = SinaUrlUtil.GetSinaUserFollowerUrl(user);
            System.Diagnostics.Process.Start(url);
        }

        private void lstUser_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (this.lstUser.SelectedItem == null) return;

            if (MessageBox.Show("确认拉黑当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var user = this.lstUser.SelectedItem.ToString();

            IgnoreUser(user);
        }

        void IgnoreUser(string user)
        {
            var rep = new SinaRepository();
            var suc = rep.IgnoreSinaUser(user);
            if (suc)
            {
                var userPath = PathUtil.GetStoreImageUserPath(SpiderName, user);
                if (Directory.Exists(userPath)) Directory.Delete(userPath, true);

                var index = this.lstUser.SelectedIndex;
                this.lstUser.Items.Remove(user);

                if (this.lstImageGroup.LargeImageList != null)
                {
                    this.lstImageGroup.LargeImageList.Dispose();
                    this.lstImageGroup.Items.Clear();
                }
                if (this.lstImageStatus.LargeImageList != null)
                {
                    this.lstImageStatus.LargeImageList.Dispose();
                    this.lstImageStatus.Items.Clear();
                }
                if (this.lstUser.Items.Count > index) this.lstUser.SelectedIndex = index;
            }
        }

        private void btnFocusStatus_Click(object sender, EventArgs e)
        {
            if (this.lstImageGroup.FocusedItem == null) return;

            if (MessageBox.Show("确认关注当前图集?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var status = this.lstImageGroup.FocusedItem.Text;
            var user = this.lstImageGroup.FocusedItem.Tag.ToString();

            var rep = new SinaRepository();
            rep.FocusSinaStatus(status);
        }


        private void btnIgnoreStatus_Click(object sender, EventArgs e)
        {
            if (this.lstImageGroup.FocusedItem == null) return;

            var status = this.lstImageGroup.FocusedItem.Text;
            var user = this.lstImageGroup.FocusedItem.Tag.ToString();

            IgnoreStatus(user, status);
        }


        private void lstImageGroup_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (this.lstImageGroup.FocusedItem == null) return;

            var status = this.lstImageGroup.FocusedItem.Text;
            var user = this.lstImageGroup.FocusedItem.Tag.ToString();

            IgnoreStatus(user, status);
        }

        void IgnoreStatus(string user, string status)
        {
            if (MessageBox.Show("确认拉黑当前图集?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var suc = rep.IgnoreSinaStatus(status);
            if (suc)
            {
                var statusPath = PathUtil.GetStoreImageUserStatusPath(SpiderName, user, status);
                if (Directory.Exists(statusPath)) Directory.Delete(statusPath, true);

                if (this.lstImageGroup.Items.ContainsKey(status))
                {
                    this.lstImageGroup.Items.RemoveByKey(status);

                    if(this.lstImageStatus.LargeImageList != null)
                    {
                        this.lstImageStatus.LargeImageList.Dispose();
                        this.lstImageStatus.Items.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// 线程安全更新界面控件
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
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

    }
}
