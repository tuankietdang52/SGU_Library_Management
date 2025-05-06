using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public static class ReservationExtension
    {
        public static BorrowDevicesDTO ToBorrowDevice(this ReservationDTO reservation)
        {
            return new BorrowDevicesDTO()
            {
                DeviceId = reservation.DeviceId,
                UserId = reservation.UserId,
                Quantity = reservation.Quantity,
                DateCreate = reservation.DateCreate,
                DateBorrow = reservation.DateBorrow,
                DateReturnExpected = reservation.DateReturn,
                IsDeleted = false,
                IsReturn = false
            };
        }
    }
}
