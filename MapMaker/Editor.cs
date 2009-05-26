using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DockDotNET;
using System.IO;

namespace MapMaker
{
    public partial class Editor : Form
    {

        MapEditorState mes = new MapEditorState();
        DockManager dm;

        public Editor()
        {
            InitializeComponent();

            // load up the maps

            this.components = new Container();
            dm = new DockManager(this.components);
            dm.Dock = DockStyle.Fill;

            DockManager.FastMoveDraw = false;
            DockManager.Style = DockVisualStyle.VS2005;

            toolStripContainer1.ContentPanel.Controls.Add(this.dm);

            mes.dm = dm;

            mes.Init();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MapList f = new MapList(mes);
            dm.DockWindow(f, DockStyle.Right);
            f.Show();
        }


        
    }
}
