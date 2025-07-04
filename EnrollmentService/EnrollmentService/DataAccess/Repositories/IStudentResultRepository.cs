using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.DataAccess.Repositories
{
    public interface IStudentResultRepository
    {
        Task<StudentResult?> GetStudentResultByIdAsync(Guid id);
        Task<PaginationResult<StudentResult>> GetAllStudentResultsPaginationAsync(
            Pagination pagination,
            StudentResultListFilterParams filterParams,
            Order? order);
        Task<StudentResult> UpdateStudentResultAsync(StudentResult studentResult);
        Task<bool> DeleteStudentResultAsync(Guid id);
        Task<List<StudentResult>> GetStudentResultsByEnrollmentIdAsync(Guid enrollmentId);
        Task<List<StudentResult>> GetStudentResultsByEnrollmentIdsAsync(List<Guid> enrollmentIds);
        Task<List<StudentResult>> GetStudentResultsByClassIdAsync(Guid classId);
        Task<int> BulkUpdateScoresAsync(List<StudentResult> studentResults);
        Task<List<StudentResult>> GetStudentResultsByClassIdWithEnrollmentAsync(Guid classId);
    }
} 