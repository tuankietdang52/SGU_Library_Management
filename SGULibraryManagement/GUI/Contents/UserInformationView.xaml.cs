using System.Windows;
using System.Windows.Controls;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Helper;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class UserInformationView : UserControl
    {
        private AccountDTO? Current => AccountManager.CurrentUser;
        private readonly AccountBUS accountBUS = new();
        private readonly RoleBUS roleBUS = new();

        private string avatarFilePath = string.Empty;

        public UserInformationView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (Current is null) return;

            firstNameField.Text = Current.FirstName;
            lastNameField.Text = Current.LastName;
            usernameField.Text = Current.Username;
            passwordField.Password = Current.Password;
            emailField.Text = Current.LastName;
            phoneField.Text = Current.Phone;
            roleLabel.Text = roleBUS.FindById(Current.IdRole).Name;
            imageChooser.FilePath = Current.Avatar;

            avatarFilePath = Current.Avatar;
        }

        private void OnImageChoose(object sender, string filePath)
        {
            avatarFilePath = filePath;
        }

        private AccountDTO GatherData()
        {
            return new AccountDTO()
            {
                Id = Current!.Id,
                FirstName = firstNameField.Text,
                LastName = lastNameField.Text,
                Username = usernameField.Text,
                Password = passwordField.Password,
                Email = emailField.Text,
                Phone = phoneField.Text,
                IdRole = Current!.IdRole,
                Avatar = avatarFilePath
            };
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            var model = GatherData();

            if (accountBUS.UpdateAccount(Current!.Id, model))
            {
                OnSuccess();
            }
            else OnFail();
        }

        private async void OnSuccess()
        {
            SimpleDialog dialog = new()
            {
                Title = "Success",
                Content = "Save Successfully!",
                Width = 400,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK);

            AccountManager.CurrentUser = accountBUS.FindById(Current!.Id);
            MainView.Instance.RefreshCurrentUser();
            MainView.Instance.Navigate(new UserInformationView());
        }

        private async void OnFail()
        {
            SimpleDialog dialog = new()
            {
                Title = "Failed",
                Content = "Save Failed",
                Width = 400,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK);
        }
    }
}
