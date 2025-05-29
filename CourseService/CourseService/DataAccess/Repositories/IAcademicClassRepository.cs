using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.DataAccess.Repositories
{
    public interface IAcademicClassRepository
    {
        Task<AcademicClass> CreateAcademicClassAsync(AcademicClass academicClass);
        Task<AcademicClass?> GetAcademicClassByIdAsync(Guid id);
        Task<List<AcademicClass>> GetAcademicClassesByCourseIdAsync(Guid courseId);
        Task<List<AcademicClass>> GetAcademicClassesBySemesterIdAsync(Guid semesterId);
        Task<PaginationResult<AcademicClass>> GetAllAcademicClassesPaginationAsync(
            Pagination pagination,
            AcademicClassFilterParams? filterParams,
            Order? order);
        Task<AcademicClass> UpdateAcademicClassAsync(AcademicClass academicClass);
    }
}
