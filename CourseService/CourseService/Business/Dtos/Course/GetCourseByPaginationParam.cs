using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Dtos.Course
{
    public class GetCourseByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public CourseListFilterParams Filter { get; set; } = new CourseListFilterParams();
        public Order Order { get; set; } = new Order();
    }
}
