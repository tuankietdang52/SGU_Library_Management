using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.Components.TextFields;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.DialogGUI;
using SGULibraryManagement.GUI.ViewModels;
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
        private readonly RoleBUS roleBUS = new();

        private Action<UserFilter?>? searchDebounce;

        public ObservableCollection<AccountViewModel> Users { get; set; } = [];

        public UsersView()
        {
            InitializeComponent();
            Fetch();
            SetupComponent();
        }

        private void Fetch()
        {
            Users.ResetTo(userBUS.GetAllWithRole());
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
        }

        private UserFilter? GetFilter()
        {
            if (searchByComboBox.SelectedItem is not UserQueryOption queryOption) return null;
            if (roleComboBox.SelectedItem is not RoleDTO role) return null;

            return new UserFilter()
            {
                Query = searchField.Text,
                UserQueryOption = queryOption,
                Role = role
            };
        }

        private void OnApplyFilter(UserFilter? filter)
        {
            if (filter == null) return;

            var result = userBUS.FilterByQuery(filter.Query, filter.UserQueryOption);

            if (filter.Role is not null && filter.Role.Id != -1)
            {
                result = userBUS.FilterByRole(filter.Role, result);
            }

            App.Instance!.InvokeInMainThread(() => Users.ResetTo(result));
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

        private void OnRoleChanged(object sender, SelectionChangedEventArgs e)
        {
            UserFilter? filter = GetFilter();
            OnApplyFilter(filter);
        }

        private void OnEditClick(object sender, object model)
        {
            Dialog dialog = new("Update user", new UserDialog("update"));
            dialog.ShowDialog();
            Fetch();
        }

        private void AddUserAction(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new("Add new User", new UserDialog("create"));
            dialog.ShowDialog();
            Fetch();
        }

        private void OnViewClick(object sender, object model)
        {
            Dialog dialog = new("View user", new UserDialog("view"));
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
            public RoleDTO? Role { get; set; }
            public UserQueryOption UserQueryOption { get; set; }
        }
    }
}
