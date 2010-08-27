using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DockDotNET;
using EmeraldLibrary;

namespace MapMaker
{
    public partial class MapEditor : DockWindow
    {
        int mouseoverpointx = -1;
        int mouseoverpointy = -1;
        public MapEditor(TileBank tb, MapEditorState mes, string mapname)
        {
            InitializeComponent();
            pictureBox1.TileBank = tb;
            this.FormClosing += new FormClosingEventHandler((o, e) => { mes.CloseMapEditor(mapname); });
            vScrollBar1.LargeChange = 1;
            hScrollBar1.LargeChange = 1;

            pictureBox1.MouseMove += new MouseEventHandler((o, e) => {
                mes.coordlabel.Text = "Coords: x=" + mouseoverpointx + ", y=" + mouseoverpointy;
                m_map.ScreenToMapCoords(e.X, e.Y, out mouseoverpointx, out mouseoverpointy);

                if (e.Button == MouseButtons.Left)
                {
                    mes.ApplyFloor(m_map, mouseoverpointx, mouseoverpointy);
                }
            });

            pictureBox1.MouseDown += new MouseEventHandler((o, e) =>
            {
                m_map.ScreenToMapCoords(e.X, e.Y, out mouseoverpointx, out mouseoverpointy);

                if (e.Button == MouseButtons.Left)
                {
                    mes.ApplyFloor(m_map, mouseoverpointx, mouseoverpointy);
                }
            });

        }
        
        public void SceneHasBeenSet()
        {
            Layer selectionmarker;
            selectionmarker = new Layer(g =>
            {
                if (mouseoverpointx > -1 && mouseoverpointy > -1)
                {
                    int xx, yy;
                    m_map.MapToScreenCoords(mouseoverpointx, mouseoverpointy, out xx, out yy);
                    g.DrawRectangle(new Pen(Color.Cyan), xx, yy, 32, 32);
                }
            });
            pictureBox1.Scene.AddLayer(selectionmarker);
            selectionmarker.Visible = true;
        }

        public Map Map
        {
            get
            {
                return m_map;
            }
            set
            {

                m_map = value;
                pictureBox1_SizeChanged(this, null);
            }
        }

        Map m_map;

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Map.CameraY = vScrollBar1.Value + pictureBox1.Height / 32 / 2;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Map.CameraX = hScrollBar1.Value + pictureBox1.Width / 32 / 2;
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (Map != null)
            {
                vScrollBar1.Minimum = 0;
                vScrollBar1.Maximum = Math.Max(0, Map.Height - (pictureBox1.Height / 32));
                hScrollBar1.Minimum = 0;
                hScrollBar1.Maximum = Math.Max(0, Map.Width - (pictureBox1.Width / 32));
                Console.WriteLine("min {0}", hScrollBar1.Maximum);
            }
        }


    }
}
