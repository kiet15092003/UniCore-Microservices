using CourseService.Business.Dtos.Course;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Dtos.CoursesGroup
{
    public class CoursesGroupListResponse
    {
        public List<CoursesGroupReadDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}