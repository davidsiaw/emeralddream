using System;
using System.Windows.Forms;
namespace EmeraldLibrary
{
    interface IInteractive
    {
        event EventHandler<System.Windows.Forms.KeyEventArgs> KeyDown;
        event EventHandler<System.Windows.Forms.KeyEventArgs> KeyUp;
        event EventHandler<System.Windows.Forms.MouseEventArgs> MouseDown;
        event EventHandler<System.Windows.Forms.MouseEventArgs> MouseMove;
        event EventHandler<System.Windows.Forms.MouseEventArgs> MouseUp;

        void OnMouseMove(object sender, MouseEventArgs e);
        void OnMouseDown(object sender, MouseEventArgs e);
        void OnMouseUp(object sender, MouseEventArgs e);
        void OnKeyDown(object sender, KeyEventArgs e);
        void OnKeyUp(object sender, KeyEventArgs e);

        bool Visible { get; }
    }
}
