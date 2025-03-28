using SGULibraryManagement.Components.SideMenu;
using SGULibraryManagement.Config;
using SGULibraryManagement.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace SGULibraryManagement.GUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private UserControl? currentContent;

    public MainWindow()
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
}