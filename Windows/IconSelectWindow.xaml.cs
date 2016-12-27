using System;
using System.Windows;
using MaterialIcons;
using System.Windows.Media;
using System.Windows.Controls;

namespace Rou.Windows
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class IconSelectWindow : Window
    {
        public IconSelectWindow()
        {
            InitializeComponent();
            Init();
        }
        public void Init() {
            var white = new SolidColorBrush(Colors.White);
            var padding = 7;
            var size = 24 + padding * 2;

            foreach (MaterialIconType icon in Enum.GetValues(typeof(MaterialIconType))) {
                var i = new MaterialIcon();
                i.Icon = icon;
                i.Foreground = white;
                i.Width = size;
                i.Height = size;
                i.Padding = new Thickness(padding);
                Container.Children.Add(i);
                i.MouseEnter += I_MouseEnter;
                i.MouseLeave += I_MouseLeave;
                i.MouseLeftButtonDown += I_MouseLeftButtonDown;
            }
        }

        private void I_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedIcon.Icon = (sender as MaterialIcon).Icon;
        }

        private void I_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as MaterialIcon).Background = null;
        }

        private void I_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as MaterialIcon).Background = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128)) ;
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
