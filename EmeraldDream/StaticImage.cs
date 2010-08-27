using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmeraldLibrary;
using System.Drawing;

namespace EmeraldDream
{
    public class StaticImage : Layer
    {
        Dictionary<string, Image> imagelib;
        public StaticImage(Dictionary<string, Image> imagelib)
        {
            this.imagelib = imagelib;
        }

        public string Image
        {
            set
            {
                if (value == null)
                {
                    Hide();
                }
                else
                {
                    SetDrawFunc(g => {
                        Image img = imagelib[value];
                        g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
                    });
                    Show();
                }
            }
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

    }
}
