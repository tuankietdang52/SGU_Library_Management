using OxyPlot.Axes;
using OxyPlot;
using SGULibraryManagement.BUS;
using SGULibraryManagement.DTO;
using System.Windows.Controls;
using SGULibraryManagement.Utilities;

namespace SGULibraryManagement.Components.Dashboards
{
    public partial class DeviceStatistics : UserControl
    {
        private readonly BorrowDevicesBUS borrowDeviceBUS = new();
        private readonly DeviceBUS deviceBUS = new();

        private readonly List<DeviceDTO> devices = [new() {
            Id = -1,
            Name = "All"
        }];

        public DeviceStatistics()
        {
            InitializeComponent();
            InitializeData();
            SetupComponent();
        }

        private void InitializeData()
        {
            var list = deviceBUS.GetAll();
            devices.AddRange(list);
        }

        private void SetupComponent()
        {
            deviceCB.ItemsSource = devices;
            deviceCB.SelectedIndex = 0;

            typeCB.ItemsSource = new List<string>
            {
                "Borrow",
                "Currently Borrow"
            };
            typeCB.SelectedIndex = 0;
        }
        
        public void Fetch()
        {
            int type = typeCB.SelectedIndex;

            var list = type == 0 ? GetByDate() : GetCurrentlyBorrowByDate();
            var device = (DeviceDTO)deviceCB.SelectedItem;

            if (device.Id != -1) list = [.. list.Where(item => item.DeviceId == device.Id)];

            var dataPoints = type == 0 ? GetBorrowDataPoints(list) : GetCurrentlyBorrowDataPoints(list);

            borrowStatisticsModel.Model = StatisticsUtility.CreateDateLineChart("Number of devices borrow by student",
                                                                                "Number of devices",
                                                                                dataPoints,
                                                                                "Date",
                                                                                "Number of devices",
                                                                                "dd/MM/yyyy",
                                                                                startPicker.SelectedDate,
                                                                                endPicker.SelectedDate);
        }

        private List<DataPoint> GetBorrowDataPoints(IEnumerable<BorrowDevicesDTO> list)
        {
            if (!list.Any()) return [];

            Dictionary<DateTime, double> data = [];
            list = list.OrderBy(item => item.DateBorrow);

            foreach (var item in list)
            {
                if (!data.TryGetValue(item.DateBorrow.Date, out double value))
                {
                    data.Add(item.DateBorrow.Date, item.Quantity);
                }
                else data[item.DateBorrow.Date] = value + item.Quantity;
            }

            return [.. data.Select(pr => {
                double date = DateTimeAxis.ToDouble(pr.Key);
                double count = pr.Value;

                return new DataPoint(date, count);
            })];
        }

        private List<DataPoint> GetCurrentlyBorrowDataPoints(IEnumerable<BorrowDevicesDTO> list)
        {
            if (!list.Any()) return [];

            Dictionary<DateTime, double> data = [];
            list = list.OrderBy(item => item.DateBorrow);

            bool hasStart = startPicker.SelectedDate.HasValue;
            bool hasEnd = endPicker.SelectedDate.HasValue;

            DateTime start = hasStart ? startPicker.SelectedDate!.Value : list.First().DateBorrow;
            DateTime end = hasEnd ? endPicker.SelectedDate!.Value : list.Last().DateReturnExpected;

            foreach (var date in CollectionExtension.EachDay(start, end))
            {
                int count = list.Where(item => item.DateBorrow.Date <= date.Date && item.DateReturnExpected.Date >= date.Date)
                                .Sum(item => item.Quantity);

                if (count == 0) continue;

                if (!data.TryGetValue(date.Date, out double value)) data.Add(date.Date, count);
                else data[date.Date] = value + count;
            }

            return [.. data.Select(pr => {
                double date = DateTimeAxis.ToDouble(pr.Key);
                double count = pr.Value;

                return new DataPoint(date, count);
            })];
        }

        private List<BorrowDevicesDTO> GetByDate()
        {
            bool hasStart = startPicker.SelectedDate.HasValue;
            bool hasEnd = endPicker.SelectedDate.HasValue;

            bool all = !hasStart && !hasEnd;
            bool sToE = hasStart && hasEnd;
            bool fromStart = hasStart && !hasEnd;
            bool toEnd = !hasStart && hasEnd;

            if (all) return borrowDeviceBUS.GetAll();
            if (sToE) return borrowDeviceBUS.GetAllByBorrowDate(startPicker.SelectedDate!.Value, endPicker.SelectedDate!.Value);
            if (fromStart) return borrowDeviceBUS.GetAllByBorrowDate(startPicker.SelectedDate!.Value, true);
            if (toEnd) return borrowDeviceBUS.GetAllByBorrowDate(endPicker.SelectedDate!.Value, false);

            return [];
        }

        private List<BorrowDevicesDTO> GetCurrentlyBorrowByDate()
        {
            bool hasStart = startPicker.SelectedDate.HasValue;
            bool hasEnd = endPicker.SelectedDate.HasValue;

            bool all = !hasStart && !hasEnd;
            bool sToE = hasStart && hasEnd;
            bool fromStart = hasStart && !hasEnd;
            bool toEnd = !hasStart && hasEnd;

            if (all) return borrowDeviceBUS.GetCurrentlyBorrow();
            if (sToE) return borrowDeviceBUS.GetCurrentlyBorrowByDate(startPicker.SelectedDate!.Value, endPicker.SelectedDate!.Value);
            if (fromStart) return borrowDeviceBUS.GetCurrentlyBorrowByDate(startPicker.SelectedDate!.Value, true);
            if (toEnd) return borrowDeviceBUS.GetCurrentlyBorrowByDate(endPicker.SelectedDate!.Value, false);

            return [];
        }

        private void OnStartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnDateChanged(sender, e);
            endPicker.DisplayDateStart = startPicker.SelectedDate;

            Fetch();
        }

        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!endPicker.SelectedDate.HasValue || !startPicker.SelectedDate.HasValue)
            {
                Fetch();
                return;
            }

            if (endPicker.SelectedDate.Value < startPicker.SelectedDate.Value)
                endPicker.SelectedDate = startPicker.SelectedDate;


            Fetch();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fetch();
        }
    }
}
