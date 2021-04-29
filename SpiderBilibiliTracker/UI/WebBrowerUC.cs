using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class WebBrowerUC : UserControl
    {
        public WebBrowerUC()
        {
            InitializeComponent();

            //Xpcom.Initialize("Firefox");
            //this.geckoWebBrowser1.ProgressChanged += Gecko_ProgressChanged; ;
            //this.geckoWebBrowser1.CreateWindow += Gecko_CreateWindow; ;
            //this.geckoWebBrowser1.DocumentCompleted += Gecko_DocumentCompleted; ;
        }

        public void ShowUrl(string url)
        {
        //    if (this.geckoWebBrowser1.Url.AbsoluteUri == url) return;

        //    this.geckoWebBrowser1.Navigate(url);
        }

        //private void WebBrowerUC_Load(object sender, EventArgs e)
        //{
            
        //}

        //private void Gecko_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        //{
        //}

        //private void Gecko_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        //{
        //}

        //private void Gecko_ProgressChanged(object sender, GeckoProgressEventArgs e)
        //{
        //    if (e.MaximumProgress == 0)
        //        return;

        //    var value = (int)Math.Min(100, (e.CurrentProgress * 100) / e.MaximumProgress);
        //    if (value == 100)
        //        return;
        //    progressBar1.Value = value;
        //}
    }
}
