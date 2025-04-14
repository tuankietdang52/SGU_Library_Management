using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.Components.SideMenu;
using SGULibraryManagement.Config;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;
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
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance ??= this;

        ShowLogin();
    }

    private void ShowLogin()
    {
        var loginView = new LoginView();
        loginView.LoginSuccess += OnLoginSuccess;
        loginView.Name = "loginView";

        windowContent.Content = loginView;
    }

    public async void Logout()
    {
        ShowLoadingView(() =>
        {
            ShowLogin();
            AccountManager.CurrentUser = null;
        });
        await Task.Delay(1000);
        HideLoadingView();
    }

    private async void OnLoginSuccess(object sender, AccountDTO account)
    {
        AccountManager.CurrentUser = account;
        MainView view = new();

        ShowLoadingView(() => windowContent.Content = view);
        await Task.Delay(1000);
        HideLoadingView();
    }

    public void ShowLoadingView(Action? loadingTask = null)
    {
        if (loadingTask is not null)
        {
            void LoadingTask(object? sender, EventArgs e)
            {
                loadingTask();
                loadingView.ShowCompleted -= LoadingTask;
            }

            loadingView.ShowCompleted += LoadingTask;
        }

        loadingView.Show();
    }

    public void HideLoadingView()
    {
        loadingView.Hide();
    }

    public async Task<SimpleDialogResult> ShowSimpleDialogAsync(SimpleDialog simpleDialog, SimpleDialogType options, ContentPresenter? host = null)
    {
        ContentDialog dialog = new(host ?? dialogHost)
        {
            Content = simpleDialog.Content,
            Title = simpleDialog.Title,
            DialogWidth = simpleDialog.Width,
            DialogHeight = simpleDialog.Height,
            DialogMaxWidth = simpleDialog.Width,
            DialogMaxHeight = simpleDialog.Height,
        };

        SimpleDialogResult firstChoice, secondChoice;

        switch (options)
        {
            case SimpleDialogType.OK:
                dialog.CloseButtonText = "OK";
                dialog.CloseButtonAppearance = ControlAppearance.Primary;
                firstChoice = SimpleDialogResult.OK;
                secondChoice = SimpleDialogResult.OK;
                break;

            case SimpleDialogType.OKCancel:
                dialog.PrimaryButtonText = "OK";
                dialog.CloseButtonText = "Cancel";
                firstChoice = SimpleDialogResult.OK;
                secondChoice = SimpleDialogResult.Cancel;
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