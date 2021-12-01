using SpiderCore.Config;
using SpiderCore.Repository;
using SpiderCore.Util;
using SpiderDomain.Entity;
using SpiderService.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class ServiceConfigForm : Form
    {
        public ServiceConfigForm()
        {
            InitializeComponent();
        }

        RunningConfig RunningConfig = new RunningConfig();

        SinaRepository Repository = new SinaRepository();

        private void ServiceConfigForm_Load(object sender, EventArgs e)
        {
            var service = new SpiderConfigSerivce();
            service.LoadConfig(RunningConfig);
            this.spiderConfiguc1.Initialize(RunningConfig);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            RunningConfig = this.spiderConfiguc1.GetRunningConfig();

            var configs = Repository.GetConfigs();
            var dic = ObjectUtil.GetPropertyValues(RunningConfig, true);
            foreach (var item in dic)
            {
                var config = configs.FirstOrDefault(c => c.Name == item.Key);
                if (config == null)
                {
                    config = new GlobalConfigEntity()
                    {
                        Name = item.Key,
                        Value = $"{item.Value}"
                    };
                    Repository.CreateSinaConfig(config);
                }
                else
                {
                    config.Value = $"{item.Value}";
                    Repository.UpdateSinaConfig(config);
                }
            }
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();   
        }
    }
}
