using SpiderTracker.Imp;
using SpiderTracker.Imp.Util;
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
        private SpiderRunningConfig _runningConfig;
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
                    (findCtl as TextBox).TextChanged += SpiderConfigUC_TextChanged;
                }

            }
        }

        private void SpiderConfigUC_TextChanged(object sender, EventArgs e)
        {
            RefreshConfig(GetRunningConfig(_runningConfig));
        }

        private void SpiderConfigUC_ValueChanged(object sender, EventArgs e)
        {
            RefreshConfig(GetRunningConfig(_runningConfig));
        }

        private void SpiderConfigUC_CheckedChanged(object sender, EventArgs e)
        {
            RefreshConfig(GetRunningConfig(_runningConfig));
        }

        public delegate void RefreshConfigEventHander(SpiderRunningConfig spiderRunninConfig);

        public event RefreshConfigEventHander OnRefreshConfig;

        public void RefreshConfig(SpiderRunningConfig spiderRunninConfig)
        {
            OnRefreshConfig?.Invoke(spiderRunninConfig);
        }

        /// <summary>
        /// TODO:初始化后变量值丢掉
        /// </summary>
        /// <param name="runningConfig"></param>
        public void Initialize(SpiderRunningConfig runningConfig)
        {
            _runningConfig = runningConfig.Clone();

            var configs = GetConfigFields();
            foreach (var config in configs)
            {
                var findCtl = this.Controls.Find(config.Name, true).FirstOrDefault();
                if (findCtl == null) continue;

                var objValue = ObjectUtil.GetPropertyValue(runningConfig, config);
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
                    (findCtl as TextBox).Text = $"{objValue}";
                }
            }
        }

        public SpiderRunningConfig GetRunningConfig(SpiderRunningConfig runningConfig)
        {
            var configs = GetConfigFields();
            foreach(var config in configs)
            {
                var findCtl = this.Controls.Find(config.Name, true).FirstOrDefault();
                if (findCtl == null) continue;

                if (config.PropertyType.Equals(typeof(bool)))
                {
                    ObjectUtil.SetPropertyValue(runningConfig, config, (findCtl as CheckBox).Checked);
                }
                else if (config.PropertyType.Equals(typeof(int)))
                {
                    ObjectUtil.SetPropertyValue(runningConfig, config, (int)(findCtl as NumericUpDown).Value);
                }
                else if (config.PropertyType.Equals(typeof(string)))
                {
                    ObjectUtil.SetPropertyValue(runningConfig, config, (findCtl as TextBox).Text);
                }
            }
            _runningConfig = runningConfig.Clone();
            return runningConfig;
        }

        PropertyInfo[] GetConfigFields()
        {
            var configs = new List<PropertyInfo>();
            var props = typeof(SpiderRunningConfig).GetProperties();
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
    }
}
