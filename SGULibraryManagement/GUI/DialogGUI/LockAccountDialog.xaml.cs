using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.Contents;
using SGULibraryManagement.Utilities;
using System.Windows;
using System.Windows.Controls;
using Logger = SGULibraryManagement.Utilities.Logger;

namespace SGULibraryManagement.GUI.DialogGUI
{
    public partial class LockAccountDialog : UserControl, IDialog
    {
        private readonly ViolationBUS violationBUS = new();
        private readonly AccountBUS accountBUS = new();
        private readonly AccountViolationBUS accountViolationBUS = new();
        private bool isBanEternal = false;
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

            banExpiredDatePicker.DisplayDateStart = DateTime.Now;
            banExpiredDatePicker.SelectedDate = DateTime.Now;

            statusCB.ItemsSource = Enum.GetValues<AccountViolationStatus>();
            statusCB.SelectedIndex = 0;
        }

        private void SetupUpdateComponent()
        {
            if (accountViolation is null) return;

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

            banExpiredDatePicker.SelectedDate = accountViolation.BanExpired;
            compensationTB.Value = accountViolation.Compensation;
            statusCB.SelectedItem = accountViolation.Status;

            checkbox.Checked -= OnChecked;
            if (accountViolation.IsBanEternal.HasValue && accountViolation.IsBanEternal.Value)
            {
                isBanEternal = true;
                checkbox.IsChecked = isBanEternal;
                banExpiredDatePicker.IsEnabled = false;
                banExpiredDatePicker.SelectedDate = null;
            }

            checkbox.Checked += OnChecked;

            lockButton.Visibility = Visibility.Collapsed;
            updateButtonContainer.Visibility = Visibility.Visible;
        }

        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            banExpiredDatePicker.SelectedDate ??= DateTime.Now;
        }

        private void OnViolationRuleChanged(object sender, SelectionChangedEventArgs e)
        {
            if (violationCB.SelectedItem is not ViolationDTO violation) return;
            violationDescription.Text = violation.Description;
        }


        private void OnChecked(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                isBanEternal = true;
                banExpiredDatePicker.IsEnabled = false;
                banExpiredDatePicker.SelectedDate = null;
            }
            else
            {
                checkbox.IsChecked = false;
                isBanEternal = false;
                banExpiredDatePicker.IsEnabled = true;

            }
        }

        private void OnUnChecked(object sender, EventArgs e)
        {
            isBanEternal = false;
            banExpiredDatePicker.IsEnabled = true;

        }

        private async void OnLockAccount(object sender, RoutedEventArgs e)
        {
            if (!await LockingAccount()) return;
            OnCloseDialog?.Invoke(this);
        }

        private async void OnUpdateLockAccount(object sender, RoutedEventArgs e)
        {
            if (!await SavingLockAccount()) return;
            OnCloseDialog?.Invoke(this);
        }

        private AccountViolationDTO? GatherDataBanEternal()
        {
            if (violationCB.SelectedItem is not ViolationDTO violation) return null;
            if (statusCB.SelectedItem is not AccountViolationStatus status) return null;

            return new()
            {
                UserId = account.Mssv,
                ViolationId = violation.Id,
                DateCreate = DateTime.Now,
                Compensation = long.Parse(compensationTB.Text),
                Status = status,
                IsDeleted = false , 
                IsBanEternal = true
            };
        }
        private AccountViolationDTO? GatherData()
        {
            if (violationCB.SelectedItem is not ViolationDTO violation) return null;
            if (statusCB.SelectedItem is not AccountViolationStatus status) return null;

            return new()
            {
                UserId = account.Mssv,
                ViolationId = violation.Id,
                DateCreate = DateTime.Now,
                BanExpired = banExpiredDatePicker.SelectedDate!.Value,
                Compensation = long.Parse(compensationTB.Text),
                Status = status,
                IsDeleted = false,
                IsBanEternal = false
            };
        }

        private async Task<bool> LockingAccount()
        {
            Logger.Log($"is ban eternal : {isBanEternal}");
            var model = isBanEternal ? GatherDataBanEternal() : GatherData();
            if (model is null) return false;

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
            if (accountViolation is null) return false;

            var model = isBanEternal ? GatherDataBanEternal() : GatherData();
            if (model is null) return false;

            model.DateCreate = accountViolation.DateCreate;
            if (accountViolationBUS.Update(accountViolation.Id, model))
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
