namespace EnrollmentService.Utils.Pagination
{
    public class Pagination
    {
        const int DEFAULT_PERPAGE = 10;
        const int FIRST_PAGE = 1;
        public int PageNumber { get; set; } = FIRST_PAGE;
        public int ItemsPerpage { get; set; } = DEFAULT_PERPAGE;
    }

    public class PaginationResult<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
