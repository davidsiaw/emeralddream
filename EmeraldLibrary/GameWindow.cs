using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.DirectX.DirectInput;

namespace EmeraldLibrary
{
    public partial class GameWindow : Form
    {
        public static int TileSize = 32;
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;



        class Keyboard
        {
            Timer t = new Timer();
            Device device = new Device(SystemGuid.Keyboard);

            public Keyboard(Control ctl)
            {
                device.SetCooperativeLevel(ctl, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                device.SetDataFormat(DeviceDataFormat.Keyboard);
                try
                {
                    device.Acquire();
                }
                catch
                {   
                }
                t.Interval = 1000 / 60;
                t.Tick += new EventHandler(t_Tick);
                t.Start();
            }

            void t_Tick(object sender, EventArgs e)
            {
                Poll();
            }

            void Poll()
            {
                KeyboardState state;
                try
                {
                    device.Poll();
                    state = device.GetCurrentKeyboardState();
                    keyDown(state);
                }
                catch
                {
                    device.Acquire();
                }
            }

            public Action<KeyboardState> keyDown = null;

        }

        Keyboard kb;

        public GameWindow()
        {
            InitializeComponent();
            ClearAll();
            canvas.Image = b;

            kb = new Keyboard(this);
            kb.keyDown = state => {
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
                kea = new KeyEventArgs(k);
                Game_DXKeyDown(this, kea);
            };

            this.FormClosing += new FormClosingEventHandler(GameWindow_FormClosing);
        }

        void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
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


        private void Game_DXKeyDown(object sender, KeyEventArgs e)
        {
            if (m_scene != null)
            {
                m_scene.OnDXKeyDown(sender, e);
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

        private void GameWindow_KeyPress(object sender, KeyPressEventArgs e)
        {

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
