using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.GUI.ViewModels
{
    public class AccountViewModel
    {
        public required AccountDTO Account { get; set; }
        public required RoleDTO Role { get; set; }
    }
}
