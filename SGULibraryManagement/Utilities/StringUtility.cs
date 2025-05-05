using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public static class StringUtility
    {
        public static bool IsNullOrEmpty(this IEnumerable<string> list)
        {
            foreach (string item in list)
            {
                if (string.IsNullOrEmpty(item)) return true;
            }

            return false;
        }

        public static bool IsNullOrWhiteSpace(this IEnumerable<string> list)
        {
            foreach (string item in list)
            {
                if (string.IsNullOrWhiteSpace(item)) return true;
            }

            return false;
        }
    }
}
