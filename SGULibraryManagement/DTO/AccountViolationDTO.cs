using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DTO
{
    public class AccountViolationDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ViolationId { get; set; }
        public AccountViolationStatus Status { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime BanExpired { get; set; }
        public long Compensation { get; set; }
        public bool IsDeleted { get; set; }

        public bool? IsBanEternal { get; set; }
    }

    public enum AccountViolationStatus
    {
        Handled,
        BeingProcessed
    }
}
