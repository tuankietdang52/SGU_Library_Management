using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SGULibraryManagement.GUI.DialogGUI
{
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
                lbTitle.Text = $"Update User Id {model.Mssv}";
                btn.Content = "Save";
                lockButton.Visibility = Visibility.Visible;
            }
            else // view
            {
                lbTitle.Text = $"View User With Id {model.Mssv} Detail";
                btn.Visibility = Visibility.Hidden;
                historyOptionContainer.Visibility = Visibility.Visible;
                DisableForm();
            }
        }

        private void ShowViolation()
        {
            var violation = violationBUS.FindById(accountViolation!.ViolationId);
            if (violation is null) return;

            violationDescriptionContainer.Visibility = Visibility.Visible;
            violationDescription.Text = $"This account is currently lock for {violation.Name} at {accountViolation.DateCreate}";
            Height = 800;
        }

        private void HideViolation()
        {
            violationDescriptionContainer.Visibility = Visibility.Collapsed;
            Height = 780;
        }

        private void LoadFormData()
        {
            if (model is null) return;

            if (accountViolation is not null) ShowViolation();
            else HideViolation();

            txtTaiKhoan.Text = model.Mssv + "";
            txtMatKhau.Password = model.Password;
            passwordTB.Text = model.Password;
            txtTen.Text = model.FirstName;
            txtHo.Text = model.LastName;
            txtSdt.Text = model.Phone;
            txtEmail.Text = model.Email;
            facultyTB.Text = model.Faculty;
            majorTB.Text = model.Major;
            cbxQuyen.SelectedIndex = roles.FindIndex(role => role.Id == model.IdRole);
            imageChooser.FilePath = model.Avatar;

            txtTaiKhoan.IsEnabled = false;
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
            facultyTB.IsEnabled = false;
            majorTB.IsEnabled = false;
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

        private async void OnAlert(string title, string message)
        {
            SimpleDialog dialog = new()
            {
                Title = title,
                Content = message,
                Width = 400,
                Height = 300
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }

        private async void OnSuccess(string message)
        {
            SimpleDialog dialog = new()
            {
                Title = "Success",
                Content = message,
                Width = 400,
                Height = 300
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
            OnCloseDialog?.Invoke(this);
        }

        private AccountDTO GetData()
        {
            List<string> fields =
            [
                txtTaiKhoan.Text,
                txtMatKhau.Password,
                txtHo.Text,
                txtSdt.Text,
                txtEmail.Text,
                facultyTB.Text,
                majorTB.Text
            ];

            if (fields.IsNullOrWhiteSpace())
            {
                OnAlert("Fail", "All field must not be empty!");
                return null!;
            }
            return new AccountDTO()
            {
                Mssv = long.Parse(txtTaiKhoan.Text),
                Password = txtMatKhau.Password,
                FirstName = txtTen.Text,
                LastName = txtHo.Text,
                Phone = txtSdt.Text,
                Email = txtEmail.Text,
                Faculty = facultyTB.Text,
                Major = majorTB.Text,
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
                OnSuccess("Create Successfully!");
            }
            else
            {
                OnAlert("Fail", "Create Failed! Unexpected Error");
            }
        }

        private void UpdateUser()
        {
            AccountDTO modelData = GetData();
            if (modelData == null) return;

            modelData.Mssv = model!.Mssv;

            if (accountBUS.UpdateAccount(modelData.Mssv, modelData))
            {
                OnSuccess("Update Successfully!");
            }
            else
            {
                OnAlert("Fail", "Update Failed! Unexpected Error");
            }
        }

        private void OnLockUserClick(object sender, RoutedEventArgs e)
        {
            if (model is null) return;

            if (accountViolation is not null) OnShowUpdateLockDialog(accountViolation);
            else OnShowLockDialog();

            //refetch model
            model = accountBUS.FindById(model.Mssv);
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

        private void OnShowViolationHistory(object sender, RoutedEventArgs e)
        {
            if (model is null) return;

            Dialog dialog = new($"{model.FullName} Violation History", new ViolationHistoryDialog(model));
            dialog.ShowDialog();
        }

        private void OnShowBorrowHistory(object sender, RoutedEventArgs e)
        {
            if (model is null) return;

            Dialog dialog = new($"{model.FullName} Borrow Device History", new BorrowDeviceHistoryDialog(model));
            dialog.ShowDialog();
        }
    }
}