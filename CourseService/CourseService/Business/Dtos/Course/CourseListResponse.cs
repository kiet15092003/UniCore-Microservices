namespace CourseService.Business.Dtos.Course
{
    public class CourseListResponse
    {
        public List<CourseReadDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
