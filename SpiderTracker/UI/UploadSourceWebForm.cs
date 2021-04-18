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
        public UploadSourceWebForm(SpiderRunningConfig runningConfig, string status)
        {
            InitializeComponent();

            this.RunningConfig = runningConfig;
            this.ShowStatus = status;
        }

        private void UploadSourceWebForm_Load(object sender, EventArgs e)
        {
            var images = HttpUtil.GetRemoteImageFiles(RunningConfig, ShowStatus, false);
            var html = MakeDocumentHtml(images);
            this.webBrowser1.DocumentText = html;
        }


        string MakeDocumentHtml(string[] files)
        {
            var sb = new StringBuilder();
            sb.Append($@"<style>
                .thumbimg {{
                    width: {RunningConfig.DefaultDisplayWebImageWidth}px;
                    height: {RunningConfig.DefaultDisplayWebImageHeight}px;
                    object-fit: cover;
                    margin: 5px;
                }}
            </style>");
            sb = MakeMakeDocumentDivHtml(sb, files, 3);
            return sb.ToString();
        }

        StringBuilder MakeMakeDocumentDivHtml(StringBuilder sb, string[] files, int rowCount)
        {
            var count = (int)Math.Ceiling(files.Length * 1.0 / rowCount * 1.0);
            for (var n = 0; n < count; n++)
            {
                sb.Append("<div>");
                var fs = files.Skip(n * rowCount).Take(rowCount).ToArray();
                for (var i = 0; i < 3; i++)
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
    }
}
