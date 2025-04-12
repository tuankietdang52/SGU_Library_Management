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
using SGULibraryManagement.BUS;
using SGULibraryManagement.DTO;

namespace SGULibraryManagement.GUI
{
    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : UserControl
    {
        private readonly AccountBUS accountBUS = new();
        private AccountDTO? currentUser;

        public SettingView() : this("quang")
        {
        }

        public SettingView(string username)
        {
            InitializeComponent();
            LoadData(username);
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                MessageBox.Show("Không tìm thấy người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            currentUser.FirstName = txtHo.Text.Trim();
            currentUser.LastName = txtTen.Text.Trim();
            currentUser.Password = txtPassword.Text.Trim();
            currentUser.Phone = txtPhone.Text.Trim();
            

            bool success = accountBUS.UpdateAccount(currentUser);

            if (success)
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadData(string username)
        {
            currentUser = accountBUS.FindByUsername(username);

            if (currentUser != null)
            {
                txtHo.Text = currentUser.FirstName;
                txtTen.Text = currentUser.LastName;
                //txtEmail.Text = currentUser.Email;
                txtPhone.Text = currentUser.Phone;
                txtPassword.Text = currentUser.Password;
            }
        }
    }
}
