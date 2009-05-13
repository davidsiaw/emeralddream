using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EmeraldDream
{
    public class NarrationDialog : Dialog
    {
        static Font font = new Font("Lucida Console", 20);


        Image downicon = Bitmap.FromFile("edit/res/window/next.png");

        public NarrationDialog(int x, int y, int w, int h)
            : base(x, y, w, h)
        {

            this.SetDialogDrawingFunc(g => {

                // Draw the letters one by one
                if (!complete)
                {
                    if (lines.Count == 0 || lines[lines.Count - 1] == line)
                    {
                        narrating = false;
                        complete = true;
                    }
                    else
                    {
                        if (lines[currentline] != line)
                        {
                            narrating = true;
                            line = lines[currentline].Substring(0, line.Length + 1);
                        }
                        else
                        {
                            narrating = false;
                            // Draw the arrow
                            DrawArrow(g);
                        }
                    }
                }
                g.DrawString(line, font, new SolidBrush(Color.White), InsideArea);

            });

            this.KeyDown += new EventHandler<System.Windows.Forms.KeyEventArgs>((o, e) => {
                if (!narrating)
                {
                    if (complete || currentline + 1 >= lines.Count)
                    {
                        Close();
                    }
                    else
                    {
                        currentline++;
                        line = "";
                    }
                }

            });
        }


        bool narrating = false;

        int animcount = 0;
        private void DrawArrow(Graphics g)
        {
            Rectangle r = Area;
            r.X = r.X + r.Width - 40;
            r.Y = r.Y + r.Height - 40;
            r.Height = 20;
            r.Width = 20;

            if (animcount > 10 && animcount < 15)
            {
                r.Y += 2;
            }

            g.DrawImage(downicon, r);

            animcount = (animcount + 1) % 20;
        }

        int currentline = 0;
        string line = "";
        List<string> lines = new List<string>();
        bool complete = true;

        public void SetNarration(IEnumerable<string> lines)
        {
            narrating = true;
            this.lines = new List<string>(lines);
            complete = false;
            currentline = 0;
            line = "";
        }


    }
}
