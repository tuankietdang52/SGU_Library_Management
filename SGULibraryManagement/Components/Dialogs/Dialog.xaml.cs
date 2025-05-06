using SGULibraryManagement.GUI.Contents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace SGULibraryManagement.Components.Dialogs
{
    public partial class Dialog : FluentWindow
    {
        protected IDialog? DialogContent;

        public Dialog()
        {
            InitializeComponent();
        }

        public Dialog(string title, IDialog content)
        {
            InitializeComponent();
            DialogContent = content;

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
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        protected virtual void OnCloseDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public partial class Dialog<TReturn> : Dialog
    {
        private IDialog<TReturn>? DContent => DialogContent as IDialog<TReturn>;
        public TReturn? Return => DContent is null ? default : DContent.Return;

        public Dialog() : base()
        {

        }

        public Dialog(string title, IDialog<TReturn> content) : base(title, content)
        {

        }
    }
}
