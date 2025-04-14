using SGULibraryManagement.Components.SideMenu;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SGULibraryManagement.GUI
{
    public partial class MainView : UserControl
    {
        private UserControl? currentContent;

        public MainView()
        {
            InitializeComponent();
            Navigate(SideMenuHomeItem, null!);
        }

        private void Navigate(object sender, MouseButtonEventArgs e)
        {
            SideMenuItem item = (SideMenuItem)sender;

            if (currentContent == item.ContentView) return;

            OnChangeContent();
            item.IsSelected = true;

            currentContent = item.ContentView;
            content.Navigate(currentContent);

            PlayNavigateAnimation();
        }

        private void OnChangeContent()
        {
            var list = sideMenu.Children;

            foreach (var child in list)
            {
                if (child is not SideMenuItem) continue;
                var sideMenuItem = (SideMenuItem)child;

                sideMenuItem.IsSelected = false;
            }
        }

        private void OnLogout(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance!.Logout();
        }

        private void PlayNavigateAnimation()
        {
            if (FindResource("ContentFadeIn") is Storyboard storyboard)
            {
                storyboard.Begin();
            }
        }
    }
}
