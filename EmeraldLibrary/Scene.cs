using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmeraldLibrary
{
    public class Scene : IInteractive
    {
        protected Map map;
        List<Layer> layers = new List<Layer>();

        public Scene(Map mp)
        {
            this.map = mp;
            t.Tick += new EventHandler((o, e) =>
            {
                game.Draw(g =>
                {
                    game.ClearAll();
                    if (map != null)
                    {
                        map.DrawMap(g);
                    }

                    layers.ForEach(layer => { if (layer.Visible) layer.draw(g); });
                });


                game.Refresh();
            });
        }

        Timer t = new Timer();
        GameWindow game = null;

        public void Start(GameWindow game)
        {
            this.game = game;
            t.Interval = 1000 / 30;
            t.Start();
        }

        public void Stop()
        {
            t.Stop();
        }

        public void AddLayer(Layer renderlayer)
        {
            layers.Add(renderlayer);
        }

        public void ChangeMap(Map map)
        {
            this.map = map;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            for (int i = layers.Count - 1; i > -1; i--)
            {
                if (layers[i] is IInteractive)
                {
                    IInteractive l = layers[i] as IInteractive;
                    if (l.Visible)
                    {
                        l.OnMouseMove(sender, e);
                        return;
                    }
                }
            }
            if (MouseMove != null)
            {
                MouseMove(sender, e);
            }
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            for (int i = layers.Count - 1; i > -1; i--)
            {
                if (layers[i] is IInteractive)
                {
                    IInteractive l = layers[i] as IInteractive;
                    if (l.Visible)
                    {
                        l.OnMouseDown(sender, e);
                        return;
                    }
                }
            }
            if (MouseDown != null)
            {
                MouseDown(sender, e);
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            for (int i = layers.Count - 1; i > -1; i--)
            {
                if (layers[i] is IInteractive)
                {
                    IInteractive l = layers[i] as IInteractive;
                    if (l.Visible)
                    {
                        l.OnMouseUp(sender, e);
                        return;
                    }
                }
            }
            if (MouseUp != null)
            {
                MouseUp(sender, e);
            }
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            for (int i = layers.Count - 1; i > -1; i--)
            {
                if (layers[i] is IInteractive)
                {
                    IInteractive l = layers[i] as IInteractive;
                    if (l.Visible)
                    {
                        l.OnKeyDown(sender, e);
                        return;
                    }
                }
            }
            if (KeyDown != null)
            {
                KeyDown(sender, e);
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            for (int i = layers.Count - 1; i > -1; i--)
            {
                if (layers[i] is IInteractive)
                {
                    IInteractive l = layers[i] as IInteractive;
                    if (l.Visible)
                    {
                        l.OnKeyUp(sender, e);
                        return;
                    }
                }
            }
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
        public event EventHandler<KeyEventArgs> DXKeyDown;

        #region IInteractive Members


        public bool Visible
        {
            get { return true; }
        }

        public void OnDXKeyDown(object sender, KeyEventArgs e)
        {
            for (int i = layers.Count - 1; i > -1; i--)
            {
                if (layers[i] is IInteractive)
                {
                    IInteractive l = layers[i] as IInteractive;
                    if (l.Visible)
                    {
                        l.OnDXKeyDown(sender, e);
                        return;
                    }
                }
            }
            if (DXKeyDown != null)
            {
                DXKeyDown(sender, e);
            }
        }

        #endregion
    }
}
