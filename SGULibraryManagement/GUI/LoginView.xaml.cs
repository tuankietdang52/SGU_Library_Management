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
        private readonly AccountViolationBUS accountViolationBUS = new();

        private Dictionary<string, AccountDTO>? accounts;

        public event OnLoginSuccessHandler? LoginSuccess;

        public LoginView()
        {
            InitializeComponent();
            Fetch();
        }

        private void Fetch()
        {
            accounts = accountBUS.GetAll().ToDictionary(account => account.Mssv.ToString());
        }

        private bool IsLocked(AccountDTO account)
        {
            return accountViolationBUS.IsAccountLocked(account, out var _);
        }

        private Result ValidateAccount(string mssv, string password)
        {
            Fetch();
            string wrongMessage = "Wrong username or password";

            if (!accounts!.TryGetValue(mssv, out var account)) return new Result(false, wrongMessage);

            if (long.Parse(mssv) == account.Mssv && password.Equals(account.Password))
            {
                if (account.IdRole != 1) return new Result(false, "This account is not admin");
                if (IsLocked(account)) return new Result(false, "This account is locked");

                return new Result(true, "");
            }

            return new Result(false, wrongMessage);
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            string mssv = usernameField.Text;
            string password = passwordField.Password;

            var result = ValidateAccount(mssv, password);

            if (result.Value) OnLoginSuccess(accounts![mssv]);
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
            passwordField.Focus();
        }

        private void OnLoginSuccess(AccountDTO account)
        {
            loginButton.IsEnabled = false;
            LoginSuccess?.Invoke(this, account);
        }
    }
}
