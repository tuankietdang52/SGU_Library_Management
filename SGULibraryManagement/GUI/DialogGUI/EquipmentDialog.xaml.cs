using Google.Protobuf.WellKnownTypes;
using Mysqlx.Connection;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.Components.FileChoosers;
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
        public event OnCloseDialogHandler? OnCloseDialog;

        private string imageFilePath = "";
        private readonly DeviceBUS BUS = new();
        private readonly EDialogType dialogType;

        private ContentPresenter? popupHost;
        public ContentPresenter? PopupHost
        {
            get => popupHost;
            set
            {
                popupHost = value;
                imageChooserControl.PopupHost = value;
            }
        }

        private readonly DeviceDTO? currentModel = null;

        public EquipmentDialog()
        {
            InitializeComponent();

            dialogType = EDialogType.Create;
            statusCb.ItemsSource = new List<string>() { "Available", "Not Available" };
            statusCb.SelectedIndex = 0;
        }

        public EquipmentDialog(EDialogType type, DeviceDTO model) : this()
        {
            if (type == EDialogType.Create) return;

            currentModel = model;
            dialogType = type;

            if (dialogType == EDialogType.View) SetupViewDialog();
            else if (dialogType == EDialogType.Edit) SetupUpdateDialog();
        }
        private void SetupViewDialog()
        {
            if (currentModel is null) return;

            var app = App.Instance!;

            SolidColorBrush availableColor = Brushes.LightGreen;
            if (app.Resources["ErrorColor"] is not SolidColorBrush notAvailableColor)
            {
                notAvailableColor = Brushes.Red;
            }

            editableDialog.Visibility = Visibility.Collapsed;
            nonEditableDialog.Visibility = Visibility.Visible;

            statusLabel.Text = currentModel.IsAvailable ? "Available" : "Not Available";
            statusLabel.Background = currentModel.IsAvailable ? availableColor : notAvailableColor;
            equipmentNameTB.Text = currentModel.Name;
            quantityTB.Text = currentModel.Quantity.ToString();
            descriptionTB.Text = currentModel.Description;
            Height = 550;

            try
            {
                image.Source = new BitmapImage(new Uri(currentModel.ImageSource));
            }
            catch { }
        }

        private void SetupUpdateDialog()
        {
            if (currentModel is null) return;

            imageFilePath = currentModel.ImageSource;
            imageChooserControl.FilePath = imageFilePath;
            statusCb.SelectedIndex = currentModel.IsAvailable ? 0 : 1; 
            equipmentNameField.Text = currentModel.Name;
            quantityField.Value = currentModel.Quantity;
            descriptionField.Text = currentModel.Description;
            saveButton.Content = "Update";
        }

        private void OnImageChoose(object sender, string filePath)
        {
            imageFilePath = filePath;
        }

        private DeviceDTO? FetchData()
        {
            try
            {
                int quantity = (int)quantityField.Value!;

                bool isAvailable = statusCb.SelectedIndex == 0 && quantity > 0;

                return new DeviceDTO()
                {
                    Name = equipmentNameField.Text,
                    Quantity = quantity,
                    ImageSource = imageFilePath.Replace("\\", "/"),
                    Description = descriptionField.Text,
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
                OnSaveFail("Failed to get equipment data");
                return;
            }

            if (dialogType == EDialogType.Create) OnCreate(model);
            else if (dialogType == EDialogType.Edit) OnUpdate(model);
        }

        private void OnCreate(DeviceDTO model)
        {
            if (BUS.Create(model) != null) OnSaveSuccess("Create successfully!");
            else OnSaveFail("Failed to create equipment");
        }

        private void OnUpdate(DeviceDTO model)
        {
            if (currentModel is null)
            {
                OnSaveFail("Failed to get equipment data");
                return;
            }

            model.Id = currentModel.Id;

            if (BUS.Update(model.Id, model)) OnSaveSuccess("Update successfully!");
            else OnSaveFail("Failed to update equipment");
        }


        private async void OnSaveSuccess(string successMessage)
        {
            var dialog = new SimpleDialog()
            {
                Title = "Success",
                Content = successMessage,
                Width = 350,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
            if (result == SimpleDialogResult.OK) OnCloseDialog?.Invoke(this);
        }

        private async void OnSaveFail(string failMessage)
        {
            var dialog = new SimpleDialog()
            {
                Title = "Failed",
                Content = failMessage,
                Width = 350,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }
    }
}
