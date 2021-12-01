using SpiderCore.Config;
using SpiderCore.Repository;
using SpiderCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderService.Service
{
    public class SpiderConfigSerivce
    {
        SinaRepository Repository;

        public SpiderConfigSerivce()
        {
            Repository = new SinaRepository();
        }

        public void LoadConfig(RunningConfig runningConfig)
        {
            var configs = Repository.GetConfigs();
            foreach (var config in configs)
            {
                var prepInfo = ObjectUtil.GetPropertyInfo(runningConfig, config.Name);
                if (prepInfo == null) continue;

                var value = ObjectUtil.ToValue(config.Value, prepInfo.PropertyType);
                if (value != null)
                {
                    ObjectUtil.SetPropertyValue(runningConfig, config.Name, value);
                }
            }
        }
    }
}
