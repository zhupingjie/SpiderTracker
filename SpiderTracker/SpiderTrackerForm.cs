using SpiderTracker.Imp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading;
using SpiderTracker.UI;
using log4net;
using System.Reflection;
using log4net.Config;
using SpiderTracker.Imp.Model;

namespace SpiderTracker
{
    public partial class SpiderTrackerForm : Form
    {
        ILog Log { get; set; }
        MWeiboSpiderService SinaSpiderService { get; set; }

        bool SortByDate { get; set; } = true;

        public SpiderTrackerForm()
        {
            InitializeComponent();

            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        /// <summary>
        /// 界面加载开启读取本地缓存用户文章列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpiderTrackerForm_Load(object sender, EventArgs e)
        {
            Log = LogManager.GetLogger("logAppender");

            SQLiteDBHelper.Instance.InitSpiderDB();
            InitSpiderRunningConfig();

            //SQLiteDBHelper.Instance.InitSQLiteDB();

            SinaSpiderService = new MWeiboSpiderService();
            SinaSpiderService.OnShowStatus += WeiboSpiderService_OnShowStatus;
            SinaSpiderService.OnSpiderStarted += WeiboSpiderService_OnSpiderStarted;
            SinaSpiderService.OnSpiderComplete += WeiboSpiderService_OnSpiderComplete;
            SinaSpiderService.OnSpiderStoping += WeiboSpiderService_OnSpiderStoping;
            SinaSpiderService.OnRefreshConfig += WeiboSpiderService_OnRefreshConfig;
            //SinaSpiderService.OnChangeUserStatus += SinaSpiderService_OnChangeUserStatus;
                 
            Task.Factory.StartNew(() => {
                LoadCacheNameList();
            });
        }


        #region Spider Event

        //private void SinaSpiderService_OnChangeUserStatus(string uid, bool ignore)
        //{
        //    if (ignore)
        //    {
        //        InvokeControl(this.lstUser, new Action(() =>
        //        {
        //            if (this.lstUser.Items.Contains(uid))
        //            {
        //                var index = this.lstUser.SelectedIndex;
        //                this.lstUser.Items.Remove(uid);
        //                if (this.lstUser.Items.Count > index) this.lstUser.SelectedIndex = index;
        //            }
        //        }));
        //    }
        //}

        private void WeiboSpiderService_OnRefreshConfig(SpiderRunningConfig spiderRunninConfig)
        {

        }

        private void WeiboSpiderService_OnSpiderStarted()
        {
            ActiveLoggerCtl();

            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "Stop";
                this.btnSearch.Enabled = true;
            }));

            InvokeControl(this.btnLoop, new Action(() =>
            {
                this.btnLoop.Text = "Stop";
                this.btnLoop.Enabled = true;
            }));


            InvokeControl(this.btnGatherFoucs, new Action(() =>
            {
                this.btnGatherFoucs.Text = "Stop";
                this.btnGatherFoucs.Enabled = true;
            }));

            InvokeControl(this.btnAutoGather, new Action(() =>
            {
                this.btnAutoGather.Text = "Stop";
                this.btnAutoGather.Enabled = true;
            }));
        }

        private void WeiboSpiderService_OnSpiderStoping()
        {
            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "Stop...";
                this.btnSearch.Enabled = false;
            }));

            InvokeControl(this.btnLoop, new Action(() =>
            {
                this.btnLoop.Text = "Stop...";
                this.btnLoop.Enabled = false;
            }));

            InvokeControl(this.btnGatherFoucs, new Action(() =>
            {
                this.btnGatherFoucs.Text = "Stop...";
                this.btnGatherFoucs.Enabled = false;
            }));
            InvokeControl(this.btnAutoGather, new Action(() =>
            {
                this.btnAutoGather.Text = "Stop...";
                this.btnAutoGather.Enabled = false;
            }));
        }

        private void WeiboSpiderService_OnSpiderComplete()
        {
            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "单次采集";
                this.btnSearch.Enabled = true;
            }));

            InvokeControl(this.btnLoop, new Action(() =>
            {
                this.btnLoop.Text = "循环采集";
                this.btnLoop.Enabled = true;
            }));

            InvokeControl(this.btnGatherFoucs, new Action(() =>
            {
                this.btnGatherFoucs.Text = "我的关注";
                this.btnGatherFoucs.Enabled = true;
            }));

            InvokeControl(this.btnAutoGather, new Action(() =>
            {
                this.btnAutoGather.Text = "他的关注";
                this.btnAutoGather.Enabled = true;
            }));
        }

        private void WeiboSpiderService_OnShowStatus(string msg, bool bLog = true, Exception ex = null)
        {
            InvokeControl(this.statusStrip1, new Action(() =>
            {
                this.tplStatus.Text = msg;
            }));
            if (bLog)
            {
                Log.Debug(msg, ex);
            }
            InvokeControl(this.lstLog, new Action(() =>
            {
                //this.lstLog.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss")} {msg}");
                this.lstLog.Items.Add($"{DateTime.Now.ToString("HH:mm:ss")} {msg}");
                this.lstLog.TopIndex = this.lstLog.Items.Count - (int)(this.lstLog.Height / this.lstLog.ItemHeight);
            }));
        }

        #endregion

        #region 已采集数据加载

        void ClearCacheUser()
        {
            InvokeControl(this.lstUser, new Action(() =>
            {
                this.lstUser.Items.Clear();
            }));

            InvokeControl(this.lstArc, new Action(() =>
            {
                this.lstArc.Items.Clear();
            }));
        }

        void LoadCacheUserTask()
        {
            while (true)
            {
                if (ResetLoadCacheTask)
                {
                    ResetLoadCacheTask = false;
                    ClearCacheUser();
                }

                Thread.Sleep(2 * 1000);
                
                LoadCacheUserList(LoadCacheName);

                UpdateCacheUserInfo(LoadCacheName);

                Thread.Sleep(3 * 1000);
            }
        }

        void LoadCacheNameList()
        {
            while (true)
            {
                //var names = PathUtil.GetStoreNames();
                var names = SinaSpiderService.Repository.GetGroupNames();

                InvokeControl(this.cbxName, new Action(() =>
                {
                    bool bNew = false;
                    foreach (var name in names)
                    {
                        if (!this.cbxName.Items.Contains(name))
                        {
                            bNew = true;
                            break;
                        }
                    }
                    if (bNew)
                    {
                        var selectName = this.cbxName.Text;
                        this.cbxName.BeginUpdate();
                        this.cbxName.Items.Clear();
                        this.cbxName.Items.AddRange(names);
                        if (!selectName.Equals("default"))
                        {
                            this.cbxName.Text = selectName;
                        }
                        this.cbxName.EndUpdate();
                    }
                    if (names.Length > 0)
                    {
                        LoadCacheName = names.FirstOrDefault();

                        LoadCacheUserList(LoadCacheName);
                    }
                }));

                Thread.Sleep(10 * 1000);
            }
        }

        void LoadCacheUserList(string name)
        {
            var keyword = this.txtUserFilter.Text.Trim();
            var users = SinaSpiderService.Repository.GetUsers(name, keyword);

            if (SortByDate) users = users.OrderByDescending(c => c.lastdate).ToList();
            else users = users.OrderBy(c => c.uid).ToList();

            var needUsers = new List<SinaUser>();
            if (this.lstUser.Items.Count > 0)
            {
                foreach (var user in users)
                {
                    if (this.lstUser.FindItemWithText(user.uid, true, 0, true) == null)
                    {
                        needUsers.Add(user);
                    }
                }
            }
            else
            {
                needUsers.AddRange(users);
            }
            InvokeControl(this.lstUser, new Action(() =>
            {
                this.lstUser.BeginUpdate();
                foreach(var item in needUsers)
                {
                    var subItem = new ListViewItem();
                    subItem.Text = item.uid;
                    subItem.SubItems.Add(item.name);
                    subItem.SubItems.Add($"{item.piccount}");
                    this.lstUser.Items.Add(subItem);
                }
                this.lstUser.EndUpdate();
            }));
        }
        
        void LoadCacheUserStatusList(string user)
        {
            var needArcs = new List<SinaStatus>();
            var statuses = SinaSpiderService.Repository.GetUserStatuses(user);
            if (this.lstArc.Items.Count > 0)
            {
                foreach (var status in statuses.OrderByDescending(c => c.lastdate))
                {
                    if (this.lstArc.FindItemWithText(status.bid, true, 0, true) == null)
                    {
                        needArcs.Add(status);
                    }
                }
            }
            else
            {
                needArcs.AddRange(statuses);
            }
            InvokeControl(this.lstArc, new Action(() =>
            {
                this.lstArc.BeginUpdate();
                foreach (var item in needArcs)
                {
                    var subItem = new ListViewItem();
                    subItem.Text = item.bid;
                    subItem.SubItems.Add($"{item.pics}");

                    var local = 0;
                    var path = PathUtil.GetStoreImageUserStatusPathBySelect(LoadCacheName, user, item.bid);
                    if(Directory.Exists(path))
                    {
                        local = Directory.GetFiles(path).Where(c => c.EndsWith(".jpg")).Count();
                    }
                    subItem.SubItems.Add($"{local}");
                    subItem.SubItems.Add(item.archive == 1 ? "✔" : "×");
                    this.lstArc.Items.Add(subItem);
                }
                this.lstArc.EndUpdate();

                this.lblStatusCount.Text = $"【{statuses.Count} 个图集】";
            }));
        }
        
        public void UpdateCacheUserInfo(string name)
        {
            string root = PathUtil.GetStoreImagePath(name);

            var userCount = Directory.GetDirectories(root).Length;
            InvokeControl(this.lblUserCount, new Action(() =>
            {
                this.lblUserCount.Text = $"{userCount}";
            }));

            var arcCount = GetDirectoryCount(root);
            InvokeControl(this.lblArcCount, new Action(() =>
            {
                this.lblArcCount.Text = $"{arcCount}";
            }));

            var picCount = GetFilesCount(root);
            InvokeControl(this.lblPicCount, new Action(() =>
            {
                this.lblPicCount.Text = $"{picCount}";
            }));
        }
        #endregion

        #region 采集功能操作
        /// <summary>
        /// 单次采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!SinaSpiderService.IsSpiderStarted)
            {
                SinaSpiderService.StartSpider(GetSpiderRunningConfig());
            }
            else
            {
                SinaSpiderService.StopSpider();
            }
        }

        /// <summary>
        /// 循环采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoop_Click(object sender, EventArgs e)
        {
            if (!SinaSpiderService.IsSpiderStarted)
            {
                var userIds = new List<string>();
                foreach (var item in this.lstUser.SelectedItems)
                {
                    var u = item.ToString();
                    if (!userIds.Contains(u)) userIds.Add(u);
                }

                var runningConfig = GetSpiderRunningConfig();
                runningConfig.UserIds = userIds.ToArray();
                runningConfig.GatherType = GatherTypeEnum.MultiGather;

                SinaSpiderService.StartSpider(runningConfig);
            }
            else
            {
                SinaSpiderService.StopSpider();
            }
        }

        /// <summary>
        /// 关注采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGatherFoucs_Click(object sender, EventArgs e)
        {
            if (!SinaSpiderService.IsSpiderStarted)
            {
                var runningConfig = GetSpiderRunningConfig();
                runningConfig.GatherType = GatherTypeEnum.MyFocusGather;
                SinaSpiderService.StartSpider(runningConfig);
            }
            else
            {
                SinaSpiderService.StopSpider();
            }
        }

        /// <summary>
        /// 智能分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoGather_Click(object sender, EventArgs e)
        {
            if (!SinaSpiderService.IsSpiderStarted)
            {
                var userIds = new List<string>();
                foreach (var item in this.lstUser.SelectedItems)
                {
                    var u = item.ToString();
                    if (!userIds.Contains(u)) userIds.Add(u);
                }

                var runningConfig = GetSpiderRunningConfig();
                runningConfig.UserIds = userIds.ToArray();
                runningConfig.GatherType = GatherTypeEnum.HeFocusGather;

                SinaSpiderService.StartSpider(runningConfig);
            }
            else
            {
                SinaSpiderService.StopSpider();
            }
        }

        #endregion

        #region 已采集数据操作

        bool ResetLoadCacheTask = false;
        string LoadCacheName = string.Empty;

        private void cbxName_Leave(object sender, EventArgs e)
        {
            var selectName = this.cbxName.Text.Trim();
            if (string.IsNullOrEmpty(selectName)) return;

            if (!string.IsNullOrEmpty(LoadCacheName) && LoadCacheName != selectName)
            {
                LoadCacheName = selectName;
                ResetLoadCacheTask = true;
            }
            else
            {
                LoadCacheName = selectName;
            }

            Task.Factory.StartNew(() => {
                LoadCacheUserTask();
            });
        }

        private void lstUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItems == null) return;

            this.lstArc.Items.Clear();

            foreach (ListViewItem selectItem in this.lstUser.SelectedItems)
            {
                var userId = selectItem.SubItems[0].Text;
                var userPath = PathUtil.GetStoreImageUserPath(LoadCacheName, userId);
                var userUrl = SinaUrlUtil.GetSinaUserUrl(userId);

                this.txtStartUrl.Text = userUrl;

                //LoadCacheUserStatusList(userPath, userId);
                LoadCacheUserStatusList(userId);

                if(this.lstArc.Items.Count > 0)
                {
                    this.lstArc.Items[0].Selected = true;
                }
            }
        }

        private void lstArc_SelectedIndexChanged(object sender, EventArgs e)
        {
            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            var files = PathUtil.GetStoreImageFiles(LoadCacheName, uid, bid);
            if (files.Length > 0)
            {
                ShowImage(files, 0);
            }
            else
            {
                ClearImage();
            }

            this.lblImgCount.Text = $"【{files.Length} 张图片】";

            if (this.txtShowImg.Checked)
            {
                ActiveImageCtl();
            }
        }

        string GetSelectUserId()
        {
            if (this.lstUser.SelectedItems == null) return null;

            return this.lstUser.SelectedItems[0].SubItems[0].Text;
        }

       string GetSelectStatusId()
        {
            if (this.lstArc.SelectedItems == null || this.lstArc.SelectedItems.Count == 0) return string.Empty;

            return this.lstArc.SelectedItems[0].SubItems[0].Text;
        }

        private void btnBrowseUser_Click(object sender, EventArgs e)
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var url = SinaUrlUtil.GetSinaUserUrl(userId);
            System.Diagnostics.Process.Start(url);
        }

        private void btnMarkUser_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认已存档当前用户的所有图集?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var user = GetSelectUserId();
            if (!string.IsNullOrEmpty(user))
            {
                var rep = new SinaRepository();
                rep.ArchiveSinaUser(user);
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            if (MessageBox.Show("确认删除当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var suc = rep.DeleteSinaUser(userId);
            if (suc)
            {
                var userPath = PathUtil.GetStoreImageUserPath(LoadCacheName, userId);
                if (Directory.Exists(userPath)) Directory.Delete(userPath, true);

                if(this.lstUser.Items.Count > 0)
                {
                    this.lstUser.Items[0].Selected = true;
                }
            }
        }
        private void btnNewUser_Click(object sender, EventArgs e)
        {
            var user = this.txtUserFilter.Text.Trim();
            if (string.IsNullOrEmpty(user)) return;

            if (MessageBox.Show("确认添加当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            if (!rep.ExistsSinaUser(user))
            {
                var suc = rep.CreateSinaUser(new SinaUser()
                {
                    uid = user,
                    groupname = LoadCacheName
                });
                if (suc)
                {
                    this.lstUser.Items.Insert(0, user);
                    this.lstUser.Items[0].Selected = true;
                }
            }
            else
            {
                var selectItem = this.lstUser.FindItemWithText(user, true, 0, true);
                if(selectItem != null)
                {
                    this.lstUser.Items[selectItem.Index].Selected = true;
                }
            }
        }

        private void btnIgnoreUser_Click(object sender, EventArgs e)
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            if (MessageBox.Show("确认拉黑当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var suc = rep.IgnoreSinaUser(userId);
            if (suc)
            {
                var userPath = PathUtil.GetStoreImageUserPath(LoadCacheName, userId);
                if (Directory.Exists(userPath)) Directory.Delete(userPath, true);

                if (this.lstUser.Items.Count > 0)
                {
                    this.lstUser.Items[0].Selected = true;
                }
            }
        }

        private void btnBrowseStatus_Click(object sender, EventArgs e)
        {
            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            if (!string.IsNullOrEmpty(uid))
            {
                var url = SinaUrlUtil.GetSinaUserStatusUrl(bid);
                System.Diagnostics.Process.Start(url);
            }
        }

        private void btnOpenStatus_Click(object sender, EventArgs e)
        {
            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            if (!string.IsNullOrEmpty(uid))
            {
                var path = PathUtil.GetStoreImageUserStatusPath(LoadCacheName, uid, bid);

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                psi.Arguments = "/e,/select," + path;
                System.Diagnostics.Process.Start(psi);
            }
        }

        private void btnGetStatusByBid_Click(object sender, EventArgs e)
        {
            var bid = GetSelectStatusId();
            if (!string.IsNullOrEmpty(bid))
            {
                if (!SinaSpiderService.IsSpiderStarted)
                {
                    this.txtStartUrl.Text = $"https://m.weibo.cn/status/{bid}";
                    SinaSpiderService.StartSpider(GetSpiderRunningConfig());
                }
            }
        }

        private void btnIgnoreStatus_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认拉黑当前图集?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            if (!string.IsNullOrEmpty(uid))
            {
                var rep = new SinaRepository();
                var suc = rep.IgnoreSinaStatus(bid);
                if (suc)
                {
                    var userStatusPath = PathUtil.GetStoreImageUserStatusPath(LoadCacheName, uid, bid);
                    if (Directory.Exists(userStatusPath)) Directory.Delete(userStatusPath, true);

                    if(this.lstArc.Items.Count > 0)
                    {
                        this.lstArc.Items[0].Selected = true;
                    }
                }
            }
        }

        private void btnFollowerUser_Click(object sender, EventArgs e)
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var url = SinaUrlUtil.GetSinaUserFollowerUrl(userId);
            System.Diagnostics.Process.Start(url);
        }




        private void btnArchiveStatus_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认已存档当前图集?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            if (!string.IsNullOrEmpty(uid))
            {
                var rep = new SinaRepository();
                rep.ArchiveSinaStatus(bid);
            }
        }

        private void btnFoucsUser_Click(object sender, EventArgs e)
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            if (MessageBox.Show("确认关注当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            rep.FocusSinaUser(userId);
        }

        private void txtStartUrl_TextChanged(object sender, EventArgs e)
        {
            var userId = SinaUrlUtil.GetSinaUserByStartUrl(this.txtStartUrl.Text.Trim());
            if (!string.IsNullOrEmpty(userId))
            {
                var listItem = this.lstUser.FindItemWithText(userId, true, 0, true);
                if (listItem != null)
                {
                    this.lstUser.Items[listItem.Index].Selected = true;
                }
            }
        }

        private void txtUserFilter_TextChanged(object sender, EventArgs e)
        {
            var keyword = this.txtUserFilter.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadCacheUserList(LoadCacheName);
            }
            else
            {
                FilterUserIds(keyword);
            }
        }

        private void txtUserFilter_DoubleClick(object sender, EventArgs e)
        {
            this.txtUserFilter.Clear();
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            if (this.btnLock.Tag == null)
            {
                this.btnLock.Tag = "lock";
                LockImageCtl(false);
            }
            else
            {
                this.btnLock.Tag = null;
                LockImageCtl(true);
            }
        }

        private void btnFocusConfig_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoadCacheName)) return;

            FocusConfigForm frm = new FocusConfigForm();
            frm.SpiderName = LoadCacheName;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();

            if (!string.IsNullOrEmpty(frm.GatherUser))
            {
                this.txtStartUrl.Text = SinaUrlUtil.GetSinaUserUrl(frm.GatherUser);
            }
        }

        private void btnIgnoreConfig_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoadCacheName)) return;

            IgnoreConfigForm frm = new IgnoreConfigForm();
            frm.SpiderName = LoadCacheName;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();

            if (!string.IsNullOrEmpty(frm.GatherUser))
            {
                this.txtStartUrl.Text = SinaUrlUtil.GetSinaUserUrl(frm.GatherUser);
            }
        }

        private void btnAnlayse_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoadCacheName)) return;

            AnalyseConfigForm frm = new AnalyseConfigForm();
            frm.SpiderName = LoadCacheName;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        private void btnManager_Click(object sender, EventArgs e)
        {
            CacheImageViewForm frm = new CacheImageViewForm();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Size = new Size(1400, 768);
            frm.Show();
        }

        private void label5_DoubleClick(object sender, EventArgs e)
        {
            SortByDate = !SortByDate;

            this.lstUser.Items.Clear();
        }
        #endregion

        #region 图集功能操作
        /// <summary>
        /// 图片预览上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlLeft_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Tag == null) return;
            int imgIndex = 0;
            int.TryParse(this.pictureBox1.Tag.ToString(), out imgIndex);

            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            if (!string.IsNullOrEmpty(bid))
            {
                var files = PathUtil.GetStoreImageFiles(LoadCacheName, uid, bid);
                if (files.Length > 0 && imgIndex > 0)
                {
                    imgIndex -= 1;
                    ShowImage(files, imgIndex);
                }
            }
        }

        /// <summary>
        /// 图片预览下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlRight_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Tag == null) return;
            int imgIndex = 0;
            int.TryParse(this.pictureBox1.Tag.ToString(), out imgIndex);

            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            if (!string.IsNullOrEmpty(bid))
            {
                var files = PathUtil.GetStoreImageFiles(LoadCacheName, uid, bid);
                if (files.Length > 0 && imgIndex < files.Length - 1)
                {
                    imgIndex += 1;
                    ShowImage(files, imgIndex);
                }
            }
        }

        /// <summary>
        /// 图片预览原图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Tag == null) return;
            int imgIndex = 0;
            int.TryParse(this.pictureBox1.Tag.ToString(), out imgIndex);

            var uid = GetSelectUserId();
            var bid = GetSelectStatusId();

            if (!string.IsNullOrEmpty(bid))
            {
                var files = PathUtil.GetStoreImageFiles(LoadCacheName, uid, bid);
                if (files.Length > 0 && imgIndex >= 0 && imgIndex <= files.Length - 1)
                {
                    ViewImgForm frm = new ViewImgForm();
                    frm.ViewImgPath = files[imgIndex];
                    frm.ShowDialog();
                }
            }
        }
        #endregion

        #region 私有方法

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

        void InitSpiderRunningConfig()
        {
            DataTable dt = new DataTable();
            var columns = new string[] { "配置项", "配置值" };
            foreach (var column in columns)
            {
                dt.Columns.Add(new DataColumn(column));
            }
            var dr = dt.NewRow();
            dr["配置项"] = "起始采集页码";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "最大采集页数";
            dr["配置值"] = "3";
            dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr["配置项"] = "每页等待秒数";
            //dr["配置值"] = "5";
            //dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr["配置项"] = "每条等待秒数";
            //dr["配置值"] = "5";
            //dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "最少图片数量";
            dr["配置值"] = "3";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "只采集本用户";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "只读关注用户";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "只采集用户数";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "只采集微博数";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "只采集图片数";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "图片最小尺寸";
            dr["配置值"] = "480";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "图片最大尺寸";
            dr["配置值"] = "8000";
            dt.Rows.Add(dr);

            this.dataGridView1.DataSource = dt.DefaultView;
            this.dataGridView1.Columns[0].Width = 120;
            this.dataGridView1.Columns[1].Width = 120;
        }

        SpiderRunningConfig GetSpiderRunningConfig()
        {
            var runningConfig = new SpiderRunningConfig()
            {
                StartUrl = this.txtStartUrl.Text.Trim(),
                Name = this.cbxName.Text.Trim()
            };
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (row.Cells["配置项"].Value == null) continue;

                if (row.Cells["配置项"].Value.ToString() == "每条等待秒数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadNextStatusWaitSecond = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "每页等待秒数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadNextPageWaitSecond = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "最大采集页数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadPageCount = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "起始采集页码")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.StartPageIndex = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "只采集本用户")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.OnlyReadOwnerUser = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "最少图片数量")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadMinOfImgCount = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "图片最小尺寸")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadMinOfImgSize = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "图片最大尺寸")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadMaxOfImgSize = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "只读关注用户")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.OnlyReadFocusUser = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "只采集用户数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.OnlyReadUserInfo = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "只采集微博数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.OnlyReadUserStatus = intValue;
                }
                else if(row.Cells["配置项"].Value.ToString() == "只采集图片数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue); ;
                    runningConfig.OnlyReadUserPicture = intValue;
                }
            }
            return runningConfig;
        }

        void UpdateSpiderRunningConfig(string key, string value)
        {
            InvokeControl(this.dataGridView1, new Action(() =>
            {
                var dv = this.dataGridView1.DataSource as DataView;
                var drs = dv.Table.Select("配置项='" + key + "'");
                if (drs.Length > 0)
                {
                    drs[0]["配置值"] = value;
                }
            }));
        }

        public int GetDirectoryCount(string path)
        {
            int count = 0;
            foreach (var folder in Directory.GetDirectories(path))
            {
                count += Directory.GetDirectories(folder).Length;
            }
            return count;
        }

        public int GetFilesCount(string path)
        {
            int count = Directory.GetFiles(path).Where(c => c.EndsWith(".jpg")).Count();
            foreach (var folder in Directory.GetDirectories(path))
            {
                count += GetFilesCount(folder);
            }
            return count;
        }

        void FilterUserIds(string keyword)
        {
            var searchItems = new List<ListViewItem>();
            foreach (ListViewItem item in this.lstUser.Items)
            {
                if (item.SubItems[0].Text.Contains(keyword))
                {
                    searchItems.Add(item);
                }
            }
            this.lstUser.Items.Clear();
            this.lstUser.Items.AddRange(searchItems.ToArray());
        }

        void ShowImage(string[] files, int index)
        {
            this.pictureBox1.Tag = index;
            var resImg = Image.FromFile(files[index]);
            Image bmp = new Bitmap(resImg);
            resImg.Dispose();
            this.pictureBox1.Image = bmp;
        }

        void ClearImage()
        {
            this.pictureBox1.Image = null;
        }

        /// <summary>
        /// 激活图片控件
        /// </summary>
        void ActiveImageCtl()
        {
            InvokeControl(this.tabControl1, new Action(() =>
            {
                if (this.tabControl1.Enabled == false) return;
                this.tabControl1.SelectedIndex = 0;
            }));
        }

        /// <summary>
        /// 激活日志控件
        /// </summary>
        void ActiveLoggerCtl()
        {
            InvokeControl(this.tabControl1, new Action(() =>
            {
                if (this.tabControl1.Enabled == false) return;
                this.tabControl1.SelectedIndex = 1;
            }));
        }

        /// <summary>
        /// 锁定只显示日志控件
        /// </summary>
        /// <param name="enabled"></param>
        void LockImageCtl(bool enabled)
        {
            if(enabled == false)
            {
                ActiveLoggerCtl();
            }
            this.tabControl1.Enabled = enabled;
        }
        #endregion

        #region 关闭&最小化

        private void SpiderTrackerForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }
        private void SpiderTrackerForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        #endregion

    }
}
