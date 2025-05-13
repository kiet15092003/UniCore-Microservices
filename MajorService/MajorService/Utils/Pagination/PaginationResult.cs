namespace MajorService.Utils.Pagination
{
    public class PaginationResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
