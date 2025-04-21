using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
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

namespace SGULibraryManagement.GUI.DialogGUI
{
    /// <summary>
    /// Interaction logic for UserModal.xaml
    /// </summary>
    public partial class UserDialog : UserControl, IDialog
    {
        private readonly RoleBUS roleBUS = new();
        private readonly AccountBUS accountBUS = new();
        private readonly ViolationBUS violationBUS = new();
        private readonly AccountViolationBUS accountViolationBUS = new();

        private List<RoleDTO> roles = [];

        private readonly EDialogType dialogType;
        private AccountDTO? model;
        private AccountViolationDTO? accountViolation;

        private ContentPresenter? popupHost;
        public ContentPresenter? PopupHost {
            get => popupHost;
            set {
                popupHost = value;
                imageChooser.PopupHost = value;
            }
        }

        private string imageFilePath = string.Empty;
        public event OnCloseDialogHandler? OnCloseDialog;

        public UserDialog()
        {
            InitializeComponent();

            dialogType = EDialogType.Create;
            LoadDataCombobox();
        }

        public UserDialog(EDialogType type, AccountDTO model) : this()
        {
            if (type == EDialogType.Create) return;
       
            dialogType = type;
            this.model = model;
            accountViolationBUS.IsAccountLocked(model, out accountViolation);

            AdjustForm();
            LoadFormData();
        }

        private void AdjustForm()
        {
            if (model is null) return;

            if (dialogType == EDialogType.Edit)
            {
                lbTitle.Text = $"Update User Id {model.Id}";
                btn.Content = "Save";
                lockButton.Visibility = Visibility.Visible;
            }
            else // view
            {
                lbTitle.Text = $"View User With Id {model.Id} Detail";
                btn.Visibility = Visibility.Hidden;
                DisableForm();
            }
        }

        private void ShowViolation()
        {
            var violation = violationBUS.FindById(accountViolation!.ViolationId);
            if (violation is null) return;

            violationDescriptionContainer.Visibility = Visibility.Visible;
            violationDescription.Text = $"This account is currently lock for {violation.Name}";
            Height = 650;
        }

        private void HideViolation()
        {
            violationDescriptionContainer.Visibility = Visibility.Collapsed;
            Height = 600;
        }

        private void LoadFormData()
        {
            if (model is null) return;

            if (accountViolation is not null) ShowViolation();
            else HideViolation();

            txtTaiKhoan.Text = model.Username;
            txtMatKhau.Password = model.Password;
            passwordTB.Text = model.Password;
            txtTen.Text = model.FirstName;
            txtHo.Text = model.LastName;
            txtSdt.Text = model.Phone;
            txtEmail.Text = model.Email;
            cbxQuyen.SelectedIndex = roles.FindIndex(role => role.Id == model.IdRole);
            imageChooser.FilePath = model.Avatar;
        }

        private void DisableForm()
        {
            txtTaiKhoan.IsEnabled = false;
            txtMatKhau.Visibility = Visibility.Collapsed;
            passwordTB.Visibility = Visibility.Visible;
            txtTen.IsEnabled = false;
            txtHo.IsEnabled = false;
            txtSdt.IsEnabled = false;
            txtEmail.IsEnabled = false;
            cbxQuyen.IsEnabled = false;
            cbxQuyen.Foreground = Brushes.Black;

            btn.Visibility = Visibility.Collapsed;
            imageChooser.Visibility = Visibility.Collapsed;

            try
            {
                image.Source = new BitmapImage(new Uri(model!.Avatar));
            }
            catch { }
            image.Visibility = Visibility.Visible;
        }

        private void LoadDataCombobox()
        {
            cbxQuyen.Items.Clear();
            roles = roleBUS.GetAll();
            roles.ForEach(role =>
            {
                cbxQuyen.Items.Add(role.Name);
            });
            
        }

        private void OnChooseImage(object sender, string filePath)
        {
            imageFilePath = filePath;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (dialogType == EDialogType.Create) CreateUser();
            else if (dialogType == EDialogType.Edit) UpdateUser();
        }

        private AccountDTO GetData()
        {
            if (string.IsNullOrEmpty(txtTaiKhoan.Text) || string.IsNullOrEmpty(txtMatKhau.Password) || string.IsNullOrEmpty(txtTen.Text) || string.IsNullOrEmpty(txtHo.Text) || string.IsNullOrEmpty(txtSdt.Text) || string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return null!;
            }
            return new AccountDTO()
            {
                Username = txtTaiKhoan.Text,
                Password = txtMatKhau.Password,
                FirstName = txtTen.Text,
                LastName = txtHo.Text,
                Phone = txtSdt.Text,
                Email = txtEmail.Text,
                IdRole = roles[cbxQuyen.SelectedIndex].Id,
                Avatar = imageFilePath.Replace("\\", "/"),
                IsDeleted = false
            };
        }

        private void CreateUser()
        {
            AccountDTO account = GetData();
            if (account == null) return;


            AccountDTO result = accountBUS.CreateAccount(account);
            if (result != null)
            {
                MessageBox.Show("Create Successful!");
                OnCloseDialog?.Invoke(this);
            }
            else
            {
                MessageBox.Show("Create Failed");
            }
        }

        private void UpdateUser()
        {
            AccountDTO modelData = GetData();
            if (modelData == null) return;

            modelData.Id = model!.Id;

            if (accountBUS.UpdateAccount(modelData.Id, modelData))
            {
                MessageBox.Show("Update Successful!");
                OnCloseDialog?.Invoke(this);
            }
            else
            {
                MessageBox.Show("Update Failed");
            }
        }

        private void OnLockUserClick(object sender, RoutedEventArgs e)
        {
            if (model is null) return;

            if (accountViolation is not null) OnShowUpdateLockDialog(accountViolation);
            else OnShowLockDialog();

            //refetch model
            model = accountBUS.FindById(model.Id);
            accountViolationBUS.IsAccountLocked(model, out accountViolation);
            LoadFormData();
        }

        private void OnShowLockDialog()
        {
            Dialog dialog = new("Lock account", new LockAccountDialog(model!));
            dialog.ShowDialog();
        }

        private void OnShowUpdateLockDialog(AccountViolationDTO accountViolation)
        {
            Dialog dialog = new("Lock account", new LockAccountDialog(model!, accountViolation));
            dialog.ShowDialog();
        }
    }
}
