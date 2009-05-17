using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace EmeraldLibrary
{
    public partial class GameWindow : Form
    {
        public static int TileSize = 32;
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;


        static unsafe class Win32
        {
            const uint SPI_GETKEYBOARDDELAY = 22;
            const uint SPI_SETKEYBOARDDELAY = 23;
            const uint SPI_GETKEYBOARDSPEED = 10;
            const uint SPI_SETKEYBOARDSPEED = 11;
            const uint SPIF_UPDATEINIFILE = 1;
              
            [DllImport("User32.dll")]
            static extern uint SystemParametersInfo(uint uiAction, uint uiParam, uint* pvParam, uint fWinIni);

            public static void SetKeyboardDelay(uint delay)
            {
                // Set the new keyboard delay
                SystemParametersInfo(SPI_SETKEYBOARDDELAY, delay, null, 0);
            }

            public static uint GetKeyboardDelay()
            {
                uint nKBDelay = 0;  // The old keyboard delay.

                SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, &nKBDelay, 0);
                return nKBDelay;
            }

            public static void SetKeyboardSpeed(uint speed)
            {
                // Set the new keyboard delay
                SystemParametersInfo(SPI_SETKEYBOARDSPEED, speed, null, SPIF_UPDATEINIFILE);
            }

            public static uint GetKeyboardSpeed()
            {
                uint spd = 0;  // The old keyboard delay.
                SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, &spd, SPIF_UPDATEINIFILE);
                return spd;
            }
        }

        uint oldKeyboardDelay = 0;
        uint oldKeyboardSpd = 0;

        public GameWindow()
        {
            InitializeComponent();
            ClearAll();
            canvas.Image = b;

            oldKeyboardDelay = Win32.GetKeyboardDelay();
            oldKeyboardSpd = Win32.GetKeyboardSpeed();
            Win32.SetKeyboardDelay(0);
            Win32.SetKeyboardSpeed(255);

            this.FormClosing += new FormClosingEventHandler(GameWindow_FormClosing);
        }

        void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Win32.SetKeyboardDelay(oldKeyboardDelay);
            Win32.SetKeyboardSpeed(oldKeyboardSpd);
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

        private void GameWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

    }





}
