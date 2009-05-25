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


        public GameWindow()
        {
            InitializeComponent();
        }



        public Scene Scene
        {
            get
            {
                return canvas.Scene;
            }
            set
            {
                canvas.Scene = value;
            }
        }
    


        public void Print(string words)
        {
            canvas.Print(words);
        }

        public void ClearAll()
        {
            canvas.ClearAll();
        }

        public void Draw(Action<Graphics> drawfunc)
        {
            canvas.Draw(drawfunc);
        }

        internal void Draw(int tilenum, int x, int y)
        {
            canvas.Draw(tilenum, x, y);
        }

        public int LoadTile(string name, string filename)
        {
            return canvas.LoadTile(name, filename);
        }

        public int LoadTile(string name, Image img)
        {
            return canvas.LoadTile(name, img);
        }

        public string GetTileName(int tile)
        {
            return canvas.GetTileName(tile);
        }

        public int GetTileByName(string name)
        {
            return canvas.GetTileByName(name);
        }


        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (canvas.Scene != null)
            {
                canvas.Scene.OnKeyDown(sender, e);
            }
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {

            if (canvas.Scene != null)
            {
                canvas.Scene.OnKeyUp(sender, e);
            }
        }

        internal TileBank Tiles {
            get
            {
                return canvas.tiles;
            }
        }
    }





}
