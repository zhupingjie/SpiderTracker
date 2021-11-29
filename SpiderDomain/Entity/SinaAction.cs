using SpiderDomain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDomain.Entity
{
    public class SinaAction : BaseEntity
    {
        public SinaAction()
        {
            lastdate = DateTime.Now;
        }

        public string actid { get; set; }

        /// <summary>
        /// 0:上传,1:撤销上传,2:忽略
        /// </summary>
        public int acttype { get; set; }
        public string category { get; set; }

        public string uid { get; set; }

        public string bid { get; set; }

        public string file { get; set; }

        public string createtime { get; set; }
        
        public int action { get; set; }

        public string actiontime { get; set; }
    }
}
