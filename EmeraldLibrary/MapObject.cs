using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmeraldLibrary
{

    public interface MapObject
    {
        int TileIndex { get; }
    }

    public interface MapFloor : MapObject
    {
        bool Passable { get; }
    }
}
