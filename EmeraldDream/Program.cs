using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EmeraldLibrary;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace EmeraldDream
{
    static class Program
    {

        class Overlay : Layer
        {
            public Overlay(int x, int y)
                : base(g => { g.FillRectangle(Brushes.Blue, new Rectangle(x, y, 50, 50)); })
            {
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameWindow g = new GameWindow();

            Story s = new Story(g, "res");

            s.LoadNew();

            //Scene s = new Scene(null);
            //g.Scene = s;

            //NarrationDialog d = new NarrationDialog(50, 400, 700, 200);
            //d.ResetAnimation();

            //s.AddLayer(d);

            //s.KeyDown += new EventHandler<KeyEventArgs>((o, e) =>
            //{
            //    d.Visible = true;
            //    string[] lines = new string[] { "Hello", "I am a narration dialog", "wahaha", "こんにちは","私は今日あなたのゲームのナレータです。ここから私の声だけを聞こえます。","ご協力してくれてありがとうございました" };
            //    d.SetNarration(lines);
            //});

            Application.Run(g);
        }

    }
}
