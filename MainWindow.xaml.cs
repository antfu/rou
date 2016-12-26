using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Rou.Utils;
using Rou.Actions;
using System.Windows.Interop;

namespace Rou
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly double RouRaduis = 100;
        public readonly double RouInnderRaduis = 30;
        public readonly double RouPadding = 40;
        public readonly double RouIconSize = 30;
        public readonly Brush RouBackBrush = new SolidColorBrush(Color.FromArgb(128, 70, 70, 70));
        public readonly Brush RouStrokeBrush = new SolidColorBrush(Colors.Gray);

        public List<Action> actions;

        public MainWindow()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            rouBack.Width = RouRaduis * 2;
            rouBack.Height = RouRaduis * 2;
            cavans.Width = RouRaduis * 2;
            cavans.Height = RouRaduis * 2;
            RouContainer.Width = RouRaduis * 2 + RouPadding;
            RouContainer.Height = RouRaduis * 2 + RouPadding;

            actions = new List<Action>();
            actions.Add(new NextTrackAction());
            actions.Add(new PauseTrackAction());
            actions.Add(new PrevTrackAction());
            actions.Add(new WinAction());

            initSector();
        }

        public void ShowRou()
        {
            ShowByMouse();
            this.Show();
        }

        public void HideRou()
        {
            this.Hide();
        }

        public void initSector()
        {
            cavans.Children.Clear();

            int count = actions.Count;

            double sectorTheta = Math.PI * 2 / count;
            double offest = -Math.PI / 2 + sectorTheta / 2;
            double iconRaduis = (RouRaduis + RouInnderRaduis) / 2;
            for (int i = 0; i < count; i++)
            {
                var action = actions[i];
                var path = CreateSector(RouRaduis, RouInnderRaduis, offest + sectorTheta * i, offest + sectorTheta * (i + 1), RouBackBrush, RouStrokeBrush);
                cavans.Children.Add(path);
                path.Tag = action;
                Canvas.SetLeft(path, RouRaduis);
                Canvas.SetTop(path, RouRaduis);
                var icon = action.Icon;
                cavans.Children.Add(icon);
                icon.Width = RouIconSize;
                icon.Height = RouIconSize;
                icon.Foreground = new SolidColorBrush(Colors.White);
                var iconPos = PolarToRect(iconRaduis, (i + 0.5) * sectorTheta + offest);
                Canvas.SetLeft(icon, RouRaduis - RouIconSize / 2 + iconPos.X);
                Canvas.SetTop(icon, RouRaduis - RouIconSize / 2 + iconPos.Y);
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            //Set the window style to noactivate.
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE,
                GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);



        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private const int HOTKEY_ID = 9000;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //[NONE]
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        private const uint MOD_WIN = 0x0008; //WINDOWS
                                             //CAPS LOCK:
        private const uint VK_CAPITAL = 0x14;

        private HwndSource source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

            RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL + MOD_SHIFT, (UInt32)System.Windows.Forms.Keys.Z);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);

                            ShowRou();

                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }



        public void ShowAtPosition(double x, double y)
        {
            RouContainer.Margin = new Thickness(x - RouRaduis, y - RouRaduis, 0, 0);
        }

        public void ShowByMouse()
        {
            Point point = GetMousePosition();
            ShowAtPosition(point.X, point.Y);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowRou();
        }

        private static Point PolarToRect(double r, double theta)
        {
            return new Point(Math.Cos(theta) * r, Math.Sin(theta) * r);
        }

        private Path CreateSector(double outerRadius, double innerRadius, double fromDegree, double toDegree, Brush fill, Brush stroke)
        {
            if (fromDegree > toDegree)
            {
                var temp = toDegree;
                toDegree = fromDegree;
                fromDegree = temp;
            }

            var p1 = PolarToRect(outerRadius, fromDegree);
            var p2 = PolarToRect(innerRadius, fromDegree);
            var p3 = PolarToRect(innerRadius, toDegree);
            var p4 = PolarToRect(outerRadius, toDegree);

            var path = new Path();
            var largeArc = (toDegree - fromDegree) >= Math.PI;

            var geo = new PathGeometry(new List<PathFigure>() {
                new PathFigure(p1, new List<PathSegment>() {
                    new LineSegment(p2, false),
                    new ArcSegment(p3, new Size(innerRadius, innerRadius), 0, largeArc, SweepDirection.Clockwise, true),
                    new LineSegment(p4, false),
                    new ArcSegment(p1, new Size(outerRadius, outerRadius), 0, largeArc, SweepDirection.Counterclockwise, true),
                }, true)
            });

            path.Data = geo;
            path.Fill = fill;
            path.Stroke = stroke;
            path.Opacity = 0.5;

            path.MouseEnter += Path_MouseEnter;
            path.MouseLeave += Path_MouseLeave;
            path.MouseDown += Path_MouseDown;

            return path;
        }

        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var path = (Path)sender;
            (path.Tag as Action)?.Click();
            HideRou();
        }

        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity = 0.5;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = false;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = false;
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity = 1;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = true;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = true;
        }

        private void rouCenter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideRou();
        }
    }
}
