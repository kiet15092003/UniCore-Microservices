using EnrollmentService.Business.Dtos.Exam;
using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.DataAccess.Repositories
{
    public interface IExamRepository
    {
        Task<Exam?> GetExamByIdAsync(Guid id);
        Task<List<Exam>> GetExamsByAcademicClassIdAsync(Guid academicClassId);
        Task<Exam> CreateExamAsync(Exam exam);
        Task<Exam> UpdateExamAsync(Exam exam);        
        Task<bool> DeleteExamAsync(Guid id);
        Task<List<Exam>> GetAllExamsAsync();
        Task<PaginationResult<Exam>> GetAllExamsPaginationAsync(Pagination pagination, ExamListFilterParams filterParams, Order? order);
        Task<List<EnrollmentExam>> CreateEnrollmentExamsAsync(List<EnrollmentExam> enrollmentExams);
        Task<List<Exam>> GetExamsByEnrollmentIdsAsync(List<Guid> enrollmentIds);
    }
}
