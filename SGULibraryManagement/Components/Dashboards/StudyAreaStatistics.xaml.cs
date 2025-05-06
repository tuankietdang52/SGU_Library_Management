using OxyPlot;
using OxyPlot.Axes;
using SGULibraryManagement.BUS;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
using System.Windows.Controls;

namespace SGULibraryManagement.Components.Dashboards
{
    public partial class StudyAreaStatistics : UserControl
    {
        private readonly StudyAreaBUS studyAreaBUS = new();
        private readonly AccountBUS accountBUS = new();

        private Dictionary<long, AccountDTO> accounts = [];

        private readonly HashSet<string> faculties = ["All"];
        private readonly HashSet<string> majors = ["All"];

        public StudyAreaStatistics()
        {
            InitializeComponent();
            InitializeData();
            SetupComponent();
        }

        private void InitializeData()
        {
            var list = accountBUS.GetAll();
            accounts = list.ToDictionary(pr => pr.Mssv);

            faculties.UnionWith(list.Select(item => item.Faculty));
            majors.UnionWith(list.Select(item => item.Major));
        }

        private void SetupComponent()
        {
            facultyCB.ItemsSource = faculties;
            facultyCB.SelectedIndex = 0;

            majorCB.ItemsSource = majors;
            majorCB.SelectedIndex = 0;
        }

        public void Fetch()
        {
            var list = GetByDate();
            string faculty = (string)facultyCB.SelectedItem;
            string major = (string)majorCB.SelectedItem;

            IEnumerable<StudyAreaDTO> l = list.Where(item =>
            {
                var account = accounts[item.MSSV];

                if (faculty == "All" && major == "All") return true; 
                else if (faculty == "All") return account.Major == major;
                else if (major == "All") return account.Faculty == faculty;

                else return account.Faculty == faculty && account.Major == major;
            });

            var dataPoints = GetDataPoints(l);
            studyAreaModel.Model = StatisticsUtility.CreateDateLineChart("Number of students entering the study area", 
                                                                         "Number of students", 
                                                                         dataPoints,
                                                                         "Number of students",
                                                                         "Date", 
                                                                         "dd/MM/yyyy",
                                                                         startPicker.SelectedDate,
                                                                         endPicker.SelectedDate);
        }

        private List<DataPoint> GetDataPoints(IEnumerable<StudyAreaDTO> list)
        {
            Dictionary<DateTime, double> data = [];
            list = list.OrderBy(item => item.CheckinDate);

            foreach (var item in list)
            {
                if (!data.TryGetValue(item.CheckinDate.Date, out double value))
                {
                    data.Add(item.CheckinDate.Date, 1);
                }
                else data[item.CheckinDate.Date] = value + 1;
            }

            return [.. data.Select(pr => {
                double date = DateTimeAxis.ToDouble(pr.Key);
                double count = pr.Value;

                return new DataPoint(date, count);
            })];
        }

        private List<StudyAreaDTO> GetByDate()
        {
            bool hasStart = startPicker.SelectedDate.HasValue;
            bool hasEnd = endPicker.SelectedDate.HasValue;

            bool all = !hasStart && !hasEnd;
            bool sToE = hasStart && hasEnd;
            bool fromStart = hasStart && !hasEnd;
            bool toEnd = !hasStart && hasEnd;

            if (all) return studyAreaBUS.GetAll();
            if (sToE) return studyAreaBUS.GetAllByDate(startPicker.SelectedDate!.Value, endPicker.SelectedDate!.Value);
            if (fromStart) return studyAreaBUS.GetAllByDate(startPicker.SelectedDate!.Value, true);
            if (toEnd) return studyAreaBUS.GetAllByDate(endPicker.SelectedDate!.Value, false);

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
