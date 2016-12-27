using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;

namespace Rou.Utils
{
    static class C
    {
        public const Keys HotKey = Keys.F7;
        public const double RouRaduis = 100;
        public const double RouInnderRaduis = 30;
        public const double RouPadding = 40;
        public const double RouIconSize = 30;
        public readonly static Brush RouBackBrush = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
        public readonly static Brush RouStrokeBrush = new SolidColorBrush(Color.FromArgb(200, 128, 128, 128));
    }
}
