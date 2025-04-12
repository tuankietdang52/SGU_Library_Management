using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
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
        private List<DeviceDTO> devices = [];

        public List<DeviceDTO> GetAll()
        {
            return devices = DAO.GetAll(true);
        }

        public DeviceDTO Create(DeviceDTO request)
        {
            return DAO.Create(request);
        }

        public bool Update(long id, DeviceDTO request)
        {
            return DAO.Update(id, request);
        }

        public bool Delete(long id)
        {
            return DAO.Delete(id);
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
        NameAscending = 0,
        NameDescending = 1,
        QuantityAscending = 2,
        QuantityDescending = 3
    }
}