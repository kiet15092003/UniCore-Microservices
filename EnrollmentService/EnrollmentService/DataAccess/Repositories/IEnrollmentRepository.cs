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
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
        Task<List<Enrollment>> CreateMultipleEnrollmentsAsync(List<Enrollment> enrollments);
        Task<bool> ExistsAsync(Guid studentId, Guid academicClassId);
        Task<int> GetEnrollmentCountByAcademicClassIdAsync(Guid academicClassId);
        Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId);
    }
}
