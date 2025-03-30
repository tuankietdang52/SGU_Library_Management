using System.Windows;
using System.Windows.Controls;

namespace SGULibraryManagement.Components.Dropdown
{
    public class DropdownMenu : Menu
    {
        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register(nameof(CornerRadius),
                                      typeof(CornerRadius),
                                      typeof(DropdownMenu),
                                      new PropertyMetadata(null));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        static DropdownMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropdownMenu), new FrameworkPropertyMetadata(typeof(DropdownMenu)));
        }
    }
}
