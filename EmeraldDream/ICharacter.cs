using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmeraldLibrary;
using System.IO;

namespace EmeraldDream
{
    // Describes objects, monsters and people that you can interact with in the game
    public interface ICharacter : MapObject
    {
        Direction Direction { set; get; }

        Action<Story> Use { get; set; }
    }
}
