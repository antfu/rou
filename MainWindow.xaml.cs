using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        public readonly Brush RouBackBrush = new SolidColorBrush(Color.FromArgb(128, 70, 70, 70));
        public readonly Brush RouStrokeBrush = new SolidColorBrush(Colors.Gray);

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
            this.Width = RouRaduis * 2 + RouPadding;
            this.Height = RouRaduis * 2 + RouPadding;

            initSector(8);

        }

        public void initSector(int count)
        {
            cavans.Children.Clear();

            double sectorTheta = Math.PI * 2 / count;
            for (int i = 0; i < count; i++)
            {
                var path = CreateSector(RouRaduis, RouInnderRaduis, -Math.PI / 2 + sectorTheta * i, -Math.PI / 2 + sectorTheta * (i + 1), RouBackBrush, RouStrokeBrush);
                cavans.Children.Add(path);
                Canvas.SetLeft(path, RouRaduis);
                Canvas.SetTop(path, RouRaduis);
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

        public void ShowAtPosition(double x, double y)
        {
            this.Left = x - this.Width / 2;
            this.Top = y - this.Height / 2;
        }

        public void ShowByMouse()
        {
            Point point = GetMousePosition();
            ShowAtPosition(point.X, point.Y);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowByMouse();
        }

        private static Point PolarToRect(double r, double theta)
        {
            return new Point(Math.Cos(theta) * r, Math.Sin(theta) * r);
        }

        private static Path CreateSector(double outerRadius, double innerRadius, double fromDegree, double toDegree, Brush fill, Brush stroke)
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

        private static void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var path = (Path)sender;
            MessageBox.Show("Hello");
        }

        private static void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            var path = (Path)sender;
            path.Opacity = 0.5;
            (path.Data as PathGeometry).Figures[0].Segments[0].IsStroked = false;
            (path.Data as PathGeometry).Figures[0].Segments[2].IsStroked = false;
        }

        private static void Path_MouseEnter(object sender, MouseEventArgs e)
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
    }
}
