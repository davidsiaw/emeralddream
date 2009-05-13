using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace EmeraldLibrary
{
    public class InteractiveLayer : Layer, IInteractive
    {

        protected InteractiveLayer()
            : base()
        {
        }

        public InteractiveLayer(Action<Graphics> drawFunc)
            : base(drawFunc)
        {
            
        }


        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (MouseMove != null)
            {
                MouseMove(sender, e);
            }
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (MouseDown != null)
            {
                MouseDown(sender, e);
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (MouseUp != null)
            {
                MouseUp(sender, e);
            }
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyDown != null)
            {
                KeyDown(sender, e);
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (KeyUp != null)
            {
                KeyUp(sender, e);
            }
        }

        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;

        #region IInteractive Members

        public new bool Visible
        {
            get {
                return base.Visible;
            }
            set {
                base.Visible = value;
            }
        }

        #endregion
    }
}
