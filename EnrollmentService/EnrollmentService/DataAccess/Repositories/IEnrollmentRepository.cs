using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.DataAccess.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetEnrollmentByIdAsync(Guid id);
        Task<PaginationResult<Enrollment>> GetAllEnrollmentsPaginationAsync(
                Pagination pagination,
                EnrollmentListFilterParams enrollmentListFilterParams,
                Order? order);
    }
}
