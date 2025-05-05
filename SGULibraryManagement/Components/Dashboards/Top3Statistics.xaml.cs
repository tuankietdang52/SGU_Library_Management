using OxyPlot.Series;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Utilities;
using System.Windows.Controls;

namespace SGULibraryManagement.Components.Dashboards
{
    public partial class Top3Statistics : UserControl
    {
        private readonly BorrowDevicesBUS borrowDevicesBUS = new();
        private readonly ReservationBUS reservationBUS = new();

        public Top3Statistics()
        {
            InitializeComponent();
        }

        public void Fetch()
        {
            FetchTop3Borrow();
            FetchTop3User();
        }

        private void FetchTop3Borrow()
        {
            Dictionary<string, int> top = [];

            var borrows = borrowDevicesBUS.GetAllWithDetail();
            var reservations = reservationBUS.GetAllWithDetail()
                                             .Where(item => !item.IsExpired && !item.IsCheckedOut);

            foreach (var item in borrows)
            {
                if (!top.ContainsKey(item.Device.Name))
                {
                    top.Add(item.Device.Name, item.Quantity);
                }
                else top[item.Device.Name] += item.Quantity;
            }

            foreach (var item in reservations)
            {
                if (!top.ContainsKey(item.Device.Name))
                {
                    top.Add(item.Device.Name, item.Quantity);
                }
                else top[item.Device.Name] += item.Quantity;
            }

            Dictionary<string, BarItem> top3 = top.OrderByDescending(pr => pr.Value)
                                                  .Take(3)
                                                  .ToDictionary(pr => pr.Key,
                                                                pr => new BarItem() { Value = pr.Value });

            top3DeviceChart.Model = StatisticsUtility.CreateHorizontalBarChart("Top 3 most borrowed devices", "Borrow Quantity", top3, "Borrow Quantity", "Device");
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

            top3UserChart.Model = StatisticsUtility.CreateHorizontalBarChart("Top 3 User", "Borrow Time", top3, "Borrow Time", "Student Code");
        }

    }
}
