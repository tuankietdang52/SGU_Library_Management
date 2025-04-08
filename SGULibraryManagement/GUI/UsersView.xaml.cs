using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.TextFields;
using SGULibraryManagement.DTO;
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
        private readonly UserBUS _userBUS = new();
        private Action<string, UserQueryOption>? searchFunction;

        public ObservableCollection<UserDTO> Users { get; set; } = [];

        public UsersView()
        {
            InitializeComponent();
            Fetch();
            SetupComponent();
        }

        private void Fetch()
        {
            Users.ResetTo(_userBUS.Users);
        }

        private void SetupComponent()
        {
            DataContext = this;
            SetupSearch();
        }

        private void SetupSearch()
        {
            searchFunction = (query, searchBy) =>
            {
                var list = _userBUS.FilterByQuery(query, searchBy);
                App.Instance!.InvokeInMainThread(() => Users.ResetTo(list));
            };

            searchFunction = searchFunction.Debounce(200);

            searchByComboBox.ItemsSource = Enum.GetValues<UserQueryOption>();
            searchByComboBox.SelectedIndex = 0;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchFunction is null) return;
            if (searchByComboBox.SelectedItem is not UserQueryOption queryOption) return;

            searchFunction(searchField.Text, queryOption);
        }

        private void OnSearchByChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchByComboBox.SelectedItem is not UserQueryOption queryOption) return;

            var list = _userBUS.FilterByQuery(searchField.Text, queryOption);
            App.Instance!.InvokeInMainThread(() => Users.ResetTo(list));
        }

        private void OnView(object sender, object model)
        {

        }

        private void OnEdit(object sender, object model)
        {

        }

        private void OnDelete(object sender, object model)
        {
            var result = MessageBox.Show("Are you really want to delete this user ?", "Delete user", MessageBoxButton.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    MessageBox.Show("Delete Successfully!");
                    return;

                case MessageBoxResult.No:
                    return;

                default: 
                    return;
            }
        }
    }
}
