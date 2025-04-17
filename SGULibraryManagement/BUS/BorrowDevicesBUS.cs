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

        public BorrowDevicesDTO FindById(long id)
        {
            return dao.FindById(id);
        }

        public List<BorrowDevicesDTO> FindByDevice(DeviceDTO device)
        {
            return dao.FindByDeviceId(device.Id);
        }

        public BorrowDevicesDTO Create(BorrowDevicesDTO request)
        {
            return dao.Create(request);
        }

        public bool Update(long id, BorrowDevicesDTO request)
        {
            return dao.Update(id, request);
        }

        public bool Delete(long id)
        {
            return dao.Delete(id);
        }
    }
}
