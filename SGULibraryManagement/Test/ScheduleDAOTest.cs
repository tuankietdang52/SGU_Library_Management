using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
namespace SGULibraryManagement.Test
{
    [TestClass]
    public class ScheduleDAOTest
    {
        [TestMethod]
        public void test()
        {
            BorrowDevicesDAO a = new BorrowDevicesDAO();
            List<BorrowDevicesDTO> b = a.GetAll(true);
            foreach (var item in b)
            {
                Console.WriteLine(item);
            }
        }
    }
}
