using SGULibraryManagement.BUS;
using SGULibraryManagement.Components.Equipments;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
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

namespace SGULibraryManagement.GUI
{
    public partial class EquipmentsView : UserControl
    {
        private readonly DeviceBUS BUS = new();
        private Paginate<DeviceDTO>? Paginate;

        public EquipmentsView()
        {
            InitializeComponent();
            Fetch();
        }

        private void Fetch()
        {
            Paginate = new(BUS.GetAll(), 6);
            var first = Paginate.GetPageAt(1);

            foreach ( var item in Paginate.GetSource() )
            {
                EquipmentItem equipmentItem = new()
                {
                    Model = item,
                    Margin = new Thickness(0, 0, 15, 15),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                };

                equipmentsContainer.Children.Add(equipmentItem);
            }
        }
    }
}
