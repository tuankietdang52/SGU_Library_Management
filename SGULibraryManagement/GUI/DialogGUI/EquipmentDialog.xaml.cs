using Mysqlx.Connection;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
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

namespace SGULibraryManagement.GUI.DialogGUI
{
    public partial class EquipmentDialog : UserControl, IDialog
    {
        private string imageFileName = "";
        private readonly DeviceBUS BUS = new();

        public event OnCloseDialogHandler? OnCloseDialog;
        public ContentPresenter? PopupHost { get; set; }

        public EquipmentDialog(EDialogType type)
        {
            InitializeComponent();
        }

        private void OnImageChoose(object sender, string filePath)
        {
            imageFileName = filePath;
        }

        private DeviceDTO? FetchData()
        {
            // TODO: Copy source to Resources/Images with random name for this request object

            try
            {
                int quantity = (int)quantityField.Value!;
                bool isAvailable = quantity > 0;

                return new DeviceDTO()
                {
                    Name = equipmentNameField.Text,
                    Quantity = (int)quantityField.Value,
                    ImageSource = imageFileName.Replace("\\", "/"),
                    IsAvailable = isAvailable,
                    IsDeleted = false
                };
            }
            catch { return null; }
        }

        private async void OnSave(object sender, RoutedEventArgs e)
        {
            var model = FetchData();
            if (model == null)
            {
                await OnSaveFail();
                return;
            }

            var result = BUS.Create(model);
            if (result != null)
            {
                OnSaveSuccess();
            }
            else await OnSaveFail();
        }

        private async void OnSaveSuccess()
        {
            var dialog = new SimpleDialog()
            {
                Title = "Success",
                Content = "Create successfully!",
                Width = 350,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
            if (result == SimpleDialogResult.OK) OnCloseDialog?.Invoke(this);
        }

        private async Task OnSaveFail()
        {
            // adjust the string if this dialog be resused as edit, view dialog
            var dialog = new SimpleDialog()
            {
                Title = "Failed",
                Content = "Failed to create equipment",
                Width = 350,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }
    }
}
