using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class GetEnrollmentByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public EnrollmentListFilterParams Filter { get; set; } = new EnrollmentListFilterParams();
        public Order? Order { get; set; }
    }
}
