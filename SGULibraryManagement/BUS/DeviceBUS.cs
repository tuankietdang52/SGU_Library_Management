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

        public List<DeviceDTO> GetAll()
        {
            return DAO.GetAll(true);
        }
    }
}