using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.BUS
{
    public class StudyAreaBUS
    {
        private readonly StudyAreaDAO Dao = new();


        public StudyAreaDTO Create(StudyAreaDTO request)
        {
            return Dao.Create(request);
        }

        public List<StudyAreaDTO> FindByMSSV(long mssv)
        {
            return Dao.FindByMSSV(mssv);
        }


    }
}
