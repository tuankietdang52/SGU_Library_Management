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
        public List<DeviceDTO> Devices { get; private set; } = [];

        public List<DeviceDTO> GetAll()
        {
            return Devices = DAO.GetAll(true);
        }

        public List<DeviceDTO> FilterByQuery(string query, List<DeviceDTO>? list = null)
        {
            var devices = list ?? Devices;
            return [.. devices.Where(device => device.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase))];
        }

        public List<DeviceDTO> FilterByStatus(bool isAvailable, List<DeviceDTO>? list = null)
        {
            var devices = list ?? Devices;
            return [.. devices.Where(device => device.IsAvailable == isAvailable)];
        }

        public List<DeviceDTO> SortBy(DeviceSort sort, List<DeviceDTO> list)
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