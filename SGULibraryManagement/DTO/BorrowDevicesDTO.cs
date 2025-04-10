using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DTO
{
    public class BorrowDevicesDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long DeviceId { get; set; }
        public DateTime CreateAt {  get; set; }
        public int Quantity { get; set; }
        public DateTime DateBorrow { get; set; }
        public DateTime DateReturn { get; set; }
        public Boolean IsDeleted { get; set; }
        public BorrowDevicesDTO() { }
    }
}
