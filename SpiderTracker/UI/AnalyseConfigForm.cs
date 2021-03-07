using SpiderTracker.Imp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class AnalyseConfigForm : Form
    {
        public string SpiderName { get; set; }
        public AnalyseConfigForm()
        {
            InitializeComponent();
        }

        private void CookieConfigForm_Load(object sender, EventArgs e)
        {
            var result = PathUtil.GetStoreConfigResult(SpiderName, StoreConfigEnum.StoreAnalyse);
            this.listBox1.BeginUpdate();
            this.listBox1.Items.AddRange(result.ToArray());
            this.listBox1.EndUpdate();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;

            if (MessageBox.Show("确认拉黑当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var users = new List<string>();
            foreach (var item in this.listBox1.SelectedItems)
            {
                users.Add(item.ToString());
            }
            UserUtil.UpdateIgnoreUsers(SpiderName, users);
            UserUtil.RemoveAnalyseUsers(SpiderName, users);

            foreach (var user in users)
            {
                this.listBox1.Items.Remove(user);
            }
        }
        private void btnFocus_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;

            if (MessageBox.Show("确认关注当前用户?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var users = new List<string>();
            foreach (var item in this.listBox1.SelectedItems)
            {
                users.Add(item.ToString());
            }
            UserUtil.UpdateFocusUsers(SpiderName, users);
            UserUtil.RemoveAnalyseUsers(SpiderName, users);

            foreach (var user in users)
            {
                this.listBox1.Items.Remove(user);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;

            var user = this.listBox1.SelectedItem.ToString();
            var url = SinaUrlUtil.GetSinaUserUrl(user);
            System.Diagnostics.Process.Start(url);
        }

    }
}
