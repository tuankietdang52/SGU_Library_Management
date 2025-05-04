using SGULibraryManagement.Components.Buttons;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register(nameof(PlaceholderForeground),
                                        typeof(Brush),
                                        typeof(TextField),
                                        new PropertyMetadata(Brushes.DarkGray));

        public static readonly DependencyProperty MaxLengthProperty =
           DependencyProperty.Register(nameof(MaxLength),
                                       typeof(int),
                                       typeof(TextField),
                                       new PropertyMetadata(int.MaxValue));


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

        public Brush PlaceholderForeground
        {
            get => (Brush)GetValue(PlaceholderForegroundProperty);
            set => SetValue(PlaceholderForegroundProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        
        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public bool AcceptNumberOnly { get; set; } = false;
        public bool IsEmailField { get; set; } = false;

        private Brush? initBorderBrush;

        public new bool Focus()
        {
            if (GetTemplateChild("textBox") is not TextBox textBox) return false;
            return textBox.Focus();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("clearButton") is not RoundButton clearButton) return;
            if (GetTemplateChild("textBox") is not TextBox textBox) return;

            initBorderBrush = BorderBrush;
            SetupTextBox(textBox);

            clearButton.Click += (sender, e) =>
            {
                textBox.Clear();
            };
        }

        private void SetupTextBox(TextBox textBox)
        {
            textBox.TextChanged += (sender, e) =>
            {
                Text = textBox.Text;
                TextChanged?.Invoke(sender, e);
            };

            textBox.PreviewMouseDown += (sender, e) =>
            {
                textBox.Focus();
                textBox.CaretIndex = textBox.Text.Length;
            };

            textBox.PreviewTextInput += (sender, e) =>
            {
                if (AcceptNumberOnly) ApplyNumberOnlyFormat(sender, e);
            };

            textBox.LostFocus += (sender, e) =>
            {
                if (IsEmailField) CheckEmailFormat(textBox);
            };
        }

        private void ApplyNumberOnlyFormat(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }

        private void CheckEmailFormat(TextBox textBox)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            bool isValid = Regex.IsMatch(textBox.Text, pattern);

            if (!isValid) textBox.BorderBrush = Brushes.Red;
            else textBox.BorderBrush = initBorderBrush;
        }

        static TextField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextField), new FrameworkPropertyMetadata(typeof(TextField)));
        }
    }
}
