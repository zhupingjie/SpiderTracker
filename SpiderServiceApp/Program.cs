using SpiderCore.Util;
using SpiderService.Service;
using System;
using System.ServiceProcess;

namespace SpiderServiceApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogUtil.Debug($"程序开始启动...");
#if DEBUG
            SpiderSinaService.Instance.Start();
            while (true)
            {
                var input = Console.ReadLine();
                if (input.Equals("exit"))
                {
                    LogUtil.Debug($"程序准备结束运行...");
                    break;
                }
            }
#else
            ServiceBase[] services = new ServiceBase[] { new SpiderServiceApp() };
            ServiceBase.Run(services);
#endif
            LogUtil.Debug($"程序结束运行...");
        }
    }
}
