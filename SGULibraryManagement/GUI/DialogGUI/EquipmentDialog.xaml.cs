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
        private DeviceDTO? currentModel = null;

        public EquipmentDialog(EDialogType type, DeviceDTO? model = null)
        {
            InitializeComponent();
            currentModel = model;

            if (type == EDialogType.Edit && currentModel != null)
            {
                equipmentNameField.Text = currentModel.Name;
                quantityField.Value = currentModel.Quantity;
                imageFileName = currentModel.ImageSource;
                saveButton.Content = "Update";
                imageChooserControl.FilePath = imageFileName;
            }
        }


        private void OnImageChoose(object sender, string filePath)
        {
            imageFileName = filePath;
        }

        private DeviceDTO? FetchData()
        {
            try
            {
                int quantity = (int)quantityField.Value!;
                bool isAvailable = quantity > 0;

                return new DeviceDTO()
                {
                    Id = currentModel?.Id ?? 0, // giữ lại ID khi update
                    Name = equipmentNameField.Text,
                    Quantity = quantity,
                    ImageSource = imageFileName.Replace("\\", "/"),
                    IsAvailable = isAvailable,
                    IsDeleted = false
                };
            }
            catch { return null; }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            var model = FetchData();
            if (model == null)
            {
                OnSaveFail();
                return;
            }

            bool isUpdate = currentModel != null;
            bool isSuccess;

            if (isUpdate)
            {
                model.Id = currentModel!.Id; // đảm bảo gán lại ID nếu đang update
                isSuccess = BUS.Update(model.Id, model);
            }
            else
            {
                var result = BUS.Create(model);
                isSuccess = result != null;
            }

            if (isSuccess)
            {
                OnSaveSuccess();
            }
            else
            {
                OnSaveFail();
            }
        }


        private async void OnSaveSuccess()
        {
            string message = currentModel != null ? "Update successfully!" : "Create successfully!";
            var dialog = new SimpleDialog()
            {
                Title = "Success",
                Content = message,
                Width = 350,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
            if (result == SimpleDialogResult.OK) OnCloseDialog?.Invoke(this);
        }

        private async void OnSaveFail()
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
