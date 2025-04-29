using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SGULibraryManagement.GUI.ViewModels
{
    public class AccountViewModel
    {
        public required AccountDTO Account { get; set; }
        public required RoleDTO Role { get; set; }
        public required SolidColorBrush RoleBackgroundColor { get; set; }
        public required SolidColorBrush BgColor { get; set; }
    }
}
