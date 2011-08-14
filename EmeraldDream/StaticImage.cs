using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmeraldLibrary;
using System.Drawing;
using System.Drawing.Imaging;

namespace EmeraldDream
{
    public enum Transition
    {
        FadeIn,
        Instant,
    }

    public class StaticImage : Layer
    {
        Dictionary<string, Image> imagelib;
        public StaticImage(Dictionary<string, Image> imagelib)
        {
            this.imagelib = imagelib;
        }


        public Transition Transition
        {
            get;set;
        }

        string lastimage;
        public string Image
        {
            set
            {
                if (value == null)
                {
                    if (Transition == EmeraldDream.Transition.FadeIn)
                    {
                        float t = 1.0f; // opacity
                        float dt = -0.1f;
                        t = ApplyImage(lastimage, t, dt);
                    }
                    else
                    {
                        Hide();
                    }
                }
                else
                {
                    float t = 0.0f; // opacity
                    float dt = 0.1f;
                    switch (Transition)
                    {
                        case Transition.FadeIn:
                            t = 0;
                            break;
                        case Transition.Instant:
                            t = 1;
                            break;
                    }

                    t = ApplyImage(value, t, dt);
                    lastimage = value;
                    Show();

                }
            }
        }

        private float ApplyImage(string value, float t, float dt)
        {
            SetDrawFunc(g =>
            {

                ImageAttributes imgattr = new ImageAttributes();

                ColorMatrix cm = new ColorMatrix(
                    new float[][]{
                                new float[]{1, 0, 0, 0, 0},
                                new float[]{0, 1, 0, 0, 0},
                                new float[]{0, 0, 1, 0, 0},
                                new float[]{0, 0, 0, t, 0},
                                new float[]{0, 0, 0, 0, 1},
                            }
                    );

                imgattr.SetColorMatrix(cm,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);

                Image img = imagelib[value];
                g.DrawImage(img,
                    new Rectangle(0, 0, img.Width, img.Height),
                    0, 0, img.Width, img.Height,
                    GraphicsUnit.Pixel,
                    imgattr
                    );

                if ((dt > 0 && t < 1) || (dt < 0 && t > 0))
                {
                    t += dt;
                }
                if (dt < 0 && t == 0)
                {
                    Hide();
                }
            });
            return t;
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
