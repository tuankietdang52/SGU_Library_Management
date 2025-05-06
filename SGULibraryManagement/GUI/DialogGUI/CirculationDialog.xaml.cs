using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.Contents;
using SGULibraryManagement.Utilities;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SGULibraryManagement.GUI.DialogGUI
{
    public partial class CirculationDialog : UserControl, IDialog
    {
        private readonly CirculationDialogType type;
        private readonly BorrowDevicesBUS borrowDevicesBUS = new();
        private readonly AccountBUS accountBUS = new();
        private readonly DeviceBUS deviceBUS = new();
        private readonly AccountViolationBUS accountViolationBUS = new();

        private BorrowDevicesDTO? Current;

        public ContentPresenter? PopupHost { get; set; }

        public event OnCloseDialogHandler? OnCloseDialog;

        public CirculationDialog(CirculationDialogType type)
        {
            InitializeComponent();
            this.type = type;

            if (type == CirculationDialogType.Borrow) SetupBorrowComponent();
            else SetupReturnComponent();
        }

        private void SetupBorrowComponent()
        {
            title.Text = "Borrow a Device";
            dueDatePicker.DisplayDateStart = DateTime.Now;
        }

        private void SetupReturnComponent()
        {
            title.Text = "Return a Device";
            borrowContainer.Visibility = Visibility.Collapsed;
            returnContainer.Visibility = Visibility.Visible;
        }

        private async void OnAlert(string title, string message)
        {
            SimpleDialog dialog = new()
            {
                Title = "Failed",
                Content = message,
                Width = 500,
                Height = 400
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }
        
        private async void OnSuccess(string message)
        {
            SimpleDialog dialog = new()
            {
                Title = "Success",
                Content = message,
                Width = 500,
                Height = 400
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
            
            OnCloseDialog?.Invoke(this);
        }

        private int GetBorrowQuantity(long deviceId)
        {
            if (!deviceBUS.DeviceBorrowQuantity!.TryGetValue(deviceId, out int brQuantity))
                return 0;

            return brQuantity;
        }

        private Result ValidateField()
        {
            string emptyMessage = "All field must not empty";

            if (string.IsNullOrWhiteSpace(studentCodeTF.Text) ||
                string.IsNullOrWhiteSpace(deviceIdTF.Text) ||
                string.IsNullOrEmpty(quantityTextField.Text))
            {
                return new(false, emptyMessage);
            }

            if (dueDatePicker.SelectedDate is null) return new(false, emptyMessage);

            return new(true, "");
        }

        private Result Validate()
        {
            var validateFieldRS = ValidateField();
            if (!validateFieldRS.Value) return validateFieldRS;

            long studentCode = long.Parse(studentCodeTF.Text);
            long deviceId = long.Parse(deviceIdTF.Text);

            var account = accountBUS.FindById(long.Parse(studentCodeTF.Text));
            if (account is null)
            {
                return new(false, $"Cant find student with code {studentCode}");
            }

            if (accountViolationBUS.IsAccountLocked(account, out var _))
            {
                return new(false, $"This student is being banned");
            }

            var device = deviceBUS.FindById(deviceId);
            if (device is null)
            {
                return new(false, $"Cant find device with Id {deviceId}");
            }

            int quantity = (int)quantityTextField.Value!;
            int remain = device.Quantity - GetBorrowQuantity(deviceId);
            if (remain < quantity)
            {
                return new(false, $"Not enough quantity to borrow. Max borrow quantity is {remain}");
            }

            return new(true, "");
        }

        private BorrowDevicesDTO GatherData()
        {
            DateTime dueDate = dueDatePicker.SelectedDate!.Value;
            long studentCode = long.Parse(studentCodeTF.Text);
            long deviceId = long.Parse(deviceIdTF.Text);

            return new()
            {
                UserId = studentCode,
                DeviceId = deviceId,
                Quantity = (int)quantityTextField.Value!,
                DateCreate = DateTime.Now,
                DateBorrow = DateTime.Now,
                DateReturnExpected = dueDate,
                IsReturn = false,
                IsDeleted = false
            };
        }

        private void OnCreate(object sender, RoutedEventArgs e)
        {
            var result = Validate();
            if (!result.Value)
            {
                OnAlert("Fail", result.Message);
                return;
            }

            var model = GatherData();
            
            if (borrowDevicesBUS.Create(model) is null)
            {
                OnAlert("Fail", "Create failed! Unexpected error");
                return;
            }

            OnSuccess($"Student with code {studentCodeTF.Text} borrow device with id {deviceIdTF.Text} successfully");
        }

        private void OnFind(object sender, RoutedEventArgs e)
        {
            string code = codeTF.Text;
            if (string.IsNullOrWhiteSpace(code))
            {
                ResetModel();
                return;
            }

            Current = borrowDevicesBUS.FindByCode(code);
            if (Current is null)
            {
                HandleFindError("Not Found", $"Code {code} does not exist!");
                return;
            }

            if (Current.IsReturn)
            {
                HandleFindError("Already Return", $"Code {code} is fulfill!");
                return;
            }

            UpdateModel();
        }

        private void HandleFindError(string title, string message)
        {
            OnAlert(title, message);
            ResetModel();
        }

        private void UpdateModel()
        {
            if (Current is null) return;

            studentCodeTB.Text = Current.UserId.ToString();
            deviceIdTB.Text = Current.DeviceId.ToString();
            dateBorrowTB.Text = Current.DateBorrow.ToString("dd/MM/yyyy");
            dueDateTB.Text = Current.DateReturnExpected.ToString("dd/MM/yyyy");

            returnButton.IsEnabled = true;

            if (Current.DateReturnExpected < DateTime.Now)
            {
                lateDescription.Visibility = Visibility.Visible;
            }
        }

        private void ResetModel()
        {
            studentCodeTB.Text = "";
            deviceIdTB.Text = "";
            dateBorrowTB.Text = "";
            dueDateTB.Text = "";

            returnButton.IsEnabled = false;
        }

        private void OnReturn(object sender, RoutedEventArgs e)
        {
            Current!.IsReturn = true;
            Current.DateReturn = DateTime.Now;

            if (!borrowDevicesBUS.Update(Current.Id, Current))
            {
                OnAlert("Fail", "Failed to return device");
                return;
            }

            OnSuccess($"Return successfully!");
        }
    }

    public enum CirculationDialogType
    {
        Borrow,
        Return
    }
}
