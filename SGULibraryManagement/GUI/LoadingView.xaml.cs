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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SGULibraryManagement.GUI
{
    public partial class LoadingView : UserControl
    {
        public event EventHandler? ShowCompleted;
        public event EventHandler? HideCompleted;

        public LoadingView()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            if (FindResource("LoadingFadeIn") is Storyboard storyboard)
            {
                storyboard.Completed += (sender, e) =>
                {
                    ShowCompleted?.Invoke(sender, e);
                };
                storyboard.Begin();
            }
        }

        public void Hide()
        {
            if (FindResource("LoadingFadeOut") is Storyboard storyboard)
            {
                storyboard.Completed += (sender, e) =>
                {
                    Visibility = Visibility.Collapsed;
                    HideCompleted?.Invoke(sender, e);
                };
                storyboard.Begin();
            }
        }
    }
}