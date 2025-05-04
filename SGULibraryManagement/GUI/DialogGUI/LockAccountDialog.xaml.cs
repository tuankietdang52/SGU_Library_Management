using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.Contents;
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

namespace SGULibraryManagement.GUI.DialogGUI
{
    public partial class LockAccountDialog : UserControl, IDialog
    {
        private readonly ViolationBUS violationBUS = new();
        private readonly AccountBUS accountBUS = new();
        private readonly AccountViolationBUS accountViolationBUS = new();

        public ContentPresenter? PopupHost { get; set; }

        public event OnCloseDialogHandler? OnCloseDialog;

        private readonly AccountDTO account;
        private readonly AccountViolationDTO? accountViolation;

        public LockAccountDialog(AccountDTO account)
        {
            InitializeComponent();

            this.account = account;
            SetupComponent();
        }

        public LockAccountDialog(AccountDTO account, AccountViolationDTO accountViolation) : this(account)
        {
            this.accountViolation = accountViolation;
            SetupUpdateComponent();
        }

        private void SetupComponent()
        {
            title.Text = $"Lock {account.Mssv} for ";

            violationCB.ItemsSource = violationBUS.GetAll();
            violationCB.DisplayMemberPath = "Name";
            violationCB.SelectedIndex = 0;
        }

        private void SetupUpdateComponent()
        {
            title.Text = $"{account.Mssv} is currently lock for ";

            foreach(var item in violationCB.ItemsSource)
            {
                if (item is not ViolationDTO violation) continue;
                if (violation.Id == accountViolation!.ViolationId)
                {
                    violationCB.SelectedItem = item;
                    break;
                }
            }

            lockButton.Visibility = Visibility.Collapsed;
            updateButtonContainer.Visibility = Visibility.Visible;
        }

        private void OnViolationRuleChanged(object sender, SelectionChangedEventArgs e)
        {
            if (violationCB.SelectedItem is not ViolationDTO violation) return;
            violationDescription.Text = violation.Description;
        }

        private async void OnUnlockAccount(object sender, RoutedEventArgs e)
        {
            if (accountViolation is null) return;

            SimpleDialog dialog = new()
            {
                Title = $"Unlock {account.Mssv}",
                Content = "Do you want to unlock this account ?",
                Width = 300,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.YesNo, PopupHost);

            if (result == SimpleDialogResult.Yes)
            {
                accountViolationBUS.Delete(accountViolation.Id);
                OnCloseDialog?.Invoke(this);
            }
        }

        private async void OnLockAccount(object sender, RoutedEventArgs e)
        {
            if (!await LockingAccount()) return;

            List<Type> fetchTarget =
            [
                typeof(UsersView),
                typeof(ViolationView)
            ];

            MainView.Instance.FetchAll(fetchTarget);
            OnCloseDialog?.Invoke(this);
        }

        private async void OnUpdateLockAccount(object sender, RoutedEventArgs e)
        {
            if (!await SavingLockAccount()) return;

            List<Type> fetchTarget =
            [
                typeof(UsersView),
                typeof(ViolationView)
            ];

            MainView.Instance.FetchAll(fetchTarget);
            OnCloseDialog?.Invoke(this);
        }

        private async Task<bool> LockingAccount()
        {
            if (violationCB.SelectedItem is not ViolationDTO violation) return false;

            AccountViolationDTO model = new()
            {
                UserId = account.Mssv,
                ViolationId = violation.Id,
                DateCreate = DateTime.Now,
                IsDeleted = false
            };

            if (accountViolationBUS.Create(model) is not null)
            {
                await OnLockSucess();
                return true;
            }

            await OnLockFailed();
            return false;
        }

        private async Task<bool> SavingLockAccount()
        {
            if (violationCB.SelectedItem is not ViolationDTO violation) return false;
            if (accountViolation is null) return false;


            accountViolation.ViolationId = violation.Id;
            if (accountViolationBUS.Update(accountViolation.Id, accountViolation))
            {
                await OnLockSucess();
                return true;
            }

            await OnLockFailed();
            return false;
        }

        private async Task OnLockSucess()
        {
            SimpleDialog dialog = new()
            {
                Title = "Success",
                Content = $"Lock {account.Mssv} successful!",
                Width = 300,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }

        private async Task OnLockFailed()
        {
            SimpleDialog dialog = new()
            {
                Title = "Failed",
                Content = $"Lock {account.Mssv} fail",
                Width = 300,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }
    }
}
