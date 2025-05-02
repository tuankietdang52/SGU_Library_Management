using SGULibraryManagement.Components.Dropdown;
using SGULibraryManagement.DTO;
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

namespace SGULibraryManagement.Components.Equipments
{
    public delegate void OnEquipmentActionHandler(object sender, DeviceDTO model);

    public class EquipmentItem : ContentControl
    {
        public static readonly DependencyProperty ModelProperty =
          DependencyProperty.Register(nameof(Model),
                                      typeof(DeviceDTO),
                                      typeof(EquipmentItem),
                                      new PropertyMetadata(null));

        public static readonly DependencyProperty RemainQuantityProperty =
         DependencyProperty.Register(nameof(RemainQuantity),
                                     typeof(int),
                                     typeof(EquipmentItem),
                                     new PropertyMetadata(0));

        public static readonly DependencyProperty AvailableColorProperty =
          DependencyProperty.Register(nameof(AvailableColor),
                                      typeof(SolidColorBrush),
                                      typeof(EquipmentItem),
                                      new PropertyMetadata(null));

        public event OnEquipmentActionHandler? OnView;
        public event OnEquipmentActionHandler? OnEdit;
        public event OnEquipmentActionHandler? OnDelete;

        public DeviceDTO Model
        {
            get => (DeviceDTO)GetValue(ModelProperty);
            set {
                SetValue(ModelProperty, value);
                AvailableColor = value.IsAvailable ? Brushes.Green : Brushes.Red;
            }
        }

        public int RemainQuantity
        {
            get => (int)GetValue(RemainQuantityProperty);
            set => SetValue(RemainQuantityProperty, value);
        }

        public SolidColorBrush AvailableColor
        {
            get => (SolidColorBrush)GetValue(AvailableColorProperty);
            set => SetValue(AvailableColorProperty, value);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Template.FindName("view", this) is not DropdownMenuItem viewMenuItem) return;
            if (Template.FindName("edit", this) is not DropdownMenuItem editMenuItem) return;
            if (Template.FindName("delete", this) is not DropdownMenuItem deleteMenuItem) return;

            viewMenuItem.Click += (sender, e) => OnView?.Invoke(this, Model);
            editMenuItem.Click += (sender, e) => OnEdit?.Invoke(this, Model);
            deleteMenuItem.Click += (sender, e) => OnDelete?.Invoke(this, Model);
        }

        public EquipmentItem()
        {
            this.Loaded += OnLoaded;
        }

        static EquipmentItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EquipmentItem), new FrameworkPropertyMetadata(typeof(EquipmentItem)));
        }
    }
}