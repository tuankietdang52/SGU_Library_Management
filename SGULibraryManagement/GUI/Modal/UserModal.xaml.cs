using SGULibraryManagement.BUS;
using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace SGULibraryManagement.GUI.Modal
{
    /// <summary>
    /// Interaction logic for UserModal.xaml
    /// </summary>
    public partial class UserModal : Window
    {
        private RoleBUS roleBUS = new ();
        private AccountBUS accountBUS = new ();
        private List<RoleDTO> roles = new List<RoleDTO>();
        private string action;
        public UserModal(string action)
        {
            InitializeComponent();
            loadDataCombobox();
            this.action = action;
            loadForm();
        }

        private void loadForm()
        {
            if (action.Equals("create"))
            {
                lbTitle.Text = "Thêm tài khoản";
            }
            else if (action.Equals("update"))
            {
                lbTitle.Text = "Cập nhật tài khoản";
                btn.Content = "Cập nhật";
            }
            else // view
            {
                lbTitle.Text = "Xem thông tin tài khoản";
                btn.Visibility = Visibility.Hidden;
            }
        }

        private void loadDataCombobox()
        {
            cbxQuyen.Items.Clear();
            roles = roleBUS.GetAll();
            roles.ForEach(role =>
            {
                cbxQuyen.Items.Add(role.name);
            });
            
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (action.Equals("create"))
            {
                createUser();
            }
            else if (action.Equals("update"))
            {
                //updateUser();
            }
            
            else // view
            {

            }
        }

        private AccountDTO getData()
        {
            if (string.IsNullOrEmpty(txtTaiKhoan.Text) || string.IsNullOrEmpty(txtMatKhau.Password) || string.IsNullOrEmpty(txtTen.Text) || string.IsNullOrEmpty(txtHo.Text) || string.IsNullOrEmpty(txtSdt.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return null;
            }
            return new AccountDTO()
            {
                Username = txtTaiKhoan.Text,
                Password = txtMatKhau.Password,
                FirstName = txtTen.Text,
                LastName = txtHo.Text,
                Phone = txtSdt.Text,
                idRole = roles[cbxQuyen.SelectedIndex].id
            };
        }

        private void createUser()
        {
            AccountDTO account = getData();
            if (account == null)
            {
                return;
            }
            AccountDTO result = accountBUS.createAccount(account);
            if (result != null)
            {
                MessageBox.Show("Thêm tài khoản thành công");
                this.Close();
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }
        }
    }
}
