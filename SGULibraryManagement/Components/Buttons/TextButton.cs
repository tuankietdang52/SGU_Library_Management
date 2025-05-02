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

namespace SGULibraryManagement.Components.Buttons
{
    public class TextButton : ContentControl
    {
        public event RoutedEventHandler? Click;

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register(nameof(Text),
                                       typeof(string),
                                       typeof(TextButton),
                                       new PropertyMetadata(""));

        public static readonly DependencyProperty TextDecorationsProperty =
          DependencyProperty.Register(nameof(TextDecorations),
                                      typeof(TextDecorationCollection),
                                      typeof(TextButton),
                                      new PropertyMetadata(null));

        public static readonly DependencyProperty HoverForegroundColorProperty =
           DependencyProperty.Register(nameof(HoverForegroundColor),
                                       typeof(Brush),
                                       typeof(TextButton),
                                       new PropertyMetadata(null));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextDecorationCollection TextDecorations
        {
            get => (TextDecorationCollection)GetValue(TextDecorationsProperty);
            set => SetValue(TextDecorationsProperty, value);
        }

        public Brush HoverForegroundColor
        {
            get => (Brush)GetValue(HoverForegroundColorProperty);
            set => SetValue(HoverForegroundColorProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HoverForegroundColor ??= Foreground;

            if (GetTemplateChild("content") is TextBlock content)
            {
                content.MouseDown += (e, sender) =>
                {
                    Click?.Invoke(this, sender);
                };
            }
        }

        static TextButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextButton), new FrameworkPropertyMetadata(typeof(TextButton)));
        }
    }
}
