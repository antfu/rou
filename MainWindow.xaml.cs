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
using System.Windows.Media.Animation;

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
        public readonly Brush RouBackBrush = new SolidColorBrush(Color.FromArgb(60, 128, 128, 128));
        public readonly Brush RouStrokeBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

        public List<Action> actions;

        private bool _shown = false;
        private KeyboardHookEx hookEx = new KeyboardHookEx();
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private Action currentAction = null;
        private DoubleAnimation scalerAnimator = new DoubleAnimation()
        {
            Duration = new Duration(TimeSpan.FromMilliseconds(500)),
        };
        private Storyboard StorayboardIn;

        public MainWindow()
        {
            //Visibility = Visibility.Hidden;
            InitializeComponent();
            init();
        }

        public void init()
        {
            hookEx.HookedKeys.Add(System.Windows.Forms.Keys.F7);
            hookEx.KeyDown += HookEx_KeyDown;
            hookEx.KeyUp += HookEx_KeyUp;

            notifyIcon = new System.Windows.Forms.NotifyIcon();

            rouBack.Width = RouRaduis * 2;
            rouBack.Height = RouRaduis * 2;
            cavans.Width = RouRaduis * 2;
            cavans.Height = RouRaduis * 2;
            this.Width = RouRaduis * 2 + RouPadding*2;
            this.Height = RouRaduis * 2 + RouPadding*2;

            actions = new List<Action>();
            actions.Add(new PauseTrackAction());
            actions.Add(new NextTrackAction());
            actions.Add(new WinAction());
            actions.Add(new KeyPressAction("Task View", MaterialIcons.MaterialIconType.ic_view_quilt, new List<KeyAction>() {
                new KeyAction(System.Windows.Forms.Keys.LWin, KeyOperation.Down),
                new KeyAction(System.Windows.Forms.Keys.Tab, KeyOperation.Press),
                new KeyAction(System.Windows.Forms.Keys.LWin, KeyOperation.Up)
            }, 100));
            actions.Add(new PrevTrackAction());

            StorayboardIn =  this.TryFindResource("ScaleOutStoryboard") as Storyboard;
            initSector();
            StorayboardIn.Begin();
            StorayboardIn.Pause();
        }

        private void HookEx_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            currentAction?.Click();
            HideRou();
            e.Handled = true;
        }

        private void HookEx_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            ShowRou();
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            notifyIcon.Visible = true;
        }

        public void ShowRou()
        {
            if (!_shown)
            {
                this.Opacity = 0;
                ShowByMouse();
                currentAction = null;
                _shown = true;
             
                StorayboardIn.Resume();

                this.Opacity = 1;
            }
        }

        public void HideRou()
        {
            if (_shown)
            {
                this.Opacity = 0;
                StorayboardIn.Pause();
                StorayboardIn.Seek(TimeSpan.FromMilliseconds(1), TimeSeekOrigin.BeginTime);
                _shown = false;
            }
        }

        public void initSector()
        {
            cavans.Children.Clear();

            int count = actions.Count;

            double sectorTheta = Math.PI * 2 / count;
            double offest = -Math.PI / 2 - sectorTheta / 2;
            double iconRaduis = (RouRaduis + RouInnderRaduis) / 2;
            for (int i = 0; i < count; i++)
            {
                var action = actions[i];
                var path = CreateSector(RouRaduis, RouInnderRaduis, offest + sectorTheta * i, offest + sectorTheta * (i + 1), RouBackBrush, RouStrokeBrush);
                var icon = action.Icon;
                cavans.Children.Add(path);
                cavans.Children.Add(icon);

                path.Tag = action;
                path.MouseEnter += Path_MouseEnter;
                path.MouseLeave += Path_MouseLeave;
                path.MouseDown += Path_MouseDown;

                icon.Width = RouIconSize;
                icon.Height = RouIconSize;
                icon.Foreground = new SolidColorBrush(Colors.White);
                icon.IsHitTestVisible = false;

                var iconPos = PolarToRect(iconRaduis, (i + 0.5) * sectorTheta + offest);
                Canvas.SetLeft(path, RouRaduis);
                Canvas.SetTop(path, RouRaduis);
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
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        public void ShowAtPosition(double x, double y)
        {
            //RouContainer.Margin = new Thickness(x - RouRaduis, y - RouRaduis, 0, 0);
            Left = x - RouRaduis - RouPadding;
            Top = y - RouRaduis - RouPadding;
        }

        public void ShowByMouse()
        {
            Point point = GetMousePosition();
            ShowAtPosition(point.X, point.Y);
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
            path.Opacity = 0.4;
            
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
            path.Opacity = 0.4;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = false;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = false;
            currentAction = null;
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity = 0.5;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = true;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = true;
            currentAction = path.Tag as Action;
        }

        private void rouCenter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideRou();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notifyIcon.Visible = false;
        }
    }
}