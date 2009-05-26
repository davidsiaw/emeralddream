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

        public MapList(MapEditorState mes)
        {
            InitializeComponent();
            mes.maps.ForEach(x => { 
                listBox1.Items.Add(new MapInfo(x)); 
            });
        }


    }
}
