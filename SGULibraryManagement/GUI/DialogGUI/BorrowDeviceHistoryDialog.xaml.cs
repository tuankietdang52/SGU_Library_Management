using Google.Protobuf.WellKnownTypes;
using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace SGULibraryManagement.GUI.DialogGUI
{
    public partial class BorrowDeviceHistoryDialog : UserControl, IDialog
    {
        public ContentPresenter? PopupHost { get; set; }
        public event OnCloseDialogHandler? OnCloseDialog;

        private readonly AccountDTO account;
        private readonly BorrowDevicesBUS borrowDevicesBUS = new();
        private readonly DeviceBUS deviceBUS = new();

        public ObservableCollection<BDHistoryViewModel> HistoryItemSource { get; set; } = [];
        private IEnumerable<BDHistoryViewModel>? BorrowDevices;
        private readonly Dictionary<long, DeviceDTO> devices;

        private Action<string>? searchDebounce;

        public BorrowDeviceHistoryDialog(AccountDTO account)
        {
            InitializeComponent();
            this.account = account;

            DataContext = this;
            devices = deviceBUS.GetAll().ToDictionary(pr => pr.Id);
            SetupComponent();
        }

        private void SetupComponent()
        {
            Fetch();
            searchDebounce = ((Action<string>)(Searching)).Debounce(200);
        }

        private Brush GetStatusBackgroundColor(bool isReturn, DateTime dateReturn)
        {
            var app = App.Instance!;

            if (DateTime.Now < dateReturn) return Brushes.White;
            return isReturn ? Brushes.White : (SolidColorBrush)app.Resources["LockedBackground"];
        }

        private void Fetch()
        {
            var list = borrowDevicesBUS.FindByAccount(account);

            BorrowDevices = list.Select(item =>
            {
                var app = App.Instance!;
                var bgColor = GetStatusBackgroundColor(item.IsReturn, item.DateReturn);

                return new BDHistoryViewModel()
                {
                    Device = devices[item.DeviceId],
                    DateCreate = item.DateCreate,
                    DateBorrow = item.DateBorrow,
                    DateReturn = item.DateReturn,
                    IsReturn = item.IsReturn,
                    BgColor = bgColor!
                };
            });

            HistoryItemSource.ResetTo(BorrowDevices);
        }

        private void RenderTable(IEnumerable<BDHistoryViewModel>? collections = null)
        {
            var list = collections ?? BorrowDevices;
            if (list is null) return;

            App.Instance!.InvokeInMainThread(() => HistoryItemSource.ResetTo(list));
        }

        private void Searching(string query)
        {
            if (query == "") RenderTable();
            if (BorrowDevices is null) return;

            var list = BorrowDevices.Where(item => item.Device.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            RenderTable(list);
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;
            searchDebounce(searchField.Text);
        }
    }
}