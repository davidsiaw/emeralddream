using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace EmeraldDream
{
    class LuaDll
    {
        delegate IntPtr lua_Alloc(IntPtr ud, IntPtr ptr, int osize, int nsize);

        [DllImport("lua.dll")]
        static extern lua_Alloc getDefaultAllocator();

        [DllImport("lua.dll")]
        static extern IntPtr lua_newstate(lua_Alloc f, IntPtr ud); 
    }
}
