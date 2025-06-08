using CourseService.Business.Dtos.Semester;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public interface ISemesterService
    {
        Task<SemesterReadDto> CreateSemesterAsync(SemesterCreateDto semesterCreateDto);
        Task<PaginationResult<SemesterReadDto>> GetByPaginationAsync(Pagination pagination, SemesterFilterParams filter, Order? order);
        Task<SemesterReadDto> UpdateSemesterAsync(Guid id, SemesterUpdateDto semesterUpdateDto);
        Task<SemesterReadDto> DeactivateSemesterAsync(Guid id);
        Task<SemesterReadDto> ActivateSemesterAsync(Guid id);
        Task<SemesterReadDto> GetSemesterByIdAsync(Guid id);
    }
}
