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
using MaterialIcons;
using System.Windows.Forms;

namespace Rou
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly Keys HotKey = Keys.F7;
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
        private Storyboard StoryboardIn;
        private Storyboard StoryboardOut;

        public MainWindow()
        {
            //Visibility = Visibility.Hidden;
            InitializeComponent();
            init();
        }

        public void init()
        {
            hookEx.HookedKeys.Add(HotKey);
            hookEx.KeyDown += HookEx_KeyDown;
            hookEx.KeyUp += HookEx_KeyUp;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.icon_32_ico;
            notifyIcon.BalloonTipTitle = "Rou";
            notifyIcon.BalloonTipText = "";
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            var exit = new System.Windows.Forms.MenuItem { Text = "Exit" };
            notifyIcon.ContextMenu.MenuItems.Add(exit);
            exit.Click += (s, e) =>
            {
                Exit();
            };

            rouBack.Width = RouRaduis * 2;
            rouBack.Height = RouRaduis * 2;
            cavans.Width = RouRaduis * 2;
            cavans.Height = RouRaduis * 2;
            Width = RouRaduis * 2 + RouPadding * 2;
            Height = RouRaduis * 2 + RouPadding * 2;

            actions = new List<Action>();
            actions.Add(new KeyboardAction("Pause", MaterialIconType.ic_pause, Keys.MediaPlayPause));
            actions.Add(new KeyboardAction("Next Track", MaterialIconType.ic_skip_next, Keys.MediaNextTrack));
            actions.Add(new KeyboardAction("Mute", MaterialIconType.ic_volume_off, Keys.VolumeMute));
            actions.Add(new KeyboardAction("Task View", MaterialIconType.ic_view_quilt, new List<KeyAction>() {
                new KeyAction(Keys.LWin, KeyOperation.Down),
                new KeyAction(Keys.Tab, KeyOperation.Press),
                new KeyAction(Keys.LWin, KeyOperation.Up)
            }, 100));
            actions.Add(new KeyboardAction("Prev Track", MaterialIconType.ic_skip_previous, Keys.MediaPreviousTrack));

            StoryboardIn = this.TryFindResource("StoryboardIn") as Storyboard;
            StoryboardOut = this.TryFindResource("StoryboardOut") as Storyboard;
            initSector();
            StoryboardOut.Begin();
            StoryboardOut.Pause();
            StoryboardIn.Begin();
            StoryboardIn.Pause();
        }

        private void HookEx_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (currentAction?.Invoke() != false)
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
                _shown = true;
                this.Opacity = 0;
                ShowByMouse();
                currentAction = null;

                StoryboardIn.Seek(TimeSpan.FromMilliseconds(1), TimeSeekOrigin.BeginTime);
                StoryboardIn.Resume();

                StoryboardOut.Pause();
                StoryboardOut.Seek(TimeSpan.FromMilliseconds(1), TimeSeekOrigin.BeginTime);
                this.Opacity = 1;
            }
        }

        public void HideRou()
        {
            if (_shown)
            {
                _shown = false;
                StoryboardOut.Resume();
                StoryboardIn.Pause();
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
                path.Cursor = System.Windows.Input.Cursors.Hand;
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


        #region Noactivate Window
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            //Set the window style to noactivate.
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }
        #endregion
        

        public void ShowAtPosition(double x, double y)
        {
            Left = x - RouRaduis - RouPadding;
            Top = y - RouRaduis - RouPadding;
        }

        public void ShowByMouse()
        {
            Point point = MouseCursor.GetMousePosition();
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
            (path.Tag as Action)?.Invoke();
            HideRou();
        }

        private void Path_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity = 0.4;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = false;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = false;
            currentAction = null;
        }

        private void Path_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity = 0.7;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = true;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = true;
            currentAction = path.Tag as Action;
        }

        public void Exit() {
            hookEx.unhook();
            notifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}