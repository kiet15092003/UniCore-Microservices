using EnrollmentService.DTOs.StudentResult;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Services
{
    public interface IStudentResultService
    {
        Task<StudentResultDto?> GetStudentResultByIdAsync(Guid id);
        Task<PaginationResult<StudentResultDto>> GetAllStudentResultsPaginationAsync(
            Pagination pagination,
            StudentResultListFilterParams filterParams,
            Order? order);
        Task<StudentResultDto> UpdateStudentResultAsync(Guid id, UpdateStudentResultDto updateDto);
        Task<bool> DeleteStudentResultAsync(Guid id);
        Task<List<StudentResultDto>> GetStudentResultsByEnrollmentIdAsync(Guid enrollmentId);
        Task<List<StudentResultDto>> GetStudentResultsByEnrollmentIdsAsync(List<Guid> enrollmentIds);
    }
} 