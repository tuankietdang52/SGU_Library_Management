namespace SGULibraryManagement.Utilities
{
    public class Paginate<TItem>
    {
        private readonly List<TItem> source;
        private readonly int pageSize;
        private int totalPages;

        public Paginate(List<TItem> source, int pageSize)
        {
            this.source = source;
            this.pageSize = pageSize;

            CountTotalPages();
        }

        private void CountTotalPages()
        {
            bool divisible = source.Count % pageSize == 0;
            int pages = source.Count / pageSize;

            if (!divisible) pages++;

            this.totalPages = pages;
        }

        public List<TItem> GetSource() => source;

        public int GetTotalPages() => totalPages;

        /// <summary>
        /// Get list of item at page, throw if index is out of pages
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<TItem> GetPageAt(int index)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, totalPages);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 1);

            int skipCount = pageSize * (index - 1);
            return [.. source.Skip(skipCount).Take(pageSize)];
        }
    }
}
