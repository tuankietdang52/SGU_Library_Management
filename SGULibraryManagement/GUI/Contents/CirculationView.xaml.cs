using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.DialogGUI;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class CirculationView : UserControl, IContent
    {
        public ObservableCollection<BorrowDeviceViewModel> BorrowDevices { get; set; } = [];
        private readonly BorrowDevicesBUS BUS = new();
        private Action<BorrowDeviceFilter?>? searchDebounce;

        public CirculationView()
        {
            InitializeComponent();
            SetupComponent();
            Fetch();
        }

        public void Fetch()
        {
            _ = BUS.GetAllWithDetail();
            OnApplyFilter(GetFilter());
        }

        private void RenderTable(IEnumerable<BorrowDeviceViewModel> collections)
        {
            App.Instance!.InvokeInMainThread(() => BorrowDevices.ResetTo(collections));
        }

        private void SetupComponent()
        {
            DataContext = this;
            searchDebounce = ((Action<BorrowDeviceFilter?>)(OnApplyFilter)).Debounce(200);

            searchByComboBox.ItemsSource = new List<string>
            {
                "Device Name",
                "User Email",
                "Code"
            };
            searchByComboBox.SelectedIndex = 0;

            statusComboBox.ItemsSource = new List<string> { 
                "All", 
                "Return", 
                "Return Late",
                "Not Return", 
                "Not yet due" 
            };
            statusComboBox.SelectedIndex = 0;
        }

        private BorrowDeviceFilter? GetFilter()
        {
            if (searchByComboBox.SelectedItem is not string searchBy) return null;
            if (statusComboBox.SelectedItem is not string status) return null;

            return new BorrowDeviceFilter()
            {
                Query = searchField.Text,
                SearchBy = searchBy,
                Status = status
            };
        }

        private void OnApplyFilter(BorrowDeviceFilter? filter)
        {
            if (filter is null) return;

            var result = BUS.FilterByQuery(filter.Query, filter.SearchBy);
            result = BUS.FilterByStatus(filter.Status, result);

            RenderTable(result);
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;

            var filter = GetFilter();
            searchDebounce(filter);
        }

        private void OnFilterCbChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = GetFilter();
            OnApplyFilter(filter);
        }

        private void OnBorrow(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new("Borrow Device", new CirculationDialog(CirculationDialogType.Borrow));
            dialog.ShowDialog();

            Fetch();
        }

        private void OnReturn(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new("Return Device", new CirculationDialog(CirculationDialogType.Return));
            dialog.ShowDialog();

            Fetch();
        }
    }

    public class BorrowDeviceFilter
    {
        public required string Query { get; set; }
        public required string SearchBy { get; set; }
        public required string Status { get; set; }
    }
}
