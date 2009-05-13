using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EmeraldLibrary
{
    public class Layer
    {
        protected Layer()
        {
            draw = null;
        }

        public Layer(Action<Graphics> drawFunc)
        {
            draw = drawFunc;
        }

        protected void SetDrawFunc(Action<Graphics> drawFunc)
        {
            draw = drawFunc;
        }

        internal Action<Graphics> draw;
        public bool Visible;
    }
}
