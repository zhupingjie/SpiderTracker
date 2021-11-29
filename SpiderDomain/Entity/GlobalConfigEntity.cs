using SpiderDomain.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDomain.Entity
{
    public class GlobalConfigEntity : BaseEntity
    {
        [Description("配置项")]
        public string Name { get; set; }

        [Description("配置值")]
        public string Value { get; set; }
    }
}
