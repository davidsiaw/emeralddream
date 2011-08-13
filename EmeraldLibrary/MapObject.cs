using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmeraldLibrary
{

    public interface MapObject
    {
        void SetMode(string mode);
        void SetFrameNumber(int num);
        int TileIndex { get; }
    }

    public interface MapFloor : MapObject
    {
        bool Passable { get; }
    }
}
