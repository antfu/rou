using System;
using System.Collections.Generic;
using System.Linq;
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
using MaterialIcons;

namespace Rou.Controls
{
    /// <summary>
    /// Interaction logic for Sector.xaml
    /// </summary>
    public partial class Sector : UserControl
    {
        public double OuterRadius
        {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }
        public MaterialIconType Icon
        {
            get { return (MaterialIconType)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register(
                  "OuterRadius",
                  typeof(double),
                  typeof(Sector),
                  new PropertyMetadata(false)
              );
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
                  "Icon",
                  typeof(MaterialIconType),
                  typeof(Sector),
                  new PropertyMetadata(false)
              );
        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
                  "InnerRadius",
                  typeof(double),
                  typeof(Sector),
                  new PropertyMetadata(false)
              );
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                  "Text",
                  typeof(string),
                  typeof(Sector),
                  new PropertyMetadata(false)
              );

        public Sector()
        {
            InitializeComponent();
        }
    }
}
