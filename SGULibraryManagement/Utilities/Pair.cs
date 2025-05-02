using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public class Pair<TFirst, TLast>
    {
        public required TFirst First { get; set; }
        public required TLast Last { get; set; }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
        public Pair(TFirst first, TLast last)
        {
            First = first;
            Last = last;
        }
    }
}
