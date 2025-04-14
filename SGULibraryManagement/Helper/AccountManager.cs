using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Helper
{
    public static class AccountManager
    {
        public static AccountDTO? CurrentUser { get; set; }
    }
}
