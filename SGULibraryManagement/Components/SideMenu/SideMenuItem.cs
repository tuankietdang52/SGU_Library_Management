using SGULibraryManagement.Config;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SGULibraryManagement.Components.SideMenu
{
    public class SideMenuItem : ContentControl
    {
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(nameof(Text), 
                                        typeof(string), 
                                        typeof(SideMenuItem), 
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register(nameof(Glyph),
                                        typeof(string),
                                        typeof(SideMenuItem),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ContentViewProperty =
            DependencyProperty.Register(nameof(ContentView),
                                        typeof(Type),
                                        typeof(SideMenuItem),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected),
                                        typeof(bool),
                                        typeof(SideMenuItem),
                                        new PropertyMetadata(null));


        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        public Type ContentView
        {
            get => (Type)GetValue(ContentViewProperty);
            set => SetValue(ContentViewProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set {
                SetValue(IsSelectedProperty, value);
                OnSelected(value);
            }
        }

        static SideMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SideMenuItem), new FrameworkPropertyMetadata(typeof(SideMenuItem)));
        }

        private void OnSelected(bool isSelected)
        {
            if (App.Instance!.Resources["AppThemeSecondary"] is not SolidColorBrush color) return;

            if (isSelected) Foreground = color;
            else Foreground = Brushes.Black;
        }
    }
}
