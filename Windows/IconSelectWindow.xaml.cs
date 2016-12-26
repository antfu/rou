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
            foreach (MaterialIconType icon in Enum.GetValues(typeof(MaterialIconType))) {
                var i = new MaterialIcon();
                i.Icon = icon;
                i.Foreground = white;
                i.Width = 24;
                i.Height = 24;
                i.Margin = new Thickness(5);
                Container.Children.Add(i);
            }
        }
    }
}
