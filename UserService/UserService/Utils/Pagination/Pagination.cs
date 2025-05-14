namespace UserService.Utils.Pagination
{
    public class Pagination
    {
        const int DEFAULT_PERPAGE = 10;
        const int FIRST_PAGE = 1;
        public int PageNumber { get; set; } = FIRST_PAGE;
        public int ItemsPerpage { get; set; } = DEFAULT_PERPAGE;
    }
}
