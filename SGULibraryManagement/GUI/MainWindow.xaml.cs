using SGULibraryManagement.Components.SideMenu;
using SGULibraryManagement.Config;
using SGULibraryManagement.Utilities;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;
namespace SGULibraryManagement.GUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    private UserControl? currentContent;
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Navigate(SideMenuHomeItem, null!);

        Instance ??= this;
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

    private void PlayNavigateAnimation()
    {
        if (FindResource("ContentFadeIn") is Storyboard storyboard)
        {
            storyboard.Begin();
        }
    }

    public async Task<SimpleDialogResult> ShowSimpleDialogAsync(object content, string title, SimpleDialogType options, double width, double height)
    {
        ContentDialog dialog = new(dialogHost)
        {
            Content = content,
            Title = title,
            DialogWidth = width,
            DialogHeight = height,
            DialogMaxWidth = width,
            DialogMaxHeight = height,
        };

        SimpleDialogResult firstChoice, secondChoice;

        switch (options)
        {
            case SimpleDialogType.OK:
                dialog.CloseButtonText = "OK";
                firstChoice = GUI.SimpleDialogResult.OK;
                secondChoice = GUI.SimpleDialogResult.OK;
                break;

            case SimpleDialogType.OKCancel:
                dialog.PrimaryButtonText = "OK";
                dialog.CloseButtonText = "Cancel";
                firstChoice = GUI.SimpleDialogResult.OK;
                secondChoice = GUI.SimpleDialogResult.Cancel;
                break;

            case SimpleDialogType.YesNo:
                dialog.PrimaryButtonText = "Yes";
                dialog.CloseButtonText = "No";

                firstChoice = SimpleDialogResult.Yes;
                secondChoice = SimpleDialogResult.No;
                break;

            default:
                return SimpleDialogResult.Cancel;
        }

        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.None => secondChoice,
            ContentDialogResult.Primary => firstChoice,
            _ => SimpleDialogResult.Cancel,
        };
    }
}

public enum SimpleDialogType
{
    OK,
    OKCancel,
    YesNo
}

public enum SimpleDialogResult
{
    OK,
    Cancel,
    Yes,
    No
}