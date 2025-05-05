using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;

namespace SGULibraryManagement.BUS
{
    public class StudyAreaBUS
    {
        private readonly StudyAreaDAO Dao = new();

        public List<StudyAreaDTO> GetAll()
        {
            return Dao.GetAll(true);
        }

        public List<StudyAreaDTO> GetAllByDate(DateTime date)
        {
            return Dao.GetAllByDate(date);
        }

        public List<StudyAreaDTO> GetAllByDate(DateTime start, DateTime end)
        {
            return Dao.GetAllByDate(start, end);
        }

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
