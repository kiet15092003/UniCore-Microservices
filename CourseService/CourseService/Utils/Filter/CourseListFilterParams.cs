namespace CourseService.Utils.Filter
{
    public class CourseListFilterParams
    {
        public string SearchQuery { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}
