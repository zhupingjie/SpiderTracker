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

            this.RunningConfig = GetSpiderRunningConfig();
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

            SinaSpiderService = new MWeiboSpiderService();
            SinaSpiderService.OnShowStatus += WeiboSpiderService_OnShowStatus;
            SinaSpiderService.OnSpiderStarted += WeiboSpiderService_OnSpiderStarted;
            SinaSpiderService.OnSpiderComplete += WeiboSpiderService_OnSpiderComplete;
            SinaSpiderService.OnSpiderStoping += WeiboSpiderService_OnSpiderStoping;
            SinaSpiderService.OnRefreshConfig += WeiboSpiderService_OnRefreshConfig;
            SinaSpiderService.OnGatherNewUser += SinaSpiderService_OnGatherNewUser;
            SinaSpiderService.OnGatherNewStatus += SinaSpiderService_OnGatherNewStatus;


            Task.Factory.StartNew(() => {
                LoadCacheNameList();
            });
        }


        #region Spider Event

        private void SinaSpiderService_OnGatherNewStatus(SinaStatus newStatus)
        {
            InvokeControl(this.lstUser, new Action(() =>
            {
                if (this.lstUser.Items.Count == 0) return;

                var listItem = this.lstUser.FindItemWithText(newStatus.uid);
                if (listItem != null)
                {
                    var statusQty = 0;
                    int.TryParse(listItem.SubItems[2].Text, out statusQty);
                    statusQty += 1;
                    listItem.SubItems[2].Text = $"{statusQty}";

                    var statusCount = 0;
                    int.TryParse(this.lblArcCount.Text, out statusCount);
                    statusCount += 1;
                    this.lblArcCount.Text = $"{statusCount}";

                    var imageCount = 0;
                    int.TryParse(this.lblPicCount.Text, out imageCount);
                    imageCount += newStatus.pics;
                    this.lblPicCount.Text = $"{imageCount}";
                }

            }));
        }

        private void SinaSpiderService_OnGatherNewUser(SinaUser user)
        {
            if (!CacheSinaUsers.Any(c => c.uid == user.uid))
            {
                CacheSinaUsers.Add(user);

                LoadCacheUserList(LoadCacheName);

                InvokeControl(this.lblUserCount, new Action(() =>
                {
                    var userCount = 0;
                    int.TryParse(this.lblUserCount.Text, out userCount);
                    userCount += 1;
                    this.lblUserCount.Text = $"{userCount}";
                }));
            }
        }


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
            InvokeControl(this.lblUserCount, new Action(() =>
            {
                this.lblUserCount.Text = $"{0}";
            }));
            InvokeControl(this.lblArcCount, new Action(() =>
            {
                this.lblArcCount.Text = $"{0}";
            }));
            InvokeControl(this.lblPicCount, new Action(() =>
            {
                this.lblPicCount.Text = $"{0}";
            }));
        }

        private void WeiboSpiderService_OnSpiderStoping()
        {
            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "Stop...";
                this.btnSearch.Enabled = false;
            }));
        }

        private void WeiboSpiderService_OnSpiderComplete()
        {
            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "开始采集";
                this.btnSearch.Enabled = true;
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
            this.CacheSinaUsers.Clear();

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
            //while (true)
            {
                if (ResetLoadCacheTask)
                {
                    ResetLoadCacheTask = false;
                    ClearCacheUser();
                }

                //Thread.Sleep(2 * 1000);
                
                LoadCacheUserList(LoadCacheName);

                //Thread.Sleep(3 * 1000);
            }
        }

        void LoadCacheNameList()
        {
            //while (true)
            //{
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

                        LoadCacheUserList(LoadCacheName);
                    }

                }));

                //Thread.Sleep(10 * 1000);
            //}
        }

        void LoadCacheUserList(string name)
        {
            var keyword = this.txtUserFilter.Text.Trim();
            var users = SinaSpiderService.Repository.GetUsers(name, keyword);

            if (SortByDate) users = users.OrderByDescending(c => c.lastdate).ToList();
            else users = users.OrderBy(c => c.uid).ToList();

            var needUsers = new List<SinaUser>();
            if (CacheSinaUsers.Count > 0)
            {
                foreach (var user in users)
                {
                    if (!CacheSinaUsers.Any(c=>c.uid == user.uid))
                    {
                        needUsers.Add(user);

                        CacheSinaUsers.Add(user);
                    }
                }
            }
            else
            {
                needUsers.AddRange(users);

                CacheSinaUsers.AddRange(users);
            }
            InvokeControl(this.lstUser, new Action(() =>
            {
                this.lstUser.BeginUpdate();
                foreach(var item in needUsers)
                {
                    if(this.lstUser.Items.Count > 0)
                    {
                        var listItem = this.lstUser.FindItemWithText(item.uid);
                        if (listItem != null) continue;
                    }
                    var subItem = new ListViewItem();
                    subItem.Text = item.uid;
                    subItem.SubItems.Add(item.name);
                    subItem.SubItems.Add($"{item.piccount}");
                    subItem.SubItems.Add($"{(item.newcount>0? "◉" : "")}");
                    this.lstUser.Items.Add(subItem);
                }
                this.lstUser.EndUpdate();
                this.lblLstUserCount.Text = $"{this.lstUser.Items.Count}";
            }));
        }
        
        void LoadCacheUserStatusList(string user)
        {
            var statuses = SinaSpiderService.Repository.GetUserStatuses(user);
            this.CacheSinaStatuss.Clear();
            this.CacheSinaStatuss.AddRange(statuses);
            this.BindUserStatusList(statuses, user);
        }

        void BindUserStatusList(List<SinaStatus> sinaStatus, string user)
        {
            InvokeControl(this.lstArc, new Action(() =>
            {
                this.lstArc.BeginUpdate();
                this.lstArc.Items.Clear();
                var localImg = 0;
                var localStatus = 0;
                var archiveQty = 0;
                foreach (var item in sinaStatus.OrderByDescending(c=>c.lastdate).ToArray())
                {
                    var subItem = new ListViewItem();
                    subItem.Text = $"{item.bid}";
                    subItem.SubItems.Add($"{item.pics}");

                    var local = 0;
                    var path = PathUtil.GetStoreImageUserStatusPathBySelect(LoadCacheName, user, item.bid);
                    if (Directory.Exists(path))
                    {
                        local = Directory.GetFiles(path).Where(c => c.EndsWith(".jpg")).Count();
                        if (local > 0)
                        {
                            localImg += local;
                            localStatus += 1;
                        }
                    }
                    
                    if(item.archive == 1)
                    {
                        archiveQty += 1;
                    }
                    subItem.SubItems.Add($"{local}");
                    subItem.SubItems.Add(item.archive == 1 ? "✔" : "×");
                    this.lstArc.Items.Add(subItem);
                }
                this.lstArc.EndUpdate();
                this.lblLstStatusCount.Text = $"{localStatus}";
                this.lblLstImgCount.Text = $"{localImg}";
                this.lblLstArchiveCount.Text = $"{archiveQty}";
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
                var userIds = new List<string>();
                foreach (ListViewItem item in this.lstUser.SelectedItems)
                {
                    var u = item.SubItems[0].Text.ToString();
                    if (!userIds.Contains(u)) userIds.Add(u);
                }
                var runningConfig = GetSpiderRunningConfig();
                runningConfig.UserIds = userIds.ToArray();
                runningConfig.GatherType = GatherTypeEnum.AutoGather;
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
            if (this.lstUser.SelectedItems == null || this.lstUser.SelectedItems.Count == 0) return;
            if (this.lstUser.SelectedItems.Count > 1) return;

            var userId = GetSelectUserId();
            var userUrl = SinaUrlUtil.GetSinaUserUrl(userId);

            this.txtStartUrl.Text = userUrl;

            LoadCacheUserStatusList(userId);

            if (this.lstArc.Items.Count > 0)
            {
                this.lstArc.Items[0].Selected = true;
            }
        }


        
        private void lstArc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RunningConfig.PreviewImageNow == 1)
            {
                ActiveImageCtl();

                if (this.lstArc.SelectedItems.Count > 1) return;

                var uid = GetSelectUserId();
                var bid = GetSelectStatusId();

                var files = PathUtil.GetStoreImageFiles(LoadCacheName, uid, bid);
                this.imagePreviewUC1.ShowImages(files, 0, RunningConfig.PreviewImageCount);
            }
            else
            {
                ActiveLoggerCtl();
            }
            
        }

        string GetSelectUserId()
        {
            if (this.lstUser.SelectedItems == null || this.lstUser.SelectedItems.Count == 0) return null;

            return this.lstUser.SelectedItems[0].SubItems[0].Text;
        }

       string GetSelectStatusId()
        {
            if (this.lstArc.SelectedItems == null || this.lstArc.SelectedItems.Count == 0) return string.Empty;

            return this.lstArc.SelectedItems[0].SubItems[0].Text;
        }
        string[] GetSelectStatusIds()
        {
            if (this.lstArc.SelectedItems == null || this.lstArc.SelectedItems.Count == 0) return new string[] { };

            var ids = new List<string>();
            foreach(ListViewItem item in this.lstArc.SelectedItems)
            {
                ids.Add(item.SubItems[0].Text);
            }
            return ids.ToArray();
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
            if (MessageBox.Show($"确认已存档当前用户的所有图集[{user}]?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            if (!string.IsNullOrEmpty(user))
            {
                var rep = new SinaRepository();
                var status = rep.GetUserStatuseOfNoArchives(user);

                rep.ArchiveSinaUser(user);

                if (this.lstUser.Items.Count == 0) return;

                var listItem = this.lstUser.FindItemWithText(user);
                if (listItem != null)
                {
                    listItem.SubItems[3].Text = "◉";
                }
                if (status.Count > 0)
                {
                    var bids = status.Select(c => c.bid).ToArray();
                    ArchiveStatus(user, bids);
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
            var sinaUser = rep.GetUser(user);
            if (sinaUser == null)
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
                if (sinaUser.ignore == 1)
                {
                    sinaUser.ignore = 0;
                    rep.UpdateSinaUser(sinaUser, new string[] { "ignore" });

                    this.lstUser.Items.Insert(0, user);
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

        private void btnIgnoreUser_Click(object sender, EventArgs e)
        {
            var userId = GetSelectUserId();
            if (string.IsNullOrEmpty(userId)) return;

            if (MessageBox.Show($"确认拉黑当前用户[{userId}]?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var suc = rep.IgnoreSinaUser(userId);
            if (suc)
            {
                var userPath = PathUtil.GetStoreImageUserPath(LoadCacheName, userId);
                if (Directory.Exists(userPath)) Directory.Delete(userPath, true);

                if (this.lstUser.Items.Count == 0) return;

                var listItem = this.lstUser.FindItemWithText(userId);
                if (listItem != null)
                {
                    var index = listItem.Index;
                    this.lstUser.Items.Remove(listItem);
                    if (this.lstUser.Items.Count <= index)
                    {
                        this.lstUser.Items[this.lstUser.Items.Count - 1].Selected = true;
                    }
                    else
                    {
                        this.lstUser.Items[index].Selected = true;
                    }
                    this.lblLstUserCountTitle.Text = $"用户：{this.lstUser.Items.Count}";
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
                if (Directory.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    path = PathUtil.GetStoreImageUserPath(LoadCacheName, uid);
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
        }

        private void btnGetStatusByBid_Click(object sender, EventArgs e)
        {
            var bids = GetSelectStatusIds();
            foreach(var bid in bids)
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
            var uid = GetSelectUserId();
            var bids = GetSelectStatusIds();
            if (bids.Length == 0) return;

            if (MessageBox.Show($"确认拉黑当前选中的[{bids.Length}]个图集?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            foreach (var bid in bids)
            {
                rep.IgnoreSinaStatus(bid);

                var userStatusPath = PathUtil.GetStoreImageUserStatusPath(LoadCacheName, uid, bid);
                if (Directory.Exists(userStatusPath)) Directory.Delete(userStatusPath, true);


                if (this.lstArc.Items.Count > 0)
                {
                    var listItem = this.lstArc.FindItemWithText(bid);
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
            var bids = GetSelectStatusIds();
            if (bids.Length == 0) return;

            if (MessageBox.Show($"确认已存档当前选中的[{bids.Length}]个图集?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            foreach (var bid in bids)
            {
                rep.ArchiveSinaStatus(bid);

                if (this.lstArc.Items.Count > 0)
                {
                    var listItem = this.lstArc.FindItemWithText(bid);
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
            ArchiveStatus(uid, bids);
        }

        void ArchiveStatus(string uid, string[] bids)
        {
            var archivePath = Path.Combine(PathUtil.BaseDirectory, RunningConfig.DefaultArchivePath);
            if (!Directory.Exists(archivePath)) Directory.CreateDirectory(archivePath);

            foreach(var bid in bids)
            {
                var path = PathUtil.GetStoreImageUserStatusPath(LoadCacheName, uid, bid);
                if (!Directory.Exists(path)) continue;

                var files = Directory.GetFiles(path).Where(c => c.EndsWith(".jpg")).Select(c => new FileInfo(c)).ToArray();
                foreach(var file in files)
                {
                    var destFile = Path.Combine(archivePath, file.Name);
                    file.CopyTo(destFile, true);
                }
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

            if (!string.IsNullOrEmpty(LoadCacheName))
            {
                ResetLoadCacheTask = true;
            }

            Task.Factory.StartNew(() => {
                LoadCacheUserTask();
            });
        }


        private void txtStatusFilter_TextChanged(object sender, EventArgs e)
        {
            var user = GetSelectUserId();
            if (string.IsNullOrEmpty(user)) return;

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

        private void btnUpdateInfo_Click(object sender, EventArgs e)
        {
            this.UpdateCacheUserInfo(LoadCacheName);
        }
        private void btnManage_Click(object sender, EventArgs e)
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
                this.btnIgnoreStatus_Click(sender, e);
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
            dr["配置项"] = "忽略存档图集";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "忽略无图图集";
            dr["配置值"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "采集原创图集";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["配置项"] = "采集所有用户";
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
            dr["配置值"] = "9999";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "预览图片数量";
            dr["配置值"] = "6";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "预览显示图片";
            dr["配置值"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["配置项"] = "默认归档路径";
            dr["配置值"] = "archive";
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
                else if (row.Cells["配置项"].Value.ToString() == "采集原创图集")
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
                else if (row.Cells["配置项"].Value.ToString() == "采集用户名称")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    runningConfig.ReadUserNameLike = strValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "采集所有用户")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadAllOfUser = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "采集用户关注")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    var intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.ReadUserOfFocus = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "忽略存档图集")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue); ;
                    runningConfig.IgnoreReadArchiveStatus = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "忽略无图图集")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue); ;
                    runningConfig.IgnoreDeleteImageStatus = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "预览图片数量")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue); 
                    runningConfig.PreviewImageCount = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "预览显示图片")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.PreviewImageNow = intValue;
                }
                else if (row.Cells["配置项"].Value.ToString() == "并发用户数量")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    runningConfig.MaxReadUserThteadCount = intValue;
                }
                else if(row.Cells["配置项"].Value.ToString() == "默认归档路径")
                {
                    var strValue = row.Cells["配置值"].Value.ToString();
                    runningConfig.DefaultArchivePath = strValue;
                }
            }
            return runningConfig;
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
                    subItem.SubItems.Add($"{item.piccount}");
                    this.lstUser.Items.Add(subItem);
                }
                this.lstUser.EndUpdate();
            }));
            InvokeControl(this.lstArc, new Action(() =>
            {
                this.lstArc.Items.Clear();
            }));
        }

        void FilterStatusIds(string keyword, string user)
        {
            var searchStatuss = CacheSinaStatuss.Where(c => c.bid.ToUpper().Contains(keyword.ToUpper())).ToList();
            this.BindUserStatusList(searchStatuss, user);
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
