using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using CourseService.Business.Dtos.Semester;

namespace CourseService.DataAccess.Repositories
{
    public interface ISemesterRepository
    {
        Task<Semester> CreateSemesterAsync(Semester semester);
        Task<PaginationResult<Semester>> GetAllSemestersPaginationAsync(
            Pagination pagination,
            SemesterFilterParams filterParams,
            Order? order);
        Task<Semester?> GetSemesterByIdAsync(Guid id);
        Task<Semester> UpdateSemesterAsync(Semester semester);
        Task<bool> SemesterExistsAsync(int semesterNumber, int year, Guid? excludeId = null);
    }
}
