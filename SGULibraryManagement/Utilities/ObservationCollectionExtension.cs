using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public static class ObservationCollectionExtension
    {
        public static void ResetTo<T>(this ObservableCollection<T> collections, IEnumerable<T> enumerable)
        {
            collections.Clear();
            foreach (var item in enumerable)
            {
                collections.Add(item);
            }
        }
    }
}
