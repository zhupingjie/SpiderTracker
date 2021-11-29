using SpiderCore.Config;
using SpiderCore.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderService.Service
{
    public class SpiderSinaService
    {
        #region 静态实例

        static readonly SpiderSinaService instance = new SpiderSinaService();
        public static SpiderSinaService Instance
        {
            get
            {
                return instance;
            }
        }
        static SpiderSinaService()
        {

        }
        private SpiderSinaService()
        {

        }
        #endregion

        #region 内部属性

        /// <summary>
        /// 线程资源
        /// </summary>
        CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 环境变量
        /// </summary>
        SpiderRunningConfig RC = new SpiderRunningConfig();


        #endregion

        #region 服务启动&停止
        public void Start()
        {
            this.LoadConfigData();

            this.GatherData();
        }

        public void Stop()
        {
            this.CancellationTokenSource.Cancel();

            LogUtil.Debug($"程序停止运行...");
        }

        #endregion

        #region 启动事件

        void LoadConfigData()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    this.ActionLog("加载全局配置数据...");
                    try
                    {
                        //StockConfigService.LoadGlobalConfig(RC);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.Error($"LoadGlobalConfig Error:{ex.Message}");
                    }
                    //Thread.Sleep(RC.LoadGlobalConfigInterval * 1000);
                }
            }, CancellationTokenSource.Token);
        }

        void GatherData()
        {
            //采集价格数据
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2000);
                while (true)
                {
                    this.ActionLog("采集今日股价数据...");
                    try
                    {
                        //StockGatherService.GatherPriceData((message) =>
                        //{
                        //    this.ActionLog(message);
                        //});
                    }
                    catch (Exception ex)
                    {
                        LogUtil.Error($"GatherPriceData Error:{ex.Message}");
                    }
                    //Thread.Sleep(RC.GatherStockPriceInterval * 1000);
                }
            }, CancellationTokenSource.Token);
        }

        #endregion

        #region 通知消息日志
        public void ActionLog(string message)
        {
            //Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}\n");

            LogUtil.Debug(message);
        }
        #endregion
    }
}
