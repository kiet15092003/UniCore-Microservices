using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Services
{
    public interface IEnrollmentService
    {
        Task<EnrollmentReadDto?> GetEnrollmentByIdAsync(Guid id);
        Task<EnrollmentListResponse> GetEnrollmentsByPagination(Pagination pagination, EnrollmentListFilterParams filterParams, Order? order);
        Task<List<EnrollmentReadDto>> CreateMultipleEnrollmentsAsync(MultipleEnrollmentCreateDto createDto);
        Task<int> GetEnrollmentCountByAcademicClassIdAsync(Guid academicClassId);
        Task<bool> CheckEnrollmentExistsAsync(Guid studentId, Guid academicClassId);
        Task<CheckMultipleEnrollmentsResponse> CheckMultipleEnrollmentsExistAsync(CheckMultipleEnrollmentsRequest request);
        Task<List<EnrollmentReadDto>> GetEnrollmentsByStudentIdAsync(Guid studentId);
    }
}
