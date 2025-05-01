using SGULibraryManagement.Components.SideMenu;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace SGULibraryManagement.Components.Buttons
{
    public class RoundButton : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius),
                                        typeof(CornerRadius),
                                        typeof(RoundButton),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty HoverBackgroundColorProperty =
            DependencyProperty.Register(nameof(HoverBackgroundColor),
                                        typeof(Brush),
                                        typeof(RoundButton),
                                        new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty HoverForegroundColorProperty =
            DependencyProperty.Register(nameof(HoverForegroundColor),
                                        typeof(Brush),
                                        typeof(RoundButton),
                                        new FrameworkPropertyMetadata(Brushes.White));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Brush HoverBackgroundColor
        {
            get => (Brush)GetValue(HoverBackgroundColorProperty);
            set => SetValue(HoverBackgroundColorProperty, value);
        }

        public Brush HoverForegroundColor
        {
            get => (Brush)GetValue(HoverForegroundColorProperty);
            set => SetValue(HoverForegroundColorProperty, value);
        }

        static RoundButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundButton), new FrameworkPropertyMetadata(typeof(RoundButton)));
        }
    }
}
