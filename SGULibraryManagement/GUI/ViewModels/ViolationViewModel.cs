using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.GUI.ViewModels
{
    public class ViolationViewModel
    {
        public required ViolationDTO Violation { get; set; }
        public required int ViolationCount { get; set; }
    }
}
