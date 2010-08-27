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
    public partial class MapList : DockWindow
    {
        class MapInfo
        {
            public MapInfo(string dirname)
            {
                fullname = dirname;
                name = Path.GetFileName(dirname);
            }

            string name;
            string fullname;

            public override string ToString()
            {
                return name;
            }
        }

        public MapList(MapEditorState mes, ToolStripMenuItem mnuitem)
        {
            InitializeComponent();
            mes.maps.Keys.ToList().ForEach(x => { 
                listBox1.Items.Add(new MapInfo(x)); 
            });

            mes.RegisterWindow("MapList", this, mnuitem);
            this.FormClosing += new FormClosingEventHandler((o, e) => { mes.DeregisterWindow("MapList"); });
            this.mes = mes;
        }

        MapEditorState mes;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                mes.OpenMapEditor((listBox1.SelectedItem as MapInfo).ToString());
            }
        }


    }
}
