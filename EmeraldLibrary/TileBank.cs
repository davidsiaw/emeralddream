using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EmeraldLibrary
{
    public class TileBank
    {
        List<Image> tiles = new List<Image>();
        Dictionary<string, int> tileNameToIndexMap = new Dictionary<string, int>();

        public int LoadTile(string name, Image image)
        {
            tiles.Add(image);
            int index = tiles.Count - 1;
            tileNameToIndexMap.Add(name, index);
            return index;
        }

        public int LoadTile(string name, string filename)
        {
            Image b = Bitmap.FromFile(filename);
            return LoadTile(name, b);
        }

        public Image GetTileImage(int i)
        {
            return tiles[i];
        }

        public void DrawTile(Graphics g, int tilenum, int x, int y)
        {
            g.DrawImage(tiles[tilenum], new Rectangle(x, y, GameWindow.TileSize, GameWindow.TileSize));
        }

        public string Tilename(int x)
        {
            return tileNameToIndexMap.ToList().Find(k => k.Value == x).Key;
        }

        public int GetTileByName(string name)
        {
            return tileNameToIndexMap[name];
        }
    }
}
