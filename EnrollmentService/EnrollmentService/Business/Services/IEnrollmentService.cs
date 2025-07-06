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
        Task<List<EnrollmentReadDto>> GetEnrollmentsByAcademicClassIdAsync(Guid academicClassId);
        Task<bool> CheckEnrollmentExistsAsync(Guid studentId, Guid academicClassId);
        Task<CheckMultipleEnrollmentsResponse> CheckMultipleEnrollmentsExistAsync(CheckMultipleEnrollmentsRequest request);
        Task<List<EnrollmentReadDto>> GetEnrollmentsByStudentIdAsync(Guid studentId, Guid? semesterId = null);        Task<bool> DeleteEnrollmentAsync(Guid id);
        Task<int> ApproveEnrollmentsByAcademicClassIdAsync(Guid classId);
        Task<int> RejectEnrollmentsByAcademicClassIdAsync(Guid classId);        Task<int> MoveEnrollmentsToNewClassAsync(List<Guid> enrollmentIds, Guid toClassId);        Task<int?> GetFirstEnrollmentStatusByAcademicClassIdAsync(Guid academicClassId);
        Task<CheckClassConflictResponse> CheckClassConflictAsync(CheckClassConflictRequest request);
        Task<int> BulkChangeEnrollmentStatusAsync(BulkStatusChangeDto bulkStatusChangeDto);
        Task<int> StartEnrollmentsByAcademicClassIdAsync(Guid classId);
        Task<EnrollmentReadDto?> GetEnrollmentByStudentIdAndClassIdAsync(Guid studentId, Guid classId);
    }
}
