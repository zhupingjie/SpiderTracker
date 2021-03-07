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
    public partial class ViewImgForm : Form
    {
        public ViewImgForm()
        {
            InitializeComponent();
        }

        public string ViewImgPath { get; set; }

        private void ViewImgForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewImgPath))
            {
                var resImg = Image.FromFile(ViewImgPath);
                Image bmp = new Bitmap(resImg);
                resImg.Dispose();
                this.pictureBox1.Image = bmp;
            }
        }
    }
}
