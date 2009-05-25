using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;
using System.Windows.Forms;

namespace EmeraldLibrary
{
    class DXKeyboard
    {
        Timer t = new Timer();
        Device device = new Device(SystemGuid.Keyboard);

        public DXKeyboard(Control ctl)
        {
            device.SetCooperativeLevel(ctl, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
            device.SetDataFormat(DeviceDataFormat.Keyboard);
            try
            {
                device.Acquire();
            }
            catch
            {
            }
            t.Interval = 1000 / 60;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            Poll();
        }

        void Poll()
        {
            KeyboardState state;
            try
            {
                device.Poll();
                state = device.GetCurrentKeyboardState();
                keyDown(state);
            }
            catch
            {
                device.Acquire();
            }
        }

        public Action<KeyboardState> keyDown = null;

    }
}
