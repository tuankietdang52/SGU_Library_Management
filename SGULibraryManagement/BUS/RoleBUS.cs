using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.BUS
{
    public class RoleBUS
    {
        private RoleDAO RoleDAO = new();
        public List<RoleDTO> Roles { get; private set; } = [];
        public List<RoleDTO> GetAll()
        {
            return Roles = RoleDAO.GetAll(true);
        }

    }
}
