using OxyPlot;
using OxyPlot.Series;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Utilities;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class DashboardView : UserControl, IContent
    {
        private readonly AccountBUS accountBUS = new();
        private readonly AccountViolationBUS accountViolationBUS = new();
        private readonly DeviceBUS deviceBUS = new();
        private readonly BorrowDevicesBUS borrowDevicesBUS = new();
        private readonly ReservationBUS reservationBUS = new();
        private readonly StudyAreaBUS studyAreaBUS = new();

        public DashboardView()
        {
            InitializeComponent();
            SetupComponent();
            Fetch();

            DataContext = this;
        }

        private void SetupComponent()
        {
            statisticOptions.ItemsSource = new List<string>()
            {
                "Top 3",
                "Users Status",
                "Study Area"
            };
            statisticOptions.SelectedIndex = 0;
        }

        public void Fetch()
        {
            FetchQuantityStatistics();
            top3Container.Fetch();
        }

        private void FetchQuantityStatistics()
        {
            int borrowQuantity = borrowDevicesBUS.GetAllWithDetail()
                                                 .Where(item => !item.IsReturn && !item.IsDue)
                                                 .Count();

            int reservationQuantity = reservationBUS.GetAllWithDetail()
                                                    .Where(item => !item.IsCheckedOut && !item.IsExpired)
                                                    .Count();

            int userQuantity = accountBUS.GetAll().Count;
            int deviceQuantity = deviceBUS.GetAll()
                                          .Where(item => item.IsAvailable)
                                          .Count();

            quantityStatistics.BookingQuantity = borrowQuantity;
            quantityStatistics.ReservationQuantity = reservationQuantity;
            quantityStatistics.UserQuantity = userQuantity;
            quantityStatistics.DeviceQuantity = deviceQuantity;
        }

        private void HideAllStatistic()
        {
            top3Container.Visibility = Visibility.Collapsed;
            userStatusContainer.Visibility = Visibility.Collapsed;
            studyAreaContainer.Visibility = Visibility.Collapsed;
        }

        private void OnOptionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statisticOptions.SelectedItem is not string option) return;

            HideAllStatistic();

            switch (option)
            {
                case "Top 3":
                    top3Container.Visibility = Visibility.Visible;
                    return;

                case "Users Status":
                    userStatusContainer.Fetch();
                    userStatusContainer.Visibility = Visibility.Visible;
                    return;

                case "Study Area":
                    studyAreaContainer.Visibility = Visibility.Visible;
                    return;

                default:
                    return;
            }
        }
    }
}