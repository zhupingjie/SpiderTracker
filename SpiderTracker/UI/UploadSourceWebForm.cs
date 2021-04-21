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
    public partial class UploadSourceWebForm : Form
    {
        SpiderRunningConfig RunningConfig;
        string ShowStatus;
        string ShowName;
        bool ShowThumb;
        public UploadSourceWebForm(SpiderRunningConfig runningConfig, string status, string filename, bool thumb)
        {
            InitializeComponent();

            this.RunningConfig = runningConfig;
            this.ShowStatus = status;
            this.ShowName = filename;
            this.ShowThumb = thumb;
        }

        private void UploadSourceWebForm_Load(object sender, EventArgs e)
        {
            var images = HttpUtil.GetRemoteImageFiles(RunningConfig, ShowStatus, ShowName, ShowThumb);
            if (images.Length == 0)
            {
                var source = new SinaRepository().GetUserSource(ShowStatus, ShowName);
                if (source != null)
                {
                    images = new string[] { source.url };
                }
            }
            if (images.Length > 0)
            {
                var html = MakeDocumentHtml(images);
                this.txtWebUrl.Text = images.FirstOrDefault();
                this.webBrowser1.DocumentText = html;
            }
            else
            {
                this.webBrowser1.DocumentText = $"<h1>无有效的图片地址</h1>";
            }
        }


        string MakeDocumentHtml(string[] files)
        {
            var width = $"{RunningConfig.DefaultDisplayWebImageWidth}px";
            var height = $"{RunningConfig.DefaultDisplayWebImageHeight}px";
            if(files.Length == 1)
            {
                width = "auto";
                height = "auto";
            }
            var sb = new StringBuilder();
            sb.Append($@"<style>
                .thumbimg {{
                    width: {width};
                    height: {height};
                    object-fit: cover;
                    margin: 5px;
                }}
            </style>");
            sb = MakeMakeDocumentDivHtml(sb, files, RunningConfig.DefaultDisplayWebImageRowCount);
            return sb.ToString();
        }

        StringBuilder MakeMakeDocumentDivHtml(StringBuilder sb, string[] files, int rowCount)
        {
            var count = (int)Math.Ceiling(files.Length * 1.0 / rowCount * 1.0);
            for (var n = 0; n < count; n++)
            {
                sb.Append("<div>");
                var fs = files.Skip(n * rowCount).Take(rowCount).ToArray();
                for (var i = 0; i < rowCount; i++)
                {
                    if (fs.Length > i && fs.Length > 0)
                    {
                        sb.Append($"<img class='thumbimg' src='{fs[i]}' />");
                    }
                }
                sb.Append("</div>");
            }

            return sb;
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(this.txtWebUrl.Text);
        }
    }
}
