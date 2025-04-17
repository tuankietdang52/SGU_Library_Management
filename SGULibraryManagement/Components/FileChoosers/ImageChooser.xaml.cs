using Microsoft.Win32;
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
using System.IO;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.GUI;


namespace SGULibraryManagement.Components.FileChoosers
{
    public delegate void OnImageChooseHandler(object sender, string filePath);

    public partial class ImageChooser : UserControl
    {
        public event OnImageChooseHandler? OnImageChoose;
        public ContentPresenter? PopupHost { get; set; }

        public CornerRadius CornerRadius
        {
            get => imageChooserContainer.CornerRadius;
            set {
                imageChooserContainer.CornerRadius = value;
                imageContainer.CornerRadius = value;
            }
        }

        private string filePath = "";
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;

                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    imageContainer.Source = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute));
                    imageContainer.Visibility = Visibility.Visible;
                    nonImageContainer.Visibility = Visibility.Collapsed;
                    imageChooserContainer.BorderThickness = new Thickness(0);
                }
                else
                {
                    imageContainer.Source = null;
                    imageContainer.Visibility = Visibility.Collapsed;
                    nonImageContainer.Visibility = Visibility.Visible;
                    imageChooserContainer.BorderThickness = new Thickness(1);
                }
            }
        }
        public ImageChooser()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            imageChooserContainer.Background = App.Instance!.Resources["LightGray80"] as SolidColorBrush;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            imageChooserContainer.Background = Brushes.LightGray;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            OpenFileChooser();
        }

        private void OpenFileChooser()
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select an Image",
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                ChangeImage(dialog.FileName);
            }
        }

        private void ChangeImage(string fileName)
        {
            var source = new BitmapImage(new Uri(fileName));

            if (source.Width != source.Height)
            {
                OnAlert();
                return;
            }

            imageContainer.Visibility = Visibility.Visible;
            nonImageContainer.Visibility = Visibility.Collapsed;
            imageChooserContainer.BorderThickness = new Thickness(0);

            imageContainer.Source = source;
            OnImageChoose?.Invoke(this, fileName);
        }

        private async void OnAlert()
        {
            if (PopupHost is null)
            {
                MessageBox.Show("Please choose square image", "Load Failed", MessageBoxButton.OK);
                return;
            }

            SimpleDialog dialog = new()
            {
                Title = "Load failed",
                Content = "Please choose square image",
                Width = 300,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }
    }
}
