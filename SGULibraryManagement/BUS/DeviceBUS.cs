using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.BUS
{
    public class DeviceBUS
    {
        private readonly DeviceDAO DAO = new();
        private readonly ReservationDAO reservationDAO = new();
        private readonly BorrowDevicesDAO borrowDeviceDAO = new();

        private List<DeviceDTO> devices = [];
        public Dictionary<long, int> DeviceBorrowQuantity;

        public DeviceBUS()
        {
            DeviceBorrowQuantity = DAO.GetAllWithBorrowQuantity().ToDictionary(pair => pair.First.Id, pair => pair.Last);
        }


        public bool CreateListDevice(List<DeviceDTO> listDevice)
        {
            return DAO.CreateListDevice(listDevice);
        }

        public List<DeviceDTO> GetAll()
        {
            return devices = DAO.GetAll(true);
        }

        public DeviceDTO FindById(long id)
        {
            return DAO.FindById(id);
        } 

        public DeviceDTO Create(DeviceDTO request)
        {
            return DAO.Create(request);
        }

        public bool Update(long id, DeviceDTO request)
        {
            return DAO.Update(id, request);
        }

        public bool Delete(DeviceDTO device)
        {
            var borrows = borrowDeviceDAO.FindByDeviceId(device.Id);
            var reservations = reservationDAO.FindByDeviceId(device.Id);

            if (borrows.Count > 0 || reservations.Count > 0)
            {
                return false;
            }

            return DAO.Delete(device.Id);
        }

        public List<DeviceDTO> FilterByQuery(string query, IEnumerable<DeviceDTO>? collection = null)
        {
            var list = collection ?? devices;
            return [.. list.Where(device => device.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase))];
        }

        public List<DeviceDTO> FilterByStatus(bool isAvailable, IEnumerable<DeviceDTO>? collection = null)
        {
            var list = collection ?? devices;
            return [.. list.Where(device => device.IsAvailable == isAvailable)];
        }

        public List<DeviceDTO> SortBy(DeviceSort sort, IEnumerable<DeviceDTO> list)
        {
            return sort switch
            {
                DeviceSort.IdAscending => [.. list.OrderBy(device => device.Id)],
                DeviceSort.IdDescending => [.. list.OrderByDescending(device => device.Id)],
                DeviceSort.NameAscending => [.. list.OrderBy(device => device.Name)],
                DeviceSort.NameDescending => [.. list.OrderByDescending(device => device.Name)],
                DeviceSort.QuantityAscending => [.. list.OrderBy(device => device.Quantity)],
                DeviceSort.QuantityDescending => [.. list.OrderByDescending(device => device.Quantity)],
                _ => []
            };
        }
    }

    public enum DeviceSort
    {
        IdAscending = 0,
        IdDescending = 1,
        NameAscending = 2,
        NameDescending = 3,
        QuantityAscending = 4,
        QuantityDescending = 5
    }
}