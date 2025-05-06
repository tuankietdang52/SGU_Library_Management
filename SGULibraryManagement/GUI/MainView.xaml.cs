using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.SideMenu;
using SGULibraryManagement.GUI.Contents;
using SGULibraryManagement.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SGULibraryManagement.GUI
{
    public partial class MainView : UserControl
    {
        private UserControl? currentContent;

        private static MainView? instance;
        public static MainView Instance
        {
            get => instance ?? new MainView();
            set => instance = value;
        }

        public MainView()
        {
            InitializeComponent();
            Navigate(SideMenuHomeItem, null!);

            Instance = this;
            RefreshCurrentUser();
        }

        private void Navigate(object sender, MouseButtonEventArgs e)
        {
            SideMenuItem item = (SideMenuItem)sender;

            var nextContent = (UserControl)Activator.CreateInstance(item.ContentView)!;
            if (nextContent is null) return;

            if (currentContent?.GetType() == nextContent.GetType()) return;

            OnChangeContent();
            item.IsSelected = true;

            currentContent = nextContent;
            content.Navigate(currentContent);

            PlayNavigateAnimation();
        }

        public void Navigate<TContent>() where TContent : IContent
        {
            var list = sideMenu.Children;

            foreach (var child in list)
            {
                if (child is not SideMenuItem sideMenuItem) continue;
                if (sideMenuItem.ContentView != typeof(TContent)) continue;

                sideMenuItem.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = MouseDownEvent
                });
            }
        }

        public void Navigate(UserControl contentView)
        {
            OnChangeContent();

            currentContent = contentView;
            content.Navigate(currentContent);

            PlayNavigateAnimation();
        }

        public void RefreshCurrentUser()
        {
            var current = AccountManager.CurrentUser!;

            currentUserName.Text = current.FullName;

            var role = new RoleBUS().FindById(current.IdRole);
            currentUserRole.Text = role.Name;

            try
            {
                userAvatar.Source = new BitmapImage(new Uri(current.Avatar));
            }
            catch { }
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

        private void OnCurrentUserButtonClick(object sender, RoutedEventArgs e)
        {
            if (currentContent is UserInformationView) return;

            UserInformationView view = new();
            Navigate(view);
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
