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

namespace SpiderTracker.UI
{
    public partial class SinaLoginForm : Form
    {
        SpiderRunningConfig _runningConfig;

        public SinaLoginForm(SpiderRunningConfig runninConfig)
        {
            InitializeComponent();

            _runningConfig = runninConfig;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.txtUserName.Text) || string.IsNullOrEmpty(this.txtPassword.Text))
            {
                MessageBox.Show("用户名或密码不能为空!");
                return;
            }

            var login = HttpUtil.SinaLogin(_runningConfig, this.txtUserName.Text, this.txtPassword.Text);
            if (!login)
            {
                MessageBox.Show("用户名登陆失败!");
                return;
            }

            var token = HttpUtil.GetSinaLoginToken(_runningConfig);
            if (!token)
            {
                MessageBox.Show("获取用户授权失败!");
                return;
            }


            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SinaLoginForm_Load(object sender, EventArgs e)
        {
            this.txtUserName.Text = _runningConfig.LoginUser;
        }
    }
}
