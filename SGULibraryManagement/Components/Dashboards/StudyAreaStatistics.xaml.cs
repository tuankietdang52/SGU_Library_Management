using SGULibraryManagement.BUS;
using SGULibraryManagement.DTO;
using System.Windows.Controls;

namespace SGULibraryManagement.Components.Dashboards
{
    public partial class StudyAreaStatistics : UserControl
    {
        private readonly StudyAreaBUS studyAreaBUS = new();
        private readonly AccountBUS accountBUS = new();

        private Dictionary<long, AccountDTO> accounts = new();

        public StudyAreaStatistics()
        {
            InitializeComponent();
            SetupComponent();
        }

        private void SetupComponent()
        {
            startPicker.SelectedDate = DateTime.Now;

            typeCB.ItemsSource = new List<string>()
            {
                "Faculty",
                "Major"
            };

            typeCB.SelectedIndex = 0;
        }

        public void Fetch()
        {
            DateTime start = startPicker.SelectedDate!.Value;
            DateTime end = endPicker.SelectedDate!.Value;

            string type = typeCB.Text;
            //var list = studyAreaBUS.GetAllByDate(start, end)
            //                       .Where(item => item.);

        }

        private void OnStartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnDateChanged(sender, e);
            endPicker.DisplayDateStart = startPicker.SelectedDate;
        }

        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startPicker.SelectedDate ??= DateTime.Now;
            endPicker.SelectedDate ??= startPicker.SelectedDate;

            if (endPicker.SelectedDate.Value < startPicker.SelectedDate.Value)
                endPicker.SelectedDate = startPicker.SelectedDate;
        }
    }
}
