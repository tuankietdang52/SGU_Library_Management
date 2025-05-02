using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DTO
{
    public class ReservationDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long DeviceId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateBorrow { get; set; }
        public DateTime DateReturn { get; set; }
        public bool IsCheckedOut { get; set; }
        public bool IsDeleted { get; set; }
    }
}
