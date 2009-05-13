using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EmeraldLibrary;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MapMaker
{
    public partial class TileSelection : Form
    {
        class FloorTile : MapFloor
        {
            int tile = 0;

            public FloorTile(int tile)
            {
                this.tile = tile;
            }
            #region MapFloor Members

            public bool Passable
            {
                get { return true; }
            }

            #endregion

            #region MapObject Members

            public int TileIndex
            {
                get { return tile; }
            }

            #endregion
        }

        class AnObject : MapObject
        {
            int tile = 0;

            public AnObject(int tile)
            {
                this.tile = tile;
            }

            #region MapObject Members

            public int TileIndex
            {
                get { return tile; }
            }

            #endregion
        }

        int selectedtile = 0;

        Scene s;
        GameWindow win;
        AnObject selectionbox;
        Map m;

        public TileSelection(GameWindow win)
        {
            InitializeComponent();
            this.win = win;

            // Grab tiles
            string[] files = Directory.GetFiles("edit/res/tiles", "*.png");
            files.ToList().ForEach(x => {
                RadioButton cb = new RadioButton();
                cb.Appearance = Appearance.Button;
                cb.Image = Bitmap.FromFile(x);
                cb.Size = new Size(32, 32);
                flow_tiles.Controls.Add(cb);

                string tilename = Path.GetFileNameWithoutExtension(x);
                cb.Tag = win.LoadTile(tilename, x);

                cb.CheckedChanged += new EventHandler((ob, ev) => {
                    selectedtile = (int)cb.Tag;
                });
            });

            MapList ml = new MapList();
            ml.Show();

            (flow_tiles.Controls[0] as RadioButton).Checked = true;

            // Make the selection box
            Bitmap box = new Bitmap(32, 32);
            using (Graphics gfx = Graphics.FromImage(box))
            {
                gfx.FillRectangle(new SolidBrush(Color.FromArgb(0, Color.Black)), new Rectangle(0, 0, 32, 32));
                gfx.DrawRectangle(new Pen(Color.Cyan), new Rectangle(0, 0, 31, 31));
            }
      
            selectionbox = new AnObject(win.LoadTile("selection", box));

            m = new Map(win, 30, 30);

            m.CheckPassable = false;
            m.SetCeiling(selectionbox, 0, 0);
            m.FillFloor(new FloorTile(0));

             s = new Scene(m);
            win.Scene = s;

            // Set up the keys
            s.KeyDown += new EventHandler<KeyEventArgs>((o, e) =>
            {
                m.SetCeiling(null, m.CameraX, m.CameraY);
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        m.CameraY--;
                        break;
                    case Keys.Down:
                        m.CameraY++;
                        break;
                    case Keys.Left:
                        m.CameraX--;
                        break;
                    case Keys.Right:
                        m.CameraX++;
                        break;
                    case Keys.Space:
                        m.SetFloor(new FloorTile(selectedtile), m.CameraX, m.CameraY);
                        break;
                    //case Keys.Escape:
                    //    mnu.Visible = !mnu.Visible;
                    //    break;
                }

                m.SetCeiling(selectionbox, m.CameraX, m.CameraY);
            });
        }

        private void btn_savemap_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "edit/maps");
            sfd.ShowDialog();
            if (!string.IsNullOrEmpty(sfd.FileName))
            {
                Dictionary<int, string> tileNumberToNameMap = new Dictionary<int, string>();
                string dirname = sfd.FileName;
                Directory.CreateDirectory(dirname);

                BinaryFormatter bf = new BinaryFormatter();

                using (FileStream fs = new FileStream(Path.Combine(dirname, "floor.map"), FileMode.Create))
                {
                    int[,] floormap = new int[m.Width, m.Height];

                    for (int x = 0; x < m.Width; x++)
                    {
                        for (int y = 0; y < m.Height; y++)
                        {
                            floormap[x, y] = m.GetFloor(x, y).TileIndex;
                            if (!tileNumberToNameMap.ContainsKey(floormap[x, y]))
                            {
                                tileNumberToNameMap[floormap[x, y]] = win.GetTileName(floormap[x, y]);
                            }
                        }
                    }

                    bf.Serialize(fs, floormap);
                }

                using (StreamWriter sw = new StreamWriter(Path.Combine(dirname, "floor.tiles")))
                {
                    tileNumberToNameMap.ToList().ForEach(x => sw.WriteLine("{0}:{1}", x.Key, x.Value));
                }

            }
        }

        private void btn_newmap_Click(object sender, EventArgs e)
        {
            MapConfig mc = new MapConfig();
            mc.ShowDialog();
            m = new Map(win, mc.MapWidth, mc.MapHeight);
            m.FillFloor(new FloorTile(0));
            m.SetCeiling(selectionbox, 0, 0);
            s.ChangeMap(m);
        }

        private void btn_loadmap_Click(object sender, EventArgs e)
        {

        }
    }
}
