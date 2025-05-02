using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class ReservationView : UserControl, IContent
    {
        private readonly ReservationBUS BUS = new();
        private Action<ReservationFilter?>? searchDebounce;

        public ObservableCollection<ReservationViewModel> Reservations { get; set; } = [];
        public ICommand? CheckoutCommand { get; set; }

        public ReservationView()
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

        private void RenderTable(IEnumerable<ReservationViewModel> collections)
        {
            App.Instance!.InvokeInMainThread(() => Reservations.ResetTo(collections));
        }

        private void SetupComponent()
        {
            DataContext = this;

            CheckoutCommand = new RelayCommand<ReservationViewModel>(Checkout!);
            searchDebounce = ((Action<ReservationFilter?>)(OnApplyFilter)).Debounce(200);

            searchByComboBox.ItemsSource = new List<string>
            {
                "Device Name",
                "User Email"
            };
            searchByComboBox.SelectedIndex = 0;

            statusComboBox.ItemsSource = new List<string> { "All", "Ready", "Incoming", "Expired" };
            statusComboBox.SelectedIndex = 0;
        }

        private ReservationFilter? GetFilter()
        {
            if (searchByComboBox.SelectedItem is not string searchBy) return null;
            if (statusComboBox.SelectedItem is not string status) return null;
            if (showCheckedOutButton.IsChecked is null) return null;

            return new ReservationFilter()
            {
                Query = searchField.Text,
                SearchBy = searchBy,
                Status = status,
                IsShowCheckedOut = (bool)showCheckedOutButton.IsChecked
            };
        }

        private void OnApplyFilter(ReservationFilter? filter)
        {
            if (filter is null) return;

            var result = BUS.FilterByQuery(filter.Query, filter.SearchBy);
            result = BUS.FilterByStatus(filter.Status, result);
            result = BUS.FilterByCheckOutStatus(filter.IsShowCheckedOut, result);

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

        private void OnShowCheckedOutClick(object sender, RoutedEventArgs e)
        {
            var filter = GetFilter();
            OnApplyFilter(filter);
        }

        private async void Checkout(ReservationViewModel reservation)
        {
            SimpleDialog dialog = new()
            {
                Title = "Checkout",
                Content = $"Checkout reservation with Id: {reservation.Id} ?",
                Width = 300,
                Height = 250
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.YesNo);

            if (result == SimpleDialogResult.Yes)
            {
                BUS.Checkout(reservation);
            }

            MainView.Instance.FetchAll([
                typeof(UsersView), 
                typeof(ReservationView), 
                typeof(CirculationView)
            ]);
        }
    }

    public class ReservationFilter
    {
        public required string Query { get; set; }
        public required string SearchBy { get; set; }
        public required string Status { get; set; }
        public required bool IsShowCheckedOut { get; set; }
    }
}
