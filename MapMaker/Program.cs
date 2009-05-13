using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmeraldLibrary;
using System.Windows.Forms;
using EmeraldDream;
using System.Drawing;

namespace MapMaker
{
    class Program
    {


        [STAThread]
        static void Main(string[] args)
        {
            Editor e = new Editor();

            Application.Run(e);
        }
    }
}
