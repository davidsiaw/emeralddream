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
            Image tile = tiles[tilenum];
            int tilex = x - (tile.Width - GameWindow.TileSize) / 2;
            int tiley = y - tile.Height + GameWindow.TileSize;

            g.DrawImage(tiles[tilenum], new Rectangle(tilex, tiley, tile.Width, tile.Height));
        }

        public string Tilename(int x)
        {
            return tileNameToIndexMap.ToList().Find(k => k.Value == x).Key;
        }

        public int GetTileByName(string name)
        {
            return tileNameToIndexMap[name];
        }

        string GetTilePartName(string name, Rectangle rect)
        {
            return string.Join("?", new string[] { 
                name, 
                rect.X.ToString(),
                rect.Y.ToString(),
                rect.Width.ToString(),
                rect.Height.ToString(),
            
            });
        }

        public int GetTileByName(string name, Rectangle rect)
        {
            string tilename = GetTilePartName(name, rect);
            if (!tileNameToIndexMap.ContainsKey(tilename))
            {
                Image orig = tiles[tileNameToIndexMap[name]];

                // Cut out the portion we want and save it
                Bitmap b = new Bitmap(orig, rect.Size);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    g.DrawImage(orig, new Rectangle(Point.Empty, rect.Size), rect, GraphicsUnit.Pixel);
                }
                tiles.Add(b);
                tileNameToIndexMap[tilename] = tiles.Count - 1;
            }
            return tileNameToIndexMap[tilename];
        }


    }
}
