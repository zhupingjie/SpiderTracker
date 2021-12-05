using SpiderCore.Config;
using SpiderCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class SpiderConfigUC : UserControl
    {
        private RunningConfig _runningConfig;
        public SpiderConfigUC()
        {
            InitializeComponent();

            var configs = GetConfigFields();
            foreach (var config in configs)
            {
                var findCtl = this.Controls.Find(config.Name, true).FirstOrDefault();
                if (findCtl == null) continue;

                if (config.PropertyType.Equals(typeof(bool)))
                {
                    (findCtl as CheckBox).CheckedChanged += SpiderConfigUC_CheckedChanged;
                }
                else if (config.PropertyType.Equals(typeof(int)))
                {
                    (findCtl as NumericUpDown).ValueChanged += SpiderConfigUC_ValueChanged;
                }
                else if (config.PropertyType.Equals(typeof(string)))
                {
                    if (findCtl is ComboBox)
                    {
                        (findCtl as ComboBox).SelectedValueChanged += SpiderConfigUC_SelectedValueChanged;
                    }
                    else if (findCtl is TextBox)
                    {
                        (findCtl as TextBox).TextChanged += SpiderConfigUC_TextChanged;
                    }
                }

            }
        }

        private void SpiderConfigUC_SelectedValueChanged(object sender, EventArgs e)
        {
            RefreshConfig(_runningConfig);
        }

        private void SpiderConfigUC_TextChanged(object sender, EventArgs e)
        {
            RefreshConfig(_runningConfig);
        }

        private void SpiderConfigUC_ValueChanged(object sender, EventArgs e)
        {
            RefreshConfig(_runningConfig);
        }

        private void SpiderConfigUC_CheckedChanged(object sender, EventArgs e)
        {
            RefreshConfig(_runningConfig);
        }

        public delegate void RefreshConfigEventHander(RunningConfig spiderRunninConfig);

        public event RefreshConfigEventHander OnRefreshConfig;

        public void RefreshConfig(RunningConfig spiderRunninConfig)
        {
            OnRefreshConfig?.Invoke(spiderRunninConfig);
        }

        /// <summary>
        /// TODO:初始化后变量值丢掉
        /// </summary>
        /// <param name="runningConfig"></param>
        public void Initialize(RunningConfig runningConfig)
        {
            _runningConfig = runningConfig.Clone();

            var configs = GetConfigFields();
            foreach (var config in configs)
            {
                var findCtl = this.Controls.Find(config.Name, true).FirstOrDefault();
                if (findCtl == null) continue;

                var objValue = ObjectUtil.GetPropertyValue(_runningConfig, config);
                if (objValue == null) continue;

                if (config.PropertyType.Equals(typeof(bool)))
                {
                    (findCtl as CheckBox).Checked = bool.Parse($"{objValue}");
                }
                else if (config.PropertyType.Equals(typeof(int)))
                {
                    (findCtl as NumericUpDown).Value = int.Parse($"{objValue}");
                }
                else if (config.PropertyType.Equals(typeof(string)))
                {
                    if (findCtl is ComboBox)
                    {
                        (findCtl as ComboBox).Text = $"{objValue}";
                    }
                    else if (findCtl is TextBox)
                    {
                        (findCtl as TextBox).Text = $"{objValue}";
                    }
                }
            }
        }

        public RunningConfig GetRunningConfig()
        {
            var configs = GetConfigFields();
            foreach(var config in configs)
            {
                var findCtl = this.Controls.Find(config.Name, true).FirstOrDefault();
                if (findCtl == null) continue;

                if (config.PropertyType.Equals(typeof(bool)))
                {
                    ObjectUtil.SetPropertyValue(_runningConfig, config, (findCtl as CheckBox).Checked);
                }
                else if (config.PropertyType.Equals(typeof(int)))
                {
                    ObjectUtil.SetPropertyValue(_runningConfig, config, (int)(findCtl as NumericUpDown).Value);
                }
                else if (config.PropertyType.Equals(typeof(string)))
                {
                    if (findCtl is ComboBox)
                    {
                        ObjectUtil.SetPropertyValue(_runningConfig, config, (findCtl as ComboBox).Text);
                    }
                    else if (findCtl is TextBox)
                    {
                        ObjectUtil.SetPropertyValue(_runningConfig, config, (findCtl as TextBox).Text);
                    }
                    
                }
            }
            return _runningConfig;
        }

        PropertyInfo[] GetConfigFields()
        {
            var configs = new List<PropertyInfo>();
            var props = typeof(RunningConfig).GetProperties();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(OptionAttribute), true).Length == 0) continue;

                configs.Add(prop);
            }
            return configs.ToArray();
        }

        private void GatherThreadCount_ValueChanged(object sender, EventArgs e)
        {

        }

        private void StartPageIndex_ValueChanged(object sender, EventArgs e)
        {

        }

        private void MaxReadPageCount_ValueChanged(object sender, EventArgs e)
        {

        }

        private void MinReadImageCount_ValueChanged(object sender, EventArgs e)
        {

        }

        private void ReadOwnerUserStatus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ReadAllOfUser_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ReadUserOfMyFocus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ReadUserOfHeFocus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void IgnoreReadArchiveStatus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void IgnoreReadGetStatus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void GatherUserDataServiceInterval_ValueChanged(object sender, EventArgs e)
        {

        }

        private void GatherUserDataSort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GatherUserDataSortAsc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
