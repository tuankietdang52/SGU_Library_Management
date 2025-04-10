using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DTO
{
    public class UserDTO
    {
        public long Id { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Nganh { get; set; } = string.Empty;
        public string Khoa { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
