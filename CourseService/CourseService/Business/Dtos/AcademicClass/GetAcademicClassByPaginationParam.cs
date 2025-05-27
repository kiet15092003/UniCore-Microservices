using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class GetAcademicClassByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public AcademicClassFilterParams Filter { get; set; } = new AcademicClassFilterParams();
        public Order Order { get; set; } = new Order();
    }
}
