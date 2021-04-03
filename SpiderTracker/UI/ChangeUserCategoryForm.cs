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
    public partial class ChangeUserCategoryForm : Form
    {
        public ChangeUserCategoryForm()
        {
            InitializeComponent();
        }

        public string ChangeCategory { get; set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.ChangeCategory = this.cbxCategory.Text;
            if (string.IsNullOrEmpty(ChangeCategory)) return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ChangeUserCategoryForm_Load(object sender, EventArgs e)
        {
            var rep = new SinaRepository();
            var names = rep.GetGroupNames();

            this.cbxCategory.BeginUpdate();
            this.cbxCategory.Items.Clear();
            this.cbxCategory.Items.AddRange(names);
            this.cbxCategory.EndUpdate();
        }
    }
}
