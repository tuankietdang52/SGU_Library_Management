using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DTO
{
    public class StudyAreaDTO
    {
        public long Id { get; set; }
        public long MSSV { get; set; }
        public DateTime CheckinDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
