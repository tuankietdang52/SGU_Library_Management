using SGULibraryManagement.GUI;
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
            Width = content.Width;
            Height = content.Height + 35;
            dialogContent.Content = content;
        }

        private void OnCloseDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
