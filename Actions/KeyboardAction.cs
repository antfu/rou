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
    public struct KeyAction
    {
        public Keys Key { get; private set; }
        public KeyOperation Operation { get; private set; }
        public int Delay { get; private set; }

        public KeyAction(Keys key, KeyOperation operation = KeyOperation.Press, int delay = 10)
        {
            Key = key;
            Operation = operation;
            Delay = delay;
        }
    }
    public enum KeyOperation
    {
        Press,
        Down,
        Up
    }

    public class KeyboardAction : Action
    {
        private System.Timers.Timer timer;

        public int Delay { get; private set; }
        public IEnumerable<KeyAction> Key { get; protected set; }

        public KeyboardAction(string text, MaterialIconType type, IEnumerable<KeyAction> keys, int delay = 10) : base(text, type)
        {
            Delay = delay;
            Key = keys;
            timer = new System.Timers.Timer();
            timer.Interval = Delay; //In milliseconds here
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        public KeyboardAction(string text, MaterialIconType type, Keys key, int delay = 10) : this(text, type, new List<KeyAction>() { new KeyAction(key) }, delay)
        { }

        public override bool HoverRelease()
        {
            timer.Start();
            return true;
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            foreach (var key in Key)
            {
                switch (key.Operation)
                {
                    case KeyOperation.Press:
                        KeyboardSimulator.KeyPress(key.Key);
                        break;
                    case KeyOperation.Down:
                        KeyboardSimulator.KeyDown(key.Key);
                        break;
                    case KeyOperation.Up:
                        KeyboardSimulator.KeyUp(key.Key);
                        break;
                }
                System.Threading.Thread.Sleep(key.Delay);
            }
        }
    }
}
