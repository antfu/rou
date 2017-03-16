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
        public const double RouRaduis = 100;
        public const double RouInnderRaduis = 30;
        public const double RouPadding = 40;
        public const double RouIconSize = 30;
        public const double RouSectorOpacity = 0.8;
        public const double RouSectorActiveOpacity = 1;
        public readonly static Brush RouBackBrush = new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
        public readonly static Brush RouStrokeBrush = new SolidColorBrush(Color.FromArgb(200, 128, 128, 128));
        public readonly static Brush RouActiveStrokeBrush = new SolidColorBrush(Color.FromArgb(255, 121, 189, 143));
        public readonly static Brush RouActionIconBrush = new SolidColorBrush(Colors.White);
        public readonly static Brush RouActionIconTextBrush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
    }
}
