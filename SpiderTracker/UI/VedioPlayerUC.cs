using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderTracker.UI
{
    public partial class VedioPlayerUC : UserControl
    {
        public VedioPlayerUC()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public void ShowVideo(string file)
        {
            if (this.axWindowsMediaPlayer1.URL != file)
            {
                this.axWindowsMediaPlayer1.close();
                this.axWindowsMediaPlayer1.URL = file;
            }
        }

        public void CloseVideo()
        {
            if (this.axWindowsMediaPlayer1.URL != null)
            {
                this.axWindowsMediaPlayer1.close();
            }
        }
    }
}
