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

            if (currentContent == item.ContentView) return;

            OnChangeContent();
            item.IsSelected = true;

            currentContent = item.ContentView;
            if (currentContent is DashboardView view) view.Fetch();

            content.Navigate(currentContent);

            PlayNavigateAnimation();
        }

        public void Navigate<TContent>() where TContent : IContent
        {
            var list = sideMenu.Children;

            foreach (var child in list)
            {
                if (child is not SideMenuItem sideMenuItem) continue;
                if (sideMenuItem.ContentView is not TContent) continue;

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

        /// <summary>
        /// Refetch all content view in types, if types is null, refetch all
        /// </summary>
        /// <param name="types"></param>
        public void FetchAll(List<Type>? types = null)
        {
            var list = sideMenu.Children;
            List<string> typeNames = types != null ? [.. types.Select(type => type.Name)] : [];

            foreach (var child in list)
            {
                if (child is not SideMenuItem sideMenuItem) continue;
                if (sideMenuItem.ContentView is not IContent content) continue;

                if (typeNames.Count == 0) content.Fetch();
                else if (typeNames.Contains(content.GetType().Name)) content.Fetch();
            }
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
