using SpiderCore.Config;
using SpiderCore.Repository;
using SpiderCore.Util;
using System;
using System.Collections.Generic;
using System.Linq;
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
        RunningConfig RC = new RunningConfig();

        SinaRepository Repository = new SinaRepository();

        MWeiboSpiderService SpiderService = new MWeiboSpiderService();

        List<string> Categorys = new List<string>();

        SpiderConfigSerivce ConfigService = new SpiderConfigSerivce();

        int RunCategoryIndex = 0;
        #endregion

        #region 服务启动&停止
        public void Start()
        {
            this.Initation();
            
            this.GatherData();
        }

        public void Initation()
        {
            SpiderService.OnGatherUserComplete += SpiderService_OnGatherUserComplete;
            SpiderService.OnSpiderComplete += SpiderService_OnSpiderComplete;
            SpiderService.OnSpiderStarted += SpiderService_OnSpiderStarted;

            this.Categorys.Clear();
            this.Categorys.AddRange(Repository.GetGroupNames(GatherWebEnum.Sina));
            this.RunCategoryIndex = 0;
        }


        void SpiderService_OnSpiderStarted(RunningTask runningTask)
        {
            this.ActionLog($"采集[{runningTask.DoUsers.Count}]用户最新数据开始");
        }

        void SpiderService_OnSpiderComplete(string category)
        {
            this.ActionLog($"采集[{category}]用户最新数据完成");
            this.RunCategoryIndex++;
            if(this.RunCategoryIndex >= Categorys.Count)
            {
                Thread.Sleep(RC.GatherUserDataServiceInterval * 1000);
                this.RunCategoryIndex = 0;
            }
            this.GatherData();
        }

        void SpiderService_OnGatherUserComplete(SpiderDomain.Entity.SinaUser user, int readImageQty)
        {
            this.ActionLog($"采集[{user.category}]用户[{user.uid}]数据OK");
        }

        public void Stop()
        {
            this.CancellationTokenSource.Cancel();

            LogUtil.Debug($"程序停止运行...");
        }

        #endregion

        #region 启动事件

        void GatherData()
        {
            try
            {
                this.ConfigService.LoadConfig(RC);

                this.Categorys.Clear();
                this.Categorys.AddRange(Repository.GetGroupNames(GatherWebEnum.Sina));

                string category = null;
                if (RunCategoryIndex < Categorys.Count)
                {
                    category = Categorys[RunCategoryIndex];
                }
                else if (Categorys.Count > 0)
                {
                    category = Categorys[0];
                }
                if (!string.IsNullOrEmpty(category))
                {
                    this.ActionLog($"采集[{category}]用户最新数据...");

                    GatherUserData(category);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex, $"GatherData异常:{ex.Message}");
            }
        }

        void GatherUserData(string category)
        {
            try
            {
                var runningConfig = RC.Clone();
                runningConfig.Category = category;
                runningConfig.Site = "app";

                var startOption = new SpiderStartOption()
                {
                    GatherName = "user",
                };
                SpiderService.StartSpider(runningConfig, startOption);
            }
            catch(Exception ex)
            {
                LogUtil.Error($"GatherUserData Error:{ex.Message}");
            }
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
