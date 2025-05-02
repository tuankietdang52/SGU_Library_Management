using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SGULibraryManagement.GUI.ViewModels
{
    public class BDHistoryViewModel
    {
        public required DeviceDTO Device { get; set; }
        public required DateTime DateCreate { get; set; }
        public required DateTime DateBorrow { get; set; }
        public required DateTime DateReturn { get; set; }
        public required bool IsReturn { get; set; }
        public required Brush BgColor { get; set; }
    }
}
