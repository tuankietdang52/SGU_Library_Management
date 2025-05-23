﻿using Microsoft.Win32;
using OfficeOpenXml;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.Components.Equipments;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.DialogGUI;
using SGULibraryManagement.Utilities;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class EquipmentsView : UserControl, IContent
    {
        private readonly DeviceBUS BUS = new();
        private bool isOpenFilter = false;

        private Action<EquipmentFilter?>? searchDebounce;
        private readonly Dictionary<long, EquipmentItem> equipmentItems = [];
        private List<DeviceDTO> devices = [];

        public EquipmentsView()
        {
            InitializeComponent();
            Fetch();
            SetupComponent();
        }

        public void Fetch()
        {
            ClearEquipmentItems();
            devices = BUS.GetAll();

            foreach (var item in devices)
            {
                if (!BUS.DeviceBorrowQuantity.TryGetValue(item.Id, out int borrowQuantity)) borrowQuantity = 0;

                EquipmentItem equipmentItem = new()
                {
                    Model = item,
                    Margin = new Thickness(0, 0, 15, 15),
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    RemainQuantity = item.Quantity - borrowQuantity
                };

                equipmentItem.OnView += OnView;
                equipmentItem.OnEdit += OnEdit;
                equipmentItem.OnDelete += OnDelete;

                equipmentItems.Add(item.Id, equipmentItem);
            }
        }

        private void ClearEquipmentItems()
        {
            foreach (var item in equipmentItems.Values)
            {
                item.OnView -= OnView;
                item.OnEdit -= OnEdit;
                item.OnDelete -= OnDelete;
            }

            equipmentItems.Clear();
        }

        private void SetEquipmentItems(IEnumerable<DeviceDTO> list)
        {
            equipmentsContainer.Children.Clear();

            foreach (var item in list)
            {
                var equipmentItem = equipmentItems[item.Id];
                equipmentsContainer.Children.Add(equipmentItem);
            }
        }

        private void SetupComponent()
        {
            SetupSearchAndFilter();
            sortComboBox.SelectedIndex = 0;
        }

        private void SetupSearchAndFilter()
        {
            searchDebounce = ((Action<EquipmentFilter?>)((filter) =>
            {
                OnApplyFilter(filter);
            })).Debounce(200);

            statusComboBox.ItemsSource = new List<string>()
            {
                "All",
                "Available",
                "Not Available"
            };
            statusComboBox.SelectedIndex = 0;
        }

        private EquipmentFilter? GetFilter()
        {
            if (statusComboBox.SelectedItem is not string status) return null;

            string query = searchField.Text;
            return new EquipmentFilter()
            {
                Query = query,
                Status = status,
                Sort = sortComboBox.SelectedIndex,
            };
        }

        private void OnApplyFilter(EquipmentFilter? filter)
        {
            if (filter is null) return;

            devices = BUS.FilterByQuery(filter.Query);

            if (filter.Status != "All")
            {
                devices = BUS.FilterByStatus(filter.Status == "Available", devices);
            }

            OnApplySort(filter.Sort);
        }

        private void OnApplySort(int selectedSort = -1)
        {
            try
            {
                DeviceSort sort = selectedSort == -1 ? (DeviceSort)sortComboBox.SelectedIndex : (DeviceSort)selectedSort;
                var list = BUS.SortBy(sort, devices);

                App.Instance!.InvokeInMainThread(() => SetEquipmentItems(list));
            }
            catch { }
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;

            EquipmentFilter? filter = GetFilter();
            searchDebounce(filter);

            scrollContainer.ScrollToHome();
        }

        private void OnStatusChanged(object sender, SelectionChangedEventArgs e)
        {
            EquipmentFilter? filter = GetFilter();
            OnApplyFilter(filter);
            scrollContainer.ScrollToHome();
        }

        private void OnSort(object sender, SelectionChangedEventArgs e)
        {
            OnApplySort();
            scrollContainer.ScrollToHome();
        }

        private void OnFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (!isOpenFilter && FindResource("FilterExpand") is Storyboard expand)
            {
                isOpenFilter = true;
                expand.Begin();
            }
            else if (FindResource("CloseExpand") is Storyboard close)
            {
                isOpenFilter = false;
                close.Begin();
            }
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialog("Add new equipment", new EquipmentDialog());
            dialog.ShowDialog();

            Fetch();
            OnApplySort();
        }
        private void ImportExcel(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Excel Files|*.xlsx"
            };
            if (openFileDialog.ShowDialog() != true) return;

            var listDevice = Importing(openFileDialog);

            if (BUS.CreateListDevice(listDevice) == false)
            {
                MessageBox.Show("Import failed");
                return;
            }

            MessageBox.Show("Import successfully!");
            Fetch();
        }

        private List<DeviceDTO> Importing(OpenFileDialog openFileDialog)
        {
            string filePath = openFileDialog.FileName;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Xử lý file Excel
            using var excelPackage = new ExcelPackage(new FileInfo(filePath));
            var worksheet = excelPackage.Workbook.Worksheets[0];
            int startRow = worksheet.Dimension.Start.Row + 1;
            int endRow = worksheet.Dimension.End.Row;

            List<DeviceDTO> listDevice = [];
            for (int i = startRow; i <= endRow; i++)
            {
                List<string> rowData = [];
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    rowData.Add(worksheet.Cells[i, j].Text);
                }
                try
                {
                    listDevice.Add(new DeviceDTO()
                    {
                        Name = rowData[0],
                        Quantity = int.Parse(rowData[1]),
                        ImageSource = rowData[2],
                        Description = rowData[3],
                        IsAvailable = false,
                        IsDeleted = false
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show("One or more object in excel does not match with device object");
                    return [];
                }

            }

            return listDevice;
        }

        private void OnView(object sender, DeviceDTO model)
        {
            var dialog = new Dialog($"View equipment Id {model.Id} detail", new EquipmentDialog(EDialogType.View, model));
            dialog.ShowDialog();

            Fetch();
            OnApplySort();
        }

        private void OnEdit(object sender, DeviceDTO model)
        {
            var dialog = new Dialog($"Edit equipment Id {model.Id}", new EquipmentDialog(EDialogType.Edit, model));
            dialog.ShowDialog();

            Fetch();
            OnApplySort();
        }

        private async void OnDelete(object sender, DeviceDTO model)
        {
            SimpleDialog simpleDialog = new()
            {
                Title = $"Delete {model.Name}",
                Content = $"Do you really want to delete equipment Id {model.Id} ?",
                Width = 400,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(simpleDialog, SimpleDialogType.YesNo);

            if (result == SimpleDialogResult.Yes)
            {
                bool isSuccess = BUS.Delete(model); // giả sử Delete trả bool

                if (isSuccess)
                {
                    Fetch();
                    OnApplySort();
                }
                else
                {
                    await MainWindow.Instance!.ShowSimpleDialogAsync(
                        new SimpleDialog
                        {
                            Title = "Error",
                            Content = "Cant delete device",
                            Width = 400,
                            Height = 200
                        },
                        SimpleDialogType.OK
                    );
                }
            }
        }

        private class EquipmentFilter
        {
            public string Query { get; set; } = string.Empty;
            public string Status { get; set; } = "All";
            public int Sort { get; set; }
        }
    }
}
