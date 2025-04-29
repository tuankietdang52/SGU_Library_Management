using SGULibraryManagement.Components.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SGULibraryManagement.Components.TextFields
{
    public class TextLabel : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register(nameof(CornerRadius),
                                      typeof(CornerRadius),
                                      typeof(TextLabel),
                                      new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register(nameof(Text),
                                       typeof(string),
                                       typeof(TextLabel),
                                       new PropertyMetadata(""));

        public static readonly DependencyProperty TextDecorationsProperty =
          DependencyProperty.Register(nameof(TextDecorations),
                                      typeof(TextDecorationCollection),
                                      typeof(TextLabel),
                                      new PropertyMetadata(null));

        public static new readonly DependencyProperty PaddingProperty =
          DependencyProperty.Register(nameof(Padding),
                                      typeof(Thickness),
                                      typeof(TextLabel),
                                      new PropertyMetadata(new Thickness(6)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

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

        public new Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        static TextLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextLabel), new FrameworkPropertyMetadata(typeof(TextLabel)));
        }
    }
}
