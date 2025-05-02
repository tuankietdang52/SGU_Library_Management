using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class CirculationView : UserControl, IContent
    {
        public ObservableCollection<BorrowDeviceViewModel> BorrowDevices { get; set; } = [];
        private readonly BorrowDevicesBUS BUS = new();
        private readonly AccountViolationBUS accountViolationBUS = new();
        private readonly ViolationBUS violationBUS = new();

        public ICommand? LockCommand { get; set; }

        private Action<BorrowDeviceFilter?>? searchDebounce;

        public CirculationView()
        {
            InitializeComponent();
            SetupComponent();
            Fetch();
        }

        public void Fetch()
        {
            _ = BUS.GetAllWithDetail();
            OnApplyFilter(GetFilter());
        }

        private void RenderTable(IEnumerable<BorrowDeviceViewModel> collections)
        {
            App.Instance!.InvokeInMainThread(() => BorrowDevices.ResetTo(collections));
        }

        private void SetupComponent()
        {
            DataContext = this;
            LockCommand = new RelayCommand<AccountDTO>(OnLockUser!);

            searchDebounce = ((Action<BorrowDeviceFilter?>)(OnApplyFilter)).Debounce(200);

            searchByComboBox.ItemsSource = new List<string>
            {
                "Device Name",
                "User Email"
            };
            searchByComboBox.SelectedIndex = 0;

            statusComboBox.ItemsSource = new List<string> { "All", "Return", "Not Return", "Not yet due" };
            statusComboBox.SelectedIndex = 0;
        }

        private BorrowDeviceFilter? GetFilter()
        {
            if (searchByComboBox.SelectedItem is not string searchBy) return null;
            if (statusComboBox.SelectedItem is not string status) return null;

            return new BorrowDeviceFilter()
            {
                Query = searchField.Text,
                SearchBy = searchBy,
                Status = status
            };
        }

        private void OnApplyFilter(BorrowDeviceFilter? filter)
        {
            if (filter is null) return;

            var result = BUS.FilterByQuery(filter.Query, filter.SearchBy);
            result = BUS.FilterByStatus(filter.Status, result);

            RenderTable(result);
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;

            var filter = GetFilter();
            searchDebounce(filter);
        }

        private void OnFilterCbChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = GetFilter();
            OnApplyFilter(filter);
        }

        private async void OnLockUser(AccountDTO account)
        {
            if (await LockingUser(account))
            {
                MainView.Instance.FetchAll([typeof(UsersView)]);
            }
        }

        private async Task<bool> LockingUser(AccountDTO account)
        {
            if (accountViolationBUS.IsAccountLocked(account, out var accountViolation))
            {
                ChangeUserViolation(account, accountViolation);
                return true;
            }

            SimpleDialog alreadyLockedDialog = new()
            {
                Title = "Lock User",
                Content = "Do you want to lock this user for not return device on time ?",
                Width = 300,
                Height = 300
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(alreadyLockedDialog, SimpleDialogType.YesNo);
            AccountViolationDTO violation = new()
            {
                UserId = account.Id,
                ViolationId = 1,
                DateCreate = DateTime.Now,
                IsDeleted = false
            };

            if (result == SimpleDialogResult.Yes)
            {
                return accountViolationBUS.Create(violation) is not null;
            }
            else return false;
        }

        private async void ChangeUserViolation(AccountDTO account, AccountViolationDTO accountViolation)
        {
            var mainWindow = MainWindow.Instance!;

            if (accountViolation.ViolationId == 1)
            {
                SimpleDialog alreadyLockedDialog = new()
                {
                    Title = "Already Locked",
                    Content = "This user already locked for not return device on time",
                    Width = 300,
                    Height = 300
                };

                await mainWindow.ShowSimpleDialogAsync(alreadyLockedDialog, SimpleDialogType.OK);
                return;
            }

            var violation = violationBUS.FindById(accountViolation.ViolationId);

            SimpleDialog changeDialog = new()
            {
                Title = "Change User Violation",
                Content = $"This user is currently lock for {violation.Name}. Do you want to change this user violation to Not Return Device ?",
                Width = 400,
                Height = 300
            };

            var result = await mainWindow.ShowSimpleDialogAsync(changeDialog, SimpleDialogType.YesNo);
            AccountViolationDTO newViolation = new()
            {
                UserId = account.Id,
                ViolationId = 1,
                DateCreate = DateTime.Now,
                IsDeleted = false,
            };

            if (result == SimpleDialogResult.Yes)
            {
                accountViolationBUS.ChangeViolation(accountViolation.Id, newViolation);
            }
        }
    }

    public class BorrowDeviceFilter
    {
        public required string Query { get; set; }
        public required string SearchBy { get; set; }
        public required string Status { get; set; }
    }
}
