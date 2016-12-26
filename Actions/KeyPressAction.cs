using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MaterialIcons;
using Rou.Utils;
using System.Windows.Forms;

namespace Rou.Actions
{
    public abstract class KeyPressAction : Action
    {
        private System.Timers.Timer timer;

        public int Delay { get; private set; }
        public int Interval { get; private set; }
        public IEnumerable<Keys> Key { get; protected set; }

        public KeyPressAction(string text, MaterialIconType type, IEnumerable<Keys> keys, int interval = 10, int delay = 10) : base(text, type)
        {
            Delay = delay;
            Interval = Interval;
            Key = keys;
            timer = new System.Timers.Timer();
            timer.Interval = Delay; //In milliseconds here
            timer.AutoReset = false; //Stops it from repeating
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        public KeyPressAction(string text, MaterialIconType type, Keys key, int delay = 10) : this(text, type, new List<Keys>() { key }, 10, delay)
        {}

        public override bool Click()
        {
            timer.Start();
            return true;
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            foreach (var key in Key)
            {
                KeyboardSimulator.KeyPress(key);
                System.Threading.Thread.Sleep(Interval);
            }
            //MessageBox.Show("Pressed " + this.Text);

        }
    }
}
