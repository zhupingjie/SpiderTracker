using SpiderTracker.Imp;
using SpiderTracker.Imp.Model;
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
    public partial class FocusConfigForm : Form
    {
        public string SpiderName { get; set; }
        public string GatherUser { get; set; }
        public FocusConfigForm()
        {
            InitializeComponent();
        }

        private void CookieConfigForm_Load(object sender, EventArgs e)
        {
            var repository = new SinaRepository();
            var result = repository.GetAnalyseFocusUsers(SpiderName);
            this.listBox1.BeginUpdate();
            this.listBox1.Items.AddRange(result.Select(c => c.uid).ToArray());
            this.listBox1.EndUpdate();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;

            var users = new List<string>();
            foreach (var item in this.listBox1.SelectedItems)
            {
                users.Add(item.ToString());
            }

            var repository = new SinaRepository();
            var sinaUsers = repository.GetUsers(users.ToArray());
            foreach(var sinaUser in sinaUsers)
            {
                sinaUser.mayfocus = 2;
                repository.UpdateSinaUser(sinaUser, new string[] { "mayfocus" });
            }
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

        private void btnFocus_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;

            var users = new List<string>();
            foreach (var item in this.listBox1.SelectedItems)
            {
                users.Add(item.ToString());
            }

            var repository = new SinaRepository();
            var sinaUsers = repository.GetUsers(users.ToArray());
            foreach (var sinaUser in sinaUsers)
            {
                sinaUser.focus = 1;
                sinaUser.mayfocus = 2;
                repository.UpdateSinaUser(sinaUser, new string[] { "focus", "mayfocus" });
            }
            foreach (var user in users)
            {
                this.listBox1.Items.Remove(user);
            }
        }

        private void btnGather_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;

            this.GatherUser = this.listBox1.SelectedItem.ToString();
            this.Close();
        }
    }
}
