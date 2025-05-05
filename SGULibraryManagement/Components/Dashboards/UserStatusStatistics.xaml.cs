using SGULibraryManagement.BUS;
using SGULibraryManagement.Utilities;
using System.Windows.Controls;
using System.Windows.Media;

namespace SGULibraryManagement.Components.Dashboards
{
    public partial class UserStatusStatistics : UserControl
    {
        private readonly AccountBUS accountBUS = new();

        public UserStatusStatistics()
        {
            InitializeComponent();
        }

        public void Fetch()
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
    }
}
