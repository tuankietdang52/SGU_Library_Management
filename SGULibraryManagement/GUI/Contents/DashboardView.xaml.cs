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
                "Users Status"
            };
            statisticOptions.SelectedIndex = 0;
        }

        public void Fetch()
        {
            FetchQuantityStatistics();
            FetchTop3Borrow();
            FetchTop3User();
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

        private void FetchTop3Borrow()
        {
            Dictionary<long, int> top = [];

            var borrows = borrowDevicesBUS.GetAllWithDetail();
            var reservations = reservationBUS.GetAllWithDetail()
                                             .Where(item => !item.IsExpired && !item.IsCheckedOut);
                                           
            foreach (var item in borrows)
            {
                if (!top.ContainsKey(item.Device.Id))
                {
                    top.Add(item.Device.Id, item.Quantity);
                }
                else top[item.Device.Id] += item.Quantity;
            }

            foreach (var item in reservations)
            {
                if (!top.ContainsKey(item.Device.Id))
                {
                    top.Add(item.Device.Id, item.Quantity);
                }
                else top[item.Device.Id] += item.Quantity;
            }

            Dictionary<string, BarItem> top3 = top.OrderByDescending(pr => pr.Value)
                                                  .Take(3)
                                                  .ToDictionary(pr => pr.Key.ToString(), 
                                                                pr => new BarItem() { Value = pr.Value });

            top3DeviceChart.Model = StatisticsUtility.CreateHorizontalBarChart("Top 3 most borrowed devices", "Borrow Quantity", top3, "Borrow Quantity", "Device Id");
        }

        private void FetchTop3User()
        {
            Dictionary<long, int> top = [];

            var borrows = borrowDevicesBUS.GetAllWithDetail();
            var reservations = reservationBUS.GetAllWithDetail()
                                             .Where(item => !item.IsExpired && !item.IsCheckedOut);

            foreach (var item in borrows)
            {
                if (!top.TryAdd(item.User.Mssv, 1))
                    top[item.User.Mssv] += 1;
            }

            foreach (var item in reservations)
            {
                if (!top.TryAdd(item.User.Mssv, 1))
                    top[item.User.Mssv] += 1;
            }

            Dictionary<string, BarItem> top3 = top.OrderByDescending(pr => pr.Value)
                                                 .Take(3)
                                                 .ToDictionary(pr => pr.Key.ToString(),
                                                               pr => new BarItem() { Value = pr.Value });

            top3UserChart.Model = StatisticsUtility.CreateHorizontalBarChart("Top 3 User", "Borrow Time", top3, "Borrow Time", "User Id");
        }

        private void FetchUsersStatus()
        {
            var users = accountBUS.GetAllWithRole();

            double sum = users.Count;
            double lockedUser = users.Where(u => u.IsLocked).Count();
            double unLockedUser = sum - lockedUser;

            var lockedColor = (SolidColorBrush)App.Instance!.Resources["ErrorColor"];
            var unLockedColor = (SolidColorBrush)App.Instance!.Resources["ActiveBackground"];

            userStatusChart.Model = StatisticsUtility.CreatePieChart("User Lock Status",
            [
                new("Locked", lockedUser) { Fill = lockedColor.ParseToOxyColor() },
                new("Unlocked", unLockedUser) { Fill = unLockedColor.ParseToOxyColor() }
            ]);
        }

        private void HideAllStatistic()
        {
            top3Container.Visibility = Visibility.Collapsed;
            userStatusContainer.Visibility = Visibility.Collapsed;
        }

        private void OnOptionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statisticOptions.SelectedItem is not string option) return;

            HideAllStatistic();

            switch (option)
            {
                case "Top 3":
                    FetchQuantityStatistics();
                    top3Container.Visibility = Visibility.Visible;
                    return;

                case "Users Status":
                    FetchUsersStatus();
                    userStatusContainer.Visibility = Visibility.Visible;
                    return;

                default:
                    return;
            }
        }
    }
}