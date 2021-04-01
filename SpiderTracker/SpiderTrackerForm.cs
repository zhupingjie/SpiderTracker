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
         
        List<SinaUser> CacheSinaUsers { get; set; } = new List<SinaUser>();
        List<SinaStatus> CacheSinaStatuss { get; set; } = new List<SinaStatus>();

        SpiderRunningConfig RunningConfig { get; set; }
        public SpiderTrackerForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //开启双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

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

            this.RunningConfig = GetSpiderRunningConfig();

            SinaSpiderService = new MWeiboSpiderService();
            SinaSpiderService.OnShowStatus += WeiboSpiderService_OnShowStatus;
            SinaSpiderService.OnSpiderStarted += WeiboSpiderService_OnSpiderStarted;
            SinaSpiderService.OnSpiderComplete += WeiboSpiderService_OnSpiderComplete;
            SinaSpiderService.OnSpiderStoping += WeiboSpiderService_OnSpiderStoping;
            SinaSpiderService.OnRefreshConfig += WeiboSpiderService_OnRefreshConfig;
            SinaSpiderService.OnGatherNewUser += SinaSpiderService_OnGatherNewUser;
            SinaSpiderService.OnGatherStatusComplete += SinaSpiderService_OnGatherStatusComplete;
            SinaSpiderService.OnGatherPageComplete += SinaSpiderService_OnGatherPageComplete;
            SinaSpiderService.OnGatherUserComplete += SinaSpiderService_OnGatherUserComplete;
            SinaSpiderService.OnGatherAppendUser += SinaSpiderService_OnGatherAppendUser;
            SinaSpiderService.OnGatherUserStarted += SinaSpiderService_OnGatherUserStarted;

            Task.Factory.StartNew(() => {
                LoadCacheNameList();
            });
        }

        #region Spider Event

        private void SinaSpiderService_OnGatherUserStarted(SinaUser user)
        {
            InvokeControl(this.lstRunstate, new Action(() =>
            {
                if (this.lstRunstate.Items.Count == 0) return;

                var listItem = this.lstRunstate.FindItemWithText(user.uid);
                if (listItem != null)
                {
                    listItem.SubItems[4].Text = "...";
                }
            }));
        }

        private void SinaSpiderService_OnGatherAppendUser(SinaUser user)
        {
            InvokeControl(this.lstRunstate, new Action(() =>
            {
                var subItem = new ListViewItem();
                subItem.Text = user.uid;
                subItem.SubItems.Add($"{user.name}");
                subItem.SubItems.Add($"0");
                subItem.SubItems.Add($"0");
                subItem.SubItems.Add($"Waitting");
                this.lstRunstate.Items.Add(subItem);
            }));
        }

        private void SinaSpiderService_OnGatherNewUser(SinaUser user)
        {
            LoadCacheUserList(false);
        }

        private void SinaSpiderService_OnGatherUserComplete(SinaUser user, int readImageQty)
        {
            InvokeControl(this.lstRunstate, new Action(() =>
            {
                if (this.lstRunstate.Items.Count == 0) return;

                var listItem = this.lstRunstate.FindItemWithText(user.uid);
                if (listItem != null)
                {
                    listItem.SubItems[3].Text = $"{readImageQty}";
                    listItem.SubItems[4].Text = "OK";
                }
            }));
        }

        private void SinaSpiderService_OnGatherStatusComplete(string uid, int readImageQty)
        {
            InvokeControl(this.lstRunstate, new Action(() =>
            {
                if (this.lstRunstate.Items.Count == 0) return;

                var listItem = this.lstRunstate.FindItemWithText(uid);
                if (listItem != null)
                {
                    var imageQty = 0;
                    int.TryParse(listItem.SubItems[3].Text, out imageQty);
                    imageQty += readImageQty;
                    listItem.SubItems[3].Text = $"{imageQty}";
                    listItem.SubItems[4].Text = "...";
                }
            }));
        }

        private void SinaSpiderService_OnGatherPageComplete(string uid, int pageIndex, int readImageQty)
        {
            InvokeControl(this.lstRunstate, new Action(() =>
            {
                var listItem = this.lstRunstate.FindItemWithText(uid);
                if (listItem != null)
                {
                    var pageQty = 0;
                    int.TryParse(listItem.SubItems[2].Text, out pageQty);
                    pageQty += 1;
                    listItem.SubItems[2].Text = $"{pageQty}";
                    listItem.SubItems[4].Text = "...";
                }
            }));
        }


        private void WeiboSpiderService_OnRefreshConfig(SpiderRunningConfig spiderRunninConfig)
        {

        }

        private void WeiboSpiderService_OnSpiderStarted(SpiderRunningConfig runningConfig)
        {
            ActiveLoggerCtl();

            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "Stop";
                this.btnSearch.Enabled = true;
            }));

            InvokeControl(this.btnAppendUser, new Action(() =>
            {
                this.btnAppendUser.Text = "追加采集";
                this.btnAppendUser.Enabled = true;
            }));

            InvokeControl(this.lstRunstate, new Action(() =>
            {
                this.lstRunstate.Items.Clear();
                foreach (var user in runningConfig.DoUsers)
                {
                    var subItem = new ListViewItem();
                    subItem.Text = user.uid;
                    subItem.SubItems.Add($"{user.name}");
                    subItem.SubItems.Add($"0");
                    subItem.SubItems.Add($"0");
                    subItem.SubItems.Add($"Waitting");
                    this.lstRunstate.Items.Add(subItem);
                }
            }));
        }

        private void WeiboSpiderService_OnSpiderStoping()
        {
            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "Stop...";
                this.btnSearch.Enabled = false;
            }));

            InvokeControl(this.btnAppendUser, new Action(() =>
            {
                this.btnAppendUser.Text = "Stop...";
                this.btnAppendUser.Enabled = false;
            }));
        }

        private void WeiboSpiderService_OnSpiderComplete()
        {
            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "开始采集";
                this.btnSearch.Enabled = true;
            }));

            InvokeControl(this.btnAppendUser, new Action(() =>
            {
                this.btnAppendUser.Text = "等待开始";
                this.btnAppendUser.Enabled = false;
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

        #region 加载用户及用户微博
        void LoadCacheNameList()
        {
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
                    if (!selectName.Equals("cosplay"))
                    {
                        this.cbxName.Text = selectName;
                    }
                    this.cbxName.EndUpdate();
                }
                if (names.Length > 0)
                {
                    LoadCacheName = names.FirstOrDefault();

                    LoadCacheUserList(true);
                }

            }));
        }

        SinaUser[] GetShowUsers(IList<SinaUser> users)
        {
            switch (this.cbxUserSort.Text)
            {
                case "更新":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.lastdate).ToArray();
                    else
                        users = users.OrderBy(c => c.lastdate).ToArray();
                    break;
                case "微博":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.statuses).ToArray();
                    else
                        users = users.OrderBy(c => c.statuses).ToArray();
                    break;
                case "采集":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.finds).ToArray();
                    else
                        users = users.OrderBy(c => c.finds).ToArray();
                    break;
                case "下载":
                    if (this.cbxUserSortAsc.Text == "下载")
                        users = users.OrderByDescending(c => c.gets).ToArray();
                    else
                        users = users.OrderBy(c => c.gets).ToArray();
                    break;
                case "忽略":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.ignores).ToArray();
                    else
                        users = users.OrderBy(c => c.ignores).ToArray();
                    break;
                case "关注":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.focus).ToArray();
                    else
                        users = users.OrderBy(c => c.focus).ToArray();
                    break;
            }

            //保留显示用户数
            return users.Take(RunningConfig.LoadUserCount).ToArray();
        }
        void LoadCacheUserList(bool reload)
        {
            var keyword = this.txtUserFilter.Text.Trim();
            var users = SinaSpiderService.Repository.GetUsers(LoadCacheName, keyword);

            InvokeControl(this.lstUser, new Action(() =>
            {
                var showUsers = GetShowUsers(users);

                this.lstUser.BeginUpdate();
                if (reload) this.lstUser.Items.Clear();

                foreach (var item in showUsers)
                {
                    if (this.lstUser.Items.Count > 0 && !reload)
                    {
                        var listItem = this.lstUser.FindItemWithText(item.uid);
                        if (listItem != null) continue;
                    }
                    var subItem = new ListViewItem();
                    subItem.Text = item.uid;
                    subItem.SubItems.Add(item.name);
                    subItem.SubItems.Add($"{item.statuses}");
                    subItem.SubItems.Add($"{item.finds}");
                    subItem.SubItems.Add($"{item.gets}");
                    subItem.SubItems.Add($"{item.ignores}");
                    subItem.SubItems.Add($"{item.follows}");
                    subItem.SubItems.Add($"{(item.focus > 0 ? "◉" : "")}");
                    subItem.Tag = item;
                    this.lstUser.Items.Add(subItem);
                }
                this.lstUser.EndUpdate();
                this.lblLstUserCount.Text = $"{users.Count}";
            }));
        }

        void LoadCacheUserStatusList(SinaUser user)
        {
            var statuses = SinaSpiderService.Repository.GetUserStatuses(user.uid);
            this.CacheSinaStatuss.Clear();
            this.CacheSinaStatuss.AddRange(statuses);
            this.BindUserStatusList(statuses, user.uid, user.gets);
        }

        void BindUserStatusList(List<SinaStatus> sinaStatus, string user, int newQty)
        {
            InvokeControl(this.lstArc, new Action(() =>
            {
                this.lstArc.BeginUpdate();
                this.lstArc.Items.Clear();
                var localImg = 0;
                foreach (var item in sinaStatus.OrderByDescending(c => c.lastdate).ToArray())
                {
                    var subItem = new ListViewItem();
                    subItem.Tag = item;
                    subItem.Text = $"{item.bid}";
                    subItem.SubItems.Add($"{item.qty}");
                    var local = 0;
                    if (item.mtype == 0)
                    {
                        var files = PathUtil.GetStoreUserThumbnailImageFiles(LoadCacheName, user, item.bid);
                        local = files.Length;
                        localImg += files.Length;
                    }
                    else if (item.mtype == 1)
                    {
                        var file = PathUtil.GetStoreUserVideoFile(LoadCacheName, user, item.bid);
                        local = 1;
                        localImg += 1;
                    }
                    subItem.SubItems.Add($"{local}");
                    subItem.SubItems.Add($"{item.archive}");
                    this.lstArc.Items.Add(subItem);
                }
                this.lstArc.EndUpdate();
                this.lblLstStatusCount.Text = $"{sinaStatus.Count}";
                this.lblLstImgCount.Text = $"{localImg}";
                this.lblLstArchiveCount.Text = $"{sinaStatus.Sum(c => c.archive)}";
                this.lblLstStatusImageCount.Text = $"{sinaStatus.Sum(c => c.qty)}";
                this.lblLstGetImgCount.Text = $"{sinaStatus.Sum(c => c.getqty)}";
            }));
        }

        #endregion

        #region 用户列表及用户微博事件

        private void sltLastDate_CheckedChanged(object sender, EventArgs e)
        {
            LoadCacheUserList(true);
        }

        private void sltUserID_CheckedChanged(object sender, EventArgs e)
        {
            LoadCacheUserList(true);
        }

        private void sltImageQty_CheckedChanged(object sender, EventArgs e)
        {
            LoadCacheUserList(true);
        }

        private void sltNewQty_CheckedChanged(object sender, EventArgs e)
        {
            LoadCacheUserList(true);
        }

        private void sltFocus_CheckedChanged(object sender, EventArgs e)
        {
            LoadCacheUserList(true);
        }


        private void btnFocusUser_Click(object sender, EventArgs e)
        {
            this.FocusUser();
        }

        private void btnIgnoreUser_Click(object sender, EventArgs e)
        {
            this.IgnoreUser(true);
        }

        private void cbxUserSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCacheUserList(true);
        }

        private void cbxUserSortAsc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCacheUserList(true);
        }
        private void lstUser_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.IgnoreUser(true);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                this.FocusUser();
            }
        }

        private void lstUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItems == null || this.lstUser.SelectedItems.Count == 0) return;
            if (this.lstUser.SelectedItems.Count > 1) return;

            var user = GetSelectUser();
            var userId = GetSelectUserId();
            var userUrl = SinaUrlUtil.GetSinaUserUrl(userId);

            this.txtStartUrl.Text = userUrl;

            LoadCacheUserStatusList(user);
        }

        private void lstArc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RunningConfig.PreviewImageNow == 1)
            {
                if (this.lstArc.SelectedItems.Count > 1) return;

                var uid = GetSelectUserId();
                var status = GetSelectStatus();
                if (status == null) return;
                if (status.mtype == 0)
                {
                    ActiveImageCtl();

                    var files = PathUtil.GetStoreUserThumbnailImageFiles(LoadCacheName, uid, status.bid);
                    this.imagePreviewUC1.ShowImages(files, 0, RunningConfig.PreviewImageCount, LoadCacheName, uid, status.bid, RunningConfig.DefaultArchivePath);
                }
                else if (status.mtype == 1)
                {
                    ActiveVedioCtl();

                    var file = PathUtil.GetStoreUserVideoFile(LoadCacheName, uid, status.bid);
                    if (File.Exists(file))
                    {
                        this.vedioPlayerUC1.ShowVideo(file);
                    }
                }
            }
            else
            {
                ActiveLoggerCtl();
            }

        }


        string LoadCacheName = string.Empty;
        private void cbxName_Leave(object sender, EventArgs e)
        {
            var selectName = this.cbxName.Text.Trim();
            if (string.IsNullOrEmpty(selectName)) return;

            LoadCacheName = selectName;

            Task.Factory.StartNew(() => {
                LoadCacheUserList(true);
            });
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
            var user = GetSelectUserId();
            if (MessageBox.Show($"确认已存档当前用户的所有微博[{user}]?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            if (!string.IsNullOrEmpty(user))
            {
                var rep = new SinaRepository();
                rep.ArchiveSinaUser(user);

                var status = rep.GetUserStatuseOfNoArchives(user);
                if (status.Count > 0)
                {
                    ArchiveStatus(user, status.ToArray());
                }
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
                var userPath = PathUtil.GetStoreUserPath(LoadCacheName, userId);
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
                var path = PathUtil.GetStoreUserPath(LoadCacheName, uid);
                if (Directory.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    path = PathUtil.BaseDirectory;
                    System.Diagnostics.Process.Start(path);
                }
            }
        }

        private void btnGetStatusByBid_Click(object sender, EventArgs e)
        {
            var statusIds = new List<string>();
            var status = GetSelectStatuss();
            foreach (var item in status)
            {
                statusIds.Add(item.bid);
            }
            if (!SinaSpiderService.IsSpiderStarted)
            {
                this.RunningConfig.Category = LoadCacheName;
                this.RunningConfig.GatherType = GatherTypeEnum.GahterStatus;
                SinaSpiderService.StartSpider(this.RunningConfig, null, statusIds);
            }
        }

        private void btnIgnoreStatus_Click(object sender, EventArgs e)
        {
            this.IgnoreStatus(true);
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
            var uid = GetSelectUserId();
            var status = GetSelectStatuss();
            if (status.Length == 0) return;

            if (MessageBox.Show($"确认已存档当前选中的[{status.Length}]个微博?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            foreach (var item in status)
            {
                rep.ArchiveSinaStatus(item.bid);

                if (this.lstArc.Items.Count > 0)
                {
                    var listItem = this.lstArc.FindItemWithText(item.bid);
                    if (listItem != null)
                    {
                        listItem.SubItems[3].Text = "✔";
                    }
                    var archiveQty = 0;
                    int.TryParse(this.lblLstArchiveCount.Text, out archiveQty);
                    archiveQty += 1;
                    this.lblLstArchiveCount.Text = $"{archiveQty} ";
                }
            }
            ArchiveStatus(uid, status);
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
                if (this.lstUser.Items.Count == 0) return;

                var listItem = this.lstUser.FindItemWithText(userId);
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
                LoadCacheUserList(true);
            }
            else
            {
                FilterUserIds(keyword);
            }
        }

        private void txtUserFilter_Leave(object sender, EventArgs e)
        {
            var user = this.txtUserFilter.Text.Trim();
            if (string.IsNullOrEmpty(user)) return;
            if (user.Length != 10) return;

            //if (MessageBox.Show("确认添加当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var sinaUser = rep.GetUser(user);
            if (sinaUser == null)
            {
                sinaUser = new SinaUser()
                {
                    uid = user,
                    category = LoadCacheName
                };
                var suc = rep.CreateSinaUser(sinaUser);
                if (suc)
                {
                    var subItem = new ListViewItem();
                    subItem.Text = sinaUser.uid;
                    subItem.SubItems.Add(sinaUser.name);
                    subItem.SubItems.Add($"{sinaUser.finds}");
                    subItem.Tag = sinaUser;
                    this.lstUser.Items.Insert(0, subItem);
                    this.lstUser.Items[0].Selected = true;
                }
            }
            else
            {
                if (sinaUser.ignore > 0)
                {
                    sinaUser.ignore = 0;
                    rep.UpdateSinaUser(sinaUser, new string[] { "ignore" });

                    var subItem = new ListViewItem();
                    subItem.Text = sinaUser.uid;
                    subItem.SubItems.Add(sinaUser.name);
                    subItem.SubItems.Add($"{sinaUser.finds}");
                    subItem.Tag = sinaUser;
                    this.lstUser.Items.Insert(0, subItem);
                    this.lstUser.Items[0].Selected = true;
                }
                else
                {
                    if (this.lstUser.Items.Count == 0) return;

                    var selectItem = this.lstUser.FindItemWithText(user);
                    if (selectItem != null)
                    {
                        this.lstUser.Items[selectItem.Index].Selected = true;
                    }
                }
            }
        }

        private void txtUserFilter_DoubleClick(object sender, EventArgs e)
        {
            this.txtUserFilter.Clear();

            Task.Factory.StartNew(() => {
                LoadCacheUserList(true);
            });
        }


        private void txtStatusFilter_TextChanged(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            var userid = GetSelectUserId();
            if (string.IsNullOrEmpty(userid)) return;

            var keyword = this.txtStatusFilter.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadCacheUserStatusList(user);
            }
            else
            {
                FilterStatusIds(keyword, user);
            }
        }

        private void txtStatusFilter_DoubleClick(object sender, EventArgs e)
        {
            this.txtStatusFilter.Clear();

            var user = GetSelectUser();

            Task.Factory.StartNew(() =>
            {
                LoadCacheUserStatusList(user);
            });
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            if (this.btnLock.Tag == null)
            {
                RunningConfig.PreviewImageNow = 0;
                this.btnLock.Tag = "lock";
                LockImageCtl(false);
            }
            else
            {
                RunningConfig.PreviewImageNow = 1;

                this.btnLock.Tag = null;
                LockImageCtl(true);
            }
        }

        private void lstRunstate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstUser.Items.Count == 0) return;
            if (this.lstRunstate.SelectedItems.Count == 0) return;
            var selectItem = this.lstRunstate.SelectedItems[0];
            var uid = selectItem.SubItems[0].Text;

            var lstItem = this.lstUser.FindItemWithText(uid);
            if (lstItem != null)
            {
                foreach (ListViewItem item in this.lstUser.SelectedItems)
                {
                    item.Selected = false;
                }
                this.lstUser.Items[lstItem.Index].Selected = true;
            }
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
                var startUrl = this.txtStartUrl.Text;
                var users = new List<SinaUser>();
                foreach (ListViewItem item in this.lstUser.SelectedItems)
                {
                    var user = item.Tag;
                    if (user != null)
                    {
                        users.Add(user as SinaUser);
                    }
                }
                var statusIds = new List<string>();
                if(SinaUrlUtil.GetSinaUrlEnum(startUrl) == SinaUrlEnum.StatusUrl)
                {
                    statusIds.Add(startUrl);

                    this.RunningConfig.GatherType = GatherTypeEnum.GahterStatus;
                }
                this.RunningConfig.Category = LoadCacheName;
                SinaSpiderService.StartSpider(this.RunningConfig, users, statusIds);
            }
            else
            {
                SinaSpiderService.StopSpider();
            }
        }
        private void btnAppendUser_Click(object sender, EventArgs e)
        {
            if (SinaSpiderService.IsSpiderStarted)
            {
                foreach (ListViewItem item in this.lstUser.SelectedItems)
                {
                    var user = item.Tag;
                    if (user != null)
                    {
                        SinaSpiderService.AppendUser(user as SinaUser);
                    }
                }
            }
        }

        #endregion

        #region 用户及微博功能操作

        void IgnoreUser(bool confirm)
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            if (confirm && MessageBox.Show($"确认拉黑当前用户[{userId}]?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var suc = rep.IgnoreSinaUser(userId);
            if (suc)
            {
                var userPath = PathUtil.GetStoreUserPath(LoadCacheName, userId);
                if (Directory.Exists(userPath)) Directory.Delete(userPath, true);

                if (this.lstUser.Items.Count == 0) return;

                var listItem = this.lstUser.FindItemWithText(userId);
                if (listItem != null)
                {
                    var index = listItem.Index;
                    this.lstUser.Items.Remove(listItem);
                    if (this.lstUser.Items.Count > 0)
                    {
                        if (this.lstUser.Items.Count <= index)
                        {
                            this.lstUser.Items[this.lstUser.Items.Count - 1].Selected = true;
                        }
                        else
                        {
                            this.lstUser.Items[index].Selected = true;
                        }
                    }
                    this.lblLstUserCount.Text = $"{this.lstUser.Items.Count}";
                }
            }
        }

        void FocusUser()
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            if (MessageBox.Show($"确认关注当前用户[{userId}]?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var focus = rep.FocusSinaUser(userId);
            if (this.lstUser.Items.Count == 0) return;

            var listItem = this.lstUser.FindItemWithText(userId);
            if (listItem != null)
            {
                listItem.SubItems[3].Text = (focus ? "◉" : "");
            }
        }

        void IgnoreStatus(bool confirm)
        {
            var uid = GetSelectUserId();
            var status = GetSelectStatuss();

            if (confirm && MessageBox.Show($"确认拉黑当前选中的[{status.Length}]个微博?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            foreach (var item in status)
            {
                rep.IgnoreSinaStatus(item.bid);
                if (item.mtype == 0)
                {
                    PathUtil.DeleteStoreUserImageFiles(LoadCacheName, uid, item.bid);
                }
                else if(item.mtype == 1)
                {
                    PathUtil.DeleteStoreUserVideoFile(LoadCacheName, uid, item.bid);
                }
                if (this.lstArc.Items.Count > 0)
                {
                    var listItem = this.lstArc.FindItemWithText(item.bid);
                    if (listItem != null)
                    {
                        var index = listItem.Index;
                        var local = 0;
                        int.TryParse(listItem.SubItems[2].Text, out local);
                        var archive = listItem.SubItems[3].Text;

                        this.lstArc.Items.Remove(listItem);
                        if (this.lstArc.Items.Count > 0)
                        {
                            if (this.lstArc.Items.Count <= index)
                            {
                                this.lstArc.Items[this.lstArc.Items.Count - 1].Selected = true;
                            }
                            else
                            {
                                this.lstArc.Items[index].Selected = true;
                            }
                        }
                        var localStatus = 0;
                        int.TryParse(this.lblLstStatusCount.Text, out localStatus);
                        localStatus -= 1;
                        this.lblLstStatusCount.Text = $"{localStatus} ";
                        var localImg = 0;
                        int.TryParse(this.lblLstImgCount.Text, out localImg);
                        localImg -= local;
                        this.lblLstImgCount.Text = $"{localImg} ";

                        if (archive == "✔")
                        {
                            var archiveQty = 0;
                            int.TryParse(this.lblLstArchiveCount.Text, out archiveQty);
                            archiveQty -= 1;
                            this.lblLstArchiveCount.Text = $"{archiveQty} ";
                        }
                    }
                }
            }
        }

        SinaUser GetSelectUser()
        {
            if (this.lstUser.SelectedItems == null || this.lstUser.SelectedItems.Count == 0) return null;

            var obj = this.lstUser.SelectedItems[0].Tag;
            if (obj == null) return null;

            return obj as SinaUser;
        }

        string GetSelectUserId()
        {
            if (this.lstUser.SelectedItems == null || this.lstUser.SelectedItems.Count == 0) return null;

            return this.lstUser.SelectedItems[0].SubItems[0].Text;
        }

        SinaStatus GetSelectStatus()
        {
            if (this.lstArc.SelectedItems == null || this.lstArc.SelectedItems.Count == 0) return null;

            var obj = this.lstArc.SelectedItems[0].Tag;
            if (obj == null) return null;

            return obj as SinaStatus;
        }

       string GetSelectStatusId()
        {
            if (this.lstArc.SelectedItems == null || this.lstArc.SelectedItems.Count == 0) return string.Empty;

            return this.lstArc.SelectedItems[0].SubItems[0].Text;
        }
        SinaStatus[] GetSelectStatuss()
        {
            if (this.lstArc.SelectedItems == null || this.lstArc.SelectedItems.Count == 0) return new SinaStatus[] { };

            var ids = new List<SinaStatus>();
            foreach(ListViewItem item in this.lstArc.SelectedItems)
            {
                if (item.Tag == null) continue;
                ids.Add(item.Tag as SinaStatus);
            }
            return ids.ToArray();
        }

        void ArchiveStatus(string uid, SinaStatus[] status)
        {
            var archivePath = Path.Combine(PathUtil.BaseDirectory, RunningConfig.DefaultArchivePath);
            if (!Directory.Exists(archivePath)) Directory.CreateDirectory(archivePath);

            foreach(var item in status)
            {
                if (item.mtype == 0)
                {
                    var files = PathUtil.GetStoreUserImageFiles(LoadCacheName, uid, item.bid);
                    foreach (var file in files.Select(c => new FileInfo(c)).ToArray())
                    {
                        var destFile = Path.Combine(archivePath, file.Name);
                        file.CopyTo(destFile, true);
                    }
                }
                else if(item.mtype == 1)
                {
                    var path = PathUtil.GetStoreUserVideoFile(LoadCacheName, uid, item.bid);
                    if (File.Exists(path))
                    {
                        var destFile = Path.Combine(archivePath, new FileInfo(path).Name);
                        File.Copy(path, destFile);
                    }
                }
            }
        }

        #endregion

        #region 微博功能操作

        void UpSelectStatus()
        {
            var index = 0;
            if (this.lstArc.SelectedItems.Count == 0)
            {
                index = 1;
            }
            else
            {
                index = this.lstArc.SelectedItems[0].Index;

                this.lstArc.Items[index].Selected = false;
            }
            if (index > 0)
            {
                index -= 1;
                this.lstArc.Items[index].Selected = true;
            }
            else
            {
                this.lstArc.Items[0].Selected = true;
            }
        }

        void DownSelectStatus()
        {
            var index = 0;
            if(this.lstArc.SelectedItems.Count == 0)
            {
                index = -1;
            }
            else
            {
                index = this.lstArc.SelectedItems[0].Index;

                this.lstArc.Items[index].Selected = false;
            }
            if(index < this.lstArc.Items.Count - 1)
            {
                index += 1;
                this.lstArc.Items[index].Selected = true;
            }
            else if(this.lstArc.Items.Count > 0)
            {
                this.lstArc.Items[this.lstArc.Items.Count - 1].Selected = true;
            }
        }

        private void lstArc_KeyDown(object sender, KeyEventArgs e)
        {
             if(e.KeyCode == Keys.W)
            {
                this.UpSelectStatus();
            }
            else if(e.KeyCode == Keys.S)
            {
                this.DownSelectStatus();
            }
            else if(e.KeyCode == Keys.Delete)
            {
                this.IgnoreStatus(false);
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
            dr["配置项"] = "并发用户数量";
            dr["配置值"] = "3";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "起始采集页码";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "最大采集页数";
            dr["配置值"] = "10";
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
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "忽略存档微博";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集用户视频";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集用户图集";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集原创微博";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集所有用户";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集所有关注";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集用户关注";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集用户名称";
            dr["配置值"] = "";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "图片最小尺寸";
            dr["配置值"] = "600";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "图片最大尺寸";
            dr["配置值"] = "99999";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "显示用户数量";
            dr["配置值"] = "100";
            dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr["配置项"] = "缩略图宽度";
            //dr["配置值"] = "140";
            //dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr["配置项"] = "缩略图高度";
            //dr["配置值"] = "190";
            //dt.Rows.Add(dr);

            
            //dr = dt.NewRow();
            //dr["配置项"] = "预览图片数量";
            //dr["配置值"] = "6";
            //dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr["配置项"] = "预览显示资源";
            //dr["配置值"] = "1";
            //dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr["配置项"] = "默认归档路径";
            //dr["配置值"] = "archive";
            //dt.Rows.Add(dr);

            this.dataGridView1.DataSource = dt.DefaultView;
            this.dataGridView1.Columns[0].Width = 120;
            this.dataGridView1.Columns[1].Width = 120;
        }

        SpiderRunningConfig GetSpiderRunningConfig()
        {
            if (this.RunningConfig == null)
            {
                RunningConfig = new SpiderRunningConfig()
                {
                    StartUrl = this.txtStartUrl.Text.Trim(),
                    Category = this.cbxName.Text.Trim(),
                    Id = DateTime.Now.Ticks
                };
            }
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (row.Cells["配置项"].Value == null) continue;

                if (row.Cells["配置项"].Value.ToString() == "每条等待秒数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadNextStatusWaitSecond = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "每页等待秒数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadNextPageWaitSecond = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "最大采集页数")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadPageCount = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "起始采集页码")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.StartPageIndex = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "采集原创微博")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.OnlyReadOwnerUser = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "最少图片数量")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadMinOfImgCount = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "图片最小尺寸")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadMinOfImgSize = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "图片最大尺寸")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadMaxOfImgSize = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "采集用户名称")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    RunningConfig.ReadUserNameLike = strValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "采集所有用户")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadAllOfUser = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "采集用户关注")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadUserOfHeFocus = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "采集所有关注")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ReadUserOfMyFocus = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "忽略存档微博")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue); ;
                    RunningConfig.IgnoreReadArchiveStatus = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "预览图片数量")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.PreviewImageCount = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "预览显示资源")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.PreviewImageNow = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "并发用户数量")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);                    
                    RunningConfig.MaxReadUserThreadCount = intValue;
                }
                else if(row.Cells["配置项"].Value.ToString() == "默认归档路径")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    RunningConfig.DefaultArchivePath = strValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "显示用户数量")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.LoadUserCount = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "缩略图宽度")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ThumbnailImageWidth = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "缩略图高度")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    RunningConfig.ThumbnailImageHeight = intValue;
                }
            }
            return RunningConfig;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.RunningConfig = GetSpiderRunningConfig();
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
            var searchUsers = CacheSinaUsers.Where(c => c.uid.Contains(keyword) || c.name.Contains(keyword)).ToArray();

            InvokeControl(this.lstUser, new Action(() =>
            {
                this.lstUser.Items.Clear();
                this.lstUser.BeginUpdate();
                foreach (var item in searchUsers)
                {
                    var subItem = new ListViewItem();
                    subItem.Text = item.uid;
                    subItem.SubItems.Add(item.name);
                    subItem.SubItems.Add($"{item.finds}");
                    subItem.SubItems.Add($"{item.gets}");
                    subItem.SubItems.Add($"{(item.focus > 0 ? "◉" : "")}");
                    subItem.Tag = item;
                    this.lstUser.Items.Add(subItem);
                }
                this.lstUser.EndUpdate();
            }));
            InvokeControl(this.lstArc, new Action(() =>
            {
                this.lstArc.Items.Clear();
            }));
        }

        void FilterStatusIds(string keyword, SinaUser user)
        {
            var searchStatuss = CacheSinaStatuss.Where(c => c.bid.ToUpper().Contains(keyword.ToUpper())).ToList();
            this.BindUserStatusList(searchStatuss, user.uid, user.gets);
        }

        /// <summary>
        /// 激活图片控件
        /// </summary>
        void ActiveImageCtl()
        {
            InvokeControl(this.tabControl1, new Action(() =>
            {
                if (this.tabControl1.Enabled == false) return;
                this.tabControl1.SelectedIndex = 1;
            }));
        }
        void ActiveVedioCtl()
        {
            InvokeControl(this.tabControl1, new Action(() =>
            {
                if (this.tabControl1.Enabled == false) return;
                this.tabControl1.SelectedIndex = 2;
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
                this.tabControl1.SelectedIndex = 0;
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
