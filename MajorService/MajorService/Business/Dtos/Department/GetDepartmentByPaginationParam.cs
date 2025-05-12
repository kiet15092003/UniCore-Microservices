using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Dtos.Department
{
    public class GetDepartmentByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public DepartmentListFilterParams Filter { get; set; } = new DepartmentListFilterParams();
        public Order Order { get; set; } = new Order();
    }
}
