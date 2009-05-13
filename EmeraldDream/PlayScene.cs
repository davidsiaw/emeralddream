using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmeraldLibrary;

namespace EmeraldDream
{
    public class PlayScene : Scene
    {
        ICharacter chr;

        public ICharacter Player
        {
            get
            {
                return chr;
            }
        }


        public PlayScene(Map map, ICharacter chr)
            : base(map)
        {
            this.chr = chr;
            this.map = map;
        }

        public void ChangeCharacter(ICharacter chr)
        {
            this.chr = chr;
        }

        public void Move(Direction dir)
        {
            map.CheckPassable = true;
            int px = map.CameraX;
            int py = map.CameraY;
            chr.Direction = dir;

            if (map.MoveCharacter(px, py, dir))
            {
                map.MoveCamera(dir);
            }

            // Check if we are colliding with another object and back out if we are
            //if (map.GetCharacter(map.CameraX, map.CameraY) != null)
            //{
            //    map.CameraX = px;
            //    map.CameraY = py;
            //}
            //else
            //{
            //    map.SetCharacter(null, px, py);
            //    map.SetCharacter(chr, map.CameraX, map.CameraY);
            //}
        }

        public void Rotate(Direction dir)
        {
            map.SetCharacter(chr, map.CameraX, map.CameraY);
            chr.Direction = dir;
        }


    }
}
