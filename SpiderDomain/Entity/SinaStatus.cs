using SpiderDomain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDomain.Entity
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
        /// 0：图片，1：视频
        /// </summary>
        public int mtype { get; set; }
        /// <summary>
        /// 4230218891961645
        /// </summary>
        public string mid { get; set; }
        public string url { get; set; }
        public string text { get; set; }
        public int retweeted { get; set; }
        public string retuid { get; set; }
        public string retbid { get; set; }
        public int qty { get; set; }
        public int gets { get; set; }
        public int ignore { get; set; }
        public int upload { get; set; }
        public string site { get; set; }
        public string createtime { get; set; }
    }
}
