namespace EnrollmentService.Utils.Pagination
{
    public class Pagination
    {
        private int _page = 1;
        private int _pageSize = 10;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : value;
        }
    }

    public class PaginationResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; } = 0;
        public int TotalPages { get; set; } = 1;
    }
}
