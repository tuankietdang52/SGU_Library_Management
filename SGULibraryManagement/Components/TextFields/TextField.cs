using SGULibraryManagement.Components.Buttons;
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

namespace SGULibraryManagement.Components.TextFields
{
    public class TextField : ContentControl
    {
        public event TextChangedEventHandler? TextChanged;

        public static readonly DependencyProperty CornerRadiusProperty =
           DependencyProperty.Register(nameof(CornerRadius),
                                       typeof(CornerRadius),
                                       typeof(TextField),
                                       new PropertyMetadata(null));

        public static readonly DependencyProperty PlaceholderProperty =
           DependencyProperty.Register(nameof(Placeholder),
                                       typeof(string),
                                       typeof(TextField),
                                       new PropertyMetadata(""));

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register(nameof(Text),
                                       typeof(string),
                                       typeof(TextField),
                                       new PropertyMetadata(""));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("clearButton") is not RoundButton clearButton) return;
            if (GetTemplateChild("textBox") is not TextBox textBox) return;

            textBox.TextChanged += (sender, e) =>
            {
                Text = textBox.Text;
                TextChanged?.Invoke(sender, e);
            };

            clearButton.Click += (sender, e) =>
            {
                textBox.Clear();
            };
        }

        static TextField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextField), new FrameworkPropertyMetadata(typeof(TextField)));
        }
    }
}
