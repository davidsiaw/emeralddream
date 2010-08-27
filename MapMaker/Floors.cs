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
    public partial class Floors : DockWindow
    {
        public Floors(MapEditorState mes, ToolStripMenuItem tsmi)
        {
            InitializeComponent();
            mes.RegisterWindow("Floors", this, tsmi);
            this.FormClosing += new FormClosingEventHandler((o, e) =>
            {
                mes.DeregisterWindow("Floors");
            });

            mes.floortypes.ToList().ForEach(x =>
            {

                MapFloor floor = (MapFloor)Activator.CreateInstance(x.Value);
                Image img = mes.tilebank.GetTileImage(floor.TileIndex);

                RadioButton rb = new RadioButton();
                rb.Appearance = Appearance.Button;
                rb.Image = img;
                rb.Size = img.Size;
                flowLayoutPanel1.Controls.Add(rb);
                rb.Tag = floor;
                rb.MouseDown += new MouseEventHandler((ob, ev) =>
                {
                    if (ev.Button == MouseButtons.Right)
                    {   
                        contextMenuStrip1.Show(rb, ev.Location);
                        contextMenuStrip1.Tag = rb;
                    }
                });
                rb.CheckedChanged += new EventHandler((ob, ev) => {
                    if (rb.Checked)
                    {
                        mes.selectedFloor = x.Value;
                    }
                });

                if (mes.selectedFloor == x.Value)
                {
                    rb.Checked = true;
                }

            });


        }


    }
}
