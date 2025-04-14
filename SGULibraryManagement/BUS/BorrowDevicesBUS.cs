using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.BUS
{
    public class BorrowDevicesBUS
    {
        private readonly BorrowDevicesDAO dao = new();
        public List<BorrowDevicesDTO> BorrowDevices_List { get; private set; } = [];

        public List<BorrowDevicesDTO> GetAll()
        {
            return dao.GetAll(true);
        }

        public List<BorrowDevicesDTO> FindByDevice(DeviceDTO device)
        {
            return dao.FindByDeviceId(device.Id);
        }
    }
}
