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
    public class EquipmentItem : ContentControl
    {
        public static readonly DependencyProperty ModelProperty =
          DependencyProperty.Register(nameof(Model),
                                      typeof(DeviceDTO),
                                      typeof(EquipmentItem),
                                      new PropertyMetadata(null));

        public static readonly DependencyProperty AvailableColorProperty =
          DependencyProperty.Register(nameof(AvailableColor),
                                      typeof(SolidColorBrush),
                                      typeof(EquipmentItem),
                                      new PropertyMetadata(null));

        public DeviceDTO Model
        {
            get => (DeviceDTO)GetValue(ModelProperty);
            set {
                SetValue(ModelProperty, value);
                AvailableColor = value.IsAvailable ? Brushes.Green : Brushes.Red;
            }
        }

        public SolidColorBrush AvailableColor
        {
            get => (SolidColorBrush)GetValue(AvailableColorProperty);
            set => SetValue(AvailableColorProperty, value);
        }

        static EquipmentItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EquipmentItem), new FrameworkPropertyMetadata(typeof(EquipmentItem)));
        }
    }
}
