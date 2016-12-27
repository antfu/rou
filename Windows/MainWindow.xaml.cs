using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using MaterialIcons;
using Rou.Utils;
using Rou.Actions;
using Rou.Windows;

namespace Rou
{
    public partial class MainWindow : Window
    {
        public List<Action> actions;

        private bool _shown = false;
        private KeyboardHookEx hookEx = new KeyboardHookEx();
        private NotifyIcon notifyIcon = null;
        private Action currentAction = null;
        private Storyboard StoryboardIn;
        private Storyboard StoryboardOut;

        public bool ShowText { get; private set; } = true;

        public MainWindow()
        {
            IsHitTestVisible = false;
            InitializeComponent();
            init();
        }

        public void init()
        {
            foreach (var key in C.HotKey)
                hookEx.HookedKeys.Add(key);
            hookEx.KeyDown += HookEx_KeyDown;
            hookEx.KeyUp += HookEx_KeyUp;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.icon_32_ico;
            notifyIcon.BalloonTipTitle = "Rou";
            notifyIcon.BalloonTipText = "";
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            var options = new System.Windows.Forms.MenuItem { Text = "Options" };
            var exit = new System.Windows.Forms.MenuItem { Text = "Exit" };
            notifyIcon.ContextMenu.MenuItems.Add(options);
            notifyIcon.ContextMenu.MenuItems.Add(exit);
            options.Click += (s, e) =>
            {
                (new IconSelectWindow()).Show();
            };
            exit.Click += (s, e) =>
            {
                Exit();
            };

            rouBack.Width = C.RouRaduis * 2;
            rouBack.Height = C.RouRaduis * 2;
            cavans.Width = C.RouRaduis * 2;
            cavans.Height = C.RouRaduis * 2;
            Width = C.RouRaduis * 2 + C.RouPadding * 2;
            Height = C.RouRaduis * 2 + C.RouPadding * 2;

            /*
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
            */
            actions = JsonLoader.LoadAction(@"Preset\default.json");

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
            if (currentAction?.HoverRelease() != false)
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
                Opacity = 0;
                ShowByMouse();
                currentAction = null;

                StoryboardIn.Seek(TimeSpan.FromMilliseconds(1), TimeSeekOrigin.BeginTime);
                StoryboardIn.Resume();

                StoryboardOut.Pause();
                StoryboardOut.Seek(TimeSpan.FromMilliseconds(1), TimeSeekOrigin.BeginTime);
                Opacity = 1;
                IsHitTestVisible = true;
            }
        }

        public void HideRou()
        {
            if (_shown)
            {
                IsHitTestVisible = false;
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
            double iconRaduis = (C.RouRaduis + C.RouInnderRaduis) / 2;
            for (int i = 0; i < count; i++)
            {
                var action = actions[i];
                var path = CreateSector(C.RouRaduis, C.RouInnderRaduis, offest + sectorTheta * i, offest + sectorTheta * (i + 1), C.RouBackBrush, C.RouStrokeBrush);
                var icon = action.Icon;
                cavans.Children.Add(path);
                cavans.Children.Add(icon);

                path.Tag = action;
                path.Cursor = System.Windows.Input.Cursors.Hand;
                path.MouseEnter += Path_MouseEnter;
                path.MouseLeave += Path_MouseLeave;

                icon.Width = C.RouIconSize;
                icon.Height = C.RouIconSize;
                icon.Foreground = C.RouActionIconBrush;
                icon.IsHitTestVisible = false;

                var iconPos = PolarToRect(iconRaduis, (i + 0.5) * sectorTheta + offest);
                Canvas.SetLeft(path, C.RouRaduis);
                Canvas.SetTop(path, C.RouRaduis);
                Canvas.SetLeft(icon, C.RouRaduis - C.RouIconSize / 2 + iconPos.X);
                Canvas.SetTop(icon, C.RouRaduis - C.RouIconSize / 2 + iconPos.Y - (ShowText ? 10 : 0));

                if (ShowText)
                {
                    var label = new TextBlock();
                    label.Text = action.Text;
                    label.Foreground = C.RouActionIconTextBrush;
                    label.VerticalAlignment = VerticalAlignment.Top;
                    label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    label.FontSize = 9;
                    label.TextTrimming = TextTrimming.CharacterEllipsis;
                    label.TextAlignment = TextAlignment.Center;
                    label.Width = C.RouIconSize / 2 * 3;
                    cavans.Children.Add(label);
                    Canvas.SetLeft(label, C.RouRaduis + iconPos.X - label.Width / 2);
                    Canvas.SetTop(label, C.RouRaduis + iconPos.Y + 5);
                }
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
            Left = x - C.RouRaduis - C.RouPadding;
            Top = y - C.RouRaduis - C.RouPadding;
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
            path.Opacity = C.RouSectorOpacity;

            return path;
        }

        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var path = (Path)sender;
            (path.Tag as Action)?.HoverRelease();
            HideRou();
        }

        private void Path_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity =C.RouSectorOpacity;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = false;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = false;
            currentAction = null;
        }

        private void Path_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity = C.RouSectorActiveOpacity;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = true;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = true;
            currentAction = path.Tag as Action;
        }

        public void Exit()
        {
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