using SGULibraryManagement.Components.Buttons;
using SGULibraryManagement.GUI;
using SGULibraryManagement.GUI.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace SGULibraryManagement.Components.Dashboards
{
    public partial class DashboardQuantityStatistics : UserControl
    {

        public static readonly DependencyProperty BookingQuantityProperty =
           DependencyProperty.Register(nameof(BookingQuantity),
                                       typeof(int),
                                       typeof(DashboardQuantityStatistics),
                                       new PropertyMetadata(0));

        public static readonly DependencyProperty ReservationQuantityProperty =
          DependencyProperty.Register(nameof(ReservationQuantity),
                                      typeof(int),
                                      typeof(DashboardQuantityStatistics),
                                      new PropertyMetadata(0));

        public static readonly DependencyProperty UserQuantityProperty =
          DependencyProperty.Register(nameof(UserQuantity),
                                      typeof(int),
                                      typeof(DashboardQuantityStatistics),
                                      new PropertyMetadata(0));

        public static readonly DependencyProperty DeviceQuantityProperty =
          DependencyProperty.Register(nameof(DeviceQuantity),
                                      typeof(int),
                                      typeof(DashboardQuantityStatistics),
                                      new PropertyMetadata(0));


        public int BookingQuantity
        {
            get => (int)GetValue(BookingQuantityProperty);
            set => SetValue(BookingQuantityProperty, value);
        }

        public int ReservationQuantity
        {
            get => (int)GetValue(ReservationQuantityProperty);
            set => SetValue(ReservationQuantityProperty, value);
        }

        public int UserQuantity
        {
            get => (int)GetValue(UserQuantityProperty);
            set => SetValue(UserQuantityProperty, value);
        }

        public int DeviceQuantity
        {
            get => (int)GetValue(DeviceQuantityProperty);
            set => SetValue(DeviceQuantityProperty, value);
        }

        private void OnBookingClick(object sender, RoutedEventArgs e)
        {
            MainView.Instance.Navigate<CirculationView>();
        }

        private void OnReservationClick(object sender, RoutedEventArgs e)
        {
            MainView.Instance.Navigate<ReservationView>();
        }

        private void OnUserClick(object sender, RoutedEventArgs e)
        {
            MainView.Instance.Navigate<UsersView>();
        }

        private void OnDeviceClick(object sender, RoutedEventArgs e)
        {
            MainView.Instance.Navigate<EquipmentsView>();
        }

        public DashboardQuantityStatistics()
        {
            InitializeComponent();
        }
    }
}
