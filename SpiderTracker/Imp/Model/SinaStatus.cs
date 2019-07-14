using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class SinaStatus : BaseEntity
    {
        public SinaStatus()
        {
            lastdate = DateTime.Now; ;
        }
        /// <summary>
        /// 3314883543
        /// </summary>
        public string bid { get; set; }
        /// <summary>
        /// 3314883543
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// 4230218891961645
        /// </summary>
        public string mid { get; set; }
        public string url { get; set; }
        public int retweeted { get; set; }
        public string retuid { get; set; }
        public string retbid { get; set; }
        public string text { get; set; }
        public int pics { get; set; }
        public int getpics { get; set; }
        public int focus { get; set; }
        public int ignore { get; set; }
        public int mayfocus { get; set; }
        public int mayignore { get; set; }
    }
}
