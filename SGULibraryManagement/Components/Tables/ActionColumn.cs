using SGULibraryManagement.Components.Buttons;
using SGULibraryManagement.Components.SideMenu;
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
using Wpf.Ui.Controls;

namespace SGULibraryManagement.Components.Tables
{
    public delegate void OnActionClickHandler(object sender, object model);

    public class ActionColumn : Control
    {
        public event OnActionClickHandler? OnViewClick;
        public event OnActionClickHandler? OnEditClick;
        public event OnActionClickHandler? OnDeleteClick;

        public static readonly DependencyProperty ModelProperty =
          DependencyProperty.Register(nameof(Model),
                                      typeof(object),
                                      typeof(ActionColumn),
                                      new PropertyMetadata(null));


        public object Model
        {
            get => (object)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("viewAction") is RoundButton view)
            {
                view.Click += OnViewActionClick;
            }

            if (GetTemplateChild("editAction") is RoundButton edit)
            {
                edit.Click += OnEditActionClick;
            }

            if (GetTemplateChild("deleteAction") is RoundButton delete)
            {
                delete.Click += OnDeleteActionClick;
            }
        }

        private void OnViewActionClick(object sender, RoutedEventArgs e)
        {
            OnViewClick?.Invoke(this, Model);
        }

        private void OnEditActionClick(object sender, RoutedEventArgs e)
        {
            OnEditClick?.Invoke(this, Model);
        }

        private void OnDeleteActionClick(object sender, RoutedEventArgs e)
        {
            OnDeleteClick?.Invoke(this, Model);
        }

        static ActionColumn()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ActionColumn), new FrameworkPropertyMetadata(typeof(ActionColumn)));
        }
    }
}
