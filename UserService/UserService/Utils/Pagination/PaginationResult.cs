namespace UserService.Utils.Pagination
{
    public class PaginationResult<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
