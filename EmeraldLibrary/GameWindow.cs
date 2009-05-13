using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmeraldLibrary
{
    public partial class GameWindow : Form
    {
        public static int TileSize = 32;
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;

        public GameWindow()
        {
            InitializeComponent();
            ClearAll();
            canvas.Image = b;
        }

        internal TileBank tiles = new TileBank();
        Scene m_scene = null;
        Bitmap b = new Bitmap(ScreenWidth, ScreenHeight);
        Font deffont = new Font(FontFamily.GenericMonospace, 10);

        public Scene Scene
        {
            get
            {
                return m_scene;
            }
            set
            {
                if (m_scene != null)
                {
                    m_scene.Stop();
                }

                m_scene = value;

                m_scene.Start(this);
            }
        }
    

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnMouseDown(sender, e);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnMouseMove(sender, e);
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnMouseUp(sender, e);
            }
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnKeyDown(sender, e);
            }
        }

        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnKeyUp(sender, e);
            }
        }

        public void Print(string words)
        {
            Draw(g => { g.DrawString(words, deffont, new SolidBrush(Color.White), new PointF(0, 0)); });
        }

        public void ClearAll()
        {
            Draw(g => { g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, ScreenWidth, ScreenHeight)); });
        }

        public void Draw(Action<Graphics> drawfunc)
        {
            using (Graphics g = Graphics.FromImage(b))
            {
                drawfunc(g);
            }
        }

        internal void Draw(int tilenum, int x, int y)
        {
            Draw(g => tiles.DrawTile(g, tilenum, x, y));
        }

        public int LoadTile(string name, string filename)
        {
            return tiles.LoadTile(name, filename);
        }

        public int LoadTile(string name, Image img)
        {
            return tiles.LoadTile(name, img);
        }

        public string GetTileName(int tile)
        {
            return tiles.Tilename(tile);
        }

        public int GetTileByName(string name)
        {
            return tiles.GetTileByName(name);
        }
    }





}
