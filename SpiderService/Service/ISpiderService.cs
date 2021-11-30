using SpiderCore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderService.Service
{
    public interface ISpiderService
    {
        bool IsSpiderStarted { get; set; }

        void StartSpider(RunningConfig runninConfig, SpiderStartOption option);

        void StopSpider();
    }
}
