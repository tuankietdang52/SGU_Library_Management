using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.Components.TextFields;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.DialogGUI;
using SGULibraryManagement.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class UsersView : UserControl
    {
        private readonly AccountBUS userBUS = new();
        private Action<UserFilter?>? searchDebounce;

        public ObservableCollection<AccountDTO> Users { get; set; } = [];

        public UsersView()
        {
            InitializeComponent();
            Fetch();
            SetupComponent();
        }

        private void Fetch()
        {
            Users.ResetTo(userBUS.GetAll());
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

            statusComboBox.ItemsSource = new List<string>()
            {
                "All",
                "Available",
                "Not Available"
            };
            statusComboBox.SelectedIndex = 0;
        }

        private UserFilter? GetFilter()
        {
            //if (searchByComboBox.SelectedItem is not UserQueryOption queryOption) return null;
            //if (statusComboBox.SelectedItem is not string status) return null;

            //return new UserFilter()
            //{
            //Query = searchField.Text,
            //    UserQueryOption = queryOption,
            //    Status = status
            //};
            return null;
        }

        private void OnApplyFilter(UserFilter? filter)
        {
            //if (filter == null) return;

            //var result = _userBUS.FilterByQuery(filter.Query, filter.UserQueryOption);

            //if (filter.Status != "All")
            //{
            //    result = _userBUS.FilterByStatus(filter.Status == "Available", result);
            //}

            //App.Instance!.InvokeInMainThread(() => Users.ResetTo(result));
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;

            UserFilter? filter = GetFilter();
            searchDebounce(filter);
        }

        private void OnSearchByChanged(object sender, SelectionChangedEventArgs e)
        {
            UserFilter? filter = GetFilter();
            OnApplyFilter(filter);
        }

        private void OnStatusChanged(object sender, SelectionChangedEventArgs e)
        {
            UserFilter? filter = GetFilter();
            OnApplyFilter(filter);
        }

        private void ActionColumn_OnEditClick(object sender, object model)
        {
            Dialog dialog = new("Update user", new UserDialog("update", (AccountDTO)model));
            dialog.ShowDialog();
            Fetch();
        }

        private void AddUserAction(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new("Add new User", new UserDialog("create", null));
            dialog.ShowDialog();
            Fetch();

        }

        private void ActionColumn_OnViewClick(object sender, object model)
        {
            Dialog dialog = new("View user", new UserDialog("view", (AccountDTO)model));
            dialog.ShowDialog();
            Fetch();
        }

        private async void ActionColumn_OnDeleteClick(object sender, object model)
        {
            if (model is not AccountDTO user) return;

            SimpleDialog dialog = new()
            {
                Content = "Are you really want to delete this user ?",
                Title = "Delete",
                Width = 400,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK);
            if (result == SimpleDialogResult.OK)
            {
                userBUS.DeleteAccount(user.Username);
            }
            else return;
            Fetch();
        }

        private class UserFilter
        {
            public string Query { get; set; } = string.Empty;
            public string Status { get; set; } = "All";
            public UserQueryOption UserQueryOption { get; set; }
        }
    }
}
