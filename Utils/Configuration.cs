using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;

namespace Rou.Utils
{
    class Configuration
    {
        public Keys Hotkey { get; set; }

        public Configuration()
        {
            Hotkey = Keys.Apps;
        }
    }
}
