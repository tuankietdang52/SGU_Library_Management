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
    public partial class ViolationHistoryDialog : UserControl, IDialog
    {
        public ContentPresenter? PopupHost { get; set; }
        public event OnCloseDialogHandler? OnCloseDialog;

        private readonly AccountDTO account;
        private readonly AccountViolationBUS accountViolationBUS = new();
        private readonly ViolationBUS violationBUS = new();

        public ObservableCollection<ACHistoryViewModel> HistoryItemSource { get; set; } = [];
        private IEnumerable<ACHistoryViewModel>? AccountViolations;
        private readonly Dictionary<long, ViolationDTO> violations;

        private Action<string>? searchDebounce;

        public ViolationHistoryDialog(AccountDTO account)
        {
            InitializeComponent();
            this.account = account;

            DataContext = this;
            violations = violationBUS.GetAll().ToDictionary(pr => pr.Id);

            SetupComponent();
        }

        private void SetupComponent()
        {
            Fetch();
            searchDebounce = ((Action<string>)(Searching)).Debounce(200);
        }

        private void Fetch()
        {
            var list = accountViolationBUS.FindByAccount(account);
            AccountViolations = list.Select(item =>
            {
                var app = App.Instance!;
                var bgColor = item.BanExpired.Date < DateTime.Now.Date ? Brushes.White : app.Resources["ActiveBackground"] as SolidColorBrush;

                return new ACHistoryViewModel()
                {
                    Violation = violations[item.ViolationId],
                    DateCreate = item.DateCreate,
                    BanExpired = item.BanExpired,
                    Compensation = item.Compensation,
                    BgColor = bgColor!
                };
            });

            HistoryItemSource.ResetTo(AccountViolations);
        }

        private void RenderTable(IEnumerable<ACHistoryViewModel>? collections = null)
        {
            var list = collections ?? AccountViolations;
            if (list is null) return;

            App.Instance!.InvokeInMainThread(() => HistoryItemSource.ResetTo(list));
        }

        private void Searching(string query)
        {
            if (query == "") RenderTable();
            if (AccountViolations is null) return;

            var list = AccountViolations.Where(item => item.Violation.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            RenderTable(list);
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;
            searchDebounce(searchField.Text);
        }
    }
}
