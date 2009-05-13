using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapMaker
{
    public partial class MapConfig : Form
    {
        public MapConfig()
        {
            InitializeComponent();
        }

        public int MapWidth
        {
            get
            {
                return int.Parse(txt_width.Text);
            }
        }

        public int MapHeight
        {
            get
            {
                return int.Parse(txt_height.Text);
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
