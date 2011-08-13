using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace EmeraldLibrary
{
    public class Map
    {
        public GameControl GameControl;
        MapFloor[,] floorLevel;
        MapObject[,] characterLevel;
        MapObject[,] ceilingLevel;
        int x, y;
        int w, h;

        public bool CheckPassable = false;

        int camx, camy;

        public int Width
        {
            get
            {
                return w;
            }
        }

        public int Height
        {
            get
            {
                return h;
            }
        }

        public int CameraX
        {
            get
            {
                return camx;
            }
            set
            {
                if (subx == 0 && suby == 0)
                {

                    if (value >= w)
                    {
                        value = w - 1;
                    }
                    if (value < 0)
                    {
                        value = 0;
                    }

                    if (CheckPassable && FloorIsPassable(value, camy))
                    {
                        return;
                    }

                    camx = value;

                    x = camx - (GameControl.Width / 2 / 32);

                    if (Submovementdirection == Direction.Right)
                    {
                        if (x - 1 < 0)
                        { x = -1; }
                    }
                    
                    if (x < 0)
                    {
                        x = 0;
                        Submovementdirection = null;
                    }
                    else if (x + GameControl.Width / 32 >= w)
                    {
                        if (x + GameControl.Width / 32 == w && Submovementdirection == Direction.Left)
                        {
                            Submovementdirection = null;
                        }
                        if (x + GameControl.Width / 32 > w)
                        {
                            Submovementdirection = null;
                        }
                        x = w - GameControl.Width / 32;
                    }
                }
            }
        }

        public int CameraY
        {
            get
            {
                return camy;
            }
            set
            {
                if (subx == 0 && suby == 0)
                {

                    if (value >= h)
                    {
                        value = h - 1;
                    }
                    if (value < 0)
                    {
                        value = 0;
                    }

                    if (CheckPassable && FloorIsPassable(camx, value))
                    {
                        return;
                    }

                    camy = value;

                    y = camy - (GameControl.Height / 2 / 32);

                    if (Submovementdirection == Direction.Down)
                    {
                        if (y - 1 < 0)
                        { y = -1; }
                    }
                    if (y < 0)
                    {
                        y = 0;
                        Submovementdirection = null;
                    }
                    else if (y + GameControl.Height / 32 >= h)
                    {
                        if (y + GameControl.Height / 32 == h && Submovementdirection == Direction.Up)
                        {
                            Submovementdirection = null;
                        }
                        if (y + GameControl.Height / 32 > h)
                        {
                            Submovementdirection = null;
                        }
                        y = h - GameControl.Height / 32;
                    }
                }
            }
        }

    

        public Map(GameControl game, int w, int h)
        {
            this.GameControl = game;
            floorLevel = new MapFloor[w, h];
            characterLevel = new MapObject[w, h];
            ceilingLevel = new MapObject[w, h];
            x = 0;
            y = 0;
            this.w = w;
            this.h = h;
            CameraX = 0;
            CameraY = 0;
        }

        public MapFloor GetFloor(int x, int y)
        {
            return floorLevel[x, y];
        }

        public MapObject GetCharacter(int x, int y)
        {
            return characterLevel[x, y];
        }

        public MapObject GetCeiling(int x, int y)
        {
            return ceilingLevel[x, y];
        }

        public void SetFloor(MapFloor floor, int x, int y)
        {
            floorLevel[x, y] = floor;
        }

        Dictionary<MapObject, int> objectToSubx = new Dictionary<MapObject, int>();
        Dictionary<MapObject, int> objectToSuby = new Dictionary<MapObject, int>();
        Dictionary<MapObject, Direction> objectToDir = new Dictionary<MapObject, Direction>();

        bool IsOutOfBounds(int x, int y)
        {
            if (x < 0 || x >= w || y < 0 || y >= h)
            {
                return true;
            }
            return false;
        }

        bool IsPassable(int fromx, int fromy, Direction dir)
        {
            int tox = fromx;
            int toy = fromy;
            switch (dir)
            {
                case Direction.Up:
                    toy = fromy - 1;
                    break;
                case Direction.Down:
                    toy = fromy + 1;
                    break;
                case Direction.Left:
                    tox = fromx - 1;
                    break;
                case Direction.Right:
                    tox = fromx + 1;
                    break;
            }

            if (IsOutOfBounds(tox,toy) || characterLevel[tox, toy] != null ||
                FloorIsPassable(tox, toy))
            {
                return false;
            }
            return true;
        }

        private bool FloorIsPassable(int tox, int toy)
        {
            return (floorLevel[tox, toy] != null && !floorLevel[tox, toy].Passable);
        }

        public bool MoveCharacter(int fromx, int fromy, Direction dir)
        {
            MapObject obj = characterLevel[fromx, fromy];
            if (obj == null) { return false; }

            switch (dir)
            {
                case Direction.Up:
                    if (!IsPassable(fromx, fromy, dir))
                    {
                        return false;
                    }
                    break;
                case Direction.Down:
                    if (!IsPassable(fromx, fromy, dir))
                    {
                        return false;
                    }
                    break;
                case Direction.Left:
                    if (!IsPassable(fromx, fromy, dir))
                    {
                        return false;
                    }
                    break;
                case Direction.Right:
                    if (!IsPassable(fromx, fromy, dir))
                    {
                        return false;
                    }
                    break;
            }

            if (!objectToDir.ContainsKey(obj))
            {
                objectToDir[obj] = dir;
                objectToSubx[obj] = 0;
                objectToSuby[obj] = 0;
                return true;
            }
            return false;
        }

        public void SetCharacter(MapObject obj, int x, int y)
        {
            characterLevel[x, y] = obj;
        }

        public void SetCeiling(MapObject obj, int x, int y)
        {
            ceilingLevel[x, y] = obj;
        }

        public void FillFloor(MapFloor floor)
        {
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    SetFloor(floor, x, y);
                }
            }
        }

        Direction? Submovementdirection
        {
            get
            {
                return m_submovement;
            }
            set
            {
                m_submovement = value;
            }
        }

        Direction? m_submovement = null;

        public void MoveCamera(Direction dir)
        {
            //Debug.WriteLine(" submovementdirection=" + Submovementdirection);
            if (Submovementdirection == null)
            {
                Submovementdirection = dir;
                switch (dir)
                {
                    case Direction.Up:
                        CameraY--;
                        if (Submovementdirection != null)
                        {
                            suby += GameWindow.TileSize;
                        }
                        break;
                    case Direction.Down:
                        CameraY++;
                        if (Submovementdirection != null)
                        {
                            suby -= GameWindow.TileSize;
                        }
                        break;
                    case Direction.Left:
                        CameraX--;
                        if (Submovementdirection != null)
                        {
                            subx += GameWindow.TileSize;
                        }
                        break;
                    case Direction.Right:
                        CameraX++;
                        if (Submovementdirection != null)
                        {
                            subx -= GameWindow.TileSize;
                            //Debug.WriteLine(" subx=" + subx);
                        }
                        break;
                }
            }
        }

        public void ScreenToMapCoords(int x, int y, out int mapx, out int mapy)
        {
            int viewportx;
            int viewporty;
            int xLeftMost;
            int yTopMost;
            int xRightMost;
            int yBottomMost;
            AnalyseMap(out viewportx, out viewporty, out xLeftMost, out yTopMost, out xRightMost, out yBottomMost);

            // Adjust the coordinates we are looking at based on the viewport coords
            x = x - viewportx;
            y = y - viewporty;
            if (x < 0 || x > Width * GameWindow.TileSize || y < 0 || y > Height * GameWindow.TileSize)
            {
                mapx = -1;
                mapy = -1;
                return;
            }

            mapx = x / GameWindow.TileSize + xLeftMost;
            mapy = y / GameWindow.TileSize + yTopMost;
        }

        int subx = 0;

        public void MapToScreenCoords(int mapx, int mapy, out int x, out int y)
        {
            int viewportx;
            int viewporty;
            int xLeftMost;
            int yTopMost;
            int xRightMost;
            int yBottomMost;
            AnalyseMap(out viewportx, out viewporty, out xLeftMost, out yTopMost, out xRightMost, out yBottomMost);

            mapx -= xLeftMost;
            mapy -= yTopMost;

            x = mapx * GameWindow.TileSize + viewportx;
            y = mapy * GameWindow.TileSize + viewporty;
        }

        int suby = 0;

        const int moveamount = 8;
        public void DrawMap(Graphics g)
        {
            int viewportx;
            int viewporty;
            int xLeftMost;
            int yTopMost;
            int xRightMost;
            int yBottomMost;
            AnalyseMap(out viewportx, out viewporty, out xLeftMost, out yTopMost, out xRightMost, out yBottomMost);

            if (xLeftMost > 0)
            {
                xLeftMost -= 1;
                viewportx -= GameWindow.TileSize;
            }
            if (yTopMost > 0)
            {
                yTopMost -= 1;
                viewporty -= GameWindow.TileSize;
            }
            
            // Draw the tiles!
            
            int xcount = 0;
            for (int xx = xLeftMost; xx < xRightMost && xx < w; xx++)
            {
                int ycount = 0;
                for (int yy = yTopMost; yy < yBottomMost && yy < h; yy++)
                {
                    if (floorLevel[xx, yy] != null)
                    {
                        GameControl.tiles.DrawTile(g, floorLevel[xx, yy].TileIndex, xcount * GameWindow.TileSize + viewportx, ycount * GameWindow.TileSize + viewporty);
                    }
                    ycount++;
                }
                xcount++;
            }

            xcount = 0;
            for (int xx = xLeftMost; xx < xRightMost && xx < w; xx++)
            {
                int ycount = 0;
                for (int yy = yTopMost; yy < yBottomMost && yy < h; yy++)
                {
                    if (characterLevel[xx, yy] != null)
                    {
                        MapObject obj = characterLevel[xx, yy];
                        if (objectToDir.ContainsKey(obj))
                        {
                            switch (objectToDir[obj])
                            {
                                case Direction.Up:
                                    objectToSuby[obj] -= moveamount;
                                    break;
                                case Direction.Down:
                                    objectToSuby[obj] += moveamount;
                                    break;
                                case Direction.Left:
                                    objectToSubx[obj] -= moveamount;
                                    break;
                                case Direction.Right:
                                    objectToSubx[obj] += moveamount;
                                    break;
                            }
                            obj.SetMode("walk");
                            obj.SetFrameNumber(GameControl.Frame);
                            GameControl.tiles.DrawTile(g, obj.TileIndex, xcount * GameWindow.TileSize + viewportx + objectToSubx[obj], ycount * GameWindow.TileSize + viewporty + objectToSuby[obj]);

                            if (Math.Abs(objectToSuby[obj]) >= GameWindow.TileSize || Math.Abs(objectToSubx[obj]) >= GameWindow.TileSize)
                            {
                                characterLevel[xx, yy] = null;
                                characterLevel[xx + objectToSubx[obj] / GameWindow.TileSize, yy + objectToSuby[obj] / GameWindow.TileSize] = obj;
                                objectToDir.Remove(obj);
                                objectToSubx.Remove(obj);
                                objectToSuby.Remove(obj);
                            }
                        }
                        else
                        {
                            obj.SetMode("stand");
                            obj.SetFrameNumber(GameControl.Frame);
                            GameControl.tiles.DrawTile(g, obj.TileIndex, xcount * GameWindow.TileSize + viewportx, ycount * GameWindow.TileSize + viewporty);
                        }
                    }
                    ycount++;
                }
                xcount++;
            }
        
            xcount = 0;
            for (int xx = xLeftMost; xx < xRightMost && xx < w; xx++)
            {
                int ycount = 0;
                for (int yy = yTopMost; yy < yBottomMost && yy < h; yy++)
                {
                    if (ceilingLevel[xx, yy] != null)
                    {
                        GameControl.tiles.DrawTile(g, ceilingLevel[xx, yy].TileIndex, xcount * GameWindow.TileSize + viewportx, ycount * GameWindow.TileSize + viewporty);
                    }
                    ycount++;
                }
                xcount++;
            }
        }

        private void AnalyseMap(out int viewportx, out int viewporty, out int xLeftMost, out int yTopMost, out int xRightMost, out int yBottomMost)
        {

            viewportx = 0;
            viewporty = 0;

            xLeftMost = 0;
            yTopMost = 0;

            xRightMost = w;
            yBottomMost = h;

            // If the size of the map is equal or less than the screen size, we center it on the screen
            if (w * GameWindow.TileSize <= GameControl.Width)
            {
                viewportx = (GameControl.Width / 2) - (w * GameWindow.TileSize / 2);
                x = 0;
            }
            else
            {
                xLeftMost = x;
                xRightMost = x + GameControl.Width / 32 + 1;
            }

            if (h * GameWindow.TileSize <= GameControl.Height)
            {
                viewporty = (GameControl.Height / 2) - (h * GameWindow.TileSize / 2);
                y = 0;
            }
            else
            {
                yTopMost = y;
                yBottomMost = y + GameControl.Height / 32 + 2;
            }

            if (Submovementdirection != null)
            {
                switch (Submovementdirection)
                {
                    case Direction.Up:
                        suby -= moveamount;
                        break;
                    case Direction.Down:
                        suby += moveamount;
                        break;
                    case Direction.Left:
                        subx -= moveamount;
                        break;
                    case Direction.Right:
                        subx += moveamount;
                        break;
                }
                if (subx == 0 && suby == 0)
                {
                    Submovementdirection = null;
                }
            }

            // subtile scrolling
            viewportx -= subx;
            viewporty -= suby;
        }
    }
}
