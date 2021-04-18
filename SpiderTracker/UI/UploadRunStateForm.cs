using SpiderTracker.Imp;
using SpiderTracker.Imp.Model;
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
        MWeiboSpiderService SinaSpiderService;
        public UploadRunStateForm(MWeiboSpiderService spiderService)
        {
            InitializeComponent();

            SinaSpiderService = spiderService;
            SinaSpiderService.OnSpiderUploadShow += SinaSpiderService_OnSpiderUploadShow;
            SinaSpiderService.OnSpiderUploadRefresh += SinaSpiderService_OnSpiderUploadComplete;
        }

        private void UploadRunStateForm_Load(object sender, EventArgs e)
        {
            var uploads = SinaSpiderService.GetUploadTask();
            this.LoadUploadState(uploads.ToArray());
        }


        private void SinaSpiderService_OnSpiderUploadComplete(SinaUpload upload, string state)
        {
            InvokeControl(this.lstUpload, new Action(() =>
            {
                if (this.lstUpload.Items.Count == 0) return;

                var listItem = this.lstUpload.FindItemWithText(upload.file);
                if (listItem != null)
                {
                    listItem.SubItems[3].Text = upload.uploadtime;
                    listItem.SubItems[4].Text = state;
                }
            }));
        }

        private void SinaSpiderService_OnSpiderUploadShow(SinaUpload[] uploads)
        {
            this.LoadUploadState(uploads);
        }

        void LoadUploadState(SinaUpload[] uploads)
        {
            InvokeControl(this.lstUpload, new Action(() =>
            {
                foreach (var item in uploads)
                {
                    var listItem = this.lstUpload.FindItemWithText(item.file);
                    if (listItem != null) continue; ;

                    var subItem = new ListViewItem();
                    subItem.Text = item.file;
                    subItem.SubItems.Add($"{item.category}");
                    subItem.SubItems.Add($"{item.uid}");
                    subItem.SubItems.Add($"{item.uploadtime}");
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
