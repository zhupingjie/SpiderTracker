using SpiderCore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Service
{
    public interface ISpiderService
    {
        bool IsSpiderStarted { get; set; }

        void StartSpider(SpiderRunningConfig runninConfig, SpiderStartOption option);

        void StopSpider();
    }
}
