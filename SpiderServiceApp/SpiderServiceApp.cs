using SpiderService.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace SpiderServiceApp
{
    partial class SpiderServiceApp : ServiceBase
    {
        public SpiderServiceApp()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SpiderSinaService.Instance.Start();
        }

        protected override void OnStop()
        {
            SpiderSinaService.Instance.Stop();
        }
    }
}
