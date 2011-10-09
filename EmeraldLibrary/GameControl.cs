using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;

namespace EmeraldLibrary
{
    public partial class GameControl : PictureBox
    {
        public static int TileSize = 128;
        //public int ScreenWidth = 800;
        //public int ScreenHeight = 600;

        DXKeyboard kb;

        public GameControl()
        {
            InitializeComponent();
            ClearAll();
            Image = new Bitmap(Width, Height);

            

            this.Resize += new EventHandler(GameControl_Resize);
            this.LoadCompleted += new AsyncCompletedEventHandler(GameControl_LoadCompleted);

        }

        void GameControl_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            kb = new DXKeyboard(Form.ActiveForm);
            kb.keyDown = state =>
            {
                KeyEventArgs kea;
                Keys k = Keys.None;
                if (state[Key.Up])
                {
                    k |= Keys.Up;
                }
                if (state[Key.Down])
                {
                    k |= Keys.Down;
                }
                if (state[Key.Left])
                {
                    k |= Keys.Left;
                }
                if (state[Key.Right])
                {
                    k |= Keys.Right;
                }
                if (state[Key.Return])
                {
                    k |= Keys.Return;
                }
                for (char i = 'A'; i <= 'Z'; i++)
                {
                    if (state[(Key)Enum.Parse(typeof(Key), i.ToString())])
                    {
                        k |= (Keys)Enum.Parse(typeof(Keys), i.ToString());
                    }
                }
                kea = new KeyEventArgs(k);
                Game_DXKeyDown(this, kea);
            };
        }

        public GameControl(TileBank tb) : this()
        {
            this.tiles = tb;
        }

        void GameControl_Resize(object sender, EventArgs e)
        {
            Image = new Bitmap(Width, Height);
        }

        public TileBank TileBank
        {
            set
            {
                tiles = value;
            }
        }

        internal TileBank tiles = new TileBank();
        Scene m_scene = null;
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
                if (value != null)
                {
                    m_scene = value;

                    m_scene.Start(this);
                }
            }
        }
   


        private void Game_DXKeyDown(object sender, KeyEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnDXKeyDown(sender, e);
            }

        }

        private void GameControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnMouseDown(sender, e);
            }
        }

        private void GameControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnMouseUp(sender, e);
            }
        }

        private void GameControl_MouseMove(object sender, MouseEventArgs e)
        {

            if (m_scene != null)
            {
                m_scene.OnMouseMove(sender, e);
            }
        }


        public void Print(string words)
        {
            Draw(g => { g.DrawString(words, deffont, new SolidBrush(Color.White), new PointF(0, 0)); });
        }

        public void ClearAll()
        {
            Draw(g => { g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, Width, Height)); });
        }

        const int slowness=8;
        int frame = 0;
        public int Frame { get; private set; }

        public void Draw(Action<Graphics> drawfunc)
        {
            frame++;
            if (frame == slowness * 4)
            {
                frame = 0;
            }
            Frame = frame / slowness;

            if (Image != null)
            {
                using (Graphics g = Graphics.FromImage(Image))
                {
                    drawfunc(g);
                }
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

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {

            if (m_scene != null)
            {
                m_scene.OnKeyDown(sender, e);
            }
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {

            if (m_scene != null)
            {
                m_scene.OnKeyUp(sender, e);
            }
        }

    }
}
