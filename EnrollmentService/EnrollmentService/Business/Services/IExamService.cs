using EnrollmentService.Business.Dtos.Exam;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Services
{
    public interface IExamService
    {
        Task<ExamReadDto?> GetExamByIdAsync(Guid id);
        Task<List<ExamReadDto>> GetExamsByAcademicClassIdAsync(Guid academicClassId);
        Task<ExamListResponse> GetExamsByPagination(Pagination pagination, ExamListFilterParams filterParams, Order? order);
        Task<ExamReadDto> CreateExamAsync(ExamCreateDto createDto);        Task<ExamReadDto> UpdateExamAsync(Guid id, ExamCreateDto updateDto);
        Task<bool> DeleteExamAsync(Guid id);
        Task<List<ExamReadDto>> GetAllExamsAsync();
        Task<List<EnrollmentExamDto>> AddEnrollmentToExamAsync(AddEnrollmentToExamDto addEnrollmentDto);
        Task<List<ExamReadDto>> GetExamsByStudentIdAsync(Guid studentId);
    }
}
