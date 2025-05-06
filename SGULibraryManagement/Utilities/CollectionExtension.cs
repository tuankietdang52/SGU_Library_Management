using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public static class CollectionExtension
    {
        public static void ResetTo<T>(this ObservableCollection<T> collections, IEnumerable<T> enumerable)
        {
            collections.Clear();
            foreach (var item in enumerable)
            {
                collections.Add(item);
            }
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}
