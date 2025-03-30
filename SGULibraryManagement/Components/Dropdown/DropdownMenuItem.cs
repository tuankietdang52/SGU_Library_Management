using SGULibraryManagement.Components.Buttons;
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
    public class DropdownMenuItem : MenuItem
    {
        public static readonly DependencyProperty HoverForegroundProperty =
         DependencyProperty.Register(nameof(HoverForeground),
                                     typeof(SolidColorBrush),
                                     typeof(DropdownMenuItem),
                                     new PropertyMetadata(Brushes.White));

        public SolidColorBrush HoverForeground
        {
            get => (SolidColorBrush)GetValue(HoverForegroundProperty);
            set => SetValue(HoverForegroundProperty, value);
        }

        static DropdownMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropdownMenuItem), new FrameworkPropertyMetadata(typeof(DropdownMenuItem)));
        }
    }
}
