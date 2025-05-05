using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SGULibraryManagement.GUI.ViewModels
{
    /// <summary>
    /// Account Violation History view model
    /// </summary>
    public class ACHistoryViewModel
    {
        public required ViolationDTO Violation { get; set; }
        public required DateTime DateCreate { get; set; }
        public required DateTime BanExpired { get; set; }
        public required long Compensation { get; set; }
        public required SolidColorBrush BgColor { get; set; }
    }
}
