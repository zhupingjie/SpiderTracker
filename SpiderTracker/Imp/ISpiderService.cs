using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public interface ISpiderService
    {
        bool IsSpiderStarted { get; set; }

        void StartSpider(SpiderRunningConfig runninConfig);

        void StopSpider();
    }
}
