using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;

namespace SGULibraryManagement.BUS
{
    public class StudyAreaBUS
    {
        private readonly DAO.StudyAreaDAO Dao = new();

        public List<StudyAreaDTO> GetAll()
        {
            return Dao.GetAll(true);
        }

        /// <summary>
        /// Get all by date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fromStart">if true will get from date, else will get to date</param>
        /// <returns></returns>
        public List<StudyAreaDTO> GetAllByDate(DateTime date, bool fromStart)
        {
            return Dao.GetAllByDate(date, fromStart);
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

        public bool DeleteMultipleByAccount(List<AccountDTO> accounts)
        {
            return Dao.DeleteMultipleByStudentCode([.. accounts.Select(a => a.Mssv)]);
        }
    }
}
