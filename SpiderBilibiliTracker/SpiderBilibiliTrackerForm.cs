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
using System.Reflection;
using SpiderCore.Config;
using SpiderCore.Service;
using SpiderCore.Repository;
using SpiderCore.Entity;
using SpiderCore.Util;

namespace SpiderTracker
{
    public partial class SpiderBilibiliTrackerForm : Form
    {
        BilibiliSpiderService BilibiliSpiderService { get; set; }
        SpiderRunningConfig RunningConfig { get; set; } = new SpiderRunningConfig();
        public SpiderBilibiliTrackerForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //开启双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// 界面加载开启读取本地缓存用户文章列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpiderTrackerForm_Load(object sender, EventArgs e)
        {
            SQLiteDBHelper.Instance.InitSpiderDB();

            InitSpiderRunningConfig();

            this.RunningConfig = GetSpiderRunningConfig();

            BilibiliSpiderService = new BilibiliSpiderService();
            BilibiliSpiderService.OnShowGatherStatus += WeiboSpiderService_OnShowStatus;
            BilibiliSpiderService.OnSpiderStarted += WeiboSpiderService_OnSpiderStarted;
            BilibiliSpiderService.OnSpiderComplete += WeiboSpiderService_OnSpiderComplete;
            BilibiliSpiderService.OnSpiderStoping += WeiboSpiderService_OnSpiderStoping;
            BilibiliSpiderService.OnGatherNewUser += SinaSpiderService_OnGatherNewUser;
            BilibiliSpiderService.OnGatherStatusComplete += SinaSpiderService_OnGatherStatusComplete;
            BilibiliSpiderService.OnGatherPageComplete += SinaSpiderService_OnGatherPageComplete;
            BilibiliSpiderService.OnGatherUserComplete += SinaSpiderService_OnGatherUserComplete;
            BilibiliSpiderService.OnGatherAppendUser += SinaSpiderService_OnGatherAppendUser;
            BilibiliSpiderService.OnGatherUserStarted += SinaSpiderService_OnGatherUserStarted;
            BilibiliSpiderService.OnNewActionCount += SinaSpiderService_OnNewActionCount;

            BilibiliSpiderService.StartBackgroundTask();

            spiderConfigUC1.OnRefreshConfig += SpiderConfigUC1_OnRefreshConfig;

            Task.Factory.StartNew(() =>
            {
                LoadCacheNameList();
            });
        }


        #region Spider Event

        private void SinaSpiderService_OnNewActionCount(int needUploads)
        {
            InvokeControl(btnBackTask, new Action(() =>
            {
                var text = this.btnBackTask.Text.Substring(0, 6);
                if (needUploads > 0) text = $"{text}({needUploads})";

                this.btnBackTask.Text = text;
            }));
        }

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
                if (this.lstRunstate.Items.Count > 0)
                {
                    var listItem = this.lstRunstate.FindItemWithText(user.uid);
                    if (listItem != null) return;
                }

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
            InvokeControl(this.lstRunstate, new Action(() =>
            {
                if (this.lstRunstate.Items.Count > 0)
                {
                    var listItem = this.lstRunstate.FindItemWithText(user.uid);
                    if (listItem != null) return;
                }

                var subItem = new ListViewItem();
                subItem.Text = user.uid;
                subItem.SubItems.Add($"{user.name}");
                subItem.SubItems.Add($"0");
                subItem.SubItems.Add($"0");
                subItem.SubItems.Add($"OK");
                this.lstRunstate.Items.Add(subItem);
            }));
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

        private void WeiboSpiderService_OnSpiderStarted(SpiderRunningTask runningTask)
        {
            ActiveLoggerCtl();

            InvokeControl(this.cbxCategory, new Action(() =>
            {
                this.cbxCategory.Enabled = false;
            }));

            InvokeControl(this.cbxSite, new Action(() =>
            {
                this.cbxSite.Enabled = false;
            }));

            InvokeControl(this.btnSearch, new Action(() =>
            {
                this.btnSearch.Text = "Stop";
                this.btnSearch.Enabled = true;
            }));

            InvokeControl(this.btnAppendUser, new Action(() =>
            {
                if(runningTask.GatherType == GatherTypeEnum.GatherUser)
                {
                    this.btnAppendUser.Text = "追加采集";
                    this.btnAppendUser.Enabled = true;
                }
                else
                {
                    this.btnAppendUser.Enabled = false;
                }
            }));

            InvokeControl(this.lstRunstate, new Action(() =>
            {
                this.lstRunstate.Items.Clear();
                foreach (var user in runningTask.DoUsers)
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
            InvokeControl(this.cbxCategory, new Action(() =>
            {
                this.cbxCategory.Enabled = true;
            }));

            InvokeControl(this.cbxSite, new Action(() =>
            {
                this.cbxSite.Enabled = true;
            }));

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

        private void WeiboSpiderService_OnShowStatus(string msg, bool bLog = false, Exception ex = null)
        {
            InvokeControl(this.statusStrip1, new Action(() =>
            {
                this.tplStatus.Text = msg;
            }));
            if (ex != null)
            {
                LogUtil.Error(ex);
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
            var names = BilibiliSpiderService.Repository.GetGroupNames(GatherWebEnum.Bilibili);

            InvokeControl(this.cbxCategory, new Action(() =>
            {
                bool bNew = false;
                foreach (var name in names)
                {
                    if (!this.cbxCategory.Items.Contains(name))
                    {
                        bNew = true;
                        break;
                    }
                }
                if (bNew)
                {
                    var selectName = this.cbxCategory.Text;
                    this.cbxCategory.BeginUpdate();
                    this.cbxCategory.Items.Clear();
                    this.cbxCategory.Items.AddRange(names);
                    if (!selectName.Equals("cosplay"))
                    {
                        this.cbxCategory.Text = selectName;
                    }
                    this.cbxCategory.EndUpdate();
                }
                if (names.Length > 0)
                {
                    RunningConfig.Category = names.FirstOrDefault();

                    Task.Factory.StartNew(() =>
                    {
                        LoadCacheUserList(true);
                    });
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
                case "用户":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.uid).ToArray();
                    else
                        users = users.OrderBy(c => c.uid).ToArray();
                    break;
                case "名称":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.name).ToArray();
                    else
                        users = users.OrderBy(c => c.name).ToArray();
                    break;
                case "微博":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.statuses).ToArray();
                    else
                        users = users.OrderBy(c => c.statuses).ToArray();
                    break;
                case "读取":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.finds).ToArray();
                    else
                        users = users.OrderBy(c => c.finds).ToArray();
                    break;
                case "原创":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.originals).ToArray();
                    else
                        users = users.OrderBy(c => c.originals).ToArray();
                    break;
                case "转发":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.retweets).ToArray();
                    else
                        users = users.OrderBy(c => c.retweets).ToArray();
                    break;
                case "采集":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.gets).ToArray();
                    else
                        users = users.OrderBy(c => c.gets).ToArray();
                    break;
                case "上传":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.uploads).ToArray();
                    else
                        users = users.OrderBy(c => c.uploads).ToArray();
                    break;
                case "忽略":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.ignores).ToArray();
                    else
                        users = users.OrderBy(c => c.ignores).ToArray();
                    break;
                case "关注":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.follows).ToArray();
                    else
                        users = users.OrderBy(c => c.follows).ToArray();
                    break;
                case "点赞":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.focus).ToArray();
                    else
                        users = users.OrderBy(c => c.focus).ToArray();
                    break;
                case "页码":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.readpage).ToArray();
                    else
                        users = users.OrderBy(c => c.readpage).ToArray();
                    break;
                case "末页":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.lastpage).ToArray();
                    else
                        users = users.OrderBy(c => c.lastpage).ToArray();
                    break;
                case "来源":
                    if (this.cbxUserSortAsc.Text == "降序")
                        users = users.OrderByDescending(c => c.site).ToArray();
                    else
                        users = users.OrderBy(c => c.site).ToArray();
                    break;
            }
            var pageIndex = 0;
            int.TryParse(this.cbxUserSortIndex.Text, out pageIndex);
            var pageCount = 0;
            int.TryParse(this.cbxUserSortPage.Text, out pageCount);
            if (pageCount == 0)
                return users.ToArray();
            else
                return users.Skip((pageIndex - 1) * pageCount).Take(pageCount).ToArray();
        }

        void LoadCacheUserList(bool reload)
        {
            var keyword = this.txtUserFilter.Text.Trim();
            var users = BilibiliSpiderService.Repository.GetUsers(RunningConfig.Category, keyword, GatherWebEnum.Bilibili);

            InvokeControl(this.lstUser, new Action(() =>
            {
                var user = GetSelectUser();

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
                    subItem.SubItems.Add($"{item.uploads}");
                    subItem.SubItems.Add($"{item.ignores}");
                    subItem.SubItems.Add($"{item.originals}");
                    subItem.SubItems.Add($"{item.retweets}");
                    subItem.SubItems.Add($"{item.follows}");
                    subItem.SubItems.Add($"{item.readpage}");
                    subItem.SubItems.Add($"{(item.focus > 0 ? "◉" : "")}");
                    subItem.SubItems.Add($"{(item.lastpage > 0 ? "✔" : "")}");
                    subItem.SubItems.Add($"{item.site}");
                    subItem.SubItems.Add($"{(item.lastdate.ToString("yyyy/MM/dd HH:mm"))}");
                    subItem.Tag = item;
                    this.lstUser.Items.Add(subItem);
                }
                this.lstUser.EndUpdate();
                this.lblUserUid.Text = $"用户:{users.Count}";

                this.LoadUserPageIndex(users.Count);

                if (user != null)
                {
                    var listItem = this.lstUser.FindItemWithText(user.uid);
                    if (listItem != null)
                    {
                        this.lstUser.Items[listItem.Index].Selected = true;
                    }
                }
            }));
        }

        void LoadUserPageIndex(int userCount)
        {
            var pageCount = 0;
            int.TryParse(this.cbxUserSortPage.Text, out pageCount);

            if (pageCount == 0) return;
            var pageNum = (int)Math.Ceiling(1.0 * userCount / pageCount);
            this.cbxUserSortIndex.Items.Clear();
            for (var i = 1; i <= pageNum; i++)
            {
                this.cbxUserSortIndex.Items.Add(i);
            }
        }

        SinaStatus[] GetShowStatus(IList<SinaStatus> status)
        {
            switch (this.cbxStatusSort.Text)
            {
                case "更新":
                    if (this.cbxStatusSortAsc.Text == "降序")
                        status = status.OrderByDescending(c => c.lastdate).ToArray();
                    else
                        status = status.OrderBy(c => c.lastdate).ToArray();
                    break;
                case "数量":
                    if (this.cbxStatusSortAsc.Text == "降序")
                        status = status.OrderByDescending(c => c.qty).ToArray();
                    else
                        status = status.OrderBy(c => c.qty).ToArray();
                    break;
                case "采集":
                    if (this.cbxStatusSortAsc.Text == "降序")
                        status = status.OrderByDescending(c => c.gets).ToArray();
                    else
                        status = status.OrderBy(c => c.gets).ToArray();
                    break;
                case "上传":
                    if (this.cbxStatusSortAsc.Text == "降序")
                        status = status.OrderByDescending(c => c.upload).ToArray();
                    else
                        status = status.OrderBy(c => c.upload).ToArray();
                    break;
                case "日期":
                    if (this.cbxStatusSortAsc.Text == "降序")
                        status = status.OrderByDescending(c => c.createtime).ToArray();
                    else
                        status = status.OrderBy(c => c.createtime).ToArray();
                    break;
                case "来源":
                    if (this.cbxStatusSortAsc.Text == "降序")
                        status = status.OrderByDescending(c => c.site).ToArray();
                    else
                        status = status.OrderBy(c => c.site).ToArray();
                    break;
            }
            var pageIndex = 0;
            int.TryParse(this.cbxStatusSortIndex.Text, out pageIndex);
            var pageCount = 0;
            int.TryParse(this.cbxStatusSortPage.Text, out pageCount);
            if (pageCount == 0) 
                return status.ToArray();
            else 
                return status.Skip((pageIndex - 1) * pageCount).Take(pageCount).ToArray();
        }
        void LoadCacheUserStatusList(SinaUser user, bool reload)
        {
            var keyword = this.txtStatusFilter.Text.Trim();
            var sinaStatus = BilibiliSpiderService.Repository.GetUserStatuseByIds(user.uid, keyword);

            InvokeControl(this.lstArc, new Action(() =>
            {
                var status = GetSelectStatus();

                var showStatus = GetShowStatus(sinaStatus);

                var imageFiles = PathUtil.GetStoreUserThumbnailImageFiles(RunningConfig.Category, user.uid);
                var videoFiles = PathUtil.GetStoreUserVideoFiles(RunningConfig.Category, user.uid);

                this.lstArc.BeginUpdate();
                if(reload) this.lstArc.Items.Clear();
                var localImg = 0;
                foreach (var item in showStatus)
                {
                    if (this.lstArc.Items.Count > 0 && !reload)
                    {
                        var listItem = this.lstArc.FindItemWithText(item.bid);
                        if (listItem != null) continue;
                    }

                    var subItem = new ListViewItem();
                    subItem.Tag = item;
                    subItem.Text = $"{item.bid}";
                    var local = "";
                    if (item.mtype == 0)
                    {
                        subItem.SubItems.Add($"{item.qty}");
                        subItem.SubItems.Add($"{item.gets}");
                        //var files = PathUtil.GetStoreUserThumbnailImageFiles(RunningConfig.Category, user.uid, item.bid);
                        var files = imageFiles.Where(c => c.Contains($"{item.bid}")).ToArray();
                        local = $"{files.Length}";
                        localImg += files.Length;
                    }
                    else if (item.mtype == 1)
                    {
                        subItem.SubItems.Add($"{item.qty}✦");
                        subItem.SubItems.Add($"{item.gets}");
                        //local = PathUtil.GetStoreUserVideoCount(RunningConfig.Category, user.uid, item.bid);
                        //localImg += 1;
                        var files = videoFiles.Where(c => c.Contains($"{item.bid}")).ToArray();
                        local = $"{files.Length}";
                        localImg += files.Length;
                    }
                    subItem.SubItems.Add($"{local}");
                    subItem.SubItems.Add($"{item.upload}");
                    subItem.SubItems.Add($"{item.site}");
                    subItem.SubItems.Add($"{item.createtime}");
                    this.lstArc.Items.Add(subItem);                    
                }
                this.lstArc.EndUpdate();
                this.lblStatusBid.Text = $"微博:{sinaStatus.Count}";

                this.LoadStatusPageIndex(sinaStatus.Count);

                if (status != null)
                {
                    var listItem = this.lstArc.FindItemWithText(status.bid);
                    if (listItem != null)
                    {
                        this.lstArc.Items[listItem.Index].Selected = true;
                    }
                }
            }));
        }

        void LoadStatusPageIndex(int userCount)
        {
            var pageCount = 0;
            int.TryParse(this.cbxStatusSortPage.Text, out pageCount);

            if (pageCount == 0) return;
            var pageNum = (int)Math.Ceiling(1.0 * userCount / pageCount);
            this.cbxStatusSortIndex.Items.Clear();
            for (var i = 1; i <= pageNum; i++)
            {
                this.cbxStatusSortIndex.Items.Add(i);
            }
        }

        void LoadCacheTopicList()
        {
            var rep = new SinaRepository();
            var topics = rep.GetSinaTopics(RunningConfig.Category);

            InvokeControl(this.cbxSelect, new Action(() =>
            {
                this.cbxSelect.BeginUpdate();
                this.cbxSelect.Items.Clear();
                this.cbxSelect.DisplayMember = "name";
                this.cbxSelect.ValueMember = "name";
                foreach (var item in topics)
                {
                    this.cbxSelect.Items.Add(item);
                }
                this.cbxSelect.EndUpdate();
            }));
        }

        void LoadCacheSuperList()
        {
            var rep = new SinaRepository();
            var topics = rep.GetSinaSupers(RunningConfig.Category);

            InvokeControl(this.cbxSelect, new Action(() =>
            {
                this.cbxSelect.BeginUpdate();
                this.cbxSelect.Items.Clear();
                this.cbxSelect.DisplayMember = "name";
                this.cbxSelect.ValueMember = "containerid";
                foreach (var item in topics)
                {
                    this.cbxSelect.Items.Add(item);
                }
                this.cbxSelect.EndUpdate();
            }));
        }

        #endregion

        #region 用户列表及用户微博事件

        private void cbxSelect_DropDown(object sender, EventArgs e)
        {
            switch (this.cbxSite.Text)
            {
                case "topic":
                    this.cbxSelect.Enabled = true;
                    this.LoadCacheTopicList();
                    break;
                case "super":
                    this.cbxSelect.Enabled = true;
                    this.LoadCacheSuperList();
                    break;
                default:
                    this.cbxSelect.Text = "......";
                    this.cbxSelect.Enabled = false;
                    break;
            }
        }

        private void lblStatusBid_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.lblStatusBid.Text, true, 3, 100);
        }

        private void lblUserUid_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.lblUserUid.Text, true, 3, 100);
        }

        private void txtStartUrl_DoubleClick(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text) || iData.GetDataPresent(DataFormats.OemText))
            {
                this.txtStartUrl.Text = (String)iData.GetData(DataFormats.Text);
            }
        }
        private void SpiderConfigUC1_OnRefreshConfig(SpiderRunningConfig spiderRunninConfig)
        {
            this.RunningConfig = this.spiderConfigUC1.GetRunningConfig();
        }

        private void btnRefreshConfig_Click(object sender, EventArgs e)
        {
            this.InitSpiderRunningConfig();
            this.RunningConfig = GetSpiderRunningConfig();
        }

        private void btnReadConfig_Click(object sender, EventArgs e)
        {
            this.InitSpiderRunningConfig(true);
            this.RunningConfig = GetSpiderRunningConfig();
        }

        private void cbxUserSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                LoadCacheUserList(true);
            });
        }

        private void cbxUserSortAsc_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                LoadCacheUserList(true);
            });
        }

        private void cbxUserSortPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                LoadCacheUserList(true);
            });
        }


        private void cbxUserSortIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                LoadCacheUserList(true);
            });
        }

        private void btnFocusUser_Click(object sender, EventArgs e)
        {
            this.FocusUser();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            this.ChageUser();
        }

        private void btnIgnoreUser_Click(object sender, EventArgs e)
        {
            this.IgnoreUser(true);
        }

        private void lstUser_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Escape)
            {
                this.IgnoreUser(true);
            }
        }

        private void lstUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItems == null || this.lstUser.SelectedItems.Count == 0) return;
            if (this.lstUser.SelectedItems.Count > 1) return;

            var user = GetSelectUser();
            if (user == null) return;

            //var userUrl = SinaUrlUtil.GetSinaUserUrl(user.uid);

            this.txtStartUrl.Text = "";
            this.lblUserUid.Text = user.uid;
            //this.cbxStatusSortIndex.Text = $"1";

            Task.Factory.StartNew(() =>
            {
                LoadCacheUserStatusList(user, true);
            });
        }

        private void lstArc_SelectedIndexChanged(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user == null) return;
            var status = GetSelectStatus();
            if (status == null) return;
            if (this.lstArc.SelectedItems.Count > 1) return;

            this.lblStatusBid.Text = status.bid;

            if (RunningConfig.PreviewImageNow == 1)
            {   
                if (status.mtype == 0)
                {
                    ActiveImageCtl();
                    var files = PathUtil.GetStoreUserThumbnailImageFiles(RunningConfig.Category, user.uid, status.bid);
                    var rep = new SinaRepository();
                    var sources = rep.GetUserSources(user.uid, status.bid);

                    this.imagePreviewUC1.ShowImages(files, RunningConfig, status, sources);
                }
                else if (status.mtype == 1)
                {
                    ActiveVedioCtl();

                    var file = PathUtil.GetStoreUserVideoFile(RunningConfig.Category, user.uid, status.bid);
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

        private void cbxName_Leave(object sender, EventArgs e)
        {
            this.RunningConfig = GetSpiderRunningConfig();

            //this.cbxUserSortIndex.Text = $"1";

            this.ChangeSelect();

            Task.Factory.StartNew(() =>
            {
                LoadCacheUserList(true);
            });
        }

        private void cbxSite_Leave(object sender, EventArgs e)
        {
            this.RunningConfig = GetSpiderRunningConfig();
            this.ChangeSelect();
        }
        private void cbxSelect_Leave(object sender, EventArgs e)
        {
            var selectItem = this.cbxSelect.SelectedItem;
            if(selectItem != null)
            {
                var topic = selectItem as SinaTopic;
                var startUrl = topic.containerid;
                if (this.cbxSite.Text == "topic") startUrl = topic.name;
                this.txtStartUrl.Text = startUrl;
            }
        }

        private void chkBweTopic_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkBweTopic.Checked)
            {
                var selectItem = this.cbxSelect.SelectedItem;
                if (selectItem == null) return;

                var topic = selectItem as SinaTopic;
                System.Diagnostics.Process.Start(topic.profile);

                this.chkBweTopic.Checked = false;
            }
        }

        private void chkUploadWeb_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUploadWeb.Checked)
            {
                var status = GetSelectStatusId();

                var frm = new UploadSourceWebForm(RunningConfig, status, "", true);
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();

                this.chkUploadWeb.Checked = false;
            }
        }

        private void btnBrowseUser_Click(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user == null) return;

            var url = SinaUrlUtil.GetBilibiliUserUrl(user.uid);
            System.Diagnostics.Process.Start(url);
        }

        private void btnBrowseStatus_Click(object sender, EventArgs e)
        {
            var bid = GetSelectStatusId();
            var url = SinaUrlUtil.GetBilibiliUserStatusUrl(bid);
            System.Diagnostics.Process.Start(url);
        }

        private void btnOpenStatus_Click(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user == null) return;

            if (!string.IsNullOrEmpty(user.uid))
            {
                var path = PathUtil.GetStoreUserPath(RunningConfig.Category, user.uid);
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
            var user = GetSelectUser();
            if (user == null) return;

            var status = GetSelectStatuss();
            foreach (var item in status)
            {
                statusIds.Add(item.bid);
            }
            if (!BilibiliSpiderService.IsSpiderStarted)
            {
                var option = new SpiderStartOption()
                {
                    GatherName = "status",
                    StatusIds = statusIds.ToArray(),
                    SelectUserId = user.uid
                };
                BilibiliSpiderService.StartSpider(RunningConfig, option);
            }
        }

        private void btnIgnoreStatus_Click(object sender, EventArgs e)
        {
            this.IgnoreStatus(true);
        }

        private void btnFollowerUser_Click(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user == null) return;

            var url = SinaUrlUtil.GetSinaUserFollowerUrl(user.uid);
            System.Diagnostics.Process.Start(url);
        }

        private void btnArchiveStatus_Click(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user == null) return;

            var status = GetSelectStatuss();
            if (status.Length == 0) return;

            if (MessageBox.Show($"确认已上传当前选中的[{status.Length}]个微博?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            foreach (var item in status)
            {
                if (item.mtype == 0)
                {
                    var files = PathUtil.GetStoreUserImageFiles(RunningConfig.Category, item.uid, item.bid);
                    if (files.Length > 0)
                    {
                        rep.MakeUploadAction(RunningConfig.Category, item.bid, files, false);
                    }
                }
                else if(item.mtype == 1)
                {
                    //var file = PathUtil.GetStoreUserVideoFile(RunningConfig.Category, item.uid, item.bid);
                    //if (File.Exists(file))
                    //{
                    //    if (rep.UploadSinaStatus(RunningConfig.Category, item.bid, new string[] { file }))
                    //    {
                    //        CopyUploadImageFiles(new string[] { file });
                    //    }
                    //}
                }
            }
        }

        private void btnFoucsUser_Click(object sender, EventArgs e)
        {
            var user = GetSelectUser();
           if(user == null) return;

            if (MessageBox.Show("确认关注当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            rep.FocusSinaUser(user.uid);
        }

        private void txtUserFilter_Leave(object sender, EventArgs e)
        { 
            var user = this.txtUserFilter.Text.Trim();
            if (user.Length >= 6)
            {
                var rep = new SinaRepository();
                var sinaUser = rep.GetUser(user);
                if (sinaUser == null)
                {
                    sinaUser = new SinaUser()
                    {
                        uid = user,
                        category = RunningConfig.Category,
                        web = (int)GatherWebEnum.Bilibili
                    };
                    rep.CreateSinaUser(sinaUser);
                }
                else
                {
                    if (sinaUser.ignore > 0)
                    {
                        sinaUser.ignore = 0;
                        rep.UpdateSinaUser(sinaUser, new string[] { "ignore" });
                    }
                }
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserList(true);
                });
            }
            else if(user.Length > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserList(true);
                });
            }
        }

        private void txtUserFilter_DoubleClick(object sender, EventArgs e)
        {
            this.txtUserFilter.Clear();

            Task.Factory.StartNew(() =>
            {
                LoadCacheUserList(true);
            });
        }

        private void txtStatusFilter_Leave(object sender, EventArgs e)
        {
            var status = this.txtStatusFilter.Text.Trim();
            var user = GetSelectUser();
            if (user != null && status.Length > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserStatusList(user, true);
                });
            }
        }

        private void cbxStatusSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user != null)
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserStatusList(user, true);
                });
            }
        }

        private void cbxStatusSortAsc_SelectedIndexChanged(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user != null)
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserStatusList(user, true);
                });
            }
        }

        private void cbxStatusSortPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user != null)
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserStatusList(user, true);
                });
            }
        }

        private void cbxStatusSortIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            var user = GetSelectUser();
            if (user != null)
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserStatusList(user, true);
                });
            }
        }

        private void txtStatusFilter_DoubleClick(object sender, EventArgs e)
        {
            this.txtStatusFilter.Clear();

            var user = GetSelectUser();
            if (user != null)
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCacheUserStatusList(user, true);
                });
            }
        }

        private void chkShowSource_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkShowSource.Checked)
            {
                RunningConfig.PreviewImageNow = 1;
                LockImageCtl(true);
            }
            else
            {
                RunningConfig.PreviewImageNow = 0;
                LockImageCtl(false);
            }
        }

        private void chkShowWinBkg_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkShowWinBkg.Checked)
            {
                BilibiliSpiderService.StartShowWinBackgroundTask();
            }
            else
            {
                BilibiliSpiderService.StopShowWinBackgroundTask();
            }
        }

        private void btnBackTask_CheckedChanged(object sender, EventArgs e)
        {
            if (btnBackTask.Checked)
            {
                var frm = new UploadRunStateForm(BilibiliSpiderService);
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();

                this.btnBackTask.Checked = false;
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
            if (!BilibiliSpiderService.IsSpiderStarted)
            {
                var users = new List<SinaUser>();
                foreach (ListViewItem item in this.lstUser.SelectedItems)
                {
                    var user = item.Tag;
                    if (user != null)
                    {
                        users.Add(user as SinaUser);
                    }
                }
                var option = new SpiderStartOption()
                {
                    GatherName = RunningConfig.Site,
                    SelectUsers = users.ToArray(),
                    SelectUserId = GetSelectUserId(),
                    StartUrl = this.txtStartUrl.Text
                };
                BilibiliSpiderService.StartSpider(RunningConfig, option);
            }
            else
            {
                BilibiliSpiderService.StopSpider();
            }
        }
        private void btnAppendUser_Click(object sender, EventArgs e)
        {
            if (BilibiliSpiderService.IsSpiderStarted)
            {
                if (this.lstUser.SelectedItems.Count == 0) return;

                foreach (ListViewItem item in this.lstUser.SelectedItems)
                {
                    var user = item.Tag;
                    if (user != null)
                    {
                        BilibiliSpiderService.AppendUser(user as SinaUser);
                    }
                }
            }
        }

        private void btnCancelUser_Click(object sender, EventArgs e)
        {
            if(BilibiliSpiderService.IsSpiderStarted)
            {
                if (this.lstRunstate.SelectedItems.Count == 0) return;

                foreach (ListViewItem item in this.lstRunstate.SelectedItems)
                {
                    var uid = item.SubItems[0].Text;
                    if (!string.IsNullOrEmpty(uid))
                    {
                        BilibiliSpiderService.CancelUser(uid);
                    }
                    item.SubItems[4].Text = "Cancel...";
                }
            }
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            var status = GetSelectStatus();
            if (status == null) return;

            var url = HttpUtil.GetSinaSoureImageUrl(RunningConfig.DefaultUploadServerIP, RunningConfig.Category, status.bid, false);
            System.Diagnostics.Process.Start(url);
        }

        void ChangeSelect()
        {
            this.cbxSelect.Text = "...";
            switch (this.cbxSite.Text)
            {
                case "topic":
                    this.cbxSelect.Enabled = true;
                    break;
                case "super":
                    this.cbxSelect.Enabled = true;
                    break;
                default:
                    this.cbxSelect.Text = "......";
                    this.cbxSelect.Enabled = false;
                    break;
            }
            this.txtStartUrl.Text = "";
        }

        #endregion

        #region 用户及微博功能操作

        void IgnoreUser(bool confirm)
        {
            var user = GetSelectUser();
            if (user == null) return;

            if (confirm && MessageBox.Show($"确认拉黑当前用户[{user.uid}]?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            rep.MakeIgnoreUserAction(RunningConfig.Category, user.uid);
            if (this.lstUser.Items.Count == 0) return;

            this.vedioPlayerUC1.CloseVideo();

            var listItem = this.lstUser.FindItemWithText(user.uid);
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
            }
        }

        void ChageUser()
        {
            var user = GetSelectUser();
            if (user == null) return;

            var frm = new ChangeUserCategoryForm();
            frm.StartPosition = FormStartPosition.CenterParent;
            if(frm.ShowDialog() == DialogResult.OK)
            {
                var rep = new SinaRepository();
                var suc = rep.ChangeUserCategory(user.uid, frm.ChangeCategory);
                if (suc)
                {
                    Task.Factory.StartNew(() =>
                    {
                        PathUtil.MoveStoreUserSource(RunningConfig.Category, frm.ChangeCategory, user.uid);
                    });

                    var lstItem = this.lstUser.FindItemWithText(user.uid);
                    if (lstItem != null)
                    {
                        var index = lstItem.Index;
                        this.lstUser.Items.Remove(lstItem);
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
                    }
                }
            }
        }

        void FocusUser()
        {
            var user = GetSelectUser();
            if (user == null) return;

            if (MessageBox.Show($"确认关注当前用户[{user.uid}]?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            var focus = rep.FocusSinaUser(user.uid);
        }

        void IgnoreStatus(bool confirm)
        {
            var user = GetSelectUser();
            if (user == null) return;
            var status = GetSelectStatuss();

            if (confirm && MessageBox.Show($"确认拉黑当前选中的[{status.Length}]个微博?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var rep = new SinaRepository();
            foreach (var item in status)
            {
                rep.MakeIgnoreStatusAction(RunningConfig.Category, item.bid);
                if(item.mtype == 2)
                {
                    this.vedioPlayerUC1.CloseVideo();
                }
                if (this.lstArc.Items.Count > 0)
                {
                    var listItem = this.lstArc.FindItemWithText(item.bid);
                    if (listItem != null)
                    {
                        var index = listItem.Index;
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
            if (e.KeyCode == Keys.W)
            {
                this.UpSelectStatus();
            }
            else if (e.KeyCode == Keys.S)
            {
                this.DownSelectStatus();
            }
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Escape)
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

        void InitSpiderRunningConfig(bool onlyRead = false)
        {
            if (onlyRead)
            {
                this.RunningConfig.IgnoreDownloadSource = true;
                this.RunningConfig.IgnoreReadGetStatus = true;
                this.RunningConfig.MaxReadPageCount = 0;
            }
            else
            {
                this.RunningConfig.IgnoreDownloadSource = false;
                this.RunningConfig.MaxReadPageCount = 3;
            }
            this.spiderConfigUC1.Initialize(this.RunningConfig);
        }

        SpiderRunningConfig GetSpiderRunningConfig()
        {
            this.RunningConfig = this.spiderConfigUC1.GetRunningConfig();
            this.RunningConfig.Category = this.cbxCategory.Text.Trim();
            this.RunningConfig.Site = this.cbxSite.Text.Trim();
            return RunningConfig;
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

        void ActiveWebCtl()
        {
            InvokeControl(this.tabControl1, new Action(() =>
            {
                if (this.tabControl1.Enabled == false) return;
                this.tabControl1.SelectedIndex = 3;
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
