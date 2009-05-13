using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EmeraldLibrary;

namespace EmeraldDream
{
    public class Dialog : InteractiveLayer
    {
        static Image tl = null;
        static Image tr = null;
        static Image bl = null;
        static Image br = null;
        static Image l = null;
        static Image r = null;
        static Image t = null;
        static Image b = null;
        static Image c = null;

        static Dialog()
        {
            tl = Bitmap.FromFile("edit/res/window/tl.png");
            tr = Bitmap.FromFile("edit/res/window/tr.png");
            bl = Bitmap.FromFile("edit/res/window/bl.png");
            br = Bitmap.FromFile("edit/res/window/br.png");
            l = Bitmap.FromFile("edit/res/window/l.png");
            r = Bitmap.FromFile("edit/res/window/r.png");
            t = Bitmap.FromFile("edit/res/window/t.png");
            b = Bitmap.FromFile("edit/res/window/b.png");
            c = Bitmap.FromFile("edit/res/window/c.png");
        }

        int x = 0;
        int y = 400;
        int w = GameWindow.ScreenWidth;
        int h = 200;

        int h0;
        int w0;
        int x0;
        int y0;

        public Rectangle InsideArea
        {
            get
            {
                return new Rectangle(x + tl.Width, y + tl.Height, w - tl.Width - tr.Width, h - tl.Height - tr.Height);
            }
        }

        public Rectangle Area
        {
            get
            {
                return new Rectangle(x0, y0, w0, h0);
            }
        }

        public void SetDialogDrawingFunc(Action<Graphics> func)
        {
            drawDialogContent = func;
        }

        Action<Graphics> drawDialogContent = null;

        public Dialog(int x, int y, int w, int h)
        {
            this.x = x0 = x;
            this.y = y0 = y;
            this.w = w0 = w;
            this.h = h0 = h;
            SetUpDrawMethod();
        }

        private void SetUpDrawMethod()
        {
            this.SetDrawFunc(g =>
            {

                g.DrawImage(tl, new Rectangle(x, y, tl.Width, tl.Height));
                g.DrawImage(tr, new Rectangle(x + w - tr.Width, y, tr.Width, tr.Height));
                g.DrawImage(bl, new Rectangle(x, y + h - bl.Height, bl.Width, bl.Height));
                g.DrawImage(br, new Rectangle(x + w - br.Width, y + h - br.Height, br.Width, br.Height));

                g.DrawImage(t, new Rectangle(x + tl.Width, y, w - tr.Width - tl.Width, tl.Height));
                g.DrawImage(l, new Rectangle(x, y + tl.Height, tl.Width, h - bl.Height - tl.Height));
                g.DrawImage(r, new Rectangle(x + w - tr.Width, y + tl.Height, tl.Width, h - bl.Height - tl.Height));
                g.DrawImage(b, new Rectangle(x + tl.Width, y + h - b.Height, w - tr.Width - tl.Width, tl.Height));

                g.FillRectangle(new TextureBrush(c), new Rectangle(x + tl.Width, y + tl.Height, w - tl.Width - tr.Width, h - tl.Height - tr.Height));

                if (h == h0 && w == w0 && x == x0 && y == y0)
                {
                    if (drawDialogContent != null)
                    {
                        drawDialogContent(g);
                    }
                }

                if (h < h0)
                {
                    h += h0 / 5;
                    if (h > h0)
                    {
                        h = h0;
                    }
                }

                if (w < w0)
                {
                    w += w0 / 5;
                    if (w > w0)
                    {
                        w = w0;
                    }
                }

                if (x > x0)
                {
                    x -= w0 / 10;
                    if (x < x0)
                    {
                        x = x0;
                    }
                }

                if (y > y0)
                {
                    y -= h0 / 10;
                    if (y < y0)
                    {
                        y = y0;
                    }
                }
            });
        }

        private void ResetAnimation()
        {
            h = 65;
            w = 65;
            x = x0 + w0 / 2 - 32;
            y = y0 + h0 / 2 - 32;
        }

        Action doOnceOnClose = null;

        private new bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        public void SetDoOnceOnClose(Action a)
        {
            doOnceOnClose = a;
        }

        public void Open()
        {
            ResetAnimation();
            Visible = true;
        }

        public void Close()
        {
            Visible = false;
            if (doOnceOnClose != null)
            {
                doOnceOnClose();
                doOnceOnClose = null;
            }
        }
    }
}
