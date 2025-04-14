using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
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

namespace SGULibraryManagement.GUI
{
    public delegate void OnLoginSuccessHandler(object sender, AccountDTO account);

    public partial class LoginView : UserControl
    {
        private readonly AccountBUS accountBUS = new();
        private readonly Dictionary<string, AccountDTO> accounts;

        public event OnLoginSuccessHandler? LoginSuccess;

        public LoginView()
        {
            InitializeComponent();
            accounts = accountBUS.GetAll().ToDictionary(account => account.Username);
        }

        private Result ValidateAccount(string username, string password)
        {
            string failMessage = "Wrong username or password";

            if (!accounts.TryGetValue(username, out var account)) return new Result(false, failMessage);

            if (username.Equals(account.Username) && password.Equals(account.Password))
            {
                return new Result(true, "");
            }

            return new Result(false, failMessage);
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            string username = usernameField.Text;
            string password = passwordField.Password;

            var result = ValidateAccount(username, password);

            if (result.Value) OnLoginSuccess(accounts[username]);
            else OnLoginFail(result);
        }

        private async void OnLoginFail(Result result)
        {
            SimpleDialog dialog = new()
            {
                Title = "Login Failed",
                Content = result.Message,
                Width = 300,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK);
        }

        private void OnLoginSuccess(AccountDTO account)
        {
            LoginSuccess?.Invoke(this, account);
        }
    }
}
