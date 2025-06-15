using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.DataAccess.Repositories
{    public interface IAcademicClassRepository
    {
        Task<AcademicClass> CreateAcademicClassAsync(AcademicClass academicClass);
        Task<AcademicClass?> GetAcademicClassByIdAsync(Guid id);        
        Task<List<AcademicClass>> GetAcademicClassesByCourseIdAsync(Guid courseId);
        Task<List<AcademicClass>> GetAcademicClassesBySemesterIdAsync(Guid semesterId);
        Task<List<AcademicClass>> GetAcademicClassesBySemesterAndCourseIdAsync(Guid semesterId, Guid courseId);
        Task<List<AcademicClass>> GetAcademicClassesForMajorAsync(Guid majorId);
        Task<List<AcademicClass>> GetAcademicClassesForMajorAndBatchAsync(Guid majorId, Guid batchId);
        Task<PaginationResult<AcademicClass>> GetAllAcademicClassesPaginationAsync(
            Pagination pagination,
            AcademicClassFilterParams? filterParams,
            Order? order);
        Task<AcademicClass> UpdateAcademicClassAsync(AcademicClass academicClass);
        Task<List<AcademicClass>> GetAcademicClassesByIdsAsync(List<Guid> ids);
        Task<List<AcademicClass>> GetAcademicClassesBySemesterWithSchedulesAsync(Guid semesterId);        Task SaveChangesAsync();        IQueryable<AcademicClass> GetQuery();
        IQueryable<AcademicClass> GetQueryWithIncludes();
        IQueryable<AcademicClass> ApplyFiltersToQuery(IQueryable<AcademicClass> query, AcademicClassFilterParams? filterParams);
        Task<bool> DeleteAcademicClassAsync(Guid id);
    }
}
