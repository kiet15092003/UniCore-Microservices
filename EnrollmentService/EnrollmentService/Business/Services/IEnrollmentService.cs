using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Services
{
    public interface IEnrollmentService
    {
        Task<EnrollmentReadDto?> GetEnrollmentByIdAsync(Guid id);
        Task<EnrollmentListResponse> GetEnrollmentsByPagination(Pagination pagination, EnrollmentListFilterParams filterParams, Order? order);
    }
}
