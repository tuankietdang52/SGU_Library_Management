using Microsoft.Win32;
using OfficeOpenXml;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.DialogGUI;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using LicenseContext = OfficeOpenXml.LicenseContext;


namespace SGULibraryManagement.GUI.Contents
{
    public partial class UsersView : UserControl, IContent
    {
        private readonly AccountBUS userBUS = new();
        private readonly RoleBUS roleBUS = new();

        private Action<UserFilter?>? searchDebounce;

        public ObservableCollection<AccountViewModel> Users { get; set; } = [];

        public UsersView()
        {
            InitializeComponent();
            SetupComponent();
            Fetch();
        }

        public void Fetch()
        {
            _ = userBUS.GetAllWithRole();
            OnApplyFilter(GetFilter());
        }

        private void RenderTable(List<AccountViewModel>? collections = null)
        {
            var list = collections ?? userBUS.GetAllWithRole();
            AccountDTO currentAccount = AccountManager.CurrentUser!;

            list.RemoveAll(vm => vm.Account.Mssv == currentAccount.Mssv);
            App.Instance!.InvokeInMainThread(() => Users.ResetTo(list));
        }

        private void SetupComponent()
        {
            DataContext = this;
            SetupSearchAndFilter();
        }

        private void SetupSearchAndFilter()
        {
            searchDebounce = ((Action<UserFilter?>)(OnApplyFilter)).Debounce(200);

            searchByComboBox.ItemsSource = Enum.GetValues<UserQueryOption>();
            searchByComboBox.SelectedIndex = 0;

            List<RoleDTO> roles = [new RoleDTO() {
                Id = -1,
                Name = "All",
                IsDeleted = false,
            }];
            roles.AddRange(roleBUS.GetAll());

            roleComboBox.ItemsSource = roles;
            roleComboBox.SelectedIndex = 0;

            statusComboBox.ItemsSource = new List<string>() { "All", "Locked", "Normal" };
            statusComboBox.SelectedIndex = 0;
        }

        private UserFilter? GetFilter()
        {
            if (searchByComboBox.SelectedItem is not UserQueryOption queryOption) return null;
            if (roleComboBox.SelectedItem is not RoleDTO role) return null;
            if (statusComboBox.SelectedItem is not string status) return null;

            return new UserFilter()
            {
                Query = searchField.Text,
                UserQueryOption = queryOption,
                Role = role,
                Status = status
            };
        }

        private void OnApplyFilter(UserFilter? filter)
        {
            if (filter is null) return;

            var result = userBUS.FilterByQuery(filter.Query, filter.UserQueryOption);

            if (filter.Role is not null && filter.Role.Id != -1)
            {
                result = userBUS.FilterByRole(filter.Role, result);
            }

            result = userBUS.FilterByLockStatus(filter.Status, result);
            RenderTable(result);
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;

            UserFilter? filter = GetFilter();
            searchDebounce(filter);
        }

        private void OnFilterCbChanged(object sender, SelectionChangedEventArgs e)
        {
            UserFilter? filter = GetFilter();
            OnApplyFilter(filter);
        }

        private void AddUserAction(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new("Add new User", new UserDialog());
            dialog.ShowDialog();
            Fetch();

        }

        private void ImportExcel(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Excel Files|*.xlsx"
            };

            if (openFileDialog.ShowDialog() != true) return;

            var listAccount = Importing(openFileDialog);

            if (userBUS.CreateListAccount(listAccount) == false)
            {
                MessageBox.Show("Import failed");
                return;
            }

            MessageBox.Show("Import successfully!");
            MainView.Instance.FetchAll([typeof(UsersView), typeof(ViolationView)]);
        }

        private List<AccountDTO> Importing(OpenFileDialog openFileDialog)
        {
            string filePath = openFileDialog.FileName;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Xử lý file Excel
            using var excelPackage = new ExcelPackage(new FileInfo(filePath));
            var worksheet = excelPackage.Workbook.Worksheets[0];
            int startRow = worksheet.Dimension.Start.Row + 1;
            int endRow = worksheet.Dimension.End.Row;

            List<AccountDTO> listAccount = [];
            for (int i = startRow; i <= endRow; i++)
            {
                List<string> rowData = [];
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    rowData.Add(worksheet.Cells[i, j].Text);
                }
                try
                {
                    listAccount.Add(new AccountDTO()
                    {
                        Mssv = long.Parse(rowData[0]),
                        Password = rowData[1],
                        FirstName = rowData[2],
                        LastName = rowData[3],
                        Phone = rowData[4],
                        Email = rowData[5],
                        IdRole = int.Parse(rowData[6]),
                        Faculty = rowData[7],
                        Major = rowData[8],
                        IsDeleted = false,
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show("One or more object in excel does not match with account object");
                    return [];
                }
            }

            return listAccount;
        }


        private void OnViewClick(object sender, object model)
        {
            Dialog dialog = new("View user", new UserDialog(EDialogType.View, (AccountDTO)model));
            dialog.ShowDialog();
            Fetch();
        }

        private void OnEditClick(object sender, object model)
        {
            Dialog dialog = new("Update user", new UserDialog(EDialogType.Edit, (AccountDTO)model));
            dialog.ShowDialog();
            Fetch();
        }

        private async void OnDeleteClick(object sender, object model)
        {
            if (model is not AccountDTO user) return;

            SimpleDialog dialog = new()
            {
                Content = $"Are you really want to delete {user.FullName} ?",
                Title = $"Delete {user.FullName}",
                Width = 400,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.YesNo);
            if (result == SimpleDialogResult.Yes)
            {
                userBUS.DeleteAccount(user);
            }
            else return;

            Fetch();
        }

        private class UserFilter
        {
            public string Query { get; set; } = string.Empty;
            public RoleDTO? Role { get; set; }
            public UserQueryOption UserQueryOption { get; set; }
            public string Status { get; set; } = "All";
        }
    }
}
