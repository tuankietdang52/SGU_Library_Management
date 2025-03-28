using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DTO
{
    public class DeviceDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? ImageSource { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAvailable { get; set; }

        public DeviceDTO()
        {

        }
    }
}
