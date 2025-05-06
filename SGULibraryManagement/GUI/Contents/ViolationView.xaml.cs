using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Dialogs;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.DialogGUI;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class ViolationView : UserControl, IContent
    {
        private readonly ViolationBUS BUS = new();
        private readonly AccountViolationBUS accountViolationBUS = new();
        private readonly ViolationBUS violationBUS = new();

        public ObservableCollection<ViolationViewModel> Violations { get; set; } = [];
        private Action<string>? searchDebounce;


        public ViolationView()
        {
            InitializeComponent();
            Fetch();
            SetupComponent();
        }

        public void Fetch()
        {
            Violations.ResetTo(BUS.GetAllWithViolationCount());
        }

        private void SetupComponent()
        {
            DataContext = this;
            SetupSearch();
        }

        private void SetupSearch()
        {
            searchDebounce = ((Action<string>)(OnFilterByQuery)).Debounce(200);
        }

        private void OnFilterByQuery(string query)
        {
            var result = BUS.FilterByQuery(query);
            App.Instance!.InvokeInMainThread(() => Violations.ResetTo(result));
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            if (searchDebounce is null) return;
            searchDebounce(searchField.Text);
        }

        private void OnCreateClick(object sender, RoutedEventArgs e)
        {
            Dialog dialog = new("Create new rule", new ViolationDialog());
            dialog.ShowDialog();

            Fetch();
        }

        private void OnViewClick(object sender, object model)
        {
            if (model is not ViolationDTO violation) return;

            Dialog dialog = new($"Edit {violation.Name}", new ViolationDialog(EDialogType.View, violation));
            dialog.ShowDialog();

            Fetch();
        }

        private void OnEditClick(object sender, object model)
        {
            if (model is not ViolationDTO violation) return;

            Dialog dialog = new($"Edit {violation.Name}", new ViolationDialog(EDialogType.Edit, violation));
            dialog.ShowDialog();

            Fetch();
        }

        private async void AlertNoneSelected()
        {
            SimpleDialog dialog = new()
            {
                Content = $"Please select user to delete ?",
                Title = $"Fail",
                Width = 400,
                Height = 200
            };

            await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK);
        }

        private async void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            var list = violationTable.SelectedItems;

            if (list.Count == 0)
            {
                AlertNoneSelected();
                return;
            }

            if (list[0] is not ViolationViewModel) return;

            SimpleDialog dialog = new()
            {
                Content = $"Are you really want to delete selected item ?",
                Title = $"Delete",
                Width = 400,
                Height = 200
            };

            var result = await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.YesNo);
            List<ViolationDTO> violations = [.. list.Cast<ViolationViewModel>().Select(v => v.Violation)];

            if (result == SimpleDialogResult.Yes) Deleting(violations);
            else return;

            Fetch();
        }

        private async void Deleting(List<ViolationDTO> violations)
        {
            var list = accountViolationBUS.IsRulesViolatedByUser(violations);
            if (list.Count != 0)
            {
                var violation = violationBUS.FindById(list.First().ViolationId);
                SimpleDialog dialog = new()
                {
                    Content = $"Cannot delete '{violation.Name}' rule because there are users who violated this rule",
                    Title = $"Delete Failed",
                    Width = 400,
                    Height = 200
                };

                await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK);
                return;
            }

            if (!BUS.DeleteMultiple(violations))
            {
                SimpleDialog dialog = new()
                {
                    Content = $"Delete selected item failed",
                    Title = $"Delete Failed",
                    Width = 400,
                    Height = 200
                };

                await MainWindow.Instance!.ShowSimpleDialogAsync(dialog, SimpleDialogType.OK);
            }
        }
    }
}