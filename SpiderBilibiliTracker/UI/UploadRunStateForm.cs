using SpiderCore.Entity;
using SpiderCore.Service;
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
    public partial class UploadRunStateForm : Form
    {
        BilibiliSpiderService SinaSpiderService;
        public UploadRunStateForm(BilibiliSpiderService spiderService)
        {
            InitializeComponent();

            SinaSpiderService = spiderService;
            SinaSpiderService.OnNewActions += SinaSpiderService_OnNewActions;
            SinaSpiderService.OnShowActionStatus += SinaSpiderService_OnShowActionStatus;
        }

        private void UploadRunStateForm_Load(object sender, EventArgs e)
        {
            var uploads = SinaSpiderService.GetWaitForDoActions();
            this.LoadUploadState(uploads.ToArray());
        }


        private void SinaSpiderService_OnShowActionStatus(SinaAction upload, string state)
        {
            InvokeControl(this.lstUpload, new Action(() =>
            {
                if (this.lstUpload.Items.Count == 0) return;

                var listItem = this.lstUpload.FindItemWithText(upload.actid);
                if (listItem != null)
                {
                    listItem.SubItems[6].Text = upload.actiontime;
                    listItem.SubItems[7].Text = state;
                }
            }));
        }

        private void SinaSpiderService_OnNewActions(SinaAction[] uploads)
        {
            this.LoadUploadState(uploads);
        }

        void LoadUploadState(SinaAction[] uploads)
        {
            InvokeControl(this.lstUpload, new Action(() =>
            {
                foreach (var item in uploads)
                {
                    var listItem = this.lstUpload.FindItemWithText(item.actid);
                    if (listItem != null) continue;

                    var subItem = new ListViewItem();
                    subItem.Text = item.actid;
                    subItem.SubItems.Add($"{item.category}");
                    subItem.SubItems.Add($"{item.uid}");
                    subItem.SubItems.Add($"{item.bid}");
                    subItem.SubItems.Add($"{item.file}");
                    subItem.SubItems.Add($"{(item.acttype == 0 ? "上传" : item.acttype == 1 ? "撤销" : "删除")}");
                    subItem.SubItems.Add($"{item.actiontime}");
                    subItem.SubItems.Add($"❌");
                    this.lstUpload.Items.Add(subItem);
                }
            }));
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
    }
}
