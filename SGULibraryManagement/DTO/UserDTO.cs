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
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => FirstName + LastName;
        public string? Phone { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsDeleted { get; set; }
    }
}
