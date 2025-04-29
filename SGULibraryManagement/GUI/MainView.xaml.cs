using SGULibraryManagement.Components.SideMenu;
using SGULibraryManagement.GUI.Contents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SGULibraryManagement.GUI
{
    public partial class MainView : UserControl
    {
        private UserControl? currentContent;

        private static MainView? mainView;
        public static MainView Instance
        {
            get => mainView ?? new MainView();
            set => mainView = value;
        }

        public MainView()
        {
            InitializeComponent();
            Navigate(SideMenuHomeItem, null!);

            Instance = this;
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

        public void Navigate(UserControl contentView)
        {
            OnChangeContent();

            currentContent = contentView;
            content.Navigate(currentContent);

            PlayNavigateAnimation();
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
                if (child is not IContent) continue;
                var content = (IContent)child;

                if (typeNames.Count == 0) content.Fetch();
                else if (typeNames.Contains(child.GetType().Name)) content.Fetch();
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
