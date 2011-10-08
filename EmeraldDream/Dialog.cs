using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EmeraldLibrary;
using System.Windows.Forms;

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
            tl = Bitmap.FromFile("res/window/tl.png");
            tr = Bitmap.FromFile("res/window/tr.png");
            bl = Bitmap.FromFile("res/window/bl.png");
            br = Bitmap.FromFile("res/window/br.png");
            l = Bitmap.FromFile("res/window/l.png");
            r = Bitmap.FromFile("res/window/r.png");
            t = Bitmap.FromFile("res/window/t.png");
            b = Bitmap.FromFile("res/window/b.png");
            c = Bitmap.FromFile("res/window/c.png");
        }

        int x = 0;
        int y = 400;
        int w = 800;
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
                //doOnceOnClose = null;
            }
        }

    }

    public class MenuDialog : Dialog
    {
        static Font font = new Font("Lucida Console", 20);

        public MenuDialog(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            this.SetDialogDrawingFunc(g => {

                if (selectedindex != -1)
                {
                    int curr_y = 0;

                    if (!string.IsNullOrEmpty(question))
                    {
                        SizeF remainingsize = InsideArea.Size;
                        remainingsize.Height -= curr_y;
                        SizeF sz = g.MeasureString(question, font, remainingsize);
                        g.DrawString(question, font, Brushes.White, new Point(InsideArea.Left, InsideArea.Top + curr_y));
                        curr_y += (int)sz.Height;
                    }

                    menuitemlist.ForEach(s => {
                        SizeF remainingsize = InsideArea.Size;
                        remainingsize.Height -= curr_y;

                        SizeF sz = g.MeasureString(s, font, remainingsize);

                        // Color differently if selected
                        if (menuitemlist[selectedindex] == s)
                        {
                            g.FillRectangle(Brushes.White, new Rectangle(InsideArea.Left, InsideArea.Top + curr_y, InsideArea.Width, (int)sz.Height));
                            g.DrawString(s, font, Brushes.Black, new Point(InsideArea.Left, InsideArea.Top + curr_y));
                        }
                        else
                        {
                            g.DrawString(s, font, Brushes.White, new Point(InsideArea.Left, InsideArea.Top + curr_y));
                        }

                        curr_y += (int)sz.Height;
                        
                    });

                }
            });

            this.KeyDown += new EventHandler<KeyEventArgs>((o, e) =>
            {
                if (selectedindex != -1)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Up:
                            selectedindex = (selectedindex - 1) % menuitems.Count;
                            if (selectedindex < 0)
                            {
                                selectedindex = menuitems.Count - 1;
                            }
                            break;
                        case Keys.Down:
                            selectedindex = (selectedindex + 1) % menuitems.Count;
                            break;
                        case Keys.Enter:
                            Close();
                            break;
                    }
                }
            });

        }


        Dictionary<string, string> menuitems = new Dictionary<string, string>();
        List<string> menuitemlist = new List<string>();

        string question = null;

        public void SetQuestion(string q)
        {
            this.question = q;
        }

        public void AddMenuItem(string name, string text)
        {
            selectedindex = 0;
            menuitemlist.Add(text);
            menuitems[text] = name;
        }

        public void ClearMenuItems()
        {
            selectedindex = -1;
            menuitemlist.Clear();
            menuitems.Clear();
        }

        int selectedindex = -1;

        public string SelectedItem
        {
            get
            {
                return menuitems[menuitemlist[selectedindex]];
            }
        }
    }
}
