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
            this.GatherUserDataServiceInterval.Value = RunningConfig.GatherUserDataServiceInterval;
            this.GatherUserDataSort.Text = RunningConfig.GatherUserDataSort;
            this.GatherUserDataSortAsc.Text = RunningConfig.GatherUserDataSortAsc;
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

            var intervalConfig = configs.FirstOrDefault(c => c.Name == this.GatherUserDataServiceInterval.Name);
            if (intervalConfig == null)
            {
                intervalConfig = new GlobalConfigEntity()
                {
                    Name = this.GatherUserDataServiceInterval.Name,
                    Value = $"{this.GatherUserDataServiceInterval.Value}"
                };
                Repository.CreateSinaConfig(intervalConfig);
            }
            else
            {
                intervalConfig.Value = $"{this.GatherUserDataServiceInterval.Value}";
                Repository.UpdateSinaConfig(intervalConfig);
            }
            var sortConfig = configs.FirstOrDefault(c => c.Name == this.GatherUserDataSort.Name);
            if (sortConfig == null)
            {
                sortConfig = new GlobalConfigEntity()
                {
                    Name = this.GatherUserDataSort.Name,
                    Value = $"{this.GatherUserDataSort.Text}"
                };
                Repository.CreateSinaConfig(sortConfig);
            }
            else
            {
                sortConfig.Value = $"{this.GatherUserDataSort.Text}";
                Repository.UpdateSinaConfig(sortConfig);
            }
            var sortAscConfig = configs.FirstOrDefault(c => c.Name == this.GatherUserDataSortAsc.Name);
            if (sortAscConfig == null)
            {
                sortAscConfig = new GlobalConfigEntity()
                {
                    Name = this.GatherUserDataSortAsc.Name,
                    Value = $"{this.GatherUserDataSortAsc.Text}"
                };
                Repository.CreateSinaConfig(sortAscConfig);
            }
            else
            {
                sortAscConfig.Value = $"{this.GatherUserDataSortAsc.Text}";
                Repository.UpdateSinaConfig(intervalConfig);
            }

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();   
        }
    }
}
