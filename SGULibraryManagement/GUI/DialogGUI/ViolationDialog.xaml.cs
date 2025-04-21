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
    public partial class ViolationDialog : UserControl, IDialog
    {
        private readonly ViolationBUS violationBUS = new();

        private ViolationDTO? model;
        private readonly EDialogType dialogType;

        public ContentPresenter? PopupHost { get; set; }

        public event OnCloseDialogHandler? OnCloseDialog;


        public ViolationDialog()
        {
            InitializeComponent();
            dialogType = EDialogType.Create;
        }

        public ViolationDialog(EDialogType dialogType, ViolationDTO model) : this()
        {
            if (dialogType == EDialogType.Create) return;

            this.model = model;
            this.dialogType = dialogType;

            // remove create event
            saveButton.Click -= OnCreate;
            if (dialogType == EDialogType.Edit) SetupEditComponent();
            else if (dialogType == EDialogType.View) SetupViewComponent();

            FetchData();
        }

        public void SetupViewComponent()
        {
            title.Text = $"Rule Id {model!.Id}";
            saveButton.Visibility = Visibility.Collapsed;

            nameField.IsEnabled = false;
            descriptionField.IsEnabled = false;
        }

        public void SetupEditComponent()
        {
            title.Text = $"Update Rule Id {model!.Id}";
            saveButton.Content = "Save";
            saveButton.Click += OnEdit;
        }

        private void FetchData()
        {
            if (model is null) return;

            nameField.Text = model.Name;
            descriptionField.Text = model.Description;
        }

        private ViolationDTO GatherData()
        {
            return new ViolationDTO()
            {
                Name = nameField.Text,
                Description = descriptionField.Text,
                IsDeleted = false
            };
        }

        private void OnCreate(object sender, RoutedEventArgs e)
        {
            var data = GatherData();

            if (violationBUS.Create(data) is not null)
            {
                OnSuccess("Create successfully!");
            }
            else OnFailed("Create Failed");
        }

        private void OnEdit(object sender, RoutedEventArgs e)
        {
            var data = GatherData();
            data.Id = model!.Id;

            if (violationBUS.Update(model.Id, data))
            {
                OnSuccess("Update successfully!");
            }
            else OnFailed("Update Failed");
        }

        private async void OnSuccess(string message)
        {
            SimpleDialog dialog = new()
            {
                Title = "Success",
                Content = message,
                Width = 300,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
            OnCloseDialog?.Invoke(this);
        }

        private async void OnFailed(string message)
        {
            SimpleDialog dialog = new()
            {
                Title = "Failed",
                Content = message,
                Width = 300,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK, PopupHost);
        }
    }
}
