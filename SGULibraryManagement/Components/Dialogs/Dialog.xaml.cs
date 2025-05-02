using SGULibraryManagement.GUI;
using SGULibraryManagement.GUI.Contents;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace SGULibraryManagement.Components.Dialogs
{
    public partial class Dialog : FluentWindow
    {
        public Dialog()
        {
            InitializeComponent();
        }

        public Dialog(string title, IDialog content)
        {
            InitializeComponent();

            if (content is not UserControl userControl)
            {
                throw new InvalidOperationException("content is not User Control");
            }

            SetupDialog(title, userControl);

            content.OnCloseDialog += (sender) => OnCloseDialog(sender, null!);
            content.PopupHost = popup;
        }
        private void SetupDialog(string title, UserControl content)
        {
            titleBarTitle.Text = title;
            Title = title;
            
            MinWidth = content.Width;
            MaxWidth = content.Width;
            MinHeight = content.Height + 35;
            MaxHeight = content.Height + 35;

            Width = content.Width;
            Height = content.Height + 35;

            content.SizeChanged += OnSizeChanged;
            dialogContent.Content = content;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;

            MinWidth = size.Width;
            MaxWidth = size.Width;
            MinHeight = size.Height + 35;
            MaxHeight = size.Height + 35;

            Width = size.Width;
            Height = size.Height + 35;
        }

        private void OnStatusBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void OnCloseDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
