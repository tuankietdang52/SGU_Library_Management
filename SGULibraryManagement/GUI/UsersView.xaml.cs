using SGULibraryManagement.GUI.ViewModels;
using System;
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
        public UsersView()
        {
            InitializeComponent();
            Fetch();
        }

        private void Fetch()
        {
            UserViewModel model = new()
            {
                Id = 1,
                FullName = "SHIBA LAKAKA",
                Phone = "0321321",
                Username = "shiba123",
                Password = "123",
                IsAvailable = false
            };

            UserViewModel model2 = new()
            {
                Id = 1,
                FullName = "LAKAKA LAKAKA",
                Phone = "0321321",
                Username = "lakaka123",
                Password = "123",
                IsAvailable = true
            };

            ObservableCollection<UserViewModel> a = [];
            a.Add(model);
            a.Add(model2);
            a.Add(model2);
            a.Add(model2);
            a.Add(model2);
            a.Add(model2);
            a.Add(model2);

            userTable.ItemsSource = a;
        }
    }
}
