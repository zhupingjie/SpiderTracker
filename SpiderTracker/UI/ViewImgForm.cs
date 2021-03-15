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

        public string[] ViewImgPaths { get; set; }
        public int ViewImgIndex { get; set; }

        private void ViewImgForm_Load(object sender, EventArgs e)
        {
            if (ViewImgPaths != null && ViewImgPaths.Length > 0 && ViewImgIndex < ViewImgPaths.Length)
            {
                this.imageCtl.BackgroundImage = Image.FromFile(ViewImgPaths[ViewImgIndex]);
            }
        }

        private void imageLeft_Click(object sender, EventArgs e)
        {
            ViewImgIndex--;
            if (ViewImgIndex < 0) ViewImgIndex = 0;
            if (ViewImgPaths != null && ViewImgPaths.Length > 0 && ViewImgIndex < ViewImgPaths.Length)
            {
                this.imageCtl.BackgroundImage = Image.FromFile(ViewImgPaths[ViewImgIndex]);
            }
        }

        private void imageRight_Click(object sender, EventArgs e)
        {
            ViewImgIndex++;
            if (ViewImgIndex >= ViewImgPaths.Length) ViewImgIndex = ViewImgPaths.Length - 1;
            if (ViewImgPaths != null && ViewImgPaths.Length > 0 && ViewImgIndex < ViewImgPaths.Length)
            {
                this.imageCtl.BackgroundImage = Image.FromFile(ViewImgPaths[ViewImgIndex]);
            }
        }

        private void ViewImgForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    imageLeft_Click(sender, e);
                    break;
                case Keys.Right:
                    imageRight_Click(sender, e);
                    break;
            }
        }
    }
}
