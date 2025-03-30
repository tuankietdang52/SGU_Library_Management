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

namespace SGULibraryManagement.Components.Dropdown
{
    public class DropdownSubMenu : MenuItem
    {
        public static readonly DependencyProperty HoverBackgroundProperty =
          DependencyProperty.Register(nameof(HoverBackground),
                                      typeof(SolidColorBrush),
                                      typeof(DropdownSubMenu),
                                      new PropertyMetadata(Brushes.LightGray));

        public static readonly DependencyProperty HoverForegroundProperty =
         DependencyProperty.Register(nameof(HoverForeground),
                                     typeof(SolidColorBrush),
                                     typeof(DropdownSubMenu),
                                     new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty PopupBackgroundProperty =
         DependencyProperty.Register(nameof(PopupBackground),
                                     typeof(SolidColorBrush),
                                     typeof(DropdownSubMenu),
                                     new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register(nameof(CornerRadius),
                                      typeof(CornerRadius),
                                      typeof(DropdownSubMenu),
                                      new PropertyMetadata(null));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public SolidColorBrush HoverBackground
        {
            get => (SolidColorBrush)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }

        public SolidColorBrush HoverForeground
        {
            get => (SolidColorBrush)GetValue(HoverForegroundProperty);
            set => SetValue(HoverForegroundProperty, value);
        }

        public SolidColorBrush PopupBackground
        {
            get => (SolidColorBrush)GetValue(PopupBackgroundProperty);
            set => SetValue(PopupBackgroundProperty, value);
        }

        static DropdownSubMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropdownSubMenu), new FrameworkPropertyMetadata(typeof(DropdownSubMenu)));
        }
    }
}
