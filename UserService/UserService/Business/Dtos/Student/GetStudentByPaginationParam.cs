using UserService.Utils.Filter;
using UserService.Utils.Pagination;

namespace UserService.Business.Dtos.Student
{
    public class GetStudentByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public StudentListFilterParams Filter { get; set; } = new StudentListFilterParams();
        public Order Order { get; set; } = new Order();

    }
}
