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
        }
        
        public void Fetch()
        {
            var list = GetByDate();
            var device = (DeviceDTO)deviceCB.SelectedItem;

            if (device.Id != -1) list = [.. list.Where(item => item.DeviceId == device.Id)];

            var dataPoints = GetBorrowDataPoints(list);

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
